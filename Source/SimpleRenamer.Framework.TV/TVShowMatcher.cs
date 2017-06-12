using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Common.TV.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Sarjee.SimpleRenamer.Framework.TV
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.TV.Interface.ITVShowMatcher" />
    public class TVShowMatcher : ITVShowMatcher
    {
        private ILogger _logger;
        private IConfigurationManager _configurationManager;
        private Settings settings;
        private ITvdbManager _tvdbManager;
        private IHelper _helper;
        /// <summary>
        /// Fired whenever some noticeable progress is made
        /// </summary>
        public event EventHandler<ProgressTextEventArgs> RaiseProgressEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="TVShowMatcher"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="configManager">The configuration manager.</param>
        /// <param name="tvdbManager">The TVDB manager.</param>
        /// <exception cref="System.ArgumentNullException">
        /// logger
        /// or
        /// configManager
        /// or
        /// tvdbManager
        /// </exception>
        public TVShowMatcher(ILogger logger, IConfigurationManager configManager, ITvdbManager tvdbManager, IHelper helper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configurationManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
            _tvdbManager = tvdbManager ?? throw new ArgumentNullException(nameof(tvdbManager));
            settings = _configurationManager.Settings;
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
        }

        /// <summary>
        /// Scrape the TVDB and use the results for a better file name
        /// </summary>
        /// <param name="episode">Episode to scrape</param>
        /// <returns></returns>
        public async Task<MatchedFile> ScrapeDetailsAsync(MatchedFile episode)
        {
            _logger.TraceMessage("ScrapeDetailsAsync - Start");
            RaiseProgressEvent(this, new ProgressTextEventArgs($"Scraping details for file {episode.SourceFilePath}"));
            //read the mapping file and try and find any already selected matches
            episode = FixMismatchTitles(episode);
            MatchedFile file = null;
            //scrape the episode name - if we haven't already got the show ID then search for it
            if (string.IsNullOrEmpty(episode.TVDBShowId))
            {
                file = await ScrapeShowAsync(episode);
            }
            else
            {
                file = await ScrapeSpecificShow(episode, episode.TVDBShowId, false);
            }

            //generate the new file name
            file = GenerateFileName(file);

            _logger.TraceMessage("ScrapeDetailsAsync - End");
            return file;
        }

        /// <summary>
        /// If user has selected a specific show in the past then lets find and automatically use this
        /// </summary>
        /// <param name="episode">The episode.</param>
        /// <returns></returns>
        private MatchedFile FixMismatchTitles(MatchedFile episode)
        {
            ShowNameMapping showNameMapping = _configurationManager.ShowNameMappings;
            _logger.TraceMessage("FixMismatchTitles - Start");
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

            _logger.TraceMessage("FixMismatchTitles - End");
            return episode;
        }

        /// <summary>
        /// Scrape the show details from the given showname, season and episode number
        /// </summary>
        /// <param name="episode">The episode to scrape</param>
        /// <returns></returns>
        private async Task<MatchedFile> ScrapeShowAsync(MatchedFile episode)
        {
            _logger.TraceMessage("ScrapeShowAsync - Start");
            var series = await _tvdbManager.SearchSeriesByNameAsync(episode.ShowName);
            string seriesId = string.Empty;
            //IF we have no results or more than 1 result then flag the file to be manually matched
            if (series == null || series.Count > 1)
            {
                episode.ActionThis = false;
                episode.SkippedExactSelection = true;
            }
            else if (series.Count == 1)
            {
                //if theres only one match then scape the specific show
                seriesId = series[0].Id.ToString();
                episode = await ScrapeSpecificShow(episode, seriesId, true);
            }

            _logger.TraceMessage("ScrapeShowAsync - End");
            return episode;
        }

        /// <summary>
        /// Scrapes the specific show.
        /// </summary>
        /// <param name="episode">The episode.</param>
        /// <param name="seriesId">The series identifier.</param>
        /// <param name="newMatch">if set to <c>true</c> [new match].</param>
        /// <returns></returns>
        private async Task<MatchedFile> ScrapeSpecificShow(MatchedFile episode, string seriesId, bool newMatch)
        {
            _logger.TraceMessage("ScrapeSpecificShow - Start");
            uint.TryParse(episode.Season, out uint season);
            int.TryParse(episode.EpisodeNumber, out int episodeNumber);
            CompleteSeries matchedSeries = await _tvdbManager.GetSeriesByIdAsync(seriesId);
            episode.TVDBShowId = seriesId;
            episode.ShowName = matchedSeries.Series.SeriesName;
            episode.EpisodeName = matchedSeries.Episodes.Where(s => s.AiredSeason.Value == season && s.AiredEpisodeNumber == episodeNumber).FirstOrDefault().EpisodeName;
            int.TryParse(episode.Season, out int seasonAsInt);
            List<SeriesImageQueryResult> seasonBanners = matchedSeries.SeasonPosters.Where(s => s.SubKey.Equals(episode.Season) || s.SubKey.Equals(seasonAsInt.ToString())).ToList();
            if (seasonBanners != null && seasonBanners.Count > 0)
            {
                episode.SeasonImage = seasonBanners.OrderByDescending(s => s.RatingsInfo.Average).FirstOrDefault().FileName;
            }
            if (matchedSeries.Posters != null && matchedSeries.Posters.Count > 0)
            {
                episode.ShowImage = matchedSeries.Posters.OrderByDescending(s => s.RatingsInfo.Average).FirstOrDefault().FileName;
            }

            _logger.TraceMessage("ScrapeSpecificShow - End");
            return episode;
        }

        /// <summary>
        /// Gets a list of possible series that a TVEpisode name could relate to
        /// </summary>
        /// <param name="showName">The showname to be searched</param>
        /// <returns>
        /// A list of series
        /// </returns>
        public async Task<List<DetailView>> GetPossibleShowsForEpisode(string showName)
        {
            return await Task.Run(async () =>
            {
                _logger.TraceMessage("GetPossibleShowsForEpisode - Start");
                var series = await _tvdbManager.SearchSeriesByNameAsync(showName);
                string airedDate;
                List<DetailView> shows = new List<DetailView>();
                if (series != null)
                {
                    foreach (SeriesSearchData s in series)
                    {
                        try
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

                            bool parsed = DateTime.TryParseExact(s.FirstAired, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt);
                            airedDate = parsed ? dt.Year.ToString() : "N/A";
                            shows.Add(new DetailView(s.Id.ToString(), s.SeriesName, airedDate, desc));
                        }
                        catch (Exception ex)
                        {
                            //TODO just swallow this?
                        }
                    }
                }

                _logger.TraceMessage("SelectShowFromListGetPossibleShowsForEpisode - End");
                return shows;
            });
        }

        /// <summary>
        /// Updates a TV episode with the details of a selected series
        /// </summary>
        /// <param name="selectedSeriesId">The TVDB show id selected</param>
        /// <param name="episode">Episode to be updated</param>
        /// <returns>
        /// The updated TV episode
        /// </returns>
        public async Task<MatchedFile> UpdateEpisodeWithMatchedSeries(string selectedSeriesId, MatchedFile episode)
        {
            return await Task.Run(async () =>
            {
                //if user selected a match then scrape the details
                if (!string.IsNullOrEmpty(selectedSeriesId))
                {
                    episode = await ScrapeSpecificShow(episode, selectedSeriesId, true);
                    episode.ActionThis = true;
                    episode.SkippedExactSelection = false;

                    //generate the file name and update the mapping file
                    episode = GenerateFileName(episode);

                    if (!string.IsNullOrWhiteSpace(episode.TVDBShowId))
                    {
                        ShowNameMapping showNameMapping = _configurationManager.ShowNameMappings;
                        Mapping map = new Mapping(episode.ShowName, episode.ShowName, episode.TVDBShowId);
                        if (!showNameMapping.Mappings.Any(x => x.TVDBShowID.Equals(map.TVDBShowID)))
                        {
                            showNameMapping.Mappings.Add(map);
                        }
                        _configurationManager.ShowNameMappings = showNameMapping;
                    }
                    episode.FileType = FileType.TvShow;
                }
                else
                {
                    episode.ActionThis = false;
                    episode.SkippedExactSelection = true;
                }

                return episode;
            });
        }

        /// <summary>
        /// Generate the new file name based on the details we have scraped
        /// </summary>
        /// <param name="episode">The episode to rename</param>
        /// <returns></returns>
        private MatchedFile GenerateFileName(MatchedFile episode)
        {
            _logger.TraceMessage("GenerateFileName - Start");

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
                temp = temp.Replace("{Episode}", episode.EpisodeNumber);
            }
            if (temp.Contains("{EpisodeName}"))
            {
                temp = temp.Replace("{EpisodeName}", string.IsNullOrEmpty(episode.EpisodeName) ? "" : episode.EpisodeName);
            }
            episode.NewFileName = _helper.RemoveSpecialCharacters(temp);

            _logger.TraceMessage("GenerateFileName - End");
            return episode;
        }

        /// <summary>
        /// Gets the show with banner asynchronous.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <returns></returns>
        public async Task<(CompleteSeries series, BitmapImage banner)> GetShowWithBannerAsync(string showId)
        {
            _logger.TraceMessage("GetSeriesInfo - Start");
            CompleteSeries matchedSeries = await _tvdbManager.GetSeriesByIdAsync(showId);
            BitmapImage bannerImage = new BitmapImage();
            if (matchedSeries.SeriesBanners != null && matchedSeries.SeriesBanners.Count > 0)
            {
                bannerImage.BeginInit();
                bannerImage.UriSource = new Uri(_tvdbManager.GetBannerUri(matchedSeries.SeriesBanners.OrderByDescending(s => s.RatingsInfo.Average).FirstOrDefault().FileName));
                bannerImage.EndInit();
            }
            else
            {
                //TODO create a no image found banner
            }

            _logger.TraceMessage("GetSeriesInfo - End");
            return (matchedSeries, bannerImage);
        }
    }
}
