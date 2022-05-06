using Com.GGIT.Database.Settings;
using NHibernate;

namespace Com.GGIT.Database
{
    public class SessionDB : ISessionDB
    {
        private ISessionFactory sessionFactory;

        public SessionDB() => InitFactory();

        private void InitFactory() => sessionFactory = DataSettingsHelper.GetSessionFactory();

        public ISession OpenSession() => sessionFactory.OpenSession();

        public void CloseSessions() => sessionFactory.Close();

        public bool Disconnected() => sessionFactory.IsClosed;
    }
}
