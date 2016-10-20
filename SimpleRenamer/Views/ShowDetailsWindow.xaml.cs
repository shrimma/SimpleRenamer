using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleRenamer.Views
{
    /// <summary>
    /// Interaction logic for ShowDetailsForm.xaml
    /// </summary>
    public partial class ShowDetailsWindow : Window
    {
        private ILogger logger;
        private IGetShowDetails getShowDetails;

        public ShowDetailsWindow(ILogger log, IGetShowDetails getShow)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            if (getShow == null)
            {
                throw new ArgumentNullException(nameof(getShow));
            }

            logger = log;
            getShowDetails = getShow;

            try
            {
                InitializeComponent();
                this.Closing += Window_Closing;
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public async void GetSeries(string showId)
        {
            await GetSeriesInfo(showId);
        }

        private async Task GetSeriesInfo(string showId)
        {
            logger.TraceMessage("GetSeriesInfo - Start");

            SeriesWithBanner series = await getShowDetails.GetShowWithBanner(showId);

            //set the title, show description, rating and firstaired values
            this.Title = string.Format("{0} - Rating {1} - First Aired {2}", series.Series.Title, string.IsNullOrEmpty(series.Series.Rating.ToString()) ? "0.0" : series.Series.Rating.ToString(), string.IsNullOrEmpty(series.Series.FirstAired.ToString()) ? "1900" : series.Series.FirstAired.ToString());
            ShowDescriptionTextBox.Text = series.Series.Description;

            //set the actor listbox
            ActorsListBox.ItemsSource = series.Series.Actors;

            //set the episodes listbox
            EpisodesListBox.ItemsSource = series.Series.Episodes;

            //set the banner
            BannerImage.Source = series.BannerImage;

            logger.TraceMessage("GetSeriesInfo - End");
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            logger.TraceMessage("OKButton_Click - Start");
            this.Close();
        }
    }
}
