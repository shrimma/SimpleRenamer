using SimpleRenamer.Framework.Interface;
using System;
using System.IO;
using System.Threading.Tasks;
using TheTVDBSharp;

namespace SimpleRenamer.Framework
{
    public class BannerDownloader : IBannerDownloader
    {
        private string apiKey;

        public BannerDownloader(IConfigurationManager configurationManager)
        {
            if (configurationManager == null)
            {
                throw new ArgumentNullException(nameof(configurationManager));
            }
            apiKey = configurationManager.TvDbApiKey;
        }

        public async Task<bool> SaveBannerAsync(string tvdbBannerPath, string destinationFolder)
        {
            string fullBannerPath = Path.Combine(destinationFolder, "Folder.jpg");
            if (!File.Exists(fullBannerPath))
            {
                TheTvdbManager tvdbManager = new TheTvdbManager(apiKey);
                using (Stream stream = await tvdbManager.GetBanner(tvdbBannerPath))
                {
                    using (FileStream fileStream = File.Create(fullBannerPath, (int)stream.Length))
                    {
                        byte[] bytesInStream = new byte[stream.Length];
                        stream.Read(bytesInStream, 0, bytesInStream.Length);
                        // Use write method to write to the file specified above
                        await fileStream.WriteAsync(bytesInStream, 0, bytesInStream.Length);
                    }
                }
            }

            return true;
        }
    }
}
