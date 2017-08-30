using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sarjee.SimpleRenamer.Common.Model
{
    /// <summary>
    /// ShowNameMapping
    /// </summary>
    [JsonObject("showNameMapping")]
    public class ShowNameMapping : IEquatable<ShowNameMapping>
    {
        /// <summary>
        /// Gets the mappings.
        /// </summary>
        /// <value>
        /// The mappings.
        /// </value>
        [JsonProperty("mappings")]
        public List<Mapping> Mappings { get; internal set; } = new List<Mapping>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowNameMapping"/> class.
        /// </summary>
        public ShowNameMapping()
        {

        }

        #region Equality
        /// <summary>
        /// Determines whether two <see cref="ShowNameMapping"/> contain the same values
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ShowNameMapping other)
        {
            if (other == null)
            {
                return false;
            }

            return
                (
                    Mappings == other.Mappings ||
                    Mappings != null &&
                    Mappings.SequenceEqual(other.Mappings)
                );
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as ShowNameMapping);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)2166136261;
                if (Mappings != null)
                {
                    foreach (var item in Mappings)
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
