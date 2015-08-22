using SimpleRenamer.Framework;
using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();
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

        public Settings settings;

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            cts = new CancellationTokenSource();
            RunButton.IsEnabled = false;
            SettingsButton.IsEnabled = false;
            CancelButton.IsEnabled = true;
            try
            {
                LogTextBox.Text = string.Format("{0} - Starting", DateTime.Now.ToShortTimeString());
                SetSettings();
                List<string> videoFiles = FileWatcher.SearchTheseFoldersAsync(settings, cts.Token);
                List<TVEpisode> episodes = await MatchTVShows(videoFiles, settings, cts.Token);
                await MoveTVShows(episodes, settings, cts.Token);
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

        public async Task<List<TVEpisode>> MatchTVShows(List<string> videoFiles, Settings settings, CancellationToken ct)
        {
            List<TVEpisode> episodes = new List<TVEpisode>();
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
                    WriteNewLineToTextBox(string.Format("{0} - S{1}E{2} - {3}", tempEp.ShowName, tempEp.Season, tempEp.Episode, tempEp.EpisodeName));
                    episodes.Add(tempEp);
                }
            }
            return episodes;
        }

        public async Task<bool> MoveTVShows(List<TVEpisode> episodes, Settings settings, CancellationToken ct)
        {
            List<Task<FileMoveResult>> tasks = new List<Task<FileMoveResult>>();
            foreach (TVEpisode ep in episodes)
            {
                if (ep.SkippedExactSelection)
                {
                    WriteNewLineToTextBox(string.Format("Skipped {0} as user didn't select exact show match.", ep.FilePath));
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

        }
    }
}
