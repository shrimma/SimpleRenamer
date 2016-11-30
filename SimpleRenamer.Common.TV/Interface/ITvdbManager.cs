using SimpleRenamer.Common.TV.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleRenamer.Common.TV.Interface
{
    public interface ITvdbManager
    {
        Task<List<SeriesSearchData>> SearchSeriesByNameAsync(string seriesName);

        Task<CompleteSeries> GetSeriesByIdAsync(string tmdbId);

        string GetBannerUri(string bannerPath);
    }
}
