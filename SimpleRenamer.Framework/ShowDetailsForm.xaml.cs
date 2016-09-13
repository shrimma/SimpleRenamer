using SimpleRenamer.Framework.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using TheTVDBSharp;
using TheTVDBSharp.Models;

namespace SimpleRenamer.Framework
{
    /// <summary>
    /// Interaction logic for ShowDetailsForm.xaml
    /// </summary>
    public partial class ShowDetailsForm : Window
    {
        private ILogger logger;
        public ShowDetailsForm(ILogger log)
        {
            logger = log;
        }
        private static string apiKey = "820147144A5BB54E";
        public ShowDetailsForm(string showId)
        {
            try
            {
                InitializeComponent();
                GetSeriesInfo(showId);
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        private async void GetSeriesInfo(string showId)
        {
            uint sId = 0;
            uint.TryParse(showId, out sId);
            TheTvdbManager tvdbManager = new TheTvdbManager(apiKey);
            Series matchedSeries = await tvdbManager.GetSeries(sId, TheTVDBSharp.Models.Language.English);

            //get the show banner
            List<SeriesBanner> seriesBanners = matchedSeries.Banners.OfType<SeriesBanner>().Where(s => s.Language == TheTVDBSharp.Models.Language.English).ToList();
            if (seriesBanners != null && seriesBanners.Count > 0)
            {
                string bannerPath = seriesBanners.OrderByDescending(s => s.Rating).FirstOrDefault().RemotePath;
                using (Stream stream = await tvdbManager.GetBanner(bannerPath))
                {
                    byte[] bytesInStream = new byte[stream.Length];
                    stream.Read(bytesInStream, 0, bytesInStream.Length);
                    BannerImage.Source = LoadImage(bytesInStream);
                }
            }

            //set the title, show description, rating and firstaired values
            this.Title = string.Format("{0} - Rating {1} - First Aired {2}", matchedSeries.Title, string.IsNullOrEmpty(matchedSeries.Rating.ToString()) ? "0.0" : matchedSeries.Rating.ToString(), string.IsNullOrEmpty(matchedSeries.FirstAired.ToString()) ? "1900" : matchedSeries.FirstAired.ToString());
            ShowDescriptionTextBox.Text = matchedSeries.Description;

            //set the actor listbox
            ActorsListBox.ItemsSource = matchedSeries.Actors;

            //set the episodes listbox
            EpisodesListBox.ItemsSource = matchedSeries.Episodes;
        }

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
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
            return image;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
