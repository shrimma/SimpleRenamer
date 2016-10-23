﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace SimpleRenamer.Framework.TmdbModel
{
    public class SearchContainer<T>
    {
        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("results")]
        public List<T> Results { get; set; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

        [JsonProperty("total_results")]
        public int TotalResults { get; set; }
    }
}
