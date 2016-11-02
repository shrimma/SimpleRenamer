using SimpleRenamer.EventArguments;
using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Threading;
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
        }

        private void DisableUi()
        {
            SelectButton.IsEnabled = false;
            ViewButton.IsEnabled = false;
        }

        void SelectShowWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RaiseSelectShowWindowEvent(this, new SelectShowEventArgs(null, currentFileType));
            e.Cancel = true;
            //clear the item list
            this.ShowListBox.ItemsSource = null;
            this.Hide();
        }

        public void SetView(List<ShowView> showViews, string title, string searchString, FileType fileType)
        {
            currentFileType = fileType;
            if (showViews != null && showViews.Count > 0)
            {
                ShowListBox.ItemsSource = showViews;
                EnableUi();
            }
            else
            {
                DisableUi();
            }

            this.SearchTextBox.Text = searchString;
            this.Title = title;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            ShowView current = (ShowView)ShowListBox.SelectedItem;
            if (current != null)
            {
                MessageBoxResult r = MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButton.OKCancel);
                if (r == MessageBoxResult.OK)
                {
                    RaiseSelectShowWindowEvent(this, new SelectShowEventArgs(current.Id, currentFileType));
                    //clear the item list
                    this.ShowListBox.ItemsSource = null;
                    this.Hide();
                }
            }
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

        private async void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            string searchText = e.Parameter.ToString();
            List<ShowView> possibleMatches;
            if (currentFileType == FileType.TvShow)
            {
                possibleMatches = await showMatcher.GetPossibleShowsForEpisode(searchText, cts.Token);
            }
            else
            {
                possibleMatches = await movieMatcher.GetPossibleMoviesForFile(searchText, cts.Token);
            }
            SetView(possibleMatches, this.Title, searchText, currentFileType);
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
