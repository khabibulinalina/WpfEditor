using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using WpfApp1.Helpers;
using WpfApp1.Mvvm;

namespace WpfApp1.Ui
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly MainViewModel _viewModel;

        private readonly List<Shape> _clipboard = new List<Shape>();
        private readonly UndoRedoManager _redoManager = new UndoRedoManager();

        public MainWindow()
        {
            _viewModel = new MainViewModel();
            DataContext = _viewModel;

            InitializeComponent(); //загружает интерфейс
            Bind();
            _viewModel.Init();

            Canvas.MouseDown += CanvasOnMouseDown; //подписка на события нажатия и перемещения мыши
            Canvas.MouseMove += CanvasOnMouseMove;
            Canvas.MouseUp += CanvasOnMouseUp;
            Canvas.PreviewKeyDown += Canvas_PreviewKeyDown; //подписка на нажатие кнопок клавиатуры
            Canvas.EditingMode = InkCanvasEditingMode.None;
            
        }

        private void Bind()
        {
            _viewModel.ChangeCanvasEditingModeViewCommand = new RelayCommand(o =>{Canvas.EditingMode = (InkCanvasEditingMode)o;});
            _viewModel.CanvasRemoveChildAtIndexViewCommand = new RelayCommand(o => { });
            _viewModel.CanvasRemoveChildViewCommand = new RelayCommand(o => {Canvas.Children.Remove((UIElement)o);});
            _viewModel.CanvasAddChildViewCommand = new RelayCommand(o => { Canvas.Children.Add((UIElement) o); });
            _viewModel.InvalidateCanvasViewCommand = new RelayCommand(o => {Canvas.InvalidateVisual();});
            _viewModel.ChangeSelectedShapesLineTypesViewCommand = new RelayCommand(ChangeSelectedShapesLineTypes);
            _viewModel.ChangeSelectedShapesLineColorViewCommand = new RelayCommand(ChangeSelectedShapesLineColor);
            _viewModel.ChangeSelectedShapesBackColorViewCommand = new RelayCommand(ChangeSelectedShapesBackColor);
            _viewModel.ChangeSelectedShapesLineThicknessViewCommand = new RelayCommand(ChangeSelectedShapesThickness);
            _viewModel.RemoveSelectedItemsViewCommand = new RelayCommand(RemoveSelectedItems);
            _viewModel.CanvasMoveItemViewCommand = new RelayCommand(MoveItem);
            _viewModel.CanvasMoveItemByTwoPointsViewCommand = new RelayCommand(CanvasMoveItemByTwoPoints);
        }

        private void CanvasMoveItemByTwoPoints(object obj)
        {
            var args = (MoveFigureByTwoPointsEventArgs)obj;
            InkCanvas.SetLeft(args.Figure, args.LeftTopPoint.X);
            InkCanvas.SetTop(args.Figure, args.LeftTopPoint.Y);
            InkCanvas.SetRight(args.Figure, args.RightBottomPoint.X);
            InkCanvas.SetBottom(args.Figure, args.RightBottomPoint.Y);
        }

        private void MoveItem(object obj)
        {
            var args = (MoveFigureEventArgs) obj;
            InkCanvas.SetLeft(args.Figure, args.Position.X);
            InkCanvas.SetTop(args.Figure, args.Position.Y);
        }

        private void RemoveSelectedItems(object obj)
        {
            var items = Canvas.GetSelectedElements().ToArray();
            foreach (var item in items)
            {
                Canvas.Children.Remove(item);
            }
        }

        private void ChangeSelectedShapesThickness(object obj)
        {
            var thickness = (double) obj;
            if (Canvas == null) return;
            var selectedItems = Canvas.GetSelectedElements().ToArray();
            if (selectedItems.Length != 0)
            {
                foreach (var uiElement in selectedItems)
                {
                    var item = (Shape)uiElement;
                    _redoManager.Execute(new ChangeThicknessCommand(item, thickness));
                    item.StrokeThickness = thickness;
                }
            }
        }

        private void ChangeSelectedShapesBackColor(object obj)
        {
            var color = (Color) obj;
            if (Canvas == null) return;
            var selectedItems = Canvas.GetSelectedElements().ToArray();
            if (selectedItems.Length != 0)
            {
                foreach (var uiElement in selectedItems)
                {
                    var item = (Shape)uiElement;
                    if (item is Polyline || item is Line) return;
                    _redoManager.Execute(new ChangeFillColorCommand(item, new SolidColorBrush(color)));
                    item.Fill = new SolidColorBrush(color);
                }
            }
        }

        private void ChangeSelectedShapesLineColor(object obj)
        {
            var color = (Color) obj;
            if (Canvas == null) return;
            var selectedItems = Canvas.GetSelectedElements().ToArray();
            if (selectedItems.Length != 0)
            {
                foreach (var uiElement in selectedItems)
                {
                    var item = (Shape)uiElement;
                    _redoManager.Execute(new ChangeStrokeColorCommand(item, new SolidColorBrush(color)));
                    item.Stroke = new SolidColorBrush(color);
                }
            }
        }

        private void ChangeSelectedShapesLineTypes(object obj)
        {
            var array = (DoubleCollection) obj;
            if (Canvas == null) return;
            var selectedItems = Canvas.GetSelectedElements().ToArray();
            if (selectedItems.Length == 0) return;
            foreach (var uiElement in selectedItems)
            {
                var item = (Shape)uiElement;
                _redoManager.Execute(new ChangeLineTypesCommand(item, array));
                item.StrokeDashArray = array;
            }
        }

        private void CanvasOnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _viewModel.CanvasOnMouseUpCommand.Execute(e);
        }

        private void Canvas_PreviewKeyDown(object sender, KeyEventArgs e) //функции копирования, вставки и вырезания
        {
            switch (e.Key)
            {
                case Key.C when Keyboard.Modifiers == ModifierKeys.Control:
                    Copy();
                    break;
                case Key.V when Keyboard.Modifiers == ModifierKeys.Control:
                    Paste();
                    break;
                case Key.X when Keyboard.Modifiers == ModifierKeys.Control:
                    Cut();
                    break;
                case Key.Z when Keyboard.Modifiers == ModifierKeys.Control:
                    _redoManager.Undo();
                    break;
                case Key.Z when Keyboard.Modifiers.HasFlag(ModifierKeys.Control):
                    if(Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                        _redoManager.Redo();
                    break;
            }

            Canvas.InvalidateVisual(); //перерисовываем канвас
        }

        private void Copy()
        {
            if (Canvas.GetSelectedElements().Count == 0) return;
            _clipboard.Clear(); //очистить буфер
            foreach (var uiElement in Canvas.GetSelectedElements())
            {
                var item = (Shape)uiElement;
                _clipboard.Add(item);
            }
        }

        private void Paste()
        {
            if (_clipboard == null || _clipboard.Count == 0) return;
            foreach (var shape in _clipboard)
            {
                switch (shape)
                {
                    case System.Windows.Shapes.Ellipse circle:
                        Canvas.Children.Add(circle.Clone());
                        break;
                    case System.Windows.Shapes.Line line:
                        Canvas.Children.Add(line.Clone());
                        break;
                    case System.Windows.Shapes.Polyline polyline:
                        Canvas.Children.Add(polyline.Clone());
                        break;
                    case Polygon polygon:
                        Canvas.Children.Add(polygon.Clone());
                        break;
                }
            }
        }

        private void Cut()
        {
            if (Canvas.GetSelectedElements().Count == 0) return;
            _clipboard.Clear();
            foreach (var uiElement in Canvas.GetSelectedElements())
            {
                var shape = (Shape)uiElement;
                _clipboard.Add(shape);
            }
            foreach (var shape in _clipboard)
            {
                Canvas.Children.Remove(shape);
            }
        }

        private void CanvasOnMouseMove(object sender, MouseEventArgs args)
        {
            var eventArgs = new CanvasOnMouseUpEventArgs { MouseArgs = args, Position = args.GetPosition(Canvas) };
            _viewModel.CanvasOnMouseMoveCommand.Execute(eventArgs);
        }

        private void CanvasOnMouseDown(object sender, MouseButtonEventArgs args)
        {
            var eventArgs = new CanvasOnMouseDownEventArgs{MouseArgs = args, Position = args.GetPosition(Canvas)};
            _viewModel.CanvasOnMouseDownCommand.Execute(eventArgs);
        }

        private void LineTypesComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.LineTypesComboBoxIndexChangeCommand.Execute(LineTypesComboBox.SelectedIndex);
        }

        private void ClearClicked(object sender, RoutedEventArgs e)
        {
            Canvas.Children.Clear();
        }

        private void Remove_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RemoveFiguresCommand.Execute(null);
        }

        private void FiguresComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Canvas == null) return;
            _viewModel.FigureChangedCommand.Execute(FiguresComboBox.SelectedIndex);
        }

        //private UIElement GetCanvasHoveredElement()
        //{
        //    var elems = Canvas.Children.OfType<UIElement>()
        //        .Where(e => e.Visibility == Visibility.Visible && e.IsMouseOver);
        //    return elems.DefaultIfEmpty(null).First();
        //}

        private void SaveClicked(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter =
                    "Bitmap Image (.bmp)|*.bmp|Gif Image (.gif)|*.gif|JPEG Image (.jpeg)|*.jpeg|Png Image (.png)|*.png|Tiff Image (.tiff)|*.tiff|Wmf Image (.wmf)|*.wmf"
            };
            if (saveFileDialog.ShowDialog() != true) return;
            var bounds = VisualTreeHelper.GetDescendantBounds(Canvas);
            var dpi = 96d;

            var rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, PixelFormats.Default);

            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
            {
                var vb = new VisualBrush(Canvas);
                dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);

            var pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

            try
            {
                var ms = new MemoryStream();

                pngEncoder.Save(ms);
                ms.Close();
                File.WriteAllBytes(saveFileDialog.FileName, ms.ToArray());
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenClicked(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() != true) return;
            var brush = new ImageBrush {ImageSource = new BitmapImage(new Uri(dialog.FileName, UriKind.Relative))};
            Canvas.Background = brush;
        }

        private void NewClicked(object sender, RoutedEventArgs e)
        {
            Canvas.Children.Clear();
        }

        private void CutClicked(object sender, RoutedEventArgs e)
        {
            Cut();
        }

        private void CopyClicked(object sender, RoutedEventArgs e)
        {
            Copy();
        }

        private void PasteClicked(object sender, RoutedEventArgs e)
        {
            Paste();
        }

        private void UndoClicked(object sender, RoutedEventArgs e)
        {
            _redoManager.Undo();
        }

        private void RedoClicked(object sender, RoutedEventArgs e)
        {
            _redoManager.Redo();
        }
    }
}