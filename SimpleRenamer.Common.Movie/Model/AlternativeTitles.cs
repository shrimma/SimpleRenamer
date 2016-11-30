using Newtonsoft.Json;
using System.Collections.Generic;

namespace SimpleRenamer.Common.Movie.Model
{
    public class AlternativeTitles
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("titles")]
        public List<AlternativeTitle> Titles { get; set; }
    }
}
