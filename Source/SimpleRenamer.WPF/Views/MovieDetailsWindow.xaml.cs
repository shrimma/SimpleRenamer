using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.Movie.Model;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
            e.Cancel = true;
            //clear the UI
            MovieTagLineTextBox.Text = string.Empty;
            MovieDescriptionTextBox.Text = string.Empty;
            ActorsListBox.ItemsSource = null;
            CrewListBox.ItemsSource = null;
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

        public async Task GetMovieInfo(string movieId)
        {
            _logger.TraceMessage("GetMovieInfo - Start");
            //enable progress spinner
            LoadingProgress.IsActive = true;
            CancellationTokenSource cts = new CancellationTokenSource();
            MovieInfo movie = await _movieMatcher.GetMovieWithBanner(movieId, cts.Token);

            //set the title, show description, rating and firstaired values
            this.Title = string.Format("{0} - Rating {1} - Year {2}", movie.Movie.Movie.Title, string.IsNullOrEmpty(movie.Movie.Movie.VoteAverage.ToString()) ? "0.0" : movie.Movie.Movie.VoteAverage.ToString(), movie.Movie.Movie.ReleaseDate.HasValue ? movie.Movie.Movie.ReleaseDate.Value.Year.ToString() : "1900");

            if (!string.IsNullOrEmpty(movie.Movie.Movie.Tagline))
            {
                MovieTagLineTextBox.Text = movie.Movie.Movie.Tagline;
                MovieTagLineTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                MovieTagLineTextBox.Visibility = Visibility.Collapsed;
            }

            MovieDescriptionTextBox.Text = movie.Movie.Movie.Overview;

            //set the actor listbox
            ActorsListBox.ItemsSource = movie.Movie.Credits.Cast;

            //set the crew listbox
            CrewListBox.ItemsSource = movie.Movie.Credits.Crew;

            //set the banner
            BannerImage.Source = movie.BannerImage;

            _logger.TraceMessage("GetMovieInfo - End");
        }
    }
}
