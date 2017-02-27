using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Framework.TV
{
    public class BannerDownloader : IBannerDownloader
    {
        private ILogger _logger;
        private ITvdbManager _tvdbManager;

        public BannerDownloader(ILogger logger, ITvdbManager tvdbManager)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            if (tvdbManager == null)
            {
                throw new ArgumentNullException(nameof(tvdbManager));
            }

            _logger = logger;
            _tvdbManager = tvdbManager;
        }

        /// <inheritdoc/>
        public async Task<bool> SaveBannerAsync(string tvdbBannerPath, string destinationFolder)
        {
            _logger.TraceMessage("SaveBannerAsync - Start");
            string fullBannerPath = Path.Combine(destinationFolder, "Folder.jpg");
            if (!File.Exists(fullBannerPath))
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFileAsync(new Uri(_tvdbManager.GetBannerUri(tvdbBannerPath)), fullBannerPath);
                }
            }

            _logger.TraceMessage("SaveBannerAsync - End");
            return true;
        }
    }
}
