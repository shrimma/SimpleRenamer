using Newtonsoft.Json;
using System.Collections.Generic;

namespace SimpleRenamer.Common.Movie.Model
{
    public class Change
    {
        [JsonProperty("items")]
        public List<ChangeItemBase> Items { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }
    }
}
