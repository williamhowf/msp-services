using Com.GGIT.Enumeration;
using Com.GGIT.Infrastructure;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Com.GGIT.Database.Settings
{
    /// <summary>
    /// Manager of data settings (connection string)
    /// </summary>
    public partial class DataSettingsManager
    {
        #region Properties
        /// <summary>
        /// Gets the path to file that contains data settings
        /// </summary>
        public static string DataSettingsFilePath { get; } = string.Concat(AppDomain.CurrentDomain.BaseDirectory, PathEnum.DatabasePath);
        #endregion

        #region Methods
        /// <summary>
        /// Load settings
        /// </summary>
        /// <param name="filePath">File path; pass null to use default settings file path</param>
        /// <param name="reloadSettings">Indicates whether to reload data, if they already loaded</param>
        /// <returns>Data settings</returns>
        public virtual DataSettings LoadSettings(string filePath = null, bool reloadSettings = false)
        {
            if (!reloadSettings && Singleton<DataSettings>.Instance != null)
                return Singleton<DataSettings>.Instance;

            filePath ??= DataSettingsFilePath;

            var text = File.ReadAllText(filePath);

            if (string.IsNullOrEmpty(text))
                return new DataSettings();

            //get data settings from the JSON file
            Singleton<DataSettings>.Instance = JsonConvert.DeserializeObject<DataSettings>(text);

            return Singleton<DataSettings>.Instance;
        }

        /// <summary>
        /// Save settings to a file
        /// </summary>
        /// <param name="settings">Data settings</param>
        public virtual void SaveSettings(DataSettings settings)
        {
            Singleton<DataSettings>.Instance = settings ?? throw new ArgumentNullException(nameof(settings));

            var filePath = DataSettingsFilePath;

            //create file if not exists
            if (!File.Exists(filePath))
            {
                //we use 'using' to close the file after it's created
                using (File.Create(filePath)) { }
            }

            //save data settings to the file
            var text = JsonConvert.SerializeObject(Singleton<DataSettings>.Instance, Formatting.Indented);
            File.WriteAllText(filePath, text);
        }
        #endregion
    }
}
