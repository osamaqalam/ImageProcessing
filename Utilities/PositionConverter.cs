// Utilities/PositionConverter.cs

using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ImageProcessing.App.Utilities
{
    /// <summary>
    /// Used for converting the margin from double x, double y to Thickness object
    /// </summary>
    public class PositionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 4
                && values[0] is double x
                && values[1] is double y
                && values[2] is double width
                && values[3] is double height)
            {
                // Center the node by subtracting half its dimensions
                return new Thickness(x - width / 2, y - height / 2, 0, 0);
            }
            return new Thickness(0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}