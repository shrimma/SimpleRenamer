using SimpleRenamer.EventArguments;
using SimpleRenamer.Framework.Interface;
using System;
using System.Windows;

namespace SimpleRenamer.Views
{
    /// <summary>
    /// Interaction logic for AddExtensionsWindow.xaml
    /// </summary>
    public partial class AddExtensionsWindow : Window
    {
        public event EventHandler<ExtensionEventArgs> RaiseCustomEvent;
        private IHelper helper;

        public AddExtensionsWindow(IHelper help)
        {
            if (help == null)
            {
                throw new ArgumentNullException(nameof(help));
            }

            helper = help;
            InitializeComponent();
            this.Closing += AddExtensionsWindow_Closing;
        }

        void AddExtensionsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RaiseCustomEvent(this, new ExtensionEventArgs(null));
            e.Cancel = true;
            this.Hide();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (helper.IsFileExtensionValid(ExtensionTextBox.Text))
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
    }
}
