using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Sarjee.SimpleRenamer.Common.TV.Model
{
    /// <summary>
    /// SearchData
    /// </summary>
    /// <seealso cref="System.IEquatable{Sarjee.SimpleRenamer.Common.TV.Model.SearchData}" />
    [DataContract]
    public class SearchData : IEquatable<SearchData>
    {
        /// <summary>
        /// Gets or sets the series.
        /// </summary>
        /// <value>
        /// The series.
        /// </value>
        [DataMember(Name = "data", EmitDefaultValue = false)]
        public List<SeriesSearchData> Series { get; set; } = new List<SeriesSearchData>();

        #region Equality
        /// <summary>
        /// Determines whether two <see cref="SearchData"/> contain the same values
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(SearchData other)
        {
            if (other == null)
            {
                return false;
            }

            return
                (
                    Series == other.Series ||
                    Series != null &&
                    Series.SequenceEqual(other.Series)
                );
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as SearchData);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)2166136261;
                if (Series != null)
                {
                    foreach (var item in Series)
                    {
                        hashCode = (hashCode * 16777619) + item.GetHashCode();
                    }
                }

                return hashCode;
            }
        }
        #endregion Equality
    }
}
