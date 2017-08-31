﻿using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Common.TV.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Sarjee.SimpleRenamer.Framework.TV
{
    /// <summary>
    /// TV Show Matcher
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.TV.Interface.ITVShowMatcher" />
    public class TVShowMatcher : ITVShowMatcher
    {
        private const string _fileNameShowName = "{ShowName}";
        private const string _fileNameSeason = "{Season}";
        private const string _fileNameEpisodeNumber = "{Episode}";
        private const string _fileNameEpisodeName = "{EpisodeName}";

        private ILogger _logger;
        private IConfigurationManager _configurationManager;
        private ISettings _settings;
        private ITvdbManager _tvdbManager;
        private IHelper _helper;
        private ParallelOptions _parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = (Environment.ProcessorCount + 2) };

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
            _settings = _configurationManager.Settings;
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
        }

        /// <summary>
        /// Searches the show by name asynchronous.
        /// </summary>
        /// <param name="showName">Name of the show.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">showName</exception>
        public async Task<CompleteSeries> SearchShowByNameAsync(string showName)
        {
            if (string.IsNullOrWhiteSpace(showName))
            {
                throw new ArgumentNullException(nameof(showName));
            }

            _logger.TraceMessage($"Searching for Show by Name: {showName}.", EventLevel.Verbose);
            List<SeriesSearchData> searchResults = await _tvdbManager.SearchSeriesByNameAsync(showName);
            //if theres only one match then scape the specific show and return this
            if (searchResults?.Count == 1)
            {
                _logger.TraceMessage($"Found only one show for Name: {showName}. So will grab all series data.", EventLevel.Verbose);
                string seriesId = searchResults[0].Id.ToString();
                return await _tvdbManager.GetSeriesByIdAsync(seriesId);
            }

            //else there were no matches or more than 1 possible match so return null
            _logger.TraceMessage($"Found {searchResults?.Count} shows for Name: {showName}. So cannot match this accurately, must be matched by user.", EventLevel.Verbose);
            return null;
        }

        /// <summary>
        /// Searches the show by identifier asynchronous.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">showId</exception>
        public async Task<CompleteSeries> SearchShowByIdAsync(string showId)
        {
            if (string.IsNullOrWhiteSpace(showId))
            {
                throw new ArgumentNullException(nameof(showId));
            }

            _logger.TraceMessage($"Searching for Show details by ShowId: {showId}.", EventLevel.Verbose);
            CompleteSeries series = await _tvdbManager.GetSeriesByIdAsync(showId);
            _logger.TraceMessage($"Found Show details by ShowId: {showId}.", EventLevel.Verbose);
            return series;
        }

        /// <summary>
        /// Updates the file with series details.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="series">The series.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// file
        /// or
        /// series
        /// </exception>
        public MatchedFile UpdateFileWithSeriesDetails(MatchedFile file, CompleteSeries series)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }
            if (series == null)
            {
                throw new ArgumentNullException(nameof(series));
            }

            _logger.TraceMessage($"Updating File {file.SourceFilePath} with SeriesInfo {series.Series?.SeriesName}.", EventLevel.Verbose);
            OnProgressTextChanged(new ProgressTextEventArgs(string.Format("Matching {0} with data", file.SourceFilePath)));
            bool seasonBannerFound = false;
            bool seriesBannerFound = false;
            int.TryParse(file.EpisodeNumber, out int episodeNumber);
            int.TryParse(file.Season, out int seasonAsInt);
            file.TVDBShowId = series.Series?.Id.ToString();
            file.ShowName = series.Series?.SeriesName;
            if (series.Episodes?.Count > 0)
            {
                file.EpisodeName = series.Episodes.Where(s => s?.AiredSeason.Value == seasonAsInt && s?.AiredEpisodeNumber == episodeNumber).FirstOrDefault().EpisodeName;
            }
            if (series.SeasonPosters?.Count > 0)
            {
                List<SeriesImageQueryResult> seasonBanners = series.SeasonPosters.Where(s => s.SubKey.Equals(file.Season) || s.SubKey.Equals(seasonAsInt.ToString())).ToList();
                if (seasonBanners?.Count > 0)
                {
                    file.SeasonImage = seasonBanners.OrderByDescending(s => s.RatingsInfo.Average).FirstOrDefault().FileName;
                    seasonBannerFound = true;
                }
            }
            if (series.Posters?.Count > 0)
            {
                file.ShowImage = series.Posters.OrderByDescending(s => s.RatingsInfo.Average).FirstOrDefault().FileName;
                seriesBannerFound = true;
            }
            file.NewFileName = GenerateFileName(file.ShowName, file.Season, file.EpisodeNumber, file.EpisodeName);
            file.ActionThis = true;
            file.SkippedExactSelection = false;

            _logger.TraceMessage($"Updated File {file.SourceFilePath} with SeriesInfo {series.Series?.SeriesName}. Season Image Found {seasonBannerFound}. Show Image Found {seriesBannerFound}.", EventLevel.Verbose);
            return file;
        }

        /// <summary>
        /// Generate the new file name based on the details we have scraped
        /// </summary>
        /// <param name="file">The episode to rename</param>
        /// <returns></returns>
        private string GenerateFileName(string showName, string season, string episodeNumber, string episodeName)
        {
            _logger.TraceMessage($"Generating FileName for Show: {showName}, Season: {season}, Episode: {episodeNumber}.", EventLevel.Verbose);

            string temp = _settings.NewFileNameFormat;
            if (temp.Contains(_fileNameShowName))
            {
                temp = temp.Replace(_fileNameShowName, showName);
            }
            if (temp.Contains(_fileNameSeason))
            {
                temp = temp.Replace(_fileNameSeason, season);
            }
            if (temp.Contains(_fileNameEpisodeNumber))
            {
                temp = temp.Replace(_fileNameEpisodeNumber, episodeNumber);
            }
            if (temp.Contains(_fileNameEpisodeName))
            {
                temp = temp.Replace(_fileNameEpisodeName, string.IsNullOrWhiteSpace(episodeName) ? "" : episodeName);
            }
            _logger.TraceMessage("Generated FileName {temp}.", EventLevel.Verbose);
            return _helper.RemoveSpecialCharacters(temp);
        }

        /// <summary>
        /// If user has selected a specific show in the past then lets find and automatically use this
        /// </summary>
        /// <param name="episode">The episode.</param>
        /// <returns></returns>
        public MatchedFile FixShowsFromMappings(MatchedFile episode)
        {
            if (episode == null)
            {
                throw new ArgumentNullException(nameof(episode));
            }

            _logger.TraceMessage($"FixShowName based on Mappings for {episode.SourceFilePath}.", EventLevel.Verbose);
            ShowNameMapping showNameMapping = _configurationManager.ShowNameMappings;
            if (showNameMapping?.Mappings?.Count > 0)
            {
                foreach (Mapping mapping in showNameMapping.Mappings)
                {
                    if (mapping.FileShowName.Equals(episode.ShowName))
                    {
                        _logger.TraceMessage($"FixShowName found match {mapping.TVDBShowName} for {episode.SourceFilePath}.", EventLevel.Verbose);
                        if (!string.IsNullOrWhiteSpace(mapping.TVDBShowID))
                        {
                            episode.TVDBShowId = mapping.TVDBShowID;
                        }
                        if (!string.IsNullOrWhiteSpace(mapping.TVDBShowName))
                        {
                            episode.ShowName = mapping.TVDBShowName;
                        }
                        break;
                    }
                }
            }

            _logger.TraceMessage($"FixShowName finished for {episode.SourceFilePath}.", EventLevel.Verbose);
            return episode;
        }

        /// <summary>
        /// Gets a list of possible series that a TVEpisode name could relate to
        /// </summary>
        /// <param name="showName">The showname to be searched</param>
        /// <returns>
        /// A list of series
        /// </returns>
        public async Task<List<DetailView>> GetPossibleShowsForEpisodeAsync(string showName)
        {
            if (string.IsNullOrWhiteSpace(showName))
            {
                throw new ArgumentNullException(nameof(showName));
            }

            return await Task.Run(async () =>
            {
                _logger.TraceMessage($"Get possible matches for show: {showName}.", EventLevel.Verbose);
                ConcurrentBag<DetailView> shows = new ConcurrentBag<DetailView>();
                List<SeriesSearchData> seriesSearchData = await _tvdbManager.SearchSeriesByNameAsync(showName);
                if (seriesSearchData?.Count > 0)
                {
                    Parallel.ForEach(seriesSearchData, _parallelOptions, (series) =>
                    {
                        try
                        {
                            string desc = string.Empty;
                            if (series.Overview?.Length > 50)
                            {
                                desc = string.Format("{0}...", series.Overview.Substring(0, 50));
                            }
                            else if (series.Overview?.Length <= 50)
                            {
                                desc = series.Overview;
                            }
                            string airedDate = DateTime.TryParseExact(series.FirstAired, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt) ? dt.Year.ToString() : "N/A";
                            shows.Add(new DetailView(series.Id.ToString(), series.SeriesName, airedDate, desc));
                        }
                        catch (Exception)
                        {
                            //TODO just swallow this?
                        }
                    });
                }

                _logger.TraceMessage($"Found {shows.Count} possible matches for show: {showName}.", EventLevel.Verbose);
                return shows.ToList();
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
        public async Task<MatchedFile> UpdateEpisodeWithMatchedSeriesAsync(string selectedSeriesId, MatchedFile episode)
        {
            if (episode == null)
            {
                throw new ArgumentNullException(nameof(episode));
            }
            //if no series was selected then set action and skipped and return immediately
            if (string.IsNullOrWhiteSpace(selectedSeriesId))
            {
                _logger.TraceMessage($"No series selected so not updating {episode.SourceFilePath}.", EventLevel.Verbose);
                episode.ActionThis = false;
                episode.SkippedExactSelection = true;
                return episode;
            }

            return await Task.Run(async () =>
            {
                _logger.TraceMessage($"Updating {episode.SourceFilePath} with Matched SeriesId {selectedSeriesId}.", EventLevel.Verbose);
                string originalShowName = episode.ShowName;
                CompleteSeries seriesInfo = await SearchShowByIdAsync(selectedSeriesId);
                episode = UpdateFileWithSeriesDetails(episode, seriesInfo);

                if (!string.IsNullOrWhiteSpace(episode.TVDBShowId))
                {
                    ShowNameMapping showNameMapping = _configurationManager.ShowNameMappings;
                    Mapping map = new Mapping(originalShowName, episode.ShowName, episode.TVDBShowId);
                    if (!showNameMapping.Mappings.Any(x => x.TVDBShowID.Equals(map.TVDBShowID)))
                    {
                        showNameMapping.Mappings.Add(map);
                    }
                    _configurationManager.ShowNameMappings = showNameMapping;
                }
                episode.FileType = FileType.TvShow;

                _logger.TraceMessage($"Updated {episode.SourceFilePath} with Matched SeriesId {selectedSeriesId}.", EventLevel.Verbose);
                return episode;
            });
        }

        /// <summary>
        /// Gets the show with banner asynchronous.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <returns></returns>
        public async Task<(CompleteSeries series, BitmapImage banner)> GetShowWithBannerAsync(string showId)
        {
            if (string.IsNullOrWhiteSpace(showId))
            {
                throw new ArgumentNullException(nameof(showId));
            }

            _logger.TraceMessage($"Getting show and banner for ShowId: {showId}.", EventLevel.Verbose);
            CompleteSeries matchedSeries = await _tvdbManager.GetSeriesByIdAsync(showId);
            BitmapImage bannerImage = null;
            if (matchedSeries?.SeriesBanners?.Count > 0)
            {
                bannerImage = InitializeBannerImage(new Uri(_tvdbManager.GetBannerUri(matchedSeries.SeriesBanners.OrderByDescending(s => s.RatingsInfo.Average).FirstOrDefault().FileName)));
            }
            else
            {
                //TODO create a no image found banner
                bannerImage = new BitmapImage();
            }

            _logger.TraceMessage($"Got show and banner for ShowId: {showId}.", EventLevel.Verbose);
            return (matchedSeries, bannerImage);
        }

        protected virtual BitmapImage InitializeBannerImage(Uri uri)
        {
            BitmapImage banner = new BitmapImage();
            banner.BeginInit();
            banner.UriSource = uri;
            banner.EndInit();

            return banner;
        }

        protected virtual void OnProgressTextChanged(ProgressTextEventArgs e)
        {
            RaiseProgressEvent?.Invoke(this, e);
        }
    }
}