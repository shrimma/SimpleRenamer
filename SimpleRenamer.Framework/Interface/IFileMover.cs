using SimpleRenamer.Framework.DataModel;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Interface
{
    public interface IFileMover
    {
        Task<bool> MoveFileAsync(TVEpisode episode, Settings settings, string destinationFilePath);

        Task<FileMoveResult> CreateDirectoriesAndDownloadBannersAsync(TVEpisode episode, Mapping mapping, Settings settings, bool downloadBanner);
    }
}
