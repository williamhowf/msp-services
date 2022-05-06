using NHibernate;

namespace Com.GGIT.Database
{
    public interface ISessionDB
    {
        public ISession OpenSession();
        public void CloseSessions();
    }
}