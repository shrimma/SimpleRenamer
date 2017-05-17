using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sarjee.SimpleRenamer.Common.Model
{
    /// <summary>
    /// IgnoreList
    /// </summary>
    [JsonObject("ignoreList")]
    public class IgnoreList
    {
        /// <summary>
        /// Gets or sets the ignore files.
        /// </summary>
        /// <value>
        /// The ignore files.
        /// </value>
        [JsonProperty("ignoreFiles")]
        public List<string> IgnoreFiles { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IgnoreList"/> class.
        /// </summary>
        public IgnoreList()
        {
            IgnoreFiles = new List<string>();
        }
    }
}
