using System.Xml.Serialization;

namespace Sarjee.SimpleRenamer.Common.Model
{
    /// <summary>
    /// Mapping
    /// </summary>
    public class Mapping
    {
        /// <summary>
        /// Gets or sets the name of the file show.
        /// </summary>
        /// <value>
        /// The name of the file show.
        /// </value>
        [XmlAttribute]
        public string FileShowName { get; set; }
        /// <summary>
        /// Gets or sets the name of the TVDB show.
        /// </summary>
        /// <value>
        /// The name of the TVDB show.
        /// </value>
        [XmlAttribute]
        public string TVDBShowName { get; set; }
        /// <summary>
        /// Gets or sets the TVDB show identifier.
        /// </summary>
        /// <value>
        /// The TVDB show identifier.
        /// </value>
        [XmlAttribute]
        public string TVDBShowID { get; set; }
        /// <summary>
        /// Gets or sets the name of the custom folder.
        /// </summary>
        /// <value>
        /// The name of the custom folder.
        /// </value>
        [XmlAttribute]
        public string CustomFolderName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mapping"/> class.
        /// </summary>
        /// <param name="fileShowName">Name of the file show.</param>
        /// <param name="tvdbShowName">Name of the TVDB show.</param>
        /// <param name="tvdbShowID">The TVDB show identifier.</param>
        public Mapping(string fileShowName, string tvdbShowName, string tvdbShowID)
        {
            FileShowName = fileShowName;
            TVDBShowName = tvdbShowName;
            TVDBShowID = tvdbShowID;
            CustomFolderName = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mapping"/> class.
        /// </summary>
        public Mapping()
        {
        }
    }
}
