using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MissionAssistant
{
    public static class Tester
    {
        static MapLine line1;
        static HeadingBox heading1;
        static Polygon poly1;
        static PolygonLine ln2;
        static FuelCircle fc;
        static Checkpoint ck;
        static RouteLeg rl;
        static Checkpoint cp;
        static Circle cl;
        static Route r;
        public static void TestBtn1()
        {
            //StackPanel test = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Center };
            //((MainWindow)Application.Current.MainWindow).drawCanvas.Children.Add(test);
            //Canvas.SetLeft(test, 250);
            //Canvas.SetTop(test, 250);
            //fc = new FuelCircle();
            //fc.Draw(test, 0);
            //line1 = new MapLine();
            //poly1 = new Polygon();
            //var ln1 = new PolygonLine() { StartPoint = new GMap.NET.PointLatLng(22, 89), EndPoint = new GMap.NET.PointLatLng(23, 90) };
            //ln2 = new PolygonLine() { StartPoint = new GMap.NET.PointLatLng(23, 90), EndPoint = new GMap.NET.PointLatLng(23.5, 90.5) };
            //poly1.Lines.Add(ln1);
            //poly1.Lines.Add(new PolygonLine() { StartPoint = new GMap.NET.PointLatLng(22, 89), EndPoint = new GMap.NET.PointLatLng(22.5, 89.5) });
            //ck = new Checkpoint();
            //ln2 = new PolygonLine(null);
            //r = new Route { Aircraft = "Mig-29"};

            MainWindow wnd = Application.Current.MainWindow as MainWindow;

            Console.WriteLine(wnd.drawCanvas.Children.Contains(wnd.bafChart));

            Console.WriteLine(Canvas.GetLeft(wnd.bafChart));
            Console.WriteLine(Canvas.GetTop(wnd.bafChart));
        }

        public static void TestBtn2()
        {
            //fc.RemainingFuel = 1200;
            //fc.SuggestedFuel = 5000;
            //poly1.Draw(((MainWindow)Application.Current.MainWindow).drawCanvas);
            //line1.StartPoint = new GMap.NET.PointLatLng(23, 87);
            //line1.EndPoint = new GMap.NET.PointLatLng(25, 90);
            //ck.Distance = 10;
            //rl.Draw(((MainWindow)Application.Current.MainWindow).drawCanvas);
            //rl.StartPoint = new GMap.NET.PointLatLng(23, 87);
            //rl.EndPoint = new GMap.NET.PointLatLng(25, 90);
            rl = new RouteLeg(r) { LocalStartPoint = new Point(300, 300), LocalEndPoint = new Point(600, 400) };
            r.Legs.Add(rl);
        }

        public static void TestBtn3()
        {
            var wnd = ((MainWindow)Application.Current.MainWindow);
            r.Draw(wnd.drawCanvas);
            //cl = new Circle { LocalCenter = new Point(700, 500), Radius = 100 };
            //cl.Draw(wnd.drawCanvas);
            //rl.Type = RouteLegType.Enroute;
            //poly1.Color = Brushes.Black;
            //poly1.Thickness = 3;
            //poly1.NodeRadius = 5;
            //line1.EndPoint = new GMap.NET.PointLatLng(25, 90);
            //poly1.Lines[0].EndPoint = new GMap.NET.PointLatLng(24, 92);
            //Canvas cn = new Canvas() { Width = 200, Height = 800, Background = Brushes.Blue };
            //Canvas.SetLeft(cn, 500);
            //Canvas.SetTop(cn, 400);
            //((MainWindow)Application.Current.MainWindow).drawCanvas.Children.Add(cn);
            //ck.Draw(cn);
            //Ellipse el = new Ellipse() { Stroke = Brushes.Red, Fill = Brushes.Red, Width = 10, Height = 10, Margin = new Thickness(-5) };

        }

        public static void TestBtn4()
        {
            UI.RepositionPoints();
            //cl.Radius = 200;
            //rl.Type = RouteLegType.Landing;
            var wnd = ((MainWindow)Application.Current.MainWindow);
            //ck.IsFlipped = !ck.IsFlipped;
            //poly1.Lines.Add(ln2);
            //line1.Draw(((MainWindow)Application.Current.MainWindow).drawCanvas);
        }

        public static void TestBtn5()
        {
            string lat = DataFormatter.FormatGeo(23.777176, GeoFormat.DMS);
            Console.WriteLine();
            //Console.WriteLine(cl.Center);
            //Console.WriteLine(cl.Radius);
            //Console.WriteLine(rl.Distance);
            //Console.WriteLine(DataCalculations.GetDistance(DataCalculations.GetLatLngFromPhysical(new Point(400, 400)), DataCalculations.GetLatLngFromPhysical(new Point(400, 401))));
            //Console.WriteLine(poly1.Lines[0].Distance);
            //Console.WriteLine(poly1.Lines[1].Distance);
            //Console.WriteLine(poly1.TotalDistance);
            //Console.WriteLine(line1.Distance);
            //Console.WriteLine(line1.Track);
            //Console.WriteLine(line1.StartPoint);
            //Console.WriteLine(line1.EndPoint);

            //Console.WriteLine(((RouteLeg)UI.selectedObject).Markers[0].Distance);
            //Console.WriteLine(((RouteLeg)UI.selectedObject).Markers[0].LocalDistance);
            //Console.WriteLine(((RouteLeg)UI.selectedObject).Markers[0].IsVisible);
            //Console.WriteLine(((RouteLeg)UI.selectedObject).Markers[1].Distance);
            //Console.WriteLine(((RouteLeg)UI.selectedObject).Markers[1].LocalDistance);
            //Console.WriteLine(((RouteLeg)UI.selectedObject).Markers[1].IsVisible);
        }
    }
}
