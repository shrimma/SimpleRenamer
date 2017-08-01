using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sarjee.SimpleRenamer.Common.Model
{
    /// <summary>
    /// ShowNameMapping
    /// </summary>
    [JsonObject("showNameMapping")]
    public class ShowNameMapping
    {
        /// <summary>
        /// Gets the mappings.
        /// </summary>
        /// <value>
        /// The mappings.
        /// </value>
        [JsonProperty("mappings")]
        public List<Mapping> Mappings { get; } = new List<Mapping>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowNameMapping"/> class.
        /// </summary>
        public ShowNameMapping()
        {

        }
    }
}
