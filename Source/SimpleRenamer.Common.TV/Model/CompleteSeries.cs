using System.Collections.Generic;

namespace Sarjee.SimpleRenamer.Common.TV.Model
{
    public class CompleteSeries
    {
        public Series Series { get; set; }
        public List<SeriesActorsData> Actors { get; set; }
        public List<BasicEpisode> Episodes { get; set; }
        public List<SeriesImageQueryResult> Posters { get; set; }
        public List<SeriesImageQueryResult> SeasonPosters { get; set; }
        public List<SeriesImageQueryResult> SeriesBanners { get; set; }

        public CompleteSeries(Series series, List<SeriesActorsData> actors, List<BasicEpisode> episodes, List<SeriesImageQueryResult> posters, List<SeriesImageQueryResult> seasonPosters, List<SeriesImageQueryResult> seriesBanners)
        {
            Series = series;
            Actors = actors;
            Episodes = episodes;
            Posters = posters;
            SeasonPosters = seasonPosters;
            SeriesBanners = seriesBanners;
        }
    }
}
