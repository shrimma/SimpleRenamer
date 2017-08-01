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
        private ILogger _logger;
        private IConfigurationManager _configurationManager;
        private IFileWatcher _fileWatcher;
        private ITVShowMatcher _tvShowMatcher;
        private IMovieMatcher _movieMatcher;
        private IFileMatcher _fileMatcher;
        private Settings _settings;
        private ParallelOptions parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
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
                List<string> videoFiles = await _fileWatcher.SearchFoldersAsync(ct);
                //use regex to attempt to figure out some details about the files ie showname, episode number, etc
                List<MatchedFile> matchedFiles = await _fileMatcher.SearchFilesAsync(videoFiles, ct);

                //try and match the tv shows with any TV scrapers we have available
                Task<List<MatchedFile>> scanTvShowsTask = MatchTVShows(matchedFiles.Where(x => x.FileType == FileType.TvShow).ToList(), ct);
                //try and match movies with TMDB
                Task<List<MatchedFile>> scanMovieTask = MatchMovies(matchedFiles.Where(x => x.FileType == FileType.Movie).ToList(), ct);
                //check there aren't any completely unmatched files (due to undecypherable filenames)
                List<MatchedFile> otherVideoFiles = matchedFiles.Where(x => x.FileType == FileType.Unknown).ToList();

                //wait for tv and movie scanning to complete
                await Task.WhenAll(scanTvShowsTask, scanMovieTask);
                List<MatchedFile> scannedEpisodes = scanTvShowsTask.Result;
                List<MatchedFile> scannedMovies = scanMovieTask.Result;

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
            ConcurrentBag<CompleteSeries> matchedSeries = new ConcurrentBag<CompleteSeries>();
            ConcurrentBag<MatchedFile> outputEpisodes = new ConcurrentBag<MatchedFile>();
            ShowNameMapping showNameMapping = _configurationManager.ShowNameMappings;
            ShowNameMapping originalMapping = _configurationManager.ShowNameMappings;

            //fixup mismatch titles
            parallelOptions.CancellationToken = ct;
            Parallel.ForEach(matchedFiles, parallelOptions, (file) =>
            {
                _tvShowMatcher.FixShowsFromMappings(file);
            });

            //ONLY DO ALL THIS STUFF IF WE ARE RENAMING FILES
            //find unique show ids or show names                        
            List<string> uniqueShowIds = matchedFiles.Where(x => !string.IsNullOrEmpty(x.TVDBShowId)).Select(x => x.TVDBShowId).Distinct().ToList();
            List<string> uniqueShowNames = matchedFiles.Where(x => string.IsNullOrEmpty(x.TVDBShowId)).Select(x => x.ShowName).Distinct().ToList();

            //block for searching unique show ids
            var searchShowIdsAsyncBlock = new ActionBlock<string>(async (showId) =>
            {
                ct.ThrowIfCancellationRequested();
                CompleteSeries series = await _tvShowMatcher.SearchShowByIdAsync(showId);
                if (series != null)
                {
                    RaiseProgressEvent(this, new ProgressTextEventArgs(string.Format("Found data for {0}", series.Series.SeriesName)));
                    matchedSeries.Add(series);
                }
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 2 });

            //block for searching unique shownames
            var searchShowNamesAsyncBlock = new ActionBlock<string>(async (showName) =>
            {
                ct.ThrowIfCancellationRequested();
                CompleteSeries series = await _tvShowMatcher.SearchShowByNameAsync(showName);
                if (series != null)
                {
                    RaiseProgressEvent(this, new ProgressTextEventArgs(string.Format("Found data for {0}", series.Series.SeriesName)));
                    matchedSeries.Add(series);
                }
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 2 });

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

            //for each series we matched
            foreach (CompleteSeries series in matchedSeries)
            {
                ct.ThrowIfCancellationRequested();
                Parallel.ForEach(matchedFiles, parallelOptions, (file) =>
                {
                    ct.ThrowIfCancellationRequested();
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
                        if (!showNameMapping.Mappings.Any(x => x.TVDBShowID.Equals(map.TVDBShowID)))
                        {
                            lock (lockList)
                            {
                                showNameMapping.Mappings.Add(map);
                            }
                        }
                        _tvShowMatcher.UpdateFileWithSeriesDetails(file, series);
                    }
                });
            };

            //final check that files need moving and are not already in correct location
            var ensureFileNeedsMovingBlock = new ActionBlock<MatchedFile>((file) =>
            {
                ct.ThrowIfCancellationRequested();
                //only add the file if it needs renaming/moving
                string destinationDirectory = Path.Combine(_settings.DestinationFolderTV, file.ShowName, string.Format("Season {0}", file.Season));
                string destinationFilePath = Path.Combine(destinationDirectory, file.NewFileName + Path.GetExtension(file.SourceFilePath));
                if (!file.SourceFilePath.Equals(destinationFilePath))
                {
                    _logger.TraceMessage(string.Format("Will move file {0} to {1}", file.SourceFilePath, file.NewFileName), EventLevel.Verbose);
                    RaiseProgressEvent(this, new ProgressTextEventArgs(string.Format("Will move file {0} to {1}.", file.SourceFilePath, file.NewFileName)));
                    outputEpisodes.Add(file);
                }
                else
                {
                    _logger.TraceMessage(string.Format("File is already in good location {0}", file.SourceFilePath), EventLevel.Verbose);
                    RaiseProgressEvent(this, new ProgressTextEventArgs(string.Format("File {0} will be ignored as already in correct location.", file.SourceFilePath)));
                }
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded });

            foreach (MatchedFile file in matchedFiles)
            {
                ensureFileNeedsMovingBlock.Post(file);
            }
            ensureFileNeedsMovingBlock.Complete();
            await ensureFileNeedsMovingBlock.Completion;

            if (showNameMapping.Mappings != originalMapping.Mappings || showNameMapping.Mappings.Count != originalMapping.Mappings.Count)
            {
                _configurationManager.ShowNameMappings = showNameMapping;
            }

            return outputEpisodes.ToList();
        }

        /// <summary>
        /// Matches the movies.
        /// </summary>
        /// <param name="matchedFiles">The matched files.</param>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        private async Task<List<MatchedFile>> MatchMovies(List<MatchedFile> matchedFiles, CancellationToken ct)
        {
            ConcurrentBag<MatchedFile> scannedMovies = new ConcurrentBag<MatchedFile>();

            //for each file
            var searchFilesAsyncBlock = new ActionBlock<MatchedFile>(async (file) =>
            {
                ct.ThrowIfCancellationRequested();
                file = await _movieMatcher.ScrapeDetailsAsync(file);

                //only add the file if it needs renaming/moving
                string movieDirectory = Path.Combine(_settings.DestinationFolderMovie, $"{file.ShowName} ({file.Season})");
                string destinationFilePath = Path.Combine(movieDirectory, file.ShowName + Path.GetExtension(file.SourceFilePath));
                if (!file.SourceFilePath.Equals(destinationFilePath))
                {
                    _logger.TraceMessage(string.Format("Will move file {0} to {1}", file.SourceFilePath, destinationFilePath), EventLevel.Verbose);
                    RaiseProgressEvent(this, new ProgressTextEventArgs(string.Format("Will move file {0} to {1}.", file.SourceFilePath, destinationFilePath)));
                    scannedMovies.Add(file);
                }
                else
                {
                    _logger.TraceMessage(string.Format("File is already in good location {0}", file.SourceFilePath), EventLevel.Verbose);
                    RaiseProgressEvent(this, new ProgressTextEventArgs(string.Format("File {0} will be ignored as already in correct location.", file.SourceFilePath)));
                }
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded });

            //post all our files to our dataflow
            foreach (MatchedFile file in matchedFiles)
            {
                searchFilesAsyncBlock.Post(file);
            }
            searchFilesAsyncBlock.Complete();
            await searchFilesAsyncBlock.Completion;

            return scannedMovies.ToList();
        }
    }
}
