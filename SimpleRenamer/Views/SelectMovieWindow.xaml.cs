using SimpleRenamer.EventArguments;
using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SimpleRenamer.Views
{
    /// <summary>
    /// Interaction logic for SelectShowWpfForm.xaml
    /// </summary>
    public partial class SelectMovieWindow
    {
        public event EventHandler<SelectMovieEventArgs> RaiseSelectMovieWindowEvent;

        private ILogger logger;
        private IMovieMatcher movieMatcher;
        private MovieDetailsWindow movieDetailsWindow;

        public SelectMovieWindow(ILogger log, IMovieMatcher movieMatch, MovieDetailsWindow movieDetails)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            if (movieMatch == null)
            {
                throw new ArgumentNullException(nameof(movieMatch));
            }
            if (movieDetails == null)
            {
                throw new ArgumentNullException(nameof(movieDetails));
            }

            logger = log;
            movieMatcher = movieMatch;
            movieDetailsWindow = movieDetails;

            InitializeComponent();
            this.Closing += Window_Closing;
        }

        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RaiseSelectMovieWindowEvent(this, new SelectMovieEventArgs(null));
            e.Cancel = true;
            this.Hide();
        }

        public void SetView(List<ShowView> showViews, string title, string searchString)
        {
            ShowListBox.ItemsSource = showViews;
            this.SearchTextBox.Text = searchString;
            this.Title = title;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            ShowView current = (ShowView)ShowListBox.SelectedItem;
            MessageBoxResult r = MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButton.OKCancel);
            if (r == MessageBoxResult.OK)
            {
                RaiseSelectMovieWindowEvent(this, new SelectMovieEventArgs(current.Id));
                this.Hide();
            }
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseSelectMovieWindowEvent(this, new SelectMovieEventArgs(null));
            this.Hide();
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            ShowView current = (ShowView)ShowListBox.SelectedItem;
            movieDetailsWindow.GetMovie(current.Id);
            movieDetailsWindow.ShowDialog();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchTextBox.Text;
            List<ShowView> possibleShows = await movieMatcher.GetPossibleMoviesForFile(searchText);
            SetView(possibleShows, this.Title, searchText);
        }
    }
}
