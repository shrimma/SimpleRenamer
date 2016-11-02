using System.Windows.Media.Imaging;

namespace SimpleRenamer.Framework.DataModel
{
    public class MovieInfo
    {
        public MovieInfo(MovieCredits movie, BitmapImage banner)
        {
            Movie = movie;
            BannerImage = banner;
        }
        public MovieCredits Movie { get; set; }
        public BitmapImage BannerImage { get; set; }
    }
}
