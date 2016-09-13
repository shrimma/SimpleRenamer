using System;
using System.Collections.Generic;
using System.Windows;

namespace SimpleRenamer.Framework
{
    /// <summary>
    /// Interaction logic for SelectShowWpfForm.xaml
    /// </summary>
    public partial class SelectShowWpfForm : Window
    {
        public event EventHandler<CustomEventArgs> RaiseCustomEvent;
        public List<ShowView> ShowViews { get; set; }
        public SelectShowWpfForm()
        {
            InitializeComponent();
            this.Closing += SelectShowWpfForm_Closing;
        }

        void SelectShowWpfForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RaiseCustomEvent(this, new CustomEventArgs(null));
            this.Hide();
        }

        public void SetView()
        {
            ShowListBox.ItemsSource = ShowViews;
        }

        public void SetTitle(string title)
        {
            this.Title = title;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            ShowView current = (ShowView)ShowListBox.SelectedItem;
            MessageBoxResult r = MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButton.OKCancel);
            if (r == MessageBoxResult.OK)
            {
                RaiseCustomEvent(this, new CustomEventArgs(current.Id));
                this.Hide();
            }
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseCustomEvent(this, new CustomEventArgs(null));
            this.Hide();
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            ShowView current = (ShowView)ShowListBox.SelectedItem;
            ShowDetailsForm sdf = new ShowDetailsForm(current.Id);
            sdf.ShowDialog();
        }
    }

    public class ShowView
    {
        public string Id { get; set; }
        public string ShowName { get; set; }
        public string Year { get; set; }
        public string Description { get; set; }
        public ShowView(string id, string showname, string year, string description)
        {
            Id = id;
            ShowName = showname;
            Year = year;
            Description = description;
        }
    }

    public class CustomEventArgs : EventArgs
    {
        public CustomEventArgs(string s)
        {
            id = s;
        }
        private string id;
        public string ID
        {
            get { return id; }
        }
    }
}
