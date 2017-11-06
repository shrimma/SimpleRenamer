using MahApps.Metro;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.EventArguments;
using Sarjee.SimpleRenamer.ThemeManagerHelper;
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
        private Settings originalSettings;
        private ObservableCollection<string> watchFolders;
        private ObservableCollection<string> validExtensions;
        private IConfigurationManager configurationManager;
        private IHelper helper;
        private AddExtensionsWindow addExtensionsWindow;
        private RegexExpressionsWindow regexExpressionsWindow;
        private (AppTheme appTheme, Accent accent) currentTheme;
        private List<AccentItem> accentItems;

        public SettingsWindow(IConfigurationManager configManager, IHelper help, AddExtensionsWindow extWindow, RegexExpressionsWindow expWindow)
        {
            //init our interfaces
            addExtensionsWindow = extWindow ?? throw new ArgumentNullException(nameof(extWindow));
            regexExpressionsWindow = expWindow ?? throw new ArgumentNullException(nameof(expWindow));
            configurationManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
            helper = help ?? throw new ArgumentNullException(nameof(help));

            InitializeComponent();

            accentItems = new List<AccentItem>();
            var mahAppsAccents = ThemeManager.Accents;
            foreach (var accent in mahAppsAccents)
            {
                accentItems.Add(new AccentItem(accent.Name, accent.Resources["AccentBaseColor"].ToString(), accent));
            }
            ChangeThemeCombo.ItemsSource = accentItems;

            //create new event handler for extensions window
            addExtensionsWindow.RaiseCustomEvent += new EventHandler<ExtensionEventArgs>(ExtensionWindowClosedEvent);

            //grab settings and display
            SetupView();

            this.Closing += Window_Closing;
        }


        private void SetupView()
        {
            //grab the current theme            
            currentTheme = ThemeManager.DetectAppStyle(System.Windows.Application.Current).ToValueTuple<AppTheme, Accent>();
            AccentItem currentAccentItem = accentItems.FirstOrDefault(x => x.AccentName.Equals(currentTheme.accent.Name));
            ChangeThemeCombo.SelectedItem = currentAccentItem;
            //grab the current settings from the factory and populate our UI
            originalSettings = new Settings()
            {
                CopyFiles = configurationManager.Settings.CopyFiles,
                DestinationFolderMovie = configurationManager.Settings.DestinationFolderMovie,
                DestinationFolderTV = configurationManager.Settings.DestinationFolderTV,
                NewFileNameFormat = configurationManager.Settings.NewFileNameFormat,
                RenameFiles = configurationManager.Settings.RenameFiles,
                SubDirectories = configurationManager.Settings.SubDirectories,
                ValidExtensions = configurationManager.Settings.ValidExtensions,
                WatchFolders = configurationManager.Settings.WatchFolders
            };
            this.DataContext = configurationManager.Settings;
            watchFolders = new ObservableCollection<string>(configurationManager.Settings.WatchFolders);
            WatchListBox.ItemsSource = watchFolders;
            validExtensions = new ObservableCollection<string>(configurationManager.Settings.ValidExtensions);
            ExtensionsListBox.ItemsSource = validExtensions;
        }

        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
            {
                //stop the window actually closing
                e.Cancel = true;

                //check if settings have been changed without saving
                var currentExtensions = new List<string>(validExtensions);
                if (helper.AreListsEqual(configurationManager.Settings.ValidExtensions, currentExtensions) == false)
                {
                    configurationManager.Settings.ValidExtensions = currentExtensions;
                }
                var currentWatchFolders = new List<string>(watchFolders);
                if (helper.AreListsEqual(configurationManager.Settings.WatchFolders, currentWatchFolders) == false)
                {
                    configurationManager.Settings.WatchFolders = currentWatchFolders;
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
            if (configurationManager.Settings.CopyFiles != originalSettings.CopyFiles)
            {
                return true;
            }
            if (configurationManager.Settings.DestinationFolderMovie != originalSettings.DestinationFolderMovie)
            {
                return true;
            }
            if (configurationManager.Settings.DestinationFolderTV != originalSettings.DestinationFolderTV)
            {
                return true;
            }
            if (configurationManager.Settings.NewFileNameFormat != originalSettings.NewFileNameFormat)
            {
                return true;
            }
            if (configurationManager.Settings.RenameFiles != originalSettings.RenameFiles)
            {
                return true;
            }
            if (configurationManager.Settings.SubDirectories != originalSettings.SubDirectories)
            {
                return true;
            }
            if (!helper.AreListsEqual(configurationManager.Settings.ValidExtensions, originalSettings.ValidExtensions))
            {
                return true;
            }
            if (!helper.AreListsEqual(configurationManager.Settings.WatchFolders, originalSettings.WatchFolders))
            {
                return true;
            }
            return false;
        }

        private void OkFlyoutButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmationFlyout.IsOpen = false;
            if (configurationManager.Settings.CopyFiles != originalSettings.CopyFiles)
            {
                configurationManager.Settings.CopyFiles = originalSettings.CopyFiles;
            }
            if (configurationManager.Settings.DestinationFolderMovie != originalSettings.DestinationFolderMovie)
            {
                configurationManager.Settings.DestinationFolderMovie = originalSettings.DestinationFolderMovie;
            }
            if (configurationManager.Settings.DestinationFolderTV != originalSettings.DestinationFolderTV)
            {
                configurationManager.Settings.DestinationFolderTV = originalSettings.DestinationFolderTV;
            }
            if (configurationManager.Settings.NewFileNameFormat != originalSettings.NewFileNameFormat)
            {
                configurationManager.Settings.NewFileNameFormat = originalSettings.NewFileNameFormat;
            }
            if (configurationManager.Settings.RenameFiles != originalSettings.RenameFiles)
            {
                configurationManager.Settings.RenameFiles = originalSettings.RenameFiles;
            }
            if (configurationManager.Settings.SubDirectories != originalSettings.SubDirectories)
            {
                configurationManager.Settings.SubDirectories = originalSettings.SubDirectories;
            }
            if (helper.AreListsEqual(configurationManager.Settings.ValidExtensions, originalSettings.ValidExtensions) == false)
            {
                configurationManager.Settings.ValidExtensions = originalSettings.ValidExtensions;
            }
            if (helper.AreListsEqual(configurationManager.Settings.WatchFolders, originalSettings.WatchFolders) == false)
            {
                configurationManager.Settings.WatchFolders = originalSettings.WatchFolders;
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
            var currentExtensions = new List<string>(validExtensions);
            if (helper.AreListsEqual(configurationManager.Settings.ValidExtensions, currentExtensions) == false)
            {
                configurationManager.Settings.ValidExtensions = currentExtensions;
            }
            var currentWatchFolders = new List<string>(watchFolders);
            if (helper.AreListsEqual(configurationManager.Settings.WatchFolders, currentWatchFolders) == false)
            {
                configurationManager.Settings.WatchFolders = currentWatchFolders;
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
                if (!watchFolders.Contains(path))
                {
                    watchFolders.Add(path);
                }
            }
        }

        private void AddExtensionButton_Click(object sender, RoutedEventArgs e)
        {
            addExtensionsWindow.ShowDialog();
        }

        private void ExtensionWindowClosedEvent(object sender, ExtensionEventArgs e)
        {
            if (helper.IsFileExtensionValid(e.Extension))
            {
                if (!watchFolders.Contains(e.Extension))
                {
                    validExtensions.Add(e.Extension);
                }
            }
        }

        private void BrowseDestinationButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                configurationManager.Settings.DestinationFolderTV = Path.GetFullPath(dialog.SelectedPath);
            }
        }

        private void BrowseDestinationMovieButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                configurationManager.Settings.DestinationFolderMovie = Path.GetFullPath(dialog.SelectedPath);
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
            regexExpressionsWindow.ShowDialog();
        }

        private void ChangeThemeCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                AccentItem selectedColor = (AccentItem)ChangeThemeCombo.SelectedItem;
                ThemeManager.ChangeAppStyle(System.Windows.Application.Current, selectedColor.Accent, currentTheme.appTheme);
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
