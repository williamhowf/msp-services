using Com.GGIT.LogLib;
using Com.GGIT.Common;
using Com.GGIT.Common.Util;
using RabbitMQ.Client;
using Rmq.Core.Settings;
using System;
using System.Threading.Tasks;
using System.Threading;
using Com.GGIT.Database;
using Com.GGIT.Database.Domain;
using System.Linq;
using Rmq.Core.Common;
using Com.GGIT.Security.Authorization;
using Newtonsoft.Json;
using System.Text;
using Rmq.Core.Model.MegopolyCashIn;

namespace Rmq.Core.Services.MegopolyCashIn.Producer
{
    public class RmqMegopolyCashInProducer : IDisposable  //wailiang 20200811 MDT-1582
    {
        private IConnection _connection;
        private readonly RabbitMQConfig settings;
        private TimeStampUtil _timeStampUtil;

        public RmqMegopolyCashInProducer(IConnection connection, RabbitMQConfig rmqSettings)
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
                            using (var session = new SessionDB().OpenSession())
                            {
                                var messages = session.Query<MSP_InterfaceIn_Megopoly_CashIn>().Where
                                    (x => x.IsSent == false && (x.Status == "S" || x.Status == "F")
                                    ).ToList();

                                if (messages.Count > 0)
                                {
                                    SingletonLogger.Info("Number of records to send = " + messages.Count);
                                    AceAuthToken aceAuthToken = new AceAuthToken();
                                    Task<AuthorizationTokenResponse> response = aceAuthToken.GetAuthorizationTokenAsync();
                                    if (response.Result.GetTokenSuccess)
                                    {
                                        int msgSuccess = 0;
                                        using (session.BeginTransaction())
                                        {
                                            try
                                            {
                                                session.FlushMode = NHibernate.FlushMode.Commit;
                                                foreach (var m in messages)
                                                {
                                                    if (!publisherCancelToken.IsCancellationRequested)
                                                    {
                                                        var publishMsg = new MegopolyCashInPublisherDto
                                                        {
                                                            SecurityToken = response.Result.AccessToken,
                                                            Guid = m.GlobalGuid,
                                                            TransactionId = m.TrxID,
                                                            Status = m.Status == "S" ? "SUCCESS" : "FAILED",
                                                            ErrorMessage = m.Status == "S" ? "" : m.SysRemark ?? ""
                                                        };
                                                        var jsonmsg = JsonConvert.SerializeObject(publishMsg);
                                                        SingletonLogger.Info("Sending to queue => " + jsonmsg);
                                                        /*Based on the query result, consumer.publish to finsys endpoint*/
                                                        channel.BasicPublish(settings.Exchange, "", null, Encoding.UTF8.GetBytes(jsonmsg));
                                                        m.IsSent = true;
                                                        m.SentOnUtc = DateTime.UtcNow;
                                                        msgSuccess++;
                                                    }
                                                    else
                                                    {
                                                        break;
                                                    }
                                                }
                                                SingletonLogger.Info("Commiting " + msgSuccess + " records...");
                                                //commit transaction
                                                session.Transaction.Commit();
                                            }
                                            catch (Exception ex)
                                            {
                                                SingletonLogger.Error(ex.ExceptionToString());
                                                session.Transaction.Rollback();
                                            }
                                        }
                                        SingletonLogger.Info("Total messages sent : " + msgSuccess + "/" + messages.Count);
                                    }
                                    else
                                    {
                                        SingletonLogger.Error("Failed to get ACE oAuth Token. Skipping publishing process to Exchange: " + settings.Exchange);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            SingletonLogger.Error(ex);
                        }
                        finally
                        {
                            _timeStampUtil.InsertLastActivityLogTimestamp("RMQ-MegopolyCashIn-Producer"); //voonkeong 20201125 MDT-1757
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
                SingletonLogger.Error(ex.ExceptionToString());
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
