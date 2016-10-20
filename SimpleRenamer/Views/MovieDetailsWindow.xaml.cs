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
    public partial class MovieDetailsWindow : Window
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

        public async void GetMovie(string movieId)
        {
            await GetMovieInfo(movieId);
        }

        private async Task GetMovieInfo(string movieId)
        {
            logger.TraceMessage("GetMovieInfo - Start");

            MovieInfo movie = await getMovieDetails.GetMovieWithBanner(movieId);

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

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            logger.TraceMessage("OKButton_Click - Start");
            this.Close();
        }
    }
}
