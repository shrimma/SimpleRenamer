using Sarjee.SimpleRenamer.Common.Movie.Model;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common.Movie.Interface
{
    public interface IGetMovieDetails
    {
        /// <summary>
        /// Gets a movie and it's banner
        /// </summary>
        /// <param name="movieId">The TMDB movie ID</param>
        /// <returns>Populated MovieInfo object</returns>
        Task<MovieInfo> GetMovieWithBanner(string movieId, CancellationToken ct);
    }
}
