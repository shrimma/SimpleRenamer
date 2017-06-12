using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Sarjee.SimpleRenamer.Framework.Core
{
    /// <summary>
    /// Scan Files
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Interface.IScanFiles" />
    public class ScanFiles : IScanFiles
    {
        private ILogger _logger;
        private IConfigurationManager _configurationManager;
        private IFileWatcher _fileWatcher;
        private ITVShowMatcher _tvShowMatcher;
        private IMovieMatcher _movieMatcher;
        private IFileMatcher _fileMatcher;
        private Settings _settings;
        /// <summary>
        /// Fired whenever some noticeable progress is made
        /// </summary>
        public event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanFiles"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="configManager">The configuration manager.</param>
        /// <param name="fileWatcher">The file watcher.</param>
        /// <param name="showMatcher">The show matcher.</param>
        /// <param name="movieMatcher">The movie matcher.</param>
        /// <param name="fileMatcher">The file matcher.</param>
        /// <exception cref="System.ArgumentNullException">
        /// logger
        /// or
        /// configManager
        /// or
        /// fileWatcher
        /// or
        /// showMatcher
        /// or
        /// movieMatcher
        /// or
        /// fileMatcher
        /// </exception>
        public ScanFiles(ILogger logger, IConfigurationManager configManager, IFileWatcher fileWatcher, ITVShowMatcher showMatcher, IMovieMatcher movieMatcher, IFileMatcher fileMatcher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configurationManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
            _fileWatcher = fileWatcher ?? throw new ArgumentNullException(nameof(fileWatcher));
            _tvShowMatcher = showMatcher ?? throw new ArgumentNullException(nameof(showMatcher));
            _movieMatcher = movieMatcher ?? throw new ArgumentNullException(nameof(movieMatcher));
            _fileMatcher = fileMatcher ?? throw new ArgumentNullException(nameof(fileMatcher));
            _settings = _configurationManager.Settings;
            _fileWatcher.RaiseProgressEvent += RaiseProgress;
            _fileMatcher.RaiseProgressEvent += RaiseProgress;
            _tvShowMatcher.RaiseProgressEvent += RaiseProgress;
            _movieMatcher.RaiseProgressEvent += RaiseProgress;
        }

        /// <summary>
        /// Raises the progress.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ProgressTextEventArgs"/> instance containing the event data.</param>
        private void RaiseProgress(object sender, ProgressTextEventArgs e)
        {
            RaiseProgressEvent(this, e);
        }

        /// <summary>
        /// Scans the watch folders and matches files against shows/movies
        /// </summary>
        /// <param name="ct">CancellationToken</param>
        /// <returns></returns>
        public async Task<List<MatchedFile>> Scan(CancellationToken ct)
        {
            return await Task.Run(async () =>
            {
                //search folders for a list of video file paths
                List<string> videoFiles = await _fileWatcher.SearchTheseFoldersAsync(ct);
                //use regex to attempt to figure out some details about the files ie showname, episode number, etc
                List<MatchedFile> matchedFiles = await _fileMatcher.SearchFilesAsync(videoFiles, ct);
                //try and match the tv shows with any TV scrapers we have available
                List<MatchedFile> scannedEpisodes = await MatchTVShows(matchedFiles.Where(x => x.FileType == FileType.TvShow).ToList(), ct);
                //try and match movies with TMDB
                List<MatchedFile> scannedMovies = await MatchMovies(matchedFiles.Where(x => x.FileType == FileType.Movie).ToList(), ct);
                //check there aren't any completely unmatched files (due to undecypherable filenames)
                List<MatchedFile> otherVideoFiles = matchedFiles.Where(x => x.FileType == FileType.Unknown).ToList();

                //add the tv shows and movies to the same list and return this
                List<MatchedFile> scannedFiles = new List<MatchedFile>();
                if (scannedEpisodes != null && scannedEpisodes.Count > 0)
                {
                    scannedFiles.AddRange(scannedEpisodes);
                }
                if (scannedMovies != null && scannedMovies.Count > 0)
                {
                    scannedFiles.AddRange(scannedMovies);
                }
                if (otherVideoFiles != null & otherVideoFiles.Count > 0)
                {
                    scannedFiles.AddRange(otherVideoFiles);
                }

                //return the full list of tv, movies, and unknown files
                return scannedFiles;
            });
        }

        /// <summary>
        /// Matches the tv shows.
        /// </summary>
        /// <param name="matchedFiles">The matched files.</param>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        private async Task<List<MatchedFile>> MatchTVShows(List<MatchedFile> matchedFiles, CancellationToken ct)
        {
            object lockList = new object();
            List<MatchedFile> scannedEpisodes = new List<MatchedFile>();
            ShowNameMapping showNameMapping = _configurationManager.ShowNameMappings;
            ShowNameMapping originalMapping = _configurationManager.ShowNameMappings;

            //block for searching the files
            var searchFilesAsyncBlock = new TransformBlock<MatchedFile, MatchedFile>(async (file) =>
            {
                ct.ThrowIfCancellationRequested();
                string originalShowName = file.ShowName;
                //scrape the episode name and incorporate this in the filename (if setting allows)
                if (_settings.RenameFiles)
                {
                    file = await _tvShowMatcher.ScrapeDetailsAsync(file);
                    if (!string.IsNullOrWhiteSpace(file.TVDBShowId))
                    {
                        Mapping map = new Mapping(originalShowName, file.ShowName, file.TVDBShowId);
                        if (!showNameMapping.Mappings.Any(x => x.TVDBShowID.Equals(map.TVDBShowID)))
                        {
                            lock (lockList)
                            {
                                showNameMapping.Mappings.Add(map);
                            }
                        }
                    }
                }
                else
                {
                    file.NewFileName = Path.GetFileNameWithoutExtension(file.SourceFilePath);
                }
                _logger.TraceMessage(string.Format("Matched: {0} - S{1}E{2} - {3}", file.ShowName, file.Season, file.EpisodeNumber, file.EpisodeName));
                ct.ThrowIfCancellationRequested();

                //only add the file if it needs renaming/moving
                string destinationDirectory = Path.Combine(_settings.DestinationFolderTV, file.ShowName, string.Format("Season {0}", file.Season));
                string destinationFilePath = Path.Combine(destinationDirectory, file.NewFileName + Path.GetExtension(file.SourceFilePath));
                if (!file.SourceFilePath.Equals(destinationFilePath))
                {
                    _logger.TraceMessage(string.Format("Will move with name {0}", file.NewFileName), EventLevel.Verbose);
                    return file;
                }
                else
                {
                    _logger.TraceMessage(string.Format("File is already in good location {0}", file.SourceFilePath), EventLevel.Verbose);
                    return null;
                }
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded });

            //block for writing the outputs to a list
            var writeOutputBlock = new ActionBlock<MatchedFile>(file =>
            {
                if (file != null)
                {
                    //TODO make threadsafe
                    scannedEpisodes.Add(file);
                }
            });

            //link the writing to completion of search
            searchFilesAsyncBlock.LinkTo(writeOutputBlock, new DataflowLinkOptions { PropagateCompletion = true });

            //post all our files to our dataflow
            foreach (MatchedFile file in matchedFiles)
            {
                searchFilesAsyncBlock.Post(file);
            }
            searchFilesAsyncBlock.Complete();
            await writeOutputBlock.Completion;

            if (showNameMapping.Mappings != originalMapping.Mappings || showNameMapping.Mappings.Count != originalMapping.Mappings.Count)
            {
                _configurationManager.ShowNameMappings = showNameMapping;
            }

            return scannedEpisodes;
        }

        /// <summary>
        /// Matches the movies.
        /// </summary>
        /// <param name="matchedFiles">The matched files.</param>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        private async Task<List<MatchedFile>> MatchMovies(List<MatchedFile> matchedFiles, CancellationToken ct)
        {
            List<MatchedFile> scannedMovies = new List<MatchedFile>();

            //for each file
            var searchFilesAsyncBlock = new TransformBlock<MatchedFile, MatchedFile>(async (file) =>
            {
                ct.ThrowIfCancellationRequested();

                file = await _movieMatcher.ScrapeDetailsAsync(file);

                //only add the file if it needs renaming/moving
                string movieDirectory = Path.Combine(_settings.DestinationFolderMovie, $"{file.ShowName} ({file.Season})");
                string destinationFilePath = Path.Combine(movieDirectory, file.ShowName + Path.GetExtension(file.SourceFilePath));
                if (!file.SourceFilePath.Equals(destinationFilePath))
                {
                    _logger.TraceMessage(string.Format("Will move with name {0}", file.NewFileName), EventLevel.Verbose);
                    return file;
                }
                else
                {
                    _logger.TraceMessage(string.Format("File is already in good location {0}", file.SourceFilePath), EventLevel.Verbose);
                    return null;
                }
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded });

            //block for writing the outputs to a list
            var writeOutputBlock = new ActionBlock<MatchedFile>(file =>
            {
                if (file != null)
                {
                    //TODO make threadsafe
                    scannedMovies.Add(file);
                }
            });

            //link the writing to completion of search
            searchFilesAsyncBlock.LinkTo(writeOutputBlock, new DataflowLinkOptions { PropagateCompletion = true });

            //post all our files to our dataflow
            foreach (MatchedFile file in matchedFiles)
            {
                searchFilesAsyncBlock.Post(file);
            }
            searchFilesAsyncBlock.Complete();
            await writeOutputBlock.Completion;

            return scannedMovies;
        }
    }
}
