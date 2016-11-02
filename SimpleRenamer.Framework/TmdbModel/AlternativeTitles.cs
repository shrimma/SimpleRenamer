using Newtonsoft.Json;
using System.Collections.Generic;

namespace SimpleRenamer.Framework.TmdbModel
{
    public class AlternativeTitles
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("titles")]
        public List<AlternativeTitle> Titles { get; set; }
    }
}
