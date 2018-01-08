using System.Windows;
using System.Windows.Input;

namespace WpfApp1.Helpers
{
    internal class CanvasOnMouseUpEventArgs
    {
        public MouseEventArgs MouseArgs { get; set; }
        public Point Position { get; set; }
    }
}
