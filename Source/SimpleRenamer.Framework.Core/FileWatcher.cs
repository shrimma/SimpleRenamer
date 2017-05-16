using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Framework.Core
{
    public class FileWatcher : IFileWatcher
    {
        private ILogger _logger;
        private IConfigurationManager _configurationManager;
        private Settings settings;
        private IgnoreList ignoreList;
        public event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;

        public FileWatcher(ILogger logger, IConfigurationManager configManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configurationManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
            settings = _configurationManager.Settings;
        }

        public async Task<List<string>> SearchTheseFoldersAsync(CancellationToken ct)
        {
            _logger.TraceMessage("SearchTheseFoldersAsync - Start");
            List<string> foundFiles = new List<string>();
            //grab the list of ignored files

            ignoreList = _configurationManager.IgnoredFiles;
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
            _logger.TraceMessage("SearchTheseFoldersAsync - End");

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
            _logger.TraceMessage("SearchThisFolder - Start");
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

            _logger.TraceMessage("SearchThisFolder - End");
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
            _logger.TraceMessage("SearchThisFolderIsValidExtension - Start");
            foreach (string extension in settings.ValidExtensions)
            {
                if (input.ToLowerInvariant().Equals(extension.ToLowerInvariant()))
                {
                    _logger.TraceMessage("SearchThisFolderIsValidExtension - True");
                    return true;
                }
            }

            _logger.TraceMessage("SearchThisFolderIsValidExtension - False");
            return false;
        }
    }
}
