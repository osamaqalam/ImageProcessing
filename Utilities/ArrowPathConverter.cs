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

            return new LineGeometry(
                new Point(source.X, source.Y),
                new Point(target.X, target.Y)
            );
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
