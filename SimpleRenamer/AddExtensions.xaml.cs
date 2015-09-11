using System;
using System.IO;
using System.Windows;

namespace SimpleRenamer
{
    /// <summary>
    /// Interaction logic for AddExtensions.xaml
    /// </summary>
    public partial class AddExtensions : Window
    {
        public event EventHandler<ExtensionEventArgs> RaiseCustomEvent;

        public AddExtensions()
        {
            InitializeComponent();
            this.Closing += SelectShowWpfForm_Closing;
        }

        void SelectShowWpfForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RaiseCustomEvent(this, new ExtensionEventArgs(null));
            this.Hide();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsFileExtensionValid(ExtensionTextBox.Text))
            {
                RaiseCustomEvent(this, new ExtensionEventArgs(ExtensionTextBox.Text));
                this.Hide();
            }
            else
            {
                MessageBox.Show("Not a valid file extension.", "Error", MessageBoxButton.OK);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseCustomEvent(this, new ExtensionEventArgs(null));
            this.Hide();
        }

        private bool IsFileExtensionValid(string fExt)
        {
            bool answer = true;
            if (!String.IsNullOrWhiteSpace(fExt) && fExt.Length > 1 && fExt[0] == '.')
            {
                char[] invalidFileChars = Path.GetInvalidFileNameChars();
                foreach (char c in invalidFileChars)
                {
                    if (fExt.Contains(c.ToString()))
                    {
                        answer = false;
                        break;
                    }
                }
            }
            else
            {
                answer = false;
            }
            return answer;
        }
    }

    public class ExtensionEventArgs : EventArgs
    {
        public ExtensionEventArgs(string s)
        {
            extension = s;
        }
        private string extension;
        public string Extension
        {
            get { return extension; }
        }
    }
}
