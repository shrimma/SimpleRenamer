using Newtonsoft.Json;
using System.Collections.Generic;

namespace SimpleRenamer.Framework.TmdbModel
{
    public class Change
    {
        [JsonProperty("items")]
        public List<ChangeItemBase> Items { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }
    }
}
