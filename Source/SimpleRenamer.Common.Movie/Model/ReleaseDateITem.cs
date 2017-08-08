using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Release Date Item
    /// </summary>
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.ReleaseDateItem}" />
    public class ReleaseDateItem : IEquatable<ReleaseDateItem>
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
        /// A language code, e.g. en
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        [JsonProperty("iso_639_1")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        /// <value>
        /// The note.
        /// </value>
        [JsonProperty("note")]
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the release date.
        /// </summary>
        /// <value>
        /// The release date.
        /// </value>
        [JsonProperty("release_date")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [JsonProperty("type")]
        public ReleaseDateType Type { get; set; }

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
            return this.Equals(obj as ReleaseDateItem);
        }

        /// <summary>
        /// Returns true if <see cref="ReleaseDateItem"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="ReleaseDateItem"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ReleaseDateItem other)
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
                    this.LanguageCode == other.LanguageCode ||
                    this.LanguageCode != null &&
                    this.LanguageCode.Equals(other.LanguageCode)
                ) &&
                (
                    this.Note == other.Note ||
                    this.Note != null &&
                    this.Note.Equals(other.Note)
                ) &&
                (
                    this.ReleaseDate == other.ReleaseDate ||
                    this.ReleaseDate != null &&
                    this.ReleaseDate.Equals(other.ReleaseDate)
                ) &&
                (
                    this.Type == other.Type ||
                    this.Type.Equals(other.Type)
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
                if (this.LanguageCode != null)
                {
                    hash = (hash * 16777619) + this.LanguageCode.GetHashCode();
                }
                if (this.Note != null)
                {
                    hash = (hash * 16777619) + this.Note.GetHashCode();
                }
                if (this.ReleaseDate != null)
                {
                    hash = (hash * 16777619) + this.ReleaseDate.GetHashCode();
                }
                hash = (hash * 16777619) + this.Type.GetHashCode();
                return hash;
            }
        }
        #endregion Equality
    }
}
