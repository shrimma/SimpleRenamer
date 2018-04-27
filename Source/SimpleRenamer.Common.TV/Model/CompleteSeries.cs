using System;
using System.Collections.Generic;
using System.Linq;

namespace Sarjee.SimpleRenamer.Common.TV.Model
{
    /// <summary>
    /// Complete Series
    /// </summary>
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.TV.Model.CompleteSeries}" />
    public class CompleteSeries : IEquatable<CompleteSeries>
    {
        /// <summary>
        /// Gets or sets the series.
        /// </summary>
        /// <value>
        /// The series.
        /// </value>
        public Series Series { get; set; }
        /// <summary>
        /// Gets or sets the actors.
        /// </summary>
        /// <value>
        /// The actors.
        /// </value>
        public List<SeriesActorsData> Actors { get; set; }
        /// <summary>
        /// Gets or sets the episodes.
        /// </summary>
        /// <value>
        /// The episodes.
        /// </value>
        public List<BasicEpisode> Episodes { get; set; }
        /// <summary>
        /// Gets or sets the posters.
        /// </summary>
        /// <value>
        /// The posters.
        /// </value>
        public List<SeriesImageQueryResult> Posters { get; set; }
        /// <summary>
        /// Gets or sets the season posters.
        /// </summary>
        /// <value>
        /// The season posters.
        /// </value>
        public List<SeriesImageQueryResult> SeasonPosters { get; set; }
        /// <summary>
        /// Gets or sets the series banners.
        /// </summary>
        /// <value>
        /// The series banners.
        /// </value>
        public List<SeriesImageQueryResult> SeriesBanners { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompleteSeries" /> class.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="actors">The actors.</param>
        /// <param name="episodes">The episodes.</param>
        /// <param name="posters">The posters.</param>
        /// <param name="seasonPosters">The season posters.</param>
        /// <param name="seriesBanners">The series banners.</param>
        /// <exception cref="ArgumentNullException">
        /// series
        /// or
        /// actors
        /// or
        /// episodes
        /// or
        /// posters
        /// or
        /// seasonPosters
        /// or
        /// seriesBanners
        /// </exception>
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
        /// Determines whether two <see cref="CompleteSeries"/> contain the same values
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

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as CompleteSeries);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
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
