using System.Windows.Media.Imaging;

namespace Sarjee.SimpleRenamer.Common.TV.Model
{
    public class SeriesWithBanner
    {
        public SeriesWithBanner(CompleteSeries series, BitmapImage banner)
        {
            Series = series;
            BannerImage = banner;
        }
        public CompleteSeries Series { get; set; }
        public BitmapImage BannerImage { get; set; }
    }
}
