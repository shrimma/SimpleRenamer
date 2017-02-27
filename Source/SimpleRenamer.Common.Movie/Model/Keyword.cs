using Newtonsoft.Json;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    public class Keyword
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
