using System;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Production Country
    /// </summary>
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.ProductionCountry}" />
    public class ProductionCountry : IEquatable<ProductionCountry>
    {
        /// <summary>
        /// A country code, e.g. US
        /// </summary>
        /// <value>
        /// The country code.
        /// </value>
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

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
            return this.Equals(obj as ProductionCountry);
        }

        /// <summary>
        /// Returns true if <see cref="ProductionCountry"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="ProductionCountry"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ProductionCountry other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    this.CountryCode == other.CountryCode ||
                    this.CountryCode != null &&
                    this.CountryCode.Equals(other.CountryCode)
                ) &&
                (
                    this.Name == other.Name ||
                    this.Name != null &&
                    this.Name.Equals(other.Name)
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
                if (this.CountryCode != null)
                {

                    hash = (hash * 16777619) + this.CountryCode.GetHashCode();
                }
                if (this.Name != null)
                {

                    hash = (hash * 16777619) + this.Name.GetHashCode();
                }
                return hash;
            }
        }
        #endregion Equality
    }
}
