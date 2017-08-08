using Newtonsoft.Json;
using System;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Search Base
    /// </summary>
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.SearchBase}" />
    [JsonConverter(typeof(SearchBaseConverter))]
    public class SearchBase : IEquatable<SearchBase>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the type of the media.
        /// </summary>
        /// <value>
        /// The type of the media.
        /// </value>
        [JsonIgnore]
        [JsonProperty("media_type")]
        public MediaType MediaType { get; set; }

        /// <summary>
        /// Gets or sets the popularity.
        /// </summary>
        /// <value>
        /// The popularity.
        /// </value>
        [JsonProperty("popularity")]
        public double Popularity { get; set; }

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
            return this.Equals(obj as SearchBase);
        }

        /// <summary>
        /// Returns true if <see cref="SearchBase"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="SearchBase"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SearchBase other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    this.Id == other.Id ||
                    this.Id.Equals(other.Id)
                ) &&
                (
                    this.MediaType == other.MediaType ||
                    this.MediaType.Equals(other.MediaType)
                ) &&
                (
                    this.Popularity == other.Popularity ||
                    this.Popularity.Equals(other.Popularity)
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
                int hash = (int)2166136261;
                // Suitable nullity checks etc, of course :)                
                hash = (hash * 16777619) + this.Id.GetHashCode();
                hash = (hash * 16777619) + this.MediaType.GetHashCode();
                hash = (hash * 16777619) + this.Popularity.GetHashCode();
                return hash;
            }
        }
        #endregion Equality
    }
}
