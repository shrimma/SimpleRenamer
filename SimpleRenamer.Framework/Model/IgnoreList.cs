using System.Collections.Generic;
using System.Xml.Serialization;

namespace SimpleRenamer.Common.Model
{
    public class IgnoreList
    {
        [XmlArrayItem("IgnoreFile")]
        public List<string> IgnoreFiles { get; set; }

        public IgnoreList()
        {
            IgnoreFiles = new List<string>();
        }
    }
}
