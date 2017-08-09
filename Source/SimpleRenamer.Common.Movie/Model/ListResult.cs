using Newtonsoft.Json;
using System;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// List Result
    /// </summary>
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.ListResult}" />
    public class ListResult : IEquatable<ListResult>
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the favorite count.
        /// </summary>
        /// <value>
        /// The favorite count.
        /// </value>
        [JsonProperty("favorite_count")]
        public int FavoriteCount { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// A language code, e.g. en
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        [JsonProperty("iso_639_1")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Gets or sets the item count.
        /// </summary>
        /// <value>
        /// The item count.
        /// </value>
        [JsonProperty("item_Count")]
        public int ItemCount { get; set; }

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
            return this.Equals(obj as ListResult);
        }

        /// <summary>
        /// Returns true if <see cref="ListResult"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="ListResult"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ListResult other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    this.Description == other.Description ||
                    this.Description != null &&
                    this.Description.Equals(other.Description)
                ) &&
                (
                    this.FavoriteCount == other.FavoriteCount ||
                    this.FavoriteCount.Equals(other.FavoriteCount)
                ) &&
                (
                    this.Id == other.Id ||
                    this.Id.Equals(other.Id)
                ) &&
                (
                    this.LanguageCode == other.LanguageCode ||
                    this.LanguageCode != null &&
                    this.LanguageCode.Equals(other.LanguageCode)
                ) &&
                (
                    this.ItemCount == other.ItemCount ||
                    this.ItemCount.Equals(other.ItemCount)
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
                if (this.Description != null)
                {
                    hash = (hash * 16777619) + this.Description.GetHashCode();
                }
                hash = (hash * 16777619) + this.FavoriteCount.GetHashCode();
                hash = (hash * 16777619) + this.Id.GetHashCode();
                if (this.LanguageCode != null)
                {
                    hash = (hash * 16777619) + this.LanguageCode.GetHashCode();
                }
                hash = (hash * 16777619) + this.ItemCount.GetHashCode();
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
