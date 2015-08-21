
namespace SimpleRenamer.Framework
{
    public static class TVShowMatcher
    {
        /// <summary>
        /// Scrape the TVDB and use the results for a better file name
        /// </summary>
        /// <param name="episode"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static TVEpisode ScrapeDetails(TVEpisode episode, Settings settings)
        {
            //scrape the episode name
            episode = ScrapeShow(episode, settings);
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
        public static TVEpisode ScrapeShow(TVEpisode episode, Settings settings)
        {
            return episode;
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
