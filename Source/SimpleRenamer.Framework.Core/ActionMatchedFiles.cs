﻿using Newtonsoft.Json;
using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Sarjee.SimpleRenamer.Framework.Core
{
    /// <summary>
    /// Action Matched Files
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Interface.IActionMatchedFiles" />
    public class ActionMatchedFiles : IActionMatchedFiles
    {
        private readonly ILogger _logger;
        private readonly IBackgroundQueue _backgroundQueue;
        private readonly IFileMover _fileMover;
        private readonly IConfigurationManager _configurationManager;
        private readonly IMessageSender _messageSender;        
        /// <summary>
        /// Fired whenever a preprocessor action is completed on a file
        /// </summary>
        public event EventHandler<FilePreProcessedEventArgs> RaiseFilePreProcessedEvent;
        /// <summary>
        /// Fired whenever a file is moved
        /// </summary>
        public event EventHandler<FileMovedEventArgs> RaiseFileMovedEvent;
        /// <summary>
        /// Fired whenever some noticeable progress is made
        /// </summary>
        public event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionMatchedFiles"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="backgroundQueue">The background queue.</param>
        /// <param name="fileMover">The file mover.</param>
        /// <param name="configManager">The configuration manager.</param>
        /// <exception cref="System.ArgumentNullException">
        /// logger
        /// or
        /// backgroundQueue
        /// or
        /// fileMover
        /// or
        /// configManager
        /// </exception>
        public ActionMatchedFiles(ILogger logger, IBackgroundQueue backgroundQueue, IFileMover fileMover, IConfigurationManager configManager, IMessageSender messageSender)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _backgroundQueue = backgroundQueue ?? throw new ArgumentNullException(nameof(backgroundQueue));
            _fileMover = fileMover ?? throw new ArgumentNullException(nameof(fileMover));
            _configurationManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));            
        }

        /// <summary>
        /// Performs preprocessor actions and then moves a list of scanned and matched episodes
        /// </summary>
        /// <param name="scannedEpisodes">The episodes to action</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns></returns>
        public async Task<bool> ActionAsync(ObservableCollection<MatchedFile> scannedEpisodes, CancellationToken cancellationToken)
        {
            OnProgressTextChanged(new ProgressTextEventArgs("Creating directory structure and downloading any missing banners"));
            //perform pre actions on TVshows            
            Task<List<MatchedFile>> tvShowTask = PreProcessTVShows(scannedEpisodes.Where(x => x.ActionThis == true && x.FileType == FileType.TvShow).ToList(), cancellationToken);
            //perform pre actions on movies
            Task<List<MatchedFile>> movieTask = PreProcessMovies(scannedEpisodes.Where(x => x.ActionThis == true && x.FileType == FileType.Movie).ToList(), cancellationToken);
            //await both to complete
            await Task.WhenAll(tvShowTask, movieTask);
            List<MatchedFile> tvShowsToMove = tvShowTask.Result;
            List<MatchedFile> moviesToMove = movieTask.Result;
            OnProgressTextChanged(new ProgressTextEventArgs("Finished creating directory structure and downloading banners."));
            cancellationToken.ThrowIfCancellationRequested();

            //concat final list of files to move
            List<MatchedFile> filesToMove = new List<MatchedFile>();
            if (tvShowsToMove?.Count > 0)
            {
                filesToMove.AddRange(tvShowsToMove);
            }
            if (moviesToMove?.Count > 0)
            {
                filesToMove.AddRange(moviesToMove);
            }

            //if we have files to move
            if (filesToMove?.Count > 0)
            {
                //send the stats to the cloud
                SendActionStatsToCloud(filesToMove, cancellationToken);
                //move these files
                await MoveFiles(filesToMove, cancellationToken);
            }

            return true;
        }

        private void SendActionStatsToCloud(List<MatchedFile> files, CancellationToken cancellationToken)
        {
            try
            {
                //do all the stuff for sending stats in background
                Task.Run(async () =>
                {
                    List<StatsFile> scanFiles = new List<StatsFile>();
                    foreach (MatchedFile file in files)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        scanFiles.Add(StatsFile.StatsFileFromMatchedFile(file));
                    }
                    string jsonPayload = JsonConvert.SerializeObject(scanFiles);
                    //run the messaging sending in background
                    await _messageSender.SendAsync(jsonPayload);
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.TraceException(ex, "Exception occured during stats sending");
            }
        }

        /// <summary>
        /// Pres the process tv shows.
        /// </summary>
        /// <param name="scannedEpisodes">The scanned episodes.</param>
        /// <param name="cancellationToken">The ct.</param>
        /// <returns></returns>
        private async Task<List<MatchedFile>> PreProcessTVShows(List<MatchedFile> scannedEpisodes, CancellationToken cancellationToken)
        {
            ConcurrentBag<MatchedFile> processFiles = new ConcurrentBag<MatchedFile>();

            var actionFilesAsyncBlock = new ActionBlock<MatchedFile>((file) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                Mapping mapping = _configurationManager.ShowNameMappings.FirstOrDefault(x => x.TVDBShowID.Equals(file.TVDBShowId));
                MatchedFile result = _fileMover.CreateDirectoriesAndQueueDownloadBanners(file, mapping, true);
                //fire event here
                OnFilePreProcessed(new FilePreProcessedEventArgs());
                if (!string.IsNullOrWhiteSpace(result.DestinationFilePath))
                {
                    _logger.TraceMessage(string.Format("Successfully processed file and downloaded banners: {0}", result.SourceFilePath));
                    processFiles.Add(result);
                }
                else
                {
                    _logger.TraceMessage(string.Format("Failed to process {0}", result.SourceFilePath));
                }
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded, CancellationToken = cancellationToken });

            //post all our files to our dataflow
            foreach (MatchedFile file in scannedEpisodes)
            {
                actionFilesAsyncBlock.Post(file);
            }
            actionFilesAsyncBlock.Complete();
            await actionFilesAsyncBlock.Completion;

            return processFiles.ToList();
        }

        /// <summary>
        /// Pres the process movies.
        /// </summary>
        /// <param name="scannedMovies">The scanned movies.</param>
        /// <param name="cancellationToken">The ct.</param>
        /// <returns></returns>
        private async Task<List<MatchedFile>> PreProcessMovies(List<MatchedFile> scannedMovies, CancellationToken cancellationToken)
        {
            ConcurrentBag<MatchedFile> processFiles = new ConcurrentBag<MatchedFile>();
            var actionFilesAsyncBlock = new ActionBlock<MatchedFile>((file) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                MatchedFile result = _fileMover.CreateDirectoriesAndQueueDownloadBanners(file, null, false);
                //fire event here
                OnFilePreProcessed(new FilePreProcessedEventArgs());
                if (!string.IsNullOrWhiteSpace(result.DestinationFilePath))
                {
                    _logger.TraceMessage(string.Format("Successfully processed file: {0}", result.SourceFilePath));
                    processFiles.Add(result);
                }
                else
                {
                    _logger.TraceMessage(string.Format("Failed to process {0}", result.SourceFilePath));
                }
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded, CancellationToken = cancellationToken });

            //post all our files to our dataflow
            foreach (MatchedFile file in scannedMovies)
            {
                actionFilesAsyncBlock.Post(file);
            }
            actionFilesAsyncBlock.Complete();
            await actionFilesAsyncBlock.Completion;

            return processFiles.ToList();
        }

        /// <summary>
        /// Moves the files.
        /// </summary>
        /// <param name="filesToMove">The files to move.</param>
        /// <param name="cancellationToken">The ct.</param>
        /// <returns></returns>
        private async Task<bool> MoveFiles(List<MatchedFile> filesToMove, CancellationToken cancellationToken)
        {
            var actionFilesAsyncBlock = new ActionBlock<MatchedFile>(async (file) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                OnProgressTextChanged(new ProgressTextEventArgs(string.Format("Moving file {0} to {1}.", file.SourceFilePath, file.DestinationFilePath)));
                bool result = await await _backgroundQueue.QueueTaskAsync(() => _fileMover.MoveFileAsync(file, cancellationToken));
                if (result)
                {
                    OnFileMoved(new FileMovedEventArgs(file));
                    OnProgressTextChanged(new ProgressTextEventArgs(string.Format("Finished moving file to {0}.", file.DestinationFilePath)));
                    _logger.TraceMessage(string.Format("Successfully {2} {0} to {1}", file.SourceFilePath, file.DestinationFilePath, _configurationManager.Settings.CopyFiles ? "copied" : "moved"));
                }
                else
                {
                    _logger.TraceMessage(string.Format("Failed to {2} {0} to {1}", file.SourceFilePath, file.DestinationFilePath, _configurationManager.Settings.CopyFiles ? "copy" : "move"));
                }
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1, CancellationToken = cancellationToken });
            //actually move/copy the files one at a time
            foreach (MatchedFile file in filesToMove)
            {
                actionFilesAsyncBlock.Post(file);
            }
            actionFilesAsyncBlock.Complete();
            await actionFilesAsyncBlock.Completion;

            return true;
        }

        protected virtual void OnFilePreProcessed(FilePreProcessedEventArgs e)
        {
            RaiseFilePreProcessedEvent?.Invoke(this, e);
        }

        protected virtual void OnFileMoved(FileMovedEventArgs e)
        {
            RaiseFileMovedEvent?.Invoke(this, e);
        }

        protected virtual void OnProgressTextChanged(ProgressTextEventArgs e)
        {
            RaiseProgressEvent?.Invoke(this, e);
        }
    }
}
