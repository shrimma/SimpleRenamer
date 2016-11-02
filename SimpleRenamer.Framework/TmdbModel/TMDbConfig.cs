﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace SimpleRenamer.Framework.TmdbModel
{
    public class TMDbConfig
    {
        [JsonProperty("change_keys")]
        public List<string> ChangeKeys { get; set; }

        [JsonProperty("images")]
        public ConfigImageTypes Images { get; set; }
    }
}
