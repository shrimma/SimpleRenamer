using Sarjee.SimpleRenamer.Common.TV.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common.TV.Interface
{
    public interface ITvdbClient
    {        
        Task<SeriesData> GetSeriesAsync(string tmdbId, CancellationToken cancellationToken);

        Task<SeriesActors> GetActorsAsync(string tmdbId, CancellationToken cancellationToken);

        Task<SeriesEpisodes> GetEpisodesAsync(string tmdbId, CancellationToken cancellationToken);        

        Task<SeriesImageQueryResults> QuerySeriesImagesAsync(string tmdbId, string imageKeyType, CancellationToken cancellationToken);

        Task<SeriesSearchDataList> SearchSeriesByNameAsync(string seriesName, CancellationToken cancellationToken);        
    }
}
