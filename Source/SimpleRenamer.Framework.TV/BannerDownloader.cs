using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using System;
using System.Diagnostics.Tracing;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Framework.TV
{
    /// <summary>
    /// Image Downloader
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.TV.Interface.IBannerDownloader" />
    public class BannerDownloader : IBannerDownloader
    {
        private ILogger _logger;
        private ITvdbManager _tvdbManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="BannerDownloader"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="tvdbManager">The TVDB manager.</param>
        /// <exception cref="System.ArgumentNullException">
        /// logger
        /// or
        /// tvdbManager
        /// </exception>
        public BannerDownloader(ILogger logger, ITvdbManager tvdbManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tvdbManager = tvdbManager ?? throw new ArgumentNullException(nameof(tvdbManager));
        }

        /// <summary>
        /// Saves a copy of a banner in a folder
        /// </summary>
        /// <param name="tvdbBannerPath">The remote TVDB banner path</param>
        /// <param name="destinationFolder">The destination folder to save the banner</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task<bool> SaveBannerAsync(string tvdbBannerPath, string destinationFolder)
        {
            _logger.TraceMessage($"Downloading banner {tvdbBannerPath} to {destinationFolder}.", EventLevel.Verbose);
            string fullBannerPath = Path.Combine(destinationFolder, "Folder.jpg");
            if (!File.Exists(fullBannerPath))
            {
                using (WebClient client = new WebClient())
                {
                    await client.DownloadFileTaskAsync(new Uri(_tvdbManager.GetBannerUri(tvdbBannerPath)), fullBannerPath);
                }
            }

            _logger.TraceMessage($"Downloaded banner {tvdbBannerPath} to {destinationFolder}.", EventLevel.Verbose);
            return true;
        }
    }
}
