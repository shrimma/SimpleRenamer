using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SimpleRenamer.Framework.TvdbModel
{
    [DataContract]
    public partial class SeriesSearchData : IEquatable<SeriesSearchData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesSearchData" /> class.
        /// </summary>
        /// <param name="Aliases">Aliases.</param>
        /// <param name="Banner">Banner.</param>
        /// <param name="FirstAired">FirstAired.</param>
        /// <param name="Id">Id.</param>
        /// <param name="Network">Network.</param>
        /// <param name="Overview">Overview.</param>
        /// <param name="SeriesName">SeriesName.</param>
        /// <param name="Status">Status.</param>
        public SeriesSearchData(List<string> Aliases = null, string Banner = null, string FirstAired = null, int? Id = null, string Network = null, string Overview = null, string SeriesName = null, string Status = null)
        {
            this.Aliases = Aliases;
            this.Banner = Banner;
            this.FirstAired = FirstAired;
            this.Id = Id;
            this.Network = Network;
            this.Overview = Overview;
            this.SeriesName = SeriesName;
            this.Status = Status;
        }

        /// <summary>
        /// Gets or Sets Aliases
        /// </summary>
        [DataMember(Name = "aliases", EmitDefaultValue = false)]
        public List<string> Aliases { get; set; }
        /// <summary>
        /// Gets or Sets Banner
        /// </summary>
        [DataMember(Name = "banner", EmitDefaultValue = false)]
        public string Banner { get; set; }
        /// <summary>
        /// Gets or Sets FirstAired
        /// </summary>
        [DataMember(Name = "firstAired", EmitDefaultValue = false)]
        public string FirstAired { get; set; }
        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public int? Id { get; set; }
        /// <summary>
        /// Gets or Sets Network
        /// </summary>
        [DataMember(Name = "network", EmitDefaultValue = false)]
        public string Network { get; set; }
        /// <summary>
        /// Gets or Sets Overview
        /// </summary>
        [DataMember(Name = "overview", EmitDefaultValue = false)]
        public string Overview { get; set; }
        /// <summary>
        /// Gets or Sets SeriesName
        /// </summary>
        [DataMember(Name = "seriesName", EmitDefaultValue = false)]
        public string SeriesName { get; set; }
        /// <summary>
        /// Gets or Sets Status
        /// </summary>
        [DataMember(Name = "status", EmitDefaultValue = false)]
        public string Status { get; set; }
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SeriesSearchData {\n");
            sb.Append("  Aliases: ").Append(Aliases).Append("\n");
            sb.Append("  Banner: ").Append(Banner).Append("\n");
            sb.Append("  FirstAired: ").Append(FirstAired).Append("\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  Network: ").Append(Network).Append("\n");
            sb.Append("  Overview: ").Append(Overview).Append("\n");
            sb.Append("  SeriesName: ").Append(SeriesName).Append("\n");
            sb.Append("  Status: ").Append(Status).Append("\n");
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
            return this.Equals(obj as SeriesSearchData);
        }

        /// <summary>
        /// Returns true if SeriesSearchData instances are equal
        /// </summary>
        /// <param name="other">Instance of SeriesSearchData to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SeriesSearchData other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return
                (
                    this.Aliases == other.Aliases ||
                    this.Aliases != null &&
                    this.Aliases.SequenceEqual(other.Aliases)
                ) &&
                (
                    this.Banner == other.Banner ||
                    this.Banner != null &&
                    this.Banner.Equals(other.Banner)
                ) &&
                (
                    this.FirstAired == other.FirstAired ||
                    this.FirstAired != null &&
                    this.FirstAired.Equals(other.FirstAired)
                ) &&
                (
                    this.Id == other.Id ||
                    this.Id != null &&
                    this.Id.Equals(other.Id)
                ) &&
                (
                    this.Network == other.Network ||
                    this.Network != null &&
                    this.Network.Equals(other.Network)
                ) &&
                (
                    this.Overview == other.Overview ||
                    this.Overview != null &&
                    this.Overview.Equals(other.Overview)
                ) &&
                (
                    this.SeriesName == other.SeriesName ||
                    this.SeriesName != null &&
                    this.SeriesName.Equals(other.SeriesName)
                ) &&
                (
                    this.Status == other.Status ||
                    this.Status != null &&
                    this.Status.Equals(other.Status)
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
                if (this.Aliases != null)
                    hash = hash * 59 + this.Aliases.GetHashCode();
                if (this.Banner != null)
                    hash = hash * 59 + this.Banner.GetHashCode();
                if (this.FirstAired != null)
                    hash = hash * 59 + this.FirstAired.GetHashCode();
                if (this.Id != null)
                    hash = hash * 59 + this.Id.GetHashCode();
                if (this.Network != null)
                    hash = hash * 59 + this.Network.GetHashCode();
                if (this.Overview != null)
                    hash = hash * 59 + this.Overview.GetHashCode();
                if (this.SeriesName != null)
                    hash = hash * 59 + this.SeriesName.GetHashCode();
                if (this.Status != null)
                    hash = hash * 59 + this.Status.GetHashCode();
                return hash;
            }
        }
    }
}
