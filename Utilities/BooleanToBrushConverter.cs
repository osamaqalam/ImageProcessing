using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ImageProcessing.App.Utilities
{
    public class BooleanToBrushConverter : IValueConverter
    {
        public Brush TrueBrush { get; set; } = Brushes.Blue;
        public Brush FalseBrush { get; set; } = Brushes.Black;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool isSelected && isSelected) ? TrueBrush : FalseBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
