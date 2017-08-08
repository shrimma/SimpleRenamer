using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Search Container
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.SearchContainer{T}}" />
    public class SearchContainer<T> : IEquatable<SearchContainer<T>>
    {
        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        /// <value>
        /// The page.
        /// </value>
        [JsonProperty("page")]
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>
        /// The results.
        /// </value>
        [JsonProperty("results")]
        public List<T> Results { get; set; }

        /// <summary>
        /// Gets or sets the total pages.
        /// </summary>
        /// <value>
        /// The total pages.
        /// </value>
        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the total results.
        /// </summary>
        /// <value>
        /// The total results.
        /// </value>
        [JsonProperty("total_results")]
        public int TotalResults { get; set; }

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
            return this.Equals(obj as SearchContainer<T>);
        }

        /// <summary>
        /// Returns true if <see cref="SearchContainer<T>"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="SearchContainer<T>"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SearchContainer<T> other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    this.Page == other.Page ||
                    this.Page.Equals(other.Page)
                ) &&
                (
                    this.Results == other.Results ||
                    this.Results != null &&
                    this.Results.SequenceEqual(other.Results)
                ) &&
                (
                    this.TotalPages == other.TotalPages ||
                    this.TotalPages.Equals(other.TotalPages)
                ) &&
                (
                    this.TotalResults == other.TotalResults ||
                    this.TotalResults.Equals(other.TotalResults)
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
                hash = (hash * 16777619) + this.Page.GetHashCode();
                if (this.Results != null)
                {
                    foreach (var item in Results)
                    {
                        hash = (hash * 16777619) + item.GetHashCode();
                    }
                }
                hash = (hash * 16777619) + this.TotalPages.GetHashCode();
                hash = (hash * 16777619) + this.TotalResults.GetHashCode();
                return hash;
            }
        }
        #endregion Equality
    }
}
