using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    public class ChangesContainer
    {
        [JsonProperty("changes")]
        public List<Change> Changes { get; set; }
    }
}
