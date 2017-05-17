using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Framework.Core
{
    public class ScanFiles : IScanFiles
    {
        private ILogger _logger;
        private IConfigurationManager _configurationManager;
        private IFileWatcher _fileWatcher;
        private ITVShowMatcher _tvShowMatcher;
        private IMovieMatcher _movieMatcher;
        private IFileMatcher _fileMatcher;
        private Settings _settings;
        public event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;

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

        private void RaiseProgress(object sender, ProgressTextEventArgs e)
        {
            RaiseProgressEvent(this, e);
        }

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

        private async Task<List<MatchedFile>> MatchTVShows(List<MatchedFile> matchedFiles, CancellationToken ct)
        {
            object lockList = new object();
            List<MatchedFile> scannedEpisodes = new List<MatchedFile>();
            ShowNameMapping showNameMapping = _configurationManager.ShowNameMappings;
            ShowNameMapping originalMapping = _configurationManager.ShowNameMappings;
            ParallelOptions po = new ParallelOptions()
            {
                CancellationToken = ct
            };
            //for each file
            Parallel.ForEach(matchedFiles, po, (tempEp) =>
            {
                //scrape the episode name and incorporate this in the filename (if setting allows)
                if (_settings.RenameFiles)
                {
                    tempEp = _tvShowMatcher.ScrapeDetailsAsync(tempEp).GetAwaiter().GetResult();
                    if (!string.IsNullOrWhiteSpace(tempEp.TVDBShowId))
                    {
                        Mapping map = new Mapping(tempEp.ShowName, tempEp.ShowName, tempEp.TVDBShowId);
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
                    tempEp.NewFileName = Path.GetFileNameWithoutExtension(tempEp.SourceFilePath);
                }
                _logger.TraceMessage(string.Format("Matched: {0} - S{1}E{2} - {3}", tempEp.ShowName, tempEp.Season, tempEp.EpisodeNumber, tempEp.EpisodeName));
                //only add the file if it needs renaming/moving
                string destinationDirectory = Path.Combine(_settings.DestinationFolderTV, tempEp.ShowName, string.Format("Season {0}", tempEp.Season));
                string destinationFilePath = Path.Combine(destinationDirectory, tempEp.NewFileName + Path.GetExtension(tempEp.SourceFilePath));
                if (!tempEp.SourceFilePath.Equals(destinationFilePath))
                {
                    _logger.TraceMessage(string.Format("Will move with name {0}", tempEp.NewFileName));
                    lock (lockList)
                    {
                        scannedEpisodes.Add(tempEp);
                    }
                }
                else
                {
                    _logger.TraceMessage(string.Format("File is already in good location {0}", tempEp.SourceFilePath));
                }
                ct.ThrowIfCancellationRequested();
            });

            if (showNameMapping.Mappings != originalMapping.Mappings || showNameMapping.Mappings.Count != originalMapping.Mappings.Count)
            {
                _configurationManager.ShowNameMappings = showNameMapping;
            }

            return scannedEpisodes;
        }

        private async Task<List<MatchedFile>> MatchMovies(List<MatchedFile> matchedFiles, CancellationToken ct)
        {
            object lockList = new object();
            List<MatchedFile> scannedMovies = new List<MatchedFile>();

            ParallelOptions po = new ParallelOptions()
            {
                CancellationToken = ct
            };
            //for each file
            Parallel.ForEach(matchedFiles, po, (tempMovie) =>
            {
                tempMovie = _movieMatcher.ScrapeDetailsAsync(tempMovie).GetAwaiter().GetResult();

                //only add the file if it needs renaming/moving
                string movieDirectory = Path.Combine(_settings.DestinationFolderMovie, $"{tempMovie.ShowName} ({tempMovie.Season})");
                string destinationFilePath = Path.Combine(movieDirectory, tempMovie.ShowName + Path.GetExtension(tempMovie.SourceFilePath));
                if (!tempMovie.SourceFilePath.Equals(destinationFilePath))
                {
                    _logger.TraceMessage(string.Format("Will move with name {0}", tempMovie.NewFileName));
                    lock (lockList)
                    {
                        scannedMovies.Add(tempMovie);
                    }
                }
                else
                {
                    _logger.TraceMessage(string.Format("File is already in good location {0}", tempMovie.SourceFilePath));
                }
                ct.ThrowIfCancellationRequested();
            });

            return scannedMovies;
        }
    }
}
