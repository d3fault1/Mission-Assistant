using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MissionAssistant
{
    partial class MapLine : DependencyObject
    {
        #region Dependency Properties
        //Dependency view properties
        public static readonly DependencyProperty LocalStartPointProperty = DependencyProperty.Register("LocalStartPoint", typeof(Point), typeof(MapLine), new PropertyMetadata(new PropertyChangedCallback((d, e) => { LocalStartPointPropertyChanged((MapLine)d, e); })));
        public static readonly DependencyProperty LocalEndPointProperty = DependencyProperty.Register("LocalEndPoint", typeof(Point), typeof(MapLine), new PropertyMetadata(new PropertyChangedCallback((d, e) => { LocalEndPointPropertyChanged((MapLine)d, e); })));
        public static readonly DependencyProperty LineColorProperty = DependencyProperty.Register("LineColor", typeof(Brush), typeof(MapLine), new PropertyMetadata(Brushes.White, new PropertyChangedCallback((d, e) => { LineColorPropertyChanged((MapLine)d, e); })));
        public static readonly DependencyProperty LineThicknessProperty = DependencyProperty.Register("LineThickness", typeof(double), typeof(MapLine), new PropertyMetadata(2.0, new PropertyChangedCallback((d, e) => { LineThicknessPropertyChanged((MapLine)d, e); })));
        public static readonly DependencyProperty LineDashArrayProperty = DependencyProperty.Register("LineDashArray", typeof(DoubleCollection), typeof(MapLine), new PropertyMetadata(new DoubleCollection { 1, 0 }, new PropertyChangedCallback((d, e) => { LineDashArrayPropertyChanged((MapLine)d, e); })));
        public static readonly DependencyProperty StartNodeRadiusProperty = DependencyProperty.Register("StartNodeRadius", typeof(double?), typeof(MapLine), new PropertyMetadata(new PropertyChangedCallback((d, e) => { StartNodeRadiusPropertyChanged((MapLine)d, e); })));
        public static readonly DependencyProperty EndNodeRadiusProperty = DependencyProperty.Register("EndNodeRadius", typeof(double?), typeof(MapLine), new PropertyMetadata(new PropertyChangedCallback((d, e) => { EndNodeRadiusPropertyChanged((MapLine)d, e); })));
        public static readonly DependencyProperty NodeRadiusProperty = DependencyProperty.Register("NodeRadius", typeof(double), typeof(MapLine), new PropertyMetadata(5.0, new PropertyChangedCallback((d, e) => { NodeRadiusPropertyChanged((MapLine)d, e); })));
        public static readonly DependencyProperty NodeColorProperty = DependencyProperty.Register("NodeColor", typeof(Brush), typeof(MapLine), new PropertyMetadata(Brushes.Blue, new PropertyChangedCallback((d, e) => { NodeColorPropertyChanged((MapLine)d, e); })));
        public static readonly DependencyProperty NodeColorBindsToLineProperty = DependencyProperty.Register("NodeColorBindsToLine", typeof(bool), typeof(MapLine), new PropertyMetadata(true, new PropertyChangedCallback((d, e) => { NodeColorBindsToLinePropertyChanged((MapLine)d, e); })));
        public static readonly DependencyProperty NodesFilledProperty = DependencyProperty.Register("NodesFilled", typeof(bool), typeof(MapLine), new PropertyMetadata(true, new PropertyChangedCallback((d, e) => { NodesFilledPropertyChanged((MapLine)d, e); })));
        public static readonly DependencyProperty RestrictPositionUpdateProperty = DependencyProperty.Register("RestrictPositionUpdate", typeof(bool), typeof(MapLine), new PropertyMetadata(false, new PropertyChangedCallback((d, e) => { RestrictPositionUpdatePropertyChanged((MapLine)d, e); })));
        #endregion

        #region Property Fields
        //Property fields of this class
        public Point LocalStartPoint
        {
            get
            {
                return (Point)GetValue(LocalStartPointProperty);
            }
            set
            {
                SetValue(LocalStartPointProperty, value);
            }
        }
        public Point LocalEndPoint
        {
            get
            {
                return (Point)GetValue(LocalEndPointProperty);
            }
            set
            {
                SetValue(LocalEndPointProperty, value);
            }
        }
        public Brush LineColor
        {
            get
            {
                return (Brush)GetValue(LineColorProperty);
            }
            set
            {
                SetValue(LineColorProperty, value);
            }
        }
        public double LineThickness
        {
            get
            {
                return (double)GetValue(LineThicknessProperty);
            }
            set
            {
                SetValue(LineThicknessProperty, value);
            }
        }
        public DoubleCollection LineDashArray
        {
            get
            {
                return (DoubleCollection)GetValue(LineDashArrayProperty);
            }
            set
            {
                SetValue(LineDashArrayProperty, value);
            }
        }
        public double? StartNodeRadius
        {
            get
            {
                return (double?)GetValue(StartNodeRadiusProperty);
            }
            set
            {
                SetValue(StartNodeRadiusProperty, value);
            }
        }
        public double? EndNodeRadius
        {
            get
            {
                return (double?)GetValue(EndNodeRadiusProperty);
            }
            set
            {
                SetValue(EndNodeRadiusProperty, value);
            }
        }
        public double NodeRadius
        {
            get
            {
                return (double)GetValue(NodeRadiusProperty);
            }
            set
            {
                SetValue(NodeRadiusProperty, value);
            }
        }
        public Brush NodeColor
        {
            get
            {
                return (Brush)GetValue(NodeColorProperty);
            }
            set
            {
                SetValue(NodeColorProperty, value);
            }
        }
        public bool NodeColorBindsToLine
        {
            get
            {
                return (bool)GetValue(NodeColorBindsToLineProperty);
            }
            set
            {
                SetValue(NodeColorBindsToLineProperty, value);
            }
        }
        public bool NodesFilled
        {
            get
            {
                return (bool)GetValue(NodesFilledProperty);
            }
            set
            {
                SetValue(NodesFilledProperty, value);
            }
        }
        public bool RestrictPositionUpdate
        {
            get
            {
                return (bool)GetValue(RestrictPositionUpdateProperty);
            }
            set
            {
                SetValue(RestrictPositionUpdateProperty, value);
            }
        }
        #endregion

        #region Property Callback Functions
        private static void LocalStartPointPropertyChanged(MapLine obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                Point pos = (Point)e.NewValue;

                //Update Canvas
                if (!obj.restrictCanvasStartLoop)
                {
                    obj.restrictCanvasStartLoop = true;
                    Canvas.SetLeft(obj.startnode, pos.X);
                    obj.restrictCanvasStartLoop = true;
                    Canvas.SetTop(obj.startnode, pos.Y);
                }
                else obj.restrictCanvasStartLoop = false;

                //Update Data
                if (!obj.RestrictPositionUpdate)
                {
                    if (!obj.restrictStartUpdate)
                    {
                        obj.restrictStartUpdate = true;
                        obj.StartPoint = DataCalculations.GetLatLngFromPhysical(pos);
                    }
                    else obj.restrictStartUpdate = false;
                }
            }
        }
        private static void LocalEndPointPropertyChanged(MapLine obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                Point pos = (Point)e.NewValue;

                //Update Canvas
                if (!obj.restrictCanvasEndLoop)
                {
                    obj.restrictCanvasEndLoop = true;
                    Canvas.SetLeft(obj.endnode, pos.X);
                    obj.restrictCanvasEndLoop = true;
                    Canvas.SetTop(obj.endnode, pos.Y);
                }
                else obj.restrictCanvasEndLoop = false;

                //Update Data
                if (!obj.RestrictPositionUpdate)
                {
                    if (!obj.restrictEndUpdate)
                    {
                        obj.restrictEndUpdate = true;
                        obj.EndPoint = DataCalculations.GetLatLngFromPhysical(pos);
                    }
                    else obj.restrictEndUpdate = false;
                }
            }
        }
        private static void LineColorPropertyChanged(MapLine obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                Brush stroke = (Brush)e.NewValue;
                obj.line.Stroke = stroke;
                if (obj.NodeColorBindsToLine)
                {
                    obj.startnode.Stroke = stroke;
                    obj.endnode.Stroke = stroke;
                }
            }
        }
        private static void LineThicknessPropertyChanged(MapLine obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double thickness = (double)e.NewValue;
                obj.line.StrokeThickness = thickness;
                obj.startnode.StrokeThickness = thickness;
                obj.endnode.StrokeThickness = thickness;
            }
        }
        private static void LineDashArrayPropertyChanged(MapLine obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                DoubleCollection dasharray = (DoubleCollection)e.NewValue;
                obj.line.StrokeDashArray = dasharray;
            }
        }
        private static void StartNodeRadiusPropertyChanged(MapLine obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                var radius = (double?)e.NewValue;
                if (!radius.HasValue)
                {
                    obj.startnode.Width = obj.NodeRadius * 2;
                    obj.startnode.Height = obj.NodeRadius * 2;
                }
                else
                {
                    obj.startnode.Width = radius.Value * 2;
                    obj.startnode.Height = radius.Value * 2;
                }
            }
        }
        private static void EndNodeRadiusPropertyChanged(MapLine obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                var radius = (double?)e.NewValue;
                if (!radius.HasValue)
                {
                    obj.endnode.Width = obj.NodeRadius * 2;
                    obj.endnode.Height = obj.NodeRadius * 2;
                }
                else
                {
                    obj.endnode.Width = radius.Value * 2;
                    obj.endnode.Height = radius.Value * 2;
                }
            }
        }
        private static void NodeRadiusPropertyChanged(MapLine obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double diameter = ((double)e.NewValue) * 2;
                if (obj.ReadLocalValue(StartNodeRadiusProperty) == DependencyProperty.UnsetValue)
                {
                    obj.startnode.Width = diameter;
                    obj.startnode.Height = diameter;
                }
                if (obj.ReadLocalValue(EndNodeRadiusProperty) == DependencyProperty.UnsetValue)
                {
                    obj.endnode.Width = diameter;
                    obj.endnode.Height = diameter;
                }
            }
        }
        private static void NodeColorPropertyChanged(MapLine obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                Brush stroke = (Brush)e.NewValue;
                if (!obj.NodeColorBindsToLine)
                {
                    obj.startnode.Stroke = stroke;
                    obj.endnode.Stroke = stroke;
                }
            }
        }
        private static void NodeColorBindsToLinePropertyChanged(MapLine obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                bool isbound = (bool)e.NewValue;
                if (isbound)
                {
                    obj.startnode.Stroke = obj.LineColor;
                    obj.endnode.Stroke = obj.LineColor;
                }
                else
                {
                    obj.startnode.Stroke = obj.NodeColor;
                    obj.endnode.Stroke = obj.NodeColor;
                }
            }
        }
        private static void NodesFilledPropertyChanged(MapLine obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                bool isfilled = (bool)e.NewValue;
                if (isfilled)
                {
                    obj.startnode.SetBinding(Ellipse.FillProperty, new Binding { Source = obj.startnode, Path = new PropertyPath(Ellipse.StrokeProperty) });
                    obj.endnode.SetBinding(Ellipse.FillProperty, new Binding { Source = obj.endnode, Path = new PropertyPath(Ellipse.StrokeProperty) });
                }
                else
                {
                    BindingOperations.ClearBinding(obj.startnode, Ellipse.FillProperty);
                    BindingOperations.ClearBinding(obj.endnode, Ellipse.FillProperty);
                    obj.startnode.Fill = Brushes.Transparent;
                    obj.endnode.Fill = Brushes.Transparent;
                }
            }
        }
        private static void RestrictPositionUpdatePropertyChanged(MapLine obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                bool isrestricted = (bool)e.NewValue;
                if (!isrestricted)
                {
                    //Update StartPoint
                    if (!obj.restrictStartUpdate)
                    {
                        obj.restrictStartUpdate = !obj.restrictStartUpdate;
                        obj.StartPoint = DataCalculations.GetLatLngFromPhysical(obj.LocalStartPoint);
                    }
                    else obj.restrictStartUpdate = !obj.restrictStartUpdate;

                    //Update EndPoint
                    if (!obj.restrictEndUpdate)
                    {
                        obj.restrictEndUpdate = !obj.restrictEndUpdate;
                        obj.EndPoint = DataCalculations.GetLatLngFromPhysical(obj.LocalEndPoint);
                    }
                    else obj.restrictEndUpdate = !obj.restrictEndUpdate;
                }
                else
                {

                }
            }
        }
        #endregion

        #region Internal Fields
        //Internal fields of this class
        protected Ellipse startnode;
        protected Ellipse endnode;
        protected Line line;
        private bool restrictStartUpdate = false, restrictEndUpdate = false, restrictCanvasStartLoop = false, restrictCanvasEndLoop = false;
        #endregion

        #region Public Fields

        #endregion

        #region Member Methods
        //Constructor
        public MapLine()
        {
            CreateObject();
        }

        //Public Methods
        public virtual void Draw(Canvas canvas)
        {
            if (!isDrawn(canvas))
            {
                canvas.Children.Add(startnode);
                canvas.Children.Add(line);
                canvas.Children.Add(endnode);
            }
        }
        public virtual void Undraw(Canvas canvas)
        {
            if (isDrawn(canvas))
            {
                canvas.Children.Remove(startnode);
                canvas.Children.Remove(line);
                canvas.Children.Remove(endnode);
            }
        }
        public virtual bool isDrawn(Canvas canvas)
        {
            return canvas.Children.Contains(startnode) && canvas.Children.Contains(line) && canvas.Children.Contains(endnode);
        }

        public void UpdateLocalParameters()
        {
            restrictStartUpdate = true;
            restrictEndUpdate = true;

            LocalStartPoint = DataCalculations.GetPhysicalFromLatLng(StartPoint);
            LocalEndPoint = DataCalculations.GetPhysicalFromLatLng(EndPoint);

            restrictStartUpdate = false;
            restrictEndUpdate = false;
        }

        //Protected Methods
        protected virtual void ConstructGeometry()
        {
            startnode = new Ellipse();
            line = new Line();
            endnode = new Ellipse();

            startnode.Tag = new object[] { this, 1 };
            line.Tag = new object[] { this, 0 };
            endnode.Tag = new object[] { this, 2 };

            Canvas.SetZIndex(line, 4);
            Canvas.SetZIndex(startnode, 5);
            Canvas.SetZIndex(endnode, 5);

            startnode.Fill = Brushes.Transparent;
            endnode.Fill = Brushes.Transparent;

            if (NodeColorBindsToLine)
            {
                startnode.Stroke = LineColor;
                endnode.Stroke = LineColor;
            }
            else
            {
                startnode.Stroke = NodeColor;
                endnode.Stroke = NodeColor;
            }
            line.Stroke = LineColor;
            line.StrokeThickness = LineThickness;

            if (!StartNodeRadius.HasValue)
            {
                startnode.Width = NodeRadius * 2;
                startnode.Height = NodeRadius * 2;
            }
            else
            {
                startnode.Width = StartNodeRadius.Value * 2;
                startnode.Height = StartNodeRadius.Value * 2;
            }
            if (!EndNodeRadius.HasValue)
            {
                endnode.Width = NodeRadius * 2;
                endnode.Height = NodeRadius * 2;
            }
            else
            {
                endnode.Width = EndNodeRadius.Value * 2;
                endnode.Height = EndNodeRadius.Value * 2;
            }

            line.StrokeDashArray = LineDashArray;
            startnode.SetBinding(Ellipse.MarginProperty, new Binding { Source = startnode, Path = new PropertyPath(Ellipse.ActualWidthProperty), Converter = new EllipseMarginConverter() });
            endnode.SetBinding(Ellipse.MarginProperty, new Binding { Source = endnode, Path = new PropertyPath(Ellipse.ActualWidthProperty), Converter = new EllipseMarginConverter() });
            startnode.SetBinding(Ellipse.StrokeThicknessProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeThicknessProperty), Converter = new NullHandleDoubleConverter() });
            endnode.SetBinding(Ellipse.StrokeThicknessProperty, new Binding { Source = line, Path = new PropertyPath(Line.StrokeThicknessProperty), Converter = new NullHandleDoubleConverter() });
            line.SetBinding(Line.X1Property, new Binding { Source = startnode, Path = new PropertyPath(Canvas.LeftProperty), Converter = new NullHandleDoubleConverter() });
            line.SetBinding(Line.Y1Property, new Binding { Source = startnode, Path = new PropertyPath(Canvas.TopProperty), Converter = new NullHandleDoubleConverter() });
            line.SetBinding(Line.X2Property, new Binding { Source = endnode, Path = new PropertyPath(Canvas.LeftProperty), Converter = new NullHandleDoubleConverter() });
            line.SetBinding(Line.Y2Property, new Binding { Source = endnode, Path = new PropertyPath(Canvas.TopProperty), Converter = new NullHandleDoubleConverter() });
            if (NodesFilled)
            {
                startnode.SetBinding(Ellipse.FillProperty, new Binding { Source = startnode, Path = new PropertyPath(Ellipse.StrokeProperty) });
                endnode.SetBinding(Ellipse.FillProperty, new Binding { Source = endnode, Path = new PropertyPath(Ellipse.StrokeProperty) });
            }
            var left = DependencyPropertyDescriptor.FromProperty(Canvas.LeftProperty, typeof(Canvas));
            var top = DependencyPropertyDescriptor.FromProperty(Canvas.TopProperty, typeof(Canvas));
            left.AddValueChanged(startnode, new EventHandler(CanvasStartPosChanged));
            top.AddValueChanged(startnode, new EventHandler(CanvasStartPosChanged));
            left.AddValueChanged(endnode, new EventHandler(CanvasEndPosChanged));
            top.AddValueChanged(endnode, new EventHandler(CanvasEndPosChanged));
        }

        //Private Methods
        private void CreateObject()
        {
            ConstructGeometry();
        }

        //Private Callbacks
        private void CanvasStartPosChanged(object sender, EventArgs e)
        {
            if (!restrictCanvasStartLoop)
            {
                restrictCanvasStartLoop = true;
                LocalStartPoint = new Point(Canvas.GetLeft((UIElement)sender), Canvas.GetTop((UIElement)sender));
            }
            else restrictCanvasStartLoop = false;
        }
        private void CanvasEndPosChanged(object sender, EventArgs e)
        {
            if (!restrictCanvasEndLoop)
            {
                restrictCanvasEndLoop = true;
                LocalEndPoint = new Point(Canvas.GetLeft((UIElement)sender), Canvas.GetTop((UIElement)sender));
            }
            else restrictCanvasEndLoop = false;
        }
        #endregion
    }
}
