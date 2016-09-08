using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TheTVDBSharp;
using TheTVDBSharp.Models;

namespace SimpleRenamer.Framework
{
    public class TVShowMatcher : ITVShowMatcher
    {
        private string apiKey = "820147144A5BB54E";
        private string mappingFilePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "SelectedShowMapping.xml");
        private TaskCompletionSource<bool> taskComplete;
        private string selectedSeriesId;

        /// <summary>
        /// Scrape the TVDB and use the results for a better file name
        /// </summary>
        /// <param name="episode"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public async Task<TVEpisodeScrape> ScrapeDetailsAsync(TVEpisode episode, Settings settings, ShowNameMapping showNameMapping)
        {
            //read the mapping file and try and find any already selected matches
            episode = FixMismatchTitles(episode, settings, showNameMapping);
            TVEpisodeScrape episodeScrape = new TVEpisodeScrape();
            //scrape the episode name - if we haven't already got the show ID then search for it
            if (string.IsNullOrEmpty(episode.TVDBShowId))
            {
                episodeScrape = await ScrapeShowAsync(episode, settings);
            }
            else
            {
                episodeScrape = await ScrapeSpecificShow(episode, settings, episode.TVDBShowId, false);
            }

            //generate the new file name
            episodeScrape.tvep = GenerateFileName(episodeScrape.tvep, settings);
            return episodeScrape;
        }

        /// <summary>
        /// If user has selected a specific show in the past then lets find and automatically use this
        /// </summary>
        /// <param name="episode"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        private TVEpisode FixMismatchTitles(TVEpisode episode, Settings settings, ShowNameMapping showNameMapping)
        {
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
                        return episode;
                    }
                }
            }

            return episode;
        }

        public async Task<ShowNameMapping> ReadMappingFileAsync()
        {
            ShowNameMapping snm = new ShowNameMapping();
            //if the file doesn't yet exist then set a new version
            if (!File.Exists(mappingFilePath))
            {
                return snm;
            }
            else
            {
                using (FileStream fs = new FileStream(mappingFilePath, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ShowNameMapping));
                    snm = (ShowNameMapping)serializer.Deserialize(fs);
                }
                return snm;
            }
        }

        public async Task<bool> WriteMappingFileAsync(ShowNameMapping showNameMapping)
        {
            //only write the file if there is data
            if (showNameMapping != null && showNameMapping.Mappings.Count > 0)
            {
                using (TextWriter writer = new StreamWriter(mappingFilePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ShowNameMapping));
                    serializer.Serialize(writer, showNameMapping);
                }
            }

            return true;
        }

        /// <summary>
        /// Scrape the show details from the given showname, season and episode number
        /// </summary>
        /// <param name="episode">The episode to scrape</param>
        /// <param name="settings">Our current settings</param>
        /// <returns></returns>
        private async Task<TVEpisodeScrape> ScrapeShowAsync(TVEpisode episode, Settings settings)
        {
            TVEpisodeScrape episodeScrape = new TVEpisodeScrape();
            TheTvdbManager tvdbManager = new TheTvdbManager(apiKey);
            var series = await tvdbManager.SearchSeries(episode.ShowName, Language.English);
            var seriesList = series.GetEnumerator();
            string seriesId = string.Empty;
            //IF we have more than 1 result then popup the selection form so user can choose the exact match
            if (series.Count > 1)
            {
                //seriesId = await SelectSpecificShow(episode, settings, series);
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
                    episodeScrape = await ScrapeSpecificShow(episode, settings, seriesId, true);
                }
                else
                {
                    episode.ActionThis = false;
                    episode.SkippedExactSelection = true;
                    episodeScrape.tvep = episode;
                }
            }
            return episodeScrape;
        }

        private async Task<TVEpisodeScrape> ScrapeSpecificShow(TVEpisode episode, Settings settings, string seriesId, bool newMatch)
        {
            uint season = 0;
            uint.TryParse(episode.Season, out season);
            int episodeNumber = 0;
            int.TryParse(episode.Episode, out episodeNumber);
            uint serId = 0;
            uint.TryParse(seriesId, out serId);
            TheTvdbManager tvdbManager = new TheTvdbManager(apiKey);
            Series matchedSeries = await tvdbManager.GetSeries(serId, Language.English);
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

            return new TVEpisodeScrape(episode, matchedSeries);
        }

        public async Task<TVEpisode> SelectShowFromList(TVEpisode episode, Settings settings)
        {
            uint season = 0;
            uint.TryParse(episode.Season, out season);
            int episodeNumber = 0;
            int.TryParse(episode.Episode, out episodeNumber);

            TheTvdbManager tvdbManager = new TheTvdbManager(apiKey);
            var series = await tvdbManager.SearchSeries(episode.ShowName, Language.English);

            taskComplete = new TaskCompletionSource<bool>();
            selectedSeriesId = null;
            SelectShowWpfForm wpfForm = new SelectShowWpfForm();
            wpfForm.ShowViews = new List<ShowView>();
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
                wpfForm.ShowViews.Add(new ShowView(s.Id.ToString(), s.Title, s.FirstAired.Value.Year.ToString(), desc));
            }
            wpfForm.SetView();
            wpfForm.SetTitle(string.Format("Simple TV Renamer - Select Show for file {0}", Path.GetFileName(episode.FilePath)));
            wpfForm.RaiseCustomEvent += new EventHandler<CustomEventArgs>(WindowClosedEvent1);
            wpfForm.ShowDialog();
            await taskComplete.Task;


            TVEpisodeScrape episodeScrape = new TVEpisodeScrape();
            //if user selected a match then scrape the details
            if (!string.IsNullOrEmpty(selectedSeriesId))
            {
                episodeScrape = await ScrapeSpecificShow(episode, settings, selectedSeriesId, true);
                episodeScrape.tvep.ActionThis = true;
                episodeScrape.tvep.SkippedExactSelection = false;

                //generate the file name and update the mapping file
                episodeScrape.tvep = GenerateFileName(episode, settings);

                if (episodeScrape.series != null)
                {
                    ShowNameMapping showNameMapping = TVShowMatcher.ReadMappingFile();
                    Mapping map = new Mapping(episodeScrape.tvep.ShowName, episodeScrape.series.Title, episodeScrape.series.Id.ToString());
                    if (!showNameMapping.Mappings.Any(x => x.TVDBShowID.Equals(map.TVDBShowID)))
                    {
                        showNameMapping.Mappings.Add(map);
                    }
                    TVShowMatcher.WriteMappingFile(showNameMapping);
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

        private void WindowClosedEvent1(object sender, CustomEventArgs e)
        {
            selectedSeriesId = e.ID;
            taskComplete.SetResult(true);
        }

        /// <summary>
        /// Generate the new file name based on the details we have scraped
        /// </summary>
        /// <param name="episode">The episode to rename</param>
        /// <param name="settings">Our current settings</param>
        /// <returns></returns>
        private TVEpisode GenerateFileName(TVEpisode episode, Settings settings)
        {
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
            return episode;
        }

        private static string RemoveSpecialCharacters(string input)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(input, "");
        }
    }
}
