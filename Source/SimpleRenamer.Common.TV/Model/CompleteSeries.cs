using System;
using System.Collections.Generic;
using System.Linq;

namespace Sarjee.SimpleRenamer.Common.TV.Model
{
    public class CompleteSeries : IEquatable<CompleteSeries>
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

        #region Equality
        /// <summary>
        /// Determines whether two <see cref="DlmsCosemSession"/> contain the same values
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(CompleteSeries other)
        {
            if (other == null)
            {
                return false;
            }

            return
                (
                    Series == other.Series ||
                    Series != null &&
                    Series.Equals(other.Series)
                ) &&
                (
                    Actors == other.Actors ||
                    Actors != null &&
                    Actors.SequenceEqual(other.Actors)
                ) &&
                (
                    Episodes == other.Episodes ||
                    Episodes != null &&
                    Episodes.SequenceEqual(other.Episodes)
                ) &&
                (
                    Posters == other.Posters ||
                    Posters != null &&
                    Posters.SequenceEqual(other.Posters)
                ) &&
                (
                    SeasonPosters == other.SeasonPosters ||
                    SeasonPosters != null &&
                    SeasonPosters.SequenceEqual(other.SeasonPosters)
                ) &&
                (
                    SeriesBanners == other.SeriesBanners ||
                    SeriesBanners != null &&
                    SeriesBanners.SequenceEqual(other.SeriesBanners)
                );
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as CompleteSeries);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)2166136261;
                if (Series != null)
                {
                    hashCode = (hashCode * 16777619) + Series.GetHashCode();
                }
                if (Actors != null)
                {
                    foreach (var item in Actors)
                    {
                        hashCode = (hashCode * 16777619) + item.GetHashCode();
                    }
                }
                if (Episodes != null)
                {
                    foreach (var item in Episodes)
                    {
                        hashCode = (hashCode * 16777619) + item.GetHashCode();
                    }
                }
                if (Posters != null)
                {
                    foreach (var item in Posters)
                    {
                        hashCode = (hashCode * 16777619) + item.GetHashCode();
                    }
                }
                if (SeasonPosters != null)
                {
                    foreach (var item in SeasonPosters)
                    {
                        hashCode = (hashCode * 16777619) + item.GetHashCode();
                    }
                }
                if (SeriesBanners != null)
                {
                    foreach (var item in SeriesBanners)
                    {
                        hashCode = (hashCode * 16777619) + item.GetHashCode();
                    }
                }

                return hashCode;
            }
        }
        #endregion Equality
    }
}
