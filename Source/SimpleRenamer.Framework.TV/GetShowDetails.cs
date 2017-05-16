using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Common.TV.Model;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Sarjee.SimpleRenamer.Framework.TV
{
    public class GetShowDetails : IGetShowDetails
    {
        private ILogger _logger;
        private ITvdbManager _tvdbManager;

        public GetShowDetails(ILogger logger, ITvdbManager tvdbManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tvdbManager = tvdbManager ?? throw new ArgumentNullException(nameof(tvdbManager));
        }

        public async Task<SeriesWithBanner> GetShowWithBannerAsync(string showId)
        {
            _logger.TraceMessage("GetSeriesInfo - Start");
            CompleteSeries matchedSeries = await _tvdbManager.GetSeriesByIdAsync(showId);
            BitmapImage bannerImage = new BitmapImage();
            if (matchedSeries.SeriesBanners != null && matchedSeries.SeriesBanners.Count > 0)
            {
                bannerImage.BeginInit();
                bannerImage.UriSource = new Uri(_tvdbManager.GetBannerUri(matchedSeries.SeriesBanners.OrderByDescending(s => s.RatingsInfo.Average).FirstOrDefault().FileName));
                bannerImage.EndInit();
            }
            else
            {
                //TODO create a no image found banner
            }

            _logger.TraceMessage("GetSeriesInfo - End");
            return new SeriesWithBanner(matchedSeries, bannerImage);
        }
    }
}
