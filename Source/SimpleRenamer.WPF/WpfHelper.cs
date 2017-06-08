using System;
using System.Windows;
using System.Windows.Controls;

namespace Sarjee.SimpleRenamer.WPF
{
    public static class WpfHelper
    {
        public static void UpdateColumnsWidth(ListView listView)
        {
            int autoFillColumnIndex = (listView.View as GridView).Columns.Count - 1;
            if (Double.IsNaN(listView.ActualWidth))
            {
                listView.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            }
            double remainingSpace = listView.ActualWidth;
            for (int i = 0; i < (listView.View as GridView).Columns.Count; i++)
            {
                if (i != autoFillColumnIndex)
                {
                    remainingSpace -= (listView.View as GridView).Columns[i].ActualWidth;
                }
            }
            (listView.View as GridView).Columns[autoFillColumnIndex].Width = remainingSpace >= 0 ? remainingSpace : 0;
        }
    }
}
