using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ImageProcessing.App.Utilities
{
    public class ExecutingBorderBrushConverter : IValueConverter
    {
        public Brush ExecutingBrush { get; set; } = Brushes.DarkOrange;
        public Brush NotExecutingBrush { get; set; } = Brushes.Blue;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool isExecuting && isExecuting) ? ExecutingBrush : NotExecutingBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
