using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Change Item Deleted
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Movie.Model.ChangeItemBase" />
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.ChangeItemDeleted}" />
    public class ChangeItemDeleted : ChangeItemBase, IEquatable<ChangeItemDeleted>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeItemDeleted"/> class.
        /// </summary>
        public ChangeItemDeleted()
        {
            Action = ChangeAction.Deleted;
        }

        /// <summary>
        /// Gets or sets the original value.
        /// </summary>
        /// <value>
        /// The original value.
        /// </value>
        [JsonProperty("original_value")]
        public JToken OriginalValue { get; set; }

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
            return this.Equals(obj as ChangeItemDeleted);
        }

        /// <summary>
        /// Returns true if <see cref="ChangeItemDeleted"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="ChangeItemDeleted"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ChangeItemDeleted other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    this.OriginalValue == other.OriginalValue ||
                    this.OriginalValue != null &&
                    this.OriginalValue.Equals(other.OriginalValue)
                ) &&
                (
                    base.Equals(other)
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
                int hash = base.GetHashCode();
                // Suitable nullity checks etc, of course :) 
                if (this.OriginalValue != null)
                {
                    hash = (hash * 16777619) + this.OriginalValue.GetHashCode();
                }
                return hash;
            }
        }
        #endregion Equality
    }
}
