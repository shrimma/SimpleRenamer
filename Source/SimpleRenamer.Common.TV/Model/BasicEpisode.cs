using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace Sarjee.SimpleRenamer.Common.TV.Model
{
    /// <summary>
    /// BasicEpisode
    /// </summary>
    [DataContract]
    public partial class BasicEpisode : IEquatable<BasicEpisode>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicEpisode" /> class.
        /// </summary>
        /// <param name="AbsoluteNumber">AbsoluteNumber.</param>
        /// <param name="AiredEpisodeNumber">AiredEpisodeNumber.</param>
        /// <param name="AiredSeason">AiredSeason.</param>
        /// <param name="DvdEpisodeNumber">DvdEpisodeNumber.</param>
        /// <param name="DvdSeason">DvdSeason.</param>
        /// <param name="EpisodeName">EpisodeName.</param>
        /// <param name="Id">Id.</param>
        /// <param name="Overview">Overview.</param>
        public BasicEpisode(int? AbsoluteNumber = null, int? AiredEpisodeNumber = null, int? AiredSeason = null, int? DvdEpisodeNumber = null, int? DvdSeason = null, string EpisodeName = null, int? Id = null, string Overview = null)
        {
            this.AbsoluteNumber = AbsoluteNumber;
            this.AiredEpisodeNumber = AiredEpisodeNumber;
            this.AiredSeason = AiredSeason;
            this.DvdEpisodeNumber = DvdEpisodeNumber;
            this.DvdSeason = DvdSeason;
            this.EpisodeName = EpisodeName;
            this.Id = Id;
            this.Overview = Overview;
        }

        /// <summary>
        /// Gets or Sets AbsoluteNumber
        /// </summary>
        [DataMember(Name = "absoluteNumber", EmitDefaultValue = false)]
        public int? AbsoluteNumber { get; set; }
        /// <summary>
        /// Gets or Sets AiredEpisodeNumber
        /// </summary>
        [DataMember(Name = "airedEpisodeNumber", EmitDefaultValue = false)]
        public int? AiredEpisodeNumber { get; set; }
        /// <summary>
        /// Gets or Sets AiredSeason
        /// </summary>
        [DataMember(Name = "airedSeason", EmitDefaultValue = false)]
        public int? AiredSeason { get; set; }
        /// <summary>
        /// Gets or Sets DvdEpisodeNumber
        /// </summary>
        [DataMember(Name = "dvdEpisodeNumber", EmitDefaultValue = false)]
        public int? DvdEpisodeNumber { get; set; }
        /// <summary>
        /// Gets or Sets DvdSeason
        /// </summary>
        [DataMember(Name = "dvdSeason", EmitDefaultValue = false)]
        public int? DvdSeason { get; set; }
        /// <summary>
        /// Gets or Sets EpisodeName
        /// </summary>
        [DataMember(Name = "episodeName", EmitDefaultValue = false)]
        public string EpisodeName { get; set; }
        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public int? Id { get; set; }
        /// <summary>
        /// Gets or Sets Overview
        /// </summary>
        [DataMember(Name = "overview", EmitDefaultValue = false)]
        public string Overview { get; set; }
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class BasicEpisode {\n");
            sb.Append("  AbsoluteNumber: ").Append(AbsoluteNumber).Append("\n");
            sb.Append("  AiredEpisodeNumber: ").Append(AiredEpisodeNumber).Append("\n");
            sb.Append("  AiredSeason: ").Append(AiredSeason).Append("\n");
            sb.Append("  DvdEpisodeNumber: ").Append(DvdEpisodeNumber).Append("\n");
            sb.Append("  DvdSeason: ").Append(DvdSeason).Append("\n");
            sb.Append("  EpisodeName: ").Append(EpisodeName).Append("\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  Overview: ").Append(Overview).Append("\n");
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
            return this.Equals(obj as BasicEpisode);
        }

        /// <summary>
        /// Returns true if BasicEpisode instances are equal
        /// </summary>
        /// <param name="other">Instance of BasicEpisode to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(BasicEpisode other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    this.AbsoluteNumber == other.AbsoluteNumber ||
                    this.AbsoluteNumber != null &&
                    this.AbsoluteNumber.Equals(other.AbsoluteNumber)
                ) &&
                (
                    this.AiredEpisodeNumber == other.AiredEpisodeNumber ||
                    this.AiredEpisodeNumber != null &&
                    this.AiredEpisodeNumber.Equals(other.AiredEpisodeNumber)
                ) &&
                (
                    this.AiredSeason == other.AiredSeason ||
                    this.AiredSeason != null &&
                    this.AiredSeason.Equals(other.AiredSeason)
                ) &&
                (
                    this.DvdEpisodeNumber == other.DvdEpisodeNumber ||
                    this.DvdEpisodeNumber != null &&
                    this.DvdEpisodeNumber.Equals(other.DvdEpisodeNumber)
                ) &&
                (
                    this.DvdSeason == other.DvdSeason ||
                    this.DvdSeason != null &&
                    this.DvdSeason.Equals(other.DvdSeason)
                ) &&
                (
                    this.EpisodeName == other.EpisodeName ||
                    this.EpisodeName != null &&
                    this.EpisodeName.Equals(other.EpisodeName)
                ) &&
                (
                    this.Id == other.Id ||
                    this.Id != null &&
                    this.Id.Equals(other.Id)
                ) &&
                (
                    this.Overview == other.Overview ||
                    this.Overview != null &&
                    this.Overview.Equals(other.Overview)
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
                if (this.AbsoluteNumber != null)
                {
                    hash = (hash * 16777619) + this.AbsoluteNumber.GetHashCode();
                }
                if (this.AiredEpisodeNumber != null)
                {
                    hash = (hash * 16777619) + this.AiredEpisodeNumber.GetHashCode();
                }
                if (this.AiredSeason != null)
                {
                    hash = (hash * 16777619) + this.AiredSeason.GetHashCode();
                }
                if (this.DvdEpisodeNumber != null)
                {
                    hash = (hash * 16777619) + this.DvdEpisodeNumber.GetHashCode();
                }
                if (this.DvdSeason != null)
                {
                    hash = (hash * 16777619) + this.DvdSeason.GetHashCode();
                }
                if (this.EpisodeName != null)
                {
                    hash = (hash * 16777619) + this.EpisodeName.GetHashCode();
                }
                if (this.Id != null)
                {
                    hash = (hash * 16777619) + this.Id.GetHashCode();
                }
                if (this.Overview != null)
                {
                    hash = (hash * 16777619) + this.Overview.GetHashCode();
                }
                return hash;
            }
        }
        #endregion Equality
    }
}
