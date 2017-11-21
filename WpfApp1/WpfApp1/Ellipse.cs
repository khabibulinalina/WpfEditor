using System.Windows;
using System.Windows.Media;

namespace WpfApp1
{
    internal class Ellipse : Figure
    {
        protected override Geometry DefiningGeometry
        {
            get { return new EllipseGeometry(Center, RadiusX, RadiusY); }
        }

        public Point Center { get; set; }
        public double RadiusX { get; set; }
        public double RadiusY { get; set; }

        public Ellipse(double lineWidth, Brush lineColor, Brush backColor, LineType lineType) : base(lineWidth, lineColor, backColor, lineType)
        {
        }
    }
}