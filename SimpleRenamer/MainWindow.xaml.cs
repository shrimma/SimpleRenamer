using MahApps.Metro.Controls;
using SimpleRenamer.EventArguments;
using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.EventArguments;
using SimpleRenamer.Framework.Interface;
using SimpleRenamer.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WPFCustomMessageBox;

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
        private IScanForShows scanForShows;
        private IPerformActionsOnShows performActionsOnShows;
        private IConfigurationManager configurationManager;
        private SelectShowWindow selectShowWindow;
        private ShowDetailsWindow showDetailsWindow;
        private MovieDetailsWindow movieDetailsWindow;
        private SettingsWindow settingsWindow;
        private EditShowWindow editShowWindow;
        private Settings settings;

        public MainWindow(ILogger log, ITVShowMatcher tvShowMatch, IMovieMatcher movieMatch, IDependencyInjectionContext injection, IPerformActionsOnShows performActions, IScanForShows scanShows, IConfigurationManager configManager)
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
                editShowWindow = injectionContext.GetService<EditShowWindow>();
                editShowWindow.RaiseEditShowEvent += new EventHandler<EditShowEventArgs>(EditShowWindowClosedEvent);
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

                this.Closing += MainWindow_Closing;
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
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
                if (ScanButton.IsEnabled == false)
                {
                    e.Cancel = true;
                }
                else if (ActionButton.IsEnabled == false)
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
            ScanButton.IsEnabled = false;
            SettingsButton.IsEnabled = false;
            ActionButton.IsEnabled = false;
            MatchShowButton.IsEnabled = false;
            IgnoreShowButton.IsEnabled = false;
            CancelButton.IsEnabled = true;
            ShowsListBox.IsEnabled = false;
            ButtonsStackPanel.Visibility = Visibility.Collapsed;
            ProgressTextStackPanel.Visibility = Visibility.Visible;
            ProgressBarStackPanel.Visibility = Visibility.Visible;
        }

        private void EnableUi()
        {
            ScanButton.IsEnabled = true;
            SettingsButton.IsEnabled = true;
            CancelButton.IsEnabled = false;
            ShowsListBox.IsEnabled = true;
            ButtonsStackPanel.Visibility = Visibility.Visible;
            ProgressTextStackPanel.Visibility = Visibility.Collapsed;
            ProgressBarStackPanel.Visibility = Visibility.Collapsed;
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

        private async void MatchShowButton_Click(object sender, RoutedEventArgs e)
        {
            cts = new CancellationTokenSource();
            try
            {
                MatchedFile temp = (MatchedFile)ShowsListBox.SelectedItem;
                FileType fileType = temp.FileType;
                if (fileType == FileType.Unknown)
                {
                    //IF UNKNOWN then we have to show a dialog here and ask whether movie or TV
                    MessageBoxResult result = CustomMessageBox.ShowYesNoCancel($"Is the file at path: {temp.FilePath} a TV show or a movie?", $"TV or Movie", "TV Show", "Movie", "Cancel");
                    if (result == MessageBoxResult.Yes)
                    {
                        fileType = FileType.TvShow;
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        fileType = FileType.Movie;
                    }
                }

                List<ShowView> possibleMatches;
                string title = fileType == FileType.TvShow ? "TV" : "Movie";
                selectShowWindow.SearchForMatches($"Simple Renamer - {title} - Select Show for file {Path.GetFileName(temp.FilePath)}", temp.ShowName, fileType);
                selectShowWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        private async void SelectShowWindow_RaiseSelectShowWindowEvent(object sender, SelectShowEventArgs e)
        {
            cts = new CancellationTokenSource();
            try
            {
                MatchedFile temp = (MatchedFile)ShowsListBox.SelectedItem;
                MatchedFile updatedFile;
                if (e.Type == FileType.TvShow)
                {
                    updatedFile = await tvShowMatcher.UpdateEpisodeWithMatchedSeries(e.ID, temp, cts.Token);
                }
                else
                {
                    updatedFile = await movieMatcher.UpdateFileWithMatchedMovie(e.ID, temp, cts.Token);
                }

                //if selection was skipped then we can't enable actioning
                if (updatedFile.SkippedExactSelection == false)
                {
                    ShowsListBox.SelectedItem = updatedFile;
                }
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
                MessageBoxResult mbr = MessageBox.Show("Are you sure?", "Ignore this?", MessageBoxButton.OKCancel);
                if (mbr == MessageBoxResult.OK)
                {
                    MatchedFile tempEp = (MatchedFile)ShowsListBox.SelectedItem;
                    IgnoreList ignoreList = configurationManager.IgnoredFiles;
                    if (!ignoreList.IgnoreFiles.Contains(tempEp.FilePath))
                    {
                        ignoreList.IgnoreFiles.Add(tempEp.FilePath);
                        scannedEpisodes.Remove(tempEp);
                        configurationManager.IgnoredFiles = ignoreList;
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
                else if (tempEp.FileType == FileType.Movie)
                {

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
                //set the edit show window to the selected episode and show dialog
                editShowWindow.SetCurrentShow(tempEp, mapping);
                editShowWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        public void EditShowWindowClosedEvent(object sender, EditShowEventArgs e)
        {
            try
            {
                if (e.Mapping != null)
                {
                    ShowNameMapping snm = configurationManager.ShowNameMappings;
                    if (snm != null && snm.Mappings.Count > 0)
                    {
                        Mapping mapping = snm.Mappings.Where(x => x.TVDBShowID.Equals(e.Mapping.TVDBShowID)).FirstOrDefault();
                        if (mapping != null)
                        {
                            mapping.CustomFolderName = e.NewFolder;
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
    }
}
