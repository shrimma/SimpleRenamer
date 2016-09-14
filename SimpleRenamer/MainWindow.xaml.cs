using SimpleRenamer.Framework;
using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
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
        private ITVShowMatcher tvShowMatcher;
        private IIgnoreListFramework ignoreListFramework;
        private IDependencyInjectionContext injectionContext;
        private IScanForShows scanForShows;
        private IPerformActionsOnShows performActionsOnShows;
        private Settings settings;

        public MainWindow(ILogger log, ITVShowMatcher tvShowMatch, ISettingsFactory settingsFactory, IIgnoreListFramework ignore, IDependencyInjectionContext injection, IPerformActionsOnShows performActions, IScanForShows scanShows)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            if (tvShowMatch == null)
            {
                throw new ArgumentNullException(nameof(tvShowMatch));
            }
            if (settingsFactory == null)
            {
                throw new ArgumentNullException(nameof(settingsFactory));
            }
            if (ignore == null)
            {
                throw new ArgumentNullException(nameof(ignore));
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
            logger = log;
            tvShowMatcher = tvShowMatch;
            ignoreListFramework = ignore;
            injectionContext = injection;
            scanForShows = scanShows;
            performActionsOnShows = performActions;

            try
            {
                InitializeComponent();
                logger.TraceMessage("Starting Application");
                settings = settingsFactory.GetSettings();
                scannedEpisodes = new ObservableCollection<TVEpisode>();
                ShowsListBox.ItemsSource = scannedEpisodes;

                //setup the perform actions event handlers
                performActionsOnShows.RaiseFileMovedEvent += PerformActionsOnShows_RaiseFileMovedEvent;
                performActionsOnShows.RaiseFilePreProcessedEvent += PerformActionsOnShows_RaiseFilePreProcessedEvent;

                this.Closing += MainWindow_Closing;
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        private void PerformActionsOnShows_RaiseFilePreProcessedEvent(object sender, FilePreProcessedEventArgs e)
        {
            //TODO modify the progress bar here
        }

        private void PerformActionsOnShows_RaiseFileMovedEvent(object sender, FileMovedEventArgs e)
        {
            //TODO modify the progress bar here!
            scannedEpisodes.Remove(e.Episode);
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                logger.TraceMessage("Closing");
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
                logger.TraceMessage(string.Format("Starting"));
                scannedEpisodes = new ObservableCollection<TVEpisode>(await scanForShows.Scan(cts.Token));
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

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //show the settings window
                SettingsWindow settingsWindow = injectionContext.GetService<SettingsWindow>();
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
                logger.TraceMessage(string.Format("Starting"));
                FileMoveProgressBar.Value = 0;
                FileMoveProgressBar.Maximum = (scannedEpisodes.Where(x => x.ActionThis == true).Count()) * 2;
                await performActionsOnShows.Action(scannedEpisodes, cts.Token);
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
                RunButton.IsEnabled = true;
                SettingsButton.IsEnabled = true;
                CancelButton.IsEnabled = false;
                ButtonsStackPanel.Visibility = System.Windows.Visibility.Visible;
                ProgressStackPanel.Visibility = System.Windows.Visibility.Collapsed;
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

        private async void IgnoreShowButton_Click(object sender, RoutedEventArgs e)
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
                logger.TraceMessage("Show Detail button clicked");
                TVEpisode tempEp = (TVEpisode)ShowsListBox.SelectedItem;
                logger.TraceMessage(string.Format("For show {0}, season {1}, episode {2}, TVDBShowId {3}", tempEp.ShowName, tempEp.Season, tempEp.Episode, tempEp.TVDBShowId));
                ShowDetailsForm sdf = new ShowDetailsForm(tempEp.TVDBShowId);
                sdf.ShowDialog();
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                logger.TraceMessage("Edit button clicked");
                TVEpisode tempEp = (TVEpisode)ShowsListBox.SelectedItem;
                logger.TraceMessage(string.Format("For show {0}, season {1}, episode {2}, TVDBShowId {3}", tempEp.ShowName, tempEp.Season, tempEp.Episode, tempEp.TVDBShowId));
                ShowNameMapping snm = await tvShowMatcher.ReadMappingFileAsync();
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
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        private void ShowEditShowWindow(TVEpisode tempEp, Mapping mapping)
        {
            try
            {
                EditShowWindow esw = new EditShowWindow(settings, tempEp, mapping);
                esw.RaiseCustomEvent += new EventHandler<EditShowEventArgs>(EditShowWindowClosedEvent);
                esw.ShowDialog();
            }
            catch (Exception ex)
            {
                logger.TraceException(ex);
            }
        }

        public async void EditShowWindowClosedEvent(object sender, EditShowEventArgs e)
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
