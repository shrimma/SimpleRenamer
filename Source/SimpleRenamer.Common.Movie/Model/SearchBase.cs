using Newtonsoft.Json;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    [JsonConverter(typeof(SearchBaseConverter))]
    public class SearchBase
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonIgnore]
        [JsonProperty("media_type")]
        public MediaType MediaType { get; set; }

        [JsonProperty("popularity")]
        public double Popularity { get; set; }
    }
}
