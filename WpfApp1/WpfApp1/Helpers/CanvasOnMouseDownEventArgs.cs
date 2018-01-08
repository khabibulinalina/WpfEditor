using System.Windows;
using System.Windows.Input;

namespace WpfApp1.Helpers
{
    internal class CanvasOnMouseDownEventArgs
    {
        public MouseButtonEventArgs MouseArgs { get; set; }
        public Point Position { get; set; }
    }
}
