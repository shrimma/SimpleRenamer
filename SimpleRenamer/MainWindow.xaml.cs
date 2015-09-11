using SimpleRenamer.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleRenamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public CancellationTokenSource cts;
        public Settings settings;
        private ObservableCollection<TVEpisode> scannedEpisodes;

        public MainWindow()
        {
            InitializeComponent();
            scannedEpisodes = new ObservableCollection<TVEpisode>();
            ShowsListBox.ItemsSource = scannedEpisodes;
            this.Closing += MainWindow_Closing;
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!RunButton.IsEnabled)
            {
                e.Cancel = true;
            }
            else
            {
                this.Hide();
            }
        }

        public Settings GetSettings()
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            Settings mySettings = new Settings();
            mySettings.SubDirectories = bool.Parse(configuration.AppSettings.Settings["SubDirectories"].Value);
            mySettings.RenameFiles = bool.Parse(configuration.AppSettings.Settings["RenameFiles"].Value);
            mySettings.CopyFiles = bool.Parse(configuration.AppSettings.Settings["CopyFiles"].Value);
            mySettings.NewFileNameFormat = configuration.AppSettings.Settings["NewFileNameFormat"].Value;
            mySettings.ValidExtensions = new List<string>(configuration.AppSettings.Settings["ValidExtensions"].Value.Split(new char[] { ';' }));
            mySettings.WatchFolders = new List<string>(configuration.AppSettings.Settings["WatchFolders"].Value.Split(new char[] { ';' }));
            mySettings.DestinationFolder = configuration.AppSettings.Settings["DestinationFolder"].Value;

            return mySettings;
        }

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            cts = new CancellationTokenSource();
            RunButton.IsEnabled = false;
            SettingsButton.IsEnabled = false;
            ActionButton.IsEnabled = false;
            CancelButton.IsEnabled = true;
            try
            {
                settings = GetSettings();
                LogTextBox.Text = string.Format("{0} - Starting", DateTime.Now.ToShortTimeString());
                List<string> videoFiles = FileWatcher.SearchTheseFoldersAsync(settings, cts.Token);
                WriteNewLineToTextBox(string.Format("Found {0} files within the watch folders", videoFiles.Count));
                await MatchTVShows(videoFiles, settings, cts.Token);
                WriteNewLineToTextBox(string.Format("Matched {0} files", scannedEpisodes.Count));
            }
            catch (OperationCanceledException)
            {
                WriteNewLineToTextBox("User canceled.");
            }
            finally
            {
                WriteNewLineToTextBox("Finished");
                RunButton.IsEnabled = true;
                SettingsButton.IsEnabled = true;
                CancelButton.IsEnabled = false;
                if (scannedEpisodes.Count > 0)
                {
                    ActionButton.IsEnabled = true;
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelButton.IsEnabled = false;
            if (cts != null)
            {
                cts.Cancel();
            }
        }

        private void WriteNewLineToTextBox(string text)
        {
            LogTextBox.Text += string.Format("\n{0} - {1}", DateTime.Now.ToShortTimeString(), text);
        }

        /// <summary>
        /// Set temp values for the console test
        /// </summary>
        public void SetSettings()
        {
            settings = new Settings();
            settings.SubDirectories = true;
            settings.RenameFiles = true;
            settings.CopyFiles = true;
            settings.NewFileNameFormat = "{ShowName} - S{Season}E{Episode} - {EpisodeName}}";
            settings.ValidExtensions = new List<string>();
            settings.ValidExtensions.Add(".avi");
            settings.ValidExtensions.Add(".mkv");
            settings.WatchFolders = new List<string>();
            settings.WatchFolders.Add(@"C:\Temp\Simple\");
            settings.WatchFolders.Add(@"C:\Temp\Simple2\");
            settings.DestinationFolder = @"C:\Temp\Root\";
        }

        public async Task MatchTVShows(List<string> videoFiles, Settings settings, CancellationToken ct)
        {
            scannedEpisodes.Clear();
            List<Task<TVEpisode>> tasks = new List<Task<TVEpisode>>();
            //spin up a task for each file
            foreach (string fileName in videoFiles)
            {
                tasks.Add(FileMatcher.SearchMeAsync(fileName));
            }
            //as each task completes
            foreach (var t in tasks.InCompletionOrder())
            {
                ct.ThrowIfCancellationRequested();
                TVEpisode tempEp = await t;
                if (tempEp != null)
                {
                    //scrape the episode name and incorporate this in the filename (if setting allows)
                    if (settings.RenameFiles)
                    {
                        tempEp = await TVShowMatcher.ScrapeDetailsAsync(tempEp, settings);
                        ct.ThrowIfCancellationRequested();
                    }
                    else
                    {
                        tempEp.NewFileName = Path.GetFileNameWithoutExtension(tempEp.FilePath);
                    }
                    WriteNewLineToTextBox(string.Format("Matched: {0} - S{1}E{2} - {3}", tempEp.ShowName, tempEp.Season, tempEp.Episode, tempEp.EpisodeName));
                    //only add the file if it needs renaming/moving
                    int season;
                    int.TryParse(tempEp.Season, out season);
                    string destinationDirectory = Path.Combine(settings.DestinationFolder, tempEp.ShowName, string.Format("Season {0}", season));
                    string destinationFilePath = Path.Combine(destinationDirectory, tempEp.NewFileName + Path.GetExtension(tempEp.FilePath));
                    if (!tempEp.FilePath.Equals(destinationFilePath))
                    {
                        WriteNewLineToTextBox(string.Format("Will move with name {0}", tempEp.NewFileName));
                        scannedEpisodes.Add(tempEp);
                    }
                    else
                    {
                        WriteNewLineToTextBox(string.Format("File is already in good location {0}", tempEp.FilePath));
                    }
                }
            }
        }

        public async Task<bool> MoveTVShows(Settings settings, CancellationToken ct)
        {
            List<Task<FileMoveResult>> tasks = new List<Task<FileMoveResult>>();
            foreach (TVEpisode ep in scannedEpisodes)
            {
                if (!ep.ActionThis)
                {
                    WriteNewLineToTextBox(string.Format("Skipped {0} as user chose not to action.", ep.FilePath));
                }
                else
                {
                    tasks.Add(FileMover.MoveFileAsync(ep, settings));
                }
            }
            foreach (var t in tasks.InCompletionOrder())
            {
                ct.ThrowIfCancellationRequested();
                FileMoveResult r = await t;
                if (r.Success)
                {
                    WriteNewLineToTextBox(string.Format("Successfully {2} {0} to {1}", r.Episode.FilePath, r.Episode.NewFileName, settings.CopyFiles ? "Copied" : "Moved"));
                    scannedEpisodes.Remove(r.Episode);
                }
                else
                {
                    WriteNewLineToTextBox(string.Format("Failed to {1} {0}", r.Episode.FilePath, settings.CopyFiles ? "Copy" : "Move"));
                }
            }

            return true;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            //show the settings window
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        private async void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            cts = new CancellationTokenSource();
            RunButton.IsEnabled = false;
            SettingsButton.IsEnabled = false;
            ActionButton.IsEnabled = false;
            CancelButton.IsEnabled = true;
            try
            {
                settings = GetSettings();
                LogTextBox.Text = string.Format("{0} - Starting", DateTime.Now.ToShortTimeString());
                await MoveTVShows(settings, cts.Token);
            }
            catch (OperationCanceledException)
            {
                WriteNewLineToTextBox("User canceled.");
            }
            finally
            {
                WriteNewLineToTextBox("Finished");
                RunButton.IsEnabled = true;
                SettingsButton.IsEnabled = true;
                CancelButton.IsEnabled = false;
            }
        }
    }
}
