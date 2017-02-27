using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sarjee.SimpleRenamer.Common.TV.Model
{
    [DataContract]
    public class SearchData
    {
        [DataMember(Name = "data", EmitDefaultValue = false)]
        public List<SeriesSearchData> Series { get; set; }
    }
}
