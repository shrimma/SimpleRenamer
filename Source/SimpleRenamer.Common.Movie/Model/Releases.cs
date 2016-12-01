using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    public class Releases
    {
        [JsonProperty("countries")]
        public List<Country> Countries { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
