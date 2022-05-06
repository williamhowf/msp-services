using Com.GGIT.Enumeration;
using Com.GGIT.Infrastructure;
using Newtonsoft.Json;
using NLog;
using System;
using System.IO;
using System.Text;

namespace Com.GGIT.Security.JsonWebToken
{
    public class JwtManager : IDisposable
    {
        private Logger _log;
        protected Logger Log => _log ?? (_log = LogManager.GetCurrentClassLogger());

        #region Properties
        public static string KeysFilePath { get; } = string.Concat(AppDomain.CurrentDomain.BaseDirectory, PathEnum.JwtPublicKeyPath);

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Methods
        public virtual JwtKeys LoadKeys(string filepath = null, bool reloadSettings = false)
        {
            if (!reloadSettings && Singleton<JwtKeys>.Instance != null)
                return Singleton<JwtKeys>.Instance;

            string jsoncontent = string.Empty;
            filepath ??= Path.GetFullPath(KeysFilePath);
            try
            {
                using var reader = new StringReader(File.ReadAllText(filepath, Encoding.UTF8));
                jsoncontent = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            Singleton<JwtKeys>.Instance = JsonConvert.DeserializeObject<JwtKeys>(jsoncontent);
            return Singleton<JwtKeys>.Instance;
        }
        #endregion
    }
}
