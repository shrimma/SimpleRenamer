using SimpleRenamer.Framework.DataModel;

namespace SimpleRenamer.Framework.Interface
{
    public interface ISettingsFactory
    {
        /// <summary>
        /// Gets the application settings
        /// </summary>
        /// <returns>Populated Settings object</returns>
        Settings GetSettings();

        /// <summary>
        /// Saves the application settings
        /// </summary>
        /// <param name="settings">Settings</param>
        void SaveSettings(Settings settings);
    }
}
