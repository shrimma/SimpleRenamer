using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SimpleRenamer.Framework.TvdbModel
{
    [DataContract(Name = "data")]
    public class SearchData
    {
        public List<SeriesSearchData> Series { get; set; }
    }
}
