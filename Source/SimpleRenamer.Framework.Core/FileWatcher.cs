using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
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
        private ISettings _settings;
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
            _settings = _configurationManager.Settings;
        }

        /// <summary>
        /// Searches the configured folders for any video files
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>
        /// A list of file paths of video files
        /// </returns>
        public async Task<List<string>> SearchFoldersAsync(CancellationToken cancellationToken)
        {
            _logger.TraceMessage("SearchTheseFoldersAsync - Start", EventLevel.Verbose);
            List<string> foundFiles = new List<string>();
            //grab the list of ignored files

            ignoreList = _configurationManager.IgnoredFiles;
            //FOR EACH WATCH FOLDER
            foreach (string folder in _settings.WatchFolders)
            {
                //throw exception if cancel requested
                cancellationToken.ThrowIfCancellationRequested();
                OnProgressTextChanged(new ProgressTextEventArgs($"Searching watch folder for video files: {folder}"));
                //if the directory exists and contains at least 1 file (search sub directories if settings allow) -- limitation of searchPattern means we can't filter video extensions here
                if (Directory.Exists(folder) && Directory.GetFiles(folder, "*", _settings.SubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).Length > 0)
                {
                    //search the folder for files with video extensions
                    List<string> temp = await SearchThisFolder(folder, cancellationToken);
                    //if we find any files here add to the global list
                    if (temp.Count > 0)
                    {
                        foundFiles.AddRange(temp);
                    }
                }
                //throw exception if cancel requested
                cancellationToken.ThrowIfCancellationRequested();
            }

            OnProgressTextChanged(new ProgressTextEventArgs($"Searched all watch folders for video files"));
            _logger.TraceMessage($"Found {foundFiles.Count} across all watch folders.", EventLevel.Verbose);

            return foundFiles;
        }

        /// <summary>
        /// Searches a given folder for all video files
        /// </summary>
        /// <param name="dir">The folder to search</param>
        /// <param name="cancellationToken">The ct.</param>
        /// <returns></returns>
        private async Task<List<string>> SearchThisFolder(string dir, CancellationToken cancellationToken)
        {
            _logger.TraceMessage($"Searching Folder {dir} for valid video files.", EventLevel.Verbose);
            ConcurrentBag<string> foundFiles = new ConcurrentBag<string>();
            _parallelOptions.CancellationToken = cancellationToken;
            Task result = Task.Run(() => Parallel.ForEach(Directory.GetFiles(dir, "*", _settings.SubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly), _parallelOptions, (file) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                //is a valid extension, is not ignored and isn't a sample
                if (IsValidExtension(Path.GetExtension(file)) && !ignoreList.IgnoreFiles.Contains(file) && !Path.GetFileName(file).Contains("*.sample.*") && !Path.GetFileName(file).Contains("*.Sample.*"))
                {
                    foundFiles.Add(file);
                }
            }));

            await result;

            _logger.TraceMessage($"Found {foundFiles.Count} video files in {dir}.", EventLevel.Verbose);
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
            foreach (string extension in _settings.ValidExtensions)
            {
                if (input.ToLowerInvariant().Equals(extension.ToLowerInvariant()))
                {
                    _logger.TraceMessage($"{input} IsValidExtension == True", EventLevel.Verbose);
                    return true;
                }
            }

            _logger.TraceMessage($"{input} IsValidExtension == False", EventLevel.Verbose);
            return false;
        }

        protected virtual void OnProgressTextChanged(ProgressTextEventArgs e)
        {
            RaiseProgressEvent?.Invoke(this, e);
        }
    }
}
