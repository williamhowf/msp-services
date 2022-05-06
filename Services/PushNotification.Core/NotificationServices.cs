using Com.GGIT.Common;
using Com.GGIT.Common.Util;
using Com.GGIT.Database;
using Com.GGIT.Database.Domain;
using Com.GGIT.Database.Extensions;
using Com.GGIT.LogLib;
using Com.GGIT.Security.Authorization;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Transform;
using PushNotification.Core.Model.Finsys;
using PushNotification.Core.Model.Notification;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PushNotification.Core
{
    public class NotificationServices : INotificationServices //wailiang 20200826 MDT-1591
    {
        private ISession session;
        private TimeStampUtil _commUtil = new TimeStampUtil();

        private const string MSP_Notification_T_Transaction = "MSP_Notification_T_Transaction";
        private const string MSP_Notification_SleepTimer = "MSP_Notification_SleepTimer";
        private const string MSP_Notification_MaxRecord = "MSP_Notification_MaxRecord";
        private const string MSP_Notification_FinsysAPI = "MSP_Notification_FinsysAPI";
        private const string MSP_Notification_Auth_Username = "MSP_Notification_Auth_Username";
        private const string MSP_Notification_Auth_Password = "MSP_Notification_Auth_Password";
        private const string MSP_Notification_Auth_URL = "MSP_Notification_Auth_URL";
        private const string MSP_Notification_ThreadNumber = "MSP_Notification_ThreadNumber";

        #region Function Method
        public void Run(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                var setting = GetSettingValue();

                string maxRecord = (setting.MaxRecord != 0) ? "top " + setting.MaxRecord : "";
                int numberError = 0;

                if (setting.PushNotificationEnable)
                {
                    using (session = new SessionDB().OpenSession())
                    {
                        try
                        {
                            var recordsList = session.CreateSQLQuery(@"SELECT " + maxRecord
                                              + " m.Id, m.BatchId, m.RefId, m.RefType, "
                                              + " m.CustomerID, m.GlobalGUID, m.TemplateCode"
                                              + " , case when(m.TemplateCode = 'CRU')"
                                              + " then l.ResourceValue else m.ParamValue"
                                              + " end as ParamValue, "
                                              + " case when (c.LanguageCode = 'zh-cn') "
                                              + " then nt.Title_CN else nt.Title_EN end as Title"
                                              + " , case when(c.LanguageCode = 'zh-cn')"
                                              + " then nt.Body_CN else nt.Body_EN end as Body,"
                                              + " m.IsSent, m.CreatedOnUtc, m.UpdatedOnUtc,ISNULL(c.LanguageCode,'zh-cn') AS LanguageCode "
                                              + " FROM MSPLog_MSP_Notification m "
                                              + " INNER JOIN Customer c on c.Id = m.CustomerId "
                                              + " INNER JOIN MSP_Membertree t on c.Id = t.CustomerID "
                                              + " INNER JOIN MSPLog_MSP_Notification_Template nt on m.TemplateCode = nt.TemplateCode"
                                              + " LEFT JOIN MSP_Language l on m.ParamValue = l.ResourceName and c.LanguageCode = l.LanguageCode "
                                              + " WHERE m.IsSent = :mIsSent "
                                              + " ORDER BY m.CreatedOnUtc ")
                                              .SetParameter("mIsSent", 0)
                                              .SetResultTransformer(Transformers.AliasToBean(typeof(NotificationDto)))
                                              .List<NotificationDto>();

                            if (recordsList.Count > 0)
                            {
                                var response = new AceAuthToken().GetAuthorizationTokenAsync();

                                if (response.Result.GetTokenSuccess)
                                {
                                    Parallel.ForEach(recordsList, new ParallelOptions { MaxDegreeOfParallelism = setting.ThreadNumber }, i =>
                                    {
                                        numberError += ProcessNotification(i, response.Result.AccessToken, setting);
                                    });
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            SingletonLogger.Error(ex.ExceptionToString());
                        }
                    }

                }

                _commUtil.InsertLastActivityLogTimestamp("Worker-PushNotification");

                if (numberError > 0) SingletonLogger.Info("Number of records unable to process : " + numberError);
                SingletonLogger.Info("\nPush Notification services sleep for " + (setting.SleepTimer / setting.SleepTimer) + " minutes...");
            }
        }

        public SettingsDto GetSettingValue()
        {
            var settingDto = new SettingsDto();

            try
            {
                using (session = new SessionDB().OpenSession())
                {
                    string[] settingList =
                    {
                        MSP_Notification_SleepTimer,
                        MSP_Notification_T_Transaction,
                        MSP_Notification_MaxRecord,
                        MSP_Notification_FinsysAPI,
                        MSP_Notification_Auth_Username,
                        MSP_Notification_Auth_Password,
                        MSP_Notification_Auth_URL,
                        MSP_Notification_ThreadNumber
                    };

                    var setttingValue = session.Query<MSP_Setting>().Where(x => settingList.Contains(x.SettingKey)).Select(x => new
                    {
                        x.SettingKey,
                        x.SettingValue
                    })
                    .Take(settingList.Count())
                    .ToList();

                    settingDto = new SettingsDto
                    {
                        PushNotificationEnable = Convert.ToBoolean(setttingValue.Where(s => s.SettingKey == MSP_Notification_T_Transaction).Select(v => v.SettingValue).FirstOrDefault()),
                        SleepTimer = Convert.ToInt32(setttingValue.Where(s => s.SettingKey == MSP_Notification_SleepTimer).Select(v => v.SettingValue).FirstOrDefault()),
                        MaxRecord = Convert.ToInt32(setttingValue.Where(s => s.SettingKey == MSP_Notification_MaxRecord).Select(v => v.SettingValue).FirstOrDefault()),
                        FinsysAPI = setttingValue.Where(s => s.SettingKey == MSP_Notification_FinsysAPI).Select(v => v.SettingValue).FirstOrDefault(),
                        Username = setttingValue.Where(s => s.SettingKey == MSP_Notification_Auth_Username).Select(v => v.SettingValue).FirstOrDefault(),
                        Password = setttingValue.Where(s => s.SettingKey == MSP_Notification_Auth_Password).Select(v => v.SettingValue).FirstOrDefault(),
                        Url = setttingValue.Where(s => s.SettingKey == MSP_Notification_Auth_URL).Select(v => v.SettingValue).FirstOrDefault(),
                        ThreadNumber = Convert.ToInt32(setttingValue.Where(s => s.SettingKey == MSP_Notification_ThreadNumber).Select(v => v.SettingValue).FirstOrDefault())
                    };

                    return settingDto;
                }
            }
            catch (Exception ex)
            {
                SingletonLogger.Error(ex.ExceptionToString());
                return settingDto;
            }
        }

        public int ProcessNotification(object state, string accessToken, SettingsDto setting)
        {
            NotificationDto notification = state as NotificationDto;

            SingletonLogger.Info(Thread.CurrentThread.Name + " ~ Posting transaction to API endpoint, info => " + JsonConvert.SerializeObject(notification));

            try
            {
                SendNotification(notification, accessToken, setting);
            }
            catch (Exception ex)
            {
                SingletonLogger.Error(ex.ExceptionToString());
                return 1;
            }
            return 0;
        }

        public void SendNotification(NotificationDto notification, string accessToken, SettingsDto setting)
        {
            FinsysRequestDto finsysRequest = new FinsysRequestDto
            {
                title = notification.Title,
                userId = notification.GlobalGUID,
                content = notification.Body
            };

            if (!string.IsNullOrEmpty(notification.ParamValue))
                finsysRequest.content = string.Format(notification.Body, notification.ParamValue.Split(','));
            else
            {
                SingletonLogger.Info("Param Assigned Fail");
                Update_MSP_Notification(notification.Id, "Param Assigned Fail", null);
                return;
            }

            FinsysResponseDto response = SendToFinSysAPIAsync(finsysRequest, accessToken, notification.Id, setting).Result;

            string respMsg = response.code + " : " + "Success";

            if (response.code != (int)HttpStatusCode.OK)
            {
                SingletonLogger.Error(response.message);
                respMsg = response.code + " : " + response.message;
                Update_MSP_Notification(notification.Id, respMsg, null);
                return;
            }

            Update_MSP_Notification(notification.Id, respMsg, finsysRequest);
        }

        public async Task<FinsysResponseDto> SendToFinSysAPIAsync(FinsysRequestDto finsysRequest, string authToken, int NotificationId, SettingsDto setting)
        {
            //post
            FinsysResponseDto response = new FinsysResponseDto();

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                    StringContent httpContent = new StringContent(JsonConvert.SerializeObject(finsysRequest), Encoding.UTF8, "application/json");

                    HttpResponseMessage result = client.PostAsync(setting.FinsysAPI, httpContent).Result;
                    string respContent = await result.Content.ReadAsStringAsync();
                    response.code = (int)result.StatusCode;

                    if (result.StatusCode != HttpStatusCode.OK)
                    {
                        if (result.StatusCode == HttpStatusCode.BadRequest || result.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            FinsysResponseDto jsonResponse = JsonConvert.DeserializeObject<FinsysResponseDto>(respContent);

                            response.error = jsonResponse.error;
                            response.message = jsonResponse.message;
                        }
                        else
                        {
                            response.message = respContent;
                            SingletonLogger.Error("ID: " + NotificationId + " " + respContent);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SingletonLogger.Error(ex.ExceptionToString());
            }

            return response;
        }

        public void Update_MSP_Notification(int NotificationId, string SysRemark, FinsysRequestDto Dto)
        {
            try
            {
                //need to declare new connection for each parallel, because using parallel insert/update/delete into db
                using (var session = new SessionDB().OpenSession())
                {
                    MSP_Notification notify = session.Query<MSP_Notification>().Where(k => k.Id == NotificationId).FirstOrDefault();

                    notify.SysRemark = (SysRemark.Length >= 300) ? SysRemark.Replace("'", "\"").Substring(0, 300) : SysRemark.Replace("'", "\"");
                    notify.IsSent = true;
                    notify.UpdatedOnUtc = DateTime.Now;

                    session.SaveTransaction(notify);

                    if (Dto != null)
                    {
                        MSP_Notification_Message notification_Message = new MSP_Notification_Message
                        {
                            NotificationID = NotificationId,
                            Title = Dto.title,
                            Body = Dto.content,
                            CreatedOnUtc = DateTime.UtcNow
                        };
                        session.SaveTransaction(notification_Message);
                    }
                    else
                    {
                        SingletonLogger.Error("Dto is null");
                    }
                }
            }
            catch (Exception ex)
            {
                SingletonLogger.Error("Update Fail: " + ex.ExceptionToString());
            }
        }

        #endregion
    }
}
