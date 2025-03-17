using ImageProcessing.App.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ImageProcessing.App.Utilities
{
    public class ArrowPathConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2 ||
                !(values[0] is FlowchartNodeViewModel source) ||
                !(values[1] is FlowchartNodeViewModel target))
                return Geometry.Empty;

            // Calculate connection points considering node dimensions
            var start = new Point(source.X, source.Y + source.Height / 2);
            var end = new Point(target.X, target.Y - target.Height / 2);

            return new LineGeometry(start, end);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
