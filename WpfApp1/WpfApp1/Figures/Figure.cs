using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1
{
    internal abstract class Figure : Shape
    {
        private double LineWidth
        {
            get { return StrokeThickness; }
            set { StrokeThickness = value; }
        }

        private Brush LineColor
        {
            get { return Stroke; }
            set { Stroke = value; }
        }

        private Brush BackColor
        {
            get { return Fill; }
            set { Fill = value; }
        }

        private LineType LineType
        {
            get { return LineType; }
            set
            {
                switch (value)
                {
                    case LineType.Solid:
                        StrokeDashArray = new DoubleCollection {100000};
                        break;
                    case LineType.Dashed:
                        StrokeDashArray = new DoubleCollection {10};
                        break;
                    case LineType.Dotted:
                        StrokeDashArray = new DoubleCollection {1};
                        break;
                }
            }
        }

        protected Figure(double lineWidth, Brush lineColor, Brush backColor, LineType lineType)
        {
            LineWidth = lineWidth;
            LineColor = lineColor;
            BackColor = backColor;
            LineType = lineType;
        }
    }
}