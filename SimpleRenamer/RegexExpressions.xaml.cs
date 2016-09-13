using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace SimpleRenamer
{
    /// <summary>
    /// Interaction logic for RegexExpressions.xaml
    /// </summary>
    public partial class RegexExpressions : Window
    {
        private RegexFile currentRegex;
        private RegexFile oldRegex;
        public ObservableCollection<RegexExpression> regExp;
        private IFileMatcher fileMatcher;
        public RegexExpressions(IFileMatcher fileMatch)
        {
            fileMatcher = fileMatch;
        }

        public RegexExpressions()
        {
            InitializeComponent();
            currentRegex = fileMatcher.ReadExpressionFileAsync().GetAwaiter().GetResult();
            oldRegex = fileMatcher.ReadExpressionFileAsync().GetAwaiter().GetResult();
            regExp = new ObservableCollection<RegexExpression>(currentRegex.RegexExpressions);
            ExpressionsListBox.ItemsSource = regExp;
        }

        private void AddExpressionButton_Click(object sender, RoutedEventArgs e)
        {
            regExp.Add(new RegexExpression("", false));
        }

        private void DeleteExpressionButton_Click(object sender, RoutedEventArgs e)
        {
            regExp.Remove((RegexExpression)ExpressionsListBox.SelectedItem);
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            currentRegex.RegexExpressions = new List<RegexExpression>(regExp);
            await fileMatcher.WriteExpressionFileAsync(currentRegex);
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
