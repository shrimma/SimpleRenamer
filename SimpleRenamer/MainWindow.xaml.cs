using SimpleRenamer.Framework;
using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Extensions;
using SimpleRenamer.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<TVEpisode> scannedEpisodes;

        //here are all our interfaces
        private ILogger logger;
        private IFileWatcher fileWatcher;
        private ITVShowMatcher tvShowMatcher;
        private IFileMatcher fileMatcher;
        private IIgnoreListFramework ignoreListFramework;
        private Settings settings;

        public MainWindow(ILogger log, IFileWatcher fileWatch, ITVShowMatcher tvShowMatch, IFileMatcher fileMatch, ISettingsFactory settingsFactory, IIgnoreListFramework ignore)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            if (fileWatch == null)
            {
                throw new ArgumentNullException(nameof(fileWatch));
            }
            if (tvShowMatch == null)
            {
                throw new ArgumentNullException(nameof(tvShowMatch));
            }
            if (fileMatch == null)
            {
                throw new ArgumentNullException(nameof(fileMatch));
            }
            if (settingsFactory == null)
            {
                throw new ArgumentNullException(nameof(settingsFactory));
            }
            if (ignore == null)
            {
                throw new ArgumentNullException(nameof(ignore));
            }
            logger = log;
            fileWatcher = fileWatch;
            tvShowMatcher = tvShowMatch;
            fileMatcher = fileMatch;
            ignoreListFramework = ignore;

            try
            {
                InitializeComponent();
                logger.TraceMessage("Starting Application");
                settings = settingsFactory.GetSettings();
                scannedEpisodes = new ObservableCollection<TVEpisode>();
                ShowsListBox.ItemsSource = scannedEpisodes;
                this.Closing += MainWindow_Closing;
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                WriteNewLineToTextBox("Closing");
                if (!RunButton.IsEnabled)
                {
                    e.Cancel = true;
                }
                else
                {
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
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
                LogTextBox.Text = string.Format("{0} - Starting", DateTime.Now.ToShortTimeString());
                List<string> videoFiles = await fileWatcher.SearchTheseFoldersAsync(cts.Token);
                WriteNewLineToTextBox(string.Format("Found {0} files within the watch folders", videoFiles.Count));
                await MatchTVShows(videoFiles, cts.Token);
                WriteNewLineToTextBox(string.Format("Matched {0} files", scannedEpisodes.Count));
            }
            catch (OperationCanceledException)
            {
                WriteNewLineToTextBox("User canceled scan.");
                logger.TraceMessage("User cancelled scan");
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
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
            try
            {
                CancelButton.IsEnabled = false;
                if (cts != null)
                {
                    cts.Cancel();
                }
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        private void WriteNewLineToTextBox(string text)
        {
            LogTextBox.Text += string.Format("\n{0} - {1}", DateTime.Now.ToShortTimeString(), text);
            logger.TraceMessage(text);
        }

        public async Task MatchTVShows(List<string> videoFiles, CancellationToken ct)
        {
            try
            {
                scannedEpisodes.Clear();
                ShowNameMapping showNameMapping = await tvShowMatcher.ReadMappingFileAsync();
                ShowNameMapping originalMapping = await tvShowMatcher.ReadMappingFileAsync();
                List<Task<TVEpisode>> tasks = new List<Task<TVEpisode>>();
                //spin up a task for each file
                foreach (string fileName in videoFiles)
                {
                    WriteNewLineToTextBox(string.Format("Trying to match {0}", fileName));
                    tasks.Add(fileMatcher.SearchFileNameAsync(fileName));
                }
                //as each task completes
                foreach (var t in tasks.InCompletionOrder())
                {
                    ct.ThrowIfCancellationRequested();
                    TVEpisode tempEp = await t;
                    TVEpisodeScrape scrapeResult = null;
                    if (tempEp != null)
                    {
                        WriteNewLineToTextBox(string.Format("Matched {0}", tempEp.EpisodeName));
                        //scrape the episode name and incorporate this in the filename (if setting allows)
                        if (settings.RenameFiles)
                        {
                            scrapeResult = await tvShowMatcher.ScrapeDetailsAsync(tempEp, showNameMapping);
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
                    else
                    {
                        WriteNewLineToTextBox(string.Format("Couldn't find a match!"));
                    }
                }
                if (showNameMapping.Mappings != originalMapping.Mappings || showNameMapping.Mappings.Count != originalMapping.Mappings.Count)
                {
                    await tvShowMatcher.WriteMappingFileAsync(showNameMapping);
                }
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        public async Task<List<FileMoveResult>> ProcessTVShows(CancellationToken ct)
        {
            List<Task<FileMoveResult>> tasks = new List<Task<FileMoveResult>>();
            List<ShowSeason> uniqueShowSeasons = new List<ShowSeason>();
            List<FileMoveResult> ProcessFiles = new List<FileMoveResult>();
            ShowNameMapping snm = await tvShowMatcher.ReadMappingFileAsync();
            FileMoveProgressBar.Value = 0;
            FileMoveProgressBar.Maximum = scannedEpisodes.Count;
            BackgroundQueue bgQueue = new BackgroundQueue();
            try
            {
                foreach (TVEpisode ep in scannedEpisodes)
                {
                    if (!ep.ActionThis)
                    {
                        WriteNewLineToTextBox(string.Format("Skipped {0} as user chose not to action.", ep.FilePath));
                        FileMoveProgressBar.Value++;
                    }
                    else
                    {
                        if (settings.RenameFiles)
                        {
                            Mapping mapping = snm.Mappings.Where(x => x.TVDBShowID.Equals(ep.TVDBShowId)).FirstOrDefault();
                            //check if this show season combo is already going to be processed
                            ShowSeason showSeason = new ShowSeason(ep.ShowName, ep.Season);
                            bool alreadyGrabbedBanners = false;
                            foreach (ShowSeason unique in uniqueShowSeasons)
                            {
                                if (unique.Season.Equals(showSeason.Season) && unique.Show.Equals(showSeason.Show))
                                {
                                    alreadyGrabbedBanners = true;
                                    break;
                                }
                            }
                            if (alreadyGrabbedBanners)
                            {
                                //if we have already processed this show season combo then dont download the banners again
                                FileMoveResult result = await await bgQueue.QueueTask(() => FileMover.CreateDirectoriesAndDownloadBannersAsync(ep, mapping, settings, false));
                                if (result.Success)
                                {
                                    ProcessFiles.Add(result);
                                    WriteNewLineToTextBox(string.Format("Successfully processed file without banners: {0}", result.Episode.FilePath));
                                }
                                FileMoveProgressBar.Value++;
                            }
                            else
                            {
                                ct.ThrowIfCancellationRequested();
                                FileMoveResult result = await await bgQueue.QueueTask(() => FileMover.CreateDirectoriesAndDownloadBannersAsync(ep, mapping, settings, true));
                                if (result.Success)
                                {
                                    ProcessFiles.Add(result);
                                    uniqueShowSeasons.Add(showSeason);
                                    WriteNewLineToTextBox(string.Format("Successfully processed file and downloaded banners: {0}", result.Episode.FilePath));
                                }
                                else
                                {
                                    WriteNewLineToTextBox(string.Format("Failed to process {0}", result.Episode.FilePath));
                                }
                                FileMoveProgressBar.Value++;
                            }
                        }
                        else
                        {
                            FileMoveResult result = await await bgQueue.QueueTask(() => FileMover.CreateDirectoriesAndDownloadBannersAsync(ep, null, settings, false));
                            if (result.Success)
                            {
                                ProcessFiles.Add(result);
                                WriteNewLineToTextBox(string.Format("Successfully processed file without renaming: {0}", result.Episode.FilePath));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
            return ProcessFiles;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //show the settings window
                SettingsWindow settingsWindow = new SettingsWindow();
                settingsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
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
                //process the folders and jpg downloads async
                LogTextBox.Text = string.Format("{0} - Starting", DateTime.Now.ToShortTimeString());
                List<FileMoveResult> filesToMove = await ProcessTVShows(cts.Token);

                if (filesToMove != null && filesToMove.Count > 0)
                {
                    await MoveTVShows(filesToMove, settings, cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                WriteNewLineToTextBox("User canceled actions.");
                logger.TraceMessage("User cancelled scan");
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
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
            try
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
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        private async void MatchShowButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TVEpisode temp = (TVEpisode)ShowsListBox.SelectedItem;
                temp = await tvShowMatcher.SelectShowFromList(temp);
                ShowsListBox.SelectedItem = temp;
                //if a selection is made then force a rescan
                if (!temp.SkippedExactSelection)
                {
                    scannedEpisodes.Clear();
                    ActionButton.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        private async Task IgnoreShowButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult mbr = MessageBox.Show("Are you sure?", "Ignore this?", MessageBoxButton.OKCancel);
                if (mbr == MessageBoxResult.OK)
                {
                    TVEpisode tempEp = (TVEpisode)ShowsListBox.SelectedItem;
                    IgnoreList ignoreList = await ignoreListFramework.ReadIgnoreListAsync();
                    if (!ignoreList.IgnoreFiles.Contains(tempEp.FilePath))
                    {
                        ignoreList.IgnoreFiles.Add(tempEp.FilePath);
                        scannedEpisodes.Remove(tempEp);
                        await ignoreListFramework.WriteIgnoreListAsync(ignoreList);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
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
            if (temp != null && !temp.SkippedExactSelection && settings.RenameFiles)
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
            try
            {
                TVEpisode tempEp = (TVEpisode)ShowsListBox.SelectedItem;
                ShowDetailsForm sdf = new ShowDetailsForm(tempEp.TVDBShowId);
                sdf.ShowDialog();
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        private async Task EditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WriteNewLineToTextBox("Edit button clicked");
                TVEpisode tempEp = (TVEpisode)ShowsListBox.SelectedItem;
                WriteNewLineToTextBox(string.Format("For show {0}, season {1}, episode {2}", tempEp.ShowName, tempEp.Season, tempEp.Episode));
                ShowNameMapping snm = await tvShowMatcher.ReadMappingFileAsync();
                if (snm != null && snm.Mappings.Count > 0)
                {
                    WriteNewLineToTextBox(string.Format("Mappings available"));
                    Mapping mapping = snm.Mappings.Where(x => x.TVDBShowID.Equals(tempEp.TVDBShowId)).FirstOrDefault();
                    if (mapping != null)
                    {
                        WriteNewLineToTextBox(string.Format("Mapping found {0}", mapping.FileShowName));
                        ShowEditShowWindow(tempEp, mapping);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        private void ShowEditShowWindow(TVEpisode tempEp, Mapping mapping)
        {
            try
            {
                EditShowWindow esw = new EditShowWindow(tempEp, mapping);
                esw.RaiseCustomEvent += new EventHandler<EditShowEventArgs>(EditShowWindowClosedEvent);
                esw.ShowDialog();
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        public async Task EditShowWindowClosedEvent(object sender, EditShowEventArgs e)
        {
            try
            {
                if (e.Mapping != null)
                {
                    ShowNameMapping snm = await tvShowMatcher.ReadMappingFileAsync();
                    if (snm != null && snm.Mappings.Count > 0)
                    {
                        Mapping mapping = snm.Mappings.Where(x => x.TVDBShowID.Equals(e.Mapping.TVDBShowID)).FirstOrDefault();
                        if (mapping != null)
                        {
                            mapping.CustomFolderName = e.NewFolder;
                            await tvShowMatcher.WriteMappingFileAsync(snm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        private void EditOkButton_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }
    }
}
