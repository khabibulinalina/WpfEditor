using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfApp1.Helpers;
using WpfApp1.Mvvm;

namespace WpfApp1.Ui
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<string> Figures { get; set; }
        public ObservableCollection<string> LineTypes { get; set; }
        public int FiguresComboBoxSelectedIndex { get; set; }

        public Color CurrentLineColor
        {
            get => _currentLineColor;
            set
            {
                _currentLineColor = value;
                if (_currentLineColor == null) return;
                ChangeSelectedShapesLineColorViewCommand.Execute(_currentLineColor);
            }
        }
        public Color CurrentBackColor
        {
            get => _currentBackColor;
            set
            {
                _currentBackColor = value;
                if (_currentBackColor == null) return;
                ChangeSelectedShapesBackColorViewCommand.Execute(_currentBackColor);
            }
        }
        public double CurrentThickness
        {
            get => _currentThickness;
            set
            {
                _currentThickness = value;
                ChangeSelectedShapesLineThicknessViewCommand.Execute(_currentThickness);
            }
        }

        private RelayCommand _figureChangedCommand;
        private RelayCommand _canvasOnMouseDownCommand;
        private Figure _currentFigure;
        private Point _polygoneStartPoint;
        private readonly List<Point> _polygonPoints = new List<Point>();
        private readonly List<Point> _polylinePoints = new List<Point>();
        private RelayCommand _lineTypesComboBoxIndexChangeCommand;
        private LineType _currentLineType;
        private RelayCommand _canvasOnMouseMoveCommand;
        private readonly List<Figure> _polygonLines = new List<Figure>();
        private readonly List<Figure> _polylineLines = new List<Figure>();
        private Color _currentLineColor;
        private Color _currentBackColor;
        private double _currentThickness;
        private RelayCommand _removeFiguresCommand;
        private RelayCommand _canvasOnMouseUpCommand;

        public RelayCommand FigureChangedCommand =>
            _figureChangedCommand ??
            (_figureChangedCommand = new RelayCommand(obj =>
            {
                var selectedIndex = (int) obj;
                FigureChanged(selectedIndex);
            }));
        public RelayCommand CanvasOnMouseDownCommand =>
            _canvasOnMouseDownCommand ??
            (_canvasOnMouseDownCommand = new RelayCommand(obj =>
            {
                var mouseButtonEventArgs = (CanvasOnMouseDownEventArgs) obj;
                CanvasOnMouseDown(mouseButtonEventArgs);
            }));
        public RelayCommand LineTypesComboBoxIndexChangeCommand =>
            _lineTypesComboBoxIndexChangeCommand ??
            (_lineTypesComboBoxIndexChangeCommand = new RelayCommand(obj =>
            {
                var index = (int) obj;
                switch (index)
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
                ChangeSelectedShapesLineTypesViewCommand.Execute(GetDashedArrayByThickness(_currentLineType) ??
                                                                 GetDashedArrayByThickness(LineType.Solid));
            }));
        public RelayCommand CanvasOnMouseMoveCommand =>
            _canvasOnMouseMoveCommand ??
            (_canvasOnMouseMoveCommand = new RelayCommand(o => CanvasOnMouseMove((CanvasOnMouseUpEventArgs)o)));
        public RelayCommand RemoveFiguresCommand =>
            _removeFiguresCommand ??
            (_removeFiguresCommand = new RelayCommand(o => RemoveSelectedItemsViewCommand.Execute(null)));
        public RelayCommand CanvasOnMouseUpCommand =>
            _canvasOnMouseUpCommand ??
            (_canvasOnMouseUpCommand = new RelayCommand(o => CanvasOnMouseUp((MouseButtonEventArgs)o)));

        public RelayCommand ChangeCanvasEditingModeViewCommand { get; set; }
        public RelayCommand CanvasRemoveChildAtIndexViewCommand { get; set; }
        public RelayCommand CanvasRemoveChildViewCommand { get; set; }
        public RelayCommand CanvasAddChildViewCommand { get; set; }
        public RelayCommand InvalidateCanvasViewCommand { get; set; }
        public RelayCommand ChangeSelectedShapesLineTypesViewCommand { get; set; }
        public RelayCommand ChangeSelectedShapesLineColorViewCommand { get; set; }
        public RelayCommand ChangeSelectedShapesBackColorViewCommand { get; set; }
        public RelayCommand ChangeSelectedShapesLineThicknessViewCommand { get; set; }
        public RelayCommand RemoveSelectedItemsViewCommand { get; set; }
        public RelayCommand CanvasMoveItemViewCommand { get; set; }
        public RelayCommand CanvasMoveItemByTwoPointsViewCommand { get; set; }

        public void Init()
        {
            Figures = new ObservableCollection<string>
            {
                "линия",
                "окружность",
                "полилиния",
                "многоугольник",
                "эллипс",
                "курсор"
            };
            OnPropertyChanged(nameof(Figures));
            LineTypes = new ObservableCollection<string>
            {
                "сплошная",
                "пунктирная",
                "точечная"
            };
            OnPropertyChanged(nameof(LineTypes));
            FiguresComboBoxSelectedIndex = 0;
            OnPropertyChanged(nameof(FiguresComboBoxSelectedIndex));

            CurrentLineColor = Color.FromRgb(255, 111, 150);
            OnPropertyChanged(nameof(CurrentLineColor));
            CurrentBackColor = Color.FromRgb(255, 255, 0);
            OnPropertyChanged(nameof(CurrentBackColor));
            CurrentThickness = 5;
            OnPropertyChanged(nameof(CurrentThickness));
        }

        private void FigureChanged(int index)
        {
            if (index == 5) return;
            ChangeCanvasEditingModeViewCommand.Execute(InkCanvasEditingMode.None);
        }

        private DoubleCollection GetDashedArrayByThickness(LineType value)
        {
            switch (value)
            {
                case LineType.Solid:
                    return new DoubleCollection {100000};
                case LineType.Dashed:
                    return new DoubleCollection {10};
                case LineType.Dotted:
                    return new DoubleCollection {1};
                default:
                    return null;
            }
        }

        private void CanvasOnMouseMove(CanvasOnMouseUpEventArgs args)
        {
            switch (_currentFigure)
            {
                case null:
                    return;
                case Polygone polygone:
                    polygone.EndPoint = args.Position;
                    polygone.InvalidateVisual();
                    break;
                case Polyline polyline:
                    polyline.EndPoint = args.Position;
                    polyline.InvalidateVisual();
                    break;
            }

            if (args.MouseArgs.LeftButton == MouseButtonState.Pressed) //если нажата левая кнопка мыши, то выполнять следующее
            {
                switch (_currentFigure)
                {
                    case Line line:
                        line.EndPoint = args.Position;
                        line.InvalidateVisual(); //обновляет фигуру
                        break;
                    case Circle circle:
                        circle.Radius = args.Position.X - circle.Center.X;
                        circle.InvalidateVisual();
                        break;
                    case Ellipse ellipse:
                        ellipse.RadiusX = args.Position.X - ellipse.Center.X;
                        ellipse.RadiusY = args.Position.Y - ellipse.Center.Y;
                        ellipse.InvalidateVisual();
                        break;
                }
            }
        }

        private void CanvasOnMouseDown(CanvasOnMouseDownEventArgs args)
        {
            if (args.MouseArgs.ChangedButton == MouseButton.Right
            ) //если была нажата правая кнопка мыши, то произойдет следующее:
            {
                if (_currentFigure is Polygone polygone
                ) // если текущая фигура - многоугольник, то завершить его рисовать(обнулить текущую фигуру)
                {
                    polygone.EndPoint = _polygoneStartPoint;
                    polygone.InvalidateVisual();

                    var p = new Polygon
                    {
                        Points = new PointCollection(_polygonPoints),
                        StrokeDashArray = _currentFigure?.StrokeDashArray,
                        Fill = new SolidColorBrush(CurrentBackColor),
                        Stroke = new SolidColorBrush(CurrentLineColor),
                        StrokeThickness = CurrentThickness
                    };


                    foreach (var item in _polygonLines) //для каждой точки нужно стереть палочки
                    {
                        CanvasRemoveChildViewCommand.Execute(item);
                        //CanvasRemoveChildAtIndexViewCommand
                        //    .Execute(Canvas.Children.Count - _polygonPoints.Count + _polygonPoints.IndexOf(item));
                    }
                    CanvasAddChildViewCommand.Execute(p);
                    _polygonPoints.Clear();
                    _polygonLines.Clear();
                    _currentFigure = null;
                    InvalidateCanvasViewCommand.Execute(null);
                }
                if (_currentFigure is Polyline
                ) // если текущая фигура - полилиния, то завершить ее рисовать(обнулить текущую фигуру)
                {
                    var polyline = new System.Windows.Shapes.Polyline
                    {
                        Points = new PointCollection(_polylinePoints),
                        StrokeThickness = CurrentThickness,
                        Stroke = new SolidColorBrush(CurrentLineColor),
                        StrokeDashArray = _currentFigure?.StrokeDashArray
                    };
                    foreach (var item in _polylineLines)
                    {
                        CanvasRemoveChildViewCommand.Execute(item);
                    }
                    CanvasAddChildViewCommand.Execute(polyline);
                    _polylinePoints.Clear();
                    _polylineLines.Clear();
                    _currentFigure = null;
                    InvalidateCanvasViewCommand.Execute(null);
                }
                return;
            }

            if (args.MouseArgs.ChangedButton == MouseButton.Left
            ) //если была нажата левая кнопка мыши, то выполнять следующее
            {
                if (_currentFigure is Polygone
                ) //если текущая фигура - многоугольник, то закончить рисовать его грань и начать рисовать новую 
                {
                    _currentFigure = new Polygone(CurrentThickness, new SolidColorBrush(CurrentLineColor), new SolidColorBrush(CurrentBackColor),
                        _currentLineType)
                    {
                        StartPoint = args.Position,
                        EndPoint = args.Position,
                    };
                    _polygonPoints.Add(args.Position);
                    _polygonLines.Add(_currentFigure);
                    CanvasAddChildViewCommand.Execute(_currentFigure);
                    return;
                }

                if (_currentFigure is Polyline
                ) //если текущая фигура - полилиния, то закончить ее рисовать и начать рисовать новую 
                {
                    _currentFigure = new Polyline(CurrentThickness, new SolidColorBrush(CurrentLineColor), new SolidColorBrush(CurrentBackColor),
                        _currentLineType)
                    {
                        StartPoint = args.Position,
                        EndPoint = args.Position,
                    };
                    _polylinePoints.Add(args.Position);
                    _polylineLines.Add(_currentFigure);
                    CanvasAddChildViewCommand.Execute(_currentFigure);
                    return;
                }

                switch (FiguresComboBoxSelectedIndex)
                {
                    case 0: // если в комбобоксе линия, то рисовать линию
                        _currentFigure = new Line(CurrentThickness, new SolidColorBrush(CurrentLineColor), new SolidColorBrush(CurrentBackColor),
                            _currentLineType)
                        {
                            StartPoint = args.Position,
                            EndPoint = args.Position
                        };
                        CanvasAddChildViewCommand.Execute(_currentFigure);
                        break;
                    case 1: // если в комбобоксе окружность, то рисовать окружность
                        _currentFigure = new Circle(CurrentThickness, new SolidColorBrush(CurrentLineColor), new SolidColorBrush(CurrentBackColor),
                            _currentLineType)
                        {
                            Center = args.Position,
                            Radius = 0
                        };
                        CanvasAddChildViewCommand.Execute(_currentFigure);
                        break;
                    case 2: // если в комбобоксе полилиния, то рисовать полилинию
                        _currentFigure = new Polyline(CurrentThickness, new SolidColorBrush(CurrentLineColor), new SolidColorBrush(CurrentBackColor),
                            _currentLineType)
                        {
                            StartPoint = args.Position,
                            EndPoint = args.Position
                        };
                        CanvasAddChildViewCommand.Execute(_currentFigure);
                        _polylinePoints.Add(args.Position);
                        _polylineLines.Add(_currentFigure);
                        break;
                    case 3: // если в комбобоксе многоугольник, то рисовать многоугольник
                        _currentFigure = new Polygone(CurrentThickness, new SolidColorBrush(CurrentLineColor), new SolidColorBrush(CurrentBackColor),
                            _currentLineType)
                        {
                            StartPoint = args.Position,
                            EndPoint = args.Position
                        };
                        _polygonPoints.Add(args.Position);
                        _polygoneStartPoint = new Point(args.Position.X, args.Position.Y);
                        _polygonLines.Add(_currentFigure);
                        CanvasAddChildViewCommand.Execute(_currentFigure);
                        break;
                    case 4: // если в комбобоксе эллипс, то рисовать его
                        _currentFigure = new Ellipse(CurrentThickness, new SolidColorBrush(CurrentLineColor), new SolidColorBrush(CurrentBackColor),
                            _currentLineType)
                        {
                            Center = args.Position,
                            RadiusX = 0,
                            RadiusY = 0
                        };
                        CanvasAddChildViewCommand.Execute(_currentFigure);
                        break;
                    case 5:
                        ChangeCanvasEditingModeViewCommand.Execute(InkCanvasEditingMode.Select);
                        _currentFigure = null;
                        //var selectedItem = (FrameworkElement)GetCanvasHoveredElement();
                        //Canvas.Children.Remove(selectedItem);
                        break;
                }
            }
        }

        private void CanvasOnMouseUp(MouseButtonEventArgs args)
        {
            if (args.ChangedButton ==  MouseButton.Left)
            {
                switch (_currentFigure)
                {
                    case null:
                        return;
                    case Line line:
                        var newLine = new System.Windows.Shapes.Line
                        {
                            X1 = line.StartPoint.X,
                            X2 = line.EndPoint.X,
                            Y1 = line.StartPoint.Y,
                            Y2 = line.EndPoint.Y,
                            StrokeThickness = line.StrokeThickness,
                            Stroke = line.Stroke,
                            StrokeDashArray = line.StrokeDashArray
                        };

                        CanvasAddChildViewCommand.Execute(newLine);
                        CanvasRemoveChildViewCommand.Execute(line);
                        InvalidateCanvasViewCommand.Execute(null);
                        break;
                    case Circle circle:
                        var newCircle = new System.Windows.Shapes.Ellipse
                        {
                            StrokeThickness = circle.StrokeThickness,
                            Stroke = circle.Stroke,
                            Fill = circle.Fill,
                            StrokeDashArray = circle.StrokeDashArray,
                            Width = Math.Abs(circle.Radius) * 2,
                            Height = Math.Abs(circle.Radius) * 2
                        };
                        CanvasAddChildViewCommand.Execute(newCircle);
                        
                        CanvasMoveItemViewCommand.Execute(new MoveFigureEventArgs
                        {
                            Position = new Point(circle.Center.X - circle.Radius, circle.Center.Y - circle.Radius),
                            Figure = newCircle
                        });
                        CanvasRemoveChildViewCommand.Execute(circle);
                        break;
                    case Ellipse ellipse:
                        var newEllipse = new System.Windows.Shapes.Ellipse
                        {
                            StrokeThickness = ellipse.StrokeThickness,
                            Stroke = ellipse.Stroke,
                            Fill = ellipse.Fill,
                            StrokeDashArray = ellipse.StrokeDashArray,
                            Width = Math.Abs(ellipse.RadiusX) * 2,
                            Height = Math.Abs(ellipse.RadiusY) * 2
                        };
                        CanvasAddChildViewCommand.Execute(newEllipse);

                        CanvasMoveItemViewCommand.Execute(new MoveFigureEventArgs
                        {
                            Position = new Point(ellipse.Center.X - ellipse.RadiusX, ellipse.Center.Y - ellipse.RadiusY),
                            Figure = newEllipse
                        });
                        CanvasRemoveChildViewCommand.Execute(ellipse);
                        break;
                }
            }
        }
    }
}