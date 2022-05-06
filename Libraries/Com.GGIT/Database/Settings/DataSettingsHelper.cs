using FluentNHibernate.Cfg;
using NHibernate;
using NConfig = NHibernate.Cfg;
using System;
using NLog;
using System.Reflection;
using NHibernate.Cache;
using Com.GGIT.Common;

namespace Com.GGIT.Database.Settings
{
    /// <summary>
    /// Data settings helper
    /// </summary>
    public partial class DataSettingsHelper
    {
        private static Logger _log;
        protected static Logger Log => _log ?? (_log = LogManager.GetCurrentClassLogger()); // print log into general logger

        private static bool? _databaseIsInstalled;
        private static DataSettings dataSettings;
        private static ISessionFactory sessionFactory;

        /// <summary>
        /// Returns a value indicating whether database is already installed
        /// </summary>
        /// <returns></returns>
        public static bool DatabaseIsInstalled()
        {
            if (!_databaseIsInstalled.HasValue)
            {
                dataSettings = new DataSettingsManager().LoadSettings(reloadSettings: true);
                _databaseIsInstalled = dataSettings != null && !string.IsNullOrEmpty(dataSettings.DataConnectionString);

                GetSessionFactory(); // initialize nhibernate factory
                if (sessionFactory == null) return false;
            }
            return _databaseIsInstalled.Value;
        }

        public static DataSettings GetDataSettings()
        {
            if (dataSettings == null)
                dataSettings = new DataSettingsManager().LoadSettings(reloadSettings: true);

            return dataSettings;
        }

        /// <summary>
        /// Reset information cached
        /// </summary>
        public static void ResetCache()
        {
            _databaseIsInstalled = null;
            sessionFactory = null;
        }

        /// <summary>
        /// Retrieve NHibernate Factory Configuration
        /// </summary>
        /// <returns></returns>
        public static ISessionFactory GetSessionFactory()
        {
            if (sessionFactory == null)
            {
                var settings = GetDataSettings();  //require System.Data.SqlClient library
                try
                {
                    sessionFactory = Fluently.Configure()
                        //configure database connection and driver
                        .Database(settings.GetMsSqlDriver().ConnectionString(settings.DataConnectionString))
                        //2nd level cache
                        .Cache(c => c.UseQueryCache()
                                .UseSecondLevelCache()
                                .ProviderClass<HashtableCacheProvider>())
                        //additional configuration
                        .ExposeConfiguration(f =>
                        {
                            f.SetProperty(NConfig.Environment.BatchSize, settings.Property.BatchSize);
                            f.SetProperty(NConfig.Environment.CommandTimeout, settings.Property.CommandTimeout);
                            f.SetProperty(NConfig.Environment.OrderInserts, settings.Property.BatchOrderInsert);
                            f.SetProperty(NConfig.Environment.OrderUpdates, settings.Property.BatchOrderUpdate);
                            f.SetProperty(NConfig.Environment.ShowSql, settings.Property.ShowSql);
                            f.SetProperty(NConfig.Environment.FormatSql, settings.Property.FormatSql);
                        })
                        //find/set the mappings
                        .Mappings(m => m.FluentMappings
                                .AddFromAssembly(Assembly.GetExecutingAssembly()))
                        .BuildSessionFactory();
                }
                catch (Exception ex)
                {
                    Log.Error("Failed to initialize database connection");
                    Log.Error(ex.ExceptionToString());
                }
            }
            return sessionFactory;
        }
    }
}
