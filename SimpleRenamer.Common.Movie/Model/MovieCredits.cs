namespace SimpleRenamer.Common.Movie.Model
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
