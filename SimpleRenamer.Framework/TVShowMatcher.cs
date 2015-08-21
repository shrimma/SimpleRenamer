using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheTVDBSharp;
using TheTVDBSharp.Models;

namespace SimpleRenamer.Framework
{
    public static class TVShowMatcher
    {
        private static string apiKey = "820147144A5BB54E";
        /// <summary>
        /// Scrape the TVDB and use the results for a better file name
        /// </summary>
        /// <param name="episode"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static async Task<TVEpisode> ScrapeDetailsAsync(TVEpisode episode, Settings settings)
        {
            //scrape the episode name
            episode = await ScrapeShowAsync(episode, settings);

            //generate the new file name
            episode = GenerateFileName(episode, settings);

            return episode;
        }

        /// <summary>
        /// Scrape the show details from the given showname, season and episode number
        /// </summary>
        /// <param name="episode">The episode to scrape</param>
        /// <param name="settings">Our current settings</param>
        /// <returns></returns>
        public static async Task<TVEpisode> ScrapeShowAsync(TVEpisode episode, Settings settings)
        {
            uint season = 0;
            uint.TryParse(episode.Season, out season);
            int episodeNumber = 0;
            int.TryParse(episode.Episode, out episodeNumber);
            TheTvdbManager tvdbManager = new TheTvdbManager(apiKey);
            var series = await tvdbManager.SearchSeries(episode.ShowName, Language.English);
            var seriesList = series.GetEnumerator();
            string seriesId = string.Empty;
            //IF we have more than 1 result then popup the selection form so user can choose the exact match
            if (series.Count > 1)
            {
                seriesId = await SelectSpecificShow(episode, settings, series);
            }
            else if (series.Count == 1)
            {
                //if theres only one match then it is easy
                seriesList.MoveNext();
                seriesId = seriesList.Current.Id.ToString();
            }

            //if seriesID is populated then grab the episode name (it's possible to be null if user skipped the selection
            if (!string.IsNullOrEmpty(seriesId))
            {
                uint serId = 0;
                uint.TryParse(seriesId, out serId);
                Series matchedSeries = await tvdbManager.GetSeries(serId, Language.English);
                episode.EpisodeName = matchedSeries.Episodes.Where(s => s.SeasonNumber.Value == season && s.Number == episodeNumber).FirstOrDefault().Title;
            }
            else
            {
                episode.SkippedExactSelection = true;
            }

            return episode;
        }

        public static TaskCompletionSource<bool> taskComplete;
        public static string selectedSeriesId;
        public static async Task<string> SelectSpecificShow(TVEpisode episode, Settings settings, IReadOnlyCollection<Series> series)
        {
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
            wpfForm.RaiseCustomEvent += new EventHandler<CustomEventArgs>(WindowClosedEvent1);
            wpfForm.Show();
            await taskComplete.Task;
            return selectedSeriesId;
        }

        public static void WindowClosedEvent1(object sender, CustomEventArgs e)
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
        public static TVEpisode GenerateFileName(TVEpisode episode, Settings settings)
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
                temp = temp.Replace("{EpisodeName}", episode.EpisodeName);
            }
            episode.NewFileName = temp;
            return episode;
        }
    }
}
