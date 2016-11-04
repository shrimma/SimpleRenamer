using SimpleRenamer.EventArguments;
using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleRenamer.Views
{
    /// <summary>
    /// Interaction logic for SelectShowWpfForm.xaml
    /// </summary>
    public partial class SelectShowWindow
    {
        public event EventHandler<SelectShowEventArgs> RaiseSelectShowWindowEvent;

        private ILogger logger;
        private ITVShowMatcher showMatcher;
        private IMovieMatcher movieMatcher;
        private ShowDetailsWindow showDetailsWindow;
        private MovieDetailsWindow movieDetailsWindow;
        private FileType currentFileType;

        public SelectShowWindow(ILogger log, ITVShowMatcher showMatch, IMovieMatcher movieMatch, ShowDetailsWindow showDetails, MovieDetailsWindow movieDetails)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            if (showMatch == null)
            {
                throw new ArgumentNullException(nameof(showMatch));
            }
            if (movieMatch == null)
            {
                throw new ArgumentNullException(nameof(movieMatch));
            }
            if (showDetails == null)
            {
                throw new ArgumentNullException(nameof(showDetails));
            }
            if (movieDetails == null)
            {
                throw new ArgumentNullException(nameof(movieDetails));
            }

            logger = log;
            showMatcher = showMatch;
            movieMatcher = movieMatch;
            showDetailsWindow = showDetails;
            movieDetailsWindow = movieDetails;

            InitializeComponent();
            this.ShowListBox.SizeChanged += ListView_SizeChanged;
            this.ShowListBox.Loaded += ListView_Loaded;
            this.Closing += SelectShowWindow_Closing;
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

        private void EnableUi()
        {
            SelectButton.IsEnabled = true;
            ViewButton.IsEnabled = true;
            ShowListBox.Visibility = Visibility.Visible;
        }

        private void DisableUi()
        {
            SelectButton.IsEnabled = false;
            ViewButton.IsEnabled = false;
            ShowListBox.Visibility = Visibility.Hidden;
        }

        void SelectShowWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RaiseSelectShowWindowEvent(this, new SelectShowEventArgs(null, currentFileType));
            e.Cancel = true;
            //clear the item list
            this.ShowListBox.ItemsSource = null;
            this.Hide();
        }

        public async Task SearchForMatches(string title, string searchString, FileType fileType)
        {
            //set the initial UI
            DisableUi();
            this.SearchTextBox.Text = searchString;
            this.Title = title;
            currentFileType = fileType;

            //grab possible matches
            List<ShowView> possibleMatches = await GetMatches(searchString, fileType);

            //if we have matches then enable UI elements
            if (possibleMatches != null && possibleMatches.Count > 0)
            {
                ShowListBox.ItemsSource = possibleMatches;
                EnableUi();
            }
            else
            {
                //even if no matches we show the listbox to hide the progressspinner
                ShowListBox.Visibility = Visibility.Visible;
            }
            SearchTextBox.Focus();
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            ShowView current = (ShowView)ShowListBox.SelectedItem;
            if (current != null)
            {
                ConfirmationFlyout.IsOpen = true;
            }
        }

        private void OkFlyoutButton_Click(object sender, RoutedEventArgs e)
        {
            this.ConfirmationFlyout.IsOpen = false;
            ShowView current = (ShowView)ShowListBox.SelectedItem;
            RaiseSelectShowWindowEvent(this, new SelectShowEventArgs(current.Id, currentFileType));
            //clear the item list
            this.ShowListBox.ItemsSource = null;
            this.Hide();
        }

        private void CancelFlyoutButton_Click(object sender, RoutedEventArgs e)
        {
            this.ConfirmationFlyout.IsOpen = false;
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseSelectShowWindowEvent(this, new SelectShowEventArgs(null, currentFileType));
            //clear the item list
            this.ShowListBox.ItemsSource = null;
            this.Hide();
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            ShowView current = (ShowView)ShowListBox.SelectedItem;
            if (current != null)
            {
                if (currentFileType == FileType.TvShow)
                {
                    showDetailsWindow.GetSeriesInfo(current.Id);
                    showDetailsWindow.ShowDialog();
                }
                else if (currentFileType == FileType.Movie)
                {
                    movieDetailsWindow.GetMovieInfo(current.Id);
                    movieDetailsWindow.ShowDialog();
                }
            }
        }

        private async Task<List<ShowView>> GetMatches(string searchText, FileType fileType)
        {
            List<ShowView> possibleMatches;
            if (currentFileType == FileType.TvShow)
            {
                possibleMatches = await showMatcher.GetPossibleShowsForEpisode(searchText);
            }
            else
            {
                possibleMatches = await movieMatcher.GetPossibleMoviesForFile(searchText);
            }

            return possibleMatches;
        }

        private async void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            string searchText = e.Parameter.ToString();
            await SearchForMatches(this.Title, searchText, currentFileType);
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
