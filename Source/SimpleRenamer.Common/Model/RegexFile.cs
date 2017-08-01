using System;
using System.Collections.Generic;
using System.Linq;

namespace Sarjee.SimpleRenamer.Common.Model
{
    /// <summary>
    /// RegexFile
    /// </summary>
    public class RegexFile : IEquatable<RegexFile>
    {
        /// <summary>
        /// Gets or sets the regex expressions.
        /// </summary>
        /// <value>
        /// The regex expressions.
        /// </value>
        public List<RegexExpression> RegexExpressions { get; set; } = new List<RegexExpression>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexFile"/> class.
        /// </summary>
        public RegexFile()
        {

        }

        #region Equality
        /// <summary>
        /// Determines whether two <see cref="RegexFile"/> contain the same values
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(RegexFile other)
        {
            if (other == null)
            {
                return false;
            }

            return
                (
                    RegexExpressions == other.RegexExpressions ||
                    RegexExpressions != null &&
                    RegexExpressions.SequenceEqual(other.RegexExpressions)
                );
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as RegexFile);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)2166136261;
                if (RegexExpressions != null)
                {
                    foreach (var item in RegexExpressions)
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
