using System.Windows.Media.Imaging;
using TheTVDBSharp.Models;

namespace SimpleRenamer.Framework.DataModel
{
    public class SeriesWithBanner
    {
        public SeriesWithBanner(Series series, BitmapImage banner)
        {
            Series = series;
            BannerImage = banner;
        }
        public Series Series { get; set; }
        public BitmapImage BannerImage { get; set; }
    }
}
