﻿using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.Common.TV.Model;
using Sarjee.SimpleRenamer.WPF;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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
            if (this.Visibility == Visibility.Visible)
            {
                e.Cancel = true;
                //clear the UI
                ShowDescriptionTextBox.Text = string.Empty;
                ActorsListBox.ItemsSource = null;
                EpisodesListBox.ItemsSource = null;
                BannerImage.Source = null;

                this.Hide();
            }
        }

        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            WpfHelper.UpdateColumnsWidth(sender as ListView);
        }

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            WpfHelper.UpdateColumnsWidth(sender as ListView);
        }

        public async Task GetSeriesInfo(string showId)
        {
            _logger.TraceMessage("GetSeriesInfo - Start");
            LoadingProgress.IsActive = true;

            (CompleteSeries series, BitmapImage banner) = await _showMatcher.GetShowWithBannerAsync(showId);

            //set the title, show description, rating and firstaired values
            this.Title = string.Format("{0} - Rating {1} - First Aired {2}", series.Series.SeriesName, string.IsNullOrEmpty(series.Series.SiteRating.ToString()) ? "0.0" : series.Series.SiteRating.ToString(), string.IsNullOrEmpty(series.Series.FirstAired.ToString()) ? "1900" : series.Series.FirstAired.ToString());
            ShowDescriptionTextBox.Text = series.Series.Overview;

            //set the actor listbox
            ActorsListBox.ItemsSource = series.Actors;

            //set the episodes listbox
            EpisodesListBox.ItemsSource = series.Episodes.OrderBy(x => x.AiredEpisodeNumber).OrderBy(x => x.AiredSeason);

            //set the banner
            BannerImage.Source = banner;

            _logger.TraceMessage("GetSeriesInfo - End");
        }
    }
}
