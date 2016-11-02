using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SimpleRenamer.Views
{
    /// <summary>
    /// Interaction logic for ShowDetailsForm.xaml
    /// </summary>
    public partial class MovieDetailsWindow
    {
        private ILogger logger;
        private IGetMovieDetails getMovieDetails;

        public MovieDetailsWindow(ILogger log, IGetMovieDetails getMovie)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            if (getMovie == null)
            {
                throw new ArgumentNullException(nameof(getMovie));
            }

            logger = log;
            getMovieDetails = getMovie;

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
                logger.TraceException(ex);
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
            logger.TraceMessage("GetMovieInfo - Start");
            //enable progress spinner
            LoadingProgress.IsActive = true;
            CancellationTokenSource cts = new CancellationTokenSource();
            MovieInfo movie = await getMovieDetails.GetMovieWithBanner(movieId, cts.Token);

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

            logger.TraceMessage("GetMovieInfo - End");
        }
    }
}
