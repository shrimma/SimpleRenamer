using SimpleRenamer.Framework;
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
        public RegexExpressions()
        {
            InitializeComponent();
            currentRegex = FileMatcher.ReadExpressionFile();
            oldRegex = FileMatcher.ReadExpressionFile();
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            currentRegex.RegexExpressions = new List<RegexExpression>(regExp);
            FileMatcher.WriteExpressionFile(currentRegex);
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
