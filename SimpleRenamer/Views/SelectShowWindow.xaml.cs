using SimpleRenamer.EventArguments;
using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SimpleRenamer
{
    /// <summary>
    /// Interaction logic for SelectShowWpfForm.xaml
    /// </summary>
    public partial class SelectShowWindow : Window
    {
        public event EventHandler<SelectShowEventArgs> RaiseSelectShowWindowEvent;

        private ILogger logger;
        private ShowDetailsWindow showDetailsWindow;

        public SelectShowWindow(ILogger log, ShowDetailsWindow showDetails)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            if (showDetails == null)
            {
                throw new ArgumentNullException(nameof(showDetails));
            }

            logger = log;
            showDetailsWindow = showDetails;

            InitializeComponent();
            this.Closing += SelectShowWindow_Closing;
        }

        void SelectShowWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RaiseSelectShowWindowEvent(this, new SelectShowEventArgs(null));
            this.Hide();
        }

        public void SetView(List<ShowView> showViews, string title)
        {
            ShowListBox.ItemsSource = showViews;
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
    }
}
