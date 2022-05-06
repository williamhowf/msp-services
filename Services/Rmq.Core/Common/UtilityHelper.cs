using Com.GGIT.Common;
using Com.GGIT.Database;
using Com.GGIT.Database.Domain;
using Com.GGIT.LogLib;
using Com.GGIT.Security.Authentication;
using NHibernate.Util;
using Rmq.Core.Model.Base;
using Rmq.Core.Model.Registration;
using Rmq.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rmq.Core.Common
{
    public class UtilityHelper : IDisposable
    {
        private readonly SessionDB sessionFactory;

        //private IDictionary<string, int> PlatformCodeId = new Dictionary<string, int>(); //WilliamHo 20201015 check with database directly, avoid any new consumption platform added without restart rabbitmq

        public UtilityHelper()
        {
            sessionFactory = new SessionDB();
            //InitPlatforms();
        }

        public void Dispose()
        {
            if (sessionFactory != null) sessionFactory.CloseSessions();
        }

        public decimal TruncateDecimal_ScaleSix(decimal value) => TruncateDecimal(value, 6);

        public decimal TruncateDecimal_ScaleEight(decimal value) => TruncateDecimal(value, 8);

        public decimal TruncateDecimal(decimal value, int precision)
        {
            decimal step = (decimal)Math.Pow(10, precision);
            decimal tmp = Math.Truncate(step * value);
            return tmp / step;
        }

        #region code comment
        //private void InitPlatforms() //WilliamHo 20201015 check with database directly, avoid any new consumption platform added without restart rabbitmq
        //{
        //    if (PlatformCodeId.Count == 0)
        //    {
        //        try
        //        {
        //            using var db = sessionFactory.OpenSession();
        //            PlatformCodeId = db.Query<MSP_Consumption_Platform>().Select(x => new KeyValuePair<string, int>(x.PlatformCode, x.PlatformID)).ToDictionary(x => x.Key, y => y.Value);
        //        }
        //        catch (Exception ex)
        //        {
        //            SingletonLogger.Error(ex.ExceptionToString());
        //        }
        //    }
        //}
        #endregion

        /// <summary>
        /// Retrieve MSP_Setting value with setting key given. If defaultValue not provided, function will return default value of specified object type[T]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">setting key</param>
        /// <param name="defaultValue">default object value if the setting value not found</param>
        /// <returns>value of specified object type</returns>
        public T GetMSP_Setting<T>(string key, object defaultValue = null)
        {
            T value = defaultValue.IsNull() ? default : (T)defaultValue;
            try
            {
                using var db = new SessionDB().OpenSession();
                string s_value = db.Query<MSP_Setting>().Where(k => k.SettingKey == key).Select(x => x.SettingValue).FirstOrDefault();
                if (!string.IsNullOrEmpty(s_value)) value = (T)Convert.ChangeType(s_value, typeof(T));
            }
            catch (Exception ex)
            {
                SingletonLogger.Error(ex.ExceptionToString());
            }
            return value;
        }

        public bool PlatformCodeValid(string platform)
        {
            if (string.IsNullOrEmpty(platform)) return false;
            //if (PlatformCodeId.Count == 0) InitPlatforms(); //WilliamHo 20201015 check with database directly, avoid any new consumption platform added without restart rabbitmq
            //return PlatformCodeId.ContainsKey(platform);
            try
            {
                using var db = sessionFactory.OpenSession();
                return db.Query<MSP_Consumption_Platform>().Where(x => x.PlatformCode == platform).Any();
            }
            catch (Exception ex)
            {
                SingletonLogger.Error(ex.ExceptionToString());
            }
            return false;
        }

        public int GetPlatformIdByPlatformCode(string code)
        {
            //if (PlatformCodeId.Count == 0) InitPlatforms(); //WilliamHo 20201015 check with database directly, avoid any new consumption platform added without restart rabbitmq
            //if (PlatformCodeId.TryGetValue(code, out int value)) return value;
            try
            {
                using var db = sessionFactory.OpenSession();
                return db.Query<MSP_Consumption_Platform>().Where(x => x.PlatformCode == code).Select(c => c.PlatformID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                SingletonLogger.Error(ex.ExceptionToString());
            }
            return 0;
        }

        public bool GlobalUserIdValid(string guid)
        {
            if (string.IsNullOrEmpty(guid)) return false;
            using var db = sessionFactory.OpenSession();
            return db.Query<MSP_MemberTree>().Any(x => x.GlobalGUID.ToUpper() == guid.ToUpper());
        }

        public bool SubscriberIdValid(string said)
        {
            if (string.IsNullOrEmpty(said)) return false;
            try
            {
                using var db = sessionFactory.OpenSession();
                return db.Query<MSP_BE_Subscriber>().Any(x => x.said == said);
            }
            catch (Exception ex)
            {
                SingletonLogger.Error(ex.ExceptionToString());
            }
            return false;
        }

        public bool ExistedInterfaceTransaction(string platformId, string distributionTrxId, string orderId, bool debug)
        {
            if (debug) return false; // debugging purpose, to skip checking
            if (string.IsNullOrEmpty(platformId) || string.IsNullOrEmpty(distributionTrxId) || string.IsNullOrEmpty(orderId)) return true;
            try
            {
                using var db = sessionFactory.OpenSession();
                return db.Query<MSP_Interface_Transactions>().Any(x => x.PlatformCode == platformId && x.DistTrxID == distributionTrxId && x.OrderID == orderId);
            }
            catch (Exception ex)
            {
                SingletonLogger.Error(ex.ExceptionToString());
            }
            return true;
        }

        public bool ValidateTokens(string AccessToken, long timestamp, RabbitMQConfig settings)
        {
            return new AuthToken(
                AccessToken: AccessToken,
                CheckExpired: settings.MSP_CheckExpiredToken,
                CheckTms: settings.CheckRabbitMQTimestamp,
                Timestamp: timestamp,
                IsMilliseconds: true).AuthTokenValid;
        }

        /// <summary>
        /// Validate tokens and retrieve information from tokens(IdToken and AccessToken)
        /// </summary>
        /// <param name="IdToken"></param>
        /// <param name="AccessToken"></param>
        /// <param name="LanguageCode"></param>
        /// <param name="result"></param>
        /// <returns>Registration_FilterParam</returns>
        public RegistrationConsumerDto ValidateTokens(string IdToken, string AccessToken, long timestamp, ResultObject result, RabbitMQConfig settings) //wailiang 20200808 MDT-1581
        {
            try
            {
                var registratonDto = new RegistrationConsumerDto();

                bool CheckExpired = settings.MSP_CheckExpiredToken;
                bool CheckTms = settings.CheckRabbitMQTimestamp;

                var Token = new AuthToken(AccessToken: AccessToken, IdentityToken: IdToken, CheckExpired: CheckExpired, CheckTms: CheckTms, Timestamp: timestamp, IsMilliseconds: true, IsRegistration: true);
                if (!Token.AuthTokenValid)
                {
                    registratonDto = new RegistrationConsumerDto()
                    {
                        GlobalGUID = Token.UserGuid,
                        IntroducerGlobalGUID = Token.ReferralGuid,
                        ErrorMessage = Token.ErrorMessage
                    };

                    result.ReturnCode = 1009;
                    result.ReturnMessage = "Invalid Token";
                    return registratonDto;
                }

                registratonDto = new RegistrationConsumerDto
                {
                    GlobalGUID = Token.UserGuid,
                    IntroducerGlobalGUID = Token.ReferralGuid,
                    UserRole = Token.UserRole,
                    IsUSCitizen = Token.IsUSCitizen,
                    Email = Token.Email,
                    AuthTokenValid = Token.AuthTokenValid
                };

                return registratonDto;
            }
            catch (Exception ex)
            {
                SingletonLogger.Error(ex);
                result.ReturnCode = 3908;
                result.ReturnMessage = "Invalid Input Parameter";
                return null;
            }
        }

        public MSP_SystemCurrency SystemCurrencyRate()
        {
            try
            {
                using var db = sessionFactory.OpenSession();
                return db.Query<MSP_SystemCurrency>().Where(x => x.Code == "DP" && x.Status == "A").FirstOrDefault();
            }
            catch (Exception ex)
            {
                SingletonLogger.Error(ex.ExceptionToString());
            }
            return null;
        }

        public decimal ConsumerReferral_MaxAmtPct() => GetMSP_Setting<decimal>("MSP_ConsumerReferral_MaxAmtPct", 0.45m);
        /*{ // WilliamHo 20201108 Simplify code
            decimal value = 0.45m;
            try
            {
                using var db = new SessionDB().OpenSession();
                string s_value = db.Query<MSP_Setting>().Where(k => k.SettingKey == "MSP_ConsumerReferral_MaxAmtPct").Select(x => x.SettingValue).FirstOrDefault();
                if (!string.IsNullOrEmpty(s_value)) value = Convert.ToDecimal(s_value);
            }
            catch (Exception ex)
            {
                SingletonLogger.Error(ex.ExceptionToString());
            }
            return value;
        }*/

        public decimal ConsumerReferral_DU_Pct_Setting() => GetMSP_Setting<decimal>("ConsumerRefAmt_DU_Pct", 0.15m);
        /*{ // WilliamHo 20201108 Simplify code
            decimal value = 0.15m;
            try
            {
                using var db = new SessionDB().OpenSession();
                string s_value = db.Query<MSP_Setting>().Where(k => k.SettingKey == "ConsumerRefAmt_DU_Pct").Select(x => x.SettingValue).FirstOrDefault();
                if (!string.IsNullOrEmpty(s_value)) value = Convert.ToDecimal(s_value);
            }
            catch (Exception ex)
            {
                SingletonLogger.Error(ex.ExceptionToString());
            }
            return value;
        }*/

        public bool IsGlobalGUIDExists(string guid) => GlobalUserIdValid(guid);

        public RegistrationPublisherDto GetUserRegisteredInfo(string guid)
        {
            RegistrationPublisherDto rg = new RegistrationPublisherDto();
            try
            {
                using var db = new SessionDB().OpenSession();
                var record = db.Query<MSP_MemberTree>().Where(k => k.GlobalGUID == guid).Select(x => new 
                {
                    CustomerId = x.CustomerID,
                    GlobalGuid = x.GlobalGUID,
                    IntroducerGlobalGuid = x.IntroducerGlobalGUID
                }).FirstOrDefault();
                if (record.IsNotNull())
                {
                    string _username = db.Query<Customer>().Where(k => k.Id == record.CustomerId).Select(x => x.Username).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(_username))
                    {
                        rg.GlobalGUID = record.GlobalGuid;
                        rg.IntroducerGlobalGUID = record.IntroducerGlobalGuid;
                        rg.Username = _username;
                        return rg;
                    }
                }
            }
            catch (Exception ex)
            {
                SingletonLogger.Error(ex.ExceptionToString());
            }
            return null;
        }

        /// <summary>
        /// Check if Introducer Exists
        /// </summary>
        /// <param name="IntroducerGlobalGUID">IntroducerGlobalGUID</param>
        /// <param name="ParentId"></param>
        /// <param name="DefaultIntroGUID"></param>
        /// <returns></returns>
        public bool IsIntroducerExists(string IntroducerGlobalGUID, out int ParentId, out bool DefaultIntroGUID) //wailiang 20200808 MDT-1581
        {
            bool IntroducerExists = false;
            ParentId = -1; //set invalid Id, if memberTree found return the correct parentid
            DefaultIntroGUID = false; //set value, if condition is true/false return the correct value

            if (string.IsNullOrEmpty(IntroducerGlobalGUID))
            {
                DefaultIntroGUID = true;
                return true;
            }

            try
            {
                using var db = new SessionDB().OpenSession();
                var memberTree = db.Query<MSP_MemberTree>().Where(x => x.GlobalGUID == IntroducerGlobalGUID).Select(x => new { x.GlobalGUID, x.CustomerID }).FirstOrDefault();

                if (memberTree != null)
                {
                    IntroducerExists = true;
                    ParentId = memberTree.CustomerID;
                    DefaultIntroGUID = false;
                }
            }
            catch (Exception ex)
            {
                SingletonLogger.Error(ex.ExceptionToString());
            }

            return IntroducerExists;
        }

        public bool ValidateInterfaceInMegopolyCashInTrx(string guid, string trxId, bool debug)  //wailiang 20200811 MDT-1582
        {
            if (debug) return false;

            using (var db = sessionFactory.OpenSession())
                return db.Query<MSP_InterfaceIn_Megopoly_CashIn>().Any(x => x.GlobalGuid == guid &&
                x.TrxID == trxId);
        }

        public bool InterfaceOutMegopolyTransactionExists(string guid, string trxId, bool debug)    //clement 20200816 MDT-1580
        {
            if (debug) return true;

            using (var db = sessionFactory.OpenSession())
                return db.Query<MSP_InterfaceOut_Megopoly>().Any(x => x.GlobalGuid == guid &&
                x.BatchID == trxId);
        }

        public bool ValidateInterfaceMegopolyMarketCashInTrx(string guid, string trxId, bool debug) //clement 20200821 MDT-1583
        {
            if (debug) return false;

            using (var db = sessionFactory.OpenSession())
                return db.Query<MSP_InterfaceIn_MegoMarket_CashIn>().Any(x => x.GlobalGuid == guid &&
                x.TrxID == trxId);
        }

        public static string GetGuidLowercase() => Guid.NewGuid().ToString().ToLower(); //clement 20200821 MDT-1583

        public decimal MinDepositBalance_PRTransfer() => GetMSP_Setting<decimal>("MinDepositBalance_PRTransfer");
    }
}
