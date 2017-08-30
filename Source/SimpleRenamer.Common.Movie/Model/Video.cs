using Newtonsoft.Json;
using System;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Video
    /// </summary>
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.Video}" />
    public class Video : IEquatable<Video>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// A country code, e.g. US
        /// </summary>
        /// <value>
        /// The country code.
        /// </value>
        [JsonProperty("iso_3166_1")]
        public string CountryCode { get; set; }

        /// <summary>
        /// A language code, e.g. en
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        [JsonProperty("iso_639_1")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the site.
        /// </summary>
        /// <value>
        /// The site.
        /// </value>
        [JsonProperty("site")]
        public string Site { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        [JsonProperty("size")]
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [JsonProperty("type")]
        public string Type { get; set; }

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
            return this.Equals(obj as Video);
        }

        /// <summary>
        /// Returns true if <see cref="Video"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="Video"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Video other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    this.Id == other.Id ||
                    this.Id != null &&
                    this.Id.Equals(other.Id)
                ) &&
                (
                    this.CountryCode == other.CountryCode ||
                    this.CountryCode != null &&
                    this.CountryCode.Equals(other.CountryCode)
                ) &&
                (
                    this.LanguageCode == other.LanguageCode ||
                    this.LanguageCode != null &&
                    this.LanguageCode.Equals(other.LanguageCode)
                ) &&
                (
                    this.Key == other.Key ||
                    this.Key != null &&
                    this.Key.Equals(other.Key)
                ) &&
                (
                    this.Name == other.Name ||
                    this.Name != null &&
                    this.Name.Equals(other.Name)
                ) &&
                (
                    this.Site == other.Site ||
                    this.Site != null &&
                    this.Site.Equals(other.Site)
                ) &&
                (
                    this.Size == other.Size ||
                    this.Size.Equals(other.Size)
                ) &&
                (
                    this.Type == other.Type ||
                    this.Type != null &&
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
                if (this.Id != null)
                {
                    hash = (hash * 16777619) + this.Id.GetHashCode();
                }
                if (this.CountryCode != null)
                {
                    hash = (hash * 16777619) + this.CountryCode.GetHashCode();
                }
                if (this.LanguageCode != null)
                {
                    hash = (hash * 16777619) + this.LanguageCode.GetHashCode();
                }
                if (this.Key != null)
                {
                    hash = (hash * 16777619) + this.Key.GetHashCode();
                }
                if (this.Name != null)
                {
                    hash = (hash * 16777619) + this.Name.GetHashCode();
                }
                if (this.Site != null)
                {
                    hash = (hash * 16777619) + this.Site.GetHashCode();
                }
                hash = (hash * 16777619) + this.Size.GetHashCode();
                if (this.Type != null)
                {
                    hash = (hash * 16777619) + this.Type.GetHashCode();
                }
                return hash;
            }
        }
        #endregion Equality
    }
}
