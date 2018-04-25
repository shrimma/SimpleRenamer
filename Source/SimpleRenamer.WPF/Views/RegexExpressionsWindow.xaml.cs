using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Sarjee.SimpleRenamer.Views
{
    /// <summary>
    /// Interaction logic for RegexExpressions.xaml
    /// </summary>
    public partial class RegexExpressionsWindow
    {
        public ObservableCollection<RegexExpression> RegularExpressions;
        private IConfigurationManager _configurationManager;
        private IHelper _helper;
        private List<RegexExpression> _originalExpressions;
        public RegexExpressionsWindow(IConfigurationManager configurationManager, IHelper helper)
        {
            _configurationManager = configurationManager ?? throw new ArgumentNullException(nameof(configurationManager));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));

            InitializeComponent();
            //grab settings and display
            SetupView();
            this.Closing += Window_Closing;
        }

        private void SetupView()
        {
            RegularExpressions = new ObservableCollection<RegexExpression>(_configurationManager.RegexExpressions);
            ExpressionsListBox.ItemsSource = RegularExpressions;
            _originalExpressions = _configurationManager.RegexExpressions;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
            {
                //stop the window actually closing
                e.Cancel = true;
                //check if settings have been changed without saving
                var currentExpressions = new List<RegexExpression>(RegularExpressions);
                if (_helper.AreListsEqual(_configurationManager.RegexExpressions, currentExpressions) == false)
                {
                    _configurationManager.RegexExpressions = currentExpressions;
                }
                if (HaveSettingsChanged() == true)
                {
                    //if settings have been changed and not saved then prompt user
                    ConfirmationFlyout.IsOpen = true;
                }
                else
                {
                    SetupView();
                    this.Hide();
                }
            }
        }

        private bool HaveSettingsChanged()
        {
            if (_helper.AreListsEqual(_configurationManager.RegexExpressions, _originalExpressions) == false)
            {
                return true;
            }
            return false;
        }

        private void OkFlyoutButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmationFlyout.IsOpen = false;
            if (_helper.AreListsEqual(_configurationManager.RegexExpressions, _originalExpressions) == false)
            {
                _configurationManager.RegexExpressions = _originalExpressions;
            }
            SetupView();
            this.Hide();
        }

        private void CancelFlyoutButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmationFlyout.IsOpen = false;
        }

        private void AddExpressionButton_Click(object sender, RoutedEventArgs e)
        {
            RegularExpressions.Add(new RegexExpression("Enter Expression Here", false, true));
        }

        private void DeleteExpressionButton_Click(object sender, RoutedEventArgs e)
        {
            RegularExpressions.Remove((RegexExpression)ExpressionsListBox.SelectedItem);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var currentExpressions = new List<RegexExpression>(RegularExpressions);
            if (_helper.AreListsEqual(_configurationManager.RegexExpressions, currentExpressions) == false)
            {
                _configurationManager.RegexExpressions = currentExpressions;
            }
            SetupView();
            this.Hide();
        }

        private void ExpressionsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            RegexExpression value = (RegexExpression)ExpressionsListBox.SelectedItem;
            if (value != null)
            {
                this.DeleteExpressionButton.IsEnabled = true;
            }
            else
            {
                this.DeleteExpressionButton.IsEnabled = false;
            }
        }
    }
}
