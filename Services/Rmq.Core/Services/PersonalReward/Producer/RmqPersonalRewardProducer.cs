using Com.GGIT.LogLib;
using Com.GGIT.Common;
using Com.GGIT.Common.Util;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Rmq.Core.Common;
using Rmq.Core.Model.Base;
using Rmq.Core.Model.PersonalReward;
using Rmq.Core.Settings;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Com.GGIT.Security.Authorization;
using Com.GGIT.Database;
using Com.GGIT.Database.Domain;
using Com.GGIT.Database.Extensions;
using System.Linq;

namespace Rmq.Core.Services.PersonalReward.Producer
{
    public class RmqPersonalRewardProducer : IDisposable    //clement 20200816 MDT-1580
    {
        private IConnection _connection;
        private readonly RabbitMQConfig settings;
        private TimeStampUtil _timeStampUtil;

        public RmqPersonalRewardProducer(IConnection connection, RabbitMQConfig rmqSettings)
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
                                var messages = session.Query<MSP_InterfaceOut_Megopoly>().Where
                                    (x => x.Status == "N" || (x.Status == "P" && x.SentOnUtc > DateTime.UtcNow.AddDays(-2) && x.SentOnUtc < DateTime.UtcNow.AddDays(-1)))
                                    .ToList();

                                if (messages.Count > 0)
                                {
                                    SingletonLogger.Info("Number of records to send = " + messages.Count);
                                    AceAuthToken aceAuthToken = new AceAuthToken();
                                    Task<AuthorizationTokenResponse> response = aceAuthToken.GetAuthorizationTokenAsync();

                                    if (response.Result.GetTokenSuccess)
                                    {
                                        int msgSuccess = 0;
                                        foreach (var m in messages)
                                        {
                                            if (!publisherCancelToken.IsCancellationRequested)
                                            {
                                                try
                                                {
                                                    var publishMsg = new PersonalRewardPublisherDto
                                                    {
                                                        SecurityToken = response.Result.AccessToken,
                                                        Guid = m.GlobalGuid,
                                                        TransactionId = m.BatchID,
                                                        RewardBtc = m.Amount
                                                    };
                                                    SingletonLogger.Info("Updating Status to P and SentOnUtc datetime. | Guid: " + m.GlobalGuid + " | TransactionId: " + m.BatchID);
                                                    m.Status = "P";
                                                    m.SentOnUtc = DateTime.UtcNow;
                                                    // call update for each record
                                                    session.UpdateTransaction(m);
                                                    var jsonmsg = JsonConvert.SerializeObject(publishMsg);
                                                    SingletonLogger.Info("Sending to queue => " + jsonmsg);
                                                    /*Based on the query result, consumer.publish to finsys endpoint*/
                                                    channel.BasicPublish(settings.Exchange, "", null, Encoding.UTF8.GetBytes(jsonmsg));
                                                    msgSuccess++;
                                                }
                                                catch (Exception ex)
                                                {
                                                    SingletonLogger.Info("Updating Status to N due to exception. | Guid: " + m.GlobalGuid + " | TransactionId: " + m.BatchID);
                                                    m.Status = "N";
                                                    session.UpdateTransaction(m);
                                                    SingletonLogger.Error("Guid: " + m.GlobalGuid + " | TransactionId: " + m.BatchID + " | " + ex);
                                                }
                                            }
                                            else
                                            {
                                                break;
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
                            _timeStampUtil.InsertLastActivityLogTimestamp("RMQ-PersonalReward-Producer");
                            Task.Delay(settings.ThreadSleepTimeSec * 1000).Wait(publisherCancelToken);
                        }
                    }
                }
            }
            catch (ThreadAbortException atex)
            {
                SingletonLogger.Info(atex.Message); // log thread status when aborting
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
