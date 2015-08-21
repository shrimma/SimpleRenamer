using System.Collections.Generic;
using System.Xml.Serialization;

namespace SimpleRenamer.Framework
{
    public class ShowNameMapping
    {
        public List<Mapping> Mappings { get; set; }
    }

    public class Mapping
    {
        [XmlAttribute]
        public string FileShowName { get; set; }
        [XmlAttribute]
        public string TVDBShowName { get; set; }
        [XmlAttribute]
        public string TVDBShowID { get; set; }

        public Mapping(string fileShowName, string tvdbShowName, string tvdbShowID)
        {
            FileShowName = fileShowName;
            TVDBShowName = tvdbShowName;
            TVDBShowID = tvdbShowID;
        }

        public Mapping()
        {
        }
    }
}
