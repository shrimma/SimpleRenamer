using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace SimpleRenamer
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public Settings currentSettings;
        public Settings oldSettings;
        public ObservableCollection<string> watchFolders;
        public ObservableCollection<string> validExtensions;
        private ISettingsFactory settingsFactory;

        public SettingsWindow(ISettingsFactory settingsFact)
        {
            InitializeComponent();
            settingsFactory = settingsFact;
            currentSettings = settingsFactory.GetSettings();
            oldSettings = settingsFactory.GetSettings();
            this.DataContext = currentSettings;
            watchFolders = new ObservableCollection<string>(currentSettings.WatchFolders);
            WatchListBox.ItemsSource = watchFolders;
            validExtensions = new ObservableCollection<string>(currentSettings.ValidExtensions);
            ExtensionsListBox.ItemsSource = validExtensions;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            settingsFactory.SaveSettings(currentSettings);
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            //SaveSettings(oldSettings);
            this.Close();
        }

        private void AddFolderButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string path = Path.GetFullPath(dialog.SelectedPath);
                if (!watchFolders.Contains(path))
                {
                    watchFolders.Add(path);
                }
            }
        }

        private void AddExtensionButton_Click(object sender, RoutedEventArgs e)
        {
            AddExtensions form = new AddExtensions();
            form.RaiseCustomEvent += new EventHandler<ExtensionEventArgs>(WindowClosedEvent1);
            form.ShowDialog();
        }

        private void WindowClosedEvent1(object sender, ExtensionEventArgs e)
        {
            if (IsFileExtensionValid(e.Extension))
            {
                if (!watchFolders.Contains(e.Extension))
                {
                    validExtensions.Add(e.Extension);
                }
            }
        }

        private bool IsFileExtensionValid(string fExt)
        {
            bool answer = true;
            if (!String.IsNullOrWhiteSpace(fExt) && fExt.Length > 1 && fExt[0] == '.')
            {
                char[] invalidFileChars = Path.GetInvalidFileNameChars();
                foreach (char c in invalidFileChars)
                {
                    if (fExt.Contains(c.ToString()))
                    {
                        answer = false;
                        break;
                    }
                }
            }
            else
            {
                answer = false;
            }
            return answer;
        }

        private void BrowseDestinationButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                currentSettings.DestinationFolder = Path.GetFullPath(dialog.SelectedPath);
            }
        }

        private void DeleteFolderButton_Click(object sender, RoutedEventArgs e)
        {
            watchFolders.Remove((string)WatchListBox.SelectedItem);
        }

        private void DeleteExtensionButton_Click(object sender, RoutedEventArgs e)
        {
            validExtensions.Remove((string)WatchListBox.SelectedItem);
        }

        private void RegexExpressionButton_Click(object sender, RoutedEventArgs e)
        {
            RegexExpressions form = new RegexExpressions();
            form.ShowDialog();
        }
    }
}
