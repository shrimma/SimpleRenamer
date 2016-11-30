using SimpleRenamer.Common.Movie.Model;
using System.Threading.Tasks;

namespace SimpleRenamer.Common.Movie.Interface
{
    public interface ITmdbManager
    {
        Task<SearchContainer<SearchMovie>> SearchMovieByNameAsync(string movieName, int movieYear);

        Task<SearchMovie> SearchMovieByIdAsync(string tmdbId);

        Task<MovieCredits> GetMovieAsync(string movieId);

        Task<string> GetPosterUriAsync(string posterPath);
    }
}
