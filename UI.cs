using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Specialized;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Windows.Data;

namespace MissionAssistant
{
    static class UI
    {
        public static ObservableCollection<object> drawnObjects = new ObservableCollection<object>();
        public static object selectedObject
        {
            get
            {
                return _selectedObject;
            }
            set
            {
                if (value == null)
                {
                    OnObjectDeselect();
                    _selectedObject = value;
                }
                else
                {
                    _selectedObject = value;
                    OnObjectSelect();
                }
            }
        }

        public static MainWindow GUI;

        public static int objecthitpoint;

        private static object _selectedObject = null;

        private static Point clickedPoint, currentPoint;

        public static Canvas drawCanvas;
        public static GMapControl map;

        private static UIElement tempObject = null;

        private static Line xline;
        private static Line yline;

        private static string mode = "none";

        private const int minoffsetX = 30, maxoffsetX = 150, minoffsetY = -50, maxoffsetY = 50;
        private const double minThickness = 0.5, maxThickness = 30, minRadius = 1, maxRadius = 2000;

        private static double top, left;

        private static double mapZoom
        {
            get
            {
                if (map != null) return map.Zoom;
                else return 0;
            }
            set
            {
                map.Zoom = value;
                OnMapZoomChanged();
            }
        }

        public static void Init(MainWindow window)
        {
            GUI = window;
            drawCanvas = window.drawCanvas;
            map = window.Gmap;
            drawnObjects.CollectionChanged += DrawnObjectsChanged;
            DrawingEngine.Init(drawCanvas, map);
            DrawingEngine.DrawingFinished += OnDrawingFinished;
            xline = new Line { Stroke = Brushes.Gray, StrokeThickness = 2, X1 = 0, IsHitTestVisible = false };
            yline = new Line { Stroke = Brushes.Gray, StrokeThickness = 2, Y1 = 0, IsHitTestVisible = false };
            xline.SetBinding(Line.X2Property, new Binding { Source = drawCanvas, Path = new PropertyPath(Canvas.ActualWidthProperty) });
            yline.SetBinding(Line.Y2Property, new Binding { Source = drawCanvas, Path = new PropertyPath(Canvas.ActualHeightProperty) });
        }

        public static void OnMapDrag()
        {
            if (mode != "none") DrawingEngine.OnMapDrag();
            RepositionMaps();
            RepositionPoints();
        }

        private static void OnMapZoomChanged()
        {
            if (mode != "none") DrawingEngine.OnMapZoomChanged();
            RepositionMaps();
            RepositionPoints();
            (GUI.bafChart.RenderTransform as ScaleTransform).ScaleX = 0.00901953125 * Math.Pow(2, map.Zoom);
            (GUI.bafChart.RenderTransform as ScaleTransform).ScaleY = 0.00909375 * Math.Pow(2, map.Zoom);
        }

        public static void OnMouseHold(Point pos, bool isLeft)
        {
            clickedPoint = pos;
            currentPoint = clickedPoint;

            if (mode != "none")
            {
                if (isLeft) DrawingEngine.OnMouseHold(pos, 1);
                else DrawingEngine.OnMouseHold(pos, 2);
            }

            if (isLeft)
            {
                if (mode == "none")
                {
                    tempObject = (UIElement)drawCanvas.InputHitTest(pos);
                    if (tempObject == map)
                    {
                        tempObject = null;
                    }
                    else
                    {
                        left = Canvas.GetLeft(tempObject);
                        top = Canvas.GetTop(tempObject);

                        //Experimental
                        if (!(tempObject is Image))
                        {
                            var info = (object[])((FrameworkElement)tempObject).Tag;

                            if (info[0].GetType() == typeof(MapLine)) ((MapLine)info[0]).RestrictPositionUpdate = true;
                            else if (info[0].GetType() == typeof(Circle)) ((Circle)info[0]).RestrictPositionUpdate = true;
                            else if (info[0].GetType() == typeof(PolygonLine)) ((PolygonLine)info[0]).Parent.RestrictPositionUpdate = true;
                            else if (info[0].GetType() == typeof(RouteLeg)) ((RouteLeg)info[0]).Parent.RestrictPositionUpdate = true;
                            else if (info[0].GetType() == typeof(DiversionLeg)) ((DiversionLeg)info[0]).Parent.RestrictPositionUpdate = true;
                        }
                    }
                }
            }
        }

