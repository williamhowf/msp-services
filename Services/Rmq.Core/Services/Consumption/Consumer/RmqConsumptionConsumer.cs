using Com.GGIT.Common;
using Com.GGIT.Common.Util;
using Com.GGIT.LogLib;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Rmq.Core.Common;
using Rmq.Core.Model.Base;
using Rmq.Core.Model.Consumption;
using Rmq.Core.Settings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rmq.Core.Services.Consumption.Consumer
{
    public class RmqConsumptionConsumer : IDisposable
    {
        private string _identity;
        private IConnection _connection;
        private readonly RabbitMQConfig settings;
        private TimeStampUtil _timeStampUtil;

        public RmqConsumptionConsumer(string identity, IConnection connection, RabbitMQConfig rmqSettings)
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
                    _timeStampUtil.InsertLastActivityLogTimestamp("RMQ-Consumption-Consumer"); //voonkeong 20201125 MDT-1757
                    Task.Delay(settings.ThreadSleepTimeSec * 30000).Wait(workerCancellationToken);
                }
            }
            catch (OperationCanceledException ocex)
            {
                SingletonLogger.Error(ocex.Message);
            }
            catch (Exception ex)
            {
                SingletonLogger.Error(ex);
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
                SingletonLogger.Error(ex);
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

            #region Validator
            public ResultObject ConsumerValidator(ConsumptionConsumerDto jsonMsg, ResultObject resultObject)
            {
                if (string.IsNullOrWhiteSpace(jsonMsg.SubscriberId))
                {
                    resultObject.ReturnCode = 8001;
                    resultObject.ReturnMessage = "Subscriber Id Is Required.";
                }
                else if (string.IsNullOrEmpty(jsonMsg.PlatformId))
                {
                    resultObject.ReturnCode = 8004;
                    resultObject.ReturnMessage = "Platform Id Is Required.";
                }
                else if (string.IsNullOrWhiteSpace(jsonMsg.DistributionTrxId))
                {
                    resultObject.ReturnCode = 8015;
                    resultObject.ReturnMessage = "Distribution Trx Id Is Required.";
                }
                else if (string.IsNullOrWhiteSpace(jsonMsg.OrderId))
                {
                    resultObject.ReturnCode = 8016;
                    resultObject.ReturnMessage = "Order Id Is Required.";
                }
                else if (jsonMsg.OrderAmt < 0)
                {
                    resultObject.ReturnCode = 8003;
                    resultObject.ReturnMessage = "Order amount must greater than or equal to 0.";
                }
                else if (jsonMsg.GuaranteedAmt <= 0)
                {
                    resultObject.ReturnCode = 8017;
                    resultObject.ReturnMessage = "Guaranteed amount must greater than 0.";
                }
                else if (jsonMsg.ConsumptionAmt <= 0)
                {
                    resultObject.ReturnCode = 8018;
                    resultObject.ReturnMessage = "Consumption amount must greater than 0.";
                }
                else if (jsonMsg.GrowthRewardAmt < 0)
                {
                    resultObject.ReturnCode = 8019;
                    resultObject.ReturnMessage = "Growth reward amount must greater than or equal to 0.";
                }
                else if (jsonMsg.ConsumerReferralAmt < 0)
                {
                    resultObject.ReturnCode = 8020;
                    resultObject.ReturnMessage = "Consumer referral amount must greater than or equal to 0.";
                }
                else if (jsonMsg.TotalDistributionAmt < 0)
                {
                    resultObject.ReturnCode = 8021;
                    resultObject.ReturnMessage = "Distribution amount must greater than or equal to 0.";
                }
                else if (jsonMsg.UsdToMbtcRate <= 0)
                {
                    resultObject.ReturnCode = 8022;
                    resultObject.ReturnMessage = "USD to Mbtc rate must greater than 0.";
                }
                else if (jsonMsg.UsdAmt < 0)
                {
                    resultObject.ReturnCode = 8023;
                    resultObject.ReturnMessage = "USD amount must greater than or equal to 0.";
                }
                else if (!util.PlatformCodeValid(jsonMsg.PlatformId))
                {
                    resultObject.ReturnCode = 8008;
                    resultObject.ReturnMessage = "Platform Id Is Not Valid.";
                }
                else if (!util.GlobalUserIdValid(jsonMsg.GlobalUserId))
                {
                    resultObject.ReturnCode = 8006;
                    resultObject.ReturnMessage = "Global User Id Is Not Valid.";
                }
                else if (!util.SubscriberIdValid(jsonMsg.SubscriberId))
                {
                    resultObject.ReturnCode = 8005;
                    resultObject.ReturnMessage = "Subscriber Is Not Valid.";
                }
                else if (util.ExistedInterfaceTransaction(jsonMsg.PlatformId, jsonMsg.DistributionTrxId, jsonMsg.OrderId, _settings.Debug))
                {
                    resultObject.ReturnCode = 8011;
                    resultObject.ReturnMessage = "Duplicate transaction.";
                }
                else if (!string.IsNullOrEmpty(jsonMsg.MerchantId) || !string.IsNullOrEmpty(jsonMsg.MerchantName))
                {
                    if (!util.GlobalUserIdValid(jsonMsg.MerchantId))
                    {
                        resultObject.ReturnCode = 8010;
                        resultObject.ReturnMessage = "Merchant id is not valid.";
                    }
                    else if (!jsonMsg.MerchantAmount.HasValue)
                    {
                        resultObject.ReturnCode = 8012;
                        resultObject.ReturnMessage = "Merchant amount is required.";
                    }
                    else if (jsonMsg.MerchantAmount.HasValue && jsonMsg.MerchantAmount.Value < 0)
                    {
                        resultObject.ReturnCode = 8013;
                        resultObject.ReturnMessage = "Merchant amount must greater than or equal to 0.";
                    }
                }
                else if (jsonMsg.MerchantAmount.HasValue)
                {
                    if (jsonMsg.MerchantAmount > 0 && string.IsNullOrEmpty(jsonMsg.MerchantId))
                    {
                        resultObject.ReturnCode = 8010;
                        resultObject.ReturnMessage = "Merchant id is not valid.";
                    }
                }
                return resultObject;
            }
            #endregion

            #region Processor
            public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
            {
                byte[] callbackmsg;
                bool validation = true;
                ConsumptionConsumerDto jsonMsg = null;
                ConsumptionPublisherDto callbackobj = null;
                var responseObject = new ResultObject();

                try
                {
                    #region Json Input Validation
                    var msg = CommUtil.DecodeMessage(body);
                    SingletonLogger.Info("\nHeader : " + CommUtil.SerializeToString(properties.Headers) + "\nMessage received : " + msg);
                    try
                    {
                        jsonMsg = CommUtil.DeserializeToObject<ConsumptionConsumerDto>(msg);
                        if (jsonMsg == null)
                        {
                            responseObject.ReturnCode = 7005;
                            responseObject.ReturnMessage = "Empty payload received. Message body =>" + msg;
                            validation = false;
                        }
                    }
                    catch (JsonReaderException jsonEx)
                    {
                        SingletonLogger.Error("Failed to deserialize json object! Invalid json path: " + jsonEx.Path);
                        responseObject.ReturnCode = 7010;
                        responseObject.ReturnMessage = "Invalid json input at " + jsonEx.Path + ". Message body =>" + msg;
                        validation = false;
                    }
                    catch (Exception ex)
                    {
                        SingletonLogger.Error("Failed to deserialize json object! Exception: " + ex.Message);
                        responseObject.ReturnCode = 7006;
                        responseObject.ReturnMessage = "Exception: " + ex.Message + ". Message body =>" + msg;
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

                                    callbackobj = new ConsumptionPublisherDto
                                    {
                                        DistributionTrxId = jsonMsg.DistributionTrxId,
                                        OrderId = jsonMsg.OrderId,
                                        Status = "Failed",
                                        Message = "Validation failed; reason = (" + responseObject.ReturnCode + ")" + responseObject.ReturnMessage
                                    };
                                    SingletonLogger.Error(CommUtil.SerializeToString(callbackobj));
                                    var callbackmessage = CommUtil.EncodeMessage(JsonConvert.SerializeObject(callbackobj));
                                    _channel.BasicPublish(_settings.Exchange, "", null, callbackmessage);
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
                        callbackobj = new ConsumptionPublisherDto
                        {
                            DistributionTrxId = jsonMsg == null ? "" : jsonMsg.DistributionTrxId,
                            OrderId = jsonMsg == null ? "" : jsonMsg.OrderId,
                            Status = "Failed",
                            Message = CommUtil.ValidationMessage(responseObject.ReturnCode, responseObject.ReturnMessage)
                        };
                        SingletonLogger.Error(CommUtil.SerializeToString(callbackobj));
                        callbackmsg = CommUtil.EncodeMessage(CommUtil.SerializeToString(callbackobj));
                        _channel.BasicPublish(_settings.Exchange, "", null, callbackmsg);
                        _channel.BasicReject(deliveryTag, false);

                        validation = false; // skip the insert part by assign false 
                    }

                    if (validation)
                    {
                        #region Insert Execution
                        ConsumptionTransactionInsert TrxInsert = new ConsumptionTransactionInsert(jsonMsg, util);

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

                                callbackobj = new ConsumptionPublisherDto
                                {
                                    DistributionTrxId = jsonMsg.DistributionTrxId,
                                    OrderId = jsonMsg.OrderId,
                                    Status = "Failed",
                                    Message = CommUtil.ErrorMessage(responseObject.ReturnCode, responseObject.ReturnMessage)
                                };
                                SingletonLogger.Error(CommUtil.SerializeToString(callbackobj));
                                callbackmsg = CommUtil.EncodeMessage(CommUtil.SerializeToString(callbackobj));
                                _channel.BasicPublish(_settings.Exchange, "", null, callbackmsg);
                                _channel.BasicReject(deliveryTag, false);
                            }
                        }
                        catch (Exception ex)
                        {
                            callbackobj = new ConsumptionPublisherDto
                            {
                                DistributionTrxId = jsonMsg.DistributionTrxId,
                                OrderId = jsonMsg.OrderId,
                                Status = "Failed",
                                Message = CommUtil.ErrorMessage(9999, ex.Message)
                            };
                            SingletonLogger.Error(ex.ExceptionToString());
                            SingletonLogger.Error(CommUtil.SerializeToString(callbackobj));
                            callbackmsg = CommUtil.EncodeMessage(CommUtil.SerializeToString(callbackobj));
                            _channel.BasicPublish(_settings.Exchange, "", null, callbackmsg);
                            _channel.BasicReject(deliveryTag, false);
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    SingletonLogger.Error(ex.ExceptionToString());
                    callbackobj = new ConsumptionPublisherDto
                    {
                        DistributionTrxId = jsonMsg.DistributionTrxId,
                        OrderId = jsonMsg.OrderId,
                        Status = "Failed",
                        Message = CommUtil.ErrorMessage(9999, ex.Message)
                    };
                    SingletonLogger.Error(CommUtil.SerializeToString(callbackobj));
                    callbackmsg = CommUtil.EncodeMessage(CommUtil.SerializeToString(callbackobj));
                    _channel.BasicPublish(_settings.Exchange, "", null, callbackmsg);
                    _channel.BasicReject(deliveryTag, false);
                }
            }
            #endregion
        }
    }
}
