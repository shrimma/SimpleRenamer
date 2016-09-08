using System.Collections.Generic;
using System.Xml.Serialization;

namespace SimpleRenamer.Framework.DataModel
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
