using Newtonsoft.Json;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    public class Genre
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
