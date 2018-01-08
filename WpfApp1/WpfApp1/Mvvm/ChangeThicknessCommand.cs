using System.Windows.Shapes;

namespace WpfApp1.Mvvm
{
    public class ChangeThicknessCommand : ICommand
    {
        private readonly Shape _shape;
        private readonly double _thickness;
        private double _prevThickness;

        public ChangeThicknessCommand(Shape shape, double thickness)
        {
            _shape = shape;
            _thickness = thickness;
        }

        public string Name => "Change thickness";

        public void Execute()
        {
            //запоминаем предыдущий цвет
            _prevThickness = _shape.StrokeThickness;
            //присваиваем новый цвет
            _shape.StrokeThickness = _thickness;
            //сигнализируем об изменениях
            _shape.InvalidateVisual();
        }

        public void UnExecute()
        {
            //возвращаем предыдущий цвет
            _shape.StrokeThickness = _prevThickness;
            //сигнализируем об изменениях
            _shape.InvalidateVisual();
        }
    }
}