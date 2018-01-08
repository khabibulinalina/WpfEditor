using System.Windows.Shapes;

namespace WpfApp1.Helpers
{
    static class ShapesExtensions
    {
        public static System.Windows.Shapes.Ellipse Clone(this System.Windows.Shapes.Ellipse circle)
        {
            return new System.Windows.Shapes.Ellipse
            {
                StrokeThickness = circle.StrokeThickness,
                Stroke = circle.Stroke,
                Fill = circle.Fill,
                StrokeDashArray = circle.StrokeDashArray,
                Width = circle.Width,
                Height = circle.Height
            };
        }

        public static System.Windows.Shapes.Line Clone(this System.Windows.Shapes.Line line)
        {
            return new System.Windows.Shapes.Line
            {
                X1 = line.X1,
                X2 = line.X2,
                Y1 = line.Y1,
                Y2 = line.Y2,
                StrokeThickness = line.StrokeThickness,
                Stroke = line.Stroke,
                StrokeDashArray = line.StrokeDashArray
            };
        }

        public static System.Windows.Shapes.Polyline Clone(this System.Windows.Shapes.Polyline polyline)
        {
            return new System.Windows.Shapes.Polyline
            {
                Points = polyline.Points,
                StrokeThickness = polyline.StrokeThickness,
                Stroke = polyline.Stroke,
                StrokeDashArray = polyline.StrokeDashArray
            };
        }

        public static Polygon Clone(this Polygon polygon)
        {
            return new Polygon
            {
                Points = polygon.Points,
                StrokeDashArray = polygon.StrokeDashArray,
                Fill = polygon.Fill,
                Stroke = polygon.Stroke,
                StrokeThickness = polygon.StrokeThickness
            };
        }
    }
}
