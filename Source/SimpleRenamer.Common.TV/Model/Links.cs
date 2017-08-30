using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace Sarjee.SimpleRenamer.Common.TV.Model
{
    /// <summary>
    /// Links
    /// </summary>
    [DataContract]
    public partial class Links : IEquatable<Links>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Links" /> class.
        /// </summary>
        /// <param name="First">First.</param>
        /// <param name="Last">Last.</param>
        /// <param name="Next">Next.</param>
        /// <param name="Previous">Previous.</param>
        public Links(int? First = null, int? Last = null, int? Next = null, int? Previous = null)
        {
            this.First = First;
            this.Last = Last;
            this.Next = Next;
            this.Previous = Previous;
        }

        /// <summary>
        /// Gets or Sets First
        /// </summary>
        [DataMember(Name = "first", EmitDefaultValue = false)]
        public int? First { get; set; }
        /// <summary>
        /// Gets or Sets Last
        /// </summary>
        [DataMember(Name = "last", EmitDefaultValue = false)]
        public int? Last { get; set; }
        /// <summary>
        /// Gets or Sets Next
        /// </summary>
        [DataMember(Name = "next", EmitDefaultValue = false)]
        public int? Next { get; set; }
        /// <summary>
        /// Gets or Sets Previous
        /// </summary>
        [DataMember(Name = "previous", EmitDefaultValue = false)]
        public int? Previous { get; set; }
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Links {\n");
            sb.Append("  First: ").Append(First).Append("\n");
            sb.Append("  Last: ").Append(Last).Append("\n");
            sb.Append("  Next: ").Append(Next).Append("\n");
            sb.Append("  Previous: ").Append(Previous).Append("\n");
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
            return this.Equals(obj as Links);
        }

        /// <summary>
        /// Returns true if Links instances are equal
        /// </summary>
        /// <param name="other">Instance of Links to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Links other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    this.First == other.First ||
                    this.First != null &&
                    this.First.Equals(other.First)
                ) &&
                (
                    this.Last == other.Last ||
                    this.Last != null &&
                    this.Last.Equals(other.Last)
                ) &&
                (
                    this.Next == other.Next ||
                    this.Next != null &&
                    this.Next.Equals(other.Next)
                ) &&
                (
                    this.Previous == other.Previous ||
                    this.Previous != null &&
                    this.Previous.Equals(other.Previous)
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
                if (this.First != null)
                {
                    hash = (hash * 16777619) + this.First.GetHashCode();
                }
                if (this.Last != null)
                {
                    hash = (hash * 16777619) + this.Last.GetHashCode();
                }
                if (this.Next != null)
                {
                    hash = (hash * 16777619) + this.Next.GetHashCode();
                }
                if (this.Previous != null)
                {
                    hash = (hash * 16777619) + this.Previous.GetHashCode();
                }
                return hash;
            }
        }
        #endregion Equality
    }
}
