using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.EventArguments;
using Sarjee.SimpleRenamer.WPF;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sarjee.SimpleRenamer.Views
{
    /// <summary>
    /// Interaction logic for SelectShowWpfForm.xaml
    /// </summary>
    public partial class SelectShowWindow
    {
        public event EventHandler<SelectShowEventArgs> RaiseSelectShowWindowEvent;
        private CancellationTokenSource _cancellationTokenSource;
        private ILogger logger;
        private ITVShowMatcher showMatcher;
        private IMovieMatcher movieMatcher;
        private ShowDetailsWindow showDetailsWindow;
        private MovieDetailsWindow movieDetailsWindow;
        private FileType currentFileType;

        public SelectShowWindow(ILogger log, ITVShowMatcher showMatch, IMovieMatcher movieMatch, ShowDetailsWindow showDetails, MovieDetailsWindow movieDetails)
        {
            logger = log ?? throw new ArgumentNullException(nameof(log));
            showMatcher = showMatch ?? throw new ArgumentNullException(nameof(showMatch));
            movieMatcher = movieMatch ?? throw new ArgumentNullException(nameof(movieMatch));
            showDetailsWindow = showDetails ?? throw new ArgumentNullException(nameof(showDetails));
            movieDetailsWindow = movieDetails ?? throw new ArgumentNullException(nameof(movieDetails));

            InitializeComponent();
            this.ShowListBox.SizeChanged += ListView_SizeChanged;
            this.ShowListBox.Loaded += ListView_Loaded;
            this.Closing += SelectShowWindow_Closing;
        }

        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            WpfHelper.UpdateColumnsWidth(sender as ListView);
        }

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            WpfHelper.UpdateColumnsWidth(sender as ListView);
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
            if (this.Visibility == Visibility.Visible)
            {
                RaiseSelectShowWindowEvent(this, new SelectShowEventArgs(null, currentFileType));
                e.Cancel = true;
                //clear the item list
                this.ShowListBox.ItemsSource = null;
                this.Hide();
            }
        }

        public async Task SearchForMatches(string title, string searchString, FileType fileType)
        {
            //set the initial UI
            _cancellationTokenSource = new CancellationTokenSource();
            DisableUi();
            this.SearchTextBox.Text = searchString;
            this.Title = title;
            currentFileType = fileType;
            ShowListBox.ItemsSource = null;

            //grab possible matches
            List<DetailView> possibleMatches = await GetMatches(searchString, fileType, _cancellationTokenSource.Token);

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
            DetailView current = (DetailView)ShowListBox.SelectedItem;
            if (current != null)
            {
                ConfirmationFlyout.IsOpen = true;
            }
        }

        private void OkFlyoutButton_Click(object sender, RoutedEventArgs e)
        {
            this.ConfirmationFlyout.IsOpen = false;
            DetailView current = (DetailView)ShowListBox.SelectedItem;
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
            _cancellationTokenSource = new CancellationTokenSource();
            DetailView current = (DetailView)ShowListBox.SelectedItem;
            if (current != null)
            {
                if (currentFileType == FileType.TvShow)
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    showDetailsWindow.GetSeriesInfo(current.Id, _cancellationTokenSource.Token);
                    showDetailsWindow.ShowDialog();
                }
                else if (currentFileType == FileType.Movie)
                {
                    movieDetailsWindow.GetMovieInfo(current.Id, _cancellationTokenSource.Token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    movieDetailsWindow.ShowDialog();
                }
            }
        }

        private async Task<List<DetailView>> GetMatches(string searchText, FileType fileType, CancellationToken cancellationToken)
        {
            List<DetailView> possibleMatches = null;
            if (currentFileType == FileType.TvShow)
            {
                possibleMatches = await showMatcher.GetPossibleShowsForEpisodeAsync(searchText, cancellationToken);
            }
            else
            {
                possibleMatches = await movieMatcher.GetPossibleMoviesForFileAsync(searchText, cancellationToken);
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
