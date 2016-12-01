using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Sarjee.SimpleRenamer.Common.TV.Model
{
    /// <summary>
    /// JSONErrors
    /// </summary>
    [DataContract]
    public partial class JSONErrors : IEquatable<JSONErrors>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JSONErrors" /> class.
        /// </summary>
        /// <param name="InvalidFilters">Invalid filters passed to route.</param>
        /// <param name="InvalidLanguage">Invalid language or translation missing.</param>
        /// <param name="InvalidQueryParams">Invalid query params passed to route.</param>
        public JSONErrors(List<string> InvalidFilters = null, string InvalidLanguage = null, List<string> InvalidQueryParams = null)
        {
            this.InvalidFilters = InvalidFilters;
            this.InvalidLanguage = InvalidLanguage;
            this.InvalidQueryParams = InvalidQueryParams;
        }

        /// <summary>
        /// Invalid filters passed to route
        /// </summary>
        /// <value>Invalid filters passed to route</value>
        [DataMember(Name = "invalidFilters", EmitDefaultValue = false)]
        public List<string> InvalidFilters { get; set; }
        /// <summary>
        /// Invalid language or translation missing
        /// </summary>
        /// <value>Invalid language or translation missing</value>
        [DataMember(Name = "invalidLanguage", EmitDefaultValue = false)]
        public string InvalidLanguage { get; set; }
        /// <summary>
        /// Invalid query params passed to route
        /// </summary>
        /// <value>Invalid query params passed to route</value>
        [DataMember(Name = "invalidQueryParams", EmitDefaultValue = false)]
        public List<string> InvalidQueryParams { get; set; }
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class JSONErrors {\n");
            sb.Append("  InvalidFilters: ").Append(InvalidFilters).Append("\n");
            sb.Append("  InvalidLanguage: ").Append(InvalidLanguage).Append("\n");
            sb.Append("  InvalidQueryParams: ").Append(InvalidQueryParams).Append("\n");
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
            return this.Equals(obj as JSONErrors);
        }

        /// <summary>
        /// Returns true if JSONErrors instances are equal
        /// </summary>
        /// <param name="other">Instance of JSONErrors to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(JSONErrors other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return
                (
                    this.InvalidFilters == other.InvalidFilters ||
                    this.InvalidFilters != null &&
                    this.InvalidFilters.SequenceEqual(other.InvalidFilters)
                ) &&
                (
                    this.InvalidLanguage == other.InvalidLanguage ||
                    this.InvalidLanguage != null &&
                    this.InvalidLanguage.Equals(other.InvalidLanguage)
                ) &&
                (
                    this.InvalidQueryParams == other.InvalidQueryParams ||
                    this.InvalidQueryParams != null &&
                    this.InvalidQueryParams.SequenceEqual(other.InvalidQueryParams)
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
                if (this.InvalidFilters != null)
                    hash = hash * 59 + this.InvalidFilters.GetHashCode();
                if (this.InvalidLanguage != null)
                    hash = hash * 59 + this.InvalidLanguage.GetHashCode();
                if (this.InvalidQueryParams != null)
                    hash = hash * 59 + this.InvalidQueryParams.GetHashCode();
                return hash;
            }
        }
    }
}
