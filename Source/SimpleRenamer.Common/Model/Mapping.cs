using Newtonsoft.Json;
using System;

namespace Sarjee.SimpleRenamer.Common.Model
{
    /// <summary>
    /// Mapping
    /// </summary>
    [JsonObject("mapping")]
    public class Mapping
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
    }
}
