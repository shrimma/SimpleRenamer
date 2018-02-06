using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Model;
using Sarjee.SimpleRenamer.WPF;
using System;
using System.Diagnostics.Tracing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Sarjee.SimpleRenamer.Views
{
    /// <summary>
    /// Interaction logic for ShowDetailsForm.xaml
    /// </summary>
    public partial class MovieDetailsWindow
    {
        private ILogger _logger;
        private IMovieMatcher _movieMatcher;

        public MovieDetailsWindow(ILogger logger, IMovieMatcher movieMatcher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _movieMatcher = movieMatcher ?? throw new ArgumentNullException(nameof(movieMatcher));

            try
            {
                InitializeComponent();
                ActorsListBox.SizeChanged += ListView_SizeChanged;
                CrewListBox.SizeChanged += ListView_SizeChanged;
                ActorsListBox.Loaded += ListView_Loaded;
                CrewListBox.Loaded += ListView_Loaded;
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
                MovieTagLineTextBox.Text = string.Empty;
                MovieDescriptionTextBox.Text = string.Empty;
                ActorsListBox.ItemsSource = null;
                CrewListBox.ItemsSource = null;
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

        public async Task GetMovieInfo(string movieId, CancellationToken cancellationToken)
        {
            _logger.TraceMessage($"Getting MovieInfo for {movieId}.", EventLevel.Verbose);
            //enable progress spinner
            LoadingProgress.IsActive = true;
            (Movie movie, Uri bannerUri) = await _movieMatcher.GetMovieWithBannerAsync(movieId, cancellationToken);

            //set the title, show description, rating and firstaired values
            this.Title = string.Format("{0} - Rating {1} - Year {2}", movie.Title, string.IsNullOrWhiteSpace(movie.VoteAverage.ToString()) ? "0.0" : movie.VoteAverage.ToString(), movie.ReleaseDate.HasValue ? movie.ReleaseDate.Value.Year.ToString() : "1900");

            if (!string.IsNullOrWhiteSpace(movie.Tagline))
            {
                MovieTagLineTextBox.Text = movie.Tagline;
                MovieTagLineTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                MovieTagLineTextBox.Visibility = Visibility.Collapsed;
            }

            MovieDescriptionTextBox.Text = movie.Overview;

            //set the actor listbox
            ActorsListBox.ItemsSource = movie.Credits.Cast;

            //set the crew listbox
            CrewListBox.ItemsSource = movie.Credits.Crew;

            //set the banner
            BitmapImage banner = new BitmapImage();
            if (bannerUri != null)
            {
                banner.BeginInit();
                banner.UriSource = bannerUri;
                banner.EndInit();
            }
            BannerImage.Source = banner;

            _logger.TraceMessage($"Got MovieInfo for {movieId}.", EventLevel.Verbose);
        }
    }
}
