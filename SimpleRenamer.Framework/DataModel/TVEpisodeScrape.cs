using TheTVDBSharp.Models;

namespace SimpleRenamer.Framework.DataModel
{
    public class TVEpisodeScrape
    {
        public TVEpisode tvep { get; set; }
        public Series series { get; set; }

        public TVEpisodeScrape(TVEpisode ep, Series s)
        {
            tvep = ep;
            series = s;
        }

        public TVEpisodeScrape(TVEpisode ep)
        {
            tvep = ep;
        }

        public TVEpisodeScrape()
        {

        }
    }
}
