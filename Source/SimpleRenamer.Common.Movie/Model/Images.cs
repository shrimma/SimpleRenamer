using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    public class Images
    {
        [JsonProperty("backdrops")]
        public List<ImageData> Backdrops { get; set; }

        [JsonProperty("posters")]
        public List<ImageData> Posters { get; set; }
    }
}
