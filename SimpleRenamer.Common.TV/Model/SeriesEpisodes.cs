using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SimpleRenamer.Common.TV.Model
{
    /// <summary>
    /// SeriesEpisodes
    /// </summary>
    [DataContract]
    public partial class SeriesEpisodes : IEquatable<SeriesEpisodes>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesEpisodes" /> class.
        /// </summary>
        /// <param name="Links">Links.</param>
        /// <param name="Data">Data.</param>
        /// <param name="Errors">Errors.</param>
        public SeriesEpisodes(Links Links = null, List<BasicEpisode> Data = null, JSONErrors Errors = null)
        {
            this.Links = Links;
            this.Data = Data;
            this.Errors = Errors;
        }

        /// <summary>
        /// Gets or Sets Links
        /// </summary>
        [DataMember(Name = "links", EmitDefaultValue = false)]
        public Links Links { get; set; }
        /// <summary>
        /// Gets or Sets Data
        /// </summary>
        [DataMember(Name = "data", EmitDefaultValue = false)]
        public List<BasicEpisode> Data { get; set; }
        /// <summary>
        /// Gets or Sets Errors
        /// </summary>
        [DataMember(Name = "errors", EmitDefaultValue = false)]
        public JSONErrors Errors { get; set; }
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SeriesEpisodes {\n");
            sb.Append("  Links: ").Append(Links).Append("\n");
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
            return this.Equals(obj as SeriesEpisodes);
        }

        /// <summary>
        /// Returns true if SeriesEpisodes instances are equal
        /// </summary>
        /// <param name="other">Instance of SeriesEpisodes to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SeriesEpisodes other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return
                (
                    this.Links == other.Links ||
                    this.Links != null &&
                    this.Links.Equals(other.Links)
                ) &&
                (
                    this.Data == other.Data ||
                    this.Data != null &&
                    this.Data.SequenceEqual(other.Data)
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
                if (this.Links != null)
                    hash = hash * 59 + this.Links.GetHashCode();
                if (this.Data != null)
                    hash = hash * 59 + this.Data.GetHashCode();
                if (this.Errors != null)
                    hash = hash * 59 + this.Errors.GetHashCode();
                return hash;
            }
        }
    }
}
