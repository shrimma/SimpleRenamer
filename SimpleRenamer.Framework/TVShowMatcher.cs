using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheTVDBSharp;
using TheTVDBSharp.Models;

namespace SimpleRenamer.Framework
{
    public class TVShowMatcher : ITVShowMatcher
    {
        private ILogger logger;
        private Settings settings;
        private ITheTvdbManager tvdbManager;
        private IConfigurationManager configurationManager;

        public TVShowMatcher(IConfigurationManager configManager, ITheTvdbManager tvdb, ILogger log)
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
        /// <param name="settings"></param>
        /// <returns></returns>
        public async Task<TVEpisodeScrape> ScrapeDetailsAsync(TVEpisode episode)
        {
            logger.TraceMessage("ScrapeDetailsAsync - Start");
            //read the mapping file and try and find any already selected matches
            episode = FixMismatchTitles(episode);
            TVEpisodeScrape episodeScrape = new TVEpisodeScrape();
            //scrape the episode name - if we haven't already got the show ID then search for it
            if (string.IsNullOrEmpty(episode.TVDBShowId))
            {
                episodeScrape = await ScrapeShowAsync(episode);
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
        /// <param name="settings"></param>
        /// <returns></returns>
        private TVEpisode FixMismatchTitles(TVEpisode episode)
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
        /// <param name="settings">Our current settings</param>
        /// <returns></returns>
        private async Task<TVEpisodeScrape> ScrapeShowAsync(TVEpisode episode)
        {
            logger.TraceMessage("ScrapeShowAsync - Start");
            TVEpisodeScrape episodeScrape = new TVEpisodeScrape();
            var series = await tvdbManager.SearchSeries(episode.ShowName, Language.English);
            var seriesList = series.GetEnumerator();
            string seriesId = string.Empty;
            //IF we have more than 1 result then popup the selection form so user can choose the exact match
            if (series.Count > 1)
            {
                episode.ActionThis = false;
                episode.SkippedExactSelection = true;
                episodeScrape.tvep = episode;
            }
            else if (series.Count == 1)
            {
                //if theres only one match then it is easy
                seriesList.MoveNext();
                seriesId = seriesList.Current.Id.ToString();
                //if seriesID is populated then grab the episode name (it's possible to be null if user skipped the selection
                if (!string.IsNullOrEmpty(seriesId))
                {
                    episodeScrape = await ScrapeSpecificShow(episode, seriesId, true);
                }
                else
                {
                    episode.ActionThis = false;
                    episode.SkippedExactSelection = true;
                    episodeScrape.tvep = episode;
                }
            }

            logger.TraceMessage("ScrapeShowAsync - End");
            return episodeScrape;
        }

        private async Task<TVEpisodeScrape> ScrapeSpecificShow(TVEpisode episode, string seriesId, bool newMatch)
        {
            logger.TraceMessage("ScrapeSpecificShow - Start");
            uint season = 0;
            uint.TryParse(episode.Season, out season);
            int episodeNumber = 0;
            int.TryParse(episode.Episode, out episodeNumber);
            uint serId = 0;
            uint.TryParse(seriesId, out serId);
            Series matchedSeries = await tvdbManager.GetSeries(serId, Language.English);
            episode.TVDBShowId = seriesId;
            episode.ShowName = matchedSeries.Title;
            episode.EpisodeName = matchedSeries.Episodes.Where(s => s.SeasonNumber.Value == season && s.Number == episodeNumber).FirstOrDefault().Title;
            List<SeasonBanner> seasonBanners = matchedSeries.Banners.OfType<SeasonBanner>().Where(s => s.Season.Value == season && s.IsWide == false && s.Language == Language.English).ToList();
            List<PosterBanner> seriesBanners = matchedSeries.Banners.OfType<PosterBanner>().Where(s => s.Language == Language.English).ToList();
            if (seasonBanners != null && seasonBanners.Count > 0)
            {
                episode.SeasonImage = seasonBanners.OrderByDescending(s => s.Rating).FirstOrDefault().RemotePath;
            }
            if (seriesBanners != null && seriesBanners.Count > 0)
            {
                episode.ShowImage = seriesBanners.OrderByDescending(s => s.Rating).FirstOrDefault().RemotePath;
            }

            logger.TraceMessage("ScrapeSpecificShow - End");
            return new TVEpisodeScrape(episode, matchedSeries);
        }

        public async Task<List<ShowView>> GetPossibleShowsForEpisode(TVEpisode episode)
        {
            logger.TraceMessage("GetPossibleShowsForEpisode - Start");
            uint season = 0;
            uint.TryParse(episode.Season, out season);
            int episodeNumber = 0;
            int.TryParse(episode.Episode, out episodeNumber);

            var series = await tvdbManager.SearchSeries(episode.ShowName, Language.English);

            List<ShowView> shows = new List<ShowView>();
            foreach (var s in series)
            {
                string desc = string.Empty;
                if (!string.IsNullOrEmpty(s.Description))
                {
                    if (s.Description.Length > 50)
                    {
                        desc = string.Format("{0}...", s.Description.Substring(0, 50));
                    }
                    else
                    {
                        desc = s.Description;
                    }
                }
                shows.Add(new ShowView(s.Id.ToString(), s.Title, s.FirstAired.Value.Year.ToString(), desc));
            }

            logger.TraceMessage("SelectShowFromListGetPossibleShowsForEpisode - End");
            return shows;
        }

        public async Task<TVEpisode> UpdateEpisodeWithMatchedSeries(string selectedSeriesId, TVEpisode episode)
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
                    Mapping map = new Mapping(episodeScrape.tvep.ShowName, episodeScrape.series.Title, episodeScrape.series.Id.ToString());
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
        private TVEpisode GenerateFileName(TVEpisode episode)
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
