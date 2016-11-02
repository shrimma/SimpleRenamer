using Newtonsoft.Json;
using System.Collections.Generic;

namespace SimpleRenamer.Framework.TmdbModel
{
    public class ResultContainer<T>
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("results")]
        public List<T> Results { get; set; }
    }
}
