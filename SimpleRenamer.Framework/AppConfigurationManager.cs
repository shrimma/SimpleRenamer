using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;

namespace SimpleRenamer.Framework
{
    public class AppConfigurationManager : IConfigurationManager
    {
        private IgnoreList ignoredFiles;
        private RegexFile regexExpressions;
        private Settings settings;
        private ShowNameMapping showNameMapping;

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
                using (FileStream fs = new FileStream(IgnoreListFilePath, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(IgnoreList));
                    ignoreList = (IgnoreList)serializer.Deserialize(fs);
                }
                return ignoreList;
            }
        }

        private void WriteIgnoreListAsync(IgnoreList ignoreList)
        {
            //only write the file if there is data
            if (ignoreList != null && ignoreList.IgnoreFiles.Count > 0)
            {
                using (TextWriter writer = new StreamWriter(IgnoreListFilePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(IgnoreList));
                    serializer.Serialize(writer, ignoreList);
                }

            }
        }

        public string IgnoreListFilePath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "IgnoreFileList.xml");
            }
        }


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
                using (FileStream fs = new FileStream(RegexFilePath, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(RegexFile));
                    regexFile = (RegexFile)serializer.Deserialize(fs);
                }
                return regexFile;
            }
        }

        private void WriteExpressionFile(RegexFile regexMatchers)
        {
            //only write the file if there is data
            if (regexMatchers != null && regexMatchers.RegexExpressions.Count > 0)
            {
                using (TextWriter writer = new StreamWriter(RegexFilePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(RegexFile));
                    serializer.Serialize(writer, regexMatchers);
                }
            }
        }

        public string RegexFilePath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "RegexExpressions.xml");
            }
        }

        public Settings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = GetSettings();
                }
                return settings;
            }

            set
            {
                settings = value;
            }
        }

        private Settings GetSettings()
        {
            Settings mySettings = new Settings();
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            mySettings.SubDirectories = bool.Parse(configuration.AppSettings.Settings["SubDirectories"].Value);
            mySettings.RenameFiles = bool.Parse(configuration.AppSettings.Settings["RenameFiles"].Value);
            mySettings.CopyFiles = bool.Parse(configuration.AppSettings.Settings["CopyFiles"].Value);
            mySettings.NewFileNameFormat = configuration.AppSettings.Settings["NewFileNameFormat"].Value;
            mySettings.ValidExtensions = new List<string>(configuration.AppSettings.Settings["ValidExtensions"].Value.Split(new char[] { ';' }));
            mySettings.WatchFolders = new List<string>(configuration.AppSettings.Settings["WatchFolders"].Value.Split(new char[] { ';' }));
            mySettings.DestinationFolderTV = configuration.AppSettings.Settings["DestinationFolderTV"].Value;
            mySettings.DestinationFolderMovie = configuration.AppSettings.Settings["DestinationFolderMovie"].Value;

            return mySettings;
        }

        public string ShowNameMappingFilePath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "SelectedShowMapping.xml");
            }
        }

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
                using (FileStream fs = new FileStream(ShowNameMappingFilePath, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ShowNameMapping));
                    snm = (ShowNameMapping)serializer.Deserialize(fs);
                }
                return snm;
            }
        }

        private void WriteMappingFile(ShowNameMapping showNameMapping)
        {
            //only write the file if there is data
            if (showNameMapping != null && showNameMapping.Mappings.Count > 0)
            {
                using (TextWriter writer = new StreamWriter(ShowNameMappingFilePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ShowNameMapping));
                    serializer.Serialize(writer, showNameMapping);
                }
            }
        }

        public void SaveConfiguration()
        {
            WriteMappingFile(this.ShowNameMappings);
            WriteIgnoreListAsync(this.IgnoredFiles);
            WriteExpressionFile(this.RegexExpressions);
            SaveSettings(this.Settings);
        }

        private void SaveSettings(Settings settings)
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

        public string TvDbApiKey
        {
            get
            {
                return "820147144A5BB54E";
            }
        }

        public string TmDbApiKey
        {
            get
            {
                return "e9b955f1140da97e65df7e1bce1780bc";
            }
        }
    }
}
