using SimpleRenamer.Common.EventArguments;
using SimpleRenamer.Common.Interface;
using SimpleRenamer.Common.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Core
{
    public class FileWatcher : IFileWatcher
    {
        private ILogger logger;
        private IConfigurationManager configurationManager;
        private Settings settings;
        private IgnoreList ignoreList;
        public event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;

        public FileWatcher(ILogger log, IConfigurationManager configManager)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            if (configManager == null)
            {
                throw new ArgumentNullException(nameof(configManager));
            }
            logger = log;
            configurationManager = configManager;
            settings = configurationManager.Settings;
        }

        public async Task<List<string>> SearchTheseFoldersAsync(CancellationToken ct)
        {
            logger.TraceMessage("SearchTheseFoldersAsync - Start");
            List<string> foundFiles = new List<string>();
            //grab the list of ignored files

            ignoreList = configurationManager.IgnoredFiles;
            //FOR EACH WATCH FOLDER
            foreach (string folder in settings.WatchFolders)
            {
                RaiseProgressEvent(this, new ProgressTextEventArgs($"Searching watch folder for video files: {folder}"));
                //if the directory exists and contains at least 1 file (search sub directories if settings allow) -- limitation of searchPattern means we can't filter video extensions here
                if (Directory.Exists(folder) && Directory.GetFiles(folder, "*", settings.SubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).Length > 0)
                {
                    //search the folder for files with video extensions
                    List<string> temp = SearchThisFolder(folder, ct);
                    //if we find any files here add to the global list
                    if (temp.Count > 0)
                    {
                        foundFiles.AddRange(temp);
                    }
                }
                //throw exception if cancel requested
                ct.ThrowIfCancellationRequested();
            }

            RaiseProgressEvent(this, new ProgressTextEventArgs($"Searched all watch folders for video files"));
            logger.TraceMessage("SearchTheseFoldersAsync - End");

            return foundFiles;
        }

        /// <summary>
        /// Searches a given folder for all video files
        /// </summary>
        /// <param name="dir">The folder to search</param>
        /// <param name="settings">Our current settings</param>
        /// <returns></returns>
        private List<string> SearchThisFolder(string dir, CancellationToken ct)
        {
            logger.TraceMessage("SearchThisFolder - Start");
            List<string> foundFiles = new List<string>();
            foreach (string file in Directory.GetFiles(dir, "*", settings.SubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                //is a valid extension, is not ignored and isn't a sample
                if (IsValidExtension(Path.GetExtension(file)) && !ignoreList.IgnoreFiles.Contains(file) && !Path.GetFileName(file).Contains("*.sample.*") && !Path.GetFileName(file).Contains("*.Sample.*"))
                {
                    foundFiles.Add(file);
                }
                ct.ThrowIfCancellationRequested();
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
