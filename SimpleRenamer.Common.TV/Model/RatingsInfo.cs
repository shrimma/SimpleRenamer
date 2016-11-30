using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace SimpleRenamer.Common.TV.Model
{
    /// <summary>
    /// SeriesImageQueryResultRatingsInfo
    /// </summary>
    [DataContract]
    public partial class RatingsInfo : IEquatable<RatingsInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RatingsInfo" /> class.
        /// </summary>
        /// <param name="Average">Average rating for the given record..</param>
        /// <param name="Count">Number of ratings for the given record..</param>
        public RatingsInfo(decimal? Average = null, int? Count = null)
        {
            this.Average = Average;
            this.Count = Count;
        }

        /// <summary>
        /// Average rating for the given record.
        /// </summary>
        /// <value>Average rating for the given record.</value>
        [DataMember(Name = "average", EmitDefaultValue = false)]
        public decimal? Average { get; set; }
        /// <summary>
        /// Number of ratings for the given record.
        /// </summary>
        /// <value>Number of ratings for the given record.</value>
        [DataMember(Name = "count", EmitDefaultValue = false)]
        public int? Count { get; set; }
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SeriesImageQueryResultRatingsInfo {\n");
            sb.Append("  Average: ").Append(Average).Append("\n");
            sb.Append("  Count: ").Append(Count).Append("\n");
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
            return this.Equals(obj as RatingsInfo);
        }

        /// <summary>
        /// Returns true if SeriesImageQueryResultRatingsInfo instances are equal
        /// </summary>
        /// <param name="other">Instance of SeriesImageQueryResultRatingsInfo to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(RatingsInfo other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return
                (
                    this.Average == other.Average ||
                    this.Average != null &&
                    this.Average.Equals(other.Average)
                ) &&
                (
                    this.Count == other.Count ||
                    this.Count != null &&
                    this.Count.Equals(other.Count)
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
                int hash = 41;
                // Suitable nullity checks etc, of course :)
                if (this.Average != null)
                    hash = hash * 59 + this.Average.GetHashCode();
                if (this.Count != null)
                    hash = hash * 59 + this.Count.GetHashCode();
                return hash;
            }
        }
    }
}
