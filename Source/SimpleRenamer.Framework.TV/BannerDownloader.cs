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
    public class BannerDownloader : IBannerDownloader, IDisposable
    {
        private ILogger _logger;
        private ITvdbManager _tvdbManager;
        private WebClient _webClient;

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
            _webClient = new WebClient();
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
            //throw if arguments are missing
            if (string.IsNullOrWhiteSpace(tvdbBannerPath))
            {
                throw new ArgumentNullException(nameof(tvdbBannerPath));
            }
            if (string.IsNullOrWhiteSpace(destinationFolder))
            {
                throw new ArgumentNullException(nameof(destinationFolder));
            }

            string fullBannerPath = Path.Combine(destinationFolder, "Folder.jpg");
            if (!File.Exists(fullBannerPath))
            {
                _logger.TraceMessage($"Downloading banner {tvdbBannerPath} to {destinationFolder}.", EventLevel.Verbose);
                await Download(tvdbBannerPath, fullBannerPath);
                _logger.TraceMessage($"Downloaded banner {tvdbBannerPath} to {destinationFolder}.", EventLevel.Verbose);
            }
            else
            {
                _logger.TraceMessage($"No need to download as banner already exists at {destinationFolder}.", EventLevel.Verbose);
            }

            return true;
        }

        protected virtual async Task<bool> Download(string tvdbBannerPath, string bannerPath)
        {

            await _webClient.DownloadFileTaskAsync(new Uri(_tvdbManager.GetBannerUri(tvdbBannerPath)), bannerPath);

            return true;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_webClient != null)
                    {
                        _webClient.Dispose();
                        _webClient = null;
                    }
                }

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~BannerDownloader() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
