using System;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    /// <summary>
    /// Change Item Created
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Movie.Model.ChangeItemBase" />
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.Movie.Model.ChangeItemCreated}" />
    public class ChangeItemCreated : ChangeItemBase, IEquatable<ChangeItemCreated>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeItemCreated"/> class.
        /// </summary>
        public ChangeItemCreated()
        {
            Action = ChangeAction.Created;
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
            return this.Equals(obj as ChangeItemCreated);
        }

        /// <summary>
        /// Returns true if <see cref="ChangeItemCreated"/> instances are equal
        /// </summary>
        /// <param name="other">Instance of <see cref="ChangeItemCreated"/> to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ChangeItemCreated other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
            {
                return false;
            }

            return
                (
                    base.Equals(other)
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
                int hash = base.GetHashCode();
                return hash;
            }
        }
        #endregion Equality
    }
}
