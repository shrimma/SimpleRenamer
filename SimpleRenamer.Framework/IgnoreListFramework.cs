using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SimpleRenamer.Framework
{
    public class IgnoreListFramework : IIgnoreListFramework
    {
        private static string ignoreFilePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "IgnoreFileList.xml");
        public async Task<IgnoreList> ReadIgnoreListAsync()
        {
            IgnoreList snm = new IgnoreList();
            //if the file doesn't yet exist then set a new version
            if (!File.Exists(ignoreFilePath))
            {
                return snm;
            }
            else
            {
                using (FileStream fs = new FileStream(ignoreFilePath, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(IgnoreList));
                    snm = (IgnoreList)serializer.Deserialize(fs);
                }
                return snm;
            }
        }

        public async Task<bool> WriteIgnoreListAsync(IgnoreList ignoreList)
        {
            //only write the file if there is data
            if (ignoreList != null && ignoreList.IgnoreFiles.Count > 0)
            {
                using (TextWriter writer = new StreamWriter(ignoreFilePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(IgnoreList));
                    serializer.Serialize(writer, ignoreList);
                }
            }

            return true;
        }
    }
}
