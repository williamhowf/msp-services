using Com.GGIT.Database;
using Com.GGIT.Database.Domain;
using System;
using System.Linq;

namespace Rmq.Core.Settings
{
    public static class MspSettings
    {
        private static byte version;

        public static byte GetVersion()
        {
            if (version == 0)
            {
                using var db = new SessionDB().OpenSession();
                var value = db.Query<MSP_Setting>().Where(k => k.SettingKey == "MSP_Version").Select(x => x.SettingValue).FirstOrDefault();
                if (!string.IsNullOrEmpty(value))
                {
                    try
                    {
                        version = Convert.ToByte(value);
                    }
                    catch (Exception)
                    {
                        version = 5; // set current default version
                    }
                }
            }
            return version;
        }

        public static void ClearVersion() => version = 0;

        public static bool ConsumptionAllowMerchant()
        {
            var AllowMerchant = false;
            using (var db = new SessionDB().OpenSession())
            {
                var value = db.Query<MSP_Setting>().Where(k => k.SettingKey == "MSP_Consumption_Allow_Merchant").Select(x => x.SettingValue).FirstOrDefault();
                if (!string.IsNullOrEmpty(value))
                {
                    try
                    {
                        AllowMerchant = Convert.ToBoolean(value);
                    }
                    catch (Exception)
                    {
                        // Do nothing
                    }
                }
            }
            return AllowMerchant;
        }
    }
}
