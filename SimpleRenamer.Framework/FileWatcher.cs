using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework
{
    public class FileWatcher : IFileWatcher
    {
        private ILogger logger;
        private IIgnoreListFramework ignoreListFramework;
        private Settings settings;

        public FileWatcher(ILogger log, IIgnoreListFramework ignoreFramework, ISettingsFactory settingFactory)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            if (ignoreFramework == null)
            {
                throw new ArgumentNullException(nameof(ignoreFramework));
            }
            if (settingFactory == null)
            {
                throw new ArgumentNullException(nameof(settingFactory));
            }
            logger = log;
            ignoreListFramework = ignoreFramework;
            settings = settingFactory.GetSettings();
        }
        public async Task<List<string>> SearchTheseFoldersAsync(CancellationToken ct)
        {
            logger.TraceMessage("SearchTheseFoldersAsync - Start");
            List<string> foundFiles = new List<string>();
            IgnoreList ignoreList = await ignoreListFramework.ReadIgnoreListAsync();

            //FOR EACH WATCH FOLDER
            foreach (string folder in settings.WatchFolders)
            {
                //if the directory exists and contains at least 1 file (search sub directories if settings allow) -- limitation of searchPattern means we can't filter video extensions here
                if (Directory.Exists(folder) && Directory.GetFiles(folder, "*", settings.SubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).Length > 0)
                {
                    //search the folder for files with video extensions
                    List<string> temp = SearchThisFolder(folder, ignoreList);
                    //if we find any files here add to the global list
                    if (temp.Count > 0)
                    {
                        foundFiles.AddRange(temp);
                    }
                }
                //throw exception if cancel requested
                ct.ThrowIfCancellationRequested();
            }

            logger.TraceMessage("SearchTheseFoldersAsync - End");
            return foundFiles;
        }

        /// <summary>
        /// Searches a given folder for all video files
        /// </summary>
        /// <param name="dir">The folder to search</param>
        /// <param name="settings">Our current settings</param>
        /// <returns></returns>
        private List<string> SearchThisFolder(string dir, IgnoreList ignoreList)
        {
            logger.TraceMessage("SearchThisFolder - Start");
            List<string> foundFiles = new List<string>();
            foreach (string file in Directory.GetFiles(dir, "*", settings.SubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                if (IsValidExtension(Path.GetExtension(file)) && !ignoreList.IgnoreFiles.Contains(file))
                {
                    foundFiles.Add(file);
                }
            }

            logger.TraceMessage("SearchThisFolder - End");
            return foundFiles;
        }

        /// <summary>
        /// Returns true if the input extension is one of our valid extensions
        /// </summary>
        /// <param name="input">The input extension</param>
        /// <param name="settings">Our current settings</param>
        /// <returns></returns>
        private bool IsValidExtension(string input)
        {
            logger.TraceMessage("SearchThisFolderIsValidExtension - Start");
            foreach (string extension in settings.ValidExtensions)
            {
                if (input.ToLowerInvariant().Equals(extension.ToLowerInvariant()))
                {
                    logger.TraceMessage("SearchThisFolderIsValidExtension - True");
                    return true;
                }
            }

            logger.TraceMessage("SearchThisFolderIsValidExtension - False");
            return false;
        }
    }
}
