using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace Sarjee.SimpleRenamer.Common.TV.Model
{
    /// <summary>
    /// SeriesImageQueryResult
    /// </summary>
    [DataContract]
    public partial class SeriesImageQueryResult : IEquatable<SeriesImageQueryResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesImageQueryResult" /> class.
        /// </summary>
        /// <param name="Id">Id.</param>
        /// <param name="KeyType">KeyType.</param>
        /// <param name="SubKey">SubKey.</param>
        /// <param name="FileName">FileName.</param>
        /// <param name="LanguageId">LanguageId.</param>
        /// <param name="Resolution">Resolution.</param>
        /// <param name="RatingsInfo">RatingsInfo.</param>
        /// <param name="Thumbnail">Thumbnail.</param>
        public SeriesImageQueryResult(int? Id = null, string KeyType = null, string SubKey = null, string FileName = null, int? LanguageId = null, string Resolution = null, RatingsInfo RatingsInfo = null, string Thumbnail = null)
        {
            this.Id = Id;
            this.KeyType = KeyType;
            this.SubKey = SubKey;
            this.FileName = FileName;
            this.LanguageId = LanguageId;
            this.Resolution = Resolution;
            this.RatingsInfo = RatingsInfo;
            this.Thumbnail = Thumbnail;
        }

        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public int? Id { get; set; }
        /// <summary>
        /// Gets or Sets KeyType
        /// </summary>
        [DataMember(Name = "keyType", EmitDefaultValue = false)]
        public string KeyType { get; set; }
        /// <summary>
        /// Gets or Sets SubKey
        /// </summary>
        [DataMember(Name = "subKey", EmitDefaultValue = false)]
        public string SubKey { get; set; }
        /// <summary>
        /// Gets or Sets FileName
        /// </summary>
        [DataMember(Name = "fileName", EmitDefaultValue = false)]
        public string FileName { get; set; }
        /// <summary>
        /// Gets or Sets LanguageId
        /// </summary>
        [DataMember(Name = "languageId", EmitDefaultValue = false)]
        public int? LanguageId { get; set; }
        /// <summary>
        /// Gets or Sets Resolution
        /// </summary>
        [DataMember(Name = "resolution", EmitDefaultValue = false)]
        public string Resolution { get; set; }
        /// <summary>
        /// Gets or Sets RatingsInfo
        /// </summary>
        [DataMember(Name = "ratingsInfo", EmitDefaultValue = false)]
        public RatingsInfo RatingsInfo { get; set; }
        /// <summary>
        /// Gets or Sets Thumbnail
        /// </summary>
        [DataMember(Name = "thumbnail", EmitDefaultValue = false)]
        public string Thumbnail { get; set; }
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SeriesImageQueryResult {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  KeyType: ").Append(KeyType).Append("\n");
            sb.Append("  SubKey: ").Append(SubKey).Append("\n");
            sb.Append("  FileName: ").Append(FileName).Append("\n");
            sb.Append("  LanguageId: ").Append(LanguageId).Append("\n");
            sb.Append("  Resolution: ").Append(Resolution).Append("\n");
            sb.Append("  RatingsInfo: ").Append(RatingsInfo).Append("\n");
            sb.Append("  Thumbnail: ").Append(Thumbnail).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

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
            return this.Equals(obj as SeriesImageQueryResult);
        }

        /// <summary>
        /// Returns true if SeriesImageQueryResult instances are equal
        /// </summary>
        /// <param name="other">Instance of SeriesImageQueryResult to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SeriesImageQueryResult other)
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
                    this.KeyType == other.KeyType ||
                    this.KeyType != null &&
                    this.KeyType.Equals(other.KeyType)
                ) &&
                (
                    this.SubKey == other.SubKey ||
                    this.SubKey != null &&
                    this.SubKey.Equals(other.SubKey)
                ) &&
                (
                    this.FileName == other.FileName ||
                    this.FileName != null &&
                    this.FileName.Equals(other.FileName)
                ) &&
                (
                    this.LanguageId == other.LanguageId ||
                    this.LanguageId != null &&
                    this.LanguageId.Equals(other.LanguageId)
                ) &&
                (
                    this.Resolution == other.Resolution ||
                    this.Resolution != null &&
                    this.Resolution.Equals(other.Resolution)
                ) &&
                (
                    this.RatingsInfo == other.RatingsInfo ||
                    this.RatingsInfo != null &&
                    this.RatingsInfo.Equals(other.RatingsInfo)
                ) &&
                (
                    this.Thumbnail == other.Thumbnail ||
                    this.Thumbnail != null &&
                    this.Thumbnail.Equals(other.Thumbnail)
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
                if (this.KeyType != null)
                {
                    hash = (hash * 16777619) + this.KeyType.GetHashCode();
                }
                if (this.SubKey != null)
                {
                    hash = (hash * 16777619) + this.SubKey.GetHashCode();
                }
                if (this.FileName != null)
                {
                    hash = (hash * 16777619) + this.FileName.GetHashCode();
                }
                if (this.LanguageId != null)
                {
                    hash = (hash * 16777619) + this.LanguageId.GetHashCode();
                }
                if (this.Resolution != null)
                {
                    hash = (hash * 16777619) + this.Resolution.GetHashCode();
                }
                if (this.RatingsInfo != null)
                {
                    hash = (hash * 16777619) + this.RatingsInfo.GetHashCode();
                }
                if (this.Thumbnail != null)
                {
                    hash = (hash * 16777619) + this.Thumbnail.GetHashCode();
                }
                return hash;
            }
        }
    }
}
