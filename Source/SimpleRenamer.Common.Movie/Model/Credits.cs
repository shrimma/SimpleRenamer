using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Credits
    /// </summary>
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.Credits}" />
    public class Credits : IEquatable<Credits>
    {
        /// <summary>
        /// Gets or sets the cast.
        /// </summary>
        /// <value>
        /// The cast.
        /// </value>
        [JsonProperty("cast")]
        public List<Cast> Cast { get; set; } = new List<Cast>();

        /// <summary>
        /// Gets or sets the crew.
        /// </summary>
        /// <value>
        /// The crew.
        /// </value>
        [JsonProperty("crew")]
        public List<Crew> Crew { get; set; } = new List<Crew>();

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("id")]
        public int Id { get; set; }

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
            return this.Equals(obj as Credits);
        }

        /// <summary>
        /// Returns true if <see cref="Credits"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="Credits"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Credits other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    this.Cast == other.Cast ||
                    this.Cast != null &&
                    this.Cast.SequenceEqual(other.Cast)
                ) &&
                (
                    this.Crew == other.Crew ||
                    this.Crew != null &&
                    this.Crew.SequenceEqual(other.Crew)
                ) &&
                (
                    this.Id == other.Id ||
                    this.Id.Equals(other.Id)
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
                if (this.Cast != null)
                {
                    foreach (var item in this.Cast)
                    {
                        hash = (hash * 16777619) + item.GetHashCode();
                    }
                }
                if (this.Crew != null)
                {
                    foreach (var item in this.Crew)
                    {
                        hash = (hash * 16777619) + item.GetHashCode();
                    }
                }
                hash = (hash * 16777619) + this.Id.GetHashCode();
                return hash;
            }
        }
        #endregion Equality
    }
}
