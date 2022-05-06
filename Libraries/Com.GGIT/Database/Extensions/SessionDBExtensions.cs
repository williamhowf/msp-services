using NHibernate;

namespace Com.GGIT.Database.Extensions
{
    public static class SessionDBExtensions
    {
        /// <summary>
        /// (INSERT) When session is not sync with the database, we need to flush the connection.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object SaveTransaction(this ISession session, object obj)
        {
            try { if (session.IsDirty()) session.Flush(); } catch { }
            return session.Save(obj);
        }

        /// <summary>
        /// (UPDATE) When session is not sync with the database, we need to flush the connection.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="obj"></param>
        public static void UpdateTransaction(this ISession session, object obj)
        {
            try { if (session.IsDirty()) session.Flush(); } catch { }
            session.Update(obj);
        }

        /// <summary>
        /// (SAVE/UPDATE) When session is not sync with the database, we need to flush the connection.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="obj"></param>
        public static void SaveUpdateTransaction(this ISession session, object obj)
        {
            try { if (session.IsDirty()) session.Flush(); } catch { }
            session.SaveOrUpdate(obj);
        }
    }
}
