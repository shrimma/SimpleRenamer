using System.Collections.Generic;

namespace SimpleRenamer.Framework.TvdbModel
{
    public class CompleteSeries
    {
        public SeriesData Series { get; set; }
        public List<SeriesActorsData> Actors { get; set; }
        public List<BasicEpisode> Episodes { get; set; }

        public CompleteSeries(SeriesData series, List<SeriesActorsData> actors, List<BasicEpisode> episodes)
        {
            Series = series;
            Actors = actors;
            Episodes = episodes;
        }
    }
}
