// Utilities/PositionConverter.cs
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ImageProcessing.App.Utilities
{

    public class PositionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is double x && values[1] is double y)
            {
                return new Thickness(x, y, 0, 0);
            }
            return new Thickness(0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}