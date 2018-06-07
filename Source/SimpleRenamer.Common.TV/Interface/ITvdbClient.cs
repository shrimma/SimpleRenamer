using Sarjee.SimpleRenamer.Common.TV.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common.TV.Interface
{
    public interface ITvdbClient
    {        
        Task<SeriesData> GetSeriesAsync(string tmdbId, CancellationToken cancellationToken);

        Task<SeriesActors> GetActorsAsync(string tmdbId, CancellationToken cancellationToken);

        Task<SeriesEpisodes> GetEpisodesAsync(string tmdbId, CancellationToken cancellationToken);

        Task<SeriesImageQueryResults> GetSeriesPostersAsync(string tmdbId, CancellationToken cancellationToken);

        Task<SeriesImageQueryResults> GetSeasonPostersAsync(string tmdbId, CancellationToken cancellationToken);

        Task<SeriesImageQueryResults> GetSeriesBannersAsync(string tmdbId, CancellationToken cancellationToken);

        Task<List<SeriesSearchData>> SearchSeriesByNameAsync(string seriesName, CancellationToken cancellationToken);        
    }
}
