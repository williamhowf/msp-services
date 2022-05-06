using Com.GGIT.LogLib;
using Com.GGIT.Common;
using Com.GGIT.Common.Util;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Rmq.Core.Common;
using Rmq.Core.Model.Base;
using Rmq.Core.Model.MegopolyCashIn;
using Rmq.Core.Settings;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Com.GGIT.Security.Authentication;
using Com.GGIT.Security.Authorization;

namespace Rmq.Core.Services.MegopolyCashIn.Consumer
{
    public class RmqMegopolyCashInConsumer  //wailiang 20200811 MDT-1582
    {
        private string _identity;
        private IConnection _connection;
        private readonly RabbitMQConfig settings;
        private TimeStampUtil _timeStampUtil;

        public RmqMegopolyCashInConsumer(string identity, IConnection connection, RabbitMQConfig rmqSettings)
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
                    _timeStampUtil.InsertLastActivityLogTimestamp("RMQ-MegopolyCashIn-Consumer"); //voonkeong 20201125 MDT-1757
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

            public string SerializeToString(object value)
            {
                return JsonConvert.SerializeObject(value);
            }

            public T DeserializeToObject<T>(string value)
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            #endregion

            #region Validator
            public ResultObject ConsumerValidator(MegopolyCashInConsumerDto jsonMsg, ResultObject resultObject)
            {
                DateTime dateValue;

                if (string.IsNullOrWhiteSpace(jsonMsg.SecurityToken))
                {
                    resultObject.ReturnCode = 8200;
                    resultObject.ReturnMessage = "Security Token Is Required.";
                }
                else if (string.IsNullOrEmpty(jsonMsg.Guid))
                {
                    resultObject.ReturnCode = 8201;
                    resultObject.ReturnMessage = "Guid Is Required.";
                }
                else if (string.IsNullOrWhiteSpace(jsonMsg.TransactionId))
                {
                    resultObject.ReturnCode = 8202;
                    resultObject.ReturnMessage = "Transaction Id Is Required.";
                }
                else if (!jsonMsg.TransactionDateTimeOnUtc.HasValue || !DateTime.TryParse(jsonMsg.TransactionDateTimeOnUtc.Value.ToString(), out dateValue))
                {
                    resultObject.ReturnCode = 8203;
                    resultObject.ReturnMessage = "Transaction DateTime Utc Is Invalid.";
                }
                else if (!jsonMsg.AmountBtc.HasValue)
                {
                    resultObject.ReturnCode = 8204;
                    resultObject.ReturnMessage = "AmountBtc Value Is Missing.";
                }
                else if (util.ValidateInterfaceInMegopolyCashInTrx(jsonMsg.Guid, jsonMsg.TransactionId, _settings.Debug))
                {
                    resultObject.ReturnCode = 8205;
                    resultObject.ReturnMessage = "Duplicate Transaction.";
                }
                return resultObject;
            }
            #endregion

