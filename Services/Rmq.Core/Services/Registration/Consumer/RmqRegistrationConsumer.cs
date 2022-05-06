using Com.GGIT.Common;
using Com.GGIT.Common.Util;
using Com.GGIT.LogLib;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Rmq.Core.Common;
using Rmq.Core.Model.Base;
using Rmq.Core.Model.Registration;
using Rmq.Core.Settings;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rmq.Core.Services.Registration.Consumer
{
    public class RmqRegistrationConsumer : IDisposable //wailiang 20200808 MDT-1581
    {
        private string _identity;
        private IConnection _connection;
        private readonly RabbitMQConfig settings;
        private TimeStampUtil _timeStampUtil;

        public RmqRegistrationConsumer(string identity, IConnection connection, RabbitMQConfig rmqSettings)
        {
            _identity = identity;
            _connection = connection;
            settings = rmqSettings;
            _timeStampUtil = new TimeStampUtil();
        }

        public void Run(CancellationToken workerCancellationToken)
        {
            try
            {
                for (int i = 0; i < settings.Channels; i++)
                {
                    Task channel = Task.Run(CreateChannel);

                    SingletonLogger.Info("Created child " + _identity + ":task@[" + channel.Id + "] channel.");
                }

                while (!workerCancellationToken.IsCancellationRequested)
                {
                    _timeStampUtil.InsertLastActivityLogTimestamp("RMQ-Registration-Consumer"); //voonkeong 20201125 MDT-1757
                    Task.Delay(settings.ThreadSleepTimeSec * 30000).Wait(workerCancellationToken);
                }
            }
            catch (OperationCanceledException ocex)
            {
                SingletonLogger.Error(ocex.Message);
            }
            catch (Exception ex)
            {
                SingletonLogger.Error(ex.ExceptionToString());
            }
            finally
            {
                Dispose();
            }
        }

        public void CreateChannel()
        {
            var channel = _connection.CreateModel();
            channel.BasicQos(0, 1, false);
            MessageReceiver messageReceiver = new MessageReceiver(channel, settings);
            channel.BasicConsume(settings.Queue, false, messageReceiver);
        }

        public void Dispose()
        {
            SingletonLogger.Info("Called Dispose() and closing RabbitMQ connection...");

            try
            {
                _connection.Close();
                if (_connection.CloseReason != null)
                    SingletonLogger.Info("Host server reply code = " + _connection.CloseReason.ReplyCode +
                        " , message = " + _connection.CloseReason.ReplyText);
            }
            catch (Exception ex)
            {
                SingletonLogger.Error(ex.ExceptionToString());
                SingletonLogger.Error(JsonConvert.SerializeObject(_connection.ShutdownReport));

                if (_connection.IsOpen || _connection.CloseReason == null) _connection.Close(); // try to close connection again
            }
            finally
            {
                _connection.Dispose(); // dispose connection and release memory
                _connection = null;
            }
        }

        public class MessageReceiver : DefaultBasicConsumer, IDisposable
        {
            #region Local variable
            private readonly IModel _channel;
            private readonly RabbitMQConfig _settings;
            private readonly UtilityHelper util;
            #endregion

            #region Ctor
            public MessageReceiver(IModel channel, RabbitMQConfig settings)
            {
                _channel = channel;
                _settings = settings;
                util = new UtilityHelper();
            }
            #endregion

            #region Dispose
            public void Dispose()
            {
                ((IDisposable)util).Dispose();
                _channel.Dispose();
            }
            #endregion

            #region Methods
            public byte[] EncodeMessage(string message)
            {
                return Encoding.UTF8.GetBytes(message);
            }

            public string DecodeMessage(byte[] message)
            {
                return Encoding.UTF8.GetString(message);
            }
            #endregion

            #region Validator
            public bool ValidationParam(ulong deliveryTag, IBasicProperties properties, byte[] body, RegistrationConsumerInput param,
            out RegistrationConsumerDto jsonMsg, ResultObject errorObject, UtilityHelper util,
            out int ParentId, IModel channel, out bool DefaultIntroGUID, out string _idToken)
            {
                jsonMsg = null;
                ParentId = -1;
                DefaultIntroGUID = false;
                _idToken = string.Empty;
                #region Param Validation
                try
                {
                    #region Json Input Validation
                    string _body = DecodeMessage(body);
                    SingletonLogger.Info("\nHeader : " + JsonConvert.SerializeObject(properties.Headers) + "\nMessage received : " + _body);
                    try
                    {
                        param = JsonConvert.DeserializeObject<RegistrationConsumerInput>(_body);
                        if (param == null)
                        {
                            errorObject.ReturnCode = 7005;
                            errorObject.ReturnMessage = "Empty payload received. Message body =>" + _body;
                            var callbackmessage = EncodeMessage(JsonConvert.SerializeObject(errorObject));
                            SingletonLogger.Info("\nMessage received : " + errorObject.ReturnMessage);
                            channel.BasicPublish(_settings.Exchange, "", null, callbackmessage);
                            channel.BasicReject(deliveryTag, false);
                            return false;
                        }
                    }
                    catch (Exception)
                    {
                        SingletonLogger.Error("Failed to deserialize json object!");
                        errorObject.ReturnCode = 7006;
                        errorObject.ReturnMessage = "Invalid json input. Message body =>" + _body;
                        var callbackmessage = EncodeMessage(JsonConvert.SerializeObject(errorObject));
                        channel.BasicPublish(_settings.Exchange, "", null, callbackmessage);
                        channel.BasicReject(deliveryTag, false);
                        return false;
                    }
                    #endregion

                    #region Validation Token & Timestamp
                    if (_settings.ValidateToken)
                    {
                        if (properties.Headers.ContainsKey("timestamp_in_ms") && !string.IsNullOrEmpty(param.id_token) && !string.IsNullOrEmpty(param.SecurityToken))
                        {
                            var timestampLong = Convert.ToInt64(properties.Headers["timestamp_in_ms"]);
                            _idToken = param.id_token;
                            jsonMsg = util.ValidateTokens(param.id_token, param.SecurityToken, timestampLong, errorObject, _settings);
                            if (!jsonMsg.AuthTokenValid)
                            {
                                var callbackobj = new RegistrationPublisherDto()
                                {
                                    GlobalGUID = jsonMsg.GlobalGUID,
                                    IntroducerGlobalGUID = jsonMsg.IntroducerGlobalGUID,
                                    IdToken = _idToken,
                                    ReturnCode = 7008,
                                    ReturnMessage = !string.IsNullOrEmpty(jsonMsg.ErrorMessage) ? "Invalid or expired token" + ". " + jsonMsg.ErrorMessage : "Invalid or expired token"
                                };
                                SingletonLogger.Error(JsonConvert.SerializeObject(callbackobj));
                                var callbackmessage = EncodeMessage(JsonConvert.SerializeObject(callbackobj));
                                channel.BasicPublish(_settings.Exchange, "", null, callbackmessage);
                                channel.BasicReject(deliveryTag, false);
                                return false;
                            }
                        }
                        else
                        {
                            errorObject.ReturnCode = 7009;
                            errorObject.ReturnMessage = "Missing timestamp/tokens";
                        }
                    }
                    #endregion

                    if (errorObject.ReturnCode == 0)
                    {
                        if (util.IsGlobalGUIDExists(jsonMsg.GlobalGUID))
                        {
                            errorObject.ReturnCode = 2909;
                            errorObject.ReturnMessage = "User has been registered.";
                        }
                        else if (!util.IsIntroducerExists(jsonMsg.IntroducerGlobalGUID, out ParentId, out DefaultIntroGUID))
                        {
                            errorObject.ReturnCode = 2925;
                            errorObject.ReturnMessage = "Introducer not exists.";
                        }
                        else if (!jsonMsg.UserRole.EqualsIgnoreCase("c") && !jsonMsg.UserRole.EqualsIgnoreCase("m"))
                        {
                            errorObject.ReturnCode = 2930;
                            errorObject.ReturnMessage = "Invalid User Role.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorObject.ReturnCode = ex.HResult;
                    errorObject.ReturnMessage = "Failed to register.";
                    SingletonLogger.Error(ex.ExceptionToString());
                }

                if (errorObject.ReturnCode != 0)
                {
                    if (errorObject.ReturnCode == 2909)
                    {
                        var rgInfo = util.GetUserRegisteredInfo(jsonMsg.GlobalGUID);
                        if (rgInfo.IsNotNull() && rgInfo.IntroducerGlobalGUID.EqualsIgnoreCase(jsonMsg.IntroducerGlobalGUID))
                        {
                            var dto = new RegistrationPublisherDto()
                            {
                                GlobalGUID = rgInfo.GlobalGUID,
                                IntroducerGlobalGUID = rgInfo.IntroducerGlobalGUID,
                                IdToken = _idToken,
                                Username = rgInfo.Username
                            };
                            _channel.BasicPublish(_settings.Exchange, "", null, EncodeMessage(JsonConvert.SerializeObject(dto)));
                            _channel.BasicAck(deliveryTag, false);
                            SingletonLogger.Info(JsonConvert.SerializeObject(dto) + " , this account was registered and return a valid message to ace. ");
                            return false; // skip registration when this account has registered.
                        }
                    }
                    #region Output Msg
                    var callbackobj = new RegistrationPublisherDto()
                    {
                        GlobalGUID = jsonMsg != null ? jsonMsg.GlobalGUID : string.Empty,
                        IntroducerGlobalGUID = jsonMsg != null ? jsonMsg.IntroducerGlobalGUID : string.Empty,
                        IdToken = _idToken,
                        ReturnCode = errorObject.ReturnCode,
                        ReturnMessage = errorObject.ReturnMessage
                    };
                    SingletonLogger.Error(JsonConvert.SerializeObject(callbackobj));
                    #endregion
                    var callbackmessage = EncodeMessage(JsonConvert.SerializeObject(callbackobj));
                    channel.BasicPublish(_settings.Exchange, "", null, callbackmessage);
                    channel.BasicReject(deliveryTag, false);
                    return false;
                }
                #endregion
                return true;
            }
            #endregion

            #region Processor
            public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
            {
                #region Rabbit work
                byte[] callbackmsg;
                var errorObject = new ResultObject()
                {
                    ReturnCode = 0,
                    ReturnMessage = "Success"
                };
                RegistrationConsumerInput param = new RegistrationConsumerInput();
                int ParentId = -1; //initialize default value, if validate introducerGUID successful, it will be replaced.
                bool validation = ValidationParam(deliveryTag, properties, body, param, out RegistrationConsumerDto jsonMsg,
                    errorObject, util, out ParentId, _channel, out bool DefaultIntroGUID, out string _idToken);

                if (validation)
                {
                    #region Execution
                    RegistrationMemberInsert memberService = new RegistrationMemberInsert();
                    try
                    {
                        jsonMsg.ParentId = ParentId;
                        var RegisterResult = memberService.Insert(jsonMsg);
                        if (RegisterResult.ReturnCode == 0)
                        {
                            #region Output Msg
                            var dto = new RegistrationPublisherDto()
                            {
                                GlobalGUID = RegisterResult.GlobalGUID,
                                IntroducerGlobalGUID = RegisterResult.IntroducerGlobalGUID,
                                IdToken = _idToken,
                                Username = RegisterResult.Username
                            };
                            callbackmsg = EncodeMessage(JsonConvert.SerializeObject(dto));
                            _channel.BasicPublish(_settings.Exchange, "", null, callbackmsg);
                            _channel.BasicAck(deliveryTag, false);
                            SingletonLogger.Info(JsonConvert.SerializeObject(dto) + " The account has successfully registered. ");
                            #endregion
                        }
                        else
                        {
                            #region Output Msg
                            var callbackobj = new RegistrationPublisherDto()
                            {
                                GlobalGUID = jsonMsg.GlobalGUID,
                                IntroducerGlobalGUID = jsonMsg.IntroducerGlobalGUID,
                                IdToken = _idToken,
                                ReturnCode = RegisterResult.ReturnCode,
                                ReturnMessage = "Unable to register this account due to reason: " + RegisterResult.ReturnMessage
                            };
                            SingletonLogger.Error(JsonConvert.SerializeObject(callbackobj));
                            #endregion
                            callbackmsg = EncodeMessage(JsonConvert.SerializeObject(callbackobj));
                            _channel.BasicPublish(_settings.Exchange, "", null, callbackmsg);
                            _channel.BasicReject(deliveryTag, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        #region Output Msg
                        var callbackobj = new RegistrationPublisherDto()
                        {
                            GlobalGUID = jsonMsg.GlobalGUID,
                            IntroducerGlobalGUID = jsonMsg.IntroducerGlobalGUID,
                            IdToken = _idToken,
                            ReturnCode = ex.HResult,
                            ReturnMessage = ex.Message
                        };
                        SingletonLogger.Error(JsonConvert.SerializeObject(callbackobj));
                        SingletonLogger.Error(ex.ExceptionToString());
                        #endregion
                        callbackmsg = EncodeMessage(JsonConvert.SerializeObject(callbackobj));
                        _channel.BasicPublish(_settings.Exchange, "", null, callbackmsg);
                        _channel.BasicReject(deliveryTag, false);
                    }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
    }
}
