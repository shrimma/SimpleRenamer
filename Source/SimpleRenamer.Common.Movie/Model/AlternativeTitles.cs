using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Alternative Titles
    /// </summary>
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.AlternativeTitles}" />
    public class AlternativeTitles : IEquatable<AlternativeTitles>
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
        /// Gets or sets the titles.
        /// </summary>
        /// <value>
        /// The titles.
        /// </value>
        [JsonProperty("titles")]
        public List<AlternativeTitle> Titles { get; set; } = new List<AlternativeTitle>();

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
            return this.Equals(obj as AlternativeTitles);
        }

        /// <summary>
        /// Returns true if <see cref="AlternativeTitles"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="AlternativeTitles"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(AlternativeTitles other)
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
                    this.Titles == other.Titles ||
                    this.Titles != null &&
                    this.Titles.SequenceEqual(other.Titles)
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
                if (this.Titles != null)
                {
                    foreach (var item in this.Titles)
                    {
                        hash = (hash * 16777619) + item.GetHashCode();
                    }
                }
                return hash;
            }
        }
        #endregion Equality
    }
}
