using System.Windows;
using System.Windows.Media;

namespace WpfApp1
{
    internal class Circle:Figure
    {
        protected override Geometry DefiningGeometry {
            get { return new EllipseGeometry(Center, Radius, Radius); }
        }

        public Point Center { get; set; }
        public double Radius { get; set; }

        public Circle(double lineWidth, Brush lineColor, Brush backColor, LineType lineType) : base(lineWidth, lineColor, backColor, lineType)
        {
        }
    }
}
