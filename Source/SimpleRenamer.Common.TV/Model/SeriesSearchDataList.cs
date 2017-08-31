using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sarjee.SimpleRenamer.Common.TV.Model
{
    /// <summary>
    /// Series Search Data List
    /// </summary>
    [DataContract]
    public class SeriesSearchDataList
    {
        /// <summary>
        /// Gets or sets the search results.
        /// </summary>
        /// <value>
        /// The search results.
        /// </value>
        [DataMember(Name = "data", EmitDefaultValue = false)]
        public List<SeriesSearchData> SearchResults { get; set; }
    }
}
