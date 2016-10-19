using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System;
using System.IO;
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
            BitmapImage bannerImage = null;

            //get the show banner            
            //using (Stream stream = await tmdbManager.GetBanner(matchedMovie.PosterPath))
            //{
            //    byte[] bytesInStream = new byte[stream.Length];
            //    stream.Read(bytesInStream, 0, bytesInStream.Length);
            //    bannerImage = LoadImage(bytesInStream);
            //}


            return new MovieInfo(matchedMovie, bannerImage);
        }

        private BitmapImage LoadImage(byte[] imageData)
        {
            logger.TraceMessage("LoadImage - Start");
            if (imageData == null || imageData.Length == 0)
            {
                return null;
            }

            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();

            logger.TraceMessage("LoadImage - End");
            return image;
        }
    }
}
