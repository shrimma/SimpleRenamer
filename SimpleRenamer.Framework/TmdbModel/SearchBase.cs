using Newtonsoft.Json;

namespace SimpleRenamer.Framework.TmdbModel
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
