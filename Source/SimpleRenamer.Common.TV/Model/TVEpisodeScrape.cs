using Sarjee.SimpleRenamer.Common.Model;

namespace Sarjee.SimpleRenamer.Common.TV.Model
{
    public class TVEpisodeScrape
    {
        public MatchedFile tvep { get; set; }
        public CompleteSeries series { get; set; }

        public TVEpisodeScrape(MatchedFile ep, CompleteSeries s)
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
