using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sarjee.SimpleRenamer.Common.Model
{
    /// <summary>
    /// IgnoreList
    /// </summary>
    public class IgnoreList
    {
        /// <summary>
        /// Gets or sets the ignore files.
        /// </summary>
        /// <value>
        /// The ignore files.
        /// </value>
        [XmlArrayItem("IgnoreFile")]
        public List<string> IgnoreFiles { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IgnoreList"/> class.
        /// </summary>
        public IgnoreList()
        {
            IgnoreFiles = new List<string>();
        }
    }
}
