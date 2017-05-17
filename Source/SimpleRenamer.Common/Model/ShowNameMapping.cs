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
        /// The mappings
        /// </summary>
        private List<Mapping> mappings = new List<Mapping>();
        /// <summary>
        /// Gets the mappings.
        /// </summary>
        /// <value>
        /// The mappings.
        /// </value>
        [JsonProperty("mappings")]
        public List<Mapping> Mappings { get { return mappings; } }
    }
}
