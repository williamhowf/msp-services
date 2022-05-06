using NLog;
using System;

namespace Com.GGIT.LogLib
{
    /// <summary>
    /// https://github.com/micheltriana/dotLogger - similar implementation
    /// </summary>
    public sealed class SingletonLogger
    {
        private static readonly Logger instance = LogManager.GetCurrentClassLogger();

        static SingletonLogger()
        {
        }

        private SingletonLogger()
        {
        }

        public static Logger Instance { get { return instance; } }

        public static void Error(string message, params object[] args)
        {
            var logEventInfo = new LogEventInfo(LogLevel.Error, Instance.Name, null, message, args);
            Instance.Log(typeof(Logger), logEventInfo);
        }

        public static void Error(Exception ex)
        {
            var logEventInfo = new LogEventInfo(LogLevel.Error, Instance.Name, null, ex.Message, null, ex);
            Instance.Log(typeof(Logger), logEventInfo);
        }

        public static void Info(string message, params object[] args)
        {
            var logEventInfo = new LogEventInfo(LogLevel.Info, Instance.Name, null, message, args);
            Instance.Log(typeof(Logger), logEventInfo);
        }
    }
}
