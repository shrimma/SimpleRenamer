using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Change Item Added
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Movie.Model.ChangeItemBase" />
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.ChangeItemAdded}" />
    public class ChangeItemAdded : ChangeItemBase, IEquatable<ChangeItemAdded>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeItemAdded"/> class.
        /// </summary>
        public ChangeItemAdded()
        {
            Action = ChangeAction.Added;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [JsonProperty("value")]
        public JToken Value { get; set; }

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
            return this.Equals(obj as ChangeItemAdded);
        }

        /// <summary>
        /// Returns true if <see cref="ChangeItemAdded"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="ChangeItemAdded"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ChangeItemAdded other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    this.Value == other.Value ||
                    this.Value != null &&
                    this.Value.Equals(other.Value)
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
                if (this.Value != null)
                {
                    hash = (hash * 16777619) + this.Value.GetHashCode();
                }
                return hash;
            }
        }
        #endregion Equality
    }
}
