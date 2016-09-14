using SimpleRenamer.Framework.DataModel;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Interface
{
    public interface IFileMover
    {
        Task<bool> MoveFileAsync(TVEpisode episode, string destinationFilePath);

        Task<FileMoveResult> CreateDirectoriesAndDownloadBannersAsync(TVEpisode episode, Mapping mapping, bool downloadBanner);
    }
}
