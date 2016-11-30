using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace SimpleRenamer.Common.TV.Model
{
    /// <summary>
    /// SeriesActorsData
    /// </summary>
    [DataContract]
    public partial class SeriesActorsData : IEquatable<SeriesActorsData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesActorsData" /> class.
        /// </summary>
        /// <param name="Id">Id.</param>
        /// <param name="SeriesId">SeriesId.</param>
        /// <param name="Name">Name.</param>
        /// <param name="Role">Role.</param>
        /// <param name="SortOrder">SortOrder.</param>
        /// <param name="Image">Image.</param>
        /// <param name="ImageAuthor">ImageAuthor.</param>
        /// <param name="ImageAdded">ImageAdded.</param>
        /// <param name="LastUpdated">LastUpdated.</param>
        public SeriesActorsData(int? Id = null, int? SeriesId = null, string Name = null, string Role = null, int? SortOrder = null, string Image = null, int? ImageAuthor = null, string ImageAdded = null, string LastUpdated = null)
        {
            this.Id = Id;
            this.SeriesId = SeriesId;
            this.Name = Name;
            this.Role = Role;
            this.SortOrder = SortOrder;
            this.Image = Image;
            this.ImageAuthor = ImageAuthor;
            this.ImageAdded = ImageAdded;
            this.LastUpdated = LastUpdated;
        }

        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public int? Id { get; set; }
        /// <summary>
        /// Gets or Sets SeriesId
        /// </summary>
        [DataMember(Name = "seriesId", EmitDefaultValue = false)]
        public int? SeriesId { get; set; }
        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string Name { get; set; }
        /// <summary>
        /// Gets or Sets Role
        /// </summary>
        [DataMember(Name = "role", EmitDefaultValue = false)]
        public string Role { get; set; }
        /// <summary>
        /// Gets or Sets SortOrder
        /// </summary>
        [DataMember(Name = "sortOrder", EmitDefaultValue = false)]
        public int? SortOrder { get; set; }
        /// <summary>
        /// Gets or Sets Image
        /// </summary>
        [DataMember(Name = "image", EmitDefaultValue = false)]
        public string Image { get; set; }
        /// <summary>
        /// Gets or Sets ImageAuthor
        /// </summary>
        [DataMember(Name = "imageAuthor", EmitDefaultValue = false)]
        public int? ImageAuthor { get; set; }
        /// <summary>
        /// Gets or Sets ImageAdded
        /// </summary>
        [DataMember(Name = "imageAdded", EmitDefaultValue = false)]
        public string ImageAdded { get; set; }
        /// <summary>
        /// Gets or Sets LastUpdated
        /// </summary>
        [DataMember(Name = "lastUpdated", EmitDefaultValue = false)]
        public string LastUpdated { get; set; }
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SeriesActorsData {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  SeriesId: ").Append(SeriesId).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  Role: ").Append(Role).Append("\n");
            sb.Append("  SortOrder: ").Append(SortOrder).Append("\n");
            sb.Append("  Image: ").Append(Image).Append("\n");
            sb.Append("  ImageAuthor: ").Append(ImageAuthor).Append("\n");
            sb.Append("  ImageAdded: ").Append(ImageAdded).Append("\n");
            sb.Append("  LastUpdated: ").Append(LastUpdated).Append("\n");
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
            return this.Equals(obj as SeriesActorsData);
        }

        /// <summary>
        /// Returns true if SeriesActorsData instances are equal
        /// </summary>
        /// <param name="other">Instance of SeriesActorsData to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SeriesActorsData other)
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
                    this.SeriesId == other.SeriesId ||
                    this.SeriesId != null &&
                    this.SeriesId.Equals(other.SeriesId)
                ) &&
                (
                    this.Name == other.Name ||
                    this.Name != null &&
                    this.Name.Equals(other.Name)
                ) &&
                (
                    this.Role == other.Role ||
                    this.Role != null &&
                    this.Role.Equals(other.Role)
                ) &&
                (
                    this.SortOrder == other.SortOrder ||
                    this.SortOrder != null &&
                    this.SortOrder.Equals(other.SortOrder)
                ) &&
                (
                    this.Image == other.Image ||
                    this.Image != null &&
                    this.Image.Equals(other.Image)
                ) &&
                (
                    this.ImageAuthor == other.ImageAuthor ||
                    this.ImageAuthor != null &&
                    this.ImageAuthor.Equals(other.ImageAuthor)
                ) &&
                (
                    this.ImageAdded == other.ImageAdded ||
                    this.ImageAdded != null &&
                    this.ImageAdded.Equals(other.ImageAdded)
                ) &&
                (
                    this.LastUpdated == other.LastUpdated ||
                    this.LastUpdated != null &&
                    this.LastUpdated.Equals(other.LastUpdated)
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
                if (this.SeriesId != null)
                    hash = hash * 59 + this.SeriesId.GetHashCode();
                if (this.Name != null)
                    hash = hash * 59 + this.Name.GetHashCode();
                if (this.Role != null)
                    hash = hash * 59 + this.Role.GetHashCode();
                if (this.SortOrder != null)
                    hash = hash * 59 + this.SortOrder.GetHashCode();
                if (this.Image != null)
                    hash = hash * 59 + this.Image.GetHashCode();
                if (this.ImageAuthor != null)
                    hash = hash * 59 + this.ImageAuthor.GetHashCode();
                if (this.ImageAdded != null)
                    hash = hash * 59 + this.ImageAdded.GetHashCode();
                if (this.LastUpdated != null)
                    hash = hash * 59 + this.LastUpdated.GetHashCode();
                return hash;
            }
        }
    }
}
