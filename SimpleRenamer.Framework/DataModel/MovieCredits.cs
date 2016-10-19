using TMDbLib.Objects.Movies;

namespace SimpleRenamer.Framework.DataModel
{
    public class MovieCredits
    {
        public Movie Movie { get; set; }
        public Credits Credits { get; set; }

        public MovieCredits(Movie movie, Credits credits)
        {
            Movie = movie;
            Credits = credits;
        }
    }
}
