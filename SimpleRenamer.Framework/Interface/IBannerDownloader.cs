using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Interface
{
    public interface IBannerDownloader
    {
        Task<bool> SaveBannerAsync(string tvdbBannerPath, string destinationFolder);
    }
}
