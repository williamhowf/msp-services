using Com.GGIT.Common.Security;
using Com.GGIT.Enumeration;
using FluentNHibernate.Cfg.Db;
using System.Collections.Generic;

namespace Com.GGIT.Database.Settings
{
    /// <summary>
    /// Data settings (connection string information)
    /// </summary>
    public partial class DataSettings
    {
        private string _DataConnection;
        private SqlDriverEnum _SqlDriverEnum;

        /// <summary>
        /// Ctor
        /// </summary>
        public DataSettings()
        {
            RawDataSettings = new Dictionary<string, string>();
        }
        public string EncryptedDataConnectionString { get; set; }

        public bool Encrypted { get; set; }

        public class Properties
        {
            /// <summary>
            /// Command timeout
            /// </summary>
            public string CommandTimeout { get; set; }

            /// <summary>
            /// Batch size
            /// </summary>
            public string BatchSize { get; set; }

            /// <summary>
            /// Batch Order Insert
            /// </summary>
            public string BatchOrderInsert { get; set; }

            /// <summary>
            /// Batch Order Update
            /// </summary>
            public string BatchOrderUpdate { get; set; }

            /// <summary>
            /// Show Sql
            /// </summary>
            public string ShowSql { get; set; }

            /// <summary>
            /// Show Sql
            /// </summary>
            public string FormatSql { get; set; }
        }

        /// <summary>
        /// Data provider
        /// </summary>
        public string DataProvider { get; set; }

        /// <summary>
        /// Connection string
        /// </summary>
        public string DataConnectionString
        {
            get { return _DataConnection; }
            set
            {
                if (Encrypted) _DataConnection = new Cryptography().TripleDES_Decryptor(EncryptedDataConnectionString);
                else _DataConnection = value;
            }
        }

        /// <summary>
        /// Raw settings file
        /// </summary>
        public IDictionary<string, string> RawDataSettings { get; }

        public virtual SqlDriverEnum SqlDriverEnum
        {
            get { return _SqlDriverEnum; }
            set { _SqlDriverEnum = value; }
        }

        public string SqlDriver
        {
            get { return _SqlDriverEnum.ToValue(); }
            set { _SqlDriverEnum = EnumExtensions.GetEnumFromValue<SqlDriverEnum>(value); }
        }

        /// <summary>
        /// Properties
        /// </summary>
        public Properties Property { get; set; }

        /// <summary>
        /// A value indicating whether entered information is valid
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(this.DataProvider) && !string.IsNullOrEmpty(this.DataConnectionString);
        }

        public MsSqlConfiguration GetMsSqlDriver()
        {
            MsSqlConfiguration configuration = _SqlDriverEnum switch
            {
                SqlDriverEnum.MsSql2017 => MsSqlConfiguration.MsSql7,
                SqlDriverEnum.MsSql2008 => MsSqlConfiguration.MsSql2008,
                SqlDriverEnum.MsSql2012 => MsSqlConfiguration.MsSql2012,
                _ => throw new KeyNotFoundException(),
            };
            return configuration;
        }

        public PostgreSQLConfiguration GetPostgreSQLDriver()
        {
            return PostgreSQLConfiguration.Standard;
        }

        public dynamic GetSqlDriver()
        {
            dynamic configuration = _SqlDriverEnum switch
            {
                SqlDriverEnum.MsSql2017 => MsSqlConfiguration.MsSql7,
                SqlDriverEnum.MsSql2008 => MsSqlConfiguration.MsSql2008,
                SqlDriverEnum.MsSql2012 => MsSqlConfiguration.MsSql2012,
                SqlDriverEnum.PostgreSqlStandard => PostgreSQLConfiguration.Standard,
                SqlDriverEnum.PostgreSql81 => PostgreSQLConfiguration.PostgreSQL81,
                SqlDriverEnum.PostgreSql82 => PostgreSQLConfiguration.PostgreSQL82,
                _ => throw new KeyNotFoundException(),
            };
            return configuration;
        }
    }
}
