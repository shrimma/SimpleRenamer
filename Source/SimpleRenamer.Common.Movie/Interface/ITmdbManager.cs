using Sarjee.SimpleRenamer.Common.Movie.Model;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common.Movie.Interface
{
    /// <summary>
    /// TMDB Manager Interface
    /// </summary>
    public interface ITmdbManager
    {
        /// <summary>
        /// Searches the movie by name asynchronous.
        /// </summary>
        /// <param name="movieName">Name of the movie.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="movieYear">The movie year.</param>
        /// <returns></returns>
        Task<SearchContainer<SearchMovie>> SearchMovieByNameAsync(string movieName, CancellationToken cancellationToken, int? movieYear = null);

        /// <summary>
        /// Searches the movie by identifier asynchronous.
        /// </summary>
        /// <param name="tmdbId">The TMDB identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<SearchMovie> SearchMovieByIdAsync(string tmdbId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the movie asynchronous.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<Model.Movie> GetMovieAsync(string movieId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the poster URI asynchronous.
        /// </summary>
        /// <param name="posterPath">The poster path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<string> GetPosterUriAsync(string posterPath, CancellationToken cancellationToken);
    }
}
