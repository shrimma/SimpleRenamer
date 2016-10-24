using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.EventArguments;
using SimpleRenamer.Framework.Interface;
using SimpleRenamer.Framework.TvdbModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework
{
    public class TVShowMatcher : ITVShowMatcher
    {
        private ILogger logger;
        private Settings settings;
        private ITvdbManager tvdbManager;
        private IConfigurationManager configurationManager;
        public event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;

        public TVShowMatcher(IConfigurationManager configManager, ITvdbManager tvdb, ILogger log)
        {
            if (configManager == null)
            {
                throw new ArgumentNullException(nameof(configManager));
            }
            if (tvdb == null)
            {
                throw new ArgumentNullException(nameof(tvdb));
            }
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            configurationManager = configManager;
            tvdbManager = tvdb;
            logger = log;
            settings = configurationManager.Settings;
        }

        /// <summary>
        /// Scrape the TVDB and use the results for a better file name
        /// </summary>
        /// <param name="episode"></param>
        /// <returns></returns>
        public async Task<TVEpisodeScrape> ScrapeDetailsAsync(MatchedFile episode, CancellationToken ct)
        {
            logger.TraceMessage("ScrapeDetailsAsync - Start");
            RaiseProgressEvent(this, new ProgressTextEventArgs($"Scraping details for file {episode.FilePath}"));
            //read the mapping file and try and find any already selected matches
            episode = FixMismatchTitles(episode);
            TVEpisodeScrape episodeScrape = new TVEpisodeScrape();
            //scrape the episode name - if we haven't already got the show ID then search for it
            if (string.IsNullOrEmpty(episode.TVDBShowId))
            {
                episodeScrape = await ScrapeShowAsync(episode, ct);
            }
            else
            {
                episodeScrape = await ScrapeSpecificShow(episode, episode.TVDBShowId, false);
            }

            //generate the new file name
            episodeScrape.tvep = GenerateFileName(episodeScrape.tvep);

            logger.TraceMessage("ScrapeDetailsAsync - End");
            return episodeScrape;
        }

        /// <summary>
        /// If user has selected a specific show in the past then lets find and automatically use this
        /// </summary>
        /// <param name="episode"></param>
        /// <returns></returns>
        private MatchedFile FixMismatchTitles(MatchedFile episode)
        {
            ShowNameMapping showNameMapping = configurationManager.ShowNameMappings;
            logger.TraceMessage("FixMismatchTitles - Start");
            if (showNameMapping != null && showNameMapping.Mappings != null && showNameMapping.Mappings.Count > 0)
            {
                foreach (Mapping m in showNameMapping.Mappings)
                {
                    if (m.FileShowName.Equals(episode.ShowName))
                    {
                        if (!string.IsNullOrEmpty(m.TVDBShowID))
                        {
                            episode.TVDBShowId = m.TVDBShowID;
                        }
                        if (!string.IsNullOrEmpty(m.TVDBShowName))
                        {
                            episode.ShowName = m.TVDBShowName;
                        }
                        break;
                    }
                }
            }

            logger.TraceMessage("FixMismatchTitles - End");
            return episode;
        }

        /// <summary>
        /// Scrape the show details from the given showname, season and episode number
        /// </summary>
        /// <param name="episode">The episode to scrape</param>
        /// <returns></returns>
        private async Task<TVEpisodeScrape> ScrapeShowAsync(MatchedFile episode, CancellationToken ct)
        {
            logger.TraceMessage("ScrapeShowAsync - Start");
            TVEpisodeScrape episodeScrape = new TVEpisodeScrape();
            var series = await tvdbManager.SearchSeriesByNameAsync(episode.ShowName);
            string seriesId = string.Empty;
            //IF we have no results or more than 1 result then flag the file to be manually matched
            if (series == null || series.Count > 1)
            {
                episode.ActionThis = false;
                episode.SkippedExactSelection = true;
                episodeScrape.tvep = episode;
            }
            else if (series.Count == 1)
            {
                //if theres only one match then scape the specific show
                seriesId = series[0].Id.ToString();
                episodeScrape = await ScrapeSpecificShow(episode, seriesId, true);
            }

            logger.TraceMessage("ScrapeShowAsync - End");
            return episodeScrape;
        }

        private async Task<TVEpisodeScrape> ScrapeSpecificShow(MatchedFile episode, string seriesId, bool newMatch)
        {
            logger.TraceMessage("ScrapeSpecificShow - Start");
            uint season = 0;
            uint.TryParse(episode.Season, out season);
            int episodeNumber = 0;
            int.TryParse(episode.Episode, out episodeNumber);
            CompleteSeries matchedSeries = await tvdbManager.GetSeriesByIdAsync(seriesId);
            episode.TVDBShowId = seriesId;
            episode.ShowName = matchedSeries.Series.SeriesName;
            episode.EpisodeName = matchedSeries.Episodes.Where(s => s.AiredSeason.Value == season && s.AiredEpisodeNumber == episodeNumber).FirstOrDefault().EpisodeName;
            List<SeriesImageQueryResult> seasonBanners = matchedSeries.SeasonPosters.Where(s => s.SubKey.Equals(episode.Season)).ToList();
            if (seasonBanners != null && seasonBanners.Count > 0)
            {
                episode.SeasonImage = seasonBanners.OrderByDescending(s => s.RatingsInfo.Average).FirstOrDefault().FileName;
            }
            if (matchedSeries.Posters != null && matchedSeries.Posters.Count > 0)
            {
                episode.ShowImage = matchedSeries.Posters.OrderByDescending(s => s.RatingsInfo.Average).FirstOrDefault().FileName;
            }

            logger.TraceMessage("ScrapeSpecificShow - End");
            return new TVEpisodeScrape(episode, matchedSeries);
        }

        public async Task<List<ShowView>> GetPossibleShowsForEpisode(string showName, CancellationToken ct)
        {
            logger.TraceMessage("GetPossibleShowsForEpisode - Start");
            var series = await tvdbManager.SearchSeriesByNameAsync(showName);
            DateTime dt = new DateTime();
            string airedDate;
            List<ShowView> shows = new List<ShowView>();
            if (series != null)
            {
                foreach (SeriesSearchData s in series)
                {
                    string desc = string.Empty;
                    if (!string.IsNullOrEmpty(s.Overview))
                    {
                        if (s.Overview.Length > 50)
                        {
                            desc = string.Format("{0}...", s.Overview.Substring(0, 50));
                        }
                        else
                        {
                            desc = s.Overview;
                        }
                    }

                    bool parsed = DateTime.TryParseExact(s.FirstAired, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
                    airedDate = parsed ? dt.Year.ToString() : "";
                    shows.Add(new ShowView(s.Id.ToString(), s.SeriesName, airedDate, desc));
                }
            }


            logger.TraceMessage("SelectShowFromListGetPossibleShowsForEpisode - End");
            return shows;
        }

        public async Task<MatchedFile> UpdateEpisodeWithMatchedSeries(string selectedSeriesId, MatchedFile episode, CancellationToken ct)
        {
            TVEpisodeScrape episodeScrape = new TVEpisodeScrape();
            //if user selected a match then scrape the details
            if (!string.IsNullOrEmpty(selectedSeriesId))
            {
                episodeScrape = await ScrapeSpecificShow(episode, selectedSeriesId, true);
                episodeScrape.tvep.ActionThis = true;
                episodeScrape.tvep.SkippedExactSelection = false;

                //generate the file name and update the mapping file
                episodeScrape.tvep = GenerateFileName(episode);

                if (episodeScrape.series != null)
                {
                    ShowNameMapping showNameMapping = configurationManager.ShowNameMappings;
                    Mapping map = new Mapping(episodeScrape.tvep.ShowName, episodeScrape.series.Series.SeriesName, episodeScrape.series.Series.Id.ToString());
                    if (!showNameMapping.Mappings.Any(x => x.TVDBShowID.Equals(map.TVDBShowID)))
                    {
                        showNameMapping.Mappings.Add(map);
                    }
                    configurationManager.ShowNameMappings = showNameMapping;
                }
            }
            else
            {
                episode.ActionThis = false;
                episode.SkippedExactSelection = true;
                episodeScrape.tvep = episode;
            }

            return episodeScrape.tvep;
        }

        /// <summary>
        /// Generate the new file name based on the details we have scraped
        /// </summary>
        /// <param name="episode">The episode to rename</param>
        /// <param name="settings">Our current settings</param>
        /// <returns></returns>
        private MatchedFile GenerateFileName(MatchedFile episode)
        {
            logger.TraceMessage("GenerateFileName - Start");

            string temp = settings.NewFileNameFormat;
            if (temp.Contains("{ShowName}"))
            {
                temp = temp.Replace("{ShowName}", episode.ShowName);
            }
            if (temp.Contains("{Season}"))
            {
                temp = temp.Replace("{Season}", episode.Season);
            }
            if (temp.Contains("{Episode}"))
            {
                temp = temp.Replace("{Episode}", episode.Episode);
            }
            if (temp.Contains("{EpisodeName}"))
            {
                temp = temp.Replace("{EpisodeName}", string.IsNullOrEmpty(episode.EpisodeName) ? "" : RemoveSpecialCharacters(episode.EpisodeName));
            }
            episode.NewFileName = temp;

            logger.TraceMessage("GenerateFileName - End");
            return episode;
        }

        private string RemoveSpecialCharacters(string input)
        {
            logger.TraceMessage("RemoveSpecialCharacters - Start");
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            logger.TraceMessage("RemoveSpecialCharacters - End");
            return r.Replace(input, "");
        }
    }
}
