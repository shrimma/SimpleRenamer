using SimpleRenamer.Framework.TvdbModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Interface
{
    public interface ITvdbManager
    {
        Task<List<SeriesSearchData>> SearchSeriesByNameAsync(string seriesName);

        Task<CompleteSeries> GetSeriesByIdAsync(string tmdbId);

        string GetBannerUri(string bannerPath);
    }
}
