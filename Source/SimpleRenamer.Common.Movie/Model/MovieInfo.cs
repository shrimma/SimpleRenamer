using System;
using System.Windows.Media.Imaging;

namespace Sarjee.SimpleRenamer.Common.Movie.Model
{
    public class MovieInfo
    {
        public MovieInfo(Movie movie, BitmapImage banner)
        {
            Movie = movie ?? throw new ArgumentNullException(nameof(movie));
            BannerImage = banner ?? throw new ArgumentNullException(nameof(banner));
        }
        public Movie Movie { get; set; }
        public BitmapImage BannerImage { get; set; }
    }
}
