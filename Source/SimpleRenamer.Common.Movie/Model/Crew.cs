using Newtonsoft.Json;
using System;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Crew
    /// </summary>
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.Crew}" />
    public class Crew : IEquatable<Crew>
    {
        /// <summary>
        /// Gets or sets the credit identifier.
        /// </summary>
        /// <value>
        /// The credit identifier.
        /// </value>
        [JsonProperty("credit_id")]
        public string CreditId { get; set; }

        /// <summary>
        /// Gets or sets the department.
        /// </summary>
        /// <value>
        /// The department.
        /// </value>
        [JsonProperty("department")]
        public string Department { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the job.
        /// </summary>
        /// <value>
        /// The job.
        /// </value>
        [JsonProperty("job")]
        public string Job { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonProperty("name")]
        public string Name { get; set; }

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
            return this.Equals(obj as Crew);
        }

        /// <summary>
        /// Returns true if <see cref="Crew"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="Crew"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Crew other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    this.CreditId == other.CreditId ||
                    this.CreditId != null &&
                    this.CreditId.Equals(other.CreditId)
                ) &&
                (
                    this.Department == other.Department ||
                    this.Department != null &&
                    this.Department.Equals(other.Department)
                ) &&
                (
                    this.Id == other.Id ||
                    this.Id.Equals(other.Id)
                ) &&
                (
                    this.Job == other.Job ||
                    this.Job != null &&
                    this.Job.Equals(other.Job)
                ) &&
                (
                    this.Name == other.Name ||
                    this.Name != null &&
                    this.Name.Equals(other.Name)
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
                if (this.CreditId != null)
                {
                    hash = (hash * 16777619) + this.CreditId.GetHashCode();
                }
                if (this.Department != null)
                {
                    hash = (hash * 16777619) + this.Department.GetHashCode();
                }
                hash = (hash * 16777619) + this.Id.GetHashCode();
                if (this.Job != null)
                {
                    hash = (hash * 16777619) + this.Job.GetHashCode();
                }
                if (this.Name != null)
                {
                    hash = (hash * 16777619) + this.Name.GetHashCode();
                }
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
