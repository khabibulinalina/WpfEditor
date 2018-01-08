using System.Windows;
using System.Windows.Shapes;

namespace WpfApp1.Helpers
{
    internal class MoveFigureByTwoPointsEventArgs
    {
        public Shape Figure { get; set; }
        public Point LeftTopPoint { get; set; }
        public Point RightBottomPoint { get; set; }
    }
}
