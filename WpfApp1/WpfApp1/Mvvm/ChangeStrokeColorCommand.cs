using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1.Mvvm
{
    public class ChangeStrokeColorCommand : ICommand
    {
        private readonly Shape _shape;
        private readonly Brush _color;
        private Brush _prevColor;

        public ChangeStrokeColorCommand(Shape shape, Brush color)
        {
            _shape = shape;
            _color = color;
        }

        public string Name => "Change color";

        public void Execute()
        {
            //запоминаем предыдущий цвет
            _prevColor = _shape.Stroke;
            //присваиваем новый цвет
            _shape.Stroke = _color;
            //сигнализируем об изменениях
            _shape.InvalidateVisual();
        }

        public void UnExecute()
        {
            //возвращаем предыдущий цвет
            _shape.Stroke = _prevColor;
            //сигнализируем об изменениях
            _shape.InvalidateVisual();
        }
    }
}