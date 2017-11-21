using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace WpfApp1
{
    class Polygone : Figure
    {
        public Polygone(double lineWidth, Brush lineColor, Brush backColor, LineType lineType) : base(lineWidth, lineColor, backColor, lineType)
        {
        }

        protected override Geometry DefiningGeometry
        {
            get { return new LineGeometry(StartPoint, EndPoint); }
        }

        public List<LineSegment> Lines { get; set; }

        public Point StartPoint { get; set; }

        public Point EndPoint { get; set; }
    }
}