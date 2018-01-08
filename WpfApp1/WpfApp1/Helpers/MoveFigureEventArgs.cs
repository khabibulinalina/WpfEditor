using System.Windows;
using System.Windows.Shapes;

namespace WpfApp1.Helpers
{
    internal class MoveFigureEventArgs
    {
        public Shape Figure { get; set; }
        public Point Position { get; set; }
    }
}
