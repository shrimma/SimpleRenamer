using Sarjee.SimpleRenamer.Common.TV.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common.TV.Interface
{
    /// <summary>
    /// TVDB Manager Interface
    /// </summary>
    public interface ITvdbManager
    {
        /// <summary>
        /// Searches the series by name asynchronous.
        /// </summary>
        /// <param name="seriesName">Name of the series.</param>
        /// <returns></returns>
        Task<List<SeriesSearchData>> SearchSeriesByNameAsync(string seriesName);

        /// <summary>
        /// Gets the series by identifier asynchronous.
        /// </summary>
        /// <param name="tmdbId">The TMDB identifier.</param>
        /// <returns></returns>
        Task<CompleteSeries> GetSeriesByIdAsync(string tmdbId);

        /// <summary>
        /// Gets the banner URI.
        /// </summary>
        /// <param name="bannerPath">The banner path.</param>
        /// <returns></returns>
        string GetBannerUri(string bannerPath);
    }
}
