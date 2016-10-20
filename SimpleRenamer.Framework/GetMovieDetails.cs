using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SimpleRenamer.Framework
{
    public class GetMovieDetails : IGetMovieDetails
    {
        private ILogger logger;
        private ITmdbManager tmdbManager;

        public GetMovieDetails(ILogger log, ITmdbManager tmdb)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            if (tmdb == null)
            {
                throw new ArgumentNullException(nameof(tmdb));
            }

            logger = log;
            tmdbManager = tmdb;
        }


        public async Task<MovieInfo> GetMovieWithBanner(string movieId)
        {
            logger.TraceMessage("GetMovieInfo - Start");
            MovieCredits matchedMovie = tmdbManager.GetMovie(movieId);
            BitmapImage bannerImage = new BitmapImage();
            bannerImage.BeginInit();
            bannerImage.UriSource = new Uri(tmdbManager.GetPosterUri(matchedMovie.Movie.PosterPath));
            bannerImage.EndInit();

            return new MovieInfo(matchedMovie, bannerImage);
        }
    }
}
