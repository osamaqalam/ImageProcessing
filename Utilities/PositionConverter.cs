// Utilities/PositionConverter.cs
using System.Globalization;
using System.Windows;
using System.Windows.Data;

public class PositionConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return new Thickness((double)values[0], (double)values[1], 0, 0);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}