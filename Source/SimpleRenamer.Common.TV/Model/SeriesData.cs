using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace Sarjee.SimpleRenamer.Common.TV.Model
{
    /// <summary>
    /// SeriesData
    /// </summary>
    [DataContract]
    public partial class SeriesData : IEquatable<SeriesData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesData" /> class.
        /// </summary>
        /// <param name="Data">Data.</param>
        /// <param name="Errors">Informative error messages (optional).</param>
        public SeriesData(Series Data = null, JSONErrors Errors = null)
        {
            this.Data = Data;
            this.Errors = Errors;
        }

        /// <summary>
        /// Gets or Sets Data
        /// </summary>
        [DataMember(Name = "data", EmitDefaultValue = false)]
        public Series Data { get; set; }
        /// <summary>
        /// Informative error messages (optional)
        /// </summary>
        /// <value>Informative error messages (optional)</value>
        [DataMember(Name = "errors", EmitDefaultValue = false)]
        public JSONErrors Errors { get; set; }
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SeriesData {\n");
            sb.Append("  Data: ").Append(Data).Append("\n");
            sb.Append("  Errors: ").Append(Errors).Append("\n");
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
            return this.Equals(obj as SeriesData);
        }

        /// <summary>
        /// Returns true if SeriesData instances are equal
        /// </summary>
        /// <param name="other">Instance of SeriesData to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SeriesData other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return
                (
                    this.Data == other.Data ||
                    this.Data != null &&
                    this.Data.Equals(other.Data)
                ) &&
                (
                    this.Errors == other.Errors ||
                    this.Errors != null &&
                    this.Errors.Equals(other.Errors)
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
                if (this.Data != null)
                    hash = hash * 59 + this.Data.GetHashCode();
                if (this.Errors != null)
                    hash = hash * 59 + this.Errors.GetHashCode();
                return hash;
            }
        }
    }
}
