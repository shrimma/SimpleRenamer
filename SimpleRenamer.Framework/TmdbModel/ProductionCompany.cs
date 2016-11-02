using Newtonsoft.Json;

namespace SimpleRenamer.Framework.TmdbModel
{
    public class ProductionCompany
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
