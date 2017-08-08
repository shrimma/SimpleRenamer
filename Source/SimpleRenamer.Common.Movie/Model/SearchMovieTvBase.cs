using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Search Movie TV Base
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Movie.Model.SearchBase" />
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.SearchMovieTvBase}" />
    public class SearchMovieTvBase : SearchBase, IEquatable<SearchMovieTvBase>
    {
        /// <summary>
        /// Gets or sets the backdrop path.
        /// </summary>
        /// <value>
        /// The backdrop path.
        /// </value>
        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }

        /// <summary>
        /// Gets or sets the genre ids.
        /// </summary>
        /// <value>
        /// The genre ids.
        /// </value>
        [JsonProperty("genre_ids")]
        public List<int> GenreIds { get; set; }

        /// <summary>
        /// Gets or sets the original language.
        /// </summary>
        /// <value>
        /// The original language.
        /// </value>
        [JsonProperty("original_language")]
        public string OriginalLanguage { get; set; }

        /// <summary>
        /// Gets or sets the overview.
        /// </summary>
        /// <value>
        /// The overview.
        /// </value>
        [JsonProperty("overview")]
        public string Overview { get; set; }

        /// <summary>
        /// Gets or sets the poster path.
        /// </summary>
        /// <value>
        /// The poster path.
        /// </value>
        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        /// <summary>
        /// Gets or sets the vote average.
        /// </summary>
        /// <value>
        /// The vote average.
        /// </value>
        [JsonProperty("vote_average")]
        public double VoteAverage { get; set; }

        /// <summary>
        /// Gets or sets the vote count.
        /// </summary>
        /// <value>
        /// The vote count.
        /// </value>
        [JsonProperty("vote_count")]
        public int VoteCount { get; set; }

        #region Equality
        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return this.Equals(obj as SearchMovieTvBase);
        }

        /// <summary>
        /// Returns true if <see cref="SearchMovieTvBase"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="SearchMovieTvBase"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SearchMovieTvBase other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    this.BackdropPath == other.BackdropPath ||
                    this.BackdropPath != null &&
                    this.BackdropPath.Equals(other.BackdropPath)
                ) &&
                (
                    this.GenreIds == other.GenreIds ||
                    this.GenreIds != null &&
                    this.GenreIds.SequenceEqual(other.GenreIds)
                ) &&
                (
                    this.OriginalLanguage == other.OriginalLanguage ||
                    this.OriginalLanguage != null &&
                    this.OriginalLanguage.Equals(other.OriginalLanguage)
                ) &&
                (
                    this.Overview == other.Overview ||
                    this.Overview != null &&
                    this.Overview.Equals(other.Overview)
                ) &&
                (
                    this.PosterPath == other.PosterPath ||
                    this.PosterPath != null &&
                    this.PosterPath.Equals(other.PosterPath)
                ) &&
                (
                    this.VoteAverage == other.VoteAverage ||
                    this.VoteAverage.Equals(other.VoteAverage)
                ) &&
                (
                    this.VoteCount == other.VoteCount ||
                    this.VoteCount.Equals(other.VoteCount)
                ) &&
                (
                    base.Equals(other)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            // credit: http://stackoverflow.com/a/263416/677735
            unchecked // Overflow is fine, just wrap
            {
                int hash = base.GetHashCode();
                // Suitable nullity checks etc, of course :)
                if (this.BackdropPath != null)
                {
                    hash = (hash * 16777619) + this.BackdropPath.GetHashCode();
                }
                if (this.GenreIds != null)
                {
                    foreach (var item in GenreIds)
                    {
                        hash = (hash * 16777619) + item.GetHashCode();
                    }
                }
                if (this.OriginalLanguage != null)
                {
                    hash = (hash * 16777619) + this.OriginalLanguage.GetHashCode();
                }
                if (this.Overview != null)
                {
                    hash = (hash * 16777619) + this.Overview.GetHashCode();
                }
                if (this.PosterPath != null)
                {
                    hash = (hash * 16777619) + this.PosterPath.GetHashCode();
                }
                hash = (hash * 16777619) + this.VoteAverage.GetHashCode();
                hash = (hash * 16777619) + this.VoteCount.GetHashCode();
                return hash;
            }
        }
        #endregion Equality
    }
}
