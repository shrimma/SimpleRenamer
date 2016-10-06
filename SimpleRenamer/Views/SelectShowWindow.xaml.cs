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
    public partial class SelectShowWindow : Window
    {
        public event EventHandler<SelectShowEventArgs> RaiseSelectShowWindowEvent;

        private ILogger logger;
        private ITVShowMatcher showMatcher;
        private ShowDetailsWindow showDetailsWindow;

        public SelectShowWindow(ILogger log, ITVShowMatcher showMatch, ShowDetailsWindow showDetails)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            if (showMatch == null)
            {
                throw new ArgumentNullException(nameof(showMatch));
            }
            if (showDetails == null)
            {
                throw new ArgumentNullException(nameof(showDetails));
            }

            logger = log;
            showMatcher = showMatch;
            showDetailsWindow = showDetails;

            InitializeComponent();
            this.Closing += SelectShowWindow_Closing;
        }

        void SelectShowWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RaiseSelectShowWindowEvent(this, new SelectShowEventArgs(null));
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
                RaiseSelectShowWindowEvent(this, new SelectShowEventArgs(current.Id));
                this.Hide();
            }
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseSelectShowWindowEvent(this, new SelectShowEventArgs(null));
            this.Hide();
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            ShowView current = (ShowView)ShowListBox.SelectedItem;
            showDetailsWindow.GetSeries(current.Id);
            showDetailsWindow.ShowDialog();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchTextBox.Text;
            List<ShowView> possibleShows = await showMatcher.GetPossibleShowsForEpisode(searchText);
            SetView(possibleShows, this.Title, searchText);
        }
    }
}
