using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TheTVDBSharp;
using TheTVDBSharp.Models;

namespace SimpleRenamer.Framework
{
    public class GetShowDetails : IGetShowDetails
    {
        private ILogger logger;
        private ITheTvdbManager tvdbManager;

        public GetShowDetails(ILogger log, IConfigurationManager configurationManager)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            if (configurationManager == null)
            {
                throw new ArgumentNullException(nameof(configurationManager));
            }

            logger = log;
            tvdbManager = new TheTvdbManager(configurationManager.TvDbApiKey);
        }


        public async Task<SeriesWithBanner> GetShowWithBanner(string showId)
        {
            logger.TraceMessage("GetSeriesInfo - Start");
            uint sId = 0;
            uint.TryParse(showId, out sId);
            Series matchedSeries = await tvdbManager.GetSeries(sId, Language.English);
            BitmapImage bannerImage = null;

            //get the show banner
            List<SeriesBanner> seriesBanners = matchedSeries.Banners.OfType<SeriesBanner>().Where(s => s.Language == Language.English).ToList();
            if (seriesBanners != null && seriesBanners.Count > 0)
            {
                string bannerPath = seriesBanners.OrderByDescending(s => s.Rating).FirstOrDefault().RemotePath;
                using (Stream stream = await tvdbManager.GetBanner(bannerPath))
                {
                    byte[] bytesInStream = new byte[stream.Length];
                    stream.Read(bytesInStream, 0, bytesInStream.Length);
                    bannerImage = LoadImage(bytesInStream);
                }
            }

            return new SeriesWithBanner(matchedSeries, bannerImage);
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
