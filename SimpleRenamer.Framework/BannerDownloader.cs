using SimpleRenamer.Framework.Interface;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework
{
    public class BannerDownloader : IBannerDownloader
    {
        private ILogger logger;
        private ITvdbManager tvdbManager;

        public BannerDownloader(ILogger log, ITvdbManager tvdb)
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
        public async Task<bool> SaveBannerAsync(string tvdbBannerPath, string destinationFolder, CancellationToken ct)
        {
            logger.TraceMessage("SaveBannerAsync - Start");
            string fullBannerPath = Path.Combine(destinationFolder, "Folder.jpg");
            if (!File.Exists(fullBannerPath))
            {

                using (WebClient client = new WebClient())
                {
                    client.DownloadFileAsync(new Uri(tvdbManager.GetBannerUri(tvdbBannerPath)), fullBannerPath);
                }
            }

            logger.TraceMessage("SaveBannerAsync - End");
            return true;
        }
    }
}
