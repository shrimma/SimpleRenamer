using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Keywords Container
    /// </summary>
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.KeywordsContainer}" />
    public class KeywordsContainer : IEquatable<KeywordsContainer>
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
        /// Gets or sets the keywords.
        /// </summary>
        /// <value>
        /// The keywords.
        /// </value>
        [JsonProperty("keywords")]
        public List<Keyword> Keywords { get; set; } = new List<Keyword>();

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
            return this.Equals(obj as KeywordsContainer);
        }

        /// <summary>
        /// Returns true if <see cref="KeywordsContainer"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="KeywordsContainer"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(KeywordsContainer other)
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
                    this.Keywords == other.Keywords ||
                    this.Keywords != null &&
                    this.Keywords.SequenceEqual(other.Keywords)
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
                if (this.Keywords != null)
                {
                    foreach (var item in Keywords)
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
