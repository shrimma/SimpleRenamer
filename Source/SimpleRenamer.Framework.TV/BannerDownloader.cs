using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using System;
using System.Collections.Concurrent;
using System.Diagnostics.Tracing;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Framework.TV
{
    /// <summary>
    /// Image Downloader
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.TV.Interface.IBannerDownloader" />
    public class BannerDownloader : IBannerDownloader, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ITvdbManager _tvdbManager;
        private readonly ConcurrentQueue<(Uri tvdbUri, string bannerFilePath)> _downloadQueue;
        private WebClient _webClient;
        protected CancellationTokenSource _cancellationTokenSource;
        protected Task _longRunningTask;

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
            _downloadQueue = new ConcurrentQueue<(Uri tvdbUri, string bannerFilePath)>();
            _cancellationTokenSource = new CancellationTokenSource();

            //start out background task that polls for banners to download
            _longRunningTask = Task.Factory.StartNew(async () =>
            {
                await DownloadItemsFromQueue(_cancellationTokenSource.Token);
            }, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        /// <summary>
        /// Saves a copy of a banner in a folder
        /// </summary>
        /// <param name="tvdbBannerPath">The remote TVDB banner path</param>
        /// <param name="destinationFolder">The destination folder to save the banner</param>
        /// <returns></returns>
        /// <inheritdoc />
        public bool QueueBannerDownload(string tvdbBannerPath, string destinationFolder)
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
                _logger.TraceMessage($"Queueing download of banner {tvdbBannerPath} to {destinationFolder}.", EventLevel.Verbose);
                _downloadQueue.Enqueue((new Uri(_tvdbManager.GetBannerUri(tvdbBannerPath)), fullBannerPath));
                _logger.TraceMessage($"Queued download of banner {tvdbBannerPath} to {destinationFolder}.", EventLevel.Verbose);
            }
            else
            {
                _logger.TraceMessage($"No need to download as banner already exists at {destinationFolder}.", EventLevel.Verbose);
            }

            return true;
        }

        private async Task DownloadItemsFromQueue(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (_downloadQueue.TryDequeue(out (Uri tvdbUri, string bannerFilePath) currentItem))
                {
                    await DownloadItem(currentItem.tvdbUri, currentItem.bannerFilePath);
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
        }
        protected virtual async Task<bool> DownloadItem(Uri tvdbUri, string bannerFilePath)
        {
            await _webClient.DownloadFileTaskAsync(tvdbUri, bannerFilePath);
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
                    if (_cancellationTokenSource != null)
                    {
                        _cancellationTokenSource.Dispose();
                        _cancellationTokenSource = null;
                    }
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
