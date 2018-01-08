using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
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
        private List<Point> _polylinePoints = new List<Point>();

        private Brush _currentLineColor;
        private Brush _currentBackColor;
        private double _currentThickness;
        private LineType _currentLineType;
        private List<Shape> _clipboard = new List<Shape>();
        private List<List<UIElement>> _history = new List<List<UIElement>>();
        private int _currentHistoryPosition;

        public MainWindow()
        {
            DataContext = this;
            Figures = new ObservableCollection<string>
            {
                "линия",
                "окружность",
                "полилиния",
                "многоугольник",
                "эллипс",
                "курсор"
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
            Canvas.PreviewKeyDown += Canvas_PreviewKeyDown; //подписка на нажатие кнопок клавиатуры
            Canvas.EditingMode = InkCanvasEditingMode.None;
            Canvas.MoveEnabled = true;
            Canvas.ResizeEnabled = true;

        }

        private void Canvas_PreviewKeyDown(object sender, KeyEventArgs e) //функции копирования, вставки и вырезания
        {
            if (e.Key == Key.Z && Keyboard.Modifiers == ModifierKeys.Control)
            {
                GoBack();
            }

            if (e.Key == Key.Z && Keyboard.Modifiers == ModifierKeys.Control && Keyboard.Modifiers == ModifierKeys.Shift)
            {

            }

            if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Canvas.CopySelection();
                //if (Canvas.GetSelectedElements() == null || Canvas.GetSelectedElements().Count == 0) return;
                //_clipboard.Clear();  //очистить буфер
                //foreach (Shape item in Canvas.GetSelectedElements())
                //{
                //    _clipboard.Add(item);
                //}
            }

            if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Canvas.Paste(new Point(Canvas.ActualWidth/2, Canvas.ActualHeight/2));
                //if (_clipboard == null || _clipboard.Count == 0) return;
                //_clipboard.Select(t => Canvas.Children.Add(t));
            }

            if (e.Key == Key.X && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Canvas.CutSelection();
                //if (Canvas.GetSelectedElements() == null || Canvas.GetSelectedElements().Count == 0) return;
                //_clipboard.Clear();
                
                //foreach (Shape element in Canvas.GetSelectedElements())
                //{
                //    _clipboard.Add(element);
                //    Canvas.Children.Remove(element);
                //}
            }

            Canvas.InvalidateVisual(); //перерисовываем канвас
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
           
            if (args.ChangedButton == MouseButton.Right)//если была нажата правая кнопка мыши, то произойдет следующее:
            {
                if (_currentFigure is Polygone polygone)// если текущая фигура - многоугольник, то завершить его рисовать(обнулить текущую фигуру)
                {

                    polygone.EndPoint = _polygoneStartPoint;
                    polygone.InvalidateVisual();

                    var p = new Polygon 
                    {
                        Points = new PointCollection(_polygonPoints),
                        StrokeDashArray = _currentFigure?.StrokeDashArray
                    };
                    p.Fill = _currentBackColor;
                    p.Stroke = _currentLineColor;
                    p.StrokeThickness = _currentThickness;
                    

                    foreach (var item in _polygonPoints)  //для каждой точки нужно стереть палочки
                    {
                        Canvas.Children.RemoveAt(Canvas.Children.Count - (_polygonPoints.Count ) + _polygonPoints.IndexOf(item));
                    }
                    Canvas.Children.Add(p);
                    _polygonPoints.Clear();
                    _currentFigure = null;
                    Canvas.InvalidateVisual();
                }
                if (_currentFigure is Polyline)// если текущая фигура - полилиния, то завершить ее рисовать(обнулить текущую фигуру)
                {
                    var polyline = new System.Windows.Shapes.Polyline
                    {
                        Points = new PointCollection(_polylinePoints),
                        StrokeThickness = _currentThickness,
                        Stroke = _currentLineColor,
                        StrokeDashArray = _currentFigure?.StrokeDashArray
                    };
                    foreach (var item in _polylinePoints)
                    {
                        Canvas.Children.RemoveAt(Canvas.Children.Count - _polylinePoints.Count + _polylinePoints.IndexOf(item));
                    }
                    Canvas.Children.Add(polyline);
                    _polylinePoints.Clear();

                    _currentFigure = null;
                    Canvas.InvalidateVisual();
                }
                SaveCanvasState();
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
                    SaveCanvasState();
                    return;
                }

                if (_currentFigure is Polyline)//если текущая фигура - полилиния, то закончить ее рисовать и начать рисовать новую 
                {
                    _currentFigure = new Polyline(_currentThickness, _currentLineColor, _currentBackColor, _currentLineType)
                    {
                        StartPoint = args.GetPosition(Canvas),
                        EndPoint = args.GetPosition(Canvas),
                    };
                    _polylinePoints.Add(args.GetPosition(Canvas));
                    Canvas.Children.Add(_currentFigure);
                    SaveCanvasState();
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
                        _polylinePoints.Add(args.GetPosition(Canvas));
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
                    case 5:
                        Canvas.EditingMode = InkCanvasEditingMode.Select;
                        _currentFigure = null;
                        //var selectedItem = (FrameworkElement)GetCanvasHoveredElement();
                        //Canvas.Children.Remove(selectedItem);
                        break;
                }
                SaveCanvasState();
            }
        }

        private void GoBack()
        {
            Canvas.Children.Clear();
            foreach(Shape item in _history[_currentHistoryPosition - 1])
            {
                Canvas.Children.Add(item);
            }

            _currentHistoryPosition--;
            
        }

        private void SaveCanvasState()
        {
            //Canvas.Children
            _history.Add(Canvas.Children.Cast<UIElement>().ToList());
            _currentHistoryPosition = _history.Count;
            
            //_history.Add(shape);
        }

        private UIElement GetCanvasHoveredElement()
        {
            var elems = Canvas.Children.OfType<UIElement>().Where(e => e.Visibility == Visibility.Visible && e.IsMouseOver);
            return elems.DefaultIfEmpty(null).First();
        }

        private void ColorPicker_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            _currentLineColor = new SolidColorBrush(e.NewValue ?? Colors.Black);

            if (Canvas == null) return;
            var selectedItems = Canvas.GetSelectedElements().ToArray();
            if (selectedItems.Count() != 0)
            {
                foreach (Shape item in selectedItems)
                {
                    item.Stroke = _currentLineColor;
                }
            }
        }

        private void BackColorPicker_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            _currentBackColor = new SolidColorBrush(e.NewValue ?? Colors.Black);

            if (Canvas == null) return;
            var selectedItems = Canvas.GetSelectedElements().ToArray();
            if (selectedItems.Count() != 0)
            {
                foreach (Shape item in selectedItems)
                {
                    if (item is Polyline || item is Line) return;
                    item.Fill = _currentBackColor;
                }
            }
        }

        private void RangeBase_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _currentThickness = e.NewValue;
            if (Canvas == null) return;
            var selectedItems = Canvas.GetSelectedElements().ToArray();
            if (selectedItems.Count() != 0)
            {
                foreach (Shape item in selectedItems)
                {
                    item.StrokeThickness = _currentThickness;
                }
            }
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
            if (Canvas == null) return;
            var selectedItems = Canvas.GetSelectedElements().ToArray();
            if (selectedItems.Count() != 0)
            {
                foreach (Shape item in selectedItems)
                {
                    item.StrokeDashArray = GetDashedArrayByThickness(_currentLineType) ?? GetDashedArrayByThickness(LineType.Solid);
                }
            }
        }

        private DoubleCollection GetDashedArrayByThickness(LineType value)
        {
            switch (value)
            {
                case LineType.Solid:
                    return new DoubleCollection { 100000 };
                case LineType.Dashed:
                    return new DoubleCollection { 10 };
                case LineType.Dotted:
                    return new DoubleCollection { 1 };
                default:
                    return null;
            }
        }

        private void ClearClicked(object sender, RoutedEventArgs e)
        {
            Canvas.Children.Clear();
        }

        private void Remove_Button_Click(object sender, RoutedEventArgs e)
        {
            var items = Canvas.GetSelectedElements().ToArray();
            foreach (var item in items)
            {
                Canvas.Children.Remove(item);
            }
        }

        private void FiguresComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) //
        {
            if (Canvas == null) return;
            if (FiguresComboBox.SelectedIndex == 5) return;
            Canvas.EditingMode = InkCanvasEditingMode.None;
        }
    }
}