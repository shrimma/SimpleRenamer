using Newtonsoft.Json;
using System;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Cast
    /// </summary>
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.Cast}" />
    public class Cast : IEquatable<Cast>
    {
        /// <summary>
        /// Gets or sets the cast identifier.
        /// </summary>
        /// <value>
        /// The cast identifier.
        /// </value>
        [JsonProperty("cast_id")]
        public int CastId { get; set; }

        /// <summary>
        /// Gets or sets the character.
        /// </summary>
        /// <value>
        /// The character.
        /// </value>
        [JsonProperty("character")]
        public string Character { get; set; }

        /// <summary>
        /// Gets or sets the credit identifier.
        /// </summary>
        /// <value>
        /// The credit identifier.
        /// </value>
        [JsonProperty("credit_id")]
        public string CreditId { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        [JsonProperty("order")]
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the profile path.
        /// </summary>
        /// <value>
        /// The profile path.
        /// </value>
        [JsonProperty("profile_path")]
        public string ProfilePath { get; set; }

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
            return this.Equals(obj as Cast);
        }

        /// <summary>
        /// Returns true if <see cref="Cast"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="Cast"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Cast other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    this.CastId == other.CastId ||
                    this.CastId.Equals(other.CastId)
                ) &&
                (
                    this.Character == other.Character ||
                    this.Character != null &&
                    this.Character.Equals(other.Character)
                ) &&
                (
                    this.CreditId == other.CreditId ||
                    this.CreditId != null &&
                    this.CreditId.Equals(other.CreditId)
                ) &&
                (
                    this.Id == other.Id ||
                    this.Id.Equals(other.Id)
                ) &&
                (
                    this.Name == other.Name ||
                    this.Name != null &&
                    this.Name.Equals(other.Name)
                ) &&
                (
                    this.Order == other.Order ||
                    this.Order.Equals(other.Order)
                ) &&
                (
                    this.ProfilePath == other.ProfilePath ||
                    this.ProfilePath != null &&
                    this.ProfilePath.Equals(other.ProfilePath)
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
                hash = (hash * 16777619) + this.CastId.GetHashCode();
                if (this.Character != null)
                {
                    hash = (hash * 16777619) + this.Character.GetHashCode();
                }
                if (this.CreditId != null)
                {
                    hash = (hash * 16777619) + this.CreditId.GetHashCode();
                }
                hash = (hash * 16777619) + this.Id.GetHashCode();
                if (this.Name != null)
                {
                    hash = (hash * 16777619) + this.Name.GetHashCode();
                }
                hash = (hash * 16777619) + this.Order.GetHashCode();
                if (this.ProfilePath != null)
                {
                    hash = (hash * 16777619) + this.ProfilePath.GetHashCode();
                }
                return hash;
            }
        }
        #endregion Equality
    }
}
