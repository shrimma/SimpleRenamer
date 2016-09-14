using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SimpleRenamer.Framework
{
    public class IgnoreListFramework : IIgnoreListFramework
    {
        private ILogger logger;

        public IgnoreListFramework(ILogger log)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            logger = log;
        }

        private static string ignoreFilePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "IgnoreFileList.xml");
        public async Task<IgnoreList> ReadIgnoreListAsync()
        {
            logger.TraceMessage("ReadIgnoreListAsync - Start");
            IgnoreList snm = new IgnoreList();
            //if the file doesn't yet exist then set a new version
            if (!File.Exists(ignoreFilePath))
            {
                logger.TraceMessage(string.Format("Ignore list file {0} does not yet exist, so creating a new version.", ignoreFilePath));
                return snm;
            }
            else
            {
                using (FileStream fs = new FileStream(ignoreFilePath, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(IgnoreList));
                    snm = (IgnoreList)serializer.Deserialize(fs);
                }
                logger.TraceMessage(string.Format("Ignore list file {0} deserialized.", ignoreFilePath));
                return snm;
            }
        }

        public async Task<bool> WriteIgnoreListAsync(IgnoreList ignoreList)
        {
            logger.TraceMessage("WriteIgnoreListAsync - Start");
            //only write the file if there is data
            if (ignoreList != null && ignoreList.IgnoreFiles.Count > 0)
            {
                using (TextWriter writer = new StreamWriter(ignoreFilePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(IgnoreList));
                    serializer.Serialize(writer, ignoreList);
                }
                logger.TraceMessage(string.Format("Ignore list file {0} serialized and written to file system.", ignoreFilePath));
            }
            else
            {
                logger.TraceMessage(string.Format("Ignore list empty so not writing the file {0}", ignoreFilePath));
            }

            logger.TraceMessage("WriteIgnoreListAsync - End");
            return true;
        }
    }
}
