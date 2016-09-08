using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace SimpleRenamer.Framework
{
    public class SettingsFactory : ISettingsFactory
    {
        public Settings GetSettings()
        {
            Settings mySettings = new Settings();
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            mySettings.SubDirectories = bool.Parse(configuration.AppSettings.Settings["SubDirectories"].Value);
            mySettings.RenameFiles = bool.Parse(configuration.AppSettings.Settings["RenameFiles"].Value);
            mySettings.CopyFiles = bool.Parse(configuration.AppSettings.Settings["CopyFiles"].Value);
            mySettings.NewFileNameFormat = configuration.AppSettings.Settings["NewFileNameFormat"].Value;
            mySettings.ValidExtensions = new List<string>(configuration.AppSettings.Settings["ValidExtensions"].Value.Split(new char[] { ';' }));
            mySettings.WatchFolders = new List<string>(configuration.AppSettings.Settings["WatchFolders"].Value.Split(new char[] { ';' }));
            mySettings.DestinationFolder = configuration.AppSettings.Settings["DestinationFolder"].Value;

            return mySettings;
        }

        public void SaveSettings(Settings settings)
        {
            throw new NotImplementedException();
        }
    }
}
