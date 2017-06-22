using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Framework.Core
{
    /// <summary>
    /// File Watcher
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Interface.IFileWatcher" />
    public class FileWatcher : IFileWatcher
    {
        private ILogger _logger;
        private IConfigurationManager _configurationManager;
        private Settings settings;
        private IgnoreList ignoreList;
        private ParallelOptions _parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };

        /// <summary>
        /// Fired whenever some noticeable progress is made
        /// </summary>
        public event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileWatcher"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="configManager">The configuration manager.</param>
        /// <exception cref="System.ArgumentNullException">
        /// logger
        /// or
        /// configManager
        /// </exception>
        public FileWatcher(ILogger logger, IConfigurationManager configManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configurationManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
            settings = _configurationManager.Settings;
        }

        /// <summary>
        /// Searches the configured folders for any video files
        /// </summary>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>
        /// A list of file paths of video files
        /// </returns>
        public async Task<List<string>> SearchFoldersAsync(CancellationToken ct)
        {
            _logger.TraceMessage("SearchTheseFoldersAsync - Start");
            List<string> foundFiles = new List<string>();
            //grab the list of ignored files

            ignoreList = _configurationManager.IgnoredFiles;
            //FOR EACH WATCH FOLDER
            foreach (string folder in settings.WatchFolders)
            {
                //throw exception if cancel requested
                ct.ThrowIfCancellationRequested();
                RaiseProgressEvent(this, new ProgressTextEventArgs($"Searching watch folder for video files: {folder}"));
                //if the directory exists and contains at least 1 file (search sub directories if settings allow) -- limitation of searchPattern means we can't filter video extensions here
                if (Directory.Exists(folder) && Directory.GetFiles(folder, "*", settings.SubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).Length > 0)
                {
                    //search the folder for files with video extensions
                    List<string> temp = await SearchThisFolder(folder, ct);
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
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        private async Task<List<string>> SearchThisFolder(string dir, CancellationToken ct)
        {
            _logger.TraceMessage("SearchThisFolder - Start");
            ConcurrentBag<string> foundFiles = new ConcurrentBag<string>();
            _parallelOptions.CancellationToken = ct;
            Task result = Task.Run(() => Parallel.ForEach(Directory.GetFiles(dir, "*", settings.SubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly), _parallelOptions, (file) =>
            {
                ct.ThrowIfCancellationRequested();
                //is a valid extension, is not ignored and isn't a sample
                if (IsValidExtension(Path.GetExtension(file)) && !ignoreList.IgnoreFiles.Contains(file) && !Path.GetFileName(file).Contains("*.sample.*") && !Path.GetFileName(file).Contains("*.Sample.*"))
                {
                    foundFiles.Add(file);
                }
            }));

            await result;

            _logger.TraceMessage("SearchThisFolder - End");
            return foundFiles.ToList();
        }

        /// <summary>
        /// Returns true if the input extension is one of our valid extensions
        /// </summary>
        /// <param name="input">The input extension</param>
        /// <returns>
        ///   <c>true</c> if [is valid extension] [the specified input]; otherwise, <c>false</c>.
        /// </returns>
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
