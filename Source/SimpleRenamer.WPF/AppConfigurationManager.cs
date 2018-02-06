using Newtonsoft.Json;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Sarjee.SimpleRenamer.Framework.Core
{
    /// <summary>
    /// App Configuration Manager
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Interface.IConfigurationManager" />
    public class AppConfigurationManager : IConfigurationManager
    {
        private IgnoreList ignoredFiles;
        private RegexFile regexExpressions;
        private ISettings settings;
        private ShowNameMapping showNameMapping;
        private JsonSerializer jsonSerializer = new JsonSerializer();

        /// <summary>
        /// Gets or sets the ignored files.
        /// </summary>
        /// <value>
        /// The ignored files.
        /// </value>
        public IgnoreList IgnoredFiles
        {
            get
            {
                if (ignoredFiles == null)
                {
                    ignoredFiles = ReadIgnoreList();
                }
                return ignoredFiles;
            }

            set
            {
                ignoredFiles = value;
            }
        }
        private IgnoreList ReadIgnoreList()
        {
            IgnoreList ignoreList = new IgnoreList();
            //if the file doesn't yet exist then set a new version
            if (!File.Exists(IgnoreListFilePath))
            {
                return ignoreList;
            }
            else
            {
                using (StreamReader file = File.OpenText(IgnoreListFilePath))
                {
                    ignoreList = (IgnoreList)jsonSerializer.Deserialize(file, typeof(IgnoreList));
                }
                return ignoreList;
            }
        }

        private void WriteIgnoreListAsync(IgnoreList ignoreList)
        {
            //only write the file if there is data
            if (ignoreList != null && ignoreList.IgnoreFiles.Count > 0)
            {
                using (StreamWriter file = File.CreateText(IgnoreListFilePath))
                {
                    jsonSerializer.Serialize(file, ignoreList);
                }
            }
        }

        private string IgnoreListFilePath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "IgnoreFileList.json");
            }
        }

        /// <summary>
        /// Gets or sets the regex expressions.
        /// </summary>
        /// <value>
        /// The regex expressions.
        /// </value>
        public RegexFile RegexExpressions
        {
            get
            {
                if (regexExpressions == null)
                {
                    regexExpressions = ReadExpressionFile();
                }
                return regexExpressions;
            }
            set
            {
                regexExpressions = value;
            }
        }

        private RegexFile ReadExpressionFile()
        {
            RegexFile regexFile = new RegexFile();
            //if the file doesn't yet exist then set a new version
            if (!File.Exists(RegexFilePath))
            {
                return regexFile;
            }
            else
            {
                using (StreamReader file = File.OpenText(RegexFilePath))
                {
                    regexFile = (RegexFile)jsonSerializer.Deserialize(file, typeof(RegexFile));
                }
                return regexFile;
            }
        }

        private void WriteExpressionFile(RegexFile regexMatchers)
        {
            //only write the file if there is data
            if (regexMatchers != null && regexMatchers.RegexExpressions.Count > 0)
            {
                using (StreamWriter file = File.CreateText(RegexFilePath))
                {
                    jsonSerializer.Serialize(file, regexMatchers);
                }
            }
        }

        private string RegexFilePath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "RegexExpressions.json");
            }
        }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        public ISettings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = new Settings();
                }
                return settings;
            }

            set
            {
                settings = value;
            }
        }

        protected virtual Settings GetSettings()
        {
            Settings mySettings = new Settings();
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            mySettings.SubDirectories = bool.Parse(configuration.AppSettings.Settings["SubDirectories"].Value);
            mySettings.RenameFiles = bool.Parse(configuration.AppSettings.Settings["RenameFiles"].Value);
            mySettings.CopyFiles = bool.Parse(configuration.AppSettings.Settings["CopyFiles"].Value);
            mySettings.NewFileNameFormat = configuration.AppSettings.Settings["NewFileNameFormat"].Value;
            List<string> extensions = new List<string>(configuration.AppSettings.Settings["ValidExtensions"].Value.Split(';'));
            extensions.Remove("");
            mySettings.ValidExtensions = extensions;
            List<string> folders = new List<string>(configuration.AppSettings.Settings["WatchFolders"].Value.Split(';'));
            folders.Remove("");
            mySettings.WatchFolders = folders;
            mySettings.DestinationFolderTV = configuration.AppSettings.Settings["DestinationFolderTV"].Value;
            mySettings.DestinationFolderMovie = configuration.AppSettings.Settings["DestinationFolderMovie"].Value;

            return mySettings;
        }

        private string ShowNameMappingFilePath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "SelectedShowMapping.json");
            }
        }

        /// <summary>
        /// Gets or sets the show name mappings.
        /// </summary>
        /// <value>
        /// The show name mappings.
        /// </value>
        public ShowNameMapping ShowNameMappings
        {
            get
            {
                if (showNameMapping == null)
                {
                    showNameMapping = ReadMappingFile();
                }
                return showNameMapping;
            }
            set
            {
                showNameMapping = value;
            }
        }

        private ShowNameMapping ReadMappingFile()
        {
            ShowNameMapping snm = new ShowNameMapping();
            //if the file doesn't yet exist then set a new version
            if (!File.Exists(ShowNameMappingFilePath))
            {
                return snm;
            }
            else
            {
                using (StreamReader file = File.OpenText(ShowNameMappingFilePath))
                {
                    snm = (ShowNameMapping)jsonSerializer.Deserialize(file, typeof(ShowNameMapping));
                }
                return snm;
            }
        }

        private void WriteMappingFile(ShowNameMapping showNameMapping)
        {
            //only write the file if there is data
            if (showNameMapping != null && showNameMapping.Mappings.Count > 0)
            {
                using (StreamWriter file = File.CreateText(ShowNameMappingFilePath))
                {
                    jsonSerializer.Serialize(file, showNameMapping);
                }
            }
        }

        /// <summary>
        /// Saves the configuration.
        /// </summary>
        public void SaveConfiguration()
        {
            SaveConfig();
        }
        protected virtual void SaveConfig()
        {
            WriteMappingFile(this.ShowNameMappings);
            WriteIgnoreListAsync(this.IgnoredFiles);
            WriteExpressionFile(this.RegexExpressions);
            //SaveSettings(this.Settings);
        }

        private void SaveSettings(ISettings settings)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings["SubDirectories"].Value = settings.SubDirectories.ToString();
            configuration.AppSettings.Settings["RenameFiles"].Value = settings.RenameFiles.ToString();
            configuration.AppSettings.Settings["CopyFiles"].Value = settings.CopyFiles.ToString();
            configuration.AppSettings.Settings["NewFileNameFormat"].Value = settings.NewFileNameFormat;
            string validExtensions = string.Empty;
            foreach (string valid in settings.ValidExtensions)
            {
                validExtensions += valid + ";";
            }
            configuration.AppSettings.Settings["ValidExtensions"].Value = validExtensions.TrimEnd(';');
            string watchFolders = string.Empty;
            foreach (string folder in settings.WatchFolders)
            {
                watchFolders += folder + ";";
            }
            configuration.AppSettings.Settings["WatchFolders"].Value = watchFolders.TrimEnd(';');
            configuration.AppSettings.Settings["DestinationFolderTV"].Value = settings.DestinationFolderTV;
            configuration.AppSettings.Settings["DestinationFolderMovie"].Value = settings.DestinationFolderMovie;
            configuration.Save(ConfigurationSaveMode.Modified);
        }

        /// <summary>
        /// Gets the The TV Database API key.
        /// </summary>
        /// <value>
        /// The TV database API key.
        /// </value>
        public string TvDbApiKey
        {
            get
            {
                return "820147144A5BB54E";
            }
        }

        /// <summary>
        /// Gets the The Movie Database API key.
        /// </summary>
        /// <value>
        /// The Movie database API key.
        /// </value>
        public string TmDbApiKey
        {
            get
            {
                return "e9b955f1140da97e65df7e1bce1780bc";
            }
        }

        /// <summary>
        /// Gets the one true error URL.
        /// </summary>
        /// <value>
        /// The one true error URL.
        /// </value>
        public string OneTrueErrorUrl
        {
            get
            {
                return "https://jsote.uksouth.cloudapp.azure.com/OneTrueError/";
            }
        }

        /// <summary>
        /// Gets the one true error application key.
        /// </summary>
        /// <value>
        /// The one true error application key.
        /// </value>
        public string OneTrueErrorApplicationKey
        {
            get
            {
                return "d574b9a210704b1ba1d75bb70442e173";
            }
        }

        /// <summary>
        /// Gets the one true error shared secret.
        /// </summary>
        /// <value>
        /// The one true error shared secret.
        /// </value>
        public string OneTrueErrorSharedSecret
        {
            get
            {
                return "696c4b84e8d04a8c99cbc17570b1cc05";
            }
        }
    }
}
