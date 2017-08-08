using Newtonsoft.Json;
using System;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Spoken Language
    /// </summary>
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.SpokenLanguage}" />
    public class SpokenLanguage : IEquatable<SpokenLanguage>
    {
        /// <summary>
        /// A language code, e.g. en
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        [JsonProperty("iso_639_1")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonProperty("name")]
        public string Name { get; set; }

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
            return this.Equals(obj as SpokenLanguage);
        }

        /// <summary>
        /// Returns true if <see cref="SpokenLanguage"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="SpokenLanguage"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SpokenLanguage other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    this.LanguageCode == other.LanguageCode ||
                    this.LanguageCode != null &&
                    this.LanguageCode.Equals(other.LanguageCode)
                ) &&
                (
                    this.Name == other.Name ||
                    this.Name != null &&
                    this.Name.Equals(other.Name)
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
                if (this.LanguageCode != null)
                {
                    hash = (hash * 16777619) + this.LanguageCode.GetHashCode();
                }
                if (this.Name != null)
                {
                    hash = (hash * 16777619) + this.Name.GetHashCode();
                }
                return hash;
            }
        }
        #endregion Equality
    }
}
