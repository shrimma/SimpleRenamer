using Newtonsoft.Json;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    public class SearchCollection
    {
        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }
    }
}
