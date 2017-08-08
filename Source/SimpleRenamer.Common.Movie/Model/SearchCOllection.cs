using Newtonsoft.Json;
using System;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.SearchCollection}" />
    public class SearchCollection : IEquatable<SearchCollection>
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
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the poster path.
        /// </summary>
        /// <value>
        /// The poster path.
        /// </value>
        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

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
            return this.Equals(obj as SearchCollection);
        }

        /// <summary>
        /// Returns true if <see cref="SearchCollection"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="SearchCollection"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SearchCollection other)
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
                    this.Id == other.Id ||
                    this.Id.Equals(other.Id)
                ) &&
                (
                    this.Name == other.Name ||
                    this.Name != null &&
                    this.Name.Equals(other.Name)
                ) &&
                (
                    this.PosterPath == other.PosterPath ||
                    this.PosterPath != null &&
                    this.PosterPath.Equals(other.PosterPath)
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
                if (this.BackdropPath != null)
                {
                    hash = (hash * 16777619) + this.BackdropPath.GetHashCode();
                }
                hash = (hash * 16777619) + this.Id.GetHashCode();
                if (this.Name != null)
                {
                    hash = (hash * 16777619) + this.Name.GetHashCode();
                }
                if (this.PosterPath != null)
                {
                    hash = (hash * 16777619) + this.PosterPath.GetHashCode();
                }
                return hash;
            }
        }
        #endregion Equality
    }
}
