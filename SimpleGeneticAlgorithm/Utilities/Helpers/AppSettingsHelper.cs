namespace Utilities.Helpers
{
    using System.Configuration;

    /// <summary>
    /// Contains methods to manipulate the app.config file in code.
    /// </summary>
    public static class AppSettingsHelper
    {
        /// <summary>
        /// Updates the app settings file with a given key value pair.
        /// </summary>
        /// <param name="key">Key to be updated with a new value.</param>
        /// <param name="value">Value to be placed into the config.</param>
        public static void UpdateAppSettings(string key, string value)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            settings[key].Value = value;

            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }
    }
}
