using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1.Mvvm
{
    public class ChangeLineTypesCommand : ICommand
    {
        private readonly Shape _shape;
        private readonly DoubleCollection _doubleCollection;
        private DoubleCollection _prevDoubleCollection;

        public ChangeLineTypesCommand(Shape shape, DoubleCollection doubleCollection)
        {
            _shape = shape;
            _doubleCollection = doubleCollection;
        }

        public string Name => "Change doubleCollection";

        public void Execute()
        {
            //запоминаем предыдущий цвет
            _prevDoubleCollection = _shape.StrokeDashArray;
            //присваиваем новый цвет
            _shape.StrokeDashArray = _doubleCollection;
            //сигнализируем об изменениях
            _shape.InvalidateVisual();
        }

        public void UnExecute()
        {
            //возвращаем предыдущий цвет
            _shape.StrokeDashArray = _prevDoubleCollection;
            //сигнализируем об изменениях
            _shape.InvalidateVisual();
        }
    }
}
