using Com.GGIT.Enumeration;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Rmq.Core.Settings
{
    public class RabbitMQSettings
    {
        private readonly static string ConfigPath = string.Concat(AppDomain.CurrentDomain.BaseDirectory, PathEnum.RabbitMQPath);
        public static RabbitMQConfig LoadConfig() => JsonConvert.DeserializeObject<RabbitMQConfig>(File.ReadAllText(ConfigPath));
    }

    public class RabbitMQConfig
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public int Port { get; set; }
        public string Queue { get; set; }
        public string Exchange { get; set; }
        public bool AutoAck { get; set; }
        public bool ValidateToken { get; set; }
        public bool MSP_CheckExpiredToken { get; set; }
        public bool CheckRabbitMQTimestamp { get; set; }
        public int Connections { get; set; }
        public int Channels { get; set; }
        public int ThreadSleepTimeSec { get; set; }
        public bool Debug { get; set; }
    }
}
