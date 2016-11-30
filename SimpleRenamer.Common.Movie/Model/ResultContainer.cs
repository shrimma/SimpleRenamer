using Newtonsoft.Json;
using System.Collections.Generic;

namespace SimpleRenamer.Common.Movie.Model
{
    public class ResultContainer<T>
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("results")]
        public List<T> Results { get; set; }
    }
}
