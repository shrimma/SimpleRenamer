using SimpleRenamer.Framework.DataModel;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Interface
{
    public interface IGetShowDetails
    {
        /// <summary>
        /// Gets a show and it's banner
        /// </summary>
        /// <param name="showId">The TVDB show ID to grab banner for</param>
        /// <returns>Populated SeriesWithBanner object</returns>
        Task<SeriesWithBanner> GetShowWithBanner(string showId);
    }
}
