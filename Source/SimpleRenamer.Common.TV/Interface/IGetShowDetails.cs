using Sarjee.SimpleRenamer.Common.TV.Model;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common.TV.Interface
{
    public interface IGetShowDetails
    {
        /// <summary>
        /// Gets a show and it's banner
        /// </summary>
        /// <param name="showId">The TVDB show ID to grab banner for</param>
        /// <returns>Populated SeriesWithBanner object</returns>
        Task<SeriesWithBanner> GetShowWithBannerAsync(string showId);
    }
}
