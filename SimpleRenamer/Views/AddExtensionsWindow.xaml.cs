using SimpleRenamer.EventArguments;
using System;
using System.IO;
using System.Windows;

namespace SimpleRenamer
{
    /// <summary>
    /// Interaction logic for AddExtensionsWindow.xaml
    /// </summary>
    public partial class AddExtensionsWindow : Window
    {
        public event EventHandler<ExtensionEventArgs> RaiseCustomEvent;

        public AddExtensionsWindow()
        {
            InitializeComponent();
            this.Closing += AddExtensionsWindow_Closing;
        }

        void AddExtensionsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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
}
