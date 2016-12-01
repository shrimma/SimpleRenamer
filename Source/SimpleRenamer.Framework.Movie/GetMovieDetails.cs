using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Model;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Sarjee.SimpleRenamer.Framework.Movie
{
    public class GetMovieDetails : IGetMovieDetails
    {
        private ILogger _logger;
        private ITmdbManager _tmdbManager;

        public GetMovieDetails(ILogger logger, ITmdbManager tmdbManager)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            if (tmdbManager == null)
            {
                throw new ArgumentNullException(nameof(tmdbManager));
            }

            _logger = logger;
            _tmdbManager = tmdbManager;
        }


        public async Task<MovieInfo> GetMovieWithBanner(string movieId, CancellationToken ct)
        {
            _logger.TraceMessage("GetMovieInfo - Start");
            MovieCredits matchedMovie = await _tmdbManager.GetMovieAsync(movieId);
            BitmapImage bannerImage = new BitmapImage();

            if (!string.IsNullOrEmpty(matchedMovie.Movie.PosterPath))
            {
                bannerImage.BeginInit();
                bannerImage.UriSource = new Uri(await _tmdbManager.GetPosterUriAsync(matchedMovie.Movie.PosterPath));
                bannerImage.EndInit();
            }
            else
            {
                //TODO add a not found poster
            }

            _logger.TraceMessage("GetMovieInfo - End");
            return new MovieInfo(matchedMovie, bannerImage);
        }
    }
}
