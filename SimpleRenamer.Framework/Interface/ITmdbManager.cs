using SimpleRenamer.Framework.DataModel;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace SimpleRenamer.Framework.Interface
{
    public interface ITmdbManager
    {
        SearchContainer<SearchMovie> SearchMovieByName(string movieName, int movieYear);

        SearchMovie SearchMovieById(string tmdbId);

        MovieCredits GetMovie(string movieId);
    }
}
