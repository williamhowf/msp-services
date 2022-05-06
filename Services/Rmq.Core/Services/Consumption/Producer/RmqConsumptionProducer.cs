using Com.GGIT.Common;
using Com.GGIT.Common.Util;
using Com.GGIT.Database;
using Com.GGIT.Database.Domain;
using Com.GGIT.Database.Extensions;
using Com.GGIT.LogLib;
using RabbitMQ.Client;
using Rmq.Core.Common;
using Rmq.Core.Model.Consumption;
using Rmq.Core.Settings;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rmq.Core.Services.Consumption.Producer
{
    public class RmqConsumptionProducer : IDisposable
    {
        private IConnection _connection;
        private readonly RabbitMQConfig settings;
        private TimeStampUtil _timeStampUtil;

        public RmqConsumptionProducer(IConnection connection, RabbitMQConfig rmqSettings)
        {
            _connection = connection;
            settings = rmqSettings;
            _timeStampUtil = new TimeStampUtil();
        }

        public void Run(CancellationToken publisherCancelToken)
        {
            try
            {
                SingletonLogger.Info("It's running...");
                using (var channel = _connection.CreateModel())
                {
                    while (!publisherCancelToken.IsCancellationRequested)
                    {
                        try
                        {
                            using var session = new SessionDB().OpenSession();
                            var messages = session.Query<MSP_Interface_Transactions>().Where
                                (x => x.IsMsgSent == false &&
                                (x.Status == "S" || x.Status == "E" || x.Status == "F")
                                ).ToList();

                            int msgSuccess = 0;
                            foreach (var m in messages)
                            {
                                if (!publisherCancelToken.IsCancellationRequested)
                                {
                                    try
                                    {
                                        var publishMsg = new ConsumptionPublisherDto
                                        {
                                            DistributionTrxId = m.DistTrxID,
                                            OrderId = m.OrderID,
                                            Status = m.Status == "S" ? "Success" : "Fail",
                                            Message = m.Status == "S" ? "" : m.SysRemark
                                        };

                                        m.IsMsgSent = true;
                                        m.MsgSentOnUTC = DateTime.UtcNow;
                                        session.UpdateTransaction(m);

                                        var jsonmsg = CommUtil.SerializeToString(publishMsg);
                                        SingletonLogger.Info("Sending to queue => " + jsonmsg);

                                        channel.BasicPublish(settings.Exchange, "", null, CommUtil.EncodeMessage(jsonmsg));
                                        msgSuccess++;
                                    }
                                    catch (Exception ex)
                                    {
                                        SingletonLogger.Error("TransactionId: " + m.OrderID + " | Exception: " + ex.ExceptionToString());
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            SingletonLogger.Info("Total messages sent : " + msgSuccess + "/" + messages.Count);
                        }
                        catch (Exception ex)
                        {
                            SingletonLogger.Error(ex);
                        }
                        finally
                        {
                            _timeStampUtil.InsertLastActivityLogTimestamp("RMQ-Consumption-Producer"); //voonkeong 20201125 MDT-1757
                            Task.Delay(settings.ThreadSleepTimeSec * 1000).Wait(publisherCancelToken);
                        }
                    }
                }
            }
            catch (OperationCanceledException atex)
            {
                SingletonLogger.Error(atex.Message); // log thread status when cancelled
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

        public void Dispose()
        {
            SingletonLogger.Info("Stopping producer...");

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
                SingletonLogger.Error(CommUtil.SerializeToString(_connection.ShutdownReport));

                if (_connection.IsOpen || _connection.CloseReason == null) _connection.Close(); // try to close connection again
            }
            finally
            {
                _connection.Dispose(); // dispose connection and release memory
                _connection = null;
            }
        }
    }
}
