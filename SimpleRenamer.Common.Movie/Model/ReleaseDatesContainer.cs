using Newtonsoft.Json;
using System.Collections.Generic;

namespace SimpleRenamer.Common.Movie.Model
{
    public class ReleaseDatesContainer
    {
        /// <summary>
        /// A country code, e.g. US
        /// </summary>
        [JsonProperty("iso_3166_1")]
        public string Iso_3166_1 { get; set; }

        [JsonProperty("release_dates")]
        public List<ReleaseDateItem> ReleaseDates { get; set; }
    }
}
