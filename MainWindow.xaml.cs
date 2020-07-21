using Dapper;
using GMap.NET;
using GMap.NET.MapProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Mission_Assistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private struct BaseUnit
        {
            public string baltUnit;
            public string bdistUnit;
            public string bspeedUnit;
            public string bfuelUnit;
            public string blfftUnit;
        }
        private List<BaseUnit> baseunits;
        private List<PerformanceData> pdatas;
        private List<FuelStartData> fsdatas;
        private List<FuelReduceData> frdatas;
        private PerformanceData pdata;
        private FuelStartData fsdata;
        private FuelReduceData frdata;
        private BaseUnit baseunit;
        public Point clickedPoint, currentPoint;
        public Line line = null;
        public Ellipse ellipse = null;
        public StackPanel headingBox = null;
        public Image bafChart;
        public string mode = "none";
        public bool set = false, draggable = false, drawn = false, adflag;
        public int lineCount = -1, routeCount = -1, circleCount = -1, polygonCount = -1, markerCount = -1, boxCount = -1, count = 0;
        public double left, top;

        private int offset = 0;
        private const int minoffset = 5, maxoffset = 100;
        private const double minThickness = 0.1, maxThickness = 30, minRadius = 0.1, maxRadius = 2000;
        public event PropertyChangedEventHandler PropertyChanged;
        public int OffSet
        {
            get
            {
                return offset;
            }
            set
            {
                if (value < minoffset) offset = minoffset;
                else if (value > maxoffset) offset = maxoffset;
                else offset = value;
                PropertyChanged(this, new PropertyChangedEventArgs("OffSet"));
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            offset = 20;
            pdatas = new List<PerformanceData>();
            fsdatas = new List<FuelStartData>();
            frdatas = new List<FuelReduceData>();
            baseunits = new List<BaseUnit>();
            lineColors.ItemsSource = typeof(Brushes).GetProperties();
            circleColors.ItemsSource = typeof(Brushes).GetProperties();
            routeColors.ItemsSource = typeof(Brushes).GetProperties();
            polygonColors.ItemsSource = typeof(Brushes).GetProperties();
            SqlMapper.SetTypeMap(typeof(PerformanceData), new CustomPropertyTypeMap(typeof(PerformanceData), (type, columnName) => type.GetProperties().FirstOrDefault(prop => prop.GetCustomAttributes(false).OfType<ColumnAttribute>().Any(attr => attr.Name == columnName))));
            SqlMapper.SetTypeMap(typeof(FuelStartData), new CustomPropertyTypeMap(typeof(FuelStartData), (type, columnName) => type.GetProperties().FirstOrDefault(prop => prop.GetCustomAttributes(false).OfType<ColumnAttribute>().Any(attr => attr.Name == columnName))));
            SqlMapper.SetTypeMap(typeof(FuelReduceData), new CustomPropertyTypeMap(typeof(FuelReduceData), (type, columnName) => type.GetProperties().FirstOrDefault(prop => prop.GetCustomAttributes(false).OfType<ColumnAttribute>().Any(attr => attr.Name == columnName))));
            GoogleHybridMapProvider.Instance.ApiKey = "AIzaSyBwyMHyzcR5EPdVwZdcf_BKizfnDQPZQHo";
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            Gmap.MapProvider = GoogleHybridMapProvider.Instance;
            Gmap.DragButton = MouseButton.Left;
            Gmap.Position = new PointLatLng(23.777176, 90.399452);
            Gmap.MinZoom = 1;
            Gmap.MaxZoom = 20;
            Gmap.Zoom = 8;
            Gmap.ShowCenter = false;
            try
            {
                bafChart = new Image { Source = (ImageSource)(new ImageSourceConverter().ConvertFrom(@"BAFMap.png")), Width = 620, Height = 690, Margin = new Thickness(-310, -345, -310, -345), RenderTransformOrigin = new Point(0.5, 0.5), IsHitTestVisible = false };
            }
            catch
            {
                MessageBox.Show("Couldn't load BAF Map Chart", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown(-1);
            }
            GPoint baf_anch = Gmap.FromLatLngToLocal(new PointLatLng(23.7250117359518, 90.50537109375));
            Canvas.SetLeft(bafChart, baf_anch.X);
            Canvas.SetTop(bafChart, baf_anch.Y);
            bafChart.RenderTransform = new ScaleTransform((0.0154453125 * Math.Pow(2, Gmap.Zoom - 1)), (0.015671875 * Math.Pow(2, Gmap.Zoom - 1)));
            drawCanvas.Children.Add(bafChart);
            Panel.SetZIndex(bafChart, 0);
            bafChart.Visibility = Visibility.Hidden;
        }

        private void drawOverlayMouseDown(object sender, MouseButtonEventArgs e)
        {
            clickedPoint = e.GetPosition(drawCanvas);
            currentPoint = clickedPoint;
            if (e.ChangedButton == MouseButton.Left)
            {
                if (mode == "none")
                {
                    var hit = drawCanvas.InputHitTest(clickedPoint);
                    if (hit is Ellipse)
                    {
                        ellipse = hit as Ellipse;
                        if (!(ellipse.Tag as RouteData).isDraggable)
                        {
                            ellipse = null;
                            draggable = false;
                        }
                        else
                        {
                            left = Canvas.GetLeft(ellipse);
                            top = Canvas.GetTop(ellipse);
                            draggable = true;
                        }
                    }
                }
            }
        }

        private void drawOverlayMouseMove(object sender, MouseEventArgs e)
        {
            currentPoint = e.GetPosition(drawCanvas);
            PointLatLng currentposition = Gmap.FromLocalToLatLng((int)currentPoint.X, (int)currentPoint.Y);
            pos_lat.Text = Math.Round(currentposition.Lat, 6).ToString();
            pos_long.Text = Math.Round(currentposition.Lng, 6).ToString();
            if (e.LeftButton == MouseButtonState.Pressed && draggable)
            {
                Canvas.SetLeft(ellipse, left + currentPoint.X - clickedPoint.X);
                Canvas.SetTop(ellipse, top + currentPoint.Y - clickedPoint.Y);
            }
            if (set)
            {
                line.X2 = currentPoint.X;
                line.Y2 = currentPoint.Y;
            }
        }

        private void drawOverlayMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (set && !Gmap.IsMouseCaptured) Gmap.CaptureMouse();
                if (currentPoint != clickedPoint && draggable)
                {
                    draggable = false;
                    RouteData datlink = ellipse.Tag as RouteData;
                    PointLatLng pos = Gmap.FromLocalToLatLng((int)Canvas.GetLeft(ellipse), (int)Canvas.GetTop(ellipse));
                    if (datlink.objType == "Circle")
                    {
                        datlink.pos1 = pos;
                        PointLatLng size = Gmap.FromLocalToLatLng((int)(Canvas.GetLeft(ellipse) + ellipse.ActualWidth / 2), (int)Canvas.GetTop(ellipse));
                        datlink.pos2 = size;
                        updateValue("circle");
                        routeProperties.Visibility = Visibility.Collapsed;
                        lineProperties.Visibility = Visibility.Collapsed;
                        polygonProperties.Visibility = Visibility.Collapsed;
                        circleProperties.Visibility = Visibility.Visible;
                    }
                    else if (datlink.objType == "Route" || datlink.objType == "Polygon")
                    {
                        if (datlink.componentID == 0)
                        {
                            datlink.pos1 = pos;
                            datlink.pos2 = pos;
                            for (int i = 0; i < drawCanvas.Children.Count; i++)
                            {
                                if (drawCanvas.Children[i] is Ellipse)
                                {
                                    if (((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objType == datlink.objType && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objID == datlink.objID && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).componentID == 1)
                                    {
                                        ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).pos1 = pos;
                                    }
                                }
                                else if (drawCanvas.Children[i] is Line)
                                {
                                    if (((drawCanvas.Children[i] as Line).Tag as RouteData).objType == datlink.objType && ((drawCanvas.Children[i] as Line).Tag as RouteData).objID == datlink.objID && ((drawCanvas.Children[i] as Line).Tag as RouteData).componentID == 0)
                                    {
                                        ((drawCanvas.Children[i] as Line).Tag as RouteData).pos1 = pos;
                                    }
                                }
                            }
                        }
                        else
                        {
                            datlink.pos2 = pos;
                            for (int i = 0; i < drawCanvas.Children.Count; i++)
                            {
                                if (drawCanvas.Children[i] is Ellipse)
                                {
                                    if (((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objType == datlink.objType && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objID == datlink.objID && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).componentID == (datlink.componentID + 1))
                                    {
                                        ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).pos1 = pos;
                                    }
                                }
                                else if (drawCanvas.Children[i] is Line)
                                {
                                    if (((drawCanvas.Children[i] as Line).Tag as RouteData).objType == datlink.objType && ((drawCanvas.Children[i] as Line).Tag as RouteData).objID == datlink.objID)
                                    {
                                        if (((drawCanvas.Children[i] as Line).Tag as RouteData).componentID == (datlink.componentID - 1))
                                        {
                                            ((drawCanvas.Children[i] as Line).Tag as RouteData).pos2 = pos;
                                        }
                                        else if (((drawCanvas.Children[i] as Line).Tag as RouteData).componentID == datlink.componentID)
                                        {
                                            ((drawCanvas.Children[i] as Line).Tag as RouteData).pos1 = pos;
                                        }

                                    }
                                }
                            }
                        }
                        if (datlink.objType == "Route")
                        {
                            addwaypntBtn.IsEnabled = true;
                            line = BindingOperations.GetBinding(ellipse, Ellipse.StrokeProperty).Source as Line;
                            refreshRoutePanel();
                            updateValue("route");
                            updateHeadingBoxInfo((ellipse.Tag as RouteData).objID);
                            lineProperties.Visibility = Visibility.Collapsed;
                            polygonProperties.Visibility = Visibility.Collapsed;
                            circleProperties.Visibility = Visibility.Collapsed;
                            routeProperties.Visibility = Visibility.Visible;
                            routeLineBlock.Visibility = Visibility.Visible;
                            routewaypointDataBlock.Visibility = Visibility.Visible;
                            routeProperties.IsEnabled = true;
                        }
                        else if (datlink.objType == "Polygon")
                        {
                            line = BindingOperations.GetBinding(ellipse, Ellipse.StrokeProperty).Source as Line;
                            updateValue("polygon");
                            circleProperties.Visibility = Visibility.Collapsed;
                            routeProperties.Visibility = Visibility.Collapsed;
                            lineProperties.Visibility = Visibility.Collapsed;
                            polygonProperties.Visibility = Visibility.Visible;
                        }
                    }
                    else if (datlink.objType == "Line")
                    {
                        if (datlink.componentID == 0)
                        {
                            datlink.pos1 = pos;
                            for (int i = 0; i < drawCanvas.Children.Count; i++)
                            {
                                if (drawCanvas.Children[i] is Ellipse)
                                {
                                    if (((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objType == datlink.objType && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objID == datlink.objID && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).componentID == 1)
                                    {
                                        ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).pos1 = pos;
                                    }
                                }
                                else if (drawCanvas.Children[i] is Line)
                                {
                                    if (((drawCanvas.Children[i] as Line).Tag as RouteData).objType == datlink.objType && ((drawCanvas.Children[i] as Line).Tag as RouteData).objID == datlink.objID && ((drawCanvas.Children[i] as Line).Tag as RouteData).componentID == 0)
                                    {
                                        ((drawCanvas.Children[i] as Line).Tag as RouteData).pos1 = pos;
                                    }
                                }
                            }
                        }
                        else
                        {
                            datlink.pos2 = pos;
                            for (int i = 0; i < drawCanvas.Children.Count; i++)
                            {
                                if (drawCanvas.Children[i] is Ellipse)
                                {
                                    if (((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objType == datlink.objType && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objID == datlink.objID && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).componentID == 0)
                                    {
                                        ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).pos2 = pos;
                                    }
                                }
                                else if (drawCanvas.Children[i] is Line)
                                {
                                    if (((drawCanvas.Children[i] as Line).Tag as RouteData).objType == datlink.objType && ((drawCanvas.Children[i] as Line).Tag as RouteData).objID == datlink.objID && ((drawCanvas.Children[i] as Line).Tag as RouteData).componentID == 0)
                                    {
                                        ((drawCanvas.Children[i] as Line).Tag as RouteData).pos2 = pos;
                                    }
                                }
                            }
                        }
                        line = BindingOperations.GetBinding(ellipse, Ellipse.StrokeProperty).Source as Line;
                        updateValue("line");
                        polygonProperties.Visibility = Visibility.Collapsed;
                        circleProperties.Visibility = Visibility.Collapsed;
                        routeProperties.Visibility = Visibility.Collapsed;
                        lineProperties.Visibility = Visibility.Visible;
                    }
                }
                if (currentPoint == clickedPoint)
                {
                    PointLatLng position = Gmap.FromLocalToLatLng(Convert.ToInt32(clickedPoint.X), Convert.ToInt32(clickedPoint.Y));
                    switch (mode)
                    {
                        case "none":
                            /////////
                            addwaypntBtn.IsEnabled = false;
                            routeLineBlock.Visibility = Visibility.Collapsed;
                            /////////
                            var hit = drawCanvas.InputHitTest(clickedPoint);
                            if (hit is Line)
                            {
                                line = hit as Line;
                                RouteData datlink = line.Tag as RouteData;
                                if (datlink.objType == "Route")
                                {
                                    addwaypntBtn.IsEnabled = true;
                                    ellipse = BindingOperations.GetBinding(line, Line.X2Property).Source as Ellipse;
                                    refreshRoutePanel();
                                    updateValue("route");
                                    lineProperties.Visibility = Visibility.Collapsed;
                                    polygonProperties.Visibility = Visibility.Collapsed;
                                    circleProperties.Visibility = Visibility.Collapsed;
                                    routeProperties.Visibility = Visibility.Visible;
                                    routeLineBlock.Visibility = Visibility.Visible;
                                    routewaypointDataBlock.Visibility = Visibility.Visible;
                                    routeProperties.IsEnabled = true;
                                }
                                else if (datlink.objType == "Line")
                                {
                                    ellipse = BindingOperations.GetBinding(line, Line.X2Property).Source as Ellipse;
                                    updateValue("line");
                                    polygonProperties.Visibility = Visibility.Collapsed;
                                    circleProperties.Visibility = Visibility.Collapsed;
                                    routeProperties.Visibility = Visibility.Collapsed;
                                    lineProperties.Visibility = Visibility.Visible;
                                }
                                else if (datlink.objType == "Polygon")
                                {
                                    ellipse = BindingOperations.GetBinding(line, Line.X2Property).Source as Ellipse;
                                    updateValue("polygon");
                                    circleProperties.Visibility = Visibility.Collapsed;
                                    routeProperties.Visibility = Visibility.Collapsed;
                                    lineProperties.Visibility = Visibility.Collapsed;
                                    polygonProperties.Visibility = Visibility.Visible;
                                }
                            }
                            else if (hit is Ellipse)
                            {
                                ellipse = hit as Ellipse;
                                RouteData datlink = ellipse.Tag as RouteData;
                                if (datlink.objType == "Circle")
                                {
                                    updateValue("circle");
                                    routeProperties.Visibility = Visibility.Collapsed;
                                    lineProperties.Visibility = Visibility.Collapsed;
                                    polygonProperties.Visibility = Visibility.Collapsed;
                                    circleProperties.Visibility = Visibility.Visible;
                                }
                                else if (datlink.objType == "Route")
                                {
                                    addwaypntBtn.IsEnabled = true;
                                    line = BindingOperations.GetBinding(ellipse, Ellipse.StrokeProperty).Source as Line;
                                    refreshRoutePanel();
                                    updateValue("route");
                                    lineProperties.Visibility = Visibility.Collapsed;
                                    polygonProperties.Visibility = Visibility.Collapsed;
                                    circleProperties.Visibility = Visibility.Collapsed;
                                    routeProperties.Visibility = Visibility.Visible;
                                    routeLineBlock.Visibility = Visibility.Visible;
                                    routewaypointDataBlock.Visibility = Visibility.Visible;
                                    routeProperties.IsEnabled = true;
                                }
                                else if (datlink.objType == "Line")
                                {
                                    line = BindingOperations.GetBinding(ellipse, Ellipse.StrokeProperty).Source as Line;
                                    updateValue("line");
                                    polygonProperties.Visibility = Visibility.Collapsed;
                                    circleProperties.Visibility = Visibility.Collapsed;
                                    routeProperties.Visibility = Visibility.Collapsed;
                                    lineProperties.Visibility = Visibility.Visible;
                                }
                                else if (datlink.objType == "Polygon")
                                {
                                    line = BindingOperations.GetBinding(ellipse, Ellipse.StrokeProperty).Source as Line;
                                    updateValue("polygon");
                                    circleProperties.Visibility = Visibility.Collapsed;
                                    routeProperties.Visibility = Visibility.Collapsed;
                                    lineProperties.Visibility = Visibility.Collapsed;
                                    polygonProperties.Visibility = Visibility.Visible;
                                }
                            }
                            else
                            {
                                clearCache();
                            }
                            break;
                        case "route":
                            if (set)
                            {
                                drawn = true;
                                count++;
                                markerCount++;
                                boxCount++;
                                ellipse = new Ellipse
                                {
                                    Fill = Brushes.Transparent,
                                    Width = 20,
                                    Height = 20
                                };
                                Panel.SetZIndex(ellipse, 5);
                                Canvas.SetLeft(ellipse, clickedPoint.X);
                                Canvas.SetTop(ellipse, clickedPoint.Y);
                                ellipse.SetBinding(Ellipse.StrokeProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeProperty) });
                                ellipse.SetBinding(Ellipse.StrokeThicknessProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeThicknessProperty) });
                                ellipse.SetBinding(Ellipse.MarginProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.ActualWidthProperty), Converter = new CircleRadiusConverter() });
                                line.SetBinding(Line.X2Property, new Binding { Source = ellipse, Path = new PropertyPath(Canvas.LeftProperty) });
                                line.SetBinding(Line.Y2Property, new Binding { Source = ellipse, Path = new PropertyPath(Canvas.TopProperty) });
                                (line.Tag as RouteData).pos2 = position;
                                var pos1 = (line.Tag as RouteData).pos1;
                                var tp = (line.Tag as RouteData).type;
                                headingBox = new StackPanel { Orientation = Orientation.Vertical, Width = 120, Height = 160, Background = Brushes.Transparent, Margin = new Thickness(-60, -80, -60, -80) };
                                headingBox.RenderTransformOrigin = new Point(0.5, 0.5);
                                RotateTransform rt = new RotateTransform();
                                headingBox.RenderTransform = rt;
                                headingBox.Children.Add(new Path { Stroke = Brushes.Black, StrokeThickness = 3, Fill = Brushes.DarkRed, Data = PathGeometry.Parse("M1.5,1.5 L1.5,38.5 118.5,38.5 Z") });
                                headingBox.Children.Add(new Label { Width = 120, Height = 30, Foreground = Brushes.White, Background = Brushes.DarkGray, BorderBrush = Brushes.Black, BorderThickness = new Thickness(3, 0, 0, 3), FontSize = 14 });
                                headingBox.Children.Add(new Label { Width = 120, Height = 30, Foreground = Brushes.White, Background = Brushes.DarkGray, BorderBrush = Brushes.Black, BorderThickness = new Thickness(3, 0, 0, 3), FontSize = 14 });
                                headingBox.Children.Add(new Label { Width = 120, Height = 30, Foreground = Brushes.White, Background = Brushes.DarkGray, BorderBrush = Brushes.Black, BorderThickness = new Thickness(3, 0, 0, 3), FontSize = 14 });
                                headingBox.Children.Add(new Label { Width = 120, Height = 30, Foreground = Brushes.White, Background = Brushes.DarkGray, BorderBrush = Brushes.Black, BorderThickness = new Thickness(3, 0, 0, 3), FontSize = 14 });
                                MultiBinding X = new MultiBinding { Converter = new OffsetConverterX() };
                                X.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.X1Property) });
                                X.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.Y1Property) });
                                X.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.X2Property) });
                                X.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.Y2Property) });
                                X.Bindings.Add(new Binding { Source = headingBox, Path = new PropertyPath(StackPanel.ActualWidthProperty) });
                                X.Bindings.Add(new Binding("OffSet") { Source = main });
                                headingBox.SetBinding(Canvas.LeftProperty, X);
                                MultiBinding Y = new MultiBinding { Converter = new OffsetConverterY() };
                                Y.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.X1Property) });
                                Y.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.Y1Property) });
                                Y.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.X2Property) });
                                Y.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.Y2Property) });
                                Y.Bindings.Add(new Binding { Source = headingBox, Path = new PropertyPath(StackPanel.ActualWidthProperty) });
                                Y.Bindings.Add(new Binding("OffSet") { Source = main });
                                headingBox.SetBinding(Canvas.TopProperty, Y);
                                MultiBinding θ = new MultiBinding { Converter = new OffsetConverterθ() };
                                θ.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.X1Property) });
                                θ.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.Y1Property) });
                                θ.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.X2Property) });
                                θ.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.Y2Property) });
                                BindingOperations.SetBinding(rt, RotateTransform.AngleProperty, θ);
                                Line temp = new Line
                                {
                                    Stroke = Brushes.Blue,
                                    StrokeThickness = 2,
                                    StrokeStartLineCap = PenLineCap.Round,
                                    StrokeEndLineCap = PenLineCap.Round,
                                    StrokeDashCap = PenLineCap.Flat,
                                    StrokeDashArray = new DoubleCollection() { 5, 0 },
                                    X1 = clickedPoint.X,
                                    Y1 = clickedPoint.Y,
                                    X2 = clickedPoint.X,
                                    Y2 = clickedPoint.Y
                                };
                                temp.SetBinding(Line.StrokeProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                                temp.SetBinding(Line.StrokeThicknessProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeThicknessProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                                temp.SetBinding(Line.StrokeStartLineCapProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeStartLineCapProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                                temp.SetBinding(Line.StrokeEndLineCapProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeEndLineCapProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                                temp.SetBinding(Line.StrokeDashCapProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeDashCapProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                                temp.SetBinding(Line.StrokeDashArrayProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeDashArrayProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                                line = temp;
                                Panel.SetZIndex(line, 4);
                                line.SetBinding(Line.X1Property, new Binding { Source = ellipse, Path = new PropertyPath(Canvas.LeftProperty) });
                                line.SetBinding(Line.Y1Property, new Binding { Source = ellipse, Path = new PropertyPath(Canvas.TopProperty) });
                                drawCanvas.Children.Add(ellipse);
                                drawCanvas.Children.Add(line);
                                drawCanvas.Children.Add(headingBox);
                                ellipse.Tag = new RouteData(drawCanvas, baseunit.baltUnit, baseunit.bdistUnit, baseunit.bspeedUnit, baseunit.bfuelUnit, baseunit.blfftUnit) { objType = "Route", objID = routeCount, componentID = markerCount, isDraggable = true, pos1 = pos1, pos2 = position, startingfuel = Convert.ToDouble(routeStartingFuelBox.Text), type = tp, totaldistanceUnit = baseunit.bdistUnit, distanceUnit = baseunit.bdistUnit };
                                line.Tag = new RouteData(drawCanvas, baseunit.baltUnit, baseunit.bdistUnit, baseunit.bspeedUnit, baseunit.bfuelUnit, baseunit.blfftUnit) { objType = "Route", objID = routeCount, componentID = count, isDraggable = false, pos1 = position, startingfuel = Convert.ToDouble(routeStartingFuelBox.Text), type = "Enroute", totaldistanceUnit = baseunit.bdistUnit, distanceUnit = baseunit.bdistUnit };
                                headingBox.Tag = new RouteData(drawCanvas, baseunit.baltUnit, baseunit.bdistUnit, baseunit.bspeedUnit, baseunit.bfuelUnit, baseunit.blfftUnit) { objType = "Route", objID = routeCount, componentID = boxCount, isDraggable = false, pos1 = pos1, pos2 = position, startingfuel = Convert.ToDouble(routeStartingFuelBox.Text), type = tp, totaldistanceUnit = baseunit.bdistUnit, distanceUnit = baseunit.bdistUnit };
                                (ellipse.Tag as RouteData).setpData(pdata, routeAltBox.SelectedIndex, routeSpeedBox.SelectedIndex + 1);
                                (line.Tag as RouteData).setpData(pdata, routeAltBox.SelectedIndex, routeSpeedBox.SelectedIndex + 1);
                                (headingBox.Tag as RouteData).setpData(pdata, routeAltBox.SelectedIndex, routeSpeedBox.SelectedIndex + 1);
                                (headingBox.Children[1] as Label).SetBinding(Label.ContentProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.TagProperty), Converter = new HeadingBoxInfoConverter(), ConverterParameter = "track" });
                                (headingBox.Children[2] as Label).SetBinding(Label.ContentProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.TagProperty), Converter = new HeadingBoxInfoConverter(), ConverterParameter = "time" });
                                (headingBox.Children[3] as Label).SetBinding(Label.ContentProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.TagProperty), Converter = new HeadingBoxInfoConverter(), ConverterParameter = "fuel" });
                                (headingBox.Children[4] as Label).SetBinding(Label.ContentProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.TagProperty), Converter = new HeadingBoxInfoConverter(), ConverterParameter = "alt" });
                                updateValue(mode);
                                updateHeadingBoxInfo((ellipse.Tag as RouteData).objID);
                            }
                            else
                            {
                                markerCount++;
                                line = new Line
                                {
                                    Stroke = Brushes.Blue,
                                    StrokeThickness = 2,
                                    StrokeStartLineCap = PenLineCap.Round,
                                    StrokeEndLineCap = PenLineCap.Round,
                                    StrokeDashCap = PenLineCap.Flat,
                                    StrokeDashArray = new DoubleCollection() { 5, 0 },
                                    X1 = clickedPoint.X,
                                    Y1 = clickedPoint.Y,
                                    X2 = clickedPoint.X,
                                    Y2 = clickedPoint.Y
                                };
                                ellipse = new Ellipse
                                {
                                    Fill = Brushes.Transparent,
                                    Width = 20,
                                    Height = 20
                                };
                                Panel.SetZIndex(line, 4);
                                Panel.SetZIndex(ellipse, 5);
                                Canvas.SetLeft(ellipse, clickedPoint.X);
                                Canvas.SetTop(ellipse, clickedPoint.Y);
                                ellipse.SetBinding(Ellipse.StrokeProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeProperty) });
                                ellipse.SetBinding(Ellipse.StrokeThicknessProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeThicknessProperty) });
                                ellipse.SetBinding(Ellipse.MarginProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.ActualWidthProperty), Converter = new CircleRadiusConverter() });
                                line.SetBinding(Line.X1Property, new Binding { Source = ellipse, Path = new PropertyPath(Canvas.LeftProperty) });
                                line.SetBinding(Line.Y1Property, new Binding { Source = ellipse, Path = new PropertyPath(Canvas.TopProperty) });
                                drawCanvas.Children.Add(ellipse);
                                drawCanvas.Children.Add(line);
                                ellipse.Tag = new RouteData(drawCanvas, baseunit.baltUnit, baseunit.bdistUnit, baseunit.bspeedUnit, baseunit.bfuelUnit, baseunit.blfftUnit) { objType = "Route", objID = routeCount, componentID = markerCount, isDraggable = true, pos1 = position, pos2 = position, startingfuel = Convert.ToDouble(routeStartingFuelBox.Text), type = "Origin", totaldistanceUnit = baseunit.bdistUnit, distanceUnit = baseunit.bdistUnit };
                                line.Tag = new RouteData(drawCanvas, baseunit.baltUnit, baseunit.bdistUnit, baseunit.bspeedUnit, baseunit.bfuelUnit, baseunit.blfftUnit) { objType = "Route", objID = routeCount, componentID = count, isDraggable = false, pos1 = position, startingfuel = Convert.ToDouble(routeStartingFuelBox.Text), type = "Starting", totaldistanceUnit = baseunit.bdistUnit, distanceUnit = baseunit.bdistUnit };
                                (ellipse.Tag as RouteData).setpData(pdata, routeAltBox.SelectedIndex, routeSpeedBox.SelectedIndex + 1);
                                (line.Tag as RouteData).setpData(pdata, routeAltBox.SelectedIndex, routeSpeedBox.SelectedIndex + 1);
                                routewaypointDataBlock.Visibility = Visibility.Visible;
                                updateValue(mode);
                                Gmap.CaptureMouse();
                                set = true;
                            }
                            break;
                        case "line":
                            if (set)
                            {
                                markerCount++;
                                (ellipse.Tag as RouteData).objID = lineCount;
                                ellipse = new Ellipse
                                {
                                    Width = 10,
                                    Height = 10
                                };
                                Panel.SetZIndex(ellipse, 5);
                                Canvas.SetLeft(ellipse, clickedPoint.X);
                                Canvas.SetTop(ellipse, clickedPoint.Y);
                                ellipse.SetBinding(Ellipse.StrokeProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeProperty) });
                                ellipse.SetBinding(Ellipse.StrokeThicknessProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeThicknessProperty) });
                                ellipse.SetBinding(Ellipse.FillProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeProperty) });
                                ellipse.SetBinding(Ellipse.MarginProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.ActualWidthProperty), Converter = new CircleRadiusConverter() });
                                (line.Tag as RouteData).pos2 = position;
                                (line.Tag as RouteData).objID = lineCount;
                                (line.Tag as RouteData).componentID = lineCount;
                                ellipse.Tag = new RouteData(drawCanvas, "KM", "KM", "KPH", "KG", "PER KM") { objType = "Line", objID = lineCount, componentID = markerCount, pos1 = (line.Tag as RouteData).pos1, pos2 = position, isDraggable = true };
                                line.SetBinding(Line.X2Property, new Binding { Source = ellipse, Path = new PropertyPath(Canvas.LeftProperty) });
                                line.SetBinding(Line.Y2Property, new Binding { Source = ellipse, Path = new PropertyPath(Canvas.TopProperty) });
                                drawCanvas.Children.Add(ellipse);
                                set = false;
                                Gmap.ReleaseMouseCapture();
                                mode = "none";
                                drawCanvas.Cursor = Cursors.Arrow;
                                updateValue("line");
                                drawn = false;
                                polygonProperties.Visibility = Visibility.Collapsed;
                                circleProperties.Visibility = Visibility.Collapsed;
                                routeProperties.Visibility = Visibility.Collapsed;
                                lineProperties.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                markerCount++;
                                line = new Line
                                {
                                    Stroke = Brushes.Blue,
                                    StrokeThickness = 2,
                                    StrokeStartLineCap = PenLineCap.Round,
                                    StrokeEndLineCap = PenLineCap.Round,
                                    StrokeDashCap = PenLineCap.Flat,
                                    StrokeDashArray = new DoubleCollection() { 5, 0 },
                                    X1 = clickedPoint.X,
                                    Y1 = clickedPoint.Y,
                                    X2 = clickedPoint.X,
                                    Y2 = clickedPoint.Y
                                };
                                ellipse = new Ellipse
                                {
                                    Width = 10,
                                    Height = 10
                                };
                                Panel.SetZIndex(line, 4);
                                Panel.SetZIndex(ellipse, 5);
                                Canvas.SetLeft(ellipse, clickedPoint.X);
                                Canvas.SetTop(ellipse, clickedPoint.Y);
                                ellipse.SetBinding(Ellipse.StrokeProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeProperty) });
                                ellipse.SetBinding(Ellipse.StrokeThicknessProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeThicknessProperty) });
                                ellipse.SetBinding(Ellipse.FillProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeProperty) });
                                ellipse.SetBinding(Ellipse.MarginProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.ActualWidthProperty), Converter = new CircleRadiusConverter() });
                                ellipse.Tag = new RouteData(drawCanvas, "KM", "KM", "KPH", "KG", "PER KM") { objType = "Line", componentID = markerCount, pos1 = position, pos2 = position, isDraggable = true };
                                line.Tag = new RouteData(drawCanvas, "KM", "KM", "KPH", "KG", "PER KM") { objType = "Line", pos1 = position, isDraggable = false };
                                line.SetBinding(Line.X1Property, new Binding { Source = ellipse, Path = new PropertyPath(Canvas.LeftProperty) });
                                line.SetBinding(Line.Y1Property, new Binding { Source = ellipse, Path = new PropertyPath(Canvas.TopProperty) });
                                drawCanvas.Children.Add(line);
                                drawCanvas.Children.Add(ellipse);
                                Gmap.CaptureMouse();
                                set = true;
                            }
                            break;
                        case "circle":
                            circleCount++;
                            PointLatLng sizePoint;
                            ellipse = new Ellipse
                            {
                                Width = 300,
                                Height = 300,
                                Stroke = Brushes.Blue,
                                StrokeThickness = 2,
                                StrokeDashCap = PenLineCap.Flat,
                                StrokeDashArray = new DoubleCollection() { 5, 0 }
                            };
                            sizePoint = Gmap.FromLocalToLatLng((int)(clickedPoint.X + ellipse.Width / 2), (int)clickedPoint.Y);
                            Panel.SetZIndex(ellipse, 5);
                            Canvas.SetLeft(ellipse, clickedPoint.X);
                            Canvas.SetTop(ellipse, clickedPoint.Y);
                            ellipse.SetBinding(Ellipse.MarginProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.ActualWidthProperty), Converter = new CircleRadiusConverter() });
                            ellipse.Tag = new RouteData(drawCanvas, "KM", "KM", "KPH", "KG", "PER KM") { objType = "Circle", objID = circleCount, componentID = circleCount, isDraggable = true, pos1 = position, pos2 = sizePoint };
                            drawCanvas.Children.Add(ellipse);
                            mode = "none";
                            drawCanvas.Cursor = Cursors.Arrow;
                            updateValue("circle");
                            drawn = false;
                            routeProperties.Visibility = Visibility.Collapsed;
                            lineProperties.Visibility = Visibility.Collapsed;
                            polygonProperties.Visibility = Visibility.Collapsed;
                            circleProperties.Visibility = Visibility.Visible;
                            break;
                        case "orbit":
                            break;
                        case "polygon":
                            if (set)
                            {
                                drawn = true;
                                count++;
                                markerCount++;
                                ellipse = new Ellipse
                                {
                                    Width = 10,
                                    Height = 10
                                };
                                Panel.SetZIndex(ellipse, 5);
                                Canvas.SetLeft(ellipse, clickedPoint.X);
                                Canvas.SetTop(ellipse, clickedPoint.Y);
                                ellipse.SetBinding(Ellipse.StrokeProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeProperty) });
                                ellipse.SetBinding(Ellipse.StrokeThicknessProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeThicknessProperty) });
                                ellipse.SetBinding(Ellipse.FillProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeProperty) });
                                ellipse.SetBinding(Ellipse.MarginProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.ActualWidthProperty), Converter = new CircleRadiusConverter() });
                                ellipse.Tag = new RouteData(drawCanvas, "KM", "KM", "KPH", "KG", "PER KM") { objType = "Polygon", objID = polygonCount, componentID = markerCount, isDraggable = true, pos1 = (line.Tag as RouteData).pos1, pos2 = position };
                                line.SetBinding(Line.X2Property, new Binding { Source = ellipse, Path = new PropertyPath(Canvas.LeftProperty) });
                                line.SetBinding(Line.Y2Property, new Binding { Source = ellipse, Path = new PropertyPath(Canvas.TopProperty) });
                                (line.Tag as RouteData).pos2 = position;
                                Line temp = new Line
                                {
                                    Stroke = Brushes.Blue,
                                    StrokeThickness = 2,
                                    StrokeStartLineCap = PenLineCap.Round,
                                    StrokeEndLineCap = PenLineCap.Round,
                                    StrokeDashCap = PenLineCap.Flat,
                                    StrokeDashArray = new DoubleCollection() { 5, 0 },
                                    X1 = clickedPoint.X,
                                    Y1 = clickedPoint.Y,
                                    X2 = clickedPoint.X,
                                    Y2 = clickedPoint.Y
                                };
                                temp.SetBinding(Line.StrokeProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                                temp.SetBinding(Line.StrokeThicknessProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeThicknessProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                                temp.SetBinding(Line.StrokeStartLineCapProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeStartLineCapProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                                temp.SetBinding(Line.StrokeEndLineCapProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeEndLineCapProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                                temp.SetBinding(Line.StrokeDashCapProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeDashCapProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                                temp.SetBinding(Line.StrokeDashArrayProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeDashArrayProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                                line = temp;
                                Panel.SetZIndex(line, 4);
                                line.SetBinding(Line.X1Property, new Binding { Source = ellipse, Path = new PropertyPath(Canvas.LeftProperty) });
                                line.SetBinding(Line.Y1Property, new Binding { Source = ellipse, Path = new PropertyPath(Canvas.TopProperty) });
                                line.Tag = new RouteData(drawCanvas, "KM", "KM", "KPH", "KG", "PER KM") { objType = "Polygon", objID = polygonCount, componentID = count, isDraggable = true, pos1 = position };
                                drawCanvas.Children.Add(ellipse);
                                drawCanvas.Children.Add(line);
                            }
                            else
                            {
                                markerCount++;
                                line = new Line
                                {
                                    Stroke = Brushes.Blue,
                                    StrokeThickness = 2,
                                    StrokeStartLineCap = PenLineCap.Round,
                                    StrokeEndLineCap = PenLineCap.Round,
                                    StrokeDashCap = PenLineCap.Flat,
                                    StrokeDashArray = new DoubleCollection() { 5, 0 },
                                    X1 = clickedPoint.X,
                                    Y1 = clickedPoint.Y,
                                    X2 = clickedPoint.X,
                                    Y2 = clickedPoint.Y
                                };
                                ellipse = new Ellipse
                                {
                                    Width = 10,
                                    Height = 10
                                };
                                Panel.SetZIndex(line, 4);
                                Panel.SetZIndex(ellipse, 5);
                                Canvas.SetLeft(ellipse, clickedPoint.X);
                                Canvas.SetTop(ellipse, clickedPoint.Y);
                                ellipse.SetBinding(Ellipse.StrokeProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeProperty) });
                                ellipse.SetBinding(Ellipse.StrokeThicknessProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeThicknessProperty) });
                                ellipse.SetBinding(Ellipse.FillProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeProperty) });
                                ellipse.SetBinding(Ellipse.MarginProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.ActualWidthProperty), Converter = new CircleRadiusConverter() });
                                ellipse.Tag = new RouteData(drawCanvas, "KM", "KM", "KPH", "KG", "PER KM") { objType = "Polygon", objID = polygonCount, componentID = markerCount, isDraggable = true, pos1 = position, pos2 = position };
                                line.Tag = new RouteData(drawCanvas, "KM", "KM", "KPH", "KG", "PER KM") { objType = "Polygon", objID = polygonCount, componentID = count, isDraggable = false, pos1 = position };
                                line.SetBinding(Line.X1Property, new Binding { Source = ellipse, Path = new PropertyPath(Canvas.LeftProperty) });
                                line.SetBinding(Line.Y1Property, new Binding { Source = ellipse, Path = new PropertyPath(Canvas.TopProperty) });
                                drawCanvas.Children.Add(ellipse);
                                drawCanvas.Children.Add(line);
                                Gmap.CaptureMouse();
                                set = true;
                            }
                            break;
                        case "image":
                            break;
                        case "surface":
                            break;
                        default:
                            return;
                    }
                }
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                if (currentPoint == clickedPoint)
                {
                    if (!set && !drawn && mode != "none")
                    {
                        if (mode == "line") lineCount--;
                        else if (mode == "route") routeCount--;
                        else if (mode == "polygon") polygonCount--;
                        mode = "none";
                        drawCanvas.Cursor = Cursors.Arrow;
                    }
                    else if (set && !drawn)
                    {
                        if (mode == "line") lineCount--;
                        else if (mode == "route") routeCount--;
                        else if (mode == "polygon") polygonCount--;
                        if (ellipse != null) drawCanvas.Children.Remove(ellipse);
                        if (line != null) drawCanvas.Children.Remove(line);
                        set = false;
                        drawn = false;
                        mode = "none";
                        drawCanvas.Cursor = Cursors.Arrow;
                        Gmap.ReleaseMouseCapture();
                    }
                    else if (set && drawn)
                    {
                        Line temp = BindingOperations.GetBinding(line, Line.StrokeProperty).Source as Line;
                        drawCanvas.Children.Remove(line);
                        line = temp;
                        (ellipse.Tag as RouteData).type = "Landing";
                        (line.Tag as RouteData).type = "Landing";
                        Gmap.ReleaseMouseCapture();
                        updateValue(mode);
                        if (mode == "route")
                        {
                            updateHeadingBoxInfo((ellipse.Tag as RouteData).objID);
                            lineProperties.Visibility = Visibility.Collapsed;
                            polygonProperties.Visibility = Visibility.Collapsed;
                            circleProperties.Visibility = Visibility.Collapsed;
                            routeLineBlock.Visibility = Visibility.Visible;
                            routeProperties.IsEnabled = true;
                        }
                        else if (mode == "polygon")
                        {
                            circleProperties.Visibility = Visibility.Collapsed;
                            routeProperties.Visibility = Visibility.Collapsed;
                            lineProperties.Visibility = Visibility.Collapsed;
                            polygonProperties.Visibility = Visibility.Visible;
                        }
                        set = false;
                        drawn = false;
                        mode = "none";
                        drawCanvas.Cursor = Cursors.Arrow;
                    }
                }
            }
        }

        private void drawOverlayMouseWheel(object sender, MouseWheelEventArgs e)
        {

        }

        private void GmapMouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void GmapMouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void GmapZoomChanged()
        {
            repositionPoints();
            (bafChart.RenderTransform as ScaleTransform).ScaleX = 0.0154453125 * Math.Pow(2, Gmap.Zoom - 1);
            (bafChart.RenderTransform as ScaleTransform).ScaleY = 0.015671875 * Math.Pow(2, Gmap.Zoom - 1);
        }

        private void GmapDrag()
        {
            repositionPoints();
        }

        private void globalButtonOperations(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Name)
            {
                case "mapBtn":
                    bafChart.Visibility = Visibility.Visible;
                    break;
                case "platformsBtn":
                    Platforms platform_editor = new Platforms();
                    platform_editor.ShowDialog();
                    break;
                case "surfaceUnitsBtn":
                    break;
                case "refPointsBtn":
                    break;
                case "channelsBtn":
                    break;
                case "coordBtn":
                    break;
                default:
                    return;
            }
        }

        private void mapDrawOperations(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            switch (btn.Name)
            {
                case "msnBtn":
                    MissionPicker mspk = new MissionPicker();
                    if (mspk.ShowDialog().Value)
                    {
                        clearCache();
                        loadAircraft(mspk.aircraftListbx.SelectedValue.ToString());
                    }
                    break;
                case "addwaypntBtn":
                    markerCount = -1;
                    count = 0;
                    boxCount = -1;
                    routeCount++;
                    mode = "route";
                    ellipse = null;
                    line = null;
                    drawCanvas.Cursor = Cursors.Cross;
                    break;
                case "lineBtn":
                    clearCache();
                    markerCount = -1;
                    lineCount++;
                    mode = "line";
                    ellipse = null;
                    line = null;
                    drawCanvas.Cursor = Cursors.Cross;
                    break;
                case "circleBtn":
                    clearCache();
                    mode = "circle";
                    ellipse = null;
                    line = null;
                    drawCanvas.Cursor = Cursors.Cross;
                    break;
                case "orbitBtn":
                    clearCache();
                    mode = "orbit";
                    drawCanvas.Cursor = Cursors.Cross;
                    break;
                case "plygnBtn":
                    clearCache();
                    markerCount = -1;
                    count = 0;
                    polygonCount++;
                    mode = "polygon";
                    ellipse = null;
                    line = null;
                    drawCanvas.Cursor = Cursors.Cross;
                    break;
                case "imgBtn":
                    //mode = "image";
                    Console.WriteLine((ellipse.Tag as RouteData).componentID);
                    Console.WriteLine((ellipse.Tag as RouteData).totaldistance);
                    break;
                case "surfaceBtn":
                    mode = "surface";
                    break;
                default:
                    mode = "none";
                    break;
            }
        }

        private void unitConverterBtnClick(object sender, RoutedEventArgs e)
        {

        }

        private void increaseElements(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            switch (btn.Name)
            {
                case "waypntPlus":
                    break;
                case "headingPlus":
                    break;
                case "fuelPlus":
                    break;
                case "labelPlus":
                    break;
                default:
                    break;
            }
        }

        private void reduceElements(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            switch (btn.Name)
            {
                case "waypntMinus":
                    break;
                case "headingMinus":
                    break;
                case "fuelMinus":
                    break;
                case "labelMinus":
                    break;
                default:
                    break;
            }
        }

        private void keyControls(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (!set && !drawn && mode != "none")
                {
                    if (mode == "line") lineCount--;
                    else if (mode == "route") routeCount--;
                    else if (mode == "polygon") polygonCount--;
                    mode = "none";
                    drawCanvas.Cursor = Cursors.Arrow;
                }
                else if (set && !drawn)
                {
                    if (mode == "line") lineCount--;
                    else if (mode == "route") routeCount--;
                    else if (mode == "polygon") polygonCount--;
                    if (ellipse != null) drawCanvas.Children.Remove(ellipse);
                    if (line != null) drawCanvas.Children.Remove(line);
                    set = false;
                    drawn = false;
                    mode = "none";
                    drawCanvas.Cursor = Cursors.Arrow;
                    Gmap.ReleaseMouseCapture();
                }
                else if (set && drawn)
                {
                    Line temp = BindingOperations.GetBinding(line, Line.StrokeProperty).Source as Line;
                    drawCanvas.Children.Remove(line);
                    line = temp;
                    (ellipse.Tag as RouteData).type = "Landing";
                    (line.Tag as RouteData).type = "Landing";
                    Gmap.ReleaseMouseCapture();
                    updateValue(mode);
                    if (mode == "route")
                    {
                        updateHeadingBoxInfo((ellipse.Tag as RouteData).objID);
                        lineProperties.Visibility = Visibility.Collapsed;
                        polygonProperties.Visibility = Visibility.Collapsed;
                        circleProperties.Visibility = Visibility.Collapsed;
                        routeLineBlock.Visibility = Visibility.Visible;
                        routeProperties.IsEnabled = true;
                    }
                    else if (mode == "polygon")
                    {
                        circleProperties.Visibility = Visibility.Collapsed;
                        routeProperties.Visibility = Visibility.Collapsed;
                        lineProperties.Visibility = Visibility.Collapsed;
                        polygonProperties.Visibility = Visibility.Visible;
                    }
                    set = false;
                    drawn = false;
                    mode = "none";
                    drawCanvas.Cursor = Cursors.Arrow;
                }
                else
                {
                    clearCache();
                }
            }
            if (e.Key == Key.Delete && ellipse != null)
            {
                int num;
                if ((ellipse.Tag as RouteData).objType == "Line")
                {
                    num = (ellipse.Tag as RouteData).objID;
                    for (int i = 0; i < drawCanvas.Children.Count; i++)
                    {
                        if (drawCanvas.Children[i] is Ellipse)
                        {
                            if (((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objType == "Line" && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objID == num)
                            {
                                drawCanvas.Children.Remove(drawCanvas.Children[i]);
                                i--;
                            }
                        }
                        else if (drawCanvas.Children[i] is Line)
                        {
                            if (((drawCanvas.Children[i] as Line).Tag as RouteData).objType == "Line" && ((drawCanvas.Children[i] as Line).Tag as RouteData).objID == num)
                            {
                                drawCanvas.Children.Remove(drawCanvas.Children[i]);
                                i--;
                            }
                        }
                        lineProperties.Visibility = Visibility.Collapsed;
                        line = null;
                        ellipse = null;
                    }
                }
                else if ((ellipse.Tag as RouteData).objType == "Circle")
                {
                    drawCanvas.Children.Remove(ellipse);
                    circleProperties.Visibility = Visibility.Collapsed;
                    ellipse = null;
                }
                else if ((ellipse.Tag as RouteData).objType == "Polygon")
                {
                    num = (ellipse.Tag as RouteData).objID;
                    for (int i = 0; i < drawCanvas.Children.Count; i++)
                    {
                        if (drawCanvas.Children[i] is Ellipse)
                        {
                            if (((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objType == "Polygon" && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objID == num)
                            {
                                drawCanvas.Children.Remove(drawCanvas.Children[i]);
                                i--;
                            }
                        }
                        else if (drawCanvas.Children[i] is Line)
                        {
                            if (((drawCanvas.Children[i] as Line).Tag as RouteData).objType == "Polygon" && ((drawCanvas.Children[i] as Line).Tag as RouteData).objID == num)
                            {
                                drawCanvas.Children.Remove(drawCanvas.Children[i]);
                                i--;
                            }
                        }
                    }
                    polygonProperties.Visibility = Visibility.Collapsed;
                    line = null;
                    ellipse = null;
                }
                else if ((ellipse.Tag as RouteData).objType == "Route")
                {
                    num = (ellipse.Tag as RouteData).objID;
                    for (int i = 0; i < drawCanvas.Children.Count; i++)
                    {
                        if (drawCanvas.Children[i] is Ellipse)
                        {
                            if (((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objType == "Route" && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objID == num)
                            {
                                drawCanvas.Children.Remove(drawCanvas.Children[i]);
                                i--;
                            }
                        }
                        else if (drawCanvas.Children[i] is Line)
                        {
                            if (((drawCanvas.Children[i] as Line).Tag as RouteData).objType == "Route" && ((drawCanvas.Children[i] as Line).Tag as RouteData).objID == num)
                            {
                                drawCanvas.Children.Remove(drawCanvas.Children[i]);
                                i--;
                            }
                        }
                        else if (drawCanvas.Children[i] is StackPanel)
                        {
                            if (((drawCanvas.Children[i] as StackPanel).Tag as RouteData).objType == "Route" && ((drawCanvas.Children[i] as StackPanel).Tag as RouteData).objID == num)
                            {
                                drawCanvas.Children.Remove(drawCanvas.Children[i]);
                                i--;
                            }
                        }
                    }
                    routeProperties.Visibility = Visibility.Collapsed;
                    line = null;
                    ellipse = null;
                }
            }
        }

        private void setColor(object sender, SelectionChangedEventArgs e)
        {
            if (line != null || ellipse != null)
            {
                ComboBox cmbX = sender as ComboBox;
                switch (cmbX.Name)
                {
                    case "lineColors":
                        line.Stroke = (Brush)(cmbX.SelectedItem as PropertyInfo).GetValue(null);
                        break;
                    case "circleColors":
                        ellipse.Stroke = (Brush)(cmbX.SelectedItem as PropertyInfo).GetValue(null);
                        break;
                    case "routeColors":
                        line.Stroke = (Brush)(cmbX.SelectedItem as PropertyInfo).GetValue(null);
                        break;
                    case "polygonColors":
                        line.Stroke = (Brush)(cmbX.SelectedItem as PropertyInfo).GetValue(null);
                        break;
                    default:
                        return;
                }
            }
        }

        private void setType(object sender, SelectionChangedEventArgs e)
        {
            if (line != null || ellipse != null)
            {
                ComboBox cmbX = sender as ComboBox;
                switch (cmbX.Name)
                {
                    case "lineType":
                    case "routeType":
                    case "polygonType":
                        switch (cmbX.SelectedIndex)
                        {
                            case 0:
                                line.StrokeDashCap = PenLineCap.Flat;
                                line.StrokeDashArray = new DoubleCollection() { 5, 0 };
                                break;
                            case 1:
                                line.StrokeDashCap = PenLineCap.Round;
                                line.StrokeDashArray = new DoubleCollection() { 1, 2 };
                                break;
                            case 2:
                                line.StrokeDashCap = PenLineCap.Round;
                                line.StrokeDashArray = new DoubleCollection() { 3, 2 };
                                break;
                            case 3:
                                line.StrokeDashCap = PenLineCap.Round;
                                line.StrokeDashArray = new DoubleCollection() { 3, 2, 1, 2 };
                                break;
                            default:
                                return;
                        }
                        break;
                    case "circleType":
                        switch (cmbX.SelectedIndex)
                        {
                            case 0:
                                ellipse.StrokeDashCap = PenLineCap.Flat;
                                ellipse.StrokeDashArray = new DoubleCollection() { 5, 0 };
                                break;
                            case 1:
                                ellipse.StrokeDashCap = PenLineCap.Round;
                                ellipse.StrokeDashArray = new DoubleCollection() { 1, 2 };
                                break;
                            case 2:
                                ellipse.StrokeDashCap = PenLineCap.Round;
                                ellipse.StrokeDashArray = new DoubleCollection() { 3, 2 };
                                break;
                            case 3:
                                ellipse.StrokeDashCap = PenLineCap.Round;
                                ellipse.StrokeDashArray = new DoubleCollection() { 3, 2, 1, 2 };
                                break;
                            default:
                                return;
                        }
                        break;
                    default:
                        return;
                }
            }

        }

        private void setValue(object sender, KeyEventArgs e)
        {
            if (line != null || ellipse != null)
            {
                TextBox txtBx = sender as TextBox;
                Double value;
                if (e.Key == Key.Enter)
                {
                    switch (txtBx.Name)
                    {
                        case "lineThickness":
                        case "routeThickness":
                        case "polygonThickness":
                            if (String.IsNullOrEmpty(txtBx.Text) || !Double.TryParse(txtBx.Text, out value)) txtBx.Text = line.StrokeThickness.ToString();
                            else if (value < minThickness || value > maxThickness) txtBx.Text = line.StrokeThickness.ToString();
                            else line.StrokeThickness = value;
                            break;
                        case "circleThickness":
                            if (String.IsNullOrEmpty(txtBx.Text) || !Double.TryParse(txtBx.Text, out value)) txtBx.Text = ellipse.StrokeThickness.ToString();
                            else if (value < minThickness || value > maxThickness) txtBx.Text = ellipse.StrokeThickness.ToString();
                            else ellipse.StrokeThickness = value;
                            break;
                        case "circleRadius":
                            if (String.IsNullOrEmpty(txtBx.Text) || !Double.TryParse(txtBx.Text, out value)) txtBx.Text = ((ellipse.ActualWidth) / 2).ToString();
                            else if (value < minRadius || value > maxRadius) txtBx.Text = ((ellipse.ActualWidth) / 2).ToString();
                            else
                            {
                                ellipse.Width = value * 2;
                                ellipse.Height = ellipse.Width;
                                (ellipse.Tag as RouteData).pos2 = Gmap.FromLocalToLatLng((int)(Canvas.GetLeft(ellipse) + ellipse.Width / 2), (int)Canvas.GetTop(ellipse));
                            }
                            break;
                        default:
                            return;
                    }
                }
                if ((e.Key == Key.OemPeriod || e.Key == Key.Decimal) && txtBx.Text.IndexOf(".") != -1) e.Handled = true;
                else if ((e.Key < Key.D0 || e.Key > Key.D9) && (e.Key < Key.NumPad0 || e.Key > Key.NumPad9) && (e.Key != Key.OemPeriod && e.Key != Key.Decimal)) e.Handled = true;
            }
        }

        private void updateValue(string item)
        {
            switch (item)
            {
                case "line":
                    for (int n = 0; n < lineColors.Items.Count; n++)
                    {
                        if ((Brush)(lineColors.Items[n] as PropertyInfo).GetValue(null) == line.Stroke)
                        {
                            lineColors.SelectedIndex = n;
                            break;
                        }
                    }
                    for (int n = 0; n < lineType.Items.Count; n++)
                    {
                        if (line.StrokeDashArray.SequenceEqual<Double>(new DoubleCollection() { 5, 0 }))
                        {
                            lineType.SelectedIndex = 0;
                            break;
                        }
                        if (line.StrokeDashArray.SequenceEqual<Double>(new DoubleCollection() { 1, 2 }))
                        {
                            lineType.SelectedIndex = 1;
                            break;
                        }
                        if (line.StrokeDashArray.SequenceEqual<Double>(new DoubleCollection() { 3, 2 }))
                        {
                            lineType.SelectedIndex = 2;
                            break;
                        }
                        if (line.StrokeDashArray.SequenceEqual<Double>(new DoubleCollection() { 3, 2, 1, 2 }))
                        {
                            lineType.SelectedIndex = 3;
                            break;
                        }
                    }
                    lineThickness.Text = line.StrokeThickness.ToString();
                    break;
                case "route":
                    for (int n = 0; n < routeColors.Items.Count; n++)
                    {
                        if ((Brush)(routeColors.Items[n] as PropertyInfo).GetValue(null) == line.Stroke)
                        {
                            routeColors.SelectedIndex = n;
                            break;
                        }
                    }
                    for (int n = 0; n < routeType.Items.Count; n++)
                    {
                        if (line.StrokeDashArray.SequenceEqual<Double>(new DoubleCollection() { 5, 0 }))
                        {
                            routeType.SelectedIndex = 0;
                            break;
                        }
                        else if (line.StrokeDashArray.SequenceEqual<Double>(new DoubleCollection() { 1, 2 }))
                        {
                            routeType.SelectedIndex = 1;
                            break;
                        }
                        else if (line.StrokeDashArray.SequenceEqual<Double>(new DoubleCollection() { 3, 2 }))
                        {
                            routeType.SelectedIndex = 2;
                            break;
                        }
                        else if (line.StrokeDashArray.SequenceEqual<Double>(new DoubleCollection() { 3, 2, 1, 2 }))
                        {
                            routeType.SelectedIndex = 3;
                            break;
                        }
                    }
                    routeThickness.Text = line.StrokeThickness.ToString();
                    RouteData dat = ellipse.Tag as RouteData;
                    routeAcNameBox.Text = pdata.performanceDatas[0].aircraft;
                    msnNameBox.Text = dat.mission;
                    routeTotalDistanceBox.Text = DataConverters.LengthUnits(dat.totaldistance, baseunit.bdistUnit, dat.totaldistanceUnit).ToString();
                    routeTotalTimeBox.Value = TimeSpan.FromSeconds(dat.totaltime);
                    routeTotalFuelBox.Text = dat.totalfuel.ToString();
                    routeStartingFuelBox.Text = dat.startingfuel.ToString();
                    routeMinimaFuelBox.Text = dat.minima.ToString();
                    routeCoordBox.Text = String.Format($"{Math.Round(dat.pos1.Lat, 6)},{Math.Round(dat.pos1.Lng, 6)}-{Math.Round(dat.pos2.Lat, 6)},{Math.Round(dat.pos2.Lng, 6)}");
                    routeNameBox.Text = dat.name;
                    routeTypeBox.Text = dat.type;
                    if (routeTypeBox.SelectedIndex == -1) routeTypeBox.IsEnabled = false;
                    else routeTypeBox.IsEnabled = true;
                    routeTrackBox.Text = dat.track.ToString();
                    routeDistanceBox.Text = DataConverters.LengthUnits(dat.distance, baseunit.bdistUnit, dat.distanceUnit).ToString();
                    routeAltBox.SelectedValue = dat.alt;
                    routeSpeedBox.SelectedValue = dat.speed;
                    routeTimeBox.Value = TimeSpan.FromSeconds(dat.time);
                    routeFuelBox.Text = dat.fuel.ToString();
                    routeRefuelDefuelBox.Text = dat.rdfuel.ToString();
                    routeTotalDistanceUnit.SelectedValue = dat.totaldistanceUnit;
                    routeTotalFuelUnit.Text = baseunit.bfuelUnit;
                    routeStartingFuelUnit.Text = baseunit.bfuelUnit;
                    routeMinimaFuelUnit.Text = baseunit.bfuelUnit;
                    routeDistanceUnit.SelectedValue = dat.distanceUnit;
                    routeAltUnit.Text = baseunit.baltUnit;
                    routeSpeedUnit.Text = baseunit.bspeedUnit;
                    routeFuelUnit.Text = baseunit.bfuelUnit;
                    routeRefuelDefuelUnit.Text = baseunit.bfuelUnit;
                    break;
                case "polygon":
                    for (int n = 0; n < polygonColors.Items.Count; n++)
                    {
                        if ((Brush)(polygonColors.Items[n] as PropertyInfo).GetValue(null) == line.Stroke)
                        {
                            polygonColors.SelectedIndex = n;
                            break;
                        }
                    }
                    for (int n = 0; n < polygonType.Items.Count; n++)
                    {
                        if (line.StrokeDashArray.SequenceEqual<Double>(new DoubleCollection() { 5, 0 }))
                        {
                            polygonType.SelectedIndex = 0;
                            break;
                        }
                        if (line.StrokeDashArray.SequenceEqual<Double>(new DoubleCollection() { 1, 2 }))
                        {
                            polygonType.SelectedIndex = 1;
                            break;
                        }
                        if (line.StrokeDashArray.SequenceEqual<Double>(new DoubleCollection() { 3, 2 }))
                        {
                            polygonType.SelectedIndex = 2;
                            break;
                        }
                        if (line.StrokeDashArray.SequenceEqual<Double>(new DoubleCollection() { 3, 2, 1, 2 }))
                        {
                            polygonType.SelectedIndex = 3;
                            break;
                        }
                    }
                    polygonThickness.Text = line.StrokeThickness.ToString();
                    break;
                case "circle":
                    for (int n = 0; n < circleColors.Items.Count; n++)
                    {
                        if ((Brush)(circleColors.Items[n] as PropertyInfo).GetValue(null) == ellipse.Stroke)
                        {
                            circleColors.SelectedIndex = n;
                            break;
                        }
                    }
                    for (int n = 0; n < circleType.Items.Count; n++)
                    {
                        if (ellipse.StrokeDashArray.SequenceEqual<Double>(new DoubleCollection() { 5, 0 }))
                        {
                            circleType.SelectedIndex = 0;
                            break;
                        }
                        else if (ellipse.StrokeDashArray.SequenceEqual<Double>(new DoubleCollection() { 1, 2 }))
                        {
                            circleType.SelectedIndex = 1;
                            break;
                        }
                        else if (ellipse.StrokeDashArray.SequenceEqual<Double>(new DoubleCollection() { 3, 2 }))
                        {
                            circleType.SelectedIndex = 2;
                            break;
                        }
                        else if (ellipse.StrokeDashArray.SequenceEqual<Double>(new DoubleCollection() { 3, 2, 1, 2 }))
                        {
                            circleType.SelectedIndex = 3;
                            break;
                        }
                    }
                    circleThickness.Text = ellipse.StrokeThickness.ToString();
                    circleRadius.Text = (ellipse.Width / 2).ToString();
                    break;
                default:
                    return;
            }
        }

        private void removeElement(object sender, RoutedEventArgs e)
        {
            if (ellipse == null) return;
            int num;
            switch ((sender as Button).Name)
            {
                case "removeLine":
                    num = (ellipse.Tag as RouteData).objID;
                    for (int i = 0; i < drawCanvas.Children.Count; i++)
                    {
                        if (drawCanvas.Children[i] is Ellipse)
                        {
                            if (((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objType == "Line" && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objID == num)
                            {
                                drawCanvas.Children.Remove(drawCanvas.Children[i]);
                                i--;
                            }
                        }
                        else if (drawCanvas.Children[i] is Line)
                        {
                            if (((drawCanvas.Children[i] as Line).Tag as RouteData).objType == "Line" && ((drawCanvas.Children[i] as Line).Tag as RouteData).objID == num)
                            {
                                drawCanvas.Children.Remove(drawCanvas.Children[i]);
                                i--;
                            }
                        }
                        lineProperties.Visibility = Visibility.Collapsed;
                        line = null;
                        ellipse = null;
                    }
                    break;
                case "removeCircle":
                    drawCanvas.Children.Remove(ellipse);
                    circleProperties.Visibility = Visibility.Collapsed;
                    ellipse = null;
                    break;
                case "removePolygon":
                    num = (ellipse.Tag as RouteData).objID;
                    for (int i = 0; i < drawCanvas.Children.Count; i++)
                    {
                        if (drawCanvas.Children[i] is Ellipse)
                        {
                            if (((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objType == "Polygon" && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objID == num)
                            {
                                drawCanvas.Children.Remove(drawCanvas.Children[i]);
                                i--;
                            }
                        }
                        else if (drawCanvas.Children[i] is Line)
                        {
                            if (((drawCanvas.Children[i] as Line).Tag as RouteData).objType == "Polygon" && ((drawCanvas.Children[i] as Line).Tag as RouteData).objID == num)
                            {
                                drawCanvas.Children.Remove(drawCanvas.Children[i]);
                                i--;
                            }
                        }
                    }
                    polygonProperties.Visibility = Visibility.Collapsed;
                    line = null;
                    ellipse = null;
                    break;
                case "removeRoute":
                    num = (ellipse.Tag as RouteData).objID;
                    for (int i = 0; i < drawCanvas.Children.Count; i++)
                    {
                        if (drawCanvas.Children[i] is Ellipse)
                        {
                            if (((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objType == "Route" && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objID == num)
                            {
                                drawCanvas.Children.Remove(drawCanvas.Children[i]);
                                i--;
                            }
                        }
                        else if (drawCanvas.Children[i] is Line)
                        {
                            if (((drawCanvas.Children[i] as Line).Tag as RouteData).objType == "Route" && ((drawCanvas.Children[i] as Line).Tag as RouteData).objID == num)
                            {
                                drawCanvas.Children.Remove(drawCanvas.Children[i]);
                                i--;
                            }
                        }
                        else if (drawCanvas.Children[i] is StackPanel)
                        {
                            if (((drawCanvas.Children[i] as StackPanel).Tag as RouteData).objType == "Route" && ((drawCanvas.Children[i] as StackPanel).Tag as RouteData).objID == num)
                            {
                                drawCanvas.Children.Remove(drawCanvas.Children[i]);
                                i--;
                            }
                        }
                    }
                    routeProperties.Visibility = Visibility.Collapsed;
                    line = null;
                    ellipse = null;
                    break;
                default:
                    return;
            }
        }

        private void repositionPoints()
        {
            GPoint baf_anch = Gmap.FromLatLngToLocal(new PointLatLng(23.7250117359518, 90.50537109375));
            Canvas.SetLeft(bafChart, baf_anch.X);
            Canvas.SetTop(bafChart, baf_anch.Y);
            foreach (UIElement tmp in drawCanvas.Children)
            {
                if (tmp is Ellipse)
                {
                    Ellipse el = tmp as Ellipse;
                    RouteData datlink = el.Tag as RouteData;
                    if (datlink.objType == "Route" || datlink.objType == "Polygon" || datlink.objType == "Line")
                    {
                        GPoint gp = Gmap.FromLatLngToLocal(datlink.pos2);
                        Canvas.SetLeft(el, gp.X);
                        Canvas.SetTop(el, gp.Y);
                    }
                    else if (datlink.objType == "Circle")
                    {
                        GPoint gp = Gmap.FromLatLngToLocal(datlink.pos1);
                        Canvas.SetLeft(el, gp.X);
                        Canvas.SetTop(el, gp.Y);
                        GPoint sp = Gmap.FromLatLngToLocal(datlink.pos2);
                        el.Width = (sp.X - gp.X) * 2;
                        el.Height = el.Width;
                    }
                }
            }
        }

        public void loadAircraft(string name)
        {
            adflag = true;
            for (int i = 0; i < pdatas.Count; i++)
            {
                if (pdatas[i].performanceDatas[0].aircraft == name)
                {
                    pdata = pdatas[i];
                    fsdata = fsdatas[i];
                    frdata = frdatas[i];
                    baseunit = baseunits[i];
                    pdata.alt = i;
                    adflag = false;
                    break;
                }
            }
            if (adflag)
            {
                pdata = new PerformanceData();
                fsdata = new FuelStartData();
                frdata = new FuelReduceData();
                baseunit = new BaseUnit();
                using (IDbConnection cnn = new SQLiteConnection(@"Data Source=.\test.db;Version=3"))
                {
                    var poutput = cnn.Query<PerformanceData>($"SELECT * FROM 'Performance Data' WHERE Aircraft='{name}' ORDER BY ALT", new DynamicParameters());
                    var fsoutput = cnn.Query<FuelStartData>($"SELECT Label, Value FROM 'Fuel Data' WHERE Aircraft='{name}' AND Type='Starting' ORDER BY Label", new DynamicParameters());
                    var froutput = cnn.Query<FuelReduceData>($"SELECT Label, Value FROM 'Fuel Data' WHERE Aircraft='{name}' AND Type='Reduction' ORDER BY Label", new DynamicParameters());
                    var datas = cnn.Query($"SELECT Value, Unit FROM 'Speed and Unit Data' WHERE Aircraft='{name}' ORDER BY SpeedID", new DynamicParameters()).ToList();
                    pdata.performanceDatas.Clear();
                    fsdata.fuelStartDatas.Clear();
                    frdata.fuelReduceDatas.Clear();
                    foreach (PerformanceData info in poutput)
                    {
                        pdata.performanceDatas.Add(info);
                    }
                    foreach (FuelStartData info in fsoutput)
                    {
                        fsdata.fuelStartDatas.Add(info);
                    }
                    foreach (FuelReduceData info in froutput)
                    {
                        frdata.fuelReduceDatas.Add(info);
                    }
                    baseunit.baltUnit = datas[0].Unit.ToString();
                    baseunit.bdistUnit = datas[1].Unit.ToString();
                    baseunit.bspeedUnit = datas[2].Unit.ToString().Split('-')[1];
                    baseunit.bfuelUnit = datas[3].Unit.ToString();
                    baseunit.blfftUnit = datas[4].Unit.ToString();
                    pdata.spd1 = Convert.ToDouble(datas[0].Value);
                    pdata.spd2 = Convert.ToDouble(datas[1].Value);
                    pdata.spd3 = Convert.ToDouble(datas[2].Value);
                    pdata.spd4 = Convert.ToDouble(datas[3].Value);
                    pdata.spd5 = Convert.ToDouble(datas[4].Value);
                    pdatas.Add(pdata);
                    fsdatas.Add(fsdata);
                    frdatas.Add(frdata);
                    baseunits.Add(baseunit);
                    pdata.alt = pdatas.Count - 1;
                }
            }
            List<double> speeds = new List<double>();
            if (pdata.spd1 != 0) speeds.Add(pdata.spd1);
            if (pdata.spd2 != 0) speeds.Add(pdata.spd2);
            if (pdata.spd3 != 0) speeds.Add(pdata.spd3);
            if (pdata.spd4 != 0) speeds.Add(pdata.spd4);
            if (pdata.spd5 != 0) speeds.Add(pdata.spd5);
            routeAcNameBox.Text = name;
            routeAltBox.ItemsSource = pdata.performanceDatas.ToList();
            routeStartingFuelBox.ItemsSource = fsdata.fuelStartDatas.ToList();
            routeSpeedBox.ItemsSource = speeds;
            routeAltBox.SelectedIndex = 0;
            routeStartingFuelBox.SelectedIndex = 0;
            routeSpeedBox.SelectedIndex = 0;
            routeTypeBox.SelectedValue = "Enroute";
            routeAltUnit.Text = baseunit.baltUnit;
            routeTotalDistanceUnit.Text = baseunit.bdistUnit;
            routeDistanceUnit.Text = baseunit.bdistUnit;
            routeSpeedUnit.Text = baseunit.bspeedUnit;
            routeFuelUnit.Text = baseunit.bfuelUnit;
            routeTotalFuelUnit.Text = baseunit.bfuelUnit;
            routeMinimaFuelUnit.Text = baseunit.bfuelUnit;
            routeStartingFuelUnit.Text = baseunit.bfuelUnit;
            routeRefuelDefuelUnit.Text = baseunit.bfuelUnit;
            lineProperties.Visibility = Visibility.Collapsed;
            polygonProperties.Visibility = Visibility.Collapsed;
            circleProperties.Visibility = Visibility.Collapsed;
            routeProperties.Visibility = Visibility.Visible;
            routeLineBlock.Visibility = Visibility.Collapsed;
            routewaypointDataBlock.Visibility = Visibility.Collapsed;
            routeProperties.IsEnabled = false;
            addwaypntBtn.IsEnabled = true;
        }

        private void updateRouteData(object sender, EventArgs e)
        {
            if (ellipse != null)
            {
                var id = (ellipse.Tag as RouteData).objID;
                var type = (ellipse.Tag as RouteData).objType;
                if (e.GetType() == typeof(RoutedEventArgs))
                {
                    RoutedEventArgs ee = e as RoutedEventArgs;
                    if (sender is TextBox)
                    {
                        switch ((sender as TextBox).Name)
                        {
                            case "msnNameBox":
                                for (int i = 0; i < drawCanvas.Children.Count; i++)
                                {
                                    if (drawCanvas.Children[i] is Line)
                                    {
                                        if (((drawCanvas.Children[i] as Line).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as Line).Tag as RouteData).objID == id)
                                        {
                                            ((drawCanvas.Children[i] as Line).Tag as RouteData).mission = msnNameBox.Text;
                                        }
                                    }
                                    else if (drawCanvas.Children[i] is Ellipse)
                                    {
                                        if (((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objID == id)
                                        {
                                            ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).mission = msnNameBox.Text;
                                        }
                                    }
                                    else if (drawCanvas.Children[i] is StackPanel)
                                    {
                                        if (((drawCanvas.Children[i] as StackPanel).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as StackPanel).Tag as RouteData).objID == id)
                                        {
                                            ((drawCanvas.Children[i] as StackPanel).Tag as RouteData).mission = msnNameBox.Text;
                                        }
                                    }
                                }
                                break;
                            case "routeMinimaFuelBox":
                                if (String.IsNullOrEmpty(routeMinimaFuelBox.Text))
                                {
                                    routeMinimaFuelBox.Text = (ellipse.Tag as RouteData).minima.ToString();
                                    break;
                                }
                                for (int i = 0; i < drawCanvas.Children.Count; i++)
                                {
                                    if (drawCanvas.Children[i] is Line)
                                    {
                                        if (((drawCanvas.Children[i] as Line).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as Line).Tag as RouteData).objID == id)
                                        {
                                            ((drawCanvas.Children[i] as Line).Tag as RouteData).minima = Convert.ToDouble(routeMinimaFuelBox.Text);
                                        }
                                    }
                                    else if (drawCanvas.Children[i] is Ellipse)
                                    {
                                        if (((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objID == id)
                                        {
                                            ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).minima = Convert.ToDouble(routeMinimaFuelBox.Text);
                                        }
                                    }
                                    else if (drawCanvas.Children[i] is StackPanel)
                                    {
                                        if (((drawCanvas.Children[i] as StackPanel).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as StackPanel).Tag as RouteData).objID == id)
                                        {
                                            ((drawCanvas.Children[i] as StackPanel).Tag as RouteData).minima = Convert.ToDouble(routeMinimaFuelBox.Text);
                                        }
                                    }
                                }
                                break;
                            case "routeNameBox":
                                (ellipse.Tag as RouteData).name = routeNameBox.Text;
                                if ((ellipse.Tag as RouteData).componentID != 0) (line.Tag as RouteData).name = routeNameBox.Text;
                                break;
                            case "routeRefuelDefuelBox":
                                if (String.IsNullOrEmpty(routeRefuelDefuelBox.Text))
                                {
                                    routeRefuelDefuelBox.Text = (ellipse.Tag as RouteData).rdfuel.ToString();
                                    break;
                                }
                                double val = 0;
                                if (routeRefuelDefuelSelector.SelectedValue.ToString() == "Refuel") val = Convert.ToDouble(routeRefuelDefuelBox.Text);
                                else if (routeRefuelDefuelSelector.SelectedValue.ToString() == "Defuel") val = Convert.ToDouble($"-{routeRefuelDefuelBox.Text}");
                                (ellipse.Tag as RouteData).rdfuel = val;
                                if ((ellipse.Tag as RouteData).componentID != 0) (line.Tag as RouteData).rdfuel = val;
                                break;
                            default:
                                return;
                        }
                    }
                    else if (sender is ComboBox)
                    {
                        switch ((sender as ComboBox).Name)
                        {
                            case "routeStartingFuelBox":
                                if (String.IsNullOrEmpty(routeStartingFuelBox.Text))
                                {
                                    routeStartingFuelBox.Text = (ellipse.Tag as RouteData).startingfuel.ToString();
                                    break;
                                }
                                for (int i = 0; i < drawCanvas.Children.Count; i++)
                                {
                                    if (drawCanvas.Children[i] is Line)
                                    {
                                        if (((drawCanvas.Children[i] as Line).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as Line).Tag as RouteData).objID == id)
                                        {
                                            ((drawCanvas.Children[i] as Line).Tag as RouteData).startingfuel = Convert.ToDouble(routeStartingFuelBox.Text);
                                        }
                                    }
                                    else if (drawCanvas.Children[i] is Ellipse)
                                    {
                                        if (((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objID == id)
                                        {
                                            ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).startingfuel = Convert.ToDouble(routeStartingFuelBox.Text);
                                        }
                                    }
                                    else if (drawCanvas.Children[i] is StackPanel)
                                    {
                                        if (((drawCanvas.Children[i] as StackPanel).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as StackPanel).Tag as RouteData).objID == id)
                                        {
                                            ((drawCanvas.Children[i] as StackPanel).Tag as RouteData).startingfuel = Convert.ToDouble(routeStartingFuelBox.Text);
                                        }
                                    }
                                }
                                break;
                            default:
                                return;
                        }
                    }
                    return;
                }
                else if (e.GetType() == typeof(KeyEventArgs))
                {
                    KeyEventArgs ee = e as KeyEventArgs;
                    if (sender is TextBox)
                    {
                        switch ((sender as TextBox).Name)
                        {
                            case "msnNameBox":
                                if (ee.Key == Key.Enter)
                                {
                                    for (int i = 0; i < drawCanvas.Children.Count; i++)
                                    {
                                        if (drawCanvas.Children[i] is Line)
                                        {
                                            if (((drawCanvas.Children[i] as Line).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as Line).Tag as RouteData).objID == id)
                                            {
                                                ((drawCanvas.Children[i] as Line).Tag as RouteData).mission = msnNameBox.Text;
                                            }
                                        }
                                        else if (drawCanvas.Children[i] is Ellipse)
                                        {
                                            if (((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objID == id)
                                            {
                                                ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).mission = msnNameBox.Text;
                                            }
                                        }
                                        else if (drawCanvas.Children[i] is StackPanel)
                                        {
                                            if (((drawCanvas.Children[i] as StackPanel).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as StackPanel).Tag as RouteData).objID == id)
                                            {
                                                ((drawCanvas.Children[i] as StackPanel).Tag as RouteData).mission = msnNameBox.Text;
                                            }
                                        }
                                    }
                                    Keyboard.ClearFocus();
                                }
                                break;
                            case "routeMinimaFuelBox":
                                if (ee.Key == Key.Enter)
                                {
                                    if (String.IsNullOrEmpty(routeMinimaFuelBox.Text))
                                    {
                                        routeMinimaFuelBox.Text = (ellipse.Tag as RouteData).minima.ToString();
                                        Keyboard.ClearFocus();
                                        break;
                                    }
                                    for (int i = 0; i < drawCanvas.Children.Count; i++)
                                    {
                                        if (drawCanvas.Children[i] is Line)
                                        {
                                            if (((drawCanvas.Children[i] as Line).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as Line).Tag as RouteData).objID == id)
                                            {
                                                ((drawCanvas.Children[i] as Line).Tag as RouteData).minima = Convert.ToDouble(routeMinimaFuelBox.Text);
                                            }
                                        }
                                        else if (drawCanvas.Children[i] is Ellipse)
                                        {
                                            if (((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objID == id)
                                            {
                                                ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).minima = Convert.ToDouble(routeMinimaFuelBox.Text);
                                            }
                                        }
                                        else if (drawCanvas.Children[i] is StackPanel)
                                        {
                                            if (((drawCanvas.Children[i] as StackPanel).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as StackPanel).Tag as RouteData).objID == id)
                                            {
                                                ((drawCanvas.Children[i] as StackPanel).Tag as RouteData).minima = Convert.ToDouble(routeMinimaFuelBox.Text);
                                            }
                                        }
                                    }
                                    Keyboard.ClearFocus();
                                }
                                else numericFilter(sender, ee);
                                break;
                            case "routeNameBox":
                                if (ee.Key == Key.Enter)
                                {
                                    (ellipse.Tag as RouteData).name = routeNameBox.Text;
                                    if ((ellipse.Tag as RouteData).componentID != 0) (line.Tag as RouteData).name = routeNameBox.Text;
                                    Keyboard.ClearFocus();
                                }
                                break;
                            case "routeRefuelDefuelBox":
                                if (ee.Key == Key.Enter)
                                {
                                    if (String.IsNullOrEmpty(routeRefuelDefuelBox.Text))
                                    {
                                        routeRefuelDefuelBox.Text = (ellipse.Tag as RouteData).rdfuel.ToString();
                                        Keyboard.ClearFocus();
                                        break;
                                    }
                                    double val = 0;
                                    if (routeRefuelDefuelSelector.SelectedValue.ToString() == "Refuel") val = Convert.ToDouble(routeRefuelDefuelBox.Text);
                                    else if (routeRefuelDefuelSelector.SelectedValue.ToString() == "Defuel") val = Convert.ToDouble($"-{routeRefuelDefuelBox.Text}");
                                    (ellipse.Tag as RouteData).rdfuel = val;
                                    if ((ellipse.Tag as RouteData).componentID != 0) (line.Tag as RouteData).rdfuel = val;
                                    Keyboard.ClearFocus();
                                }
                                else numericFilter(sender, ee);
                                break;
                            default:
                                return;
                        }
                    }
                    else if (sender is ComboBox)
                    {
                        switch ((sender as ComboBox).Name)
                        {
                            case "routeStartingFuelBox":
                                if (ee.Key == Key.Enter)
                                {
                                    if (String.IsNullOrEmpty(routeStartingFuelBox.Text))
                                    {
                                        routeStartingFuelBox.Text = (ellipse.Tag as RouteData).startingfuel.ToString();
                                        Keyboard.ClearFocus();
                                        break;
                                    }
                                    for (int i = 0; i < drawCanvas.Children.Count; i++)
                                    {
                                        if (drawCanvas.Children[i] is Line)
                                        {
                                            if (((drawCanvas.Children[i] as Line).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as Line).Tag as RouteData).objID == id)
                                            {
                                                ((drawCanvas.Children[i] as Line).Tag as RouteData).startingfuel = Convert.ToDouble(routeStartingFuelBox.Text);
                                            }
                                        }
                                        else if (drawCanvas.Children[i] is Ellipse)
                                        {
                                            if (((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objID == id)
                                            {
                                                ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).startingfuel = Convert.ToDouble(routeStartingFuelBox.Text);
                                            }
                                        }
                                        else if (drawCanvas.Children[i] is StackPanel)
                                        {
                                            if (((drawCanvas.Children[i] as StackPanel).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as StackPanel).Tag as RouteData).objID == id)
                                            {
                                                ((drawCanvas.Children[i] as StackPanel).Tag as RouteData).startingfuel = Convert.ToDouble(routeStartingFuelBox.Text);
                                            }
                                        }
                                    }
                                    Keyboard.ClearFocus();
                                }
                                else numericFilter(sender, ee);
                                break;
                            default:
                                return;
                        }
                    }
                    return;
                }
                else if (e.GetType() == typeof(SelectionChangedEventArgs))
                {
                    SelectionChangedEventArgs ee = e as SelectionChangedEventArgs;
                    var dat = ellipse.Tag as RouteData;
                    switch ((sender as ComboBox).Name)
                    {
                        case "routeTotalDistanceUnit":
                            if (routeTotalDistanceUnit.SelectedIndex == -1) return;
                            routeTotalDistanceBox.Text = DataConverters.LengthUnits(dat.totaldistance, baseunit.bdistUnit, routeTotalDistanceUnit.SelectedValue.ToString()).ToString();
                            for (int i = 0; i < drawCanvas.Children.Count; i++)
                            {
                                if (drawCanvas.Children[i] is Line)
                                {
                                    if (((drawCanvas.Children[i] as Line).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as Line).Tag as RouteData).objID == id)
                                    {
                                        ((drawCanvas.Children[i] as Line).Tag as RouteData).totaldistanceUnit = routeTotalDistanceUnit.SelectedValue.ToString();
                                    }
                                }
                                else if (drawCanvas.Children[i] is Ellipse)
                                {
                                    if (((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objID == id)
                                    {
                                        ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).totaldistanceUnit = routeTotalDistanceUnit.SelectedValue.ToString();
                                    }
                                }
                                else if (drawCanvas.Children[i] is StackPanel)
                                {
                                    if (((drawCanvas.Children[i] as StackPanel).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as StackPanel).Tag as RouteData).objID == id)
                                    {
                                        ((drawCanvas.Children[i] as StackPanel).Tag as RouteData).totaldistanceUnit = routeTotalDistanceUnit.SelectedValue.ToString();
                                    }
                                }
                            }
                            break;
                        case "routeDistanceUnit":
                            if (routeDistanceUnit.SelectedIndex == -1) return;
                            routeDistanceBox.Text = DataConverters.LengthUnits(dat.distance, baseunit.bdistUnit, routeDistanceUnit.SelectedValue.ToString()).ToString();
                            for (int i = 0; i < drawCanvas.Children.Count; i++)
                            {
                                if (drawCanvas.Children[i] is Line)
                                {
                                    if (((drawCanvas.Children[i] as Line).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as Line).Tag as RouteData).objID == id)
                                    {
                                        ((drawCanvas.Children[i] as Line).Tag as RouteData).distanceUnit = routeDistanceUnit.SelectedValue.ToString();
                                    }
                                }
                                else if (drawCanvas.Children[i] is Ellipse)
                                {
                                    if (((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objID == id)
                                    {
                                        ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).distanceUnit = routeDistanceUnit.SelectedValue.ToString();
                                    }
                                }
                                else if (drawCanvas.Children[i] is StackPanel)
                                {
                                    if (((drawCanvas.Children[i] as StackPanel).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as StackPanel).Tag as RouteData).objID == id)
                                    {
                                        ((drawCanvas.Children[i] as StackPanel).Tag as RouteData).distanceUnit = routeDistanceUnit.SelectedValue.ToString();
                                    }
                                }
                            }
                            break;
                        case "routeTypeBox":
                            if (routeTypeBox.SelectedIndex == -1) return;
                            dat.type = routeTypeBox.SelectedValue.ToString();
                            if (dat.componentID != 0) (line.Tag as RouteData).type = routeTypeBox.SelectedValue.ToString();
                            updateValue("route");
                            updateHeadingBoxInfo(dat.objID);
                            break;
                        case "routeAltBox":
                        case "routeSpeedBox":
                            if ((sender as ComboBox).SelectedIndex == -1) return;
                            dat.setpData(pdata, routeAltBox.SelectedIndex, routeSpeedBox.SelectedIndex + 1);
                            if (dat.componentID != 0) (line.Tag as RouteData).setpData(pdata, routeAltBox.SelectedIndex, routeSpeedBox.SelectedIndex + 1);
                            updateValue("route");
                            updateHeadingBoxInfo(dat.objID);
                            break;
                        case "routeStartingFuelBox":
                            if (routeStartingFuelBox.SelectedIndex == -1) return;
                            for (int i = 0; i < drawCanvas.Children.Count; i++)
                            {
                                if (drawCanvas.Children[i] is Line)
                                {
                                    if (((drawCanvas.Children[i] as Line).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as Line).Tag as RouteData).objID == id)
                                    {
                                        ((drawCanvas.Children[i] as Line).Tag as RouteData).startingfuel = Convert.ToDouble(routeStartingFuelBox.SelectedValue.ToString());
                                    }
                                }
                                else if (drawCanvas.Children[i] is Ellipse)
                                {
                                    if (((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objID == id)
                                    {
                                        ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).startingfuel = Convert.ToDouble(routeStartingFuelBox.SelectedValue.ToString());
                                    }
                                }
                                else if (drawCanvas.Children[i] is StackPanel)
                                {
                                    if (((drawCanvas.Children[i] as StackPanel).Tag as RouteData).objType == type && ((drawCanvas.Children[i] as StackPanel).Tag as RouteData).objID == id)
                                    {
                                        ((drawCanvas.Children[i] as StackPanel).Tag as RouteData).startingfuel = Convert.ToDouble(routeStartingFuelBox.SelectedValue.ToString());
                                    }
                                }
                            }
                            break;
                        case "routeRefuelDefuelSelector":
                            routeRefuelDefuelBox.Focus();
                            break;
                        default:
                            return;
                    }
                }
            }
        }

        private void refreshRoutePanel()
        {
            for (int i = 0; i < pdatas.Count; i++)
            {
                if (pdatas[i].performanceDatas[0].aircraft == (ellipse.Tag as RouteData).aircraft)
                {
                    pdata = pdatas[i];
                    fsdata = fsdatas[i];
                    frdata = frdatas[i];
                    baseunit = baseunits[i];
                    pdata.alt = i;
                    break;
                }
            }
            List<double> speeds = new List<double>();
            if (pdata.spd1 != 0) speeds.Add(pdata.spd1);
            if (pdata.spd2 != 0) speeds.Add(pdata.spd2);
            if (pdata.spd3 != 0) speeds.Add(pdata.spd3);
            if (pdata.spd4 != 0) speeds.Add(pdata.spd4);
            if (pdata.spd5 != 0) speeds.Add(pdata.spd5);
            routeStartingFuelBox.ItemsSource = fsdata.fuelStartDatas;
            routeAltBox.ItemsSource = pdata.performanceDatas;
            routeSpeedBox.ItemsSource = speeds;
            routeStartingFuelBox.Items.Refresh();
            routeAltBox.Items.Refresh();
            routeSpeedBox.Items.Refresh();
        }

        private void updateHeadingBoxInfo(int oid)
        {
            for (int i = 0; i < drawCanvas.Children.Count; i++)
            {
                if (drawCanvas.Children[i] is StackPanel)
                {
                    if (((drawCanvas.Children[i] as StackPanel).Tag as RouteData).objType == "Route" && ((drawCanvas.Children[i] as StackPanel).Tag as RouteData).objID == oid)
                    {
                        ((drawCanvas.Children[i] as StackPanel).Children[1] as Label).GetBindingExpression(Label.ContentProperty).UpdateTarget();
                        ((drawCanvas.Children[i] as StackPanel).Children[2] as Label).GetBindingExpression(Label.ContentProperty).UpdateTarget();
                        ((drawCanvas.Children[i] as StackPanel).Children[3] as Label).GetBindingExpression(Label.ContentProperty).UpdateTarget();
                        ((drawCanvas.Children[i] as StackPanel).Children[4] as Label).GetBindingExpression(Label.ContentProperty).UpdateTarget();
                    }
                }
            }
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
        }

        public void clearCache()
        {
            line = null;
            ellipse = null;
            routeProperties.Visibility = Visibility.Collapsed;
            routeLineBlock.Visibility = Visibility.Collapsed;
            circleProperties.Visibility = Visibility.Collapsed;
            lineProperties.Visibility = Visibility.Collapsed;
            polygonProperties.Visibility = Visibility.Collapsed;
        }
    }
}
