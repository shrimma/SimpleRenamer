using Newtonsoft.Json;
using System;

namespace Sarjee.SimpleRenamer.Common.Model
{
    /// <summary>
    /// Mapping
    /// </summary>
    [JsonObject("mapping")]
    public class Mapping : IEquatable<Mapping>
    {
        /// <summary>
        /// Gets or sets the name of the file show.
        /// </summary>
        /// <value>
        /// The name of the file show.
        /// </value>
        [JsonProperty("fileShowName")]
        public string FileShowName { get; set; }

        /// <summary>
        /// Gets or sets the name of the TVDB show.
        /// </summary>
        /// <value>
        /// The name of the TVDB show.
        /// </value>
        [JsonProperty("tvdbShowName")]
        public string TVDBShowName { get; set; }

        /// <summary>
        /// Gets or sets the TVDB show identifier.
        /// </summary>
        /// <value>
        /// The TVDB show identifier.
        /// </value>
        [JsonProperty("tvdbShowId")]
        public string TVDBShowID { get; set; }

        /// <summary>
        /// Gets or sets the name of the custom folder.
        /// </summary>
        /// <value>
        /// The name of the custom folder.
        /// </value>
        [JsonProperty("customFolderName")]
        public string CustomFolderName { get; set; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mapping"/> class.
        /// </summary>
        /// <param name="fileShowName">Name of the file show.</param>
        /// <param name="tvdbShowName">Name of the TVDB show.</param>
        /// <param name="tvdbShowID">The TVDB show identifier.</param>
        public Mapping(string fileShowName, string tvdbShowName, string tvdbShowID)
        {
            if (string.IsNullOrWhiteSpace(fileShowName))
            {
                throw new ArgumentNullException(nameof(fileShowName));
            }
            if (string.IsNullOrWhiteSpace(tvdbShowName))
            {
                throw new ArgumentNullException(nameof(tvdbShowName));
            }
            if (string.IsNullOrWhiteSpace(tvdbShowID))
            {
                throw new ArgumentNullException(nameof(tvdbShowID));
            }
            FileShowName = fileShowName;
            TVDBShowName = tvdbShowName;
            TVDBShowID = tvdbShowID;
        }

        #region Equality
        /// <summary>
        /// Determines whether two <see cref="Mapping"/> contain the same values
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Mapping other)
        {
            if (other == null)
            {
                return false;
            }

            return
                string.Equals(FileShowName, other.FileShowName) &&
                string.Equals(TVDBShowName, other.TVDBShowName) &&
                string.Equals(TVDBShowID, other.TVDBShowID) &&
                string.Equals(CustomFolderName, other.CustomFolderName);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Mapping);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)2166136261;
                if (!string.IsNullOrWhiteSpace(FileShowName))
                {
                    hashCode = (hashCode * 16777619) + FileShowName.GetHashCode();
                }
                if (!string.IsNullOrWhiteSpace(TVDBShowName))
                {
                    hashCode = (hashCode * 16777619) + TVDBShowName.GetHashCode();
                }
                if (!string.IsNullOrWhiteSpace(TVDBShowID))
                {
                    hashCode = (hashCode * 16777619) + TVDBShowID.GetHashCode();
                }
                if (!string.IsNullOrWhiteSpace(CustomFolderName))
                {
                    hashCode = (hashCode * 16777619) + CustomFolderName.GetHashCode();
                }

                return hashCode;
            }
        }
        #endregion Equality
    }
}
