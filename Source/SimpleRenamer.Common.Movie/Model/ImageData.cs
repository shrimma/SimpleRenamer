using Newtonsoft.Json;
using System;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Image Data
    /// </summary>
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.ImageData}" />
    public class ImageData : IEquatable<ImageData>
    {
        /// <summary>
        /// Gets or sets the aspect ratio.
        /// </summary>
        /// <value>
        /// The aspect ratio.
        /// </value>
        [JsonProperty("aspect_ratio")]
        public double AspectRatio { get; set; }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        [JsonProperty("file_path")]
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        [JsonProperty("height")]
        public int Height { get; set; }

        /// <summary>
        /// A language code, e.g. en
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        [JsonProperty("iso_639_1")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Gets or sets the vote average.
        /// </summary>
        /// <value>
        /// The vote average.
        /// </value>
        [JsonProperty("vote_average")]
        public double VoteAverage { get; set; }

        /// <summary>
        /// Gets or sets the vote count.
        /// </summary>
        /// <value>
        /// The vote count.
        /// </value>
        [JsonProperty("vote_count")]
        public int VoteCount { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        [JsonProperty("width")]
        public int Width { get; set; }

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
            return this.Equals(obj as ImageData);
        }

        /// <summary>
        /// Returns true if <see cref="ImageData"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="ImageData"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ImageData other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    this.AspectRatio == other.AspectRatio ||
                    this.AspectRatio.Equals(other.AspectRatio)
                ) &&
                (
                    this.FilePath == other.FilePath ||
                    this.FilePath != null &&
                    this.FilePath.Equals(other.FilePath)
                ) &&
                (
                    this.Height == other.Height ||
                    this.Height.Equals(other.Height)
                ) &&
                (
                    this.LanguageCode == other.LanguageCode ||
                    this.LanguageCode != null &&
                    this.LanguageCode.Equals(other.LanguageCode)
                ) &&
                (
                    this.VoteAverage == other.VoteAverage ||
                    this.VoteAverage.Equals(other.VoteAverage)
                ) &&
                (
                    this.VoteCount == other.VoteCount ||
                    this.VoteCount.Equals(other.VoteCount)
                ) &&
                (
                    this.Width == other.Width ||
                    this.Width.Equals(other.Width)
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
                hash = (hash * 16777619) + this.AspectRatio.GetHashCode();
                if (this.FilePath != null)
                {
                    hash = (hash * 16777619) + this.FilePath.GetHashCode();
                }
                hash = (hash * 16777619) + this.Height.GetHashCode();
                if (this.LanguageCode != null)
                {
                    hash = (hash * 16777619) + this.LanguageCode.GetHashCode();
                }
                hash = (hash * 16777619) + this.VoteAverage.GetHashCode();
                hash = (hash * 16777619) + this.VoteCount.GetHashCode();
                hash = (hash * 16777619) + this.Width.GetHashCode();
                return hash;
            }
        }
        #endregion Equality
    }
}
