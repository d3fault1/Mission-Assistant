using Dapper;
using GMap.NET;
using GMap.NET.MapProviders;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mission_Assistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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
        private List<StackPanel> checkpoints;
        private StackPanel _current;
        private PerformanceData pdata;
        private FuelStartData fsdata;
        private FuelReduceData frdata;
        private BaseUnit baseunit;
        public Point clickedPoint, currentPoint;
        public Line line = null, xline = null, yline = null;
        public Ellipse ellipse = null;
        public StackPanel routeDataPanel = null;
        public Image bafChart, image;
        public StackPanel current
        {
            get
            {
                return _current;
            }
            set
            {
                if (_current != null) _current.Opacity = 1;
                _current = value;
                if (_current != null) _current.Opacity = 0.5;
            }
        }
        public string mode = "none";
        public bool set = false, draggable = false, drawn = false, adflag;
        public int lineCount = -1, routeCount = -1, circleCount = -1, polygonCount = -1, markerCount = -1, boxCount = -1, imageCount = -1, count = 0;
        public double left, top;
        private const int minoffsetX = 5, maxoffsetX = 80, minoffsetY = -50, maxoffsetY = 50;
        private const double minThickness = 0.1, maxThickness = 30, minRadius = 0.1, maxRadius = 2000;
        public MainWindow()
        {
            InitializeComponent();
            pdatas = new List<PerformanceData>();
            fsdatas = new List<FuelStartData>();
            frdatas = new List<FuelReduceData>();
            baseunits = new List<BaseUnit>();
            checkpoints = new List<StackPanel>();
            lineColors.ItemsSource = typeof(Brushes).GetProperties();
            circleColors.ItemsSource = typeof(Brushes).GetProperties();
            routeColors.ItemsSource = typeof(Brushes).GetProperties();
            polygonColors.ItemsSource = typeof(Brushes).GetProperties();
            SqlMapper.SetTypeMap(typeof(PerformanceData), new CustomPropertyTypeMap(typeof(PerformanceData), (type, columnName) => type.GetProperties().FirstOrDefault(prop => prop.GetCustomAttributes(false).OfType<ColumnAttribute>().Any(attr => attr.Name == columnName))));
            SqlMapper.SetTypeMap(typeof(FuelStartData), new CustomPropertyTypeMap(typeof(FuelStartData), (type, columnName) => type.GetProperties().FirstOrDefault(prop => prop.GetCustomAttributes(false).OfType<ColumnAttribute>().Any(attr => attr.Name == columnName))));
            SqlMapper.SetTypeMap(typeof(FuelReduceData), new CustomPropertyTypeMap(typeof(FuelReduceData), (type, columnName) => type.GetProperties().FirstOrDefault(prop => prop.GetCustomAttributes(false).OfType<ColumnAttribute>().Any(attr => attr.Name == columnName))));
            GoogleHybridMapProvider.Instance.ApiKey = "AIzaSyBwyMHyzcR5EPdVwZdcf_BKizfnDQPZQHo";
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            //Gmap.CacheLocation = @"mapdata.cf";
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
                    clearCache();
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
                    else if (hit is Image)
                    {
                        if ((hit as Image).Tag is ImageData)
                        {
                            image = hit as Image;
                            if (!(image.Tag as ImageData).isDraggable)
                            {
                                image = null;
                                draggable = false;
                            }
                            else
                            {
                                left = Canvas.GetLeft(image);
                                top = Canvas.GetTop(image);
                                draggable = true;
                            }
                        }
                    }
                }
            }
        }

        private void drawOverlayMouseMove(object sender, MouseEventArgs e)
        {
            currentPoint = e.GetPosition(drawCanvas);
            PointLatLng currentposition = Gmap.FromLocalToLatLng((int)currentPoint.X, (int)currentPoint.Y);
            double[] dmsLat, dmsLng;
            dmsLat = DataConverters.CoordinateUnits(currentposition.Lat, "DEGREE", "DMS");
            dmsLng = DataConverters.CoordinateUnits(currentposition.Lng, "DEGREE", "DMS");
            pos_lat.Text = String.Format($"{dmsLat[0]}°{dmsLat[1]}'{dmsLat[2]}\"N");
            pos_long.Text = String.Format($"{dmsLng[0]}°{dmsLng[1]}'{dmsLng[2]}\"E");
            if (e.LeftButton == MouseButtonState.Pressed && draggable)
            {
                if (image != null)
                {
                    Canvas.SetLeft(image, left + currentPoint.X - clickedPoint.X);
                    Canvas.SetTop(image, top + currentPoint.Y - clickedPoint.Y);
                }
                else if (ellipse != null)
                {
                    Canvas.SetLeft(ellipse, left + currentPoint.X - clickedPoint.X);
                    Canvas.SetTop(ellipse, top + currentPoint.Y - clickedPoint.Y);
                }
            }
            else if (set)
            {
                PointLatLng fPoint = Gmap.FromLocalToLatLng((int)clickedPoint.X, (int)clickedPoint.Y);
                PointLatLng lPoint = Gmap.FromLocalToLatLng((int)currentPoint.X, (int)currentPoint.Y);
                line.X2 = currentPoint.X;
                line.Y2 = currentPoint.Y;
                if (!dst.IsVisible) dst.Visibility = Visibility.Visible;
                if (!trk.IsVisible) trk.Visibility = Visibility.Visible;
                dst.Text = new MapRoute(new List<PointLatLng>() { fPoint, lPoint }, "U").Distance.ToString();
                double tk = Math.Atan2(Math.Cos(lPoint.Lat * Math.PI / 180) * Math.Sin((lPoint.Lng - fPoint.Lng) * Math.PI / 180), Math.Cos(fPoint.Lat * Math.PI / 180) * Math.Sin(lPoint.Lat * Math.PI / 180) - Math.Sin(fPoint.Lat * Math.PI / 180) * Math.Cos(lPoint.Lat * Math.PI / 180) * Math.Cos((lPoint.Lng - fPoint.Lng) * Math.PI / 180)) * 180 / Math.PI;
                if (tk < 0) tk += 360;
                trk.Text = tk.ToString();
            }
            else if (!set)
            {
                if (dst.IsVisible) dst.Visibility = Visibility.Collapsed;
                if (trk.IsVisible) trk.Visibility = Visibility.Collapsed;
            }
            if (drawCanvas.Cursor == Cursors.Cross && xline != null && yline != null)
            {
                xline.Y1 = currentPoint.Y;
                xline.Y2 = currentPoint.Y;
                yline.X1 = currentPoint.X;
                yline.X2 = currentPoint.X;
            }
            else if (drawCanvas.Cursor != Cursors.Cross && xline != null && yline != null)
            {
                drawCanvas.Children.Remove(xline);
                drawCanvas.Children.Remove(yline);
                xline = null;
                yline = null;
            }
            if (mode == "image" && image != null)
            {
                Canvas.SetLeft(image, currentPoint.X);
                Canvas.SetTop(image, currentPoint.Y);
            }
            if (mode == "diversion")
            {
                Line tp = drawCanvas.Children[drawCanvas.Children.Count - 1] as Line;
                tp.X2 = currentPoint.X;
                tp.Y2 = currentPoint.Y;
            }
        }

        private void drawOverlayMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (set && !Gmap.IsMouseCaptured) Gmap.CaptureMouse();
                if (currentPoint != clickedPoint && draggable)
                {
                    PointLatLng pos;
                    if (ellipse != null)
                    {
                        RouteData datlink = ellipse.Tag as RouteData;
                        pos = Gmap.FromLocalToLatLng((int)Canvas.GetLeft(ellipse), (int)Canvas.GetTop(ellipse));
                        if (datlink.objType == "Circle")
                        {
                            datlink.pos1 = pos;
                            PointLatLng size = Gmap.FromLocalToLatLng((int)(Canvas.GetLeft(ellipse) + ellipse.ActualWidth / 2), (int)Canvas.GetTop(ellipse));
                            datlink.pos2 = size;
                            updateValue("circle");
                            routeProperties.Visibility = Visibility.Collapsed;
                            lineProperties.Visibility = Visibility.Collapsed;
                            polygonProperties.Visibility = Visibility.Collapsed;
                            imageProperties.Visibility = Visibility.Collapsed;
                            circleProperties.Visibility = Visibility.Visible;
                        }
                        else if (datlink.objType == "Route" || datlink.objType == "Polygon")
                        {
                            foreach (UIElement cv in drawCanvas.Children)
                            {
                                if (cv is Canvas)
                                {
                                    for (int f = 0; f < (cv as Canvas).Children.Count; f++)
                                    {
                                        if (((cv as Canvas).Children[f] as StackPanel).Tag is CheckpointData)
                                        {
                                            if (!(((cv as Canvas).Children[f] as StackPanel).Tag as CheckpointData).isMark)
                                            {
                                                if ((cv as Canvas).Children[f].Visibility == Visibility.Hidden)
                                                {
                                                    (cv as Canvas).Children.RemoveAt(f);
                                                    f--;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
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
                                if (datlink.type == "Diversion") goto skipBlock;
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
                                            if (((drawCanvas.Children[i] as Line).Tag as RouteData).type == "Diversion") continue;
                                            else if (((drawCanvas.Children[i] as Line).Tag as RouteData).componentID == (datlink.componentID - 1))
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
                        skipBlock:
                            if (datlink.objType == "Route")
                            {
                                addwaypntBtn.IsEnabled = true;
                                line = BindingOperations.GetBinding(ellipse, Ellipse.StrokeProperty).Source as Line;
                                refreshRoutePanel();
                                updateValue("route");
                                updateRouteDataInfo((ellipse.Tag as RouteData).objID);
                                lineProperties.Visibility = Visibility.Collapsed;
                                polygonProperties.Visibility = Visibility.Collapsed;
                                circleProperties.Visibility = Visibility.Collapsed;
                                imageProperties.Visibility = Visibility.Collapsed;
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
                                imageProperties.Visibility = Visibility.Collapsed;
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
                            imageProperties.Visibility = Visibility.Collapsed;
                            lineProperties.Visibility = Visibility.Visible;
                        }
                    }
                    else if (image != null)
                    {
                        pos = Gmap.FromLocalToLatLng((int)Canvas.GetLeft(image), (int)Canvas.GetTop(image));
                        if ((image.Tag is ImageData))
                        {
                            (image.Tag as ImageData).position = pos;
                        }
                        updateValue("image");
                        polygonProperties.Visibility = Visibility.Collapsed;
                        circleProperties.Visibility = Visibility.Collapsed;
                        routeProperties.Visibility = Visibility.Collapsed;
                        lineProperties.Visibility = Visibility.Collapsed;
                        imageProperties.Visibility = Visibility.Visible;
                    }
                }
                draggable = false;
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
                            clearCache();
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
                                    imageProperties.Visibility = Visibility.Collapsed;
                                    routeProperties.Visibility = Visibility.Visible;
                                    routeLineBlock.Visibility = Visibility.Visible;
                                    routewaypointDataBlock.Visibility = Visibility.Visible;
                                    routeProperties.IsEnabled = true;
                                    showCheckpointList(line);
                                }
                                else if (datlink.objType == "Line")
                                {
                                    ellipse = BindingOperations.GetBinding(line, Line.X2Property).Source as Ellipse;
                                    updateValue("line");
                                    polygonProperties.Visibility = Visibility.Collapsed;
                                    circleProperties.Visibility = Visibility.Collapsed;
                                    routeProperties.Visibility = Visibility.Collapsed;
                                    imageProperties.Visibility = Visibility.Collapsed;
                                    lineProperties.Visibility = Visibility.Visible;
                                }
                                else if (datlink.objType == "Polygon")
                                {
                                    ellipse = BindingOperations.GetBinding(line, Line.X2Property).Source as Ellipse;
                                    updateValue("polygon");
                                    circleProperties.Visibility = Visibility.Collapsed;
                                    routeProperties.Visibility = Visibility.Collapsed;
                                    lineProperties.Visibility = Visibility.Collapsed;
                                    imageProperties.Visibility = Visibility.Collapsed;
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
                                    imageProperties.Visibility = Visibility.Collapsed;
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
                                    imageProperties.Visibility = Visibility.Collapsed;
                                    routeProperties.Visibility = Visibility.Visible;
                                    routeLineBlock.Visibility = Visibility.Visible;
                                    routewaypointDataBlock.Visibility = Visibility.Visible;
                                    routeProperties.IsEnabled = true;
                                    showCheckpointList(line);
                                }
                                else if (datlink.objType == "Line")
                                {
                                    line = BindingOperations.GetBinding(ellipse, Ellipse.StrokeProperty).Source as Line;
                                    updateValue("line");
                                    polygonProperties.Visibility = Visibility.Collapsed;
                                    circleProperties.Visibility = Visibility.Collapsed;
                                    routeProperties.Visibility = Visibility.Collapsed;
                                    imageProperties.Visibility = Visibility.Collapsed;
                                    lineProperties.Visibility = Visibility.Visible;
                                }
                                else if (datlink.objType == "Polygon")
                                {
                                    line = BindingOperations.GetBinding(ellipse, Ellipse.StrokeProperty).Source as Line;
                                    updateValue("polygon");
                                    circleProperties.Visibility = Visibility.Collapsed;
                                    routeProperties.Visibility = Visibility.Collapsed;
                                    lineProperties.Visibility = Visibility.Collapsed;
                                    imageProperties.Visibility = Visibility.Collapsed;
                                    polygonProperties.Visibility = Visibility.Visible;
                                }
                            }
                            else if (hit is Image)
                            {
                                if ((hit as Image).Tag is ImageData)
                                {
                                    image = hit as Image;
                                    updateValue("image");
                                    circleProperties.Visibility = Visibility.Collapsed;
                                    routeProperties.Visibility = Visibility.Collapsed;
                                    lineProperties.Visibility = Visibility.Collapsed;
                                    polygonProperties.Visibility = Visibility.Collapsed;
                                    imageProperties.Visibility = Visibility.Visible;
                                }
                                else
                                {
                                    clearCache();
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
                                ellipse.SetBinding(Ellipse.StrokeProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                                ellipse.SetBinding(Ellipse.StrokeThicknessProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeThicknessProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                                ellipse.SetBinding(Ellipse.VisibilityProperty, new Binding { Source = line, Path = new PropertyPath(Line.VisibilityProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                                ellipse.SetBinding(Ellipse.MarginProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.ActualWidthProperty), Converter = new EllipseMarginConverter() });
                                ellipse.IsVisibleChanged += partsCleanup;
                                line.SetBinding(Line.X2Property, new Binding { Source = ellipse, Path = new PropertyPath(Canvas.LeftProperty) });
                                line.SetBinding(Line.Y2Property, new Binding { Source = ellipse, Path = new PropertyPath(Canvas.TopProperty) });
                                (line.Tag as RouteData).pos2 = position;
                                var pos1 = (line.Tag as RouteData).pos1;
                                var tp = (line.Tag as RouteData).type;
                                Viewbox arrow = new Viewbox { Child = new Path { Stroke = Brushes.Blue, StrokeThickness = 2, Data = PathGeometry.Parse(@"M20,5L0,0L5,5L0,10Z"), Fill = Brushes.Blue, Margin = new Thickness(-6, -5, -13, -5) } };
                                (arrow.Child as Path).SetBinding(Path.StrokeProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeProperty) });
                                (arrow.Child as Path).SetBinding(Path.FillProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeProperty) });
                                arrow.SetBinding(Viewbox.VisibilityProperty, new Binding { Source = line, Path = new PropertyPath(Line.VisibilityProperty) });
                                arrow.IsVisibleChanged += partsCleanup;
                                arrow.RenderTransform = new RotateTransform();
                                MultiBinding arrowX = new MultiBinding { Converter = new ArrowConverterXY() };
                                arrowX.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.X1Property) });
                                arrowX.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.X2Property) });
                                arrow.SetBinding(Canvas.LeftProperty, arrowX);
                                MultiBinding arrowY = new MultiBinding { Converter = new ArrowConverterXY() };
                                arrowY.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.Y1Property) });
                                arrowY.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.Y2Property) });
                                arrow.SetBinding(Canvas.TopProperty, arrowY);
                                MultiBinding arrowθ = new MultiBinding { Converter = new ArrowConverterθ() };
                                arrowθ.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.X1Property) });
                                arrowθ.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.Y1Property) });
                                arrowθ.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.X2Property) });
                                arrowθ.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.Y2Property) });
                                BindingOperations.SetBinding(arrow.RenderTransform as RotateTransform, RotateTransform.AngleProperty, arrowθ);
                                Canvas routeInfoContainer = new Canvas { IsHitTestVisible = false, RenderTransformOrigin = new Point(0.5, 0.5), RenderTransform = new RotateTransform() };
                                StackPanel routeDataContainer = new StackPanel { Orientation = Orientation.Horizontal, RenderTransformOrigin = new Point(0, 0.5), RenderTransform = new ScaleTransform() };
                                MultiBinding rHeight = new MultiBinding() { Converter = new DataPanelContainerHeightConverter() };
                                rHeight.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.X1Property) });
                                rHeight.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.Y1Property) });
                                rHeight.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.X2Property) });
                                rHeight.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.Y2Property) });
                                routeInfoContainer.SetBinding(Canvas.HeightProperty, rHeight);
                                routeInfoContainer.SetBinding(Canvas.WidthProperty, new Binding { Source = routeDataContainer, Path = new PropertyPath(StackPanel.ActualWidthProperty), Converter = new DataPanelContainerWidthConverter() });
                                MultiBinding rMargin = new MultiBinding() { Converter = new RectangleMarginConverter() };
                                rMargin.Bindings.Add(new Binding { Source = routeInfoContainer, Path = new PropertyPath(Canvas.ActualWidthProperty) });
                                rMargin.Bindings.Add(new Binding { Source = routeInfoContainer, Path = new PropertyPath(Canvas.ActualHeightProperty) });
                                routeInfoContainer.SetBinding(Canvas.MarginProperty, rMargin);
                                routeDataContainer.SetBinding(Canvas.LeftProperty, new Binding { Source = routeInfoContainer, Path = new PropertyPath(Canvas.ActualWidthProperty), Converter = new MiddlePositionConverter() });
                                routeDataContainer.SetBinding(Canvas.TopProperty, new Binding { Source = routeInfoContainer, Path = new PropertyPath(Canvas.ActualHeightProperty), Converter = new MiddlePositionConverter() });
                                routeInfoContainer.SetBinding(Canvas.VisibilityProperty, new Binding { Source = line, Path = new PropertyPath(Line.VisibilityProperty) });
                                routeInfoContainer.IsVisibleChanged += partsCleanup;
                                routeInfoContainer.Children.Add(routeDataContainer);
                                routeDataPanel = new StackPanel { Orientation = Orientation.Vertical, RenderTransformOrigin = new Point(0.5, 0.5), RenderTransform = new ScaleTransform() };
                                routeDataContainer.SetBinding(StackPanel.MarginProperty, new Binding { Source = routeDataContainer, Path = new PropertyPath(StackPanel.ActualHeightProperty), Converter = new DataPanelMarginConverter() });
                                routeDataContainer.Children.Add(routeDataPanel);
                                MultiBinding datapanelmargin = new MultiBinding { Converter = new DataPanelOffsetConverter() };
                                datapanelmargin.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.StrokeThicknessProperty) });
                                datapanelmargin.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.TagProperty) });
                                routeDataPanel.SetBinding(StackPanel.MarginProperty, datapanelmargin);
                                Ellipse bc = new Ellipse { Stroke = Brushes.Red, Fill = Brushes.DarkGray, StrokeThickness = 2 };
                                StackPanel fval = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Center };
                                fval.Children.Add(new Label { HorizontalContentAlignment = HorizontalAlignment.Center, Foreground = Brushes.Black, FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 5, 0, 0), BorderBrush = Brushes.Yellow, BorderThickness = new Thickness(0, 0, 0, 1) });
                                fval.Children.Add(new Label { HorizontalContentAlignment = HorizontalAlignment.Center, Foreground = Brushes.Red, FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5), BorderBrush = Brushes.Yellow, BorderThickness = new Thickness(0, 1, 0, 0) });
                                Grid fuelcircle = new Grid() { Background = Brushes.Transparent, Width = 120, Height = 120, Margin = new Thickness(0, 20, 0, 0) };
                                fuelcircle.ColumnDefinitions.Add(new ColumnDefinition());
                                fuelcircle.RowDefinitions.Add(new RowDefinition());
                                Grid.SetRow(bc, 0);
                                Grid.SetColumn(bc, 0);
                                Grid.SetRow(fval, 0);
                                Grid.SetColumn(fval, 0);
                                fuelcircle.Children.Add(bc);
                                fuelcircle.Children.Add(fval);
                                StackPanel headingBox = new StackPanel { Orientation = Orientation.Vertical, Width = 120, Height = 180, Background = Brushes.Transparent };
                                headingBox.Children.Add(new Path { Stroke = Brushes.Black, StrokeThickness = 3, Fill = Brushes.DarkRed, Data = PathGeometry.Parse("M1.5,1.5 L1.5,38.5 118.5,38.5 Z"), RenderTransformOrigin = new Point(0.4685, 0.5), RenderTransform = new ScaleTransform() });
                                headingBox.Children.Add(new Label { Width = 120, Height = 35, Foreground = Brushes.Red, Background = Brushes.DarkGray, BorderBrush = Brushes.Black, BorderThickness = new Thickness(3, 0, 0, 3), FontSize = 16, FontWeight = FontWeights.Bold });
                                headingBox.Children.Add(new Label { Width = 120, Height = 35, Foreground = Brushes.Black, Background = Brushes.DarkGray, BorderBrush = Brushes.Black, BorderThickness = new Thickness(3, 0, 0, 3), FontSize = 16, FontWeight = FontWeights.Bold });
                                headingBox.Children.Add(new Label { Width = 120, Height = 35, Foreground = Brushes.Red, Background = Brushes.DarkGray, BorderBrush = Brushes.Black, BorderThickness = new Thickness(3, 0, 0, 3), FontSize = 16, FontWeight = FontWeights.Bold });
                                headingBox.Children.Add(new Label { Width = 120, Height = 35, Foreground = Brushes.Black, Background = Brushes.DarkGray, BorderBrush = Brushes.Black, BorderThickness = new Thickness(3, 0, 0, 3), FontSize = 16, FontWeight = FontWeights.Bold });
                                routeDataPanel.Children.Add(headingBox);
                                routeDataPanel.Children.Add(fuelcircle);
                                MultiBinding X = new MultiBinding { Converter = new HeadingBoxConverterX() };
                                X.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.X1Property) });
                                X.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.Y1Property) });
                                X.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.X2Property) });
                                X.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.Y2Property) });
                                //X.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.TagProperty) });
                                //X.Bindings.Add(new Binding { Source = routeDataPanel, Path = new PropertyPath(StackPanel.ActualWidthProperty) });
                                routeInfoContainer.SetBinding(Canvas.LeftProperty, X);
                                MultiBinding Y = new MultiBinding { Converter = new HeadingBoxConverterY() };
                                Y.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.X1Property) });
                                Y.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.Y1Property) });
                                Y.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.X2Property) });
                                Y.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.Y2Property) });
                                //Y.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.TagProperty) });
                                //Y.Bindings.Add(new Binding { Source = routeDataPanel, Path = new PropertyPath(StackPanel.ActualWidthProperty) });
                                routeInfoContainer.SetBinding(Canvas.TopProperty, Y);
                                MultiBinding θ = new MultiBinding { Converter = new HeadingBoxConverterθ() };
                                θ.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.X1Property) });
                                θ.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.Y1Property) });
                                θ.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.X2Property) });
                                θ.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.Y2Property) });
                                BindingOperations.SetBinding(routeInfoContainer.RenderTransform as RotateTransform, RotateTransform.AngleProperty, θ);
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
                                temp.SetBinding(Line.VisibilityProperty, new Binding { Source = line, Path = new PropertyPath(Line.VisibilityProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                                temp.IsVisibleChanged += partsCleanup;
                                Line temp2 = line;
                                line = temp;
                                Panel.SetZIndex(line, 4);
                                line.SetBinding(Line.X1Property, new Binding { Source = ellipse, Path = new PropertyPath(Canvas.LeftProperty) });
                                line.SetBinding(Line.Y1Property, new Binding { Source = ellipse, Path = new PropertyPath(Canvas.TopProperty) });
                                (headingBox.Children[1] as Label).SetBinding(Label.ContentProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.TagProperty), Converter = new RouteInfoConverter(), ConverterParameter = "track" });
                                (headingBox.Children[2] as Label).SetBinding(Label.ContentProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.TagProperty), Converter = new RouteInfoConverter(), ConverterParameter = "time" });
                                (headingBox.Children[3] as Label).SetBinding(Label.ContentProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.TagProperty), Converter = new RouteInfoConverter(), ConverterParameter = "fuel" });
                                (headingBox.Children[4] as Label).SetBinding(Label.ContentProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.TagProperty), Converter = new RouteInfoConverter(), ConverterParameter = "alt" });
                                (fval.Children[0] as Label).SetBinding(Label.ContentProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.TagProperty), Converter = new RouteInfoConverter(), ConverterParameter = "rem" });
                                (fval.Children[1] as Label).SetBinding(Label.ContentProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.TagProperty), Converter = new RouteInfoConverter(), ConverterParameter = "frcs" });
                                drawCanvas.Children.Add(ellipse);
                                drawCanvas.Children.Add(line);
                                drawCanvas.Children.Add(routeInfoContainer);
                                drawCanvas.Children.Add(arrow);
                                ellipse.Tag = new RouteData(drawCanvas, baseunit.baltUnit, baseunit.bdistUnit, baseunit.bspeedUnit, baseunit.bfuelUnit, baseunit.blfftUnit) { objType = "Route", objID = routeCount, componentID = markerCount, isDraggable = true, pos1 = pos1, pos2 = position, startingfuel = Convert.ToDouble(routeStartingFuelBox.Text), type = tp, totaldistanceUnit = baseunit.bdistUnit, distanceUnit = baseunit.bdistUnit };
                                line.Tag = new RouteData(drawCanvas, baseunit.baltUnit, baseunit.bdistUnit, baseunit.bspeedUnit, baseunit.bfuelUnit, baseunit.blfftUnit) { objType = "Route", objID = routeCount, componentID = count, isDraggable = false, pos1 = position, startingfuel = Convert.ToDouble(routeStartingFuelBox.Text), type = "Enroute", totaldistanceUnit = baseunit.bdistUnit, distanceUnit = baseunit.bdistUnit };
                                routeDataContainer.Tag = new RouteData(drawCanvas, baseunit.baltUnit, baseunit.bdistUnit, baseunit.bspeedUnit, baseunit.bfuelUnit, baseunit.blfftUnit) { objType = "Route", objID = routeCount, componentID = boxCount, isDraggable = false, pos1 = pos1, pos2 = position, startingfuel = Convert.ToDouble(routeStartingFuelBox.Text), type = tp, totaldistanceUnit = baseunit.bdistUnit, distanceUnit = baseunit.bdistUnit };
                                (ellipse.Tag as RouteData).setfData(frdata);
                                (line.Tag as RouteData).setfData(frdata);
                                (routeDataContainer.Tag as RouteData).setfData(frdata);
                                (ellipse.Tag as RouteData).setpData(pdata, routeAltBox.SelectedIndex, Convert.ToDouble(routeSpeedBox.SelectedValue));
                                (line.Tag as RouteData).setpData(pdata, routeAltBox.SelectedIndex, Convert.ToDouble(routeSpeedBox.SelectedValue));
                                (routeDataContainer.Tag as RouteData).setpData(pdata, routeAltBox.SelectedIndex, Convert.ToDouble(routeSpeedBox.SelectedValue));
                                updateValue(mode);
                                updateRouteDataInfo((ellipse.Tag as RouteData).objID);

                                //TOC TOD Markings....... WORK HERE

                                StackPanel _checkpoint = new StackPanel { Orientation = Orientation.Horizontal, RenderTransform = new ScaleTransform() };
                                Panel.SetZIndex(_checkpoint, 2);
                                Line _chkmark = new Line { X1 = 0, Y1 = 0, Y2 = 0, StrokeThickness = 2, VerticalAlignment = VerticalAlignment.Center };
                                _chkmark.SetBinding(Line.X2Property, new Binding { Source = temp2, Path = new PropertyPath(Line.StrokeThicknessProperty), Converter = new CheckpointLengthConverter() });
                                _chkmark.SetBinding(Line.StrokeProperty, new Binding { Source = temp2, Path = new PropertyPath(Line.StrokeProperty) });
                                _chkmark.SetBinding(Line.StrokeThicknessProperty, new Binding { Source = temp2, Path = new PropertyPath(Line.StrokeThicknessProperty) });
                                _checkpoint.Children.Add(_chkmark);
                                _checkpoint.Children.Add(new Label { Content = "TOC", FontSize = 20, RenderTransformOrigin = new Point(0.5, 0.5), RenderTransform = new ScaleTransform() });
                                MultiBinding _chkmargin = new MultiBinding { Converter = new CheckpointMarginConverter() };
                                _chkmargin.Bindings.Add(new Binding { Source = _checkpoint, Path = new PropertyPath(StackPanel.ActualHeightProperty) });
                                _chkmargin.Bindings.Add(new Binding { Source = _chkmark, Path = new PropertyPath(Line.X2Property) });
                                _checkpoint.SetBinding(MarginProperty, _chkmargin);
                                MultiBinding _chkorigin = new MultiBinding { Converter = new CheckpointRenderOriginConverter() };
                                _chkorigin.Bindings.Add(new Binding { Source = _chkmark, Path = new PropertyPath(Line.X2Property) });
                                _chkorigin.Bindings.Add(new Binding { Source = _checkpoint, Path = new PropertyPath(StackPanel.ActualWidthProperty) });
                                _checkpoint.SetBinding(StackPanel.RenderTransformOriginProperty, _chkorigin);
                                _checkpoint.Tag = new CheckpointData(temp2, ellipse, "TOC");
                                (_checkpoint.Children[1] as Label).SetBinding(Label.ForegroundProperty, new Binding { Source = _chkmark, Path = new PropertyPath(Line.StrokeProperty) });

                                _checkpoint.SetBinding(Canvas.LeftProperty, new Binding { Source = routeInfoContainer, Path = new PropertyPath(Canvas.ActualWidthProperty), Converter = new CheckpointXConverter() });
                                MultiBinding _chkpos = new MultiBinding { Converter = new CheckpointPositionConverter(), ConverterParameter = Gmap };
                                _chkpos.Bindings.Add(new Binding { Source = temp2, Path = new PropertyPath(Line.X1Property) });
                                _chkpos.Bindings.Add(new Binding { Source = temp2, Path = new PropertyPath(Line.Y1Property) });
                                _chkpos.Bindings.Add(new Binding { Source = temp2, Path = new PropertyPath(Line.X2Property) });
                                _chkpos.Bindings.Add(new Binding { Source = temp2, Path = new PropertyPath(Line.Y2Property) });
                                _chkpos.Bindings.Add(new Binding { Source = _checkpoint, Path = new PropertyPath(StackPanel.TagProperty) });
                                _checkpoint.SetBinding(Canvas.BottomProperty, _chkpos);

                                MultiBinding _chkvis = new MultiBinding { Converter = new OutOfBoundConverter(), ConverterParameter = Gmap };
                                _chkvis.Bindings.Add(new Binding { Source = routeInfoContainer, Path = new PropertyPath(Canvas.ActualHeightProperty) });
                                _chkvis.Bindings.Add(new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.TagProperty) });
                                _chkvis.Bindings.Add(new Binding { Source = _checkpoint, Path = new PropertyPath(Canvas.BottomProperty) });
                                _checkpoint.SetBinding(StackPanel.VisibilityProperty, _chkvis);

                                (_checkpoint.RenderTransform as ScaleTransform).ScaleX *= -1;
                                ((_checkpoint.Children[1] as Label).RenderTransform as ScaleTransform).ScaleX *= -1;

                                routeInfoContainer.Children.Add(_checkpoint);

                                _checkpoint = new StackPanel { Orientation = Orientation.Horizontal, RenderTransform = new ScaleTransform() };
                                Panel.SetZIndex(_checkpoint, 2);
                                _chkmark = new Line { X1 = 0, Y1 = 0, Y2 = 0, StrokeThickness = 2, VerticalAlignment = VerticalAlignment.Center };
                                _chkmark.SetBinding(Line.X2Property, new Binding { Source = temp2, Path = new PropertyPath(Line.StrokeThicknessProperty), Converter = new CheckpointLengthConverter() });
                                _chkmark.SetBinding(Line.StrokeProperty, new Binding { Source = temp2, Path = new PropertyPath(Line.StrokeProperty) });
                                _chkmark.SetBinding(Line.StrokeThicknessProperty, new Binding { Source = temp2, Path = new PropertyPath(Line.StrokeThicknessProperty) });
                                _checkpoint.Children.Add(_chkmark);
                                _checkpoint.Children.Add(new Label { Content = "TOD", FontSize = 20, RenderTransformOrigin = new Point(0.5, 0.5), RenderTransform = new ScaleTransform() });
                                _chkmargin = new MultiBinding { Converter = new CheckpointMarginConverter() };
                                _chkmargin.Bindings.Add(new Binding { Source = _checkpoint, Path = new PropertyPath(StackPanel.ActualHeightProperty) });
                                _chkmargin.Bindings.Add(new Binding { Source = _chkmark, Path = new PropertyPath(Line.X2Property) });
                                _checkpoint.SetBinding(MarginProperty, _chkmargin);
                                _chkorigin = new MultiBinding { Converter = new CheckpointRenderOriginConverter() };
                                _chkorigin.Bindings.Add(new Binding { Source = _chkmark, Path = new PropertyPath(Line.X2Property) });
                                _chkorigin.Bindings.Add(new Binding { Source = _checkpoint, Path = new PropertyPath(StackPanel.ActualWidthProperty) });
                                _checkpoint.SetBinding(StackPanel.RenderTransformOriginProperty, _chkorigin);
                                _checkpoint.Tag = new CheckpointData(temp2, ellipse, "TOD");
                                (_checkpoint.Children[1] as Label).SetBinding(Label.ForegroundProperty, new Binding { Source = _chkmark, Path = new PropertyPath(Line.StrokeProperty) });

                                _checkpoint.SetBinding(Canvas.LeftProperty, new Binding { Source = routeInfoContainer, Path = new PropertyPath(Canvas.ActualWidthProperty), Converter = new CheckpointXConverter() });
                                _chkpos = new MultiBinding { Converter = new CheckpointPositionConverter(), ConverterParameter = Gmap };
                                _chkpos.Bindings.Add(new Binding { Source = temp2, Path = new PropertyPath(Line.X1Property) });
                                _chkpos.Bindings.Add(new Binding { Source = temp2, Path = new PropertyPath(Line.Y1Property) });
                                _chkpos.Bindings.Add(new Binding { Source = temp2, Path = new PropertyPath(Line.X2Property) });
                                _chkpos.Bindings.Add(new Binding { Source = temp2, Path = new PropertyPath(Line.Y2Property) });
                                _chkpos.Bindings.Add(new Binding { Source = _checkpoint, Path = new PropertyPath(StackPanel.TagProperty) });
                                _checkpoint.SetBinding(Canvas.TopProperty, _chkpos);

                                _chkvis = new MultiBinding { Converter = new OutOfBoundConverter(), ConverterParameter = Gmap };
                                _chkvis.Bindings.Add(new Binding { Source = routeInfoContainer, Path = new PropertyPath(Canvas.ActualHeightProperty) });
                                _chkvis.Bindings.Add(new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.TagProperty) });
                                _chkvis.Bindings.Add(new Binding { Source = _checkpoint, Path = new PropertyPath(Canvas.TopProperty) });
                                _checkpoint.SetBinding(StackPanel.VisibilityProperty, _chkvis);

                                (_checkpoint.RenderTransform as ScaleTransform).ScaleX *= -1;
                                ((_checkpoint.Children[1] as Label).RenderTransform as ScaleTransform).ScaleX *= -1;

                                routeInfoContainer.Children.Add(_checkpoint);

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
                                ellipse.SetBinding(Ellipse.StrokeProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                                ellipse.SetBinding(Ellipse.StrokeThicknessProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeThicknessProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                                ellipse.SetBinding(Ellipse.VisibilityProperty, new Binding { Source = line, Path = new PropertyPath(Line.VisibilityProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                                ellipse.SetBinding(Ellipse.MarginProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.ActualWidthProperty), Converter = new EllipseMarginConverter() });
                                ellipse.IsVisibleChanged += partsCleanup;
                                line.IsVisibleChanged += partsCleanup;
                                line.SetBinding(Line.X1Property, new Binding { Source = ellipse, Path = new PropertyPath(Canvas.LeftProperty) });
                                line.SetBinding(Line.Y1Property, new Binding { Source = ellipse, Path = new PropertyPath(Canvas.TopProperty) });
                                drawCanvas.Children.Add(ellipse);
                                drawCanvas.Children.Add(line);
                                ellipse.Tag = new RouteData(drawCanvas, baseunit.baltUnit, baseunit.bdistUnit, baseunit.bspeedUnit, baseunit.bfuelUnit, baseunit.blfftUnit) { objType = "Route", objID = routeCount, componentID = markerCount, isDraggable = true, pos1 = position, pos2 = position, startingfuel = Convert.ToDouble(routeStartingFuelBox.Text), type = "Origin", totaldistanceUnit = baseunit.bdistUnit, distanceUnit = baseunit.bdistUnit };
                                line.Tag = new RouteData(drawCanvas, baseunit.baltUnit, baseunit.bdistUnit, baseunit.bspeedUnit, baseunit.bfuelUnit, baseunit.blfftUnit) { objType = "Route", objID = routeCount, componentID = count, isDraggable = false, pos1 = position, startingfuel = Convert.ToDouble(routeStartingFuelBox.Text), type = "Starting", totaldistanceUnit = baseunit.bdistUnit, distanceUnit = baseunit.bdistUnit };
                                (ellipse.Tag as RouteData).setpData(pdata, routeAltBox.SelectedIndex, Convert.ToDouble(routeSpeedBox.SelectedValue));
                                (line.Tag as RouteData).setpData(pdata, routeAltBox.SelectedIndex, Convert.ToDouble(routeSpeedBox.SelectedValue));
                                (ellipse.Tag as RouteData).setfData(frdata);
                                (line.Tag as RouteData).setfData(frdata);
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
                                ellipse.SetBinding(Ellipse.MarginProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.ActualWidthProperty), Converter = new EllipseMarginConverter() });
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
                                imageProperties.Visibility = Visibility.Collapsed;
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
                                ellipse.SetBinding(Ellipse.MarginProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.ActualWidthProperty), Converter = new EllipseMarginConverter() });
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
                            ellipse.SetBinding(Ellipse.MarginProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.ActualWidthProperty), Converter = new EllipseMarginConverter() });
                            ellipse.Tag = new RouteData(drawCanvas, "KM", "KM", "KPH", "KG", "PER KM") { objType = "Circle", objID = circleCount, componentID = circleCount, isDraggable = true, pos1 = position, pos2 = sizePoint };
                            drawCanvas.Children.Add(ellipse);
                            mode = "none";
                            drawCanvas.Cursor = Cursors.Arrow;
                            updateValue("circle");
                            drawn = false;
                            routeProperties.Visibility = Visibility.Collapsed;
                            lineProperties.Visibility = Visibility.Collapsed;
                            polygonProperties.Visibility = Visibility.Collapsed;
                            imageProperties.Visibility = Visibility.Collapsed;
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
                                ellipse.SetBinding(Ellipse.MarginProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.ActualWidthProperty), Converter = new EllipseMarginConverter() });
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
                                ellipse.SetBinding(Ellipse.MarginProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.ActualWidthProperty), Converter = new EllipseMarginConverter() });
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
                        case "checkpoint":
                            Point p;
                            if (drawCanvas.InputHitTest(clickedPoint) as Line == line)
                            {
                                p = clickedPoint;
                            }
                            else
                            {
                                double xpoint = -1, ypoint = -1, xdist = 0, ydist = 0;
                                if (line.X1 < line.X2)
                                {
                                    if (clickedPoint.X > line.X1 && clickedPoint.X < line.X2)
                                    {
                                        ypoint = line.Y1 + (((clickedPoint.X - line.X1) * (line.Y1 - line.Y2)) / (line.X1 - line.X2));
                                        ydist = Math.Abs(ypoint - clickedPoint.Y);
                                    }
                                }
                                else if (line.X1 > line.X2)
                                {
                                    if (clickedPoint.X > line.X2 && clickedPoint.X < line.X1)
                                    {
                                        ypoint = line.Y1 + (((clickedPoint.X - line.X1) * (line.Y1 - line.Y2)) / (line.X1 - line.X2));
                                        ydist = Math.Abs(ypoint - clickedPoint.Y);
                                    }
                                }
                                if (line.Y1 < line.Y2)
                                {
                                    if (clickedPoint.Y > line.Y1 && clickedPoint.Y < line.Y2)
                                    {
                                        xpoint = line.X1 + (((clickedPoint.Y - line.Y1) * (line.X1 - line.X2)) / (line.Y1 - line.Y2));
                                        xdist = Math.Abs(xpoint - clickedPoint.X);
                                    }
                                }
                                else if (line.Y1 > line.Y2)
                                {
                                    if (clickedPoint.Y > line.Y2 && clickedPoint.Y < line.Y1)
                                    {
                                        xpoint = line.X1 + (((clickedPoint.Y - line.Y1) * (line.X1 - line.X2)) / (line.Y1 - line.Y2));
                                        xdist = Math.Abs(xpoint - clickedPoint.X);
                                    }
                                }
                                if (xpoint == -1 && ypoint == -1)
                                {
                                    p = new Point(-1, -1);
                                }
                                else if (xpoint == -1 && ypoint != -1)
                                {
                                    p = new Point(clickedPoint.X, ypoint);
                                }
                                else if (xpoint != -1 && ypoint == -1)
                                {
                                    p = new Point(xpoint, clickedPoint.Y);
                                }
                                else if (xpoint != -1 && ypoint != -1 && xdist <= ydist)
                                {
                                    p = new Point(xpoint, clickedPoint.Y);
                                }
                                else if (xpoint != -1 && ypoint != -1 && xdist > ydist)
                                {
                                    p = new Point(clickedPoint.X, ypoint);
                                }
                                else
                                {
                                    p = new Point(-1, -1);
                                }
                            }
                            if (p == new Point(-1, -1)) return;
                            Line ml = line;
                            Canvas container = null;
                            foreach (UIElement c in drawCanvas.Children)
                            {
                                if (c is Canvas)
                                {
                                    foreach (UIElement ch in (c as Canvas).Children)
                                    {
                                        if (ch is StackPanel)
                                        {
                                            if ((ch as StackPanel).Tag is RouteData)
                                            {
                                                if ((((ch as StackPanel).Tag as RouteData).objID == (ml.Tag as RouteData).objID) && ((ch as StackPanel).Tag as RouteData).componentID == ((ml.Tag as RouteData).componentID))
                                                {
                                                    container = c as Canvas;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            StackPanel checkpoint = new StackPanel { Orientation = Orientation.Horizontal, RenderTransform = new ScaleTransform() };
                            Line chkmark = new Line { X1 = 0, Y1 = 0, Y2 = 0, StrokeThickness = 2, VerticalAlignment = VerticalAlignment.Center };
                            chkmark.SetBinding(Line.X2Property, new Binding { Source = ml, Path = new PropertyPath(Line.StrokeThicknessProperty), Converter = new CheckpointLengthConverter() });
                            chkmark.SetBinding(Line.StrokeProperty, new Binding { Source = ml, Path = new PropertyPath(Line.StrokeProperty) });
                            chkmark.SetBinding(Line.StrokeThicknessProperty, new Binding { Source = ml, Path = new PropertyPath(Line.StrokeThicknessProperty) });
                            checkpoint.Children.Add(chkmark);
                            checkpoint.Children.Add(new Label { FontSize = 20, FontWeight = FontWeights.Bold, RenderTransformOrigin = new Point(0.5, 0.5), RenderTransform = new ScaleTransform() });
                            MultiBinding chkmargin = new MultiBinding { Converter = new CheckpointMarginConverter() };
                            chkmargin.Bindings.Add(new Binding { Source = checkpoint, Path = new PropertyPath(StackPanel.ActualHeightProperty) });
                            chkmargin.Bindings.Add(new Binding { Source = chkmark, Path = new PropertyPath(Line.X2Property) });
                            checkpoint.SetBinding(MarginProperty, chkmargin);
                            MultiBinding chkorigin = new MultiBinding { Converter = new CheckpointRenderOriginConverter() };
                            chkorigin.Bindings.Add(new Binding { Source = chkmark, Path = new PropertyPath(Line.X2Property) });
                            chkorigin.Bindings.Add(new Binding { Source = checkpoint, Path = new PropertyPath(StackPanel.ActualWidthProperty) });
                            checkpoint.SetBinding(StackPanel.RenderTransformOriginProperty, chkorigin);

                            checkpoint.Tag = new CheckpointData(Gmap, ml, ellipse, p);
                            (checkpoint.Children[1] as Label).SetBinding(Label.ContentProperty, new Binding { Source = checkpoint, Path = new PropertyPath(StackPanel.TagProperty), Converter = new TimeSpanConverter(), ConverterParameter = "Checkpoint" });
                            (checkpoint.Children[1] as Label).SetBinding(Label.ForegroundProperty, new Binding { Source = chkmark, Path = new PropertyPath(Line.StrokeProperty) });

                            checkpoint.SetBinding(Canvas.LeftProperty, new Binding { Source = container, Path = new PropertyPath(Canvas.ActualWidthProperty), Converter = new CheckpointXConverter() });
                            MultiBinding chkpos = new MultiBinding { Converter = new CheckpointPositionConverter(), ConverterParameter = Gmap };
                            chkpos.Bindings.Add(new Binding { Source = ml, Path = new PropertyPath(Line.X1Property) });
                            chkpos.Bindings.Add(new Binding { Source = ml, Path = new PropertyPath(Line.Y1Property) });
                            chkpos.Bindings.Add(new Binding { Source = ml, Path = new PropertyPath(Line.X2Property) });
                            chkpos.Bindings.Add(new Binding { Source = ml, Path = new PropertyPath(Line.Y2Property) });
                            chkpos.Bindings.Add(new Binding { Source = checkpoint, Path = new PropertyPath(StackPanel.TagProperty) });
                            checkpoint.SetBinding(Canvas.BottomProperty, chkpos);

                            MultiBinding chkvis = new MultiBinding { Converter = new OutOfBoundConverter(), ConverterParameter = "Checkpoint" };
                            chkvis.Bindings.Add(new Binding { Source = checkpoint, Path = new PropertyPath(Canvas.BottomProperty) });
                            chkvis.Bindings.Add(new Binding { Source = container, Path = new PropertyPath(Canvas.ActualHeightProperty) });
                            checkpoint.SetBinding(StackPanel.VisibilityProperty, chkvis);

                            (checkpoint.RenderTransform as ScaleTransform).ScaleX *= -1;
                            ((checkpoint.Children[1] as Label).RenderTransform as ScaleTransform).ScaleX *= -1;

                            container.Children.Add(checkpoint);
                            showCheckpointList(ml);
                            break;
                        case "diversion":
                            Ellipse ep = new Ellipse
                            {
                                Width = 20,
                                Height = 20,
                                Fill = Brushes.Transparent,
                                Margin = new Thickness(-10),
                                StrokeDashArray = new DoubleCollection { 3, 2 }
                            };
                            Canvas.SetLeft(ep, clickedPoint.X);
                            Canvas.SetTop(ep, clickedPoint.Y);
                            Line lp = drawCanvas.Children[drawCanvas.Children.Count - 1] as Line;
                            lp.SetBinding(Line.X2Property, new Binding { Source = ep, Path = new PropertyPath(Canvas.LeftProperty) });
                            lp.SetBinding(Line.Y2Property, new Binding { Source = ep, Path = new PropertyPath(Canvas.TopProperty) });
                            lp.IsVisibleChanged += partsCleanup;
                            ep.SetBinding(Ellipse.StrokeProperty, new Binding { Source = lp, Path = new PropertyPath(Line.StrokeProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                            ep.SetBinding(Ellipse.StrokeThicknessProperty, new Binding { Source = lp, Path = new PropertyPath(Line.StrokeThicknessProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                            ep.SetBinding(Ellipse.VisibilityProperty, new Binding { Source = lp, Path = new PropertyPath(Line.VisibilityProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                            ep.IsVisibleChanged += partsCleanup;
                            Viewbox _arrow = new Viewbox { Child = new Path { Stroke = Brushes.Blue, StrokeThickness = 2, Data = PathGeometry.Parse(@"M20,5L0,0L5,5L0,10Z"), Fill = Brushes.Blue, Margin = new Thickness(-6, -5, -13, -5) } };
                            (_arrow.Child as Path).SetBinding(Path.StrokeProperty, new Binding { Source = lp, Path = new PropertyPath(Line.StrokeProperty) });
                            (_arrow.Child as Path).SetBinding(Path.FillProperty, new Binding { Source = lp, Path = new PropertyPath(Line.StrokeProperty) });
                            _arrow.SetBinding(Viewbox.VisibilityProperty, new Binding { Source = lp, Path = new PropertyPath(Line.VisibilityProperty) });
                            _arrow.IsVisibleChanged += partsCleanup;
                            _arrow.RenderTransform = new RotateTransform();
                            MultiBinding _arrowX = new MultiBinding { Converter = new ArrowConverterXY() };
                            _arrowX.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.X1Property) });
                            _arrowX.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.X2Property) });
                            _arrow.SetBinding(Canvas.LeftProperty, _arrowX);
                            MultiBinding _arrowY = new MultiBinding { Converter = new ArrowConverterXY() };
                            _arrowY.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.Y1Property) });
                            _arrowY.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.Y2Property) });
                            _arrow.SetBinding(Canvas.TopProperty, _arrowY);
                            MultiBinding _arrowθ = new MultiBinding { Converter = new ArrowConverterθ() };
                            _arrowθ.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.X1Property) });
                            _arrowθ.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.Y1Property) });
                            _arrowθ.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.X2Property) });
                            _arrowθ.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.Y2Property) });
                            BindingOperations.SetBinding(_arrow.RenderTransform as RotateTransform, RotateTransform.AngleProperty, _arrowθ);
                            Canvas _routeInfoContainer = new Canvas { IsHitTestVisible = false, RenderTransformOrigin = new Point(0.5, 0.5), RenderTransform = new RotateTransform() };
                            StackPanel _routeDataContainer = new StackPanel { Orientation = Orientation.Horizontal, RenderTransformOrigin = new Point(0, 0.5), RenderTransform = new ScaleTransform() };
                            MultiBinding _rHeight = new MultiBinding() { Converter = new DataPanelContainerHeightConverter() };
                            _rHeight.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.X1Property) });
                            _rHeight.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.Y1Property) });
                            _rHeight.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.X2Property) });
                            _rHeight.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.Y2Property) });
                            _routeInfoContainer.SetBinding(Canvas.HeightProperty, _rHeight);
                            _routeInfoContainer.SetBinding(Canvas.WidthProperty, new Binding { Source = _routeDataContainer, Path = new PropertyPath(StackPanel.ActualWidthProperty), Converter = new DataPanelContainerWidthConverter() });
                            MultiBinding _rMargin = new MultiBinding() { Converter = new RectangleMarginConverter() };
                            _rMargin.Bindings.Add(new Binding { Source = _routeInfoContainer, Path = new PropertyPath(Canvas.ActualWidthProperty) });
                            _rMargin.Bindings.Add(new Binding { Source = _routeInfoContainer, Path = new PropertyPath(Canvas.ActualHeightProperty) });
                            _routeInfoContainer.SetBinding(Canvas.MarginProperty, _rMargin);
                            MultiBinding _X = new MultiBinding { Converter = new HeadingBoxConverterX() };
                            _X.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.X1Property) });
                            _X.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.Y1Property) });
                            _X.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.X2Property) });
                            _X.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.Y2Property) });
                            _routeInfoContainer.SetBinding(Canvas.LeftProperty, _X);
                            MultiBinding _Y = new MultiBinding { Converter = new HeadingBoxConverterY() };
                            _Y.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.X1Property) });
                            _Y.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.Y1Property) });
                            _Y.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.X2Property) });
                            _Y.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.Y2Property) });
                            _routeInfoContainer.SetBinding(Canvas.TopProperty, _Y);
                            MultiBinding _θ = new MultiBinding { Converter = new HeadingBoxConverterθ() };
                            _θ.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.X1Property) });
                            _θ.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.Y1Property) });
                            _θ.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.X2Property) });
                            _θ.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.Y2Property) });
                            BindingOperations.SetBinding(_routeInfoContainer.RenderTransform as RotateTransform, RotateTransform.AngleProperty, _θ);
                            _routeInfoContainer.SetBinding(Canvas.VisibilityProperty, new Binding { Source = lp, Path = new PropertyPath(Line.VisibilityProperty) });
                            _routeDataContainer.SetBinding(Canvas.LeftProperty, new Binding { Source = _routeInfoContainer, Path = new PropertyPath(Canvas.ActualWidthProperty), Converter = new MiddlePositionConverter() });
                            _routeDataContainer.SetBinding(Canvas.TopProperty, new Binding { Source = _routeInfoContainer, Path = new PropertyPath(Canvas.ActualHeightProperty), Converter = new MiddlePositionConverter() });
                            _routeDataContainer.SetBinding(StackPanel.MarginProperty, new Binding { Source = _routeDataContainer, Path = new PropertyPath(StackPanel.ActualHeightProperty), Converter = new DataPanelMarginConverter() });
                            _routeInfoContainer.Children.Add(_routeDataContainer);
                            StackPanel _routeDataPanel = new StackPanel { Orientation = Orientation.Vertical, Width = 120, RenderTransformOrigin = new Point(0.5, 0.5), RenderTransform = new ScaleTransform() };
                            MultiBinding _datapanelmargin = new MultiBinding { Converter = new DataPanelOffsetConverter() };
                            _datapanelmargin.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.StrokeThicknessProperty) });
                            _datapanelmargin.Bindings.Add(new Binding { Source = lp, Path = new PropertyPath(Line.TagProperty) });
                            _routeDataPanel.SetBinding(StackPanel.MarginProperty, _datapanelmargin);
                            _routeDataContainer.Children.Add(_routeDataPanel);
                            _routeDataPanel.Children.Add(new Label { Height = 35, RenderTransformOrigin = new Point(0.5, 0.5), RenderTransform = new ScaleTransform(), HorizontalContentAlignment = HorizontalAlignment.Center, Foreground = Brushes.Black, Background = Brushes.DarkGray, FontSize = 16, FontWeight = FontWeights.Bold });
                            _routeDataPanel.Children.Add(new Label { Height = 35, RenderTransformOrigin = new Point(0.5, 0.5), RenderTransform = new ScaleTransform(), HorizontalContentAlignment = HorizontalAlignment.Center, Foreground = Brushes.Black, Background = Brushes.DarkGray, FontSize = 16, FontWeight = FontWeights.Bold });
                            (_routeDataPanel.Children[0] as Label).SetBinding(Label.ContentProperty, new Binding { Source = ep, Path = new PropertyPath(Ellipse.TagProperty), Converter = new RouteInfoConverter(), ConverterParameter = "track", UpdateSourceTrigger = UpdateSourceTrigger.Explicit });
                            (_routeDataPanel.Children[1] as Label).SetBinding(Label.ContentProperty, new Binding { Source = ep, Path = new PropertyPath(Ellipse.TagProperty), Converter = new RouteInfoConverter(), ConverterParameter = "time", UpdateSourceTrigger = UpdateSourceTrigger.Explicit });
                            (lp.Tag as RouteData).pos2 = position;
                            ep.Tag = lp.Tag;
                            _routeDataContainer.Tag = lp.Tag;
                            drawCanvas.Children.Add(ep);
                            drawCanvas.Children.Add(_arrow);
                            drawCanvas.Children.Add(_routeInfoContainer);
                            mode = "none";
                            updateValue("route");
                            drawCanvas.Cursor = Cursors.Arrow;
                            break;
                        case "image":
                            mode = "none";
                            Canvas.SetLeft(image, clickedPoint.X);
                            Canvas.SetTop(image, clickedPoint.Y);
                            image.Opacity = 1;
                            image.Stretch = Stretch.Fill;
                            image.Tag = new ImageData(imageCount++, position, true);
                            Panel.SetZIndex(image, 2);
                            drawCanvas.Cursor = Cursors.Arrow;
                            updateValue("image");
                            routeProperties.Visibility = Visibility.Collapsed;
                            lineProperties.Visibility = Visibility.Collapsed;
                            polygonProperties.Visibility = Visibility.Collapsed;
                            circleProperties.Visibility = Visibility.Collapsed;
                            imageProperties.Visibility = Visibility.Visible;
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
                    if (mode == "image")
                    {
                        if (image != null)
                        {
                            if (drawCanvas.Children.Contains(image))
                            {
                                drawCanvas.Children.Remove(image);
                                mode = "none";
                                drawCanvas.Cursor = Cursors.Arrow;
                            }
                        }
                    }
                    else if (!set && !drawn && mode != "none")
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
                        if (mode == "route")
                        {
                            Ellipse bc = new Ellipse { Stroke = Brushes.Yellow, Fill = Brushes.DarkGray, StrokeThickness = 2 };
                            StackPanel fval = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Center };
                            fval.Children.Add(new Label { HorizontalContentAlignment = HorizontalAlignment.Center, Foreground = Brushes.Black, FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 5, 0, 0), BorderBrush = Brushes.Yellow, BorderThickness = new Thickness(0, 0, 0, 1) });
                            fval.Children.Add(new Label { HorizontalContentAlignment = HorizontalAlignment.Center, Foreground = Brushes.Red, FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5), BorderBrush = Brushes.Yellow, BorderThickness = new Thickness(0, 1, 0, 0) });
                            Grid fuelcircle = new Grid() { Background = Brushes.Transparent, Width = 120, Height = 120, Margin = new Thickness(0, 20, 0, 0) };
                            fuelcircle.ColumnDefinitions.Add(new ColumnDefinition());
                            fuelcircle.RowDefinitions.Add(new RowDefinition());
                            Grid.SetRow(bc, 0);
                            Grid.SetColumn(bc, 0);
                            Grid.SetRow(fval, 0);
                            Grid.SetColumn(fval, 0);
                            fuelcircle.Children.Add(bc);
                            fuelcircle.Children.Add(fval);
                            (fval.Children[0] as Label).SetBinding(Label.ContentProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.TagProperty), Converter = new RouteInfoConverter(), ConverterParameter = "rem2" });
                            (fval.Children[1] as Label).SetBinding(Label.ContentProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.TagProperty), Converter = new RouteInfoConverter(), ConverterParameter = "frcs2" });
                            (routeDataPanel as StackPanel).Children.Insert(0, fuelcircle);
                            updateRouteDataInfo((ellipse.Tag as RouteData).objID);
                            lineProperties.Visibility = Visibility.Collapsed;
                            polygonProperties.Visibility = Visibility.Collapsed;
                            circleProperties.Visibility = Visibility.Collapsed;
                            imageProperties.Visibility = Visibility.Collapsed;
                            routeLineBlock.Visibility = Visibility.Visible;
                            routeProperties.IsEnabled = true;
                        }
                        else if (mode == "polygon")
                        {
                            circleProperties.Visibility = Visibility.Collapsed;
                            routeProperties.Visibility = Visibility.Collapsed;
                            lineProperties.Visibility = Visibility.Collapsed;
                            imageProperties.Visibility = Visibility.Collapsed;
                            polygonProperties.Visibility = Visibility.Visible;
                        }
                        updateValue(mode);
                        set = false;
                        drawn = false;
                        mode = "none";
                        Gmap.ReleaseMouseCapture();
                        drawCanvas.Cursor = Cursors.Arrow;
                    }
                }
            }
        }

        private void drawOverlayMouseWheel(object sender, MouseWheelEventArgs e)
        {

        }

        private void drawOverlayMouseEnter(object sender, MouseEventArgs e)
        {
            if (drawCanvas.Cursor == Cursors.Cross)
            {
                xline = new Line { Stroke = Brushes.IndianRed, StrokeThickness = 2, X1 = 0, X2 = drawCanvas.ActualWidth, Y1 = currentPoint.Y, Y2 = currentPoint.Y, IsHitTestVisible = false };
                yline = new Line { Stroke = Brushes.IndianRed, StrokeThickness = 2, X1 = currentPoint.X, X2 = currentPoint.X, Y1 = 0, Y2 = drawCanvas.ActualHeight, IsHitTestVisible = false };
                drawCanvas.Children.Add(xline);
                drawCanvas.Children.Add(yline);
            }
            if (mode == "diversion")
            {
                if (ellipse == null)
                {
                    drawCanvas.Cursor = Cursors.Arrow;
                    mode = "none";
                }
                else
                {
                    Line tp = new Line
                    {
                        X1 = Canvas.GetLeft(ellipse),
                        Y1 = Canvas.GetTop(ellipse),
                        X2 = currentPoint.X,
                        Y2 = currentPoint.Y,
                        StrokeDashArray = new DoubleCollection() { 3, 2 },
                        Tag = new RouteData(drawCanvas, baseunit.baltUnit, baseunit.bdistUnit, baseunit.bspeedUnit, baseunit.bfuelUnit, baseunit.blfftUnit) { objType = "Route", objID = routeCount, totaldistanceUnit = baseunit.bdistUnit, distanceUnit = baseunit.bdistUnit, isDraggable = true }
                    };
                    (tp.Tag as RouteData).type = "Diversion";
                    (tp.Tag as RouteData).pos1 = (ellipse.Tag as RouteData).pos2;
                    (tp.Tag as RouteData).componentID = (ellipse.Tag as RouteData).componentID + 1;
                    (tp.Tag as RouteData).setpData(pdata, routeAltBox.SelectedIndex, Convert.ToDouble(routeSpeedBox.SelectedValue));
                    (tp.Tag as RouteData).totaldistance = (ellipse.Tag as RouteData).totaldistance;
                    (tp.Tag as RouteData).totaltime = (ellipse.Tag as RouteData).totaltime;
                    (tp.Tag as RouteData).totalfuel = (ellipse.Tag as RouteData).totalfuel;
                    (tp.Tag as RouteData).startingfuel = (ellipse.Tag as RouteData).startingfuel;
                    (tp.Tag as RouteData).minima = (ellipse.Tag as RouteData).minima;
                    (tp.Tag as RouteData).mission = (ellipse.Tag as RouteData).mission;
                    tp.SetBinding(Line.X1Property, new Binding { Source = ellipse, Path = new PropertyPath(Canvas.LeftProperty) });
                    tp.SetBinding(Line.Y1Property, new Binding { Source = ellipse, Path = new PropertyPath(Canvas.TopProperty) });
                    tp.SetBinding(Line.StrokeProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.StrokeProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                    tp.SetBinding(Line.StrokeThicknessProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.StrokeThicknessProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                    tp.SetBinding(Line.VisibilityProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.VisibilityProperty), Mode = BindingMode.TwoWay, NotifyOnTargetUpdated = true });
                    drawCanvas.Children.Add(tp);
                }
            }
        }

        private void drawOverlayMouseLeave(object sender, MouseEventArgs e)
        {
            if (xline != null && yline != null)
            {
                drawCanvas.Children.Remove(xline);
                drawCanvas.Children.Remove(yline);
                xline = null;
                yline = null;
            }
            if (mode == "diversion")
            {
                if (((drawCanvas.Children[drawCanvas.Children.Count - 1] as Line).Tag as RouteData).type == "Diversion")
                {
                    drawCanvas.Children.RemoveAt(drawCanvas.Children.Count - 1);
                }
            }
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
                    if (!bafChart.IsVisible) bafChart.Visibility = Visibility.Visible;
                    else bafChart.Visibility = Visibility.Hidden;
                    break;
                case "platformsBtn":
                    Platforms platform_editor = new Platforms();
                    platform_editor.ShowDialog();
                    if (platform_editor.DialogResult.Value) updateAircraftRoute();
                    break;
                case "surfaceUnitsBtn":
                    break;
                case "refPointsBtn":
                    break;
                case "channelsBtn":
                    break;
                case "coordBtn":
                    break;
                case "managecheckpoint":
                    if (checkpointssection.Visibility == Visibility.Collapsed)
                    {
                        checkpointssection.Visibility = Visibility.Visible;
                        (sender as Button).Content = "Close Manager";
                    }
                    else
                    {
                        checkpointssection.Visibility = Visibility.Collapsed;
                        current = null;
                        (sender as Button).Content = "Manage Checkpoint";
                    }
                    showCheckpointList(line);
                    break;
                case "delcheck":
                    Canvas tmp = VisualTreeHelper.GetParent(current) as Canvas;
                    tmp.Children.Remove(current);
                    showCheckpointList(line);
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
                    clearCache();
                    markerCount = -1;
                    count = 0;
                    boxCount = -1;
                    routeCount++;
                    mode = "route";
                    drawCanvas.Cursor = Cursors.Cross;
                    break;
                case "lineBtn":
                    clearCache();
                    markerCount = -1;
                    lineCount++;
                    mode = "line";
                    drawCanvas.Cursor = Cursors.Cross;
                    break;
                case "circleBtn":
                    clearCache();
                    mode = "circle";
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
                    drawCanvas.Cursor = Cursors.Cross;
                    break;
                case "imgBtn":
                    clearCache();
                    image = loadImage();
                    if (image == null) return;
                    drawCanvas.Children.Add(image);
                    mode = "image";
                    drawCanvas.Cursor = Cursors.Cross;
                    break;
                case "surfaceBtn":
                    //mode = "surface";
                    MessageBox.Show(checkpoints.Count.ToString());
                    break;
                case "addcheckpoint":
                    mode = "checkpoint";
                    drawCanvas.Cursor = Cursors.Cross;
                    break;
                case "diversionBtn":
                    mode = "diversion";
                    drawCanvas.Cursor = Cursors.Cross;
                    break;
                case "panelFlip":
                    if (line != null)
                    {
                        Canvas container = null;
                        foreach (UIElement c in drawCanvas.Children)
                        {
                            if (c is Canvas)
                            {
                                foreach (UIElement ch in (c as Canvas).Children)
                                {
                                    if (ch is StackPanel)
                                    {
                                        if ((ch as StackPanel).Tag is RouteData)
                                        {
                                            if ((((ch as StackPanel).Tag as RouteData).objID == (line.Tag as RouteData).objID) && ((ch as StackPanel).Tag as RouteData).componentID == ((line.Tag as RouteData).componentID))
                                            {
                                                container = c as Canvas;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        flipRouteDataPanel(container);
                    }
                    break;
                default:
                    mode = "none";
                    break;
            }
        }

        private void unitConverterBtnClick(object sender, RoutedEventArgs e)
        {

        }

        private async void resizeImage(object sender, RoutedEventArgs e)
        {
            if (image != null)
            {
                image.Stretch = Stretch.Uniform;
                await Task.Delay(2);
                updateValue("image");
            }
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
                case "headingOffsetXPlus":
                    if (line != null)
                    {
                        if ((line.Tag as RouteData).offsetX < maxoffsetX) (line.Tag as RouteData).offsetX += 5;
                        for (int n = 0; n < drawCanvas.Children.Count; n++)
                        {
                            if (drawCanvas.Children[n] is Canvas)
                            {
                                if (((drawCanvas.Children[n] as Canvas).Children[0] as StackPanel).Tag is RouteData)
                                {

                                    if ((((drawCanvas.Children[n] as Canvas).Children[0] as StackPanel).Tag as RouteData).objID == (line.Tag as RouteData).objID && (((drawCanvas.Children[n] as Canvas).Children[0] as StackPanel).Tag as RouteData).componentID == (line.Tag as RouteData).componentID)
                                    {
                                        BindingOperations.GetMultiBindingExpression(((drawCanvas.Children[n] as Canvas).Children[0] as StackPanel).Children[0] as StackPanel, StackPanel.MarginProperty).UpdateTarget();
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "headingOffsetYPlus":
                    if (line != null)
                    {
                        if ((line.Tag as RouteData).offsetY > minoffsetY) (line.Tag as RouteData).offsetY -= 5;
                        for (int n = 0; n < drawCanvas.Children.Count; n++)
                        {
                            if (drawCanvas.Children[n] is Canvas)
                            {
                                if (((drawCanvas.Children[n] as Canvas).Children[0] as StackPanel).Tag is RouteData)
                                {

                                    if ((((drawCanvas.Children[n] as Canvas).Children[0] as StackPanel).Tag as RouteData).objID == (line.Tag as RouteData).objID && (((drawCanvas.Children[n] as Canvas).Children[0] as StackPanel).Tag as RouteData).componentID == (line.Tag as RouteData).componentID)
                                    {
                                        BindingOperations.GetMultiBindingExpression(((drawCanvas.Children[n] as Canvas).Children[0] as StackPanel).Children[0] as StackPanel, StackPanel.MarginProperty).UpdateTarget();
                                    }
                                }
                            }
                        }
                    }
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
                case "headingOffsetXMinus":
                    if (line != null)
                    {
                        if ((line.Tag as RouteData).offsetX > minoffsetX) (line.Tag as RouteData).offsetX -= 5;
                        for (int n = 0; n < drawCanvas.Children.Count; n++)
                        {
                            if (drawCanvas.Children[n] is Canvas)
                            {
                                if (((drawCanvas.Children[n] as Canvas).Children[0] as StackPanel).Tag is RouteData)
                                {

                                    if ((((drawCanvas.Children[n] as Canvas).Children[0] as StackPanel).Tag as RouteData).objID == (line.Tag as RouteData).objID && (((drawCanvas.Children[n] as Canvas).Children[0] as StackPanel).Tag as RouteData).componentID == (line.Tag as RouteData).componentID)
                                    {
                                        BindingOperations.GetMultiBindingExpression(((drawCanvas.Children[n] as Canvas).Children[0] as StackPanel).Children[0] as StackPanel, StackPanel.MarginProperty).UpdateTarget();
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "headingOffsetYMinus":
                    if (line != null)
                    {
                        if ((line.Tag as RouteData).offsetY < maxoffsetY) (line.Tag as RouteData).offsetY += 5;
                        for (int n = 0; n < drawCanvas.Children.Count; n++)
                        {
                            if (drawCanvas.Children[n] is Canvas)
                            {
                                if (((drawCanvas.Children[n] as Canvas).Children[0] as StackPanel).Tag is RouteData)
                                {

                                    if ((((drawCanvas.Children[n] as Canvas).Children[0] as StackPanel).Tag as RouteData).objID == (line.Tag as RouteData).objID && (((drawCanvas.Children[n] as Canvas).Children[0] as StackPanel).Tag as RouteData).componentID == (line.Tag as RouteData).componentID)
                                    {
                                        BindingOperations.GetMultiBindingExpression(((drawCanvas.Children[n] as Canvas).Children[0] as StackPanel).Children[0] as StackPanel, StackPanel.MarginProperty).UpdateTarget();
                                    }
                                }
                            }
                        }
                    }
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
                    if (mode == "route")
                    {
                        Ellipse bc = new Ellipse { Stroke = Brushes.Yellow, Fill = Brushes.DarkGray, StrokeThickness = 2 };
                        StackPanel fval = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Center };
                        fval.Children.Add(new Label { HorizontalContentAlignment = HorizontalAlignment.Center, Foreground = Brushes.Black, FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 5, 0, 0), BorderBrush = Brushes.Yellow, BorderThickness = new Thickness(0, 0, 0, 1) });
                        fval.Children.Add(new Label { HorizontalContentAlignment = HorizontalAlignment.Center, Foreground = Brushes.Red, FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5), BorderBrush = Brushes.Yellow, BorderThickness = new Thickness(0, 1, 0, 0) });
                        Grid fuelcircle = new Grid() { Background = Brushes.Transparent, Width = 120, Height = 120, Margin = new Thickness(0, 20, 0, 0) };
                        fuelcircle.ColumnDefinitions.Add(new ColumnDefinition());
                        fuelcircle.RowDefinitions.Add(new RowDefinition());
                        Grid.SetRow(bc, 0);
                        Grid.SetColumn(bc, 0);
                        Grid.SetRow(fval, 0);
                        Grid.SetColumn(fval, 0);
                        fuelcircle.Children.Add(bc);
                        fuelcircle.Children.Add(fval);
                        (fval.Children[0] as Label).SetBinding(Label.ContentProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.TagProperty), Converter = new RouteInfoConverter(), ConverterParameter = "rem2" });
                        (fval.Children[1] as Label).SetBinding(Label.ContentProperty, new Binding { Source = ellipse, Path = new PropertyPath(Ellipse.TagProperty), Converter = new RouteInfoConverter(), ConverterParameter = "frcs2" });
                        (routeDataPanel as StackPanel).Children.Insert(0, fuelcircle);
                        updateRouteDataInfo((ellipse.Tag as RouteData).objID);
                        lineProperties.Visibility = Visibility.Collapsed;
                        polygonProperties.Visibility = Visibility.Collapsed;
                        circleProperties.Visibility = Visibility.Collapsed;
                        imageProperties.Visibility = Visibility.Collapsed;
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
                    updateValue(mode);
                    set = false;
                    drawn = false;
                    mode = "none";
                    Gmap.ReleaseMouseCapture();
                    drawCanvas.Cursor = Cursors.Arrow;
                }
                else
                {
                    clearCache();
                }
            }
            if (e.Key == Key.Delete)
            {
                if (ellipse != null)
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
                        ellipse.Visibility = Visibility.Collapsed;
                        //num = (ellipse.Tag as RouteData).objID;
                        //for (int i = 0; i < drawCanvas.Children.Count; i++)
                        //{
                        //    if (drawCanvas.Children[i] is Ellipse)
                        //    {
                        //        if (((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objType == "Route" && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objID == num)
                        //        {
                        //            drawCanvas.Children.Remove(drawCanvas.Children[i]);
                        //            i--;
                        //        }
                        //    }
                        //    else if (drawCanvas.Children[i] is Line)
                        //    {
                        //        if (((drawCanvas.Children[i] as Line).Tag as RouteData).objType == "Route" && ((drawCanvas.Children[i] as Line).Tag as RouteData).objID == num)
                        //        {
                        //            drawCanvas.Children.Remove(drawCanvas.Children[i]);
                        //            i--;
                        //        }
                        //    }
                        //    else if (drawCanvas.Children[i] is Canvas)
                        //    {
                        //        if (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag is RouteData)
                        //        {

                        //            if ((((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).objType == "Route" && (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).objID == num)
                        //            {
                        //                drawCanvas.Children.Remove(drawCanvas.Children[i]);
                        //                i--;
                        //            }
                        //        }
                        //    }
                        //}
                        routeProperties.Visibility = Visibility.Collapsed;
                        clearCache();
                    }
                }
                else if (image != null)
                {
                    drawCanvas.Children.Remove(image);
                    imageProperties.Visibility = Visibility.Collapsed;
                    image = null;
                }
            }
        }

        private void checkpointSelected(object sender, SelectionChangedEventArgs e)
        {
            Canvas container = null;
            foreach (UIElement c in drawCanvas.Children)
            {
                if (c is Canvas)
                {
                    foreach (UIElement ch in (c as Canvas).Children)
                    {
                        if (ch is StackPanel)
                        {
                            if ((ch as StackPanel).Tag is RouteData)
                            {
                                if ((((ch as StackPanel).Tag as RouteData).objID == (line.Tag as RouteData).objID) && ((ch as StackPanel).Tag as RouteData).componentID == ((line.Tag as RouteData).componentID))
                                {
                                    container = c as Canvas;
                                }
                            }
                        }
                    }
                }
            }
            if ((sender as ListBox).SelectedIndex == -1)
            {
                current = null;
                return;
            }
            string item = (sender as ListBox).SelectedItem.ToString();
            int num = Int32.Parse(item.Substring(item.LastIndexOf('#') + 1));
            if (container != null)
            {
                foreach (UIElement checks in container.Children)
                {
                    if (checks is StackPanel)
                    {
                        if ((checks as StackPanel).Tag is CheckpointData)
                        {
                            if (!((checks as StackPanel).Tag as CheckpointData).isMark)
                            {
                                if (((checks as StackPanel).Tag as CheckpointData).checkpointnum == num) current = checks as StackPanel;
                            }
                        }
                    }
                }
            }
        }

        private void showCheckpointList(Line route)
        {
            if (checkpointssection.IsVisible)
            {
                routecheckpoints.SelectedIndex = -1;
                int n = 0;
                List<String> checkpoints = new List<string>();
                if (route == null)
                {
                    current = null;
                    routecheckpoints.ItemsSource = checkpoints;
                    routecheckpoints.Items.Refresh();
                    return;
                }
                Canvas container = null;
                foreach (UIElement c in drawCanvas.Children)
                {
                    if (c is Canvas)
                    {
                        foreach (UIElement ch in (c as Canvas).Children)
                        {
                            if (ch is StackPanel)
                            {
                                if ((ch as StackPanel).Tag is RouteData)
                                {
                                    if (((route.Tag as RouteData).objType == "Route" && ((ch as StackPanel).Tag as RouteData).objID == (route.Tag as RouteData).objID) && ((ch as StackPanel).Tag as RouteData).componentID == (route.Tag as RouteData).componentID)
                                    {
                                        container = c as Canvas;
                                    }
                                }
                            }
                        }
                    }
                }
                if (container != null)
                {
                    foreach (UIElement checks in container.Children)
                    {
                        if (checks is StackPanel)
                        {
                            if ((checks as StackPanel).Tag is CheckpointData)
                            {
                                if (!((checks as StackPanel).Tag as CheckpointData).isMark)
                                {
                                    ((checks as StackPanel).Tag as CheckpointData).checkpointnum = n;
                                    checkpoints.Add(String.Format($"Checkpoint # {n}"));
                                    n++;
                                }
                            }
                        }
                    }
                }
                routecheckpoints.ItemsSource = checkpoints;
                routecheckpoints.Items.Refresh();
            }
        }

        private Image loadImage()
        {
            Image img = new Image();
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image files(*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dlg.Title = "Please Choose an Image";
            bool? res = dlg.ShowDialog();
            if (res.Value)
            {
                try
                {
                    new BitmapImage(new Uri(dlg.FileName));
                }
                catch (Exception e)
                {
                    return null;
                }
                img.Source = (ImageSource)(new ImageSourceConverter().ConvertFromString(dlg.FileName));
                img.Stretch = Stretch.None;
                img.Opacity = 0.5;
                MultiBinding immargin = new MultiBinding() { Converter = new RectangleMarginConverter() };
                immargin.Bindings.Add(new Binding { Source = img, Path = new PropertyPath(Image.ActualWidthProperty) });
                immargin.Bindings.Add(new Binding { Source = img, Path = new PropertyPath(Image.ActualHeightProperty) });
                img.SetBinding(Image.MarginProperty, immargin);
                return img;
            }
            else
            {
                return null;
            }

        }

        private void flipRouteDataPanel(Canvas panel)
        {
            foreach (UIElement f in panel.Children)
            {
                if ((f as StackPanel).Tag is CheckpointData)
                {
                    ((f as StackPanel).RenderTransform as ScaleTransform).ScaleX *= -1;
                    (((f as StackPanel).Children[1] as Label).RenderTransform as ScaleTransform).ScaleX *= -1;
                }
                else
                {
                    ((f as StackPanel).RenderTransform as ScaleTransform).ScaleX *= -1;
                    (((f as StackPanel).Children[0] as StackPanel).RenderTransform as ScaleTransform).ScaleX *= -1;
                    if ((f as StackPanel).Tag is RouteData)
                    {
                        if (((f as StackPanel).Tag as RouteData).type == "Diversion")
                        {
                            return;
                        }
                        else
                        {
                            foreach (UIElement g in ((f as StackPanel).Children[0] as StackPanel).Children)
                            {
                                if (g is StackPanel)
                                {
                                    (((g as StackPanel).Children[0] as Path).RenderTransform as ScaleTransform).ScaleX *= -1;
                                    if ((((g as StackPanel).Children[0] as Path).RenderTransform as ScaleTransform).ScaleX < 0)
                                    {
                                        ((g as StackPanel).Children[1] as Label).BorderThickness = new Thickness(0, 0, 3, 3);
                                        ((g as StackPanel).Children[2] as Label).BorderThickness = new Thickness(0, 0, 3, 3);
                                        ((g as StackPanel).Children[3] as Label).BorderThickness = new Thickness(0, 0, 3, 3);
                                        ((g as StackPanel).Children[4] as Label).BorderThickness = new Thickness(0, 0, 3, 3);
                                        ((g as StackPanel).Children[1] as Label).HorizontalContentAlignment = HorizontalAlignment.Right;
                                        ((g as StackPanel).Children[2] as Label).HorizontalContentAlignment = HorizontalAlignment.Right;
                                        ((g as StackPanel).Children[3] as Label).HorizontalContentAlignment = HorizontalAlignment.Right;
                                        ((g as StackPanel).Children[4] as Label).HorizontalContentAlignment = HorizontalAlignment.Right;
                                    }
                                    else
                                    {
                                        ((g as StackPanel).Children[1] as Label).BorderThickness = new Thickness(3, 0, 0, 3);
                                        ((g as StackPanel).Children[2] as Label).BorderThickness = new Thickness(3, 0, 0, 3);
                                        ((g as StackPanel).Children[3] as Label).BorderThickness = new Thickness(3, 0, 0, 3);
                                        ((g as StackPanel).Children[4] as Label).BorderThickness = new Thickness(3, 0, 0, 3);
                                        ((g as StackPanel).Children[1] as Label).HorizontalContentAlignment = HorizontalAlignment.Left;
                                        ((g as StackPanel).Children[2] as Label).HorizontalContentAlignment = HorizontalAlignment.Left;
                                        ((g as StackPanel).Children[3] as Label).HorizontalContentAlignment = HorizontalAlignment.Left;
                                        ((g as StackPanel).Children[4] as Label).HorizontalContentAlignment = HorizontalAlignment.Left;
                                    }
                                }
                            }
                        }
                    }
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

        private async void setValue(object sender, KeyEventArgs e)
        {
            if (line != null || ellipse != null || image != null)
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
                        case "imageWidth":
                            if (String.IsNullOrEmpty(txtBx.Text) || !Double.TryParse(txtBx.Text, out value)) txtBx.Text = image.ActualWidth.ToString();
                            else
                            {
                                if (!imageKeepRatio.IsChecked.Value) image.Stretch = Stretch.Fill;
                                image.Width = value;
                                await Task.Delay(2);
                                updateValue("image");
                            }
                            break;
                        case "imageHeight":
                            if (String.IsNullOrEmpty(txtBx.Text) || !Double.TryParse(txtBx.Text, out value)) txtBx.Text = image.ActualHeight.ToString();
                            else
                            {
                                if (!imageKeepRatio.IsChecked.Value) image.Stretch = Stretch.Fill;
                                image.Height = value;
                                await Task.Delay(2);
                                updateValue("image");
                            }
                            break;
                        default:
                            return;
                    }
                    Keyboard.ClearFocus();
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
                    double[] dmsLat, dmsLng;
                    dmsLat = DataConverters.CoordinateUnits(dat.pos2.Lat, "DEGREE", "DMS");
                    dmsLng = DataConverters.CoordinateUnits(dat.pos2.Lng, "DEGREE", "DMS");
                    routeAcNameBox.Text = pdata.performanceDatas[0].aircraft;
                    msnNameBox.Text = dat.mission;
                    routeTotalDistanceBox.Text = Math.Round(DataConverters.LengthUnits(dat.totaldistance, baseunit.bdistUnit, dat.totaldistanceUnit), 3).ToString();
                    //routeTotalTimeBox.Value = TimeSpan.FromSeconds(dat.totaltime);
                    TimeSpan tot = TimeSpan.FromSeconds(dat.totaltime);
                    routeTotalTimeBox.Text = $"{(int)tot.TotalMinutes}'{tot.Seconds}\"";
                    routeTotalFuelBox.Text = Math.Round(dat.totalfuel, 3).ToString();
                    routeStartingFuelBox.Text = Math.Round(dat.startingfuel, 3).ToString();
                    routeMinimaFuelBox.Text = Math.Round(dat.minima, 3).ToString();
                    routeCoordBox.Text = String.Format($"{dmsLat[0]}°{dmsLat[1]}'{dmsLat[2]}\"N {dmsLng[0]}°{dmsLng[1]}'{dmsLng[2]}\"E");
                    routeNameBox.Text = dat.name;
                    routeTypeBox.Text = dat.type;
                    if (routeTypeBox.SelectedIndex == -1) routeTypeBox.IsEnabled = false;
                    else routeTypeBox.IsEnabled = true;
                    routeTrackBox.Text = Math.Round(dat.track, 2).ToString();
                    routeDistanceBox.Text = Math.Round(DataConverters.LengthUnits(dat.distance, baseunit.bdistUnit, dat.distanceUnit), 3).ToString();
                    routeAltBox.SelectedValue = dat.alt;
                    routeSpeedBox.SelectedValue = dat.speed;
                    //routeTimeBox.Value = TimeSpan.FromSeconds(dat.time);
                    TimeSpan t = TimeSpan.FromSeconds(dat.time);
                    routeTimeBox.Text = $"{(int)t.TotalMinutes}'{t.Seconds}\"";
                    routeFuelBox.Text = Math.Round(dat.fuel, 3).ToString();
                    routeRefuelDefuelBox.Text = Math.Round(dat.rdfuel, 3).ToString();
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
                case "image":
                    imageWidth.Text = ((int)image.ActualWidth).ToString();
                    imageHeight.Text = ((int)image.ActualHeight).ToString();
                    if (image.Stretch == Stretch.Uniform) imageKeepRatio.IsChecked = true;
                    else if (image.Stretch == Stretch.Fill) imageKeepRatio.IsChecked = false;
                    break;
                default:
                    return;
            }
        }

        private void removeElement(object sender, RoutedEventArgs e)
        {
            if (ellipse != null)
            {
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
                        ellipse.Visibility = Visibility.Collapsed;
                        //num = (ellipse.Tag as RouteData).objID;
                        //for (int i = 0; i < drawCanvas.Children.Count; i++)
                        //{
                        //    if (drawCanvas.Children[i] is Ellipse)
                        //    {
                        //        if (((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objType == "Route" && ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).objID == num)
                        //        {
                        //            drawCanvas.Children.Remove(drawCanvas.Children[i]);
                        //            i--;
                        //        }
                        //    }
                        //    else if (drawCanvas.Children[i] is Line)
                        //    {
                        //        if (((drawCanvas.Children[i] as Line).Tag as RouteData).objType == "Route" && ((drawCanvas.Children[i] as Line).Tag as RouteData).objID == num)
                        //        {
                        //            drawCanvas.Children.Remove(drawCanvas.Children[i]);
                        //            i--;
                        //        }
                        //    }
                        //    else if (drawCanvas.Children[i] is Canvas)
                        //    {
                        //        if (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag is RouteData)
                        //        {
                        //            if ((((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).objType == "Route" && (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).objID == num)
                        //            {
                        //                drawCanvas.Children.Remove(drawCanvas.Children[i]);
                        //                i--;
                        //            }
                        //        }
                        //    }
                        //}
                        routeProperties.Visibility = Visibility.Collapsed;
                        clearCache();
                        break;
                    default:
                        return;
                }
            }
            else if (image != null)
            {
                drawCanvas.Children.Remove(image);
                imageProperties.Visibility = Visibility.Collapsed;
                image = null;
            }
        }

        private void partsCleanup(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((sender as UIElement).Visibility == Visibility.Collapsed)
            {
                drawCanvas.Children.Remove(sender as UIElement);
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
                    if (datlink.objType == "Route" || datlink.objType == "Polygon")
                    {
                        GPoint gp = Gmap.FromLatLngToLocal(datlink.pos2);
                        Canvas.SetLeft(el, gp.X);
                        Canvas.SetTop(el, gp.Y);
                    }
                    else if (datlink.objType == "Line")
                    {
                        if (datlink.componentID == 0)
                        {
                            GPoint gp = Gmap.FromLatLngToLocal(datlink.pos1);
                            Canvas.SetLeft(el, gp.X);
                            Canvas.SetTop(el, gp.Y);
                        }
                        else if (datlink.componentID == 1)
                        {
                            GPoint gp = Gmap.FromLatLngToLocal(datlink.pos2);
                            Canvas.SetLeft(el, gp.X);
                            Canvas.SetTop(el, gp.Y);
                        }
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
                else if (tmp is Image)
                {
                    if ((tmp as Image).Tag is ImageData)
                    {
                        Image im = tmp as Image;
                        GPoint gp = Gmap.FromLatLngToLocal(((tmp as Image).Tag as ImageData).position);
                        Canvas.SetLeft(im, gp.X);
                        Canvas.SetTop(im, gp.Y);
                    }
                }
            }
        }

        private void loadAircraft(string name, bool reload = false)
        {
            Console.WriteLine("Came Here!");
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
                if (reload)
                {

                }
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

        private void updateAircraftRoute()
        {
            //Console.WriteLine("Came Here!");
            string done = "";
            for (int i = 0; i < drawCanvas.Children.Count; i++)
            {
                if (drawCanvas.Children[i] is Ellipse)
                {
                    if ((drawCanvas.Children[i] as Ellipse).Tag is RouteData)
                    {
                        if (((drawCanvas.Children[i] as Ellipse).Tag as RouteData).aircraft == done) continue;
                        else
                        {
                            done = ((drawCanvas.Children[i] as Ellipse).Tag as RouteData).aircraft;
                            loadAircraft(done, true);
                        }
                    }
                }
            }
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
                                    else if (drawCanvas.Children[i] is Canvas)
                                    {
                                        if (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag is RouteData)
                                        {
                                            if ((((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).objType == type && (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).objID == id)
                                            {
                                                (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).mission = msnNameBox.Text;
                                            }
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
                                    else if (drawCanvas.Children[i] is Canvas)
                                    {
                                        if (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag is RouteData)
                                        {
                                            if ((((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).objType == type && (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).objID == id)
                                            {
                                                (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).minima = Convert.ToDouble(routeMinimaFuelBox.Text);
                                            }
                                        }
                                    }
                                }
                                updateRouteDataInfo(id);
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
                                double val = Convert.ToDouble(routeRefuelDefuelBox.Text);
                                if (routeRefuelDefuelSelector.SelectedValue.ToString() == "Defuel" && val > 0) val *= -1;
                                (ellipse.Tag as RouteData).rdfuel = val;
                                if ((ellipse.Tag as RouteData).componentID != 0) (line.Tag as RouteData).rdfuel = val;
                                updateRouteDataInfo(id);
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
                                    else if (drawCanvas.Children[i] is Canvas)
                                    {
                                        if (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag is RouteData)
                                        {
                                            if ((((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).objType == type && (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).objID == id)
                                            {
                                                (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).startingfuel = Convert.ToDouble(routeStartingFuelBox.Text);
                                            }
                                        }
                                    }
                                }
                                updateRouteDataInfo(id);
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
                                        else if (drawCanvas.Children[i] is Canvas)
                                        {
                                            if (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag is RouteData)
                                            {
                                                if ((((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).objType == type && (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).objID == id)
                                                {
                                                    (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).mission = msnNameBox.Text;
                                                }
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
                                        else if (drawCanvas.Children[i] is Canvas)
                                        {
                                            if (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag is RouteData)
                                            {
                                                if ((((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).objType == type && (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).objID == id)
                                                {
                                                    (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).minima = Convert.ToDouble(routeMinimaFuelBox.Text);
                                                }
                                            }
                                        }
                                    }
                                    Keyboard.ClearFocus();
                                    updateRouteDataInfo(id);
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
                                        updateRouteDataInfo(id);
                                        break;
                                    }
                                    double val = 0;
                                    if (routeRefuelDefuelSelector.SelectedValue.ToString() == "Refuel") val = Convert.ToDouble(routeRefuelDefuelBox.Text);
                                    else if (routeRefuelDefuelSelector.SelectedValue.ToString() == "Defuel") val = Convert.ToDouble($"-{routeRefuelDefuelBox.Text}");
                                    (ellipse.Tag as RouteData).rdfuel = val;
                                    if ((ellipse.Tag as RouteData).componentID != 0) (line.Tag as RouteData).rdfuel = val;
                                    Keyboard.ClearFocus();
                                    updateRouteDataInfo(id);
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
                                        else if (drawCanvas.Children[i] is Canvas)
                                        {
                                            if (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag is RouteData)
                                            {
                                                if ((((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).objType == type && (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).objID == id)
                                                {
                                                    (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).startingfuel = Convert.ToDouble(routeStartingFuelBox.Text);
                                                }
                                            }
                                        }
                                    }
                                    Keyboard.ClearFocus();
                                    updateRouteDataInfo(id);
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
                                else if (drawCanvas.Children[i] is Canvas)
                                {
                                    if (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag is RouteData)
                                    {
                                        if ((((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).objType == type && (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).objID == id)
                                        {
                                            (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).totaldistanceUnit = routeTotalDistanceUnit.SelectedValue.ToString();
                                        }
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
                                else if (drawCanvas.Children[i] is Canvas)
                                {
                                    if (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag is RouteData)
                                    {
                                        if ((((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).objType == type && (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).objID == id)
                                        {
                                            (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).distanceUnit = routeDistanceUnit.SelectedValue.ToString();
                                        }
                                    }
                                }
                            }
                            break;
                        case "routeTypeBox":
                            if (routeTypeBox.SelectedIndex == -1) return;
                            dat.type = routeTypeBox.SelectedValue.ToString();
                            if (dat.componentID != 0) (line.Tag as RouteData).type = routeTypeBox.SelectedValue.ToString();
                            updateValue("route");
                            updateRouteDataInfo(dat.objID);
                            break;
                        case "routeAltBox":
                        case "routeSpeedBox":
                            if ((sender as ComboBox).SelectedIndex == -1) return;
                            dat.setpData(pdata, routeAltBox.SelectedIndex, Convert.ToDouble(routeSpeedBox.SelectedValue));
                            if (dat.componentID != 0) (line.Tag as RouteData).setpData(pdata, routeAltBox.SelectedIndex, Convert.ToDouble(routeSpeedBox.SelectedValue));
                            updateValue("route");
                            updateRouteDataInfo(dat.objID);
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
                                else if (drawCanvas.Children[i] is Canvas)
                                {
                                    if (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag is RouteData)
                                    {
                                        if ((((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).objType == type && (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).objID == id)
                                        {
                                            (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).startingfuel = Convert.ToDouble(routeStartingFuelBox.SelectedValue.ToString());
                                        }
                                    }
                                }
                            }
                            updateRouteDataInfo(id);
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

        private void updateRouteDataInfo(int oid)
        {
            for (int i = 0; i < drawCanvas.Children.Count; i++)
            {
                if (drawCanvas.Children[i] is Canvas)
                {
                    if (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag is RouteData)
                    {
                        if ((((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).objType == "Route" && (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).objID == oid)
                        {
                            if ((((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Tag as RouteData).type == "Diversion")
                            {
                                StackPanel div = ((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Children[0] as StackPanel;
                                (div.Children[0] as Label).GetBindingExpression(Label.ContentProperty).UpdateTarget();
                                (div.Children[1] as Label).GetBindingExpression(Label.ContentProperty).UpdateTarget();
                            }
                            else
                            {
                                foreach (UIElement dtpnl in (((drawCanvas.Children[i] as Canvas).Children[0] as StackPanel).Children[0] as StackPanel).Children)
                                {
                                    if (dtpnl is StackPanel)
                                    {
                                        ((dtpnl as StackPanel).Children[1] as Label).GetBindingExpression(Label.ContentProperty).UpdateTarget();
                                        ((dtpnl as StackPanel).Children[2] as Label).GetBindingExpression(Label.ContentProperty).UpdateTarget();
                                        ((dtpnl as StackPanel).Children[3] as Label).GetBindingExpression(Label.ContentProperty).UpdateTarget();
                                        ((dtpnl as StackPanel).Children[4] as Label).GetBindingExpression(Label.ContentProperty).UpdateTarget();
                                    }
                                    if (dtpnl is Grid)
                                    {
                                        (((dtpnl as Grid).Children[1] as StackPanel).Children[0] as Label).GetBindingExpression(Label.ContentProperty).UpdateTarget();
                                        (((dtpnl as Grid).Children[1] as StackPanel).Children[1] as Label).GetBindingExpression(Label.ContentProperty).UpdateTarget();
                                    }
                                }
                            }
                        }
                    }
                    foreach (UIElement elm in (drawCanvas.Children[i] as Canvas).Children)
                    {
                        if (elm is StackPanel)
                        {
                            if ((elm as StackPanel).Tag is CheckpointData)
                            {
                                if (((elm as StackPanel).Tag as CheckpointData).isMark) ((elm as StackPanel).Tag as CheckpointData).updateData(((elm as StackPanel).Children[1] as Label).Content.ToString());
                                BindingExpression be = ((elm as StackPanel).Children[1] as Label).GetBindingExpression(Label.ContentProperty);
                                MultiBindingExpression bf = BindingOperations.GetMultiBindingExpression(elm, Canvas.BottomProperty);
                                MultiBindingExpression bg = BindingOperations.GetMultiBindingExpression(elm, Canvas.TopProperty);
                                if (be != null) be.UpdateTarget();
                                if (bf != null) bf.UpdateTarget();
                                if (bg != null) bg.UpdateTarget();
                            }
                        }
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

        private void clearCache()
        {
            line = null;
            ellipse = null;
            image = null;
            routeDataPanel = null;
            routeProperties.Visibility = Visibility.Collapsed;
            routeLineBlock.Visibility = Visibility.Collapsed;
            circleProperties.Visibility = Visibility.Collapsed;
            lineProperties.Visibility = Visibility.Collapsed;
            polygonProperties.Visibility = Visibility.Collapsed;
            imageProperties.Visibility = Visibility.Collapsed;
        }
    }
}
