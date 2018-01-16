using Sarjee.SimpleRenamer.Common.Model;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common.Interface
{
    /// <summary>
    /// FileHandler Interface
    /// </summary>
    public interface IFileMover
    {
        /// <summary>
        /// Moves a file to the destination file path. If configured will also rename the file.
        /// </summary>
        /// <param name="episode">The file to move</param>
        /// <param name="destinationFilePath">The destination path for the file</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        /// <returns></returns>
        Task<bool> MoveFileAsync(MatchedFile episode, CancellationToken cancellationToken);

        /// <summary>
        /// Create the folder structure and downloads banners if configured
        /// </summary>
        /// <param name="episode">The file to move</param>
        /// <param name="mapping">The mapping of the file to TVDB</param>
        /// <param name="downloadBanner">Whether to download a banner</param>
        /// <returns></returns>
        MatchedFile CreateDirectoriesAndQueueDownloadBanners(MatchedFile episode, Mapping mapping, bool downloadBanner);
    }
}
