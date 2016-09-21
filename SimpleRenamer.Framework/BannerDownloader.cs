using SimpleRenamer.Framework.Interface;
using System;
using System.IO;
using System.Threading.Tasks;
using TheTVDBSharp;

namespace SimpleRenamer.Framework
{
    public class BannerDownloader : IBannerDownloader
    {
        private ILogger logger;
        private ITheTvdbManager tvdbManager;

        public BannerDownloader(ITheTvdbManager tvdb, ILogger log)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            if (tvdb == null)
            {
                throw new ArgumentNullException(nameof(tvdb));
            }

            logger = log;
            tvdbManager = tvdb;
        }

        /// <inheritdoc/>
        public async Task<bool> SaveBannerAsync(string tvdbBannerPath, string destinationFolder)
        {
            logger.TraceMessage("SaveBannerAsync - Start");
            string fullBannerPath = Path.Combine(destinationFolder, "Folder.jpg");
            if (!File.Exists(fullBannerPath))
            {
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

            logger.TraceMessage("SaveBannerAsync - End");
            return true;
        }
    }
}