        public static void OnMouseRelease(Point pos, bool isLeft)
        {
            currentPoint = pos;

            if (mode != "none")
            {
                if (isLeft) DrawingEngine.OnMouseRelease(pos, 1);
                else DrawingEngine.OnMouseRelease(pos, 2);
            }

            if (clickedPoint == currentPoint)
            {
                if (mode != "none")
                {
                    if (isLeft)
                    {
                        DrawingEngine.OnMouseLeftClick(pos);
                        //Needs Work
                        if (mode == "image")
                        {
                            mode = "none";
                            Canvas.SetLeft(tempObject, clickedPoint.X);
                            Canvas.SetTop(tempObject, clickedPoint.Y);
                            tempObject.Opacity = 1;
                            ((Image)tempObject).Stretch = Stretch.Fill;
                            ((Image)tempObject).Tag = pos;
                            Panel.SetZIndex(tempObject, 1);
                            drawCanvas.Cursor = Cursors.Arrow;
                        }
                    }
                    else
                    {
                        DrawingEngine.OnMouseRightClick(pos);
                        //Needs Work
                        if (mode == "image")
                        {
                            if (tempObject is Image)
                            {
                                if (drawCanvas.Children.Contains(tempObject))
                                {
                                    drawCanvas.Children.Remove(tempObject);
                                    mode = "none";
                                    drawCanvas.Cursor = Cursors.Arrow;
                                }
                            }
                        }
                    }
                    return;
                }
            }
            if (isLeft)
            {
                if (tempObject != null)
                {
                    //Needs Work
                    if (tempObject is Image)
                    {
                        selectedObject = null;
                        selectedObject = tempObject;
                    }
                    else
                    {
                        var info = (object[])((FrameworkElement)tempObject).Tag;

                        //Experimental
                        if (info[0].GetType() == typeof(MapLine)) ((MapLine)info[0]).RestrictPositionUpdate = false;
                        else if (info[0].GetType() == typeof(Circle)) ((Circle)info[0]).RestrictPositionUpdate = false;
                        else if (info[0].GetType() == typeof(PolygonLine)) ((PolygonLine)info[0]).Parent.RestrictPositionUpdate = false;
                        else if (info[0].GetType() == typeof(RouteLeg)) ((RouteLeg)info[0]).Parent.RestrictPositionUpdate = false;
                        else if (info[0].GetType() == typeof(DiversionLeg)) ((DiversionLeg)info[0]).Parent.RestrictPositionUpdate = false;

                        selectedObject = null;
                        selectedObject = info[0];
                        objecthitpoint = (int)info[1];
                    }
                }
                else
                {
                    selectedObject = null;
                    PanelControl(-1);
                    GUI.addwaypntBtn.IsEnabled = false;
                }
            }
        }

        //hold values
        //0 for no buttons pressed
        //1 for left button pressed
        //2 for right button pressed
        public static void OnMouseMove(Point pos, int hold)
        {
            currentPoint = pos;
            if (mode != "none") DrawingEngine.OnMouseMove(pos, hold);
            MarkerLineControl(pos, "move");
            DrawParamsControl(pos, "move");
            switch (hold)
            {
                case 0:
                    //Needs Work
                    if (mode == "image")
                    {
                        Canvas.SetLeft(tempObject, left + currentPoint.X - clickedPoint.X);
                        Canvas.SetTop(tempObject, top + currentPoint.Y - clickedPoint.Y);
                    }
                    break;
                case 1:
                    if (tempObject is Ellipse)
                    {
                        Canvas.SetLeft(tempObject, left + currentPoint.X - clickedPoint.X);
                        Canvas.SetTop(tempObject, top + currentPoint.Y - clickedPoint.Y);
                    }
                    //Needs Work
                    else if (tempObject is Image)
                    {
                        Canvas.SetLeft(tempObject, left + currentPoint.X - clickedPoint.X);
                        Canvas.SetTop(tempObject, top + currentPoint.Y - clickedPoint.Y);
                    }
                    break;
                case 2:
                    break;
            }
        }

