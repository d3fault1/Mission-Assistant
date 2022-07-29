using GMap.NET;
using GMap.NET.WindowsPresentation;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MissionAssistant
{
    static class DrawingEngine
    {
        private static Ellipse dummyEllipse;
        private static Line dummyLine;
        private static bool set = false;
        private static object drawParent, drawObject;
        private static Canvas drawCanvas;
        private static GMapControl gMap;
        private static string drawMode = "none";

        public static bool isLiveDrawing { get { return isDummyDrawn(); } }

        public delegate void DrawingFinishedEventHandler(object sender, DrawingFinishedEventArgs e);
        public static event DrawingFinishedEventHandler DrawingFinished;

        public static void OnMouseHold(Point pos, int button)
        {
            
        }

        public static void OnMouseRelease(Point pos, int button)
        {
            if (button == 1) if (set && !gMap.IsMouseCaptured) gMap.CaptureMouse();
        }

        public static void OnMouseRightClick(Point pos)
        {
            CancelDrawing();
        }

        public static void OnMouseLeftClick(Point pos)
        {
            switch (drawMode)
            {
                case "line":
                    if (set)
                    {
                        drawObject = new MapLine() { LocalStartPoint = getDummyPos(), LocalEndPoint = pos };
                        ((MapLine)drawObject).Draw(drawCanvas);
                        set = false;
                        UI.drawnObjects.Add(drawObject);
                        OnDrawingFinished();
                    }
                    else
                    {
                        DrawDummy(pos);
                        gMap.CaptureMouse();
                        set = true;
                    }
                    break;
                case "polygon":
                    if (set)
                    {
                        drawObject = new PolygonLine((Polygon)drawParent) { LocalStartPoint = getDummyPos(), LocalEndPoint = pos };
                        UndrawDummy();
                        ((Polygon)drawParent).Lines.Add((PolygonLine)drawObject);
                        ((Polygon)drawParent).Draw(drawCanvas);

                        UI.selectedObject = drawObject;

                        DrawDummy(pos);
                    }
                    else
                    {
                        DrawDummy(pos);
                        gMap.CaptureMouse();
                        set = true;
                    }
                    break;
                case "route":
                    if (set)
                    {
                        drawObject = new RouteLeg((Route)drawParent) { LocalStartPoint = getDummyPos(), LocalEndPoint = pos };
                        UndrawDummy();
                        ((Route)drawParent).Legs.Add((RouteLeg)drawObject);
                        ((Route)drawParent).Draw(drawCanvas);

                        UI.selectedObject = drawObject;

                        DrawDummy(pos);
                    }
                    else
                    {
                        DrawDummy(pos);
                        gMap.CaptureMouse();
                        set = true;
                    }
                    break;
                case "circle":
                    drawObject = new Circle { LocalCenter = pos, Radius = 25 };
                    ((Circle)drawObject).Draw(drawCanvas);
                    UI.drawnObjects.Add(drawObject);
                    OnDrawingFinished();
                    break;
                case "diversion":
                    if (UI.selectedObject is RouteLeg)
                    {
                        var leg = (RouteLeg)UI.selectedObject;
                        var main = leg.Parent;
                        drawObject = new DiversionLeg(main) { LinkingLegIndex = main.Legs.IndexOf(leg), LinkingNode = UI.objecthitpoint, LocalEndPoint = pos };
                        main.DiversionLegs.Add((DiversionLeg)drawObject);
                        main.Draw(drawCanvas);
                        OnDrawingFinished();
                    }
                    break;
                case "checkpoint":
                    Point p;
                    var selected = UI.selectedObject;
                    var obj = (MapLine)selected;
                    if (drawCanvas.InputHitTest(pos) is Line)
                    {
                        if (obj == ((object[])((Line)drawCanvas.InputHitTest(pos)).Tag)[0])
                        {
                            p = pos;
                        }
                        else p = new Point(-1, -1);
                    }
                    else
                    {
                        double xpoint = -1, ypoint = -1, xdist = 0, ydist = 0;
                        if (obj.LocalStartPoint.X < obj.LocalEndPoint.X)
                        {
                            if (pos.X > obj.LocalStartPoint.X && pos.X < obj.LocalEndPoint.X)
                            {
                                ypoint = obj.LocalStartPoint.Y + (((pos.X - obj.LocalStartPoint.X) * (obj.LocalStartPoint.Y - obj.LocalEndPoint.Y)) / (obj.LocalStartPoint.X - obj.LocalEndPoint.X));
                                ydist = Math.Abs(ypoint - pos.Y);
                            }
                        }
                        else if (obj.LocalStartPoint.X > obj.LocalEndPoint.X)
                        {
                            if (pos.X > obj.LocalEndPoint.X && pos.X < obj.LocalStartPoint.X)
                            {
                                ypoint = obj.LocalStartPoint.Y + (((pos.X - obj.LocalStartPoint.X) * (obj.LocalStartPoint.Y - obj.LocalEndPoint.Y)) / (obj.LocalStartPoint.X - obj.LocalEndPoint.X));
                                ydist = Math.Abs(ypoint - pos.Y);
                            }
                        }
                        if (obj.LocalStartPoint.Y < obj.LocalEndPoint.Y)
                        {
                            if (pos.Y > obj.LocalStartPoint.Y && pos.Y < obj.LocalEndPoint.Y)
                            {
                                xpoint = obj.LocalStartPoint.X + (((pos.Y - obj.LocalStartPoint.Y) * (obj.LocalStartPoint.X - obj.LocalEndPoint.X)) / (obj.LocalStartPoint.Y - obj.LocalEndPoint.Y));
                                xdist = Math.Abs(xpoint - pos.X);
                            }
                        }
                        else if (obj.LocalStartPoint.Y > obj.LocalEndPoint.Y)
                        {
                            if (pos.Y > obj.LocalEndPoint.Y && pos.Y < obj.LocalStartPoint.Y)
                            {
                                xpoint = obj.LocalStartPoint.X + (((pos.Y - obj.LocalStartPoint.Y) * (obj.LocalStartPoint.X - obj.LocalEndPoint.X)) / (obj.LocalStartPoint.Y - obj.LocalEndPoint.Y));
                                xdist = Math.Abs(xpoint - pos.X);
                            }
                        }
                        if (xpoint == -1 && ypoint == -1) p = new Point(-1, -1);
                        else if (xpoint == -1 && ypoint != -1) p = new Point(pos.X, ypoint);
                        else if (xpoint != -1 && ypoint == -1) p = new Point(xpoint, pos.Y);
                        else if (xpoint != -1 && ypoint != -1 && xdist <= ydist) p = new Point(xpoint, pos.Y);
                        else if (xpoint != -1 && ypoint != -1 && xdist > ydist) p = new Point(pos.X, ypoint);
                        else p = new Point(-1, -1);
                    }
                    if (p == new Point(-1, -1)) return;
                    else
                    {
                        var distance = DataCalculations.GetDistance(obj.StartPoint, DataCalculations.GetLatLngFromPhysical(p));
                        if (selected is RouteLeg)
                        {
                            Checkpoint ck = new Checkpoint((RouteLeg)obj) { Distance = distance };
                            ((RouteLeg)obj).Checkpoints.Add(ck);
                        }

                        //Needs Work
                        //else if (selected is DiversionLeg)
                        //{
                        //    Checkpoint ck = new Checkpoint((DiversionLeg)selected) { Distance = distance };
                        //    ((DiversionLeg)selected).Checkpoints.Add(ck);
                        //}
                    }
                    break;
            }
        }

        public static void OnMouseMove(Point pos, int hold)
        {
            MoveDummy(pos);
        }

        public static void OnMouseEnter()
        {
            if (drawMode == "diversion")
            {
                Point pos;
                if (UI.objecthitpoint == 2) pos = ((RouteLeg)UI.selectedObject).LocalEndPoint;
                else pos = ((RouteLeg)UI.selectedObject).LocalStartPoint;
                DrawDummy(pos);
            }
        }

        public static void OnMouseLeave()
        {
            if (drawMode == "diversion")
            {
                UndrawDummy();
            }
        }

        public static void OnMapDrag()
        {
            UpdateDummy();
        }

        public static void OnMapZoomChanged()
        {
            UpdateDummy();
        }

        public static void KeyControls(string key)
        {
            switch (key)
            {
                case "escape":
                    CancelDrawing();
                    break;
            }
        }

        private static void DrawDummy(Point pos)
        {
            if (!isDummyDrawn())
            {
                switch (drawMode)
                {
                    case "line":
                    case "polygon":
                        dummyEllipse = new Ellipse
                        {
                            Tag = DataCalculations.GetLatLngFromPhysical(pos),
                            Width = 10,
                            Height = 10,
                            Stroke = Brushes.White,
                            Fill = Brushes.White,
                            StrokeThickness = 2,
                            Margin = new Thickness(-5)
                        };
                        Canvas.SetLeft(dummyEllipse, pos.X);
                        Canvas.SetTop(dummyEllipse, pos.Y);
                        dummyLine = new Line
                        {
                            Stroke = Brushes.White,
                            StrokeThickness = 2,
                            X2 = pos.X,
                            Y2 = pos.Y
                        };
                        dummyLine.SetBinding(Line.X1Property, new Binding { Source = dummyEllipse, Path = new PropertyPath(Canvas.LeftProperty) });
                        dummyLine.SetBinding(Line.Y1Property, new Binding { Source = dummyEllipse, Path = new PropertyPath(Canvas.TopProperty) });
                        drawCanvas.Children.Add(dummyEllipse);
                        drawCanvas.Children.Add(dummyLine);
                        break;
                    case "route":
                        dummyEllipse = new Ellipse
                        {
                            Tag = DataCalculations.GetLatLngFromPhysical(pos),
                            Width = 20,
                            Height = 20,
                            Stroke = Brushes.White,
                            StrokeThickness = 2,
                            Margin = new Thickness(-10)
                        };
                        Canvas.SetLeft(dummyEllipse, pos.X);
                        Canvas.SetTop(dummyEllipse, pos.Y);
                        dummyLine = new Line
                        {
                            Stroke = Brushes.White,
                            StrokeThickness = 2,
                            X2 = pos.X,
                            Y2 = pos.Y
                        };
                        dummyLine.SetBinding(Line.X1Property, new Binding { Source = dummyEllipse, Path = new PropertyPath(Canvas.LeftProperty) });
                        dummyLine.SetBinding(Line.Y1Property, new Binding { Source = dummyEllipse, Path = new PropertyPath(Canvas.TopProperty) });
                        drawCanvas.Children.Add(dummyEllipse);
                        drawCanvas.Children.Add(dummyLine);
                        break;
                    case "diversion":
                        dummyEllipse = new Ellipse
                        {
                            Tag = DataCalculations.GetLatLngFromPhysical(pos),
                            Width = 20,
                            Height = 20,
                            Stroke = Brushes.Red,
                            StrokeThickness = 2,
                            Margin = new Thickness(-10)
                        };
                        Canvas.SetLeft(dummyEllipse, pos.X);
                        Canvas.SetTop(dummyEllipse, pos.Y);
                        dummyLine = new Line
                        {
                            Stroke = Brushes.Red,
                            StrokeThickness = 2,
                            StrokeDashArray = new DoubleCollection { 3, 2 },
                            X2 = pos.X,
                            Y2 = pos.Y
                        };
                        dummyLine.SetBinding(Line.X1Property, new Binding { Source = dummyEllipse, Path = new PropertyPath(Canvas.LeftProperty) });
                        dummyLine.SetBinding(Line.Y1Property, new Binding { Source = dummyEllipse, Path = new PropertyPath(Canvas.TopProperty) });
                        drawCanvas.Children.Add(dummyEllipse);
                        drawCanvas.Children.Add(dummyLine);
                        break;
                    case "circle":
                        break;
                }
            }
        }

        private static void UpdateDummy()
        {
            if (isDummyDrawn())
            {
                var pos = (PointLatLng)dummyEllipse.Tag;
                var pnt = DataCalculations.GetPhysicalFromLatLng(pos);
                Canvas.SetLeft(dummyEllipse, pnt.X);
                Canvas.SetTop(dummyEllipse, pnt.Y);
            }
        }

        private static void MoveDummy(Point pos)
        {
            if (isDummyDrawn())
            {
                dummyLine.X2 = pos.X;
                dummyLine.Y2 = pos.Y;
            }
        }

        private static void UndrawDummy()
        {
            if (isDummyDrawn())
            {
                drawCanvas.Children.Remove(dummyEllipse);
                drawCanvas.Children.Remove(dummyLine);
            }
        }

        private static bool isDummyDrawn()
        {
            return drawCanvas.Children.Contains(dummyEllipse) && drawCanvas.Children.Contains(dummyLine);
        }

        private static Point getDummyPos()
        {
            if (isDummyDrawn()) return new Point(dummyLine.X1, dummyLine.Y1);
            return new Point(-1, -1);
        }

        public static double getLiveDistance()
        {
            var pos1 = DataCalculations.GetLatLngFromPhysical(new Point(dummyLine.X1, dummyLine.Y1));
            var pos2 = DataCalculations.GetLatLngFromPhysical(new Point(dummyLine.X2, dummyLine.Y2));
            return DataCalculations.GetDistance(pos1, pos2);
        }

        public static double getLiveHeading()
        {
            var pos1 = DataCalculations.GetLatLngFromPhysical(new Point(dummyLine.X1, dummyLine.Y1));
            var pos2 = DataCalculations.GetLatLngFromPhysical(new Point(dummyLine.X2, dummyLine.Y2));
            return DataCalculations.GetTrack(pos1, pos2);
        }

        private static void CancelDrawing()
        {
            switch (drawMode)
            {
                case "line":
                    break;
                case "polygon":
                    if (((Polygon)drawParent).Lines.Count > 0) UI.drawnObjects.Add(drawParent);
                    set = false;
                    break;
                case "route":
                    if (((Route)drawParent).Legs.Count > 0) UI.drawnObjects.Add(drawParent);
                    set = false;
                    break;
                case "diversion":
                    break;
                case "circle":
                    break;
                case "checkpoint":
                    break;
            }
            OnDrawingFinished();
        }

        public static void ModeSelect(string mode, string param = "")
        {
            set = false;
            drawObject = null;
            drawParent = null;
            switch (mode)
            {
                case "line":
                    drawMode = "line";
                    break;
                case "polygon":
                    drawParent = new Polygon();
                    drawMode = "polygon";
                    break;
                case "route":
                    drawParent = new Route() { Aircraft = param };
                    drawMode = "route";
                    break;
                case "diversion":
                    drawMode = "diversion";
                    break;
                case "circle":
                    drawMode = "circle";
                    break;
                case "checkpoint":
                    drawMode = "checkpoint";
                    break;
            }
        }

        public static void Init(Canvas canvas, GMapControl map)
        {
            drawCanvas = canvas;
            gMap = map;
        }

        private static void OnDrawingFinished()
        {
            UndrawDummy();
            gMap.ReleaseMouseCapture();
            drawMode = "none";
            DrawingFinishedEventArgs eventArgs = new DrawingFinishedEventArgs(drawObject);
            DrawingFinished?.Invoke(null, eventArgs);
        }
    }

    public struct DrawingFinishedEventArgs
    {
        public DrawingFinishedEventArgs(object drawnobject)
        {
            DrawnObject = drawnobject;
        }
        public object DrawnObject { get; }
    }
}
