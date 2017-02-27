using Newtonsoft.Json;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    public class AlternativeTitle
    {
        /// <summary>
        /// A country code, e.g. US
        /// </summary>
        [JsonProperty("iso_3166_1")]
        public string Iso_3166_1 { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
