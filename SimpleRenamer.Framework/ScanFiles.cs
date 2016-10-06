using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.EventArguments;
using SimpleRenamer.Framework.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework
{
    public class ScanFiles : IScanForShows
    {
        private ILogger logger;
        private IFileWatcher fileWatcher;
        private ITVShowMatcher tvShowMatcher;
        private IFileMatcher fileMatcher;
        private IConfigurationManager configurationManager;
        private Settings settings;
        public event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;

        public ScanFiles(ILogger log, IFileWatcher fileWatch, ITVShowMatcher showMatch, IFileMatcher fileMatch, IConfigurationManager configManager)
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
            fileMatcher = fileMatch;
            configurationManager = configManager;
            settings = configurationManager.Settings;
            fileWatcher.RaiseProgressEvent += RaiseProgress;
            fileMatcher.RaiseProgressEvent += RaiseProgress;
            tvShowMatcher.RaiseProgressEvent += RaiseProgress;
        }

        private void RaiseProgress(object sender, ProgressTextEventArgs e)
        {
            RaiseProgressEvent(this, e);
        }

        public async Task<List<MatchedFile>> Scan(CancellationToken ct)
        {
            //search folders for a list of video file paths
            List<string> videoFiles = await fileWatcher.SearchTheseFoldersAsync(ct);
            //use regex to attempt to figure out some details about the files ie showname, episode number, etc
            List<MatchedFile> matchedFiles = await SearchFileNames(videoFiles, ct);

            //now try and match the tv shows with TVDB
            List<MatchedFile> scannedEpisodes = await MatchTVShows(matchedFiles.Where(x => x.IsTVShow == true).ToList(), ct);
            List<MatchedFile> scannedMovies = await MatchMovies(matchedFiles.Where(x => x.IsMovie == true).ToList(), ct);

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

            return scannedFiles;
        }

        private async Task<List<MatchedFile>> SearchFileNames(List<string> videoFiles, CancellationToken ct)
        {
            List<MatchedFile> matchedFiles = await fileMatcher.SearchFilesAsync(videoFiles);
            return matchedFiles;
        }

        private async Task<List<MatchedFile>> MatchTVShows(List<MatchedFile> matchedFiles, CancellationToken ct)
        {
            object lockList = new object();
            List<MatchedFile> scannedEpisodes = new List<MatchedFile>();
            try
            {
                ShowNameMapping showNameMapping = configurationManager.ShowNameMappings;
                ShowNameMapping originalMapping = configurationManager.ShowNameMappings;

                //for each file
                Parallel.ForEach(matchedFiles, (tempEp) =>
                {
                    ct.ThrowIfCancellationRequested();
                    TVEpisodeScrape scrapeResult = null;

                    //scrape the episode name and incorporate this in the filename (if setting allows)
                    if (settings.RenameFiles)
                    {
                        scrapeResult = tvShowMatcher.ScrapeDetailsAsync(tempEp).GetAwaiter().GetResult();
                        tempEp = scrapeResult.tvep;
                        if (scrapeResult.series != null)
                        {
                            Mapping map = new Mapping(scrapeResult.tvep.ShowName, scrapeResult.series.Title, scrapeResult.series.Id.ToString());
                            if (!showNameMapping.Mappings.Any(x => x.TVDBShowID.Equals(map.TVDBShowID)))
                            {
                                showNameMapping.Mappings.Add(map);
                            }
                        }
                        ct.ThrowIfCancellationRequested();
                    }
                    else
                    {
                        tempEp.NewFileName = Path.GetFileNameWithoutExtension(tempEp.FilePath);
                    }
                    logger.TraceMessage(string.Format("Matched: {0} - S{1}E{2} - {3}", tempEp.ShowName, tempEp.Season, tempEp.Episode, tempEp.EpisodeName));
                    //only add the file if it needs renaming/moving
                    int season;
                    int.TryParse(tempEp.Season, out season);
                    string destinationDirectory = Path.Combine(settings.DestinationFolder, tempEp.ShowName, string.Format("Season {0}", season));
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
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }

            return scannedEpisodes;
        }

        private async Task<List<MatchedFile>> MatchMovies(List<MatchedFile> matchedFiles, CancellationToken ct)
        {
            object lockList = new object();
            List<MatchedFile> scannedMovies = new List<MatchedFile>();

            return scannedMovies;
        }
    }
}
