﻿using Jot;
using MahApps.Metro;
using MahApps.Metro.Controls;
using Sarjee.SimpleRenamer.Common.EventArguments;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using Sarjee.SimpleRenamer.Common.Movie.Interface;
using Sarjee.SimpleRenamer.Common.TV.Interface;
using Sarjee.SimpleRenamer.EventArguments;
using Sarjee.SimpleRenamer.Views;
using Sarjee.SimpleRenamer.WPF;
using Sarjee.SimpleRenamer.WPF.ThemeManagerHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Sarjee.SimpleRenamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        //here are all our interfaces
        private readonly ILogger _logger;
        private readonly ITVShowMatcher _tvShowMatcher;
        private readonly IMovieMatcher _movieMatcher;
        private readonly IDependencyInjectionContext _injectionContext;
        private readonly IScanFiles _scanFiles;
        private readonly IActionMatchedFiles _actionMatchedFiles;
        private readonly IConfigurationManager _configurationManager;
        private SelectShowWindow _selectShowWindow;
        private ShowDetailsWindow _showDetailsWindow;
        private MovieDetailsWindow _movieDetailsWindow;
        private SettingsWindow _settingsWindow;
        private string _editShowCurrentFolder;
        private string _editShowTvdbShowName;
        private string _editShowTvdbId;
        private string _mediaTypePath;
        private string _mediaTypeShowName;
        private StateTracker _stateTracker = new StateTracker();
        private MyAppTheme _currentAppTheme = new MyAppTheme();
        private CancellationTokenSource _cancellationTokenSource;
        private ObservableCollection<MatchedFile> scannedEpisodes;

        public MainWindow(ILogger logger, ITVShowMatcher tvShowMatcher, IMovieMatcher movieMatcher, IDependencyInjectionContext injectionContext, IActionMatchedFiles actionMatchedFiles, IScanFiles scanFiles, IConfigurationManager configManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tvShowMatcher = tvShowMatcher ?? throw new ArgumentNullException(nameof(tvShowMatcher));
            _movieMatcher = movieMatcher ?? throw new ArgumentNullException(nameof(movieMatcher));
            _injectionContext = injectionContext ?? throw new ArgumentNullException(nameof(injectionContext));
            _scanFiles = scanFiles ?? throw new ArgumentNullException(nameof(scanFiles));
            _actionMatchedFiles = actionMatchedFiles ?? throw new ArgumentNullException(nameof(actionMatchedFiles));
            _configurationManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
            try
            {
                InitializeComponent();
                _logger.TraceMessage("Starting Application", EventLevel.LogAlways);
                this.SourceInitialized += (s, e) =>
                {
                    //setup statetracker
                    _stateTracker.Configure(this).Apply();
                    _stateTracker.Configure(_configurationManager)
                                    .IdentifyAs("Configuration")
                                    .Apply();
                    _stateTracker.Configure(_currentAppTheme)
                                    .AddProperties<MyAppTheme>(x => x.AccentName, x => x.AppThemeName)
                                    .IdentifyAs("Theme")
                                    .Apply();

                    //get current accent from accent name                    
                    Accent currentAccent = ThemeManager.Accents.FirstOrDefault(x => x.Name.Equals(_currentAppTheme.AccentName));
                    AppTheme currentAppTheme = ThemeManager.AppThemes.FirstOrDefault(x => x.Name.Equals(_currentAppTheme.AppThemeName));
                    //set the app style as saved
                    ThemeManager.ChangeAppStyle(System.Windows.Application.Current, currentAccent, currentAppTheme);

                    //grab windows
                    _showDetailsWindow = _injectionContext.GetService<ShowDetailsWindow>();
                    _movieDetailsWindow = _injectionContext.GetService<MovieDetailsWindow>();
                    _settingsWindow = _injectionContext.GetService<SettingsWindow>();
                    _selectShowWindow = _injectionContext.GetService<SelectShowWindow>();
                    _selectShowWindow.RaiseSelectShowWindowEvent += SelectShowWindow_RaiseSelectShowWindowEvent;
                    scannedEpisodes = new ObservableCollection<MatchedFile>();

                    //set theme in settings window
                    _settingsWindow.CurrentAccent = new AccentItem(currentAccent.Name, currentAccent.Resources["AccentBaseColor"].ToString(), currentAccent);
                    _settingsWindow.CurrentAppTheme = currentAppTheme;
                    _settingsWindow.SetupView();

                    ShowsListBox.ItemsSource = scannedEpisodes;
                    ShowsListBox.SizeChanged += ListView_SizeChanged;
                    ShowsListBox.Loaded += ListView_Loaded;

                    //setup the perform actions event handlers
                    _actionMatchedFiles.RaiseFileMovedEvent += PerformActionsOnShows_RaiseFileMovedEvent;
                    _actionMatchedFiles.RaiseFilePreProcessedEvent += PerformActionsOnShows_RaiseFilePreProcessedEvent;
                    _actionMatchedFiles.RaiseProgressEvent += ProgressTextEvent;
                    _scanFiles.RaiseProgressEvent += ProgressTextEvent;
                    ScanButton.IsEnabled = IsScanEnabled();
                    this.Closing += MainWindow_Closing;

                };
            }
            catch (Exception ex)
            {
                _logger.TraceException(ex);
            }
        }

        private bool IsScanEnabled()
        {
            if (string.IsNullOrWhiteSpace(_configurationManager.Settings?.DestinationFolderMovie) && string.IsNullOrWhiteSpace(_configurationManager.Settings?.DestinationFolderTV))
            {
                ScanButton.ToolTip = "Destination TV and Movie folders must be configured in settings.";
                return false;
            }
            else if (_configurationManager.Settings?.WatchFolders?.Count < 1)
            {
                ScanButton.ToolTip = "At least one Watch Folder must be configured in settings.";
                return false;
            }
            else
            {
                ScanButton.ToolTip = null;
                return true;
            }
        }

        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            WpfHelper.UpdateColumnsWidth(sender as ListView);
        }

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            WpfHelper.UpdateColumnsWidth(sender as ListView);
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
            IncrementProgressBar();
        }

        private void PerformActionsOnShows_RaiseFileMovedEvent(object sender, FileMovedEventArgs e)
        {
            IncrementProgressBar();
            RemoveFileFromView(e.File);
        }

        private void RemoveFileFromView(MatchedFile file)
        {
            if (ShowsListBox.Dispatcher.CheckAccess())
            {
                //calling thread owns the dispatches
                scannedEpisodes.Remove(file);
            }
            else
            {
                //invokation required
                ShowsListBox.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => RemoveFileFromView(file)));
            }
        }

        private void AddFileToView(MatchedFile file)
        {
            if (ShowsListBox.Dispatcher.CheckAccess())
            {
                //calling thread owns the dispatches
                scannedEpisodes.Add(file);
            }
            else
            {
                //invokation required
                ShowsListBox.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => AddFileToView(file)));
            }
        }

        private void IncrementProgressBar()
        {
            if (FileMoveProgressBar.Dispatcher.CheckAccess())
            {
                //calling thread owns the dispatchers
                FileMoveProgressBar.Value++;
            }
            else
            {
                //invocation required
                ProgressTextBlock.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => IncrementProgressBar()));
            }
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
            {
                try
                {
                    _logger.TraceMessage("Closing", EventLevel.LogAlways);
                    if (CancelButton.Visibility == Visibility.Visible)
                    {
                        e.Cancel = true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.TraceException(ex);
                }
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
            if (scannedEpisodes?.Count > 0)
            {
                ActionButton.IsEnabled = true;
            }
        }

        private async void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            _logger.TraceMessage("Scan button clicked.", EventLevel.Verbose);
            scannedEpisodes = new ObservableCollection<MatchedFile>();
            ShowsListBox.ItemsSource = scannedEpisodes;
            _cancellationTokenSource = new CancellationTokenSource();
            DisableUi();
            try
            {
                //set to indeterminate so it keeps looping while scanning
                FileMoveProgressBar.IsIndeterminate = true;
                _logger.TraceMessage("Scan - Starting.", EventLevel.Informational);
                List<MatchedFile> episodes = await Task.Run(async () =>
                {
                    return await _scanFiles.ScanAsync(_cancellationTokenSource.Token);
                }, _cancellationTokenSource.Token);
                scannedEpisodes = new ObservableCollection<MatchedFile>(episodes);
                _logger.TraceMessage($"Scan - Grabbed {scannedEpisodes.Count} episodes.", EventLevel.Informational);
                ShowsListBox.ItemsSource = scannedEpisodes;
                _logger.TraceMessage($"Scan - Populated listbox with the scanned episodes.", EventLevel.Verbose);
                //add a bit of delay before the progress bar disappears
                await Task.Delay(TimeSpan.FromMilliseconds(300));
            }
            catch (OperationCanceledException)
            {
                _logger.TraceMessage("Scan - user canceled the operation.", EventLevel.Informational);
            }
            catch (Exception ex)
            {
                _logger.TraceException(ex);
            }
            finally
            {
                EnableUi();
                _logger.TraceMessage("Scan - Finished.", EventLevel.Informational);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.TraceMessage("Cancel button clicked.", EventLevel.Verbose);
                CancelButton.IsEnabled = false;
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                }
            }
            catch (Exception ex)
            {
                _logger.TraceException(ex);
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.TraceMessage("Settings button clicked.", EventLevel.Verbose);
                //show the settings window
                _settingsWindow.ShowDialog();
                ScanButton.IsEnabled = IsScanEnabled();
                _currentAppTheme.AccentName = _settingsWindow.CurrentAccent.AccentName;
            }
            catch (Exception ex)
            {
                _logger.TraceException(ex);
            }
        }

        private async void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.TraceMessage("Action button clicked.", EventLevel.Verbose);
                _cancellationTokenSource = new CancellationTokenSource();
                DisableUi();
                //set to indeterminate so it keeps looping while scanning
                FileMoveProgressBar.IsIndeterminate = false;
                //process the folders and jpg downloads async
                _logger.TraceMessage("Action - Starting", EventLevel.Informational);
                FileMoveProgressBar.Value = 0;
                FileMoveProgressBar.Maximum = scannedEpisodes.Count(x => x.ActionThis == true) * 2;
                await Task.Run(async () =>
                {
                    await _actionMatchedFiles.ActionAsync(scannedEpisodes, _cancellationTokenSource.Token);
                }, _cancellationTokenSource.Token);
                //add a bit of delay before the progress bar disappears
                await Task.Delay(TimeSpan.FromMilliseconds(300));
            }
            catch (OperationCanceledException)
            {
                _logger.TraceMessage("Action - user canceled the operation.", EventLevel.Informational);
            }
            catch (Exception ex)
            {
                _logger.TraceException(ex);
            }
            finally
            {
                EnableUi();
                _logger.TraceMessage("Action - Finished", EventLevel.Informational);
            }
        }

        private void MatchShowButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.TraceMessage("MatchShow button clicked.", EventLevel.Verbose);
                MatchedFile temp = (MatchedFile)ShowsListBox.SelectedItem;
                FileType fileType = temp.FileType;
                _mediaTypePath = temp.SourceFilePath;
                _mediaTypeShowName = temp.ShowName;
                _logger.TraceMessage($"User selected to match show for {temp.ShowName}.", EventLevel.Informational);
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
                _logger.TraceException(ex);
            }
        }

        private void OpenSelectShowWindow(FileType fileType)
        {
            string title = fileType == FileType.TvShow ? "TV" : "Movie";
            DisableUi();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _selectShowWindow.SearchForMatches($"Simple Renamer - {title} - Select Show For File {Path.GetFileName(_mediaTypePath)}", _mediaTypeShowName, fileType);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _selectShowWindow.ShowDialog();
        }

        private void MediaTypeTv_Click(object sender, RoutedEventArgs e)
        {
            MediaTypeFlyout.IsOpen = false;
            _logger.TraceMessage($"User selected that {_mediaTypeShowName} is a TV Show.", EventLevel.Informational);
            OpenSelectShowWindow(FileType.TvShow);
        }

        private void MediaTypeMovie_Click(object sender, RoutedEventArgs e)
        {
            MediaTypeFlyout.IsOpen = false;
            _logger.TraceMessage($"User selected that {_mediaTypeShowName} is a Movie.", EventLevel.Informational);
            OpenSelectShowWindow(FileType.Movie);
        }

        private async void SelectShowWindow_RaiseSelectShowWindowEvent(object sender, SelectShowEventArgs e)
        {
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                //if nothing was selected then just return
                if (string.IsNullOrWhiteSpace(e.ID))
                {
                    _logger.TraceMessage($"User did not match {_mediaTypeShowName}.", EventLevel.Informational);
                    return;
                }

                _logger.TraceMessage($"User selected a match for {_mediaTypeShowName}.", EventLevel.Informational);
                //remove the selected item from listbox (we are modifying it so listbox gets confused)
                MatchedFile temp = (MatchedFile)ShowsListBox.SelectedItem;
                RemoveFileFromView(temp);

                //update the file with the matched ID
                MatchedFile updatedFile;
                if (e.Type == FileType.TvShow)
                {
                    updatedFile = await _tvShowMatcher.UpdateEpisodeWithMatchedSeriesAsync(e.ID, temp, _cancellationTokenSource.Token);
                }
                else
                {
                    updatedFile = await _movieMatcher.UpdateFileWithMatchedMovieAsync(e.ID, temp, _cancellationTokenSource.Token);
                }

                //add the file back to the view
                AddFileToView(updatedFile);
            }
            catch (Exception ex)
            {
                _logger.TraceException(ex);
            }
            finally
            {
                EnableUi();
            }
        }

        private void IgnoreShowButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.TraceMessage("Ignore show button clicked.", EventLevel.Verbose);
                IgnoreFlyout.IsOpen = true;
            }
            catch (Exception ex)
            {
                _logger.TraceException(ex);
            }
        }

        private void IgnoreFlyoutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IgnoreFlyout.IsOpen = false;
                MatchedFile tempEp = (MatchedFile)ShowsListBox.SelectedItem;
                _logger.TraceMessage($"User opted to ignore {tempEp.SourceFilePath}.", EventLevel.Informational);
                if (!_configurationManager.IgnoredFiles.Contains(tempEp.SourceFilePath))
                {
                    _configurationManager.IgnoredFiles.Add(tempEp.SourceFilePath);
                    RemoveFileFromView(tempEp);
                }
            }
            catch (Exception ex)
            {
                _logger.TraceException(ex);
            }
        }

        private void ShowsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            MatchedFile temp = (MatchedFile)(sender as ListBox).SelectedItem;
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
                DetailButton.IsEnabled = false;
            }
            else
            {
                MatchShowButton.IsEnabled = false;
                DetailButton.IsEnabled = true;
            }
            if (temp != null && !temp.SkippedExactSelection && _configurationManager.Settings.RenameFiles)
            {
                //we can only edit show folders
                if (temp.FileType == FileType.TvShow)
                {
                    EditButton.IsEnabled = true;
                }
            }
            else
            {
                EditButton.IsEnabled = false;
            }
        }

        private void DetailButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _logger.TraceMessage("Detail button clicked.", EventLevel.Verbose);
                MatchedFile tempEp = (MatchedFile)ShowsListBox.SelectedItem;
                if (tempEp?.FileType == FileType.TvShow)
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    _showDetailsWindow.GetSeriesInfo(tempEp.TVDBShowId, _cancellationTokenSource.Token);
                    _showDetailsWindow.ShowDialog();
                }
                else if (tempEp?.FileType == FileType.Movie)
                {
                    _movieDetailsWindow.GetMovieInfo(tempEp.TMDBShowId.ToString(), _cancellationTokenSource.Token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    _movieDetailsWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                _logger.TraceException(ex);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.TraceMessage("Edit button clicked.", EventLevel.Verbose);
                MatchedFile tempEp = (MatchedFile)ShowsListBox.SelectedItem;
                if (tempEp?.FileType == FileType.TvShow)
                {
                    _logger.TraceMessage(string.Format("Editing - show {0}, season {1}, episode {2}, TVDBShowId {3}", tempEp.ShowName, tempEp.Season, tempEp.EpisodeNumber, tempEp.TVDBShowId), EventLevel.Verbose);
                    List<Mapping> showNameMappings = _configurationManager.ShowNameMappings;
                    if (showNameMappings?.Count > 0)
                    {
                        Mapping mapping = showNameMappings.FirstOrDefault(x => x.TVDBShowID.Equals(tempEp.TVDBShowId));
                        if (mapping != null)
                        {
                            _logger.TraceMessage(string.Format("Edit - Mapping found {0}", mapping.FileShowName), EventLevel.Verbose);
                            ShowEditShowWindow(tempEp, mapping);
                        }
                        else
                        {
                            _logger.TraceMessage("Edit - Mapping could not be found.", EventLevel.Verbose);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.TraceException(ex);
            }
        }

        private void ShowEditShowWindow(MatchedFile tempEp, Mapping mapping)
        {
            try
            {
                string folderPath = Path.Combine(_configurationManager.Settings.DestinationFolderTV, string.IsNullOrWhiteSpace(mapping.CustomFolderName) ? mapping.TVDBShowName : mapping.CustomFolderName);
                _editShowCurrentFolder = mapping.CustomFolderName;
                _editShowTvdbShowName = tempEp.ShowName;
                _editShowTvdbId = tempEp.TVDBShowId;
                EditShowFolderTextBox.Text = folderPath;
                EditFlyout.IsOpen = true;
            }
            catch (Exception ex)
            {
                _logger.TraceException(ex);
            }
        }

        private void EditShowCloseBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                EditFlyout.IsOpen = false;
                bool updateMapping = false;
                string currentText = EditShowFolderTextBox.Text;

                if (_editShowCurrentFolder.Equals(currentText))
                {
                    //if the custom folder name hasn't changed then don't do anything
                }
                else if (_editShowTvdbShowName.Equals(currentText) && string.IsNullOrWhiteSpace(_editShowCurrentFolder))
                {
                    //if the new folder name equals the tvshowname and no customfolder name then dont do anything
                }
                else if (_editShowTvdbShowName.Equals(currentText) && !string.IsNullOrWhiteSpace(_editShowCurrentFolder))
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

                if (updateMapping && _configurationManager.ShowNameMappings?.Count > 0)
                {
                    Mapping mapping = _configurationManager.ShowNameMappings.FirstOrDefault(x => x.TVDBShowID.Equals(_editShowTvdbId));
                    if (mapping != null)
                    {
                        mapping.CustomFolderName = currentText;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.TraceException(ex);
            }
        }

        private void EditShowCloseBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void EditShowFolderTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            string currentText = EditShowFolderTextBox.Text;
            EditShowFolderTextBox.Text = currentText.Replace(_configurationManager.Settings.DestinationFolderTV + @"\", "");
        }

        private void EditShowFolderTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            string currentText = EditShowFolderTextBox.Text;
            EditShowFolderTextBox.Text = Path.Combine(_configurationManager.Settings.DestinationFolderTV, currentText);
        }
    }
}
