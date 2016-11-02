using SimpleRenamer.Framework.TvdbModel;
using System.Windows.Media.Imaging;

namespace SimpleRenamer.Framework.DataModel
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
