using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Translations Container
    /// </summary>
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.TranslationsContainer}" />
    public class TranslationsContainer : IEquatable<TranslationsContainer>
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
        /// Gets or sets the translations.
        /// </summary>
        /// <value>
        /// The translations.
        /// </value>
        [JsonProperty("translations")]
        public List<TranslationWithCountry> Translations { get; set; } = new List<TranslationWithCountry>();

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
            return this.Equals(obj as TranslationsContainer);
        }

        /// <summary>
        /// Returns true if <see cref="TranslationsContainer"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="TranslationsContainer"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(TranslationsContainer other)
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
                    this.Translations == other.Translations ||
                    this.Translations != null &&
                    this.Translations.SequenceEqual(other.Translations)
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
                if (this.Translations != null)
                {
                    foreach (var item in Translations)
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
