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

            double sourceBottom = source.Y + source.Height / 2;
            double targetTop = target.Y - target.Height / 2;

            // Main line
            // Calculate connection points considering node dimensions
            if ((parameter as string) == "Line")
            {
                return new LineGeometry(
                    new Point(source.X, sourceBottom),
                    new Point(target.X, targetTop)
                );
            }

            // Arrowhead (triangle)
            else if ((parameter as string) == "Triangle")
            {
                double arrowSize = 10;
                // Calculate exact midpoint between source bottom and target top
                var end = new Point(target.X, (sourceBottom + targetTop + arrowSize) / 2);
                var left = new Point(end.X - arrowSize / 2, end.Y - arrowSize);
                var right = new Point(end.X + arrowSize / 2, end.Y - arrowSize);

                var triangle = new PathGeometry();
                var figure = new PathFigure { StartPoint = end, IsClosed = true };
                figure.Segments.Add(new LineSegment(left, true));
                figure.Segments.Add(new LineSegment(right, true));
                triangle.Figures.Add(figure);
                return triangle;
            }
            return Geometry.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
