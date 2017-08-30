using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Release Dates Container
    /// </summary>
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.ReleaseDatesContainer}" />
    public class ReleaseDatesContainer : IEquatable<ReleaseDatesContainer>
    {
        /// <summary>
        /// A country code, e.g. US
        /// </summary>
        /// <value>
        /// The country code.
        /// </value>
        [JsonProperty("iso_3166_1")]
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the release dates.
        /// </summary>
        /// <value>
        /// The release dates.
        /// </value>
        [JsonProperty("release_dates")]
        public List<ReleaseDateItem> ReleaseDates { get; set; } = new List<ReleaseDateItem>();

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
            return this.Equals(obj as ReleaseDatesContainer);
        }

        /// <summary>
        /// Returns true if <see cref="ReleaseDatesContainer"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="ReleaseDatesContainer"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ReleaseDatesContainer other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    this.CountryCode == other.CountryCode ||
                    this.CountryCode != null &&
                    this.CountryCode.Equals(other.CountryCode)
                ) &&
                (
                    this.ReleaseDates == other.ReleaseDates ||
                    this.ReleaseDates != null &&
                    this.ReleaseDates.SequenceEqual(other.ReleaseDates)
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
                if (this.CountryCode != null)
                {
                    hash = (hash * 16777619) + this.CountryCode.GetHashCode();
                }
                if (this.ReleaseDates != null)
                {
                    foreach (var item in ReleaseDates)
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
