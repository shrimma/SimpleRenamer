using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TheTVDBSharp.Models;

namespace SimpleRenamer.Framework.Interface
{
    public interface ITvdbManager
    {
        Task<List<Series>> SearchSeriesByNameAsync(string seriesName, int movieYear, CancellationToken ct);

        Task<Series> GetSeriesByIdAsync(string tmdbId, CancellationToken ct);

        Task<string> GetBannerUriAsync(string bannerPath, CancellationToken ct);
    }
}
