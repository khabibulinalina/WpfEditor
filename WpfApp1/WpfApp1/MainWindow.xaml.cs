using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public ObservableCollection<string> Figures { get; set; }
        public ObservableCollection<string> LineTypes { get; set; }
        private Shape _currentFigure;
        private Point _polygoneStartPoint;
        private List<Point> _polygonPoints = new List<Point>();

        private Brush _currentLineColor;
        private Brush _currentBackColor;
        private double _currentThickness;
        private LineType _currentLineType;

        public MainWindow()
        {
            DataContext = this;
            Figures = new ObservableCollection<string>
            {
                "линия",
                "окружность",
                "полилиния",
                "многоугольник",
                "эллипс"
            };
            LineTypes = new ObservableCollection<string>
            {
                "сплошная",
                "пунктирная",
                "точечная"
            };

            InitializeComponent();  //загружает интерфейс
            LineColorPicker.SelectedColor=Color.FromRgb(255,111,150);
            BackColorPicker.SelectedColor = Color.FromRgb(255, 255, 0);

            Canvas.MouseDown += CanvasOnMouseDown; //подписка на события нажатия и перемещения мыши
            Canvas.MouseMove += CanvasOnMouseMove;
        }

        private void CanvasOnMouseMove(object sender, MouseEventArgs args)
        {
            if (_currentFigure == null)// если никакая фигура не выбрана, то не выполнять метод
            {
                return;
            }

            if (_currentFigure is Polygone polygone)//если во время перемещения мыши выбран многоугольник, топродолжать его рисовать
            {
                polygone.EndPoint = args.GetPosition(Canvas);
                polygone.InvalidateVisual();
            }

            if (_currentFigure is Polyline polyline)//если во время перемещения мыши выбрана полилиния, топродолжать ее рисовать
            {
                polyline.EndPoint = args.GetPosition(Canvas);
                polyline.InvalidateVisual();
            }

            if (args.LeftButton == MouseButtonState.Pressed)//если нажата левая кнопка мыши, то выполнять следующее
            {
                if (_currentFigure is Line line)//если текущая фигура - линия, то рисовать ее
                {
                    line.EndPoint = args.GetPosition(Canvas);
                    line.InvalidateVisual(); //обновляет фигуру
                } else if (_currentFigure is Circle circle)//если текущая фигура - окружность, то рисовать ее
                {
                    circle.Radius = args.GetPosition(Canvas).X - circle.Center.X;
                    circle.InvalidateVisual();
                } else if (_currentFigure is Ellipse ellipse)//если текущая фигура - эллипс, то рисовать его
                {
                    ellipse.RadiusX = args.GetPosition(Canvas).X - ellipse.Center.X;
                    ellipse.RadiusY = args.GetPosition(Canvas).Y - ellipse.Center.Y;
                    ellipse.InvalidateVisual();
                }
            }
        }

        private void CanvasOnMouseDown(object sender, MouseButtonEventArgs args)
        {
           ;
            if (args.ChangedButton == MouseButton.Right)//если была нажата правая кнопка мыши, то произойдет следующее:
            {
                if (_currentFigure is Polygone polygone)// если текущая фигура - многоугольник, то завершить его рисовать(обнулить текущую фигуру)
                {

                    polygone.EndPoint = _polygoneStartPoint;
                    polygone.InvalidateVisual();
                    var p = new Polygon
                    {
                        Points = new PointCollection(_polygonPoints)
                    };
                    p.Fill = _currentBackColor;
                    Canvas.Children.Add(p);
                    Canvas.InvalidateVisual();
                    _polygonPoints.Clear();
                    _currentFigure = null;
                }
                if (_currentFigure is Polyline)// если текущая фигура - полилиния, то завершить ее рисовать(обнулить текущую фигуру)
                {
                    _currentFigure = null;
                }
                return;
            }

            if (args.ChangedButton == MouseButton.Left)//если была нажата левая кнопка мыши, то выполнять следующее
            {
                if (_currentFigure is Polygone)//если текущая фигура - многоугольник, то закончить рисовать его грань и начать рисовать новую 
                {
                    _currentFigure = new Polygone(_currentThickness, _currentLineColor, _currentBackColor, _currentLineType)
                    {
                        StartPoint = args.GetPosition(Canvas),
                        EndPoint = args.GetPosition(Canvas),
                    };
                    _polygonPoints.Add(args.GetPosition(Canvas));
                    Canvas.Children.Add(_currentFigure);
                    return;
                }

                if (_currentFigure is Polyline)//если текущая фигура - полилиния, то закончить ее рисовать и начать рисовать новую 
                {
                    _currentFigure = new Polyline(_currentThickness, _currentLineColor, _currentBackColor, _currentLineType)
                    {
                        StartPoint = args.GetPosition(Canvas),
                        EndPoint = args.GetPosition(Canvas),
                    };
                    Canvas.Children.Add(_currentFigure);
                    return;
                }

                switch (FiguresComboBox.SelectedIndex)
                {
                    case 0:// если в комбобоксе линия, то рисовать линию
                        _currentFigure = new Line(_currentThickness, _currentLineColor, _currentBackColor, _currentLineType)
                        {
                            StartPoint = args.GetPosition(Canvas),
                            EndPoint = args.GetPosition(Canvas)
                        };
                        Canvas.Children.Add(_currentFigure);
                        break;
                    case 1:// если в комбобоксе окружность, то рисовать окружность
                        _currentFigure = new Circle(_currentThickness, _currentLineColor, _currentBackColor, _currentLineType)
                        {
                            Center = args.GetPosition(Canvas),
                            Radius = 0
                        };
                        Canvas.Children.Add(_currentFigure);
                        break;
                    case 2:// если в комбобоксе полилиния, то рисовать полилинию
                        _currentFigure = new Polyline(_currentThickness, _currentLineColor, _currentBackColor, _currentLineType)
                        {
                            StartPoint = args.GetPosition(Canvas),
                            EndPoint = args.GetPosition(Canvas)
                        };
                        Canvas.Children.Add(_currentFigure);
                        break;
                    case 3:// если в комбобоксе многоугольник, то рисовать многоугольник
                        _currentFigure = new Polygone(_currentThickness, _currentLineColor, _currentBackColor, _currentLineType)
                        {
                            StartPoint = args.GetPosition(Canvas),
                            EndPoint = args.GetPosition(Canvas)
                        };
                        _polygonPoints.Add(args.GetPosition(Canvas));
                        _polygoneStartPoint = new Point(args.GetPosition(Canvas).X, args.GetPosition(Canvas).Y);
                        Canvas.Children.Add(_currentFigure);
                        break;
                    case 4:// если в комбобоксе эллипс, то рисовать его
                        _currentFigure = new Ellipse(_currentThickness, _currentLineColor, _currentBackColor, _currentLineType)
                        {
                            Center = args.GetPosition(Canvas),
                            RadiusX = 0,
                            RadiusY = 0
                        };
                        Canvas.Children.Add(_currentFigure);
                        break;
                }
            }
        }

        private void ColorPicker_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

            _currentLineColor = new SolidColorBrush(e.NewValue ?? Colors.Black) ;
        }

        private void BackColorPicker_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            _currentBackColor = new SolidColorBrush(e.NewValue ?? Colors.Black);
        }

        private void RangeBase_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _currentThickness = e.NewValue;
        }

        private void LineTypesComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (LineTypesComboBox.SelectedIndex)
            {
                case 0:
                    _currentLineType = LineType.Solid;
                    break;
                case 1:
                    _currentLineType = LineType.Dashed;
                    break;
                case 2:
                    _currentLineType = LineType.Dotted;
                    break;
            }
        }

        private void ClearClicked(object sender, RoutedEventArgs e)
        {
            Canvas.Children.Clear();
        }
    }
}