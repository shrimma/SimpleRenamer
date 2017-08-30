using Newtonsoft.Json;
using System;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Translation With Country
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Movie.Model.Translation" />
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.TranslationWithCountry}" />
    public class TranslationWithCountry : Translation, IEquatable<TranslationWithCountry>
    {
        /// <summary>
        /// A country code, e.g. US
        /// </summary>
        /// <value>
        /// The country code.
        /// </value>
        [JsonProperty("iso_3166_1")]
        public string CountryCode { get; set; }

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
            return this.Equals(obj as TranslationWithCountry);
        }

        /// <summary>
        /// Returns true if <see cref="TranslationWithCountry"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="TranslationWithCountry"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(TranslationWithCountry other)
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
                    base.Equals(other)
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
                if (this.CountryCode != null)
                {
                    hash = (hash * 16777619) + this.CountryCode.GetHashCode();
                }
                return hash;
            }
        }
        #endregion Equality
    }
}
