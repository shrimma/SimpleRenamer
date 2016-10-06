using TheTVDBSharp.Models;

namespace SimpleRenamer.Framework.DataModel
{
    public class TVEpisodeScrape
    {
        public MatchedFile tvep { get; set; }
        public Series series { get; set; }

        public TVEpisodeScrape(MatchedFile ep, Series s)
        {
            tvep = ep;
            series = s;
        }

        public TVEpisodeScrape(MatchedFile ep)
        {
            tvep = ep;
        }

        public TVEpisodeScrape()
        {

        }
    }
}
