﻿using SimpleRenamer.EventArguments;
using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace SimpleRenamer.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private Settings currentSettings;
        private ObservableCollection<string> watchFolders;
        private ObservableCollection<string> validExtensions;
        private ISettingsFactory settingsFactory;
        private IHelper helper;
        private AddExtensionsWindow addExtensionsWindow;
        private RegexExpressionsWindow regexExpressionsWindow;

        public SettingsWindow(ISettingsFactory settingsFact, IHelper help, AddExtensionsWindow extWindow, RegexExpressionsWindow expWindow)
        {
            if (settingsFact == null)
            {
                throw new ArgumentNullException(nameof(settingsFact));
            }
            if (help == null)
            {
                throw new ArgumentNullException(nameof(help));
            }
            if (extWindow == null)
            {
                throw new ArgumentNullException(nameof(extWindow));
            }
            if (expWindow == null)
            {
                throw new ArgumentNullException(nameof(expWindow));
            }

            InitializeComponent();

            //init our interfaces
            addExtensionsWindow = extWindow;
            regexExpressionsWindow = expWindow;
            settingsFactory = settingsFact;
            helper = help;

            //create new event handler for extensions window
            addExtensionsWindow.RaiseCustomEvent += new EventHandler<ExtensionEventArgs>(ExtensionWindowClosedEvent);

            //grab the current settings from the factory and populate our UI
            currentSettings = settingsFactory.GetSettings();
            this.DataContext = currentSettings;
            watchFolders = new ObservableCollection<string>(currentSettings.WatchFolders);
            WatchListBox.ItemsSource = watchFolders;
            validExtensions = new ObservableCollection<string>(currentSettings.ValidExtensions);
            ExtensionsListBox.ItemsSource = validExtensions;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            currentSettings.ValidExtensions = new List<string>(validExtensions);
            currentSettings.WatchFolders = new List<string>(watchFolders);
            settingsFactory.SaveSettings(currentSettings);
            this.Hide();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
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
            regexExpressionsWindow.ShowDialog();
        }
    }
}
