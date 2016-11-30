using Newtonsoft.Json;

namespace SimpleRenamer.Common.Movie.Model
{
    public class Translation
    {
        [JsonProperty("english_name")]
        public string EnglishName { get; set; }

        /// <summary>
        /// A language code, e.g. en
        /// </summary>
        [JsonProperty("iso_639_1")]
        public string Iso_639_1 { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
