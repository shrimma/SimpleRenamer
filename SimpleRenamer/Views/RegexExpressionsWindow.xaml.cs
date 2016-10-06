using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace SimpleRenamer.Views
{
    /// <summary>
    /// Interaction logic for RegexExpressions.xaml
    /// </summary>
    public partial class RegexExpressionsWindow : Window
    {
        public ObservableCollection<RegexExpression> regExp;
        private IConfigurationManager configurationManager;
        public RegexExpressionsWindow(IConfigurationManager configManager)
        {
            if (configManager == null)
            {
                throw new ArgumentNullException(nameof(configManager));
            }
            configurationManager = configManager;

            InitializeComponent();
            regExp = new ObservableCollection<RegexExpression>(configurationManager.RegexExpressions.RegexExpressions);
            ExpressionsListBox.ItemsSource = regExp;
        }

        private void AddExpressionButton_Click(object sender, RoutedEventArgs e)
        {
            regExp.Add(new RegexExpression("", false, true));
        }

        private void DeleteExpressionButton_Click(object sender, RoutedEventArgs e)
        {
            regExp.Remove((RegexExpression)ExpressionsListBox.SelectedItem);
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            configurationManager.RegexExpressions.RegexExpressions = new List<RegexExpression>(regExp);
            this.Hide();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
