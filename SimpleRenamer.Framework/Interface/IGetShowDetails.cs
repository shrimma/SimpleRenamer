using SimpleRenamer.Framework.DataModel;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Interface
{
    public interface IGetShowDetails
    {
        Task<SeriesWithBanner> GetShowWithBanner(string showId);
    }
}
