
using SimpleRenamer.Common.Interface;
using SimpleRenamer.EventArguments;
using System;
using System.Windows;
using System.Windows.Input;

namespace SimpleRenamer.Views
{
    /// <summary>
    /// Interaction logic for AddExtensionsWindow.xaml
    /// </summary>
    public partial class AddExtensionsWindow
    {
        public event EventHandler<ExtensionEventArgs> RaiseCustomEvent;
        private IHelper helper;
        private bool flyoutEnabled;

        public AddExtensionsWindow(IHelper help)
        {
            if (help == null)
            {
                throw new ArgumentNullException(nameof(help));
            }

            helper = help;
            InitializeComponent();
            flyoutEnabled = true;
            ExtensionTextBox.Focus();
            this.Closing += AddExtensionsWindow_Closing;
        }

        void AddExtensionsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RaiseCustomEvent(this, new ExtensionEventArgs(null));
            e.Cancel = true;
            this.ExtensionTextBox.Text = string.Empty;
            ExtensionTextBox.Focus();
            this.Hide();
        }



        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            SaveInputExtension(ExtensionTextBox.Text);
        }

        private void SaveInputExtension(string extension)
        {
            if (helper.IsFileExtensionValid(ExtensionTextBox.Text))
            {
                RaiseCustomEvent(this, new ExtensionEventArgs(ExtensionTextBox.Text));
                this.ExtensionTextBox.Text = string.Empty;
                this.ExtensionTextBox.Focus();
                this.Hide();
            }
            else
            {
                ErrorFlyout.IsOpen = true;
                this.OKButton.IsEnabled = false;
                this.ExtensionTextBox.IsEnabled = false;
                this.flyoutEnabled = false;
            }
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveInputExtension(e.Parameter.ToString());
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = flyoutEnabled;
        }

        private void CloseBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ErrorFlyout.IsOpen = false;
            this.OKButton.IsEnabled = true;
            this.ExtensionTextBox.IsEnabled = true;
            this.flyoutEnabled = true;
        }

        private void CloseBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseCustomEvent(this, new ExtensionEventArgs(null));
            //clear the text
            this.ExtensionTextBox.Text = string.Empty;
            this.ExtensionTextBox.Focus();
            this.Hide();
        }
    }
}