            #region Processor
            public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
            {
                byte[] callbackmsg;
                bool validation = true;
                MegopolyCashInConsumerDto jsonMsg = null;
                MegopolyCashInPublisherDto callbackobj = null;
                var responseObject = new ResultObject();

                try
                {
                    #region Json Input Validation
                    var msg = DecodeMessage(body);
                    SingletonLogger.Info("\nHeader : " + SerializeToString(properties.Headers) + "\nMessage received : " + msg);
                    try
                    {
                        jsonMsg = DeserializeToObject<MegopolyCashInConsumerDto>(msg);
                        if (jsonMsg == null)
                        {
                            responseObject.ReturnCode = 7005;
                            responseObject.ReturnMessage = "Empty payload received. Message body =>" + msg;
                            validation = false;
                        }
                    }
                    catch (Exception)
                    {
                        SingletonLogger.Error("Failed to deserialize json object!");
                        responseObject.ReturnCode = 7006;
                        responseObject.ReturnMessage = "Invalid json input. Message body =>" + msg;
                        validation = false;
                    }
                    #endregion

                    #region Message Validation
                    // input message validation
                    if (validation)
                    {
                        //first token validation
                        if (_settings.ValidateToken)
                        {
                            if (properties.Headers.ContainsKey("timestamp_in_ms") && !string.IsNullOrEmpty(jsonMsg.SecurityToken))
                            {
                                var timestampLong = Convert.ToInt64(properties.Headers["timestamp_in_ms"]);
                                if (!util.ValidateTokens(jsonMsg.SecurityToken, timestampLong, _settings))
                                {
                                    responseObject.ReturnCode = 7008;
                                    responseObject.ReturnMessage = "Invalid or expired token";

                                    AceAuthToken aceAuthToken = new AceAuthToken();
                                    Task<AuthorizationTokenResponse> response = aceAuthToken.GetAuthorizationTokenAsync();

                                    if (response.Result.GetTokenSuccess)
                                    {
                                        callbackobj = new MegopolyCashInPublisherDto
                                        {
                                            SecurityToken = response.Result.AccessToken,
                                            Guid = jsonMsg == null ? "" : jsonMsg.Guid,
                                            TransactionId = jsonMsg == null ? "" : jsonMsg.TransactionId,
                                            Status = "FAILED",
                                            ErrorMessage = "Validation failed; reason = (" + responseObject.ReturnCode + ")" + responseObject.ReturnMessage
                                        };
                                        SingletonLogger.Error(SerializeToString(callbackobj));
                                        var callbackmessage = EncodeMessage(JsonConvert.SerializeObject(callbackobj));
                                        _channel.BasicPublish(_settings.Exchange, "", null, callbackmessage);
                                    }
                                    else
                                    {
                                        SingletonLogger.Error("Failed to get ACE oAuth Token. Cannot send reply to Exchange: " + _settings.Exchange);

                                        callbackobj = new MegopolyCashInPublisherDto
                                        {
                                            SecurityToken = "",
                                            Guid = jsonMsg == null ? "" : jsonMsg.Guid,
                                            TransactionId = jsonMsg == null ? "" : jsonMsg.TransactionId,
                                            Status = "FAILED",
                                            ErrorMessage = "Validation failed; reason = (" + responseObject.ReturnCode + ")" + responseObject.ReturnMessage
                                        };
                                        SingletonLogger.Error(SerializeToString(callbackobj));
                                    }

                                    // ack queue regardless of reply sent to Exchange
                                    _channel.BasicReject(deliveryTag, false);
                                    return;
                                }
                            }
                            else
                            {
                                responseObject.ReturnCode = 7009;
                                responseObject.ReturnMessage = "Missing timestamp/tokens";
                            }
                        }
                        //message validation
                        if (responseObject.ReturnCode == 0)
                        {
                            responseObject = ConsumerValidator(jsonMsg, responseObject);
                        }
                    }
                    #endregion

                    // check if any error from validation
                    if (responseObject.ReturnCode != 0)
                    {
                        AceAuthToken aceAuthToken = new AceAuthToken();
                        Task<AuthorizationTokenResponse> response = aceAuthToken.GetAuthorizationTokenAsync();

                        if (response.Result.GetTokenSuccess)
                        {
                            callbackobj = new MegopolyCashInPublisherDto
                            {
                                SecurityToken = response.Result.AccessToken,
                                Guid = jsonMsg == null ? "" : jsonMsg.Guid,
                                TransactionId = jsonMsg == null ? "" : jsonMsg.TransactionId,
                                Status = "FAILED",
                                ErrorMessage = "Validation failed; reason = (" + responseObject.ReturnCode + ")" + responseObject.ReturnMessage
                            };
                            SingletonLogger.Error(SerializeToString(callbackobj));
                            callbackmsg = EncodeMessage(SerializeToString(callbackobj));
                            _channel.BasicPublish(_settings.Exchange, "", null, callbackmsg);
                        }
                        else
                        {
                            SingletonLogger.Error("Failed to get ACE oAuth Token. Cannot send reply to Exchange: " + _settings.Exchange);

                            callbackobj = new MegopolyCashInPublisherDto
                            {
                                SecurityToken = "",
                                Guid = jsonMsg == null ? "" : jsonMsg.Guid,
                                TransactionId = jsonMsg == null ? "" : jsonMsg.TransactionId,
                                Status = "FAILED",
                                ErrorMessage = "Validation failed; reason = (" + responseObject.ReturnCode + ")" + responseObject.ReturnMessage
                            };
                            SingletonLogger.Error(SerializeToString(callbackobj));
                        }

                        // ack queue regardless of reply sent to Exchange
                        _channel.BasicReject(deliveryTag, false);

                        validation = false; // skip the insert part by assign false 
                    }

                    if (validation)
                    {
                        #region Insert Execution
                        MegopolyCashInTransactionInsert TrxInsert = new MegopolyCashInTransactionInsert(jsonMsg, util);

                        try
                        {
                            if (TrxInsert.InsertTransaction())
                            {
                                _channel.BasicAck(deliveryTag, false); // when success insert, ack queue
                            }
                            else
                            {
                                responseObject.ReturnCode = 9990;
                                responseObject.ReturnMessage = "Insert failed.";

                                AceAuthToken aceAuthToken = new AceAuthToken();
                                Task<AuthorizationTokenResponse> response = aceAuthToken.GetAuthorizationTokenAsync();

                                if (response.Result.GetTokenSuccess)
                                {
                                    callbackobj = new MegopolyCashInPublisherDto
                                    {
                                        SecurityToken = response.Result.AccessToken,
                                        Guid = jsonMsg == null ? "" : jsonMsg.Guid,
                                        TransactionId = jsonMsg == null ? "" : jsonMsg.TransactionId,
                                        Status = "FAILED",
                                        ErrorMessage = "Failed to process; reason = (" + responseObject.ReturnCode + ")" + responseObject.ReturnMessage
                                    };
                                    SingletonLogger.Error(SerializeToString(callbackobj));
                                    callbackmsg = EncodeMessage(SerializeToString(callbackobj));
                                    _channel.BasicPublish(_settings.Exchange, "", null, callbackmsg);
                                }
                                else
                                {
                                    SingletonLogger.Error("Failed to get ACE oAuth Token. Cannot send reply to Exchange: " + _settings.Exchange);

                                    callbackobj = new MegopolyCashInPublisherDto
                                    {
                                        SecurityToken = "",
                                        Guid = jsonMsg == null ? "" : jsonMsg.Guid,
                                        TransactionId = jsonMsg == null ? "" : jsonMsg.TransactionId,
                                        Status = "FAILED",
                                        ErrorMessage = "Failed to process; reason = (" + responseObject.ReturnCode + ")" + responseObject.ReturnMessage
                                    };
                                    SingletonLogger.Error(SerializeToString(callbackobj));
                                }

                                // ack queue regardless of reply sent to Exchange
                                _channel.BasicReject(deliveryTag, false);
                            }
                        }
                        catch (Exception ex)
                        {
                            AceAuthToken aceAuthToken = new AceAuthToken();
                            Task<AuthorizationTokenResponse> response = aceAuthToken.GetAuthorizationTokenAsync();

                            if (response.Result.GetTokenSuccess)
                            {
                                callbackobj = new MegopolyCashInPublisherDto
                                {
                                    SecurityToken = response.Result.AccessToken,
                                    Guid = jsonMsg == null ? "" : jsonMsg.Guid,
                                    TransactionId = jsonMsg == null ? "" : jsonMsg.TransactionId,
                                    Status = "FAILED",
                                    ErrorMessage = "Failed to process. reason = (9999)" + ex.Message
                                };
                                SingletonLogger.Error(ex.ExceptionToString());
                                SingletonLogger.Error(SerializeToString(callbackobj));
                                callbackmsg = EncodeMessage(SerializeToString(callbackobj));
                                _channel.BasicPublish(_settings.Exchange, "", null, callbackmsg);
                            }
                            else
                            {
                                SingletonLogger.Error("Failed to get ACE oAuth Token. Cannot send reply to Exchange: " + _settings.Exchange);

                                callbackobj = new MegopolyCashInPublisherDto
                                {
                                    SecurityToken = "",
                                    Guid = jsonMsg == null ? "" : jsonMsg.Guid,
                                    TransactionId = jsonMsg == null ? "" : jsonMsg.TransactionId,
                                    Status = "FAILED",
                                    ErrorMessage = "Failed to process. reason = (9999)" + ex.Message
                                };
                                SingletonLogger.Error(ex.ExceptionToString());
                                SingletonLogger.Error(SerializeToString(callbackobj));
                            }

                            // ack queue regardless of reply sent to Exchange
                            _channel.BasicReject(deliveryTag, false);
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    SingletonLogger.Error(ex.ExceptionToString());
                }
            }
            #endregion
        }
    }
}
