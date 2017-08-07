using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sarjee.SimpleRenamer.Common.Model
{
    /// <summary>
    /// IgnoreList
    /// </summary>
    [JsonObject("ignoreList")]
    public class IgnoreList : IEquatable<IgnoreList>
    {
        /// <summary>
        /// Gets or sets the ignore files.
        /// </summary>
        /// <value>
        /// The ignore files.
        /// </value>
        [JsonProperty("ignoreFiles")]
        public List<string> IgnoreFiles { get; } = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="IgnoreList"/> class.
        /// </summary>
        public IgnoreList()
        {

        }

        #region Equality
        /// <summary>
        /// Determines whether two <see cref="IgnoreList"/> contain the same values
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IgnoreList other)
        {
            if (other == null)
            {
                return false;
            }

            return
                (
                    IgnoreFiles == other.IgnoreFiles ||
                    IgnoreFiles != null &&
                    IgnoreFiles.SequenceEqual(other.IgnoreFiles)
                );
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as IgnoreList);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)2166136261;
                if (IgnoreFiles != null)
                {
                    foreach (var item in IgnoreFiles)
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
