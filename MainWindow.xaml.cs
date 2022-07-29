using Dapper;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MissionAssistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Point clickedPoint, currentPoint;
        public Image bafChart;

        public MainWindow()
        {
            InitializeComponent();
            lineColors.ItemsSource = typeof(Brushes).GetProperties();
            circleColors.ItemsSource = typeof(Brushes).GetProperties();
            routeColors.ItemsSource = typeof(Brushes).GetProperties();
            polygonColors.ItemsSource = typeof(Brushes).GetProperties();

            //For Aircraft Data Chart
            SqlMapper.SetTypeMap(typeof(PerformanceData), new CustomPropertyTypeMap(typeof(PerformanceData), (type, columnName) => type.GetProperties().FirstOrDefault(prop => prop.GetCustomAttributes(false).OfType<ColumnAttribute>().Any(attr => attr.Name == columnName))));
            SqlMapper.SetTypeMap(typeof(FuelStartData), new CustomPropertyTypeMap(typeof(FuelStartData), (type, columnName) => type.GetProperties().FirstOrDefault(prop => prop.GetCustomAttributes(false).OfType<ColumnAttribute>().Any(attr => attr.Name == columnName))));
            SqlMapper.SetTypeMap(typeof(FuelReduceData), new CustomPropertyTypeMap(typeof(FuelReduceData), (type, columnName) => type.GetProperties().FirstOrDefault(prop => prop.GetCustomAttributes(false).OfType<ColumnAttribute>().Any(attr => attr.Name == columnName))));

            GoogleHybridMapProvider.Instance.ApiKey = "AIzaSyBwyMHyzcR5EPdVwZdcf_BKizfnDQPZQHo";
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            //Gmap.CacheLocation = @"mapdata.cf";
            Gmap.MapProvider = GoogleHybridMapProvider.Instance;
            Gmap.DragButton = MouseButton.Left;
            Gmap.Position = new PointLatLng(23.777176, 90.399452);
            Gmap.ScaleMode = ScaleModes.Dynamic;
            Gmap.MinZoom = 0;
            Gmap.MaxZoom = 20;
            Gmap.Zoom = 8;
            Gmap.ShowCenter = false;
            try
            {
                bafChart = new Image { Source = (ImageSource)new ImageSourceConverter().ConvertFrom(@"BAFMap.png"), Width = 620, Height = 690, Margin = new Thickness(-310, -345, -310, -345), RenderTransformOrigin = new Point(0.5, 0.5), Tag = new PointLatLng(23.3624285934088, 90.802001953125), IsHitTestVisible = false };
            }
            catch
            {
                MessageBox.Show("Couldn't load BAF Map Chart", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown(-1);
            }

            UI.Init(this);
        }

        private void Rendered(object sender, EventArgs e)
        {
            Point baf_anch = DataCalculations.GetPhysicalFromLatLng((PointLatLng)bafChart.Tag);
            Canvas.SetLeft(bafChart, baf_anch.X);
            Canvas.SetTop(bafChart, baf_anch.Y);
            bafChart.RenderTransform = new ScaleTransform(0.00901953125 * Math.Pow(2, Gmap.Zoom), 0.00909375 * Math.Pow(2, Gmap.Zoom));
            drawCanvas.Children.Add(bafChart);
            Panel.SetZIndex(bafChart, 0);
            bafChart.Visibility = Visibility.Hidden;
        }

        private void drawOverlayMouseDown(object sender, MouseButtonEventArgs e)
        {
            clickedPoint = e.GetPosition(drawCanvas);
            currentPoint = clickedPoint;
            drawCanvas.Focus();
            if (e.ChangedButton == MouseButton.Left)
            {
                UI.OnMouseHold(clickedPoint, true);
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                UI.OnMouseHold(clickedPoint, false);
            }
        }

        private void drawOverlayMouseMove(object sender, MouseEventArgs e)
        {
            currentPoint = e.GetPosition(drawCanvas);
            if (e.LeftButton == MouseButtonState.Pressed) UI.OnMouseMove(currentPoint, 1);
            else if (e.RightButton == MouseButtonState.Pressed) UI.OnMouseMove(currentPoint, 2);
            else UI.OnMouseMove(currentPoint, 0);
        }

        private void drawOverlayMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                UI.OnMouseRelease(currentPoint, true);
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                UI.OnMouseRelease(currentPoint, false);
            }
        }

        private void drawOverlayMouseWheel(object sender, MouseWheelEventArgs e)
        {
            UI.OnMouseWheel(e.Delta);
        }

        private void drawOverlayMouseEnter(object sender, MouseEventArgs e)
        {
            UI.OnMouseEnter(currentPoint);
        }

        private void drawOverlayMouseLeave(object sender, MouseEventArgs e)
        {
            UI.OnMouseLeave(currentPoint);
        }

        private void GmapDrag()
        {
            UI.OnMapDrag();
        }

        private void globalButtonOperations(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton)
            {
                UI.GlobalButtonActions(((ToggleButton)sender).Name);
            }
            else if (sender is Button)
            {
                UI.GlobalButtonActions(((Button)sender).Name);
            }
        }

        private void mapDrawOperations(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            UI.DrawButtonActions(btn.Name);
        }

        private void testButtonOperations(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            switch (btn.Name)
            {
                case "testBtn1":
                    Tester.TestBtn1();
                    break;
                case "testBtn2":
                    Tester.TestBtn2();
                    break;
                case "testBtn3":
                    Tester.TestBtn3();
                    break;
                case "testBtn4":
                    Tester.TestBtn4();
                    break;
                case "testBtn5":
                    Tester.TestBtn5();
                    break;
            }
        }

        private void unitConverterBtnClick(object sender, RoutedEventArgs e)
        {

        }

        private void resizeImage(object sender, RoutedEventArgs e)
        {
            UI.ResizeImage();
        }

        private void increaseElements(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            UI.SizeAdjustActions(btn.Name);
        }

        private void reduceElements(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            UI.SizeAdjustActions(btn.Name);
        }

        private void expand(object sender, MouseButtonEventArgs e)
        {
            var obj = sender as Path;
            UI.PanelExpansionActions(obj.Name);
        }

        private void keyControls(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                UI.KeyControls("escape");
            }
            if (e.Key == Key.Delete)
            {
                UI.KeyControls("delete");
            }
        }

        private void setBgOpacity(object sender, RoutedEventArgs e)
        {
            if (bafChart != null) bafChart.Opacity = (10 - (double)oppnl.Children.IndexOf((UIElement)sender)) / 10;
        }

        private void fieldKeyControls(object sender, KeyEventArgs e)
        {
            var itm = ((FrameworkElement)sender).Tag;
            string param = "";
            if (itm != null) param = itm.ToString();
            if (e.Key == Key.Enter)
            {
                if (param == "numeric")
                {
                    if (sender is TextBox)
                    {
                        var obj = (TextBox)sender;
                        if (String.IsNullOrEmpty(obj.Text))
                        {
                            obj.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                        }
                    }
                    if (sender is ComboBox)
                    {
                        var obj = (ComboBox)sender;
                        if (String.IsNullOrEmpty(obj.Text))
                        {
                            obj.GetBindingExpression(ComboBox.TextProperty).UpdateTarget();
                        }
                    }
                }
                FocusManager.SetFocusedElement(main, drawCanvas);
                Keyboard.ClearFocus();
            }
            else if (param == "numeric") numericFilter(sender, e);
        }

        private void editableComboboxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var obj = (ComboBox)sender;
            if (obj.SelectedIndex != -1)
            {
                FocusManager.SetFocusedElement(main, drawCanvas);
                Keyboard.ClearFocus();
            }
        }

        private void removeElement(object sender, RoutedEventArgs e)
        {
            var obj = sender as Button;
            UI.ObjectRemoveActions(obj.Name);
        }

        private void numericFilter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemPeriod || e.Key == Key.Decimal)
            {
                if (sender is TextBox)
                {
                    if ((sender as TextBox).Text.IndexOf(".") != -1) e.Handled = true;
                }
                else if (sender is ComboBox)
                {
                    if ((sender as ComboBox).Text.IndexOf(".") != -1) e.Handled = true;
                }
            }
            else if ((e.Key < Key.D0 || e.Key > Key.D9) && (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)) e.Handled = true;
            else if (e.Key == Key.Space) e.Handled = true;
        }
    }
}
