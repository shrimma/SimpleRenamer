using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Change
    /// </summary>
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.Change}" />
    public class Change : IEquatable<Change>
    {
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        [JsonProperty("items")]
        public List<ChangeItemBase> Items { get; set; } = new List<ChangeItemBase>();

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        [JsonProperty("key")]
        public string Key { get; set; }

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
            return this.Equals(obj as Change);
        }

        /// <summary>
        /// Returns true if <see cref="Change"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="Change"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Change other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    this.Items == other.Items ||
                    this.Items != null &&
                    this.Items.SequenceEqual(other.Items)
                ) &&
                (
                    this.Key == other.Key ||
                    this.Key != null &&
                    this.Key.Equals(other.Key)
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
                if (this.Items != null)
                {
                    foreach (var item in this.Items)
                    {
                        hash = (hash * 16777619) + item.GetHashCode();
                    }
                }
                if (this.Key != null)
                {
                    hash = (hash * 16777619) + this.Key.GetHashCode();
                }
                return hash;
            }
        }
        #endregion Equality
    }
}