        public static void OnMouseEnter(Point pos)
        {
            MarkerLineControl(pos, "enter");
            DrawingEngine.OnMouseEnter();
        }

        public static void OnMouseLeave(Point pos)
        {
            MarkerLineControl(pos, "leave");
            DrawingEngine.OnMouseLeave();
        }

        public static void OnMouseWheel(int delta)
        {
            if (delta > 0)
            {
                MapZoomControl("outward");
            }
            else if (delta < 0)
            {
                MapZoomControl("inward");
            }
        }

        private static void OnDrawingFinished(object sender, DrawingFinishedEventArgs e)
        {
            DrawParamsControl(currentPoint, "drawingfinished");
            MarkerLineControl(currentPoint, "drawingfinished");
            mode = "none";
            selectedObject = null;
            objecthitpoint = 2;
            selectedObject = e.DrawnObject;
            drawCanvas.Cursor = Cursors.Arrow;
        }

        public static void KeyControls(string key)
        {
            if (mode != "none") DrawingEngine.KeyControls(key);
            switch (key)
            {
                case "delete":
                    if (selectedObject != null)
                    {
                        var type = selectedObject.GetType();
                        if (type == typeof(MapLine))
                        {
                            drawnObjects.Remove(selectedObject);
                            selectedObject = null;
                            PanelControl(-1);
                        }
                        else if (type == typeof(Circle))
                        {
                            drawnObjects.Remove(selectedObject);
                            selectedObject = null;
                            PanelControl(-1);
                        }
                        else if (type == typeof(PolygonLine))
                        {
                            var parent = ((PolygonLine)selectedObject).Parent;
                            if (parent.Lines.Count == 1)
                            {
                                drawnObjects.Remove(parent);
                                selectedObject = null;
                                PanelControl(-1);
                            }
                            else
                            {
                                int index = parent.Lines.IndexOf((PolygonLine)selectedObject);
                                parent.Lines.Remove((PolygonLine)selectedObject);
                                parent.Draw(drawCanvas);
                                selectedObject = null;
                                if (index >= parent.Lines.Count) index = parent.Lines.Count - 1;
                                selectedObject = parent.Lines[index];
                            }
                        }
                        else if (type == typeof(RouteLeg))
                        {
                            var parent = ((RouteLeg)selectedObject).Parent;
                            if (parent.Legs.Count == 1)
                            {
                                drawnObjects.Remove(parent);
                                selectedObject = null;
                                PanelControl(-1);
                            }
                            else
                            {
                                int index = parent.Legs.IndexOf((RouteLeg)selectedObject);
                                parent.Legs.Remove((RouteLeg)selectedObject);
                                parent.Draw(drawCanvas);
                                selectedObject = null;
                                if (index >= parent.Legs.Count) index = parent.Legs.Count - 1;
                                selectedObject = parent.Legs[index];
                            }
                        }
                        else if (type == typeof(DiversionLeg))
                        {
                            var parent = ((DiversionLeg)selectedObject).Parent;
                            var indx = ((DiversionLeg)selectedObject).LinkingLegIndex;
                            parent.DiversionLegs.Remove((DiversionLeg)selectedObject);
                            parent.Draw(drawCanvas);
                            selectedObject = null;
                            selectedObject = parent.Legs[indx];
                        }
                    }
                    break;
            }
        }

