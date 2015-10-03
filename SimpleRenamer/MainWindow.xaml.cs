using SimpleRenamer.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
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
            MatchShowButton.IsEnabled = false;
            IgnoreShowButton.IsEnabled = false;
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
            ShowNameMapping showNameMapping = TVShowMatcher.ReadMappingFile();
            ShowNameMapping originalMapping = TVShowMatcher.ReadMappingFile();
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
                TVEpisodeScrape scrapeResult = null;
                if (tempEp != null)
                {
                    //scrape the episode name and incorporate this in the filename (if setting allows)
                    if (settings.RenameFiles)
                    {
                        scrapeResult = await TVShowMatcher.ScrapeDetailsAsync(tempEp, settings, showNameMapping);
                        tempEp = scrapeResult.tvep;
                        if (scrapeResult.series != null)
                        {
                            Mapping map = new Mapping(scrapeResult.tvep.ShowName, scrapeResult.series.Title, scrapeResult.series.Id.ToString());
                            if (!showNameMapping.Mappings.Any(x => x.TVDBShowID.Equals(map.TVDBShowID)))
                            {
                                showNameMapping.Mappings.Add(map);
                            }
                        }
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
            if (showNameMapping.Mappings != originalMapping.Mappings || showNameMapping.Mappings.Count != originalMapping.Mappings.Count)
            {
                TVShowMatcher.WriteMappingFile(showNameMapping);
            }
        }

        public class ShowSeason
        {
            public string Show { get; set; }
            public string Season { get; set; }

            public ShowSeason(string show, string season)
            {
                Show = show;
                Season = season;
            }
        }

        public async Task<List<FileMoveResult>> ProcessTVShows(Settings settings, CancellationToken ct)
        {
            List<Task<FileMoveResult>> tasks = new List<Task<FileMoveResult>>();
            List<ShowSeason> uniqueShowSeasons = new List<ShowSeason>();
            List<FileMoveResult> ProcessFiles = new List<FileMoveResult>();
            FileMoveProgressBar.Value = 0;
            FileMoveProgressBar.Maximum = scannedEpisodes.Count;
            BackgroundQueue bgQueue = new BackgroundQueue();
            foreach (TVEpisode ep in scannedEpisodes)
            {
                if (!ep.ActionThis)
                {
                    WriteNewLineToTextBox(string.Format("Skipped {0} as user chose not to action.", ep.FilePath));
                    FileMoveProgressBar.Value++;
                }
                else
                {
                    //check if this show season combo is already going to be processed
                    ShowSeason showSeason = new ShowSeason(ep.ShowName, ep.Season);
                    if (uniqueShowSeasons.Contains(showSeason))
                    {
                        ProcessFiles.Add(new FileMoveResult(true, ep));
                        FileMoveProgressBar.Value++;
                    }
                    else
                    {
                        ct.ThrowIfCancellationRequested();
                        FileMoveResult result = await await bgQueue.QueueTask(() => FileMover.CreateDirectoriesAndDownloadBannersAsync(ep, settings));
                        FileMoveProgressBar.Value++;
                        if (result.Success)
                        {
                            ProcessFiles.Add(result);
                            WriteNewLineToTextBox(string.Format("Successfully processed directory for: {0}", result.Episode.FilePath));
                        }
                        else
                        {
                            WriteNewLineToTextBox(string.Format("Failed to process directory for: {0}", result.Episode.FilePath));
                        }
                    }
                }
            }

            return ProcessFiles;
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
            ButtonsStackPanel.Visibility = System.Windows.Visibility.Collapsed;
            ProgressStackPanel.Visibility = System.Windows.Visibility.Visible;
            try
            {
                settings = GetSettings();
                //process the folders and jpg downloads async
                LogTextBox.Text = string.Format("{0} - Starting", DateTime.Now.ToShortTimeString());
                List<FileMoveResult> filesToMove = await ProcessTVShows(settings, cts.Token);

                if (filesToMove != null && filesToMove.Count > 0)
                {
                    await MoveTVShows(filesToMove, settings, cts.Token);
                }
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
                ButtonsStackPanel.Visibility = System.Windows.Visibility.Visible;
                ProgressStackPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public async Task MoveTVShows(List<FileMoveResult> filesToMove, Settings settings, CancellationToken ct)
        {
            FileMoveProgressBar.Value = 0;
            FileMoveProgressBar.Maximum = filesToMove.Count;
            BackgroundQueue bgQueue = new BackgroundQueue();
            //actually move/copy the files one at a time
            foreach (FileMoveResult fmr in filesToMove)
            {
                ct.ThrowIfCancellationRequested();
                bool result = await bgQueue.QueueTask(() => FileMover.MoveFile(fmr.Episode, settings, fmr.DestinationFilePath));
                FileMoveProgressBar.Value++;
                if (result)
                {
                    scannedEpisodes.Remove(fmr.Episode);
                    WriteNewLineToTextBox(string.Format("Successfully {2} {0} to {1}", fmr.Episode.FilePath, fmr.DestinationFilePath, settings.CopyFiles ? "copied" : "moved"));
                }
                else
                {
                    WriteNewLineToTextBox(string.Format("Failed to {2} {0} to {1}", fmr.Episode.FilePath, fmr.DestinationFilePath, settings.CopyFiles ? "copy" : "move"));
                }
            }
            //add a bit of delay before the progress bar disappears
            await bgQueue.QueueTask(() => Thread.Sleep(1000));
        }

        private async void MatchShowButton_Click(object sender, RoutedEventArgs e)
        {
            settings = GetSettings();
            TVEpisode temp = (TVEpisode)ShowsListBox.SelectedItem;
            temp = await TVShowMatcher.SelectShowFromList(temp, settings);
            ShowsListBox.SelectedItem = temp;
        }

        private void IgnoreShowButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbr = MessageBox.Show("Are you sure?", "Ignore this?", MessageBoxButton.OKCancel);
            if (mbr == MessageBoxResult.OK)
            {
                TVEpisode tempEp = (TVEpisode)ShowsListBox.SelectedItem;
                IgnoreList ignoreList = IgnoreListFramework.ReadIgnoreList();
                if (!ignoreList.IgnoreFiles.Contains(tempEp.FilePath))
                {
                    ignoreList.IgnoreFiles.Add(tempEp.FilePath);
                    scannedEpisodes.Remove(tempEp);
                    IgnoreListFramework.WriteExpressionFile(ignoreList);
                }
            }
        }

        private void ShowsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            TVEpisode temp = (TVEpisode)ShowsListBox.SelectedItem;
            if (temp != null)
            {
                IgnoreShowButton.IsEnabled = true;
            }
            else
            {
                IgnoreShowButton.IsEnabled = false;
            }
            if (temp != null && temp.SkippedExactSelection)
            {
                MatchShowButton.IsEnabled = true;
            }
            else
            {
                MatchShowButton.IsEnabled = false;
            }
            if (temp != null && !temp.SkippedExactSelection)
            {
                ShowDetailButton.IsEnabled = true;
                EditButton.IsEnabled = true;
            }
            else
            {
                ShowDetailButton.IsEnabled = false;
                EditButton.IsEnabled = false;
            }
        }

        private void ShowDetailButton_Click(object sender, RoutedEventArgs e)
        {
            TVEpisode tempEp = (TVEpisode)ShowsListBox.SelectedItem;
            ShowDetailsForm sdf = new ShowDetailsForm(tempEp.TVDBShowId);
            sdf.ShowDialog();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            TVEpisode tempEp = (TVEpisode)ShowsListBox.SelectedItem;
            ShowsListBox.IsEnabled = false;
            IgnoreShowButton.IsEnabled = false;
            MatchShowButton.IsEnabled = false;
            ShowDetailButton.IsEnabled = false;
            EditButton.IsEnabled = false;

            ShowNameTextBox.Text = tempEp.ShowName;
            ShowNameTextBox.Visibility = System.Windows.Visibility.Visible;
            EditOkButton.Visibility = System.Windows.Visibility.Visible;
            ShowNameTextBox.Focus();
        }

        private void EditOkButton_Click(object sender, RoutedEventArgs e)
        {
            TVEpisode tempEp = (TVEpisode)ShowsListBox.SelectedItem;
            string oldTitle = tempEp.ShowName;
            string newTitle = ShowNameTextBox.Text;

            if (!newTitle.Equals(oldTitle))
            {
                MessageBoxResult mbr = MessageBox.Show("Are you sure you want to change this shows name?", "Confirmation", MessageBoxButton.OKCancel);
                if (mbr == MessageBoxResult.OK)
                {
                    foreach (TVEpisode tve in scannedEpisodes)
                    {
                        if (tve.ShowName.Equals(oldTitle))
                        {
                            tve.ShowName = newTitle;
                        }
                    }
                }
            }

            ShowsListBox.IsEnabled = true;
            IgnoreShowButton.IsEnabled = true;
            MatchShowButton.IsEnabled = true;
            ShowDetailButton.IsEnabled = true;
            EditButton.IsEnabled = true;
            ShowNameTextBox.Text = "";
            ShowNameTextBox.Visibility = System.Windows.Visibility.Hidden;
            EditOkButton.Visibility = System.Windows.Visibility.Hidden;
        }


    }
}
