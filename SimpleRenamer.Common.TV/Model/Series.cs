using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SimpleRenamer.Common.TV.Model
{
    /// <summary>
    /// Series
    /// </summary>
    [DataContract]
    public partial class Series : IEquatable<Series>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Series" /> class.
        /// </summary>
        /// <param name="Id">Id.</param>
        /// <param name="SeriesName">SeriesName.</param>
        /// <param name="Aliases">Aliases.</param>
        /// <param name="Banner">Banner.</param>
        /// <param name="SeriesId">SeriesId.</param>
        /// <param name="Status">Status.</param>
        /// <param name="FirstAired">FirstAired.</param>
        /// <param name="Network">Network.</param>
        /// <param name="NetworkId">NetworkId.</param>
        /// <param name="Runtime">Runtime.</param>
        /// <param name="Genre">Genre.</param>
        /// <param name="Overview">Overview.</param>
        /// <param name="LastUpdated">LastUpdated.</param>
        /// <param name="AirsDayOfWeek">AirsDayOfWeek.</param>
        /// <param name="AirsTime">AirsTime.</param>
        /// <param name="Rating">Rating.</param>
        /// <param name="ImdbId">ImdbId.</param>
        /// <param name="Zap2itId">Zap2itId.</param>
        /// <param name="Added">Added.</param>
        /// <param name="SiteRating">SiteRating.</param>
        /// <param name="SiteRatingCount">SiteRatingCount.</param>
        public Series(int? Id = null, string SeriesName = null, List<string> Aliases = null, string Banner = null, string SeriesId = null, string Status = null, string FirstAired = null, string Network = null, string NetworkId = null, string Runtime = null, List<string> Genre = null, string Overview = null, int? LastUpdated = null, string AirsDayOfWeek = null, string AirsTime = null, string Rating = null, string ImdbId = null, string Zap2itId = null, string Added = null, decimal? SiteRating = null, int? SiteRatingCount = null)
        {
            this.Id = Id;
            this.SeriesName = SeriesName;
            this.Aliases = Aliases;
            this.Banner = Banner;
            this.SeriesId = SeriesId;
            this.Status = Status;
            this.FirstAired = FirstAired;
            this.Network = Network;
            this.NetworkId = NetworkId;
            this.Runtime = Runtime;
            this.Genre = Genre;
            this.Overview = Overview;
            this.LastUpdated = LastUpdated;
            this.AirsDayOfWeek = AirsDayOfWeek;
            this.AirsTime = AirsTime;
            this.Rating = Rating;
            this.ImdbId = ImdbId;
            this.Zap2itId = Zap2itId;
            this.Added = Added;
            this.SiteRating = SiteRating;
            this.SiteRatingCount = SiteRatingCount;
        }

        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public int? Id { get; set; }
        /// <summary>
        /// Gets or Sets SeriesName
        /// </summary>
        [DataMember(Name = "seriesName", EmitDefaultValue = false)]
        public string SeriesName { get; set; }
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
        /// Gets or Sets SeriesId
        /// </summary>
        [DataMember(Name = "seriesId", EmitDefaultValue = false)]
        public string SeriesId { get; set; }
        /// <summary>
        /// Gets or Sets Status
        /// </summary>
        [DataMember(Name = "status", EmitDefaultValue = false)]
        public string Status { get; set; }
        /// <summary>
        /// Gets or Sets FirstAired
        /// </summary>
        [DataMember(Name = "firstAired", EmitDefaultValue = false)]
        public string FirstAired { get; set; }
        /// <summary>
        /// Gets or Sets Network
        /// </summary>
        [DataMember(Name = "network", EmitDefaultValue = false)]
        public string Network { get; set; }
        /// <summary>
        /// Gets or Sets NetworkId
        /// </summary>
        [DataMember(Name = "networkId", EmitDefaultValue = false)]
        public string NetworkId { get; set; }
        /// <summary>
        /// Gets or Sets Runtime
        /// </summary>
        [DataMember(Name = "runtime", EmitDefaultValue = false)]
        public string Runtime { get; set; }
        /// <summary>
        /// Gets or Sets Genre
        /// </summary>
        [DataMember(Name = "genre", EmitDefaultValue = false)]
        public List<string> Genre { get; set; }
        /// <summary>
        /// Gets or Sets Overview
        /// </summary>
        [DataMember(Name = "overview", EmitDefaultValue = false)]
        public string Overview { get; set; }
        /// <summary>
        /// Gets or Sets LastUpdated
        /// </summary>
        [DataMember(Name = "lastUpdated", EmitDefaultValue = false)]
        public int? LastUpdated { get; set; }
        /// <summary>
        /// Gets or Sets AirsDayOfWeek
        /// </summary>
        [DataMember(Name = "airsDayOfWeek", EmitDefaultValue = false)]
        public string AirsDayOfWeek { get; set; }
        /// <summary>
        /// Gets or Sets AirsTime
        /// </summary>
        [DataMember(Name = "airsTime", EmitDefaultValue = false)]
        public string AirsTime { get; set; }
        /// <summary>
        /// Gets or Sets Rating
        /// </summary>
        [DataMember(Name = "rating", EmitDefaultValue = false)]
        public string Rating { get; set; }
        /// <summary>
        /// Gets or Sets ImdbId
        /// </summary>
        [DataMember(Name = "imdbId", EmitDefaultValue = false)]
        public string ImdbId { get; set; }
        /// <summary>
        /// Gets or Sets Zap2itId
        /// </summary>
        [DataMember(Name = "zap2itId", EmitDefaultValue = false)]
        public string Zap2itId { get; set; }
        /// <summary>
        /// Gets or Sets Added
        /// </summary>
        [DataMember(Name = "added", EmitDefaultValue = false)]
        public string Added { get; set; }
        /// <summary>
        /// Gets or Sets SiteRating
        /// </summary>
        [DataMember(Name = "siteRating", EmitDefaultValue = false)]
        public decimal? SiteRating { get; set; }
        /// <summary>
        /// Gets or Sets SiteRatingCount
        /// </summary>
        [DataMember(Name = "siteRatingCount", EmitDefaultValue = false)]
        public int? SiteRatingCount { get; set; }
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Series {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  SeriesName: ").Append(SeriesName).Append("\n");
            sb.Append("  Aliases: ").Append(Aliases).Append("\n");
            sb.Append("  Banner: ").Append(Banner).Append("\n");
            sb.Append("  SeriesId: ").Append(SeriesId).Append("\n");
            sb.Append("  Status: ").Append(Status).Append("\n");
            sb.Append("  FirstAired: ").Append(FirstAired).Append("\n");
            sb.Append("  Network: ").Append(Network).Append("\n");
            sb.Append("  NetworkId: ").Append(NetworkId).Append("\n");
            sb.Append("  Runtime: ").Append(Runtime).Append("\n");
            sb.Append("  Genre: ").Append(Genre).Append("\n");
            sb.Append("  Overview: ").Append(Overview).Append("\n");
            sb.Append("  LastUpdated: ").Append(LastUpdated).Append("\n");
            sb.Append("  AirsDayOfWeek: ").Append(AirsDayOfWeek).Append("\n");
            sb.Append("  AirsTime: ").Append(AirsTime).Append("\n");
            sb.Append("  Rating: ").Append(Rating).Append("\n");
            sb.Append("  ImdbId: ").Append(ImdbId).Append("\n");
            sb.Append("  Zap2itId: ").Append(Zap2itId).Append("\n");
            sb.Append("  Added: ").Append(Added).Append("\n");
            sb.Append("  SiteRating: ").Append(SiteRating).Append("\n");
            sb.Append("  SiteRatingCount: ").Append(SiteRatingCount).Append("\n");
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
            return this.Equals(obj as Series);
        }

        /// <summary>
        /// Returns true if Series instances are equal
        /// </summary>
        /// <param name="other">Instance of Series to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Series other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return
                (
                    this.Id == other.Id ||
                    this.Id != null &&
                    this.Id.Equals(other.Id)
                ) &&
                (
                    this.SeriesName == other.SeriesName ||
                    this.SeriesName != null &&
                    this.SeriesName.Equals(other.SeriesName)
                ) &&
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
                    this.SeriesId == other.SeriesId ||
                    this.SeriesId != null &&
                    this.SeriesId.Equals(other.SeriesId)
                ) &&
                (
                    this.Status == other.Status ||
                    this.Status != null &&
                    this.Status.Equals(other.Status)
                ) &&
                (
                    this.FirstAired == other.FirstAired ||
                    this.FirstAired != null &&
                    this.FirstAired.Equals(other.FirstAired)
                ) &&
                (
                    this.Network == other.Network ||
                    this.Network != null &&
                    this.Network.Equals(other.Network)
                ) &&
                (
                    this.NetworkId == other.NetworkId ||
                    this.NetworkId != null &&
                    this.NetworkId.Equals(other.NetworkId)
                ) &&
                (
                    this.Runtime == other.Runtime ||
                    this.Runtime != null &&
                    this.Runtime.Equals(other.Runtime)
                ) &&
                (
                    this.Genre == other.Genre ||
                    this.Genre != null &&
                    this.Genre.SequenceEqual(other.Genre)
                ) &&
                (
                    this.Overview == other.Overview ||
                    this.Overview != null &&
                    this.Overview.Equals(other.Overview)
                ) &&
                (
                    this.LastUpdated == other.LastUpdated ||
                    this.LastUpdated != null &&
                    this.LastUpdated.Equals(other.LastUpdated)
                ) &&
                (
                    this.AirsDayOfWeek == other.AirsDayOfWeek ||
                    this.AirsDayOfWeek != null &&
                    this.AirsDayOfWeek.Equals(other.AirsDayOfWeek)
                ) &&
                (
                    this.AirsTime == other.AirsTime ||
                    this.AirsTime != null &&
                    this.AirsTime.Equals(other.AirsTime)
                ) &&
                (
                    this.Rating == other.Rating ||
                    this.Rating != null &&
                    this.Rating.Equals(other.Rating)
                ) &&
                (
                    this.ImdbId == other.ImdbId ||
                    this.ImdbId != null &&
                    this.ImdbId.Equals(other.ImdbId)
                ) &&
                (
                    this.Zap2itId == other.Zap2itId ||
                    this.Zap2itId != null &&
                    this.Zap2itId.Equals(other.Zap2itId)
                ) &&
                (
                    this.Added == other.Added ||
                    this.Added != null &&
                    this.Added.Equals(other.Added)
                ) &&
                (
                    this.SiteRating == other.SiteRating ||
                    this.SiteRating != null &&
                    this.SiteRating.Equals(other.SiteRating)
                ) &&
                (
                    this.SiteRatingCount == other.SiteRatingCount ||
                    this.SiteRatingCount != null &&
                    this.SiteRatingCount.Equals(other.SiteRatingCount)
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
                if (this.Id != null)
                    hash = hash * 59 + this.Id.GetHashCode();
                if (this.SeriesName != null)
                    hash = hash * 59 + this.SeriesName.GetHashCode();
                if (this.Aliases != null)
                    hash = hash * 59 + this.Aliases.GetHashCode();
                if (this.Banner != null)
                    hash = hash * 59 + this.Banner.GetHashCode();
                if (this.SeriesId != null)
                    hash = hash * 59 + this.SeriesId.GetHashCode();
                if (this.Status != null)
                    hash = hash * 59 + this.Status.GetHashCode();
                if (this.FirstAired != null)
                    hash = hash * 59 + this.FirstAired.GetHashCode();
                if (this.Network != null)
                    hash = hash * 59 + this.Network.GetHashCode();
                if (this.NetworkId != null)
                    hash = hash * 59 + this.NetworkId.GetHashCode();
                if (this.Runtime != null)
                    hash = hash * 59 + this.Runtime.GetHashCode();
                if (this.Genre != null)
                    hash = hash * 59 + this.Genre.GetHashCode();
                if (this.Overview != null)
                    hash = hash * 59 + this.Overview.GetHashCode();
                if (this.LastUpdated != null)
                    hash = hash * 59 + this.LastUpdated.GetHashCode();
                if (this.AirsDayOfWeek != null)
                    hash = hash * 59 + this.AirsDayOfWeek.GetHashCode();
                if (this.AirsTime != null)
                    hash = hash * 59 + this.AirsTime.GetHashCode();
                if (this.Rating != null)
                    hash = hash * 59 + this.Rating.GetHashCode();
                if (this.ImdbId != null)
                    hash = hash * 59 + this.ImdbId.GetHashCode();
                if (this.Zap2itId != null)
                    hash = hash * 59 + this.Zap2itId.GetHashCode();
                if (this.Added != null)
                    hash = hash * 59 + this.Added.GetHashCode();
                if (this.SiteRating != null)
                    hash = hash * 59 + this.SiteRating.GetHashCode();
                if (this.SiteRatingCount != null)
                    hash = hash * 59 + this.SiteRatingCount.GetHashCode();
                return hash;
            }
        }
    }
}
