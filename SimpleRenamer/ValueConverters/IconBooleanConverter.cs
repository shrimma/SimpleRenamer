using SimpleRenamer.Framework.DataModel;
using System;
using System.Windows.Data;

namespace SimpleRenamer
{
    public class IconFileTypeConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.GetType() != typeof(FileType))
            {
                throw new ArgumentException("Source type must be FileType");
            }

            FileType temp = (FileType)value;

            if (temp == FileType.TvShow)
            {
                return "/Images/tv.png";
            }
            else if (temp == FileType.Movie)
            {
                return "/Images/movie.png";
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}
