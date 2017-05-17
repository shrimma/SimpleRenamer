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
        public ObservableCollection<RegexExpression> regExp;
        private IConfigurationManager configurationManager;
        private IHelper helper;
        private RegexFile originalExpressions;
        public RegexExpressionsWindow(IConfigurationManager configManager, IHelper help)
        {
            configurationManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
            helper = help ?? throw new ArgumentNullException(nameof(help));

            InitializeComponent();
            //grab settings and display
            SetupView();
            this.Closing += Window_Closing;
        }

        private void SetupView()
        {
            regExp = new ObservableCollection<RegexExpression>(configurationManager.RegexExpressions.RegexExpressions);
            ExpressionsListBox.ItemsSource = regExp;
            originalExpressions = new RegexFile()
            {
                RegexExpressions = configurationManager.RegexExpressions.RegexExpressions
            };
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
            {
                //stop the window actually closing
                e.Cancel = true;
                //check if settings have been changed without saving
                var currentExpressions = new List<RegexExpression>(regExp);
                if (helper.AreListsEqual(configurationManager.RegexExpressions.RegexExpressions, currentExpressions) == false)
                {
                    configurationManager.RegexExpressions.RegexExpressions = currentExpressions;
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
            if (helper.AreListsEqual(configurationManager.RegexExpressions.RegexExpressions, originalExpressions.RegexExpressions) == false)
            {
                return true;
            }
            return false;
        }

        private void OkFlyoutButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmationFlyout.IsOpen = false;
            if (helper.AreListsEqual(configurationManager.RegexExpressions.RegexExpressions, originalExpressions.RegexExpressions) == false)
            {
                configurationManager.RegexExpressions.RegexExpressions = originalExpressions.RegexExpressions;
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
            regExp.Add(new RegexExpression("", false, true));
        }

        private void DeleteExpressionButton_Click(object sender, RoutedEventArgs e)
        {
            regExp.Remove((RegexExpression)ExpressionsListBox.SelectedItem);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var currentExpressions = new List<RegexExpression>(regExp);
            if (helper.AreListsEqual(configurationManager.RegexExpressions.RegexExpressions, currentExpressions) == false)
            {
                configurationManager.RegexExpressions.RegexExpressions = currentExpressions;
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
