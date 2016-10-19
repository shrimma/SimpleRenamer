using SimpleRenamer.Framework.DataModel;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Interface
{
    public interface IGetMovieDetails
    {
        /// <summary>
        /// Gets a movie and it's banner
        /// </summary>
        /// <param name="movieId">The TMDB movie ID</param>
        /// <returns>Populated MovieInfo object</returns>
        Task<MovieInfo> GetMovieWithBanner(string movieId);
    }
}
