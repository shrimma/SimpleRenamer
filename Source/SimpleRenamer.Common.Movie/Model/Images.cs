using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Images
    /// </summary>
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.Images}" />
    public class Images : IEquatable<Images>
    {
        /// <summary>
        /// Gets or sets the backdrops.
        /// </summary>
        /// <value>
        /// The backdrops.
        /// </value>
        [JsonProperty("backdrops")]
        public List<ImageData> Backdrops { get; set; } = new List<ImageData>();

        /// <summary>
        /// Gets or sets the posters.
        /// </summary>
        /// <value>
        /// The posters.
        /// </value>
        [JsonProperty("posters")]
        public List<ImageData> Posters { get; set; } = new List<ImageData>();

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
            return this.Equals(obj as Images);
        }

        /// <summary>
        /// Returns true if <see cref="Images"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="Images"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Images other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    this.Backdrops == other.Backdrops ||
                    this.Backdrops != null &&
                    this.Backdrops.SequenceEqual(other.Backdrops)
                ) &&
                (
                    this.Posters == other.Posters ||
                    this.Posters != null &&
                    this.Posters.SequenceEqual(other.Posters)
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
                if (this.Backdrops != null)
                {
                    foreach (var item in Backdrops)
                    {
                        hash = (hash * 16777619) + item.GetHashCode();
                    }
                }
                if (this.Posters != null)
                {
                    foreach (var item in Posters)
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
