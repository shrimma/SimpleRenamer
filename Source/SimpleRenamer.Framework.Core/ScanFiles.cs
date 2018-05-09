using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Common.TV.Model;
using System;
using System.Collections.Concurrent;
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
        private readonly ILogger _logger;
        private readonly IConfigurationManager _configurationManager;
        private readonly IFileWatcher _fileWatcher;
        private readonly ITVShowMatcher _tvShowMatcher;
        private readonly IMovieMatcher _movieMatcher;
        private readonly IFileMatcher _fileMatcher;        
        private readonly ParallelOptions _parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
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
            OnProgressTextChanged(e);
        }

        /// <summary>
        /// Scans the watch folders and matches files against shows/movies
        /// </summary>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns></returns>
        public async Task<List<MatchedFile>> ScanAsync(CancellationToken cancellationToken)
        {
            //search folders for a list of video file paths     
            OnProgressTextChanged(new ProgressTextEventArgs("Searching WatchFolders for relevant files."));
            List<string> videoFiles = await _fileWatcher.SearchFoldersAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            //use regex to attempt to figure out some details about the files ie showname, episode number, etc
            OnProgressTextChanged(new ProgressTextEventArgs(string.Format("Attempting to parse names of '{0}' video files", videoFiles.Count)));
            List<MatchedFile> matchedFiles = await _fileMatcher.SearchFilesAsync(videoFiles, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            //try and match the tv shows with any TV scrapers we have available
            Task<List<MatchedFile>> scanTvShowsTask = MatchTVShows(matchedFiles.Where(x => x.FileType == FileType.TvShow).ToList(), cancellationToken);
            //try and match movies with TMDB
            Task<List<MatchedFile>> scanMovieTask = MatchMovies(matchedFiles.Where(x => x.FileType == FileType.Movie).ToList(), cancellationToken);
            //check there aren't any completely unmatched files (due to undecypherable filenames)
            List<MatchedFile> otherVideoFiles = matchedFiles.Where(x => x.FileType == FileType.Unknown).ToList();

            OnProgressTextChanged(new ProgressTextEventArgs(string.Format("Matching '{0}' parsed files against metadata", matchedFiles.Count)));
            //wait for tv and movie scanning to complete
            await Task.WhenAll(scanTvShowsTask, scanMovieTask);
            List<MatchedFile> scannedEpisodes = scanTvShowsTask.Result;
            List<MatchedFile> scannedMovies = scanMovieTask.Result;

            cancellationToken.ThrowIfCancellationRequested();
            //add the tv shows and movies to the same list and return this
            List<MatchedFile> scannedFiles = new List<MatchedFile>();
            if (scannedEpisodes?.Count > 0)
            {
                scannedFiles.AddRange(scannedEpisodes);
            }
            if (scannedMovies?.Count > 0)
            {
                scannedFiles.AddRange(scannedMovies);
            }
            if (otherVideoFiles?.Count > 0)
            {
                scannedFiles.AddRange(otherVideoFiles);
            }

            OnProgressTextChanged(new ProgressTextEventArgs(string.Format("Scanned and matched '{0}' files found - '{1}' TV, '{2}' Movie, '{3}' Unknown", matchedFiles.Count, scannedEpisodes?.Count, scannedMovies?.Count, otherVideoFiles?.Count)));

            //return the full list of tv, movies, and unknown files
            return scannedFiles;
        }

        /// <summary>
        /// Matches the tv shows.
        /// </summary>
        /// <param name="matchedFiles">The matched files.</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns></returns>
        private async Task<List<MatchedFile>> MatchTVShows(List<MatchedFile> matchedFiles, CancellationToken cancellationToken)
        {
            object lockList = new object();
            ConcurrentBag<CompleteSeries> matchedSeries = new ConcurrentBag<CompleteSeries>();
            ConcurrentBag<MatchedFile> outputEpisodes = new ConcurrentBag<MatchedFile>();

            //fixup mismatch titles
            _parallelOptions.CancellationToken = cancellationToken;
            Parallel.ForEach(matchedFiles, _parallelOptions, (file) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                _tvShowMatcher.FixShowsFromMappings(file);
            });

            cancellationToken.ThrowIfCancellationRequested();

            //ONLY DO ALL THIS STUFF IF WE ARE RENAMING FILES
            //find unique show ids or show names                        
            List<string> uniqueShowIds = matchedFiles.Where(x => !string.IsNullOrWhiteSpace(x.TVDBShowId)).Select(x => x.TVDBShowId).Distinct().ToList();
            List<string> uniqueShowNames = matchedFiles.Where(x => string.IsNullOrWhiteSpace(x.TVDBShowId)).Select(x => x.ShowName).Distinct().ToList();

            //block for searching unique show ids
            var searchShowIdsAsyncBlock = new ActionBlock<string>(async (showId) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                CompleteSeries series = await _tvShowMatcher.SearchShowByIdAsync(showId, cancellationToken);
                if (series != null)
                {
                    matchedSeries.Add(series);
                }
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 2, CancellationToken = cancellationToken });

            //block for searching unique shownames
            var searchShowNamesAsyncBlock = new ActionBlock<string>(async (showName) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                CompleteSeries series = await _tvShowMatcher.SearchShowByNameAsync(showName, cancellationToken);
                if (series != null)
                {
                    matchedSeries.Add(series);
                }
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 2, CancellationToken = cancellationToken });

            //execute for each unique showId
            foreach (string showId in uniqueShowIds)
            {
                searchShowIdsAsyncBlock.Post(showId);
            }
            searchShowIdsAsyncBlock.Complete();

            ///execute for each unique showname
            foreach (string showName in uniqueShowNames)
            {
                searchShowNamesAsyncBlock.Post(showName);
            }
            searchShowNamesAsyncBlock.Complete();

            //wait for both searches to complete
            await Task.WhenAll(searchShowIdsAsyncBlock.Completion, searchShowNamesAsyncBlock.Completion);
            cancellationToken.ThrowIfCancellationRequested();

            //for each series we matched
            foreach (CompleteSeries series in matchedSeries.Distinct())
            {
                cancellationToken.ThrowIfCancellationRequested();
                Parallel.ForEach(matchedFiles, _parallelOptions, (file) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    //if the file had showid set
                    if (file.TVDBShowId != null)
                    {
                        if (series.Series.Id.ToString().Equals(file.TVDBShowId))
                        {
                            _tvShowMatcher.UpdateFileWithSeriesDetails(file, series);
                        }
                    }
                    else if (series.Series.SeriesName.Equals(file.ShowName))
                    {
                        //update the mapping as found something
                        Mapping map = new Mapping(file.ShowName, series.Series.SeriesName, series.Series.Id.ToString());
                        if (!_configurationManager.ShowNameMappings.Any(x => x.TVDBShowID.Equals(map.TVDBShowID)))
                        {
                            lock (lockList)
                            {
                                _configurationManager.ShowNameMappings.Add(map);
                            }
                        }
                        _tvShowMatcher.UpdateFileWithSeriesDetails(file, series);
                    }
                });
            };

            //final check that files need moving and are not already in correct location
            var ensureFileNeedsMovingBlock = new ActionBlock<MatchedFile>((file) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                //only add the file if it needs renaming/moving
                string destinationDirectory = Path.Combine(_configurationManager.Settings.DestinationFolderTV, file.ShowName, string.Format("Season {0}", file.Season));
                string destinationFilePath = Path.Combine(destinationDirectory, file.NewFileName + Path.GetExtension(file.SourceFilePath));
                if (!file.SourceFilePath.Equals(destinationFilePath))
                {
                    outputEpisodes.Add(file);
                }
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded });

            foreach (MatchedFile file in matchedFiles)
            {
                ensureFileNeedsMovingBlock.Post(file);
            }
            ensureFileNeedsMovingBlock.Complete();
            await ensureFileNeedsMovingBlock.Completion;

            return outputEpisodes.ToList();
        }

        /// <summary>
        /// Matches the movies.
        /// </summary>
        /// <param name="matchedFiles">The matched files.</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns></returns>
        private async Task<List<MatchedFile>> MatchMovies(List<MatchedFile> matchedFiles, CancellationToken cancellationToken)
        {
            ConcurrentBag<MatchedFile> scannedMovies = new ConcurrentBag<MatchedFile>();

            //for each file
            var searchFilesAsyncBlock = new ActionBlock<MatchedFile>(async (file) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                file = await _movieMatcher.ScrapeDetailsAsync(file, cancellationToken);

                //only add the file if it needs renaming/moving
                string movieDirectory = Path.Combine(_configurationManager.Settings.DestinationFolderMovie, $"{file.ShowName} ({file.Season})");
                string destinationFilePath = Path.Combine(movieDirectory, file.ShowName + Path.GetExtension(file.SourceFilePath));
                if (!file.SourceFilePath.Equals(destinationFilePath))
                {
                    scannedMovies.Add(file);
                }
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded, CancellationToken = cancellationToken });

            //post all our files to our dataflow
            foreach (MatchedFile file in matchedFiles)
            {
                searchFilesAsyncBlock.Post(file);
            }
            searchFilesAsyncBlock.Complete();
            await searchFilesAsyncBlock.Completion;

            return scannedMovies.ToList();
        }

        protected virtual void OnProgressTextChanged(ProgressTextEventArgs e)
        {
            _logger.TraceMessage(e.Text, EventLevel.Informational);
            RaiseProgressEvent?.Invoke(this, e);
        }
    }
}
