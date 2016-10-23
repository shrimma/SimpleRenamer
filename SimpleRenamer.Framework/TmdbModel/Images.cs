using Newtonsoft.Json;
using System.Collections.Generic;

namespace SimpleRenamer.Framework.TmdbModel
{
    public class Images
    {
        [JsonProperty("backdrops")]
        public List<ImageData> Backdrops { get; set; }

        [JsonProperty("posters")]
        public List<ImageData> Posters { get; set; }
    }
}
