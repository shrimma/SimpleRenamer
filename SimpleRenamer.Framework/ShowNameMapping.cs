using System.Collections.Generic;
using System.Xml.Serialization;

namespace SimpleRenamer.Framework
{
    public class ShowNameMapping
    {
        List<Mapping> Mappings { get; set; }
    }

    public class Mapping
    {
        [XmlAttribute]
        public string FileShowName { get; set; }
        [XmlAttribute]
        public string TVDBShowName { get; set; }
    }
}
