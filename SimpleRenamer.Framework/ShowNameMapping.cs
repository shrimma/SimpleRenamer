using System.Collections.Generic;
using System.Xml.Serialization;

namespace SimpleRenamer.Framework
{
    public class ShowNameMapping
    {
        private List<Mapping> mappings = new List<Mapping>();
        public List<Mapping> Mappings { get { return mappings; } }
    }

    public class Mapping
    {
        [XmlAttribute]
        public string FileShowName { get; set; }
        [XmlAttribute]
        public string TVDBShowName { get; set; }
        [XmlAttribute]
        public string TVDBShowID { get; set; }
        [XmlAttribute]
        public string CustomFolderName { get; set; }

        public Mapping(string fileShowName, string tvdbShowName, string tvdbShowID)
        {
            FileShowName = fileShowName;
            TVDBShowName = tvdbShowName;
            TVDBShowID = tvdbShowID;
            CustomFolderName = string.Empty;
        }

        public Mapping()
        {
        }
    }
}
