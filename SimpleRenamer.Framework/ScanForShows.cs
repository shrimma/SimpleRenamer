using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Extensions;
using SimpleRenamer.Framework.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework
{
    public class ScanForShows : IScanForShows
    {
        private ILogger logger;
        private IFileWatcher fileWatcher;
        private ITVShowMatcher tvShowMatcher;
        private IFileMatcher fileMatcher;

        private Settings settings;
        public ScanForShows(ILogger log, IFileWatcher fileWatch, ITVShowMatcher showMatch, IFileMatcher fileMatch, ISettingsFactory settingsFactory)
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
            if (settingsFactory == null)
            {
                throw new ArgumentNullException(nameof(settingsFactory));
            }
            logger = log;
            fileWatcher = fileWatch;
            tvShowMatcher = showMatch;
            fileMatcher = fileMatch;
            settings = settingsFactory.GetSettings();
        }

        public async Task<List<TVEpisode>> Scan(CancellationToken ct)
        {
            List<string> videoFiles = await fileWatcher.SearchTheseFoldersAsync(ct);
            List<TVEpisode> scannedEpisodes = await MatchTVShows(videoFiles, ct);

            return scannedEpisodes;
        }

        private async Task<List<TVEpisode>> MatchTVShows(List<string> videoFiles, CancellationToken ct)
        {
            List<TVEpisode> scannedEpisodes = new List<TVEpisode>();
            try
            {
                ShowNameMapping showNameMapping = await tvShowMatcher.ReadMappingFileAsync();
                ShowNameMapping originalMapping = await tvShowMatcher.ReadMappingFileAsync();
                List<Task<TVEpisode>> tasks = new List<Task<TVEpisode>>();
                //spin up a task for each file
                foreach (string fileName in videoFiles)
                {
                    logger.TraceMessage(string.Format("Trying to match {0}", fileName));
                    tasks.Add(fileMatcher.SearchFileNameAsync(fileName));
                }
                //as each task completes
                foreach (var t in tasks.InCompletionOrder())
                {
                    ct.ThrowIfCancellationRequested();
                    TVEpisode tempEp = await t;
                    TVEpisodeScrape scrapeResult = null;
                    if (tempEp != null)
                    {
                        logger.TraceMessage(string.Format("Matched {0}", tempEp.EpisodeName));
                        //scrape the episode name and incorporate this in the filename (if setting allows)
                        if (settings.RenameFiles)
                        {
                            scrapeResult = await tvShowMatcher.ScrapeDetailsAsync(tempEp, showNameMapping);
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
                            scannedEpisodes.Add(tempEp);
                        }
                        else
                        {
                            logger.TraceMessage(string.Format("File is already in good location {0}", tempEp.FilePath));
                        }
                    }
                    else
                    {
                        logger.TraceMessage(string.Format("Couldn't find a match!"));
                    }
                }
                if (showNameMapping.Mappings != originalMapping.Mappings || showNameMapping.Mappings.Count != originalMapping.Mappings.Count)
                {
                    await tvShowMatcher.WriteMappingFileAsync(showNameMapping);
                }
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }

            return scannedEpisodes;
        }
    }
}
