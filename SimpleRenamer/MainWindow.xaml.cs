using MahApps.Metro.Controls;
using SimpleRenamer.Common.EventArguments;
using SimpleRenamer.Common.Interface;
using SimpleRenamer.Common.Model;
using SimpleRenamer.Common.Movie.Interface;
using SimpleRenamer.Common.TV.Interface;
using SimpleRenamer.EventArguments;
using SimpleRenamer.Views;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace SimpleRenamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public CancellationTokenSource cts;
        private ObservableCollection<MatchedFile> scannedEpisodes;

        //here are all our interfaces
        private ILogger logger;
        private ITVShowMatcher tvShowMatcher;
        private IMovieMatcher movieMatcher;
        private IDependencyInjectionContext injectionContext;
        private IScanFiles scanForShows;
        private IActionMatchedFiles performActionsOnShows;
        private IConfigurationManager configurationManager;
        private SelectShowWindow selectShowWindow;
        private ShowDetailsWindow showDetailsWindow;
        private MovieDetailsWindow movieDetailsWindow;
        private SettingsWindow settingsWindow;
        private Settings settings;
        private string EditShowCurrentFolder;
        private string EditShowTvdbShowName;
        private string EditShowTvdbId;
        private string MediaTypePath;
        private string MediaTypeShowName;

        public MainWindow(ILogger log, ITVShowMatcher tvShowMatch, IMovieMatcher movieMatch, IDependencyInjectionContext injection, IActionMatchedFiles performActions, IScanFiles scanShows, IConfigurationManager configManager)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            if (tvShowMatch == null)
            {
                throw new ArgumentNullException(nameof(tvShowMatch));
            }
            if (movieMatch == null)
            {
                throw new ArgumentNullException(nameof(movieMatch));
            }
            if (injection == null)
            {
                throw new ArgumentNullException(nameof(injection));
            }
            if (scanShows == null)
            {
                throw new ArgumentNullException(nameof(scanShows));
            }
            if (performActions == null)
            {
                throw new ArgumentNullException(nameof(performActions));
            }
            if (configManager == null)
            {
                throw new ArgumentNullException(nameof(configManager));
            }
            logger = log;
            tvShowMatcher = tvShowMatch;
            movieMatcher = movieMatch;
            injectionContext = injection;
            scanForShows = scanShows;
            performActionsOnShows = performActions;
            configurationManager = configManager;

            try
            {
                InitializeComponent();
                logger.TraceMessage("Starting Application");
                showDetailsWindow = injectionContext.GetService<ShowDetailsWindow>();
                movieDetailsWindow = injectionContext.GetService<MovieDetailsWindow>();
                settingsWindow = injectionContext.GetService<SettingsWindow>();
                selectShowWindow = injectionContext.GetService<SelectShowWindow>();
                selectShowWindow.RaiseSelectShowWindowEvent += SelectShowWindow_RaiseSelectShowWindowEvent;
                settings = configurationManager.Settings;
                scannedEpisodes = new ObservableCollection<MatchedFile>();
                ShowsListBox.ItemsSource = scannedEpisodes;
                ShowsListBox.SizeChanged += ListView_SizeChanged;
                ShowsListBox.Loaded += ListView_Loaded;

                //setup the perform actions event handlers
                performActionsOnShows.RaiseFileMovedEvent += PerformActionsOnShows_RaiseFileMovedEvent;
                performActionsOnShows.RaiseFilePreProcessedEvent += PerformActionsOnShows_RaiseFilePreProcessedEvent;
                performActionsOnShows.RaiseProgressEvent += ProgressTextEvent;
                scanForShows.RaiseProgressEvent += ProgressTextEvent;
                ScanButton.IsEnabled = IsScanEnabled();
                this.Closing += MainWindow_Closing;
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        private bool IsScanEnabled()
        {
            if (string.IsNullOrEmpty(settings.DestinationFolderMovie) && string.IsNullOrEmpty(settings.DestinationFolderTV))
            {
                return false;
            }
            else if (settings.WatchFolders.Count < 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateColumnsWidth(sender as ListView);
        }

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateColumnsWidth(sender as ListView);
        }

        private void UpdateColumnsWidth(ListView listView)
        {
            int autoFillColumnIndex = (listView.View as GridView).Columns.Count - 1;
            if (listView.ActualWidth == Double.NaN)
                listView.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            double remainingSpace = listView.ActualWidth;
            for (int i = 0; i < (listView.View as GridView).Columns.Count; i++)
                if (i != autoFillColumnIndex)
                    remainingSpace -= (listView.View as GridView).Columns[i].ActualWidth;
            (listView.View as GridView).Columns[autoFillColumnIndex].Width = remainingSpace >= 0 ? remainingSpace : 0;
        }

        private void ProgressTextEvent(object sender, ProgressTextEventArgs e)
        {
            SetProgressText(e.Text);
        }

        private void SetProgressText(string text)
        {
            if (ProgressTextBlock.Dispatcher.CheckAccess())
            {
                //calling thread owns the dispatchers
                ProgressTextBlock.Text = text;
            }
            else
            {
                //invocation required
                ProgressTextBlock.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => SetProgressText(text)));
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void PerformActionsOnShows_RaiseFilePreProcessedEvent(object sender, FilePreProcessedEventArgs e)
        {
            FileMoveProgressBar.Value++;
        }

        private void PerformActionsOnShows_RaiseFileMovedEvent(object sender, FileMovedEventArgs e)
        {
            FileMoveProgressBar.Value++;
            scannedEpisodes.Remove(e.Episode);
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                logger.TraceMessage("Closing");
                if (CancelButton.Visibility == Visibility.Visible)
                {
                    e.Cancel = true;
                }
                configurationManager.SaveConfiguration();
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        private void DisableUi()
        {
            //disable all action buttons
            ScanButton.IsEnabled = false;
            SettingsButton.IsEnabled = false;
            ActionButton.IsEnabled = false;
            MatchShowButton.IsEnabled = false;
            IgnoreShowButton.IsEnabled = false;
            ShowsListBox.IsEnabled = false;
            //enable the cancel button
            CancelButton.IsEnabled = true;
            //hide the scan and action buttons
            ScanButton.Visibility = Visibility.Hidden;
            ActionButton.Visibility = Visibility.Hidden;
            //hide the bottom row of buttons
            ButtonsStackPanel.Visibility = Visibility.Hidden;
            //show the cancel button and progress bar
            CancelButton.Visibility = Visibility.Visible;
            ProgressTextStackPanel.Visibility = Visibility.Visible;
            ProgressBarStackPanel.Visibility = Visibility.Visible;
        }

        private void EnableUi()
        {
            //enable all action buttons
            ScanButton.IsEnabled = IsScanEnabled();
            SettingsButton.IsEnabled = true;
            ShowsListBox.IsEnabled = true;
            //disable the cancel button
            CancelButton.IsEnabled = false;
            //show the scan and action buttons
            ScanButton.Visibility = Visibility.Visible;
            ActionButton.Visibility = Visibility.Visible;
            //show the bottom row of buttons
            ButtonsStackPanel.Visibility = Visibility.Visible;
            //hide the cancel button and progress bar
            CancelButton.Visibility = Visibility.Hidden;
            ProgressTextStackPanel.Visibility = Visibility.Hidden;
            ProgressBarStackPanel.Visibility = Visibility.Hidden;
            if (scannedEpisodes.Count > 0)
            {
                ActionButton.IsEnabled = true;
            }
        }

        private async void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            scannedEpisodes = new ObservableCollection<MatchedFile>();
            ShowsListBox.ItemsSource = scannedEpisodes;
            cts = new CancellationTokenSource();
            DisableUi();
            try
            {
                //set to indeterminate so it keeps looping while scanning
                FileMoveProgressBar.IsIndeterminate = true;

                logger.TraceMessage(string.Format("Starting"));
                var ep = await scanForShows.Scan(cts.Token);
                scannedEpisodes = new ObservableCollection<MatchedFile>(ep);
                logger.TraceMessage($"Grabbed {scannedEpisodes.Count} episodes");
                ShowsListBox.ItemsSource = scannedEpisodes;
                logger.TraceMessage($"Populated listbox with the scanned episodes");
                //add a bit of delay before the progress bar disappears
                await Task.Delay(TimeSpan.FromMilliseconds(300));
            }
            catch (OperationCanceledException)
            {
                logger.TraceMessage("User canceled scan");
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
            finally
            {
                logger.TraceMessage("Finished");
                EnableUi();
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

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //show the settings window
                settingsWindow.ShowDialog();
                settings = configurationManager.Settings;
                ScanButton.IsEnabled = IsScanEnabled();
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        private async void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            cts = new CancellationTokenSource();
            DisableUi();
            try
            {
                //set to indeterminate so it keeps looping while scanning
                FileMoveProgressBar.IsIndeterminate = false;
                //process the folders and jpg downloads async
                logger.TraceMessage(string.Format("Starting"));
                FileMoveProgressBar.Value = 0;
                FileMoveProgressBar.Maximum = (scannedEpisodes.Where(x => x.ActionThis == true).Count()) * 2;
                await performActionsOnShows.Action(scannedEpisodes, cts.Token);
                //add a bit of delay before the progress bar disappears
                await Task.Delay(TimeSpan.FromMilliseconds(300));
            }
            catch (OperationCanceledException)
            {
                logger.TraceMessage("User canceled actions");
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
            finally
            {
                logger.TraceMessage("Finished");
                EnableUi();
            }
        }

        private void MatchShowButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MatchedFile temp = (MatchedFile)ShowsListBox.SelectedItem;
                FileType fileType = temp.FileType;
                MediaTypePath = temp.FilePath;
                MediaTypeShowName = temp.ShowName;
                if (fileType == FileType.Unknown)
                {
                    //IF UNKNOWN then we have to show a dialog here and ask whether movie or TV
                    MediaTypeFlyout.IsOpen = true;
                }
                else
                {
                    //otherwise open the show search window
                    OpenSelectShowWindow(fileType);
                }
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        private void OpenSelectShowWindow(FileType fileType)
        {
            string title = fileType == FileType.TvShow ? "TV" : "Movie";
            DisableUi();
            selectShowWindow.SearchForMatches($"Simple Renamer - {title} - Select Show For File {Path.GetFileName(MediaTypePath)}", MediaTypeShowName, fileType);
            selectShowWindow.ShowDialog();
        }

        private void MediaTypeTv_Click(object sender, RoutedEventArgs e)
        {
            MediaTypeFlyout.IsOpen = false;
            OpenSelectShowWindow(FileType.TvShow);
        }

        private void MediaTypeMovie_Click(object sender, RoutedEventArgs e)
        {
            MediaTypeFlyout.IsOpen = false;
            OpenSelectShowWindow(FileType.Movie);
        }

        private async void SelectShowWindow_RaiseSelectShowWindowEvent(object sender, SelectShowEventArgs e)
        {
            try
            {
                MatchedFile temp = (MatchedFile)ShowsListBox.SelectedItem;
                MatchedFile updatedFile;
                if (e.Type == FileType.TvShow)
                {
                    updatedFile = await tvShowMatcher.UpdateEpisodeWithMatchedSeries(e.ID, temp);
                }
                else
                {
                    updatedFile = await movieMatcher.UpdateFileWithMatchedMovie(e.ID, temp);
                }

                //if selection wasn't skipped then update the selected item
                if (updatedFile.SkippedExactSelection == false)
                {
                    ShowsListBox.SelectedItem = updatedFile;
                }

                EnableUi();
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        private void IgnoreShowButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IgnoreFlyout.IsOpen = true;
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        private void IgnoreFlyoutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IgnoreFlyout.IsOpen = false;
                MatchedFile tempEp = (MatchedFile)ShowsListBox.SelectedItem;
                IgnoreList ignoreList = configurationManager.IgnoredFiles;
                if (!ignoreList.IgnoreFiles.Contains(tempEp.FilePath))
                {
                    ignoreList.IgnoreFiles.Add(tempEp.FilePath);
                    scannedEpisodes.Remove(tempEp);
                    configurationManager.IgnoredFiles = ignoreList;
                }
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        private void ShowsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            MatchedFile temp = (MatchedFile)ShowsListBox.SelectedItem;
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
                DetailButton.IsEnabled = true;
                //we can only edit show folders
                if (temp.FileType == FileType.TvShow)
                {
                    EditButton.IsEnabled = true;
                }
            }
            else
            {
                DetailButton.IsEnabled = false;
                EditButton.IsEnabled = false;
            }
        }

        private void DetailButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                logger.TraceMessage("Show Detail button clicked");
                MatchedFile tempEp = (MatchedFile)ShowsListBox.SelectedItem;
                if (tempEp.FileType == FileType.TvShow)
                {
                    showDetailsWindow.GetSeriesInfo(tempEp.TVDBShowId);
                    showDetailsWindow.ShowDialog();
                }
                else if (tempEp.FileType == FileType.Movie)
                {
                    movieDetailsWindow.GetMovieInfo(tempEp.TMDBShowId.ToString());
                    movieDetailsWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                logger.TraceMessage("Edit button clicked");
                MatchedFile tempEp = (MatchedFile)ShowsListBox.SelectedItem;
                if (tempEp.FileType == FileType.TvShow)
                {
                    logger.TraceMessage(string.Format("For show {0}, season {1}, episode {2}, TVDBShowId {3}", tempEp.ShowName, tempEp.Season, tempEp.Episode, tempEp.TVDBShowId));
                    ShowNameMapping snm = configurationManager.ShowNameMappings;
                    if (snm != null && snm.Mappings.Count > 0)
                    {
                        logger.TraceMessage(string.Format("Mappings available"));
                        Mapping mapping = snm.Mappings.Where(x => x.TVDBShowID.Equals(tempEp.TVDBShowId)).FirstOrDefault();
                        if (mapping != null)
                        {
                            logger.TraceMessage(string.Format("Mapping found {0}", mapping.FileShowName));
                            ShowEditShowWindow(tempEp, mapping);
                        }
                        else
                        {
                            logger.TraceMessage(string.Format("Mapping could not be found!"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        private void ShowEditShowWindow(MatchedFile tempEp, Mapping mapping)
        {
            try
            {
                string folderPath = Path.Combine(settings.DestinationFolderTV, string.IsNullOrEmpty(mapping.CustomFolderName) ? mapping.TVDBShowName : mapping.CustomFolderName);
                EditShowCurrentFolder = mapping.CustomFolderName;
                EditShowTvdbShowName = tempEp.ShowName;
                EditShowTvdbId = tempEp.TVDBShowId;
                EditShowFolderTextBox.Text = folderPath;
                EditFlyout.IsOpen = true;
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        private void EditShowCloseBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                EditFlyout.IsOpen = false;
                bool updateMapping = false;
                string currentText = EditShowFolderTextBox.Text;

                if (EditShowCurrentFolder.Equals(currentText))
                {
                    //if the custom folder name hasn't changed then don't do anything
                }
                else if (EditShowTvdbShowName.Equals(currentText) && string.IsNullOrEmpty(EditShowCurrentFolder))
                {
                    //if the new folder name equals the tvshowname and no customfolder name then dont do anything
                }
                else if (EditShowTvdbShowName.Equals(currentText) && !string.IsNullOrEmpty(EditShowCurrentFolder))
                {
                    //if the new folder name equals the tvshowname and there is a customfoldername already then reset customfoldername to blank
                    currentText = string.Empty;
                    updateMapping = true;
                }
                else
                {
                    //else we have a new custom folder to set
                    updateMapping = true;
                }

                if (updateMapping)
                {
                    ShowNameMapping snm = configurationManager.ShowNameMappings;
                    if (snm != null && snm.Mappings.Count > 0)
                    {
                        Mapping mapping = snm.Mappings.Where(x => x.TVDBShowID.Equals(EditShowTvdbId)).FirstOrDefault();
                        if (mapping != null)
                        {
                            mapping.CustomFolderName = currentText;
                            configurationManager.ShowNameMappings = snm;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        private void EditShowCloseBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void EditShowFolderTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            string currentText = EditShowFolderTextBox.Text;
            EditShowFolderTextBox.Text = currentText.Replace(settings.DestinationFolderTV + @"\", "");
        }

        private void EditShowFolderTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            string currentText = EditShowFolderTextBox.Text;
            EditShowFolderTextBox.Text = Path.Combine(settings.DestinationFolderTV, currentText);
        }
    }
}
