using Microsoft.Extensions.Configuration;
using System;

namespace Com.GGIT.Common.Util
{
    public class AppSettingsJson
    {
        public static string GetAppSettingsValue(string section, string settingName)
        {
            var configBuilder = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json");

            IConfigurationRoot configuration = configBuilder.Build();

            var settingSection = configuration.GetSection(section);

            if (settingSection == null)
            {
                return "";
            }
            else
            {
                return settingSection[settingName];
            }
        }
    }
}
