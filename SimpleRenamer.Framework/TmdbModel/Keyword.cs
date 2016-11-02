using Newtonsoft.Json;

namespace SimpleRenamer.Framework.TmdbModel
{
    public class Keyword
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
