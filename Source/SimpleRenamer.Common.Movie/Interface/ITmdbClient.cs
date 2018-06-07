using Sarjee.SimpleRenamer.Common.Movie.Model;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common.Movie.Interface
{
    public interface ITmdbClient
    {
        Task<SearchContainer<SearchMovie>> SearchMovieByNameAsync(string movieName, CancellationToken cancellationToken, int? movieYear = null);

        Task<Common.Movie.Model.Movie> GetMovieAsync(string movieId, CancellationToken cancellationToken);

        Task<SearchMovie> SearchMovieByIdAsync(string movieId, CancellationToken cancellationToken);

        Task<TMDbConfig> GetTmdbConfigAsync(CancellationToken cancellationToken);
    }
}
