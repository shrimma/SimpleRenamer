using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// TMDb Config
    /// </summary>
    public class TMDbConfig : IEquatable<TMDbConfig>
    {
        /// <summary>
        /// Gets or sets the change keys.
        /// </summary>
        /// <value>
        /// The change keys.
        /// </value>
        [JsonProperty("change_keys")]
        public List<string> ChangeKeys { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the images.
        /// </summary>
        /// <value>
        /// The images.
        /// </value>
        [JsonProperty("images")]
        public ConfigImageTypes Images { get; set; }

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
            return this.Equals(obj as TMDbConfig);
        }

        /// <summary>
        /// Returns true if <see cref="TMDbConfig"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="TMDbConfig"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(TMDbConfig other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    this.ChangeKeys == other.ChangeKeys ||
                    this.ChangeKeys != null &&
                    this.ChangeKeys.SequenceEqual(other.ChangeKeys)
                ) &&
                (
                    this.Images == other.Images ||
                    this.Images != null &&
                    this.Images.Equals(other.Images)
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
                if (this.ChangeKeys != null)
                {
                    foreach (var item in ChangeKeys)
                    {
                        hash = (hash * 16777619) + item.GetHashCode();
                    }
                }
                if (this.Images != null)
                {
                    hash = (hash * 16777619) + this.Images.GetHashCode();
                }
                return hash;
            }
        }
        #endregion Equality
    }
}
