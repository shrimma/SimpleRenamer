using Newtonsoft.Json;
using System;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.Country}" />
    public class Country : IEquatable<Country>
    {
        /// <summary>
        /// Gets or sets the certification.
        /// </summary>
        /// <value>
        /// The certification.
        /// </value>
        [JsonProperty("certification")]
        public string Certification { get; set; }

        /// <summary>
        /// A country code, e.g. US
        /// </summary>
        /// <value>
        /// The country code.
        /// </value>
        [JsonProperty("iso_3166_1")]
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Country"/> is primary.
        /// </summary>
        /// <value>
        ///   <c>true</c> if primary; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("primary")]
        public bool Primary { get; set; }

        /// <summary>
        /// Gets or sets the release date.
        /// </summary>
        /// <value>
        /// The release date.
        /// </value>
        [JsonProperty("release_date")]
        public DateTime? ReleaseDate { get; set; }

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
            return this.Equals(obj as Country);
        }

        /// <summary>
        /// Returns true if <see cref="Country"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="Country"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Country other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    this.Certification == other.Certification ||
                    this.Certification != null &&
                    this.Certification.Equals(other.Certification)
                ) &&
                (
                    this.CountryCode == other.CountryCode ||
                    this.CountryCode != null &&
                    this.CountryCode.Equals(other.CountryCode)
                ) &&
                (
                    this.Primary == other.Primary ||
                    this.Primary.Equals(other.Primary)
                ) &&
                (
                    this.ReleaseDate == other.ReleaseDate ||
                    this.ReleaseDate != null &&
                    this.ReleaseDate.Equals(other.ReleaseDate)
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
                if (this.Certification != null)
                {
                    hash = (hash * 16777619) + this.Certification.GetHashCode();
                }
                if (this.CountryCode != null)
                {
                    hash = (hash * 16777619) + this.CountryCode.GetHashCode();
                }
                hash = (hash * 16777619) + this.Primary.GetHashCode();
                if (this.ReleaseDate != null)
                {
                    hash = (hash * 16777619) + this.ReleaseDate.GetHashCode();
                }
                return hash;
            }
        }
        #endregion Equality
    }
}
