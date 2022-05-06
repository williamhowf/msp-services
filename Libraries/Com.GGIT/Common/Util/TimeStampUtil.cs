using Com.GGIT.Database;
using Com.GGIT.Database.Domain;
using Com.GGIT.Database.Extensions;
using Com.GGIT.LogLib;
using System;

namespace Com.GGIT.Common.Util
{
    public class TimeStampUtil : IDisposable
    {
        private SessionDB sessionFactory;

        public TimeStampUtil()
        {
            sessionFactory = new SessionDB();
        }

        public void Dispose()
        {
            if (sessionFactory != null) sessionFactory.CloseSessions();
        }

        #region Insert MSP_MonitorServices_LastActivityLog
        public void InsertLastActivityLogTimestamp(string serviceName) //voonkeong 20201124 MDT-1756
        {
            try
            {
                MSP_MonitorServices_LastActivityLog LastActivityLogRecord = new MSP_MonitorServices_LastActivityLog
                {
                    ServiceName = serviceName,
                    CreatedOnUtc = DateTime.UtcNow
                };

                if (sessionFactory.IsNull() || sessionFactory.Disconnected()) 
                    sessionFactory = new SessionDB();

                using var db = sessionFactory.OpenSession();
                db.SaveTransaction(LastActivityLogRecord);
                db.Close();
            }
            catch (Exception ex)
            {
                SingletonLogger.Error(ex.ExceptionToString());
            }
        }
        #endregion
    }
}
