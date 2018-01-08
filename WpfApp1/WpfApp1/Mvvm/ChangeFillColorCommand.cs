using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1.Mvvm
{
    public class ChangeFillColorCommand : ICommand
    {
        private readonly Shape _shape;
        private readonly Brush _color;
        private Brush _prevColor;

        public ChangeFillColorCommand(Shape shape, Brush color)
        {
            _shape = shape;
            _color = color;
        }

        public string Name => "Change color";

        public void Execute()
        {
            //запоминаем предыдущий цвет
            _prevColor = _shape.Fill;
            //присваиваем новый цвет
            _shape.Fill = _color;
            //сигнализируем об изменениях
            _shape.InvalidateVisual();
        }

        public void UnExecute()
        {
            //возвращаем предыдущий цвет
            _shape.Fill = _prevColor;
            //сигнализируем об изменениях
            _shape.InvalidateVisual();
        }
    }
}
