using SimpleRenamer.EventArguments;
using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System;
using System.IO;
using System.Windows;

namespace SimpleRenamer.Views
{
    /// <summary>
    /// Interaction logic for EditShowWindow.xaml
    /// </summary>
    public partial class EditShowWindow : Window
    {
        private Settings currentSettings;
        private MatchedFile currentEpisode;
        private Mapping currentMapping;
        public event EventHandler<EditShowEventArgs> RaiseEditShowEvent;

        public EditShowWindow(IConfigurationManager configManager)
        {
            if (configManager == null)
            {
                throw new ArgumentNullException(nameof(configManager));
            }

            InitializeComponent();
            currentSettings = configManager.Settings;
            this.Closing += EditShowWindow_Closing;
        }

        public void SetCurrentShow(MatchedFile tvEp, Mapping mapping)
        {
            currentEpisode = tvEp;
            currentMapping = mapping;
            this.Title = string.Format("{0}", currentMapping.TVDBShowName);
            string folderPath = Path.Combine(currentSettings.DestinationFolderTV, string.IsNullOrEmpty(currentMapping.CustomFolderName) ? currentMapping.TVDBShowName : currentMapping.CustomFolderName);
            ShowFolderTextBox.Text = folderPath;

        }

        private void EditShowWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RaiseEditShowEvent(this, new EditShowEventArgs(null, null));
            this.Hide();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string currentText = ShowFolderTextBox.Text;
            currentText = currentText.Replace(currentSettings.DestinationFolderTV + @"\", "");
            if (currentMapping.CustomFolderName.Equals(currentText))
            {
                //if the custom folder name hasn't changed then don't raise
                RaiseEditShowEvent(this, new EditShowEventArgs(null, null));
            }
            else if (currentMapping.TVDBShowName.Equals(currentText) && string.IsNullOrEmpty(currentMapping.CustomFolderName))
            {
                //if the new folder name equals the tvshowname and no customfolder name then dont raise
                RaiseEditShowEvent(this, new EditShowEventArgs(null, null));
            }
            else if (currentMapping.TVDBShowName.Equals(currentText) && !string.IsNullOrEmpty(currentMapping.CustomFolderName))
            {
                //if the new folder name equals the tvshowname and there is a customfoldername already then reset customfoldername to blank
                RaiseEditShowEvent(this, new EditShowEventArgs(currentMapping, string.Empty));
            }
            else
            {
                RaiseEditShowEvent(this, new EditShowEventArgs(currentMapping, currentText));
            }
            this.Hide();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEditShowEvent(this, new EditShowEventArgs(null, null));
            this.Hide();
        }

        private void ShowFolderTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            string currentText = ShowFolderTextBox.Text;
            ShowFolderTextBox.Text = currentText.Replace(currentSettings.DestinationFolderTV + @"\", "");
        }

        private void ShowFolderTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            string currentText = ShowFolderTextBox.Text;
            ShowFolderTextBox.Text = Path.Combine(currentSettings.DestinationFolderTV, currentText);
        }
    }
}
