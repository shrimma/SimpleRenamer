using SimpleRenamer.Framework.DataModel;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Interface
{
    public interface IFileMover
    {
        /// <summary>
        /// Moves a file to the destination file path. If configured will also rename the file.
        /// </summary>
        /// <param name="episode">The file to move</param>
        /// <param name="destinationFilePath">The destination path for the file</param>
        /// <returns></returns>
        Task<bool> MoveFileAsync(TVEpisode episode, string destinationFilePath);

        /// <summary>
        /// Create the series/season folder structure and downloads banners if configured
        /// </summary>
        /// <param name="episode">The file to move</param>
        /// <param name="mapping">The mapping of the file to TVDB</param>
        /// <param name="downloadBanner">Whether to download a banner</param>
        /// <returns></returns>
        Task<FileMoveResult> CreateDirectoriesAndDownloadBannersAsync(TVEpisode episode, Mapping mapping, bool downloadBanner);
    }
}
