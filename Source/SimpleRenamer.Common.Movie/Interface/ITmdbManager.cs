using Sarjee.SimpleRenamer.Common.Movie.Model;
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
        /// <param name="movieYear">The movie year.</param>
        /// <returns></returns>
        Task<SearchContainer<SearchMovie>> SearchMovieByNameAsync(string movieName, int movieYear);

        /// <summary>
        /// Searches the movie by identifier asynchronous.
        /// </summary>
        /// <param name="tmdbId">The TMDB identifier.</param>
        /// <returns></returns>
        Task<SearchMovie> SearchMovieByIdAsync(string tmdbId);

        /// <summary>
        /// Gets the movie asynchronous.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns></returns>
        Task<MovieCredits> GetMovieAsync(string movieId);

        /// <summary>
        /// Gets the poster URI asynchronous.
        /// </summary>
        /// <param name="posterPath">The poster path.</param>
        /// <returns></returns>
        Task<string> GetPosterUriAsync(string posterPath);
    }
}
