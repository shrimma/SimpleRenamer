using Newtonsoft.Json;
using System;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Search Movie
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Movie.Model.SearchMovieTvBase" />
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.SearchMovie}" />
    public class SearchMovie : SearchMovieTvBase, IEquatable<SearchMovie>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchMovie"/> class.
        /// </summary>
        public SearchMovie()
        {
            MediaType = MediaType.Movie;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SearchMovie"/> is adult.
        /// </summary>
        /// <value>
        ///   <c>true</c> if adult; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("adult")]
        public bool Adult { get; set; }

        /// <summary>
        /// Gets or sets the original title.
        /// </summary>
        /// <value>
        /// The original title.
        /// </value>
        [JsonProperty("original_title")]
        public string OriginalTitle { get; set; }

        /// <summary>
        /// Gets or sets the release date.
        /// </summary>
        /// <value>
        /// The release date.
        /// </value>
        [JsonProperty("release_date")]
        public DateTime? ReleaseDate { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SearchMovie"/> is video.
        /// </summary>
        /// <value>
        ///   <c>true</c> if video; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("video")]
        public bool Video { get; set; }

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
            return this.Equals(obj as SearchMovie);
        }

        /// <summary>
        /// Returns true if <see cref="SearchMovie"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="SearchMovie"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SearchMovie other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    this.Adult == other.Adult ||
                    this.Adult.Equals(other.Adult)
                ) &&
                (
                    this.OriginalTitle == other.OriginalTitle ||
                    this.OriginalTitle != null &&
                    this.OriginalTitle.Equals(other.OriginalTitle)
                ) &&
                (
                    this.ReleaseDate == other.ReleaseDate ||
                    this.ReleaseDate != null &&
                    this.ReleaseDate.Equals(other.ReleaseDate)
                ) &&
                (
                    this.Title == other.Title ||
                    this.Title != null &&
                    this.Title.Equals(other.Title)
                ) &&
                (
                    this.Video == other.Video ||
                    this.Video.Equals(other.Video)
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
                hash = (hash * 16777619) + this.Adult.GetHashCode();
                if (this.OriginalTitle != null)
                {
                    hash = (hash * 16777619) + this.OriginalTitle.GetHashCode();
                }
                if (this.ReleaseDate != null)
                {
                    hash = (hash * 16777619) + this.ReleaseDate.GetHashCode();
                }
                if (this.Title != null)
                {
                    hash = (hash * 16777619) + this.Title.GetHashCode();
                }
                hash = (hash * 16777619) + this.Video.GetHashCode();
                return hash;
            }
        }
        #endregion Equality
    }
}
