using SimpleRenamer.Framework.TvdbModel;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Interface
{
    public interface ITvdbManager
    {
        Task<SearchData> SearchSeriesByNameAsync(string seriesName, CancellationToken ct);

        Task<CompleteSeries> GetSeriesByIdAsync(string tmdbId, CancellationToken ct);

        Task<string> GetBannerUriAsync(string bannerPath, CancellationToken ct);
    }
}
