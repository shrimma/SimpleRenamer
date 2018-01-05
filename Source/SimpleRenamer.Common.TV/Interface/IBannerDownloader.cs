using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common.TV.Interface
{
    /// <summary>
    /// Banner Downloader Interface
    /// </summary>
    public interface IBannerDownloader
    {
        /// <summary>
        /// Queues the download of a banner
        /// </summary>
        /// <param name="tvdbBannerPath">The remote TVDB banner path</param>
        /// <param name="destinationFolder">The destination folder to save the banner</param>
        /// <returns></returns>
        bool QueueBannerDownload(string tvdbBannerPath, string destinationFolder);
    }
}
