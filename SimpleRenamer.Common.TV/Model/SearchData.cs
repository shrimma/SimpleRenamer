using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SimpleRenamer.Common.TV.Model
{
    [DataContract]
    public class SearchData
    {
        [DataMember(Name = "data", EmitDefaultValue = false)]
        public List<SeriesSearchData> Series { get; set; }
    }
}
