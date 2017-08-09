using Newtonsoft.Json;
using System;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Account State
    /// </summary>
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.AccountState}" />
    [JsonConverter(typeof(AccountStateConverter))]
    public class AccountState : IEquatable<AccountState>
    {
        /// <summary>
        /// Represents the current favorite status of the related movie for the current user session.
        /// </summary>
        /// <value>
        ///   <c>true</c> if favorite; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("favorite")]
        public bool Favorite { get; set; }

        /// <summary>
        /// The TMDb id for the related movie
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the rating.
        /// </summary>
        /// <value>
        /// The rating.
        /// </value>
        [JsonProperty("rating")]
        public double? Rating { get; set; }

        /// <summary>
        /// Represents the presence of the related movie on the current user's watchlist.
        /// </summary>
        /// <value>
        ///   <c>true</c> if watchlist; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("watchlist")]
        public bool Watchlist { get; set; }

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
            return this.Equals(obj as AccountState);
        }

        /// <summary>
        /// Returns true if <see cref="AccountState"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="AccountState"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(AccountState other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    this.Favorite == other.Favorite ||
                    this.Favorite.Equals(other.Favorite)
                ) &&
                (
                    this.Id == other.Id ||
                    this.Id.Equals(other.Id)
                ) &&
                (
                    this.Rating == other.Rating ||
                    this.Rating != null &&
                    this.Rating.Equals(other.Rating)
                ) &&
                (
                    this.Watchlist == other.Watchlist ||
                    this.Watchlist.Equals(other.Watchlist)
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
                hash = (hash * 16777619) + this.Favorite.GetHashCode();
                hash = (hash * 16777619) + this.Id.GetHashCode();
                if (this.Rating != null)
                {
                    hash = (hash * 16777619) + this.Rating.GetHashCode();
                }
                hash = (hash * 16777619) + this.Watchlist.GetHashCode();
                return hash;
            }
        }
        #endregion Equality
    }
}
