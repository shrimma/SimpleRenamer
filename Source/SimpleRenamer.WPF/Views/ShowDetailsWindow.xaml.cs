using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Common.TV.Model;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sarjee.SimpleRenamer.Views
{
    /// <summary>
    /// Interaction logic for ShowDetailsForm.xaml
    /// </summary>
    public partial class ShowDetailsWindow
    {
        private ILogger _logger;
        private ITVShowMatcher _showMatcher;

        public ShowDetailsWindow(ILogger logger, ITVShowMatcher showMatcher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _showMatcher = showMatcher ?? throw new ArgumentNullException(nameof(showMatcher));

            try
            {
                InitializeComponent();
                ActorsListBox.SizeChanged += ListView_SizeChanged;
                EpisodesListBox.SizeChanged += ListView_SizeChanged;
                ActorsListBox.Loaded += ListView_Loaded;
                EpisodesListBox.Loaded += ListView_Loaded;
                this.Closing += Window_Closing;
            }
            catch (Exception ex)
            {
                _logger.TraceException(ex);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            //clear the UI
            ShowDescriptionTextBox.Text = string.Empty;
            ActorsListBox.ItemsSource = null;
            EpisodesListBox.ItemsSource = null;
            BannerImage.Source = null;

            this.Hide();
        }

        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateColumnsWidth(sender as ListView);
        }

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateColumnsWidth(sender as ListView);
        }

        private void UpdateColumnsWidth(ListView listView)
        {
            int autoFillColumnIndex = (listView.View as GridView).Columns.Count - 1;
            if (listView.ActualWidth == Double.NaN)
                listView.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            double remainingSpace = listView.ActualWidth;
            for (int i = 0; i < (listView.View as GridView).Columns.Count; i++)
                if (i != autoFillColumnIndex)
                    remainingSpace -= (listView.View as GridView).Columns[i].ActualWidth;
            (listView.View as GridView).Columns[autoFillColumnIndex].Width = remainingSpace >= 0 ? remainingSpace : 0;
        }

        public async Task GetSeriesInfo(string showId)
        {
            _logger.TraceMessage("GetSeriesInfo - Start");
            LoadingProgress.IsActive = true;

            SeriesWithBanner series = await _showMatcher.GetShowWithBannerAsync(showId);

            //set the title, show description, rating and firstaired values
            this.Title = string.Format("{0} - Rating {1} - First Aired {2}", series.Series.Series.SeriesName, string.IsNullOrEmpty(series.Series.Series.SiteRating.ToString()) ? "0.0" : series.Series.Series.SiteRating.ToString(), string.IsNullOrEmpty(series.Series.Series.FirstAired.ToString()) ? "1900" : series.Series.Series.FirstAired.ToString());
            ShowDescriptionTextBox.Text = series.Series.Series.Overview;

            //set the actor listbox
            ActorsListBox.ItemsSource = series.Series.Actors;

            //set the episodes listbox
            EpisodesListBox.ItemsSource = series.Series.Episodes.OrderBy(x => x.AiredEpisodeNumber).OrderBy(x => x.AiredSeason);

            //set the banner
            BannerImage.Source = series.BannerImage;

            _logger.TraceMessage("GetSeriesInfo - End");
        }
    }
}
