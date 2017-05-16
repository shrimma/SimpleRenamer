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
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tvdbManager = tvdbManager ?? throw new ArgumentNullException(nameof(tvdbManager));
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
                    await client.DownloadFileTaskAsync(new Uri(_tvdbManager.GetBannerUri(tvdbBannerPath)), fullBannerPath);
                }
            }

            _logger.TraceMessage("SaveBannerAsync - End");
            return true;
        }
    }
}
