using System.Xml.Serialization;

namespace Sarjee.SimpleRenamer.Common.Model
{
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
