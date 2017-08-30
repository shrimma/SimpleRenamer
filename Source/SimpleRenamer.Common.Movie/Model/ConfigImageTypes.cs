using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Config Image Types
    /// </summary>
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.ConfigImageTypes}" />
    public class ConfigImageTypes : IEquatable<ConfigImageTypes>
    {
        /// <summary>
        /// Gets or sets the backdrop sizes.
        /// </summary>
        /// <value>
        /// The backdrop sizes.
        /// </value>
        [JsonProperty("backdrop_sizes")]
        public List<string> BackdropSizes { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the base URL.
        /// </summary>
        /// <value>
        /// The base URL.
        /// </value>
        [JsonProperty("base_url")]
        public string BaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the logo sizes.
        /// </summary>
        /// <value>
        /// The logo sizes.
        /// </value>
        [JsonProperty("logo_sizes")]
        public List<string> LogoSizes { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the poster sizes.
        /// </summary>
        /// <value>
        /// The poster sizes.
        /// </value>
        [JsonProperty("poster_sizes")]
        public List<string> PosterSizes { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the profile sizes.
        /// </summary>
        /// <value>
        /// The profile sizes.
        /// </value>
        [JsonProperty("profile_sizes")]
        public List<string> ProfileSizes { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the secure base URL.
        /// </summary>
        /// <value>
        /// The secure base URL.
        /// </value>
        [JsonProperty("secure_base_url")]
        public string SecureBaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the still sizes.
        /// </summary>
        /// <value>
        /// The still sizes.
        /// </value>
        [JsonProperty("still_sizes")]
        public List<string> StillSizes { get; set; } = new List<string>();

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
            return this.Equals(obj as ConfigImageTypes);
        }

        /// <summary>
        /// Returns true if <see cref="ConfigImageTypes"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="ConfigImageTypes"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ConfigImageTypes other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    this.BackdropSizes == other.BackdropSizes ||
                    this.BackdropSizes != null &&
                    this.BackdropSizes.SequenceEqual(other.BackdropSizes)
                ) &&
                (
                    this.BaseUrl == other.BaseUrl ||
                    this.BaseUrl != null &&
                    this.BaseUrl.Equals(other.BaseUrl)
                ) &&
                (
                    this.LogoSizes == other.LogoSizes ||
                    this.LogoSizes != null &&
                    this.LogoSizes.SequenceEqual(other.LogoSizes)
                ) &&
                (
                    this.PosterSizes == other.PosterSizes ||
                    this.PosterSizes != null &&
                    this.PosterSizes.SequenceEqual(other.PosterSizes)
                ) &&
                (
                    this.ProfileSizes == other.ProfileSizes ||
                    this.ProfileSizes != null &&
                    this.ProfileSizes.SequenceEqual(other.ProfileSizes)
                ) &&
                (
                    this.SecureBaseUrl == other.SecureBaseUrl ||
                    this.SecureBaseUrl != null &&
                    this.SecureBaseUrl.Equals(other.SecureBaseUrl)
                ) &&
                (
                    this.StillSizes == other.StillSizes ||
                    this.StillSizes != null &&
                    this.StillSizes.SequenceEqual(other.StillSizes)
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
                if (this.BackdropSizes != null)
                {
                    foreach (var item in this.BackdropSizes)
                    {
                        hash = (hash * 16777619) + item.GetHashCode();
                    }
                }
                if (this.BaseUrl != null)
                {
                    hash = (hash * 16777619) + this.BaseUrl.GetHashCode();
                }
                if (this.LogoSizes != null)
                {
                    foreach (var item in this.LogoSizes)
                    {
                        hash = (hash * 16777619) + item.GetHashCode();
                    }
                }
                if (this.PosterSizes != null)
                {
                    foreach (var item in this.PosterSizes)
                    {
                        hash = (hash * 16777619) + item.GetHashCode();
                    }
                }
                if (this.ProfileSizes != null)
                {
                    foreach (var item in this.ProfileSizes)
                    {
                        hash = (hash * 16777619) + item.GetHashCode();
                    }
                }
                if (this.SecureBaseUrl != null)
                {
                    hash = (hash * 16777619) + this.SecureBaseUrl.GetHashCode();
                }
                if (this.StillSizes != null)
                {
                    foreach (var item in this.StillSizes)
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