        public static void GlobalButtonActions(string button)
        {
            switch (button)
            {
                case "openBtn":
                    SaveLoadAndExport.LoadData(ref drawnObjects);
                    break;
                case "saveBtn":
                    SaveLoadAndExport.SaveData(drawnObjects.ToList());
                    break;
                case "saveasBtn":
                    break;
                case "exportBtn":
                    Printer.Print();
                    break;
                case "mapOpacity":
                    GUI.popOpacity.IsOpen = !GUI.popOpacity.IsOpen;
                    break;
                case "mapBtn":
                    if (!GUI.bafChart.IsVisible) GUI.bafChart.Visibility = Visibility.Visible;
                    else GUI.bafChart.Visibility = Visibility.Hidden;
                    break;
                case "platformsBtn":
                    Platforms platform_editor = new Platforms();
                    platform_editor.ShowDialog();
                    if (platform_editor.DialogResult.Value) UpdateAircraftData();
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

        public static void DrawButtonActions(string button)
        {
            switch (button)
            {
                case "msnBtn":
                    MissionPicker mspk = new MissionPicker();
                    if (mspk.ShowDialog().Value)
                    {
                        var aircraft = mspk.aircraftListbx.SelectedValue.ToString();
                        UIFieldWatcher.RefreshRoutePanel(aircraft);
                        PanelControl(3, 0);
                        GUI.addwaypntBtn.IsEnabled = true;
                    }
                    break;
                case "addwaypntBtn":
                    var ac = GUI.routeAcNameBox.Text;
                    mode = "route";
                    DrawingEngine.ModeSelect(mode, ac);
                    drawCanvas.Cursor = Cursors.Cross;
                    break;
                case "lineBtn":
                    mode = "line";
                    DrawingEngine.ModeSelect(mode);
                    drawCanvas.Cursor = Cursors.Cross;
                    break;
                case "circleBtn":
                    mode = "circle";
                    DrawingEngine.ModeSelect(mode);
                    drawCanvas.Cursor = Cursors.Cross;
                    break;
                case "orbitBtn":
                    mode = "orbit";
                    DrawingEngine.ModeSelect(mode);
                    drawCanvas.Cursor = Cursors.Cross;
                    break;
                case "plygnBtn":
                    mode = "polygon";
                    DrawingEngine.ModeSelect(mode);
                    drawCanvas.Cursor = Cursors.Cross;
                    break;
                case "imgBtn":
                    tempObject = LoadImage();
                    if (tempObject == null) return;
                    drawCanvas.Children.Add(tempObject);
                    mode = "image";
                    drawCanvas.Cursor = Cursors.Cross;
                    break;
                case "surfaceBtn":
                    break;
                case "addcheckpoint":
                    mode = "checkpoint";
                    DrawingEngine.ModeSelect(mode);
                    drawCanvas.Cursor = Cursors.Cross;
                    break;
                case "managecheckpoint":
                    if (GUI.checkpointssection.Visibility == Visibility.Collapsed)
                    {
                        GUI.checkpointssection.Visibility = Visibility.Visible;
                        GUI.managecheckpoint.Content = "Close Manager";
                    }
                    else
                    {
                        GUI.checkpointssection.Visibility = Visibility.Collapsed;
                        GUI.managecheckpoint.Content = "Manage Checkpoint";
                    }
                    break;
                case "delcheck":
                    if (GUI.routecheckpoints.SelectedIndex != -1) ((RouteLeg)selectedObject).Checkpoints.Remove((Checkpoint)GUI.routecheckpoints.SelectedItem);
                    break;
                case "diversionBtn":
                    mode = "diversion";
                    DrawingEngine.ModeSelect(mode);
                    drawCanvas.Cursor = Cursors.Cross;
                    break;
                case "panelFlip":
                    FlipControl();
                    break;
                case "updatecoord":
                    ApplyCoords();
                    break;
            }
        }

        public static void ObjectRemoveActions(string button)
        {
            object parent;
            switch (button)
            {
                case "removeLine":
                    drawnObjects.Remove(selectedObject);
                    selectedObject = null;
                    PanelControl(-1);
                    break;
                case "removeCircle":
                    drawnObjects.Remove(selectedObject);
                    selectedObject = null;
                    PanelControl(-1);
                    break;
                case "removePolygon":
                    parent = ((PolygonLine)selectedObject).Parent;
                    drawnObjects.Remove(parent);
                    selectedObject = null;
                    PanelControl(-1);
                    break;
                case "removeRoute":
                    parent = ((RouteLeg)selectedObject).Parent;
                    drawnObjects.Remove(parent);
                    selectedObject = null;
                    PanelControl(-1);
                    break;
                case "removeLeg":
                    if (selectedObject is RouteLeg)
                    {
                        var main = ((RouteLeg)selectedObject).Parent;
                        if (main.Legs.Count == 1)
                        {
                            drawnObjects.Remove(main);
                            selectedObject = null;
                            PanelControl(-1);
                        }
                        else
                        {
                            int index = main.Legs.IndexOf((RouteLeg)selectedObject);
                            main.Legs.Remove((RouteLeg)selectedObject);
                            main.Draw(drawCanvas);
                            selectedObject = null;
                            if (index >= main.Legs.Count) index = main.Legs.Count - 1;
                            selectedObject = main.Legs[index];
                        }
                    }
                    else if (selectedObject is DiversionLeg)
                    {
                        var main = ((DiversionLeg)selectedObject).Parent;
                        var indx = ((DiversionLeg)selectedObject).LinkingLegIndex;
                        main.DiversionLegs.Remove((DiversionLeg)selectedObject);
                        main.Draw(drawCanvas);
                        selectedObject = null;
                        selectedObject = main.Legs[indx];
                    }
                    break;
            }
        }

        public static void SizeAdjustActions(string button)
        {
            if (selectedObject != null)
            {
                var type = selectedObject.GetType();
                switch (button)
                {
                    case "thicknessPlus":
                        if (type == typeof(RouteLeg))
                        {
                            var obj = (RouteLeg)selectedObject;
                            if (obj.LineThickness < maxThickness) obj.LineThickness += 1;
                        }
                        break;
                    case "thicknessMinus":
                        if (type == typeof(RouteLeg))
                        {
                            var obj = (RouteLeg)selectedObject;
                            if (obj.LineThickness > minThickness) obj.LineThickness -= 1;
                        }
                        break;
                    case "waypntPlus":
                        break;
                    case "waypntMinus":
                        break;
                    case "headingPlus":
                        break;
                    case "headingMinus":
                        break;
                    case "headingOffsetXPlus":
                        if (type == typeof(RouteLeg))
                        {
                            var obj = (RouteLeg)selectedObject;
                            if (obj.PanelOffsetX < maxoffsetX) obj.PanelOffsetX += 5;
                        }
                        break;
                    case "headingOffsetXMinus":
                        if (type == typeof(RouteLeg))
                        {
                            var obj = (RouteLeg)selectedObject;
                            if (obj.PanelOffsetX > minoffsetX) obj.PanelOffsetX -= 5;
                        }
                        break;
                    case "headingOffsetYPlus":
                        if (type == typeof(RouteLeg))
                        {
                            var obj = (RouteLeg)selectedObject;
                            if (obj.PanelOffsetY > minoffsetY) obj.PanelOffsetY -= 5;
                        }
                        break;
                    case "headingOffsetYMinus":
                        if (type == typeof(RouteLeg))
                        {
                            var obj = (RouteLeg)selectedObject;
                            if (obj.PanelOffsetY < maxoffsetY) obj.PanelOffsetY += 5;
                        }
                        break;
                    case "fuelPlus":
                        break;
                    case "fuelMinus":
                        break;
                    case "labelPlus":
                        break;
                    case "labelMinus":
                        break;
                }
            }
        }

        public static void PanelExpansionActions(string button)
        {
            switch (button)
            {
                case "distdetbtn":
                    if (!GUI.distancedetails.IsVisible)
                    {
                        (GUI.distdetbtn.RenderTransform as RotateTransform).Angle = 90;
                        GUI.distancedetails.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        (GUI.distdetbtn.RenderTransform as RotateTransform).Angle = 0;
                        GUI.distancedetails.Visibility = Visibility.Collapsed;
                    }
                    break;
                case "timedetbtn":
                    if (!GUI.timedetails.IsVisible)
                    {
                        (GUI.timedetbtn.RenderTransform as RotateTransform).Angle = 90;
                        GUI.timedetails.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        (GUI.timedetbtn.RenderTransform as RotateTransform).Angle = 0;
                        GUI.timedetails.Visibility = Visibility.Collapsed;
                    }
                    break;
                case "fueldetbtn":
                    if (!GUI.fueldetails.IsVisible)
                    {
                        (GUI.fueldetbtn.RenderTransform as RotateTransform).Angle = 90;
                        GUI.fueldetails.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        (GUI.fueldetbtn.RenderTransform as RotateTransform).Angle = 0;
                        GUI.fueldetails.Visibility = Visibility.Collapsed;
                    }
                    break;
                case "coordinpbtn":
                    if (!GUI.coordinput.IsVisible)
                    {
                        (GUI.coordinpbtn.RenderTransform as RotateTransform).Angle = 90;
                        GUI.coordinput.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        (GUI.coordinpbtn.RenderTransform as RotateTransform).Angle = 0;
                        GUI.coordinput.Visibility = Visibility.Collapsed;
                    }
                    break;
            }
        }

        private static void OnObjectSelect()
        {
            var type = selectedObject.GetType();
            if (type == typeof(MapLine))
            {
                UIFieldWatcher.BindMapLine((MapLine)selectedObject, objecthitpoint);
                PanelControl(0);
            }
            else if (type == typeof(Circle))
            {
                UIFieldWatcher.BindCircle((Circle)selectedObject);
                PanelControl(1);
            }
            else if (type == typeof(PolygonLine))
            {
                UIFieldWatcher.BindPolygon((PolygonLine)selectedObject, objecthitpoint);
                PanelControl(2);
            }
            else if (type == typeof(RouteLeg))
            {
                UIFieldWatcher.BindRoute((RouteLeg)selectedObject, objecthitpoint);
                PanelControl(3, 1);
            }
            else if (type == typeof(DiversionLeg))
            {
                UIFieldWatcher.BindRoute((DiversionLeg)selectedObject, objecthitpoint);
                PanelControl(3, 1);
            }
            //Needs Work
            else if (type == typeof(Image))
            {
                UIFieldWatcher.BindImage((Image)selectedObject);
                PanelControl(4);
            }
        }

        private static void OnObjectDeselect()
        {
            if (selectedObject == null) return;
            PanelControl(-1);
            var type = selectedObject.GetType();
            if (type == typeof(MapLine))
            {
                UIFieldWatcher.Unbind("line");
            }
            else if (type == typeof(Circle))
            {
                UIFieldWatcher.Unbind("circle");
            }
            else if (type == typeof(PolygonLine))
            {
                UIFieldWatcher.Unbind("polygon");
            }
            else if (type == typeof(RouteLeg))
            {
                UIFieldWatcher.Unbind("route");
            }
            else if (type == typeof(DiversionLeg))
            {
                UIFieldWatcher.Unbind("diversion");
            }
            //Needs Work
            else if (type == typeof(Image))
            {
                UIFieldWatcher.Unbind("image");
            }
        }

        private static void FlipControl()
        {
            if (GUI.routecheckpoints.SelectedIndex != -1)
            {
                ((Checkpoint)GUI.routecheckpoints.SelectedItem).IsFlipped = !((Checkpoint)GUI.routecheckpoints.SelectedItem).IsFlipped;
                return;
            }
            if (selectedObject != null)
            {
                var type = selectedObject.GetType();
                if (type == typeof(RouteLeg))
                {
                    ((RouteLeg)selectedObject).IsFlipped = !((RouteLeg)selectedObject).IsFlipped;
                }
                else if (type == typeof(DiversionLeg))
                {
                    ((DiversionLeg)selectedObject).ContentFlip = !((DiversionLeg)selectedObject).ContentFlip;
                }
            }
        }

        /// <summary>
        /// Side Panel Visibility Control
        /// </summary>
        /// <param name="panel">Modes: 0 - Mapline, 1 - Circle, 2 - Polygon, 3 - Route, 4 - Image</param>
        private static void PanelControl(int panel, int? subpanel = null)
        {
            GUI.lineProperties.Visibility = Visibility.Collapsed;
            GUI.circleProperties.Visibility = Visibility.Collapsed;
            GUI.polygonProperties.Visibility = Visibility.Collapsed;
            GUI.routeProperties.Visibility = Visibility.Collapsed;
            GUI.imageProperties.Visibility = Visibility.Collapsed;
            switch (panel)
            {
                case 0:
                    GUI.lineProperties.Visibility = Visibility.Visible;
                    break;
                case 1:
                    GUI.circleProperties.Visibility = Visibility.Visible;
                    break;
                case 2:
                    GUI.polygonProperties.Visibility = Visibility.Visible;
                    break;
                case 3:
                    GUI.routeProperties.Visibility = Visibility.Visible;
                    break;
                case 4:
                    GUI.imageProperties.Visibility = Visibility.Visible;
                    break;
            }
            if (subpanel != null)
            {
                switch (subpanel)
                {
                    case 0:
                        GUI.routeLineBlock.Visibility = Visibility.Collapsed;
                        GUI.routeBlock.Visibility = Visibility.Collapsed;
                        GUI.routewaypointBlock.Visibility = Visibility.Collapsed;
                        GUI.routeOpblock.Visibility = Visibility.Collapsed;
                        break;
                    case 1:
                        GUI.routeLineBlock.Visibility = Visibility.Visible;
                        GUI.routeBlock.Visibility = Visibility.Visible;
                        GUI.routewaypointBlock.Visibility = Visibility.Visible;
                        GUI.routeOpblock.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private static void DrawParamsControl(Point pos, string action)
        {
            switch (action)
            {
                case "move":
                    PointLatLng geo = DataCalculations.GetLatLngFromPhysical(pos);
                    GUI.pos_lat.Text = DataFormatter.FormatGeo(geo.Lat, GeoFormat.DMS) + "N";
                    GUI.pos_long.Text = DataFormatter.FormatGeo(geo.Lng, GeoFormat.DMS) + "E";
                    if (DrawingEngine.isLiveDrawing)
                    {
                        if (!GUI.dst.IsVisible) GUI.dst.Visibility = Visibility.Visible;
                        if (!GUI.trk.IsVisible) GUI.trk.Visibility = Visibility.Visible;
                        GUI.dst.Text = DataFormatter.FormatData(DrawingEngine.getLiveDistance(), DataFormat.Short);
                        GUI.trk.Text = DataFormatter.FormatData(DrawingEngine.getLiveHeading(), DataFormat.Short);
                    }
                    else
                    {
                        if (GUI.dst.IsVisible) GUI.dst.Visibility = Visibility.Collapsed;
                        if (GUI.trk.IsVisible) GUI.trk.Visibility = Visibility.Collapsed;
                    }
                    break;
                case "drawingfinished":
                    if (GUI.dst.IsVisible) GUI.dst.Visibility = Visibility.Collapsed;
                    if (GUI.trk.IsVisible) GUI.trk.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private static void MarkerLineControl(Point pos, string action)
        {
            if (mode != "none")
            {
                switch (action)
                {
                    case "enter":
                        xline.Y1 = pos.Y;
                        xline.Y2 = pos.Y;
                        yline.X1 = pos.X;
                        yline.X2 = pos.X;
                        if (!drawCanvas.Children.Contains(xline)) drawCanvas.Children.Add(xline);
                        if (!drawCanvas.Children.Contains(yline)) drawCanvas.Children.Add(yline);
                        break;
                    case "leave":
                    case "drawingfinished":
                        if (drawCanvas.Children.Contains(xline) && drawCanvas.Children.Contains(yline))
                        {
                            drawCanvas.Children.Remove(xline);
                            drawCanvas.Children.Remove(yline);
                        }
                        break;
                    case "move":
                        xline.Y1 = pos.Y;
                        xline.Y2 = pos.Y;
                        yline.X1 = pos.X;
                        yline.X2 = pos.X;
                        break;
                }
            }
        }

        private static void MapZoomControl(string direction)
        {
            switch (direction)
            {
                case "inward":
                    mapZoom -= 0.2;
                    break;
                case "outward":
                    mapZoom += 0.2;
                    break;
            }
        }

        private static void RepositionMaps()
        {
            var pos = DataCalculations.GetPhysicalFromLatLng((PointLatLng)GUI.bafChart.Tag);
            Canvas.SetLeft(GUI.bafChart, pos.X);
            Canvas.SetTop(GUI.bafChart, pos.Y);
        }

        public static void RepositionPoints()
        {
            foreach (var obj in drawnObjects)
            {
                if (obj is MapLine)
                {
                    ((MapLine)obj).UpdateLocalParameters();
                }
                else if (obj is Circle)
                {
                    ((Circle)obj).UpdateLocalParameters();
                }
                else if (obj is Polygon)
                {
                    ((Polygon)obj).UpdateLocalParameters();
                }
                else if (obj is Route)
                {
                    ((Route)obj).UpdateLocalParameters();
                }
            }
        }

        private static void ApplyCoords()
        {
            if (!String.IsNullOrEmpty(GUI.Ncoorddeg.Text) && !String.IsNullOrEmpty(GUI.Ncoordmin.Text) && !String.IsNullOrEmpty(GUI.Ncoordsec.Text) && !String.IsNullOrEmpty(GUI.Ecoorddeg.Text) && !String.IsNullOrEmpty(GUI.Ecoordmin.Text) && !String.IsNullOrEmpty(GUI.Ecoordsec.Text))
            {
                double lat = Convert.ToDouble(GUI.Ncoorddeg.Text) + (Convert.ToDouble(GUI.Ncoordmin.Text) / 60) + (Convert.ToDouble(GUI.Ncoordsec.Text) / 3600);
                double lng = Convert.ToDouble(GUI.Ecoorddeg.Text) + (Convert.ToDouble(GUI.Ecoordmin.Text) / 60) + (Convert.ToDouble(GUI.Ecoordsec.Text) / 3600);
                var pos = new PointLatLng(lat, lng);
                var type = selectedObject.GetType();
                if (type == typeof(RouteLeg))
                {
                    if (objecthitpoint == 2) ((RouteLeg)selectedObject).EndPoint = pos;
                    else ((RouteLeg)selectedObject).StartPoint = pos;
                }
                else if (type == typeof(DiversionLeg))
                {
                    if (objecthitpoint == 1) ((DiversionLeg)selectedObject).StartPoint = pos;
                    else ((DiversionLeg)selectedObject).EndPoint = pos;
                }
            }
        }

        private static void UpdateAircraftData()
        {
            foreach (object o in drawnObjects)
            {
                if (o is Route)
                {
                    string temp = ((Route)o).Aircraft;
                    DataModel.InvalidateData(temp);
                    ((Route)o).Aircraft = "";
                    ((Route)o).Aircraft = temp;
                }
            }
        }

        //Needs Work
        private static Image LoadImage()
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

        //Needs Work
        public static void ResizeImage()
        {
            if (selectedObject is Image)
            {
                ((Image)selectedObject).Stretch = Stretch.Uniform;
            }
        }

        private static void DrawnObjectsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach(var obj in e.NewItems)
                {
                    var type = obj.GetType();
                    if (type == typeof(MapLine)) ((MapLine)obj).Draw(drawCanvas);
                    else if (type == typeof(Circle)) ((Circle)obj).Draw(drawCanvas);
                    else if (type == typeof(Polygon)) ((Polygon)obj).Draw(drawCanvas);
                    else if (type == typeof(Route)) ((Route)obj).Draw(drawCanvas);
                }
            }

            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var obj in e.OldItems)
                {
                    var type = obj.GetType();
                    if (type == typeof(MapLine)) ((MapLine)obj).Undraw(drawCanvas);
                    else if (type == typeof(Circle)) ((Circle)obj).Undraw(drawCanvas);
                    else if (type == typeof(Polygon)) ((Polygon)obj).Undraw(drawCanvas);
                    else if (type == typeof(Route)) ((Route)obj).Undraw(drawCanvas);
                }
            }
        }

    }
}
