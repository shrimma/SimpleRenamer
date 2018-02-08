using MahApps.Metro;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.EventArguments;
using Sarjee.SimpleRenamer.WPF.ThemeManagerHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace Sarjee.SimpleRenamer.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow
    {
        private Settings _originalSettings;
        private ObservableCollection<string> _watchFolders;
        private ObservableCollection<string> _validExtensions;
        private IConfigurationManager _configurationManager;
        private IHelper _helper;
        private AddExtensionsWindow _addExtensionsWindow;
        private RegexExpressionsWindow _regexExpressionsWindow;
        private List<AccentItem> _accentItems;

        public AppTheme CurrentAppTheme { get; set; }
        public AccentItem CurrentAccent { get; set; }

        public SettingsWindow(IConfigurationManager configurationManager, IHelper helper, AddExtensionsWindow addExtensionsWindow, RegexExpressionsWindow regexExpressionsWindow)
        {
            //init our interfaces
            _addExtensionsWindow = addExtensionsWindow ?? throw new ArgumentNullException(nameof(addExtensionsWindow));
            _regexExpressionsWindow = regexExpressionsWindow ?? throw new ArgumentNullException(nameof(regexExpressionsWindow));
            _configurationManager = configurationManager ?? throw new ArgumentNullException(nameof(configurationManager));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));

            InitializeComponent();
            //setup theme combo box            
            _accentItems = new List<AccentItem>();
            IEnumerable<Accent> mahAppsAccents = ThemeManager.Accents;
            foreach (var accent in mahAppsAccents)
            {
                _accentItems.Add(new AccentItem(accent.Name, accent.Resources["AccentBaseColor"].ToString(), accent));
            }
            ChangeThemeCombo.ItemsSource = _accentItems;

            //create new event handler for extensions window
            _addExtensionsWindow.RaiseCustomEvent += new EventHandler<ExtensionEventArgs>(ExtensionWindowClosedEvent);

            this.Closing += Window_Closing;
        }


        public void SetupView()
        {
            //set combo box to current theme
            AccentItem currentAccentItem = _accentItems.FirstOrDefault(x => x.AccentName.Equals(CurrentAccent.AccentName));
            ChangeThemeCombo.SelectedItem = currentAccentItem;
            //grab the current settings from the factory and populate our UI
            _originalSettings = new Settings()
            {
                CopyFiles = _configurationManager.Settings.CopyFiles,
                DestinationFolderMovie = _configurationManager.Settings.DestinationFolderMovie,
                DestinationFolderTV = _configurationManager.Settings.DestinationFolderTV,
                NewFileNameFormat = _configurationManager.Settings.NewFileNameFormat,
                RenameFiles = _configurationManager.Settings.RenameFiles,
                SubDirectories = _configurationManager.Settings.SubDirectories,
                ValidExtensions = _configurationManager.Settings.ValidExtensions,
                WatchFolders = _configurationManager.Settings.WatchFolders
            };
            this.DataContext = _configurationManager.Settings;
            _watchFolders = new ObservableCollection<string>(_configurationManager.Settings.WatchFolders);
            WatchListBox.ItemsSource = _watchFolders;
            _validExtensions = new ObservableCollection<string>(_configurationManager.Settings.ValidExtensions);
            ExtensionsListBox.ItemsSource = _validExtensions;
        }

        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
            {
                //stop the window actually closing
                e.Cancel = true;

                //check if settings have been changed without saving
                var currentExtensions = new List<string>(_validExtensions);
                if (_helper.AreListsEqual(_configurationManager.Settings.ValidExtensions, currentExtensions) == false)
                {
                    _configurationManager.Settings.ValidExtensions = currentExtensions;
                }
                var currentWatchFolders = new List<string>(_watchFolders);
                if (_helper.AreListsEqual(_configurationManager.Settings.WatchFolders, currentWatchFolders) == false)
                {
                    _configurationManager.Settings.WatchFolders = currentWatchFolders;
                }
                if (HaveSettingsChanged() == true)
                {
                    //if settings have been changed and not saved then prompt user
                    ConfirmationFlyout.IsOpen = true;
                }
                else
                {
                    SetupView();
                    this.Hide();
                }
            }
        }

        private bool HaveSettingsChanged()
        {
            if (_configurationManager.Settings.CopyFiles != _originalSettings.CopyFiles)
            {
                return true;
            }
            if (_configurationManager.Settings.DestinationFolderMovie != _originalSettings.DestinationFolderMovie)
            {
                return true;
            }
            if (_configurationManager.Settings.DestinationFolderTV != _originalSettings.DestinationFolderTV)
            {
                return true;
            }
            if (_configurationManager.Settings.NewFileNameFormat != _originalSettings.NewFileNameFormat)
            {
                return true;
            }
            if (_configurationManager.Settings.RenameFiles != _originalSettings.RenameFiles)
            {
                return true;
            }
            if (_configurationManager.Settings.SubDirectories != _originalSettings.SubDirectories)
            {
                return true;
            }
            if (!_helper.AreListsEqual(_configurationManager.Settings.ValidExtensions, _originalSettings.ValidExtensions))
            {
                return true;
            }
            if (!_helper.AreListsEqual(_configurationManager.Settings.WatchFolders, _originalSettings.WatchFolders))
            {
                return true;
            }
            return false;
        }

        private void OkFlyoutButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmationFlyout.IsOpen = false;
            if (_configurationManager.Settings.CopyFiles != _originalSettings.CopyFiles)
            {
                _configurationManager.Settings.CopyFiles = _originalSettings.CopyFiles;
            }
            if (_configurationManager.Settings.DestinationFolderMovie != _originalSettings.DestinationFolderMovie)
            {
                _configurationManager.Settings.DestinationFolderMovie = _originalSettings.DestinationFolderMovie;
            }
            if (_configurationManager.Settings.DestinationFolderTV != _originalSettings.DestinationFolderTV)
            {
                _configurationManager.Settings.DestinationFolderTV = _originalSettings.DestinationFolderTV;
            }
            if (_configurationManager.Settings.NewFileNameFormat != _originalSettings.NewFileNameFormat)
            {
                _configurationManager.Settings.NewFileNameFormat = _originalSettings.NewFileNameFormat;
            }
            if (_configurationManager.Settings.RenameFiles != _originalSettings.RenameFiles)
            {
                _configurationManager.Settings.RenameFiles = _originalSettings.RenameFiles;
            }
            if (_configurationManager.Settings.SubDirectories != _originalSettings.SubDirectories)
            {
                _configurationManager.Settings.SubDirectories = _originalSettings.SubDirectories;
            }
            if (_helper.AreListsEqual(_configurationManager.Settings.ValidExtensions, _originalSettings.ValidExtensions) == false)
            {
                _configurationManager.Settings.ValidExtensions = _originalSettings.ValidExtensions;
            }
            if (_helper.AreListsEqual(_configurationManager.Settings.WatchFolders, _originalSettings.WatchFolders) == false)
            {
                _configurationManager.Settings.WatchFolders = _originalSettings.WatchFolders;
            }
            SetupView();
            this.Hide();
        }

        private void CancelFlyoutButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmationFlyout.IsOpen = false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //check if settings have been changed without saving
            var currentExtensions = new List<string>(_validExtensions);
            if (_helper.AreListsEqual(_configurationManager.Settings.ValidExtensions, currentExtensions) == false)
            {
                _configurationManager.Settings.ValidExtensions = currentExtensions;
            }
            var currentWatchFolders = new List<string>(_watchFolders);
            if (_helper.AreListsEqual(_configurationManager.Settings.WatchFolders, currentWatchFolders) == false)
            {
                _configurationManager.Settings.WatchFolders = currentWatchFolders;
            }
            SetupView();
            this.Hide();
        }

        private void AddFolderButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string path = Path.GetFullPath(dialog.SelectedPath);
                if (!_watchFolders.Contains(path))
                {
                    _watchFolders.Add(path);
                }
            }
        }

        private void AddExtensionButton_Click(object sender, RoutedEventArgs e)
        {
            _addExtensionsWindow.ShowDialog();
        }

        private void ExtensionWindowClosedEvent(object sender, ExtensionEventArgs e)
        {
            if (_helper.IsFileExtensionValid(e.Extension))
            {
                if (!_watchFolders.Contains(e.Extension))
                {
                    _validExtensions.Add(e.Extension);
                }
            }
        }

        private void BrowseDestinationButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                _configurationManager.Settings.DestinationFolderTV = Path.GetFullPath(dialog.SelectedPath);
            }
        }

        private void BrowseDestinationMovieButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                _configurationManager.Settings.DestinationFolderMovie = Path.GetFullPath(dialog.SelectedPath);
            }
        }

        private void DeleteFolderButton_Click(object sender, RoutedEventArgs e)
        {
            _watchFolders.Remove((string)WatchListBox.SelectedItem);
        }

        private void DeleteExtensionButton_Click(object sender, RoutedEventArgs e)
        {
            _validExtensions.Remove((string)ExtensionsListBox.SelectedItem);
        }

        private void RegexExpressionButton_Click(object sender, RoutedEventArgs e)
        {
            _regexExpressionsWindow.ShowDialog();
        }

        private void ChangeThemeCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                AccentItem selectedColor = (AccentItem)ChangeThemeCombo.SelectedItem;
                ThemeManager.ChangeAppStyle(System.Windows.Application.Current, selectedColor.Accent, CurrentAppTheme);
                //set the current theme to what was selected
                CurrentAccent = selectedColor;
            }
            catch (Exception)
            {
                //TODO log this!
            }
        }

        private void WatchListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string value = (string)WatchListBox.SelectedItem;
            if (!string.IsNullOrWhiteSpace(value))
            {
                this.DeleteFolderButton.IsEnabled = true;
            }
            else
            {
                this.DeleteFolderButton.IsEnabled = false;
            }
        }

        private void ExtensionsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string value = (string)ExtensionsListBox.SelectedItem;
            if (!string.IsNullOrWhiteSpace(value))
            {
                this.DeleteExtensionButton.IsEnabled = true;
            }
            else
            {
                this.DeleteExtensionButton.IsEnabled = false;
            }
        }
    }
}
