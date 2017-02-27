using Sarjee.SimpleRenamer.Common.TV.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common.TV.Interface
{
    public interface ITvdbManager
    {
        Task<List<SeriesSearchData>> SearchSeriesByNameAsync(string seriesName);

        Task<CompleteSeries> GetSeriesByIdAsync(string tmdbId);

        string GetBannerUri(string bannerPath);
    }
}
