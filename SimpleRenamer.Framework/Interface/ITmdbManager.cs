using SimpleRenamer.Framework.DataModel;
using System.Threading;
using System.Threading.Tasks;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace SimpleRenamer.Framework.Interface
{
    public interface ITmdbManager
    {
        Task<SearchContainer<SearchMovie>> SearchMovieByNameAsync(string movieName, int movieYear, CancellationToken ct);

        Task<SearchMovie> SearchMovieByIdAsync(string tmdbId, CancellationToken ct);

        Task<MovieCredits> GetMovieAsync(string movieId, CancellationToken ct);

        Task<string> GetPosterUriAsync(string posterPath, CancellationToken ct);
    }
}
