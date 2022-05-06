using Com.GGIT.LogLib;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Rmq.Core.Common
{
    public class CommUtil
    {
        #region general
        public static void PrintLoggerConnection(string type, IConnection Connection)
        {
            string header = "============ " + type + " Connection Information [" + Connection.Endpoint + "] ==================";
            string footer = string.Empty;
            for (int i = 0; i < header.Length; i++) footer += "=";
            SingletonLogger.Info(header);
            SingletonLogger.Info("Connection status ? " + (Connection.IsOpen ? "Open" : "Closed"));
            SingletonLogger.Info("Connection Heartbeat/Port Number = " + Connection.Heartbeat + "/" + Connection.LocalPort);
            SingletonLogger.Info(footer);
        }

        public static string ErrorMessage(int code, string message) => "Failed to process; reason = (" + code + ")" + message;
        public static string ValidationMessage(int code, string message) => "Validation failed; reason = (" + code + ")" + message;
        #endregion

        #region encode / decode
        public static byte[] EncodeMessage(string message) => Encoding.UTF8.GetBytes(message);
        public static string DecodeMessage(byte[] message) => Encoding.UTF8.GetString(message);
        #endregion

        #region serialize / deserialize
        public static string SerializeToString(object value) => JsonConvert.SerializeObject(value);
        public static T DeserializeToObject<T>(string value) => JsonConvert.DeserializeObject<T>(value);
        #endregion
    }
}
