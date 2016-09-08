using SimpleRenamer.Framework.DataModel;
using System;
using System.IO;
using System.Windows;

namespace SimpleRenamer
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class EditShowWindow : Window
    {
        private Settings currentSettings;
        private TVEpisode currentEpisode;
        private Mapping currentMapping;
        public event EventHandler<EditShowEventArgs> RaiseCustomEvent;

        public EditShowWindow()
        {
            InitializeComponent();
            this.Closing += EditShowWpfForm_Closing;
        }

        public EditShowWindow(Settings settings, TVEpisode tvEpisode, Mapping mapping)
        {
            InitializeComponent();
            currentSettings = settings;
            currentEpisode = tvEpisode;
            currentMapping = mapping;

            this.Title = string.Format("{0}", currentMapping.TVDBShowName);
            string folderPath = Path.Combine(currentSettings.DestinationFolder, string.IsNullOrEmpty(currentMapping.CustomFolderName) ? currentMapping.TVDBShowName : currentMapping.CustomFolderName);
            ShowFolderTextBox.Text = folderPath;
            this.Closing += EditShowWpfForm_Closing;
        }

        void EditShowWpfForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RaiseCustomEvent(this, new EditShowEventArgs(null, null));
            this.Hide();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string currentText = ShowFolderTextBox.Text;
            currentText = currentText.Replace(currentSettings.DestinationFolder + @"\", "");
            if (currentMapping.CustomFolderName.Equals(currentText))
            {
                //if the custom folder name hasn't changed then don't raise
                RaiseCustomEvent(this, new EditShowEventArgs(null, null));
            }
            else if (currentMapping.TVDBShowName.Equals(currentText) && string.IsNullOrEmpty(currentMapping.CustomFolderName))
            {
                //if the new folder name equals the tvshowname and no customfolder name then dont raise
                RaiseCustomEvent(this, new EditShowEventArgs(null, null));
            }
            else if (currentMapping.TVDBShowName.Equals(currentText) && !string.IsNullOrEmpty(currentMapping.CustomFolderName))
            {
                //if the new folder name equals the tvshowname and there is a customfoldername already then reset customfoldername to blank
                RaiseCustomEvent(this, new EditShowEventArgs(currentMapping, string.Empty));
            }
            else
            {
                RaiseCustomEvent(this, new EditShowEventArgs(currentMapping, currentText));
            }
            this.Hide();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseCustomEvent(this, new EditShowEventArgs(null, null));
            this.Hide();
        }

        private void ShowFolderTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            string currentText = ShowFolderTextBox.Text;
            ShowFolderTextBox.Text = currentText.Replace(currentSettings.DestinationFolder + @"\", "");
        }

        private void ShowFolderTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            string currentText = ShowFolderTextBox.Text;
            ShowFolderTextBox.Text = Path.Combine(currentSettings.DestinationFolder, currentText);
        }
    }

    public class EditShowEventArgs : EventArgs
    {
        public EditShowEventArgs(Mapping m, string newFolderName)
        {
            mapping = m;
            newFolder = newFolderName;
        }
        private Mapping mapping;
        public Mapping Mapping
        {
            get { return mapping; }
        }

        private string newFolder;
        public string NewFolder
        {
            get { return newFolder; }
        }
    }
}
