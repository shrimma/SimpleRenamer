using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Common.TV.Model;
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
        private ILogger logger;
        private IFileWatcher fileWatcher;
        private ITVShowMatcher tvShowMatcher;
        private IMovieMatcher movieMatcher;
        private IFileMatcher fileMatcher;
        private IConfigurationManager configurationManager;
        private Settings settings;
        public event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;

        public ScanFiles(ILogger log, IFileWatcher fileWatch, ITVShowMatcher showMatch, IMovieMatcher movieMatch, IFileMatcher fileMatch, IConfigurationManager configManager)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            if (fileWatch == null)
            {
                throw new ArgumentNullException(nameof(fileWatch));
            }
            if (showMatch == null)
            {
                throw new ArgumentNullException(nameof(showMatch));
            }
            if (movieMatch == null)
            {
                throw new ArgumentNullException(nameof(movieMatch));
            }
            if (fileMatch == null)
            {
                throw new ArgumentNullException(nameof(fileMatch));
            }
            if (configManager == null)
            {
                throw new ArgumentNullException(nameof(configManager));
            }
            logger = log;
            fileWatcher = fileWatch;
            tvShowMatcher = showMatch;
            movieMatcher = movieMatch;
            fileMatcher = fileMatch;
            configurationManager = configManager;
            settings = configurationManager.Settings;
            fileWatcher.RaiseProgressEvent += RaiseProgress;
            fileMatcher.RaiseProgressEvent += RaiseProgress;
            tvShowMatcher.RaiseProgressEvent += RaiseProgress;
            movieMatcher.RaiseProgressEvent += RaiseProgress;
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
                List<string> videoFiles = await fileWatcher.SearchTheseFoldersAsync(ct);
                //use regex to attempt to figure out some details about the files ie showname, episode number, etc
                List<MatchedFile> matchedFiles = await fileMatcher.SearchFilesAsync(videoFiles, ct);
                //try and match the tv shows with TVDB
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
            ShowNameMapping showNameMapping = configurationManager.ShowNameMappings;
            ShowNameMapping originalMapping = configurationManager.ShowNameMappings;
            ParallelOptions po = new ParallelOptions();
            po.CancellationToken = ct;
            //for each file
            Parallel.ForEach(matchedFiles, po, (tempEp) =>
            {
                TVEpisodeScrape scrapeResult = null;
                //scrape the episode name and incorporate this in the filename (if setting allows)
                if (settings.RenameFiles)
                {
                    scrapeResult = tvShowMatcher.ScrapeDetailsAsync(tempEp).GetAwaiter().GetResult();
                    tempEp = scrapeResult.tvep;
                    if (scrapeResult.series != null)
                    {
                        Mapping map = new Mapping(scrapeResult.tvep.ShowName, scrapeResult.series.Series.SeriesName, scrapeResult.series.Series.Id.ToString());
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
                    tempEp.NewFileName = Path.GetFileNameWithoutExtension(tempEp.FilePath);
                }
                logger.TraceMessage(string.Format("Matched: {0} - S{1}E{2} - {3}", tempEp.ShowName, tempEp.Season, tempEp.Episode, tempEp.EpisodeName));
                //only add the file if it needs renaming/moving
                string destinationDirectory = Path.Combine(settings.DestinationFolderTV, tempEp.ShowName, string.Format("Season {0}", tempEp.Season));
                string destinationFilePath = Path.Combine(destinationDirectory, tempEp.NewFileName + Path.GetExtension(tempEp.FilePath));
                if (!tempEp.FilePath.Equals(destinationFilePath))
                {
                    logger.TraceMessage(string.Format("Will move with name {0}", tempEp.NewFileName));
                    lock (lockList)
                    {
                        scannedEpisodes.Add(tempEp);
                    }
                }
                else
                {
                    logger.TraceMessage(string.Format("File is already in good location {0}", tempEp.FilePath));
                }
            });

            if (showNameMapping.Mappings != originalMapping.Mappings || showNameMapping.Mappings.Count != originalMapping.Mappings.Count)
            {
                configurationManager.ShowNameMappings = showNameMapping;
            }

            return scannedEpisodes;
        }

        private async Task<List<MatchedFile>> MatchMovies(List<MatchedFile> matchedFiles, CancellationToken ct)
        {
            object lockList = new object();
            List<MatchedFile> scannedMovies = new List<MatchedFile>();

            //for each file
            Parallel.ForEach(matchedFiles, (tempMovie) =>
            {
                tempMovie = movieMatcher.ScrapeDetailsAsync(tempMovie).GetAwaiter().GetResult();

                //only add the file if it needs renaming/moving
                string movieDirectory = Path.Combine(settings.DestinationFolderMovie, $"{tempMovie.ShowName} ({tempMovie.Season})");
                string destinationFilePath = Path.Combine(movieDirectory, tempMovie.ShowName + Path.GetExtension(tempMovie.FilePath));
                if (!tempMovie.FilePath.Equals(destinationFilePath))
                {
                    logger.TraceMessage(string.Format("Will move with name {0}", tempMovie.NewFileName));
                    lock (lockList)
                    {
                        scannedMovies.Add(tempMovie);
                    }
                }
                else
                {
                    logger.TraceMessage(string.Format("File is already in good location {0}", tempMovie.FilePath));
                }
                ct.ThrowIfCancellationRequested();
            });

            return scannedMovies;
        }
    }
}
