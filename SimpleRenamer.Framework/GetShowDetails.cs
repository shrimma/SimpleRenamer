using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using SimpleRenamer.Framework.TvdbModel;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SimpleRenamer.Framework
{
    public class GetShowDetails : IGetShowDetails
    {
        private ILogger logger;
        private ITvdbManager tvdbManager;

        public GetShowDetails(ILogger log, ITvdbManager tvdb)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            if (tvdb == null)
            {
                throw new ArgumentNullException(nameof(tvdb));
            }

            logger = log;
            tvdbManager = tvdb;
        }

        public async Task<SeriesWithBanner> GetShowWithBannerAsync(string showId, CancellationToken ct)
        {
            logger.TraceMessage("GetSeriesInfo - Start");
            CompleteSeries matchedSeries = await tvdbManager.GetSeriesByIdAsync(showId, ct);
            BitmapImage bannerImage = new BitmapImage();
            bannerImage.BeginInit();
            if (matchedSeries.SeriesBanners != null && matchedSeries.SeriesBanners.Count > 0)
            {
                bannerImage.UriSource = new Uri(await tvdbManager.GetBannerUriAsync(matchedSeries.SeriesBanners.OrderByDescending(s => s.RatingsInfo.Average).FirstOrDefault().FileName, ct));
            }
            else
            {
                //TODO add a no banner found image
            }
            bannerImage.EndInit();

            return new SeriesWithBanner(matchedSeries, bannerImage);
        }
    }
}
