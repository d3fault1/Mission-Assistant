using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MissionAssistant
{
    partial class RouteLeg : MapLine
    {
        #region Dependency Properties
        //Dependency view properties
        public static readonly DependencyProperty ArrowColorProperty = DependencyProperty.Register("ArrowColor", typeof(Brush), typeof(RouteLeg), new PropertyMetadata(Brushes.White, new PropertyChangedCallback((d, e) => { ArrowColorPropertyChanged((RouteLeg)d, e); })));
        public static readonly DependencyProperty PanelOffsetXProperty = DependencyProperty.Register("PanelOffsetX", typeof(double), typeof(RouteLeg), new PropertyMetadata(100.0, new PropertyChangedCallback((d, e) => { PanelOffsetXPropertyChanged((RouteLeg)d, e); })));
        public static readonly DependencyProperty PanelOffsetYProperty = DependencyProperty.Register("PanelOffsetY", typeof(double), typeof(RouteLeg), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { PanelOffsetYPropertyChanged((RouteLeg)d, e); })));
        public static readonly DependencyProperty ArrowColorBindsToLineProperty = DependencyProperty.Register("ArrowColorBindsToLine", typeof(bool), typeof(RouteLeg), new PropertyMetadata(true, new PropertyChangedCallback((d, e) => { ArrowColorBindsToLinePropertyChanged((RouteLeg)d, e); })));
        public static readonly DependencyProperty IsFlippedProperty = DependencyProperty.Register("IsFlipped", typeof(bool), typeof(RouteLeg), new PropertyMetadata(new PropertyChangedCallback((d, e) => { IsFlippedPropertyChanged((RouteLeg)d, e); })));


        //Modified inherited view properties
        public static new readonly DependencyProperty LineColorProperty = DependencyProperty.Register("LineColor", typeof(Brush), typeof(RouteLeg), new PropertyMetadata(Brushes.White, new PropertyChangedCallback((d, e) => { LineColorPropertyChanged((RouteLeg)d, e); })));
        public static new readonly DependencyProperty LineThicknessProperty = DependencyProperty.Register("LineThickness", typeof(double), typeof(RouteLeg), new PropertyMetadata(2.0, new PropertyChangedCallback((d, e) => { LineThicknessPropertyChanged((RouteLeg)d, e); })));
        #endregion

        #region Property Fields
        public Brush ArrowColor
        {
            get
            {
                return (Brush)GetValue(ArrowColorProperty);
            }
            set
            {
                SetValue(ArrowColorProperty, value);
            }
        }
        public double PanelOffsetX
        {
            get
            {
                return (double)GetValue(PanelOffsetXProperty);
            }
            set
            {
                SetValue(PanelOffsetXProperty, value);
            }
        }
        public double PanelOffsetY
        {
            get
            {
                return (double)GetValue(PanelOffsetYProperty);
            }
            set
            {
                SetValue(PanelOffsetYProperty, value);
            }
        }
        public bool ArrowColorBindsToLine
        {
            get
            {
                return (bool)GetValue(ArrowColorBindsToLineProperty);
            }
            set
            {
                SetValue(ArrowColorBindsToLineProperty, value);
            }
        }
        public bool IsFlipped
        {
            get
            {
                return (bool)GetValue(IsFlippedProperty);
            }
            set
            {
                SetValue(IsFlippedProperty, value);
            }
        }
        public new Brush LineColor
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
        public new double LineThickness
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
        #endregion

        #region Property Callback Functions
        private static void ArrowColorPropertyChanged(RouteLeg obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                if (!obj.ArrowColorBindsToLine)
                {
                    Brush color = (Brush)e.NewValue;
                    obj.arrow.Stroke = color;
                    obj.arrow.Fill = color;
                }
            }
        }
        private static void PanelOffsetXPropertyChanged(RouteLeg obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double offsetX = (double)e.NewValue;

                double left = (obj.LineThickness / 2) + offsetX;
                double top = obj.PanelOffsetY;
                double right = 0;
                double bottom = 0;
                obj.datapanel.Margin = new Thickness(left, top, right, bottom);
            }
        }
        private static void PanelOffsetYPropertyChanged(RouteLeg obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double offsetY = (double)e.NewValue;

                double left = (obj.LineThickness / 2) + obj.PanelOffsetX;
                double top = offsetY;
                double right = 0;
                double bottom = 0;
                obj.datapanel.Margin = new Thickness(left, top, right, bottom);
            }
        }
        private static void ArrowColorBindsToLinePropertyChanged(RouteLeg obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                bool isbound = (bool)e.NewValue;
                if (isbound)
                {
                    obj.arrow.Stroke = obj.LineColor;
                    obj.arrow.Fill = obj.LineColor;
                }
                else
                {
                    obj.arrow.Stroke = obj.ArrowColor;
                    obj.arrow.Fill = obj.ArrowColor;
                }
            }
        }
        private static void IsFlippedPropertyChanged(RouteLeg obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                bool isflipped = (bool)e.NewValue;
                var transform = obj.panelholder.RenderTransform as ScaleTransform;
                if (isflipped)
                {
                    if (transform.ScaleX < 0) return;
                }
                else
                {
                    if (transform.ScaleX > 0) return;
                }
                transform.ScaleX *= -1;
                (obj.datapanel.RenderTransform as ScaleTransform).ScaleX *= -1;
                foreach (var box in obj.HeadingBoxes)
                {
                    box.IsFlipped = isflipped;
                }
                foreach (var cp in obj.Checkpoints)
                {
                    cp.IsFlipped = isflipped;
                }
            }
        }
        private static void LineColorPropertyChanged(RouteLeg obj, DependencyPropertyChangedEventArgs e)
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
                if (obj.ArrowColorBindsToLine)
                {
                    obj.arrow.Stroke = stroke;
                    obj.arrow.Fill = stroke;
                }
            }
        }
        private static void LineThicknessPropertyChanged(RouteLeg obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double thickness = (double)e.NewValue;
                obj.line.StrokeThickness = thickness;
                obj.startnode.StrokeThickness = thickness;
                obj.endnode.StrokeThickness = thickness;

                double left = (thickness / 2) + obj.PanelOffsetX;
                double top = obj.PanelOffsetY;
                double right = 0;
                double bottom = 0;
                obj.datapanel.Margin = new Thickness(left, top, right, bottom);
            }
        }
        #endregion

        #region Internal Fields
        protected Canvas datacanvas;
        protected StackPanel panelholder, datapanel;
        protected Path arrow;
        #endregion

        #region Public Fields
        public ObservableCollection<HeadingBox> HeadingBoxes;
        public ObservableCollection<FuelCircle> FuelCircles;
        public ObservableCollection<Marker> Markers;
        public ObservableCollection<Checkpoint> Checkpoints;
        #endregion

        #region Member Functions
        public RouteLeg(Route parent) : base()
        {
            CreateObject(parent);
        }

        //Public Functions
        public override bool isDrawn(Canvas canvas)
        {
            return (canvas.Children.Contains(datacanvas) && base.isDrawn(canvas));
        }
        public override void Draw(Canvas canvas)
        {
            if (!isDrawn(canvas))
            {
                base.Draw(canvas);
                canvas.Children.Add(datacanvas);
            }
        }
        public override void Undraw(Canvas canvas)
        {
            if (isDrawn(canvas))
            {
                base.Undraw(canvas);
                canvas.Children.Remove(datacanvas);
            }
        }

        //Private Functions
        #region Event Functions
        private void HeadingBoxesListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (HeadingBox hb in e.NewItems)
                {
                    //Gets the index of the newly added headingbox
                    int index = HeadingBoxes.IndexOf(hb);
                    //Checking if a pairing index of a fuelcircle exists
                    if (index > FuelCircles.Count - 1)
                    {
                        //If not just insert at the begining
                        hb.Draw(datapanel, 20, 0);
                    }
                    else
                    {
                        //Otherwise insert right before that pairing fuelcircle
                        int dpindex = FuelCircles[index].GetIndex(datapanel);
                        if (dpindex != -1) hb.Draw(datapanel, 20, dpindex);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (HeadingBox hb in e.OldItems)
                {
                    hb.Undraw(datapanel);
                }
            }
        }
        private void FuelCirclesListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Same logic as the headingbox added event
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (FuelCircle fc in e.NewItems)
                {
                    int index = FuelCircles.IndexOf(fc);
                    if (index > HeadingBoxes.Count - 1)
                    {
                        fc.Draw(datapanel, 20, 0);
                    }
                    else
                    {
                        int dpindex = HeadingBoxes[index].GetIndex(datapanel);
                        if (dpindex != -1) fc.Draw(datapanel, 20, dpindex + 1);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (FuelCircle fc in e.OldItems)
                {
                    fc.Undraw(datapanel);
                }
            }
        }

        //Very static function. Needs Work
        private void MarkersListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Marker mk in e.NewItems)
                {
                    if (Markers.Count == 1) mk.Draw(datacanvas, false);
                    else mk.Draw(datacanvas, true);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Marker mk in e.OldItems)
                {
                    mk.Undraw(datacanvas);
                }
            }

            if (Markers.Count == 2)
            {
                MultiBinding visBind1 = new MultiBinding { Converter = new OutOfBoundConverter(), ConverterParameter = Markers[0] };
                visBind1.Bindings.Add(new Binding { Source = Markers[0], Path = new PropertyPath(Marker.LocalDistanceProperty) });
                visBind1.Bindings.Add(new Binding { Source = Markers[1], Path = new PropertyPath(Marker.LocalDistanceProperty) });
                visBind1.Bindings.Add(new Binding { Source = datacanvas, Path = new PropertyPath(Canvas.ActualHeightProperty) });
                BindingOperations.SetBinding(Markers[0], Marker.IsVisibleProperty, visBind1);

                MultiBinding visBind2 = new MultiBinding { Converter = new OutOfBoundConverter(), ConverterParameter = Markers[1] };
                visBind2.Bindings.Add(new Binding { Source = Markers[1], Path = new PropertyPath(Marker.LocalDistanceProperty) });
                visBind2.Bindings.Add(new Binding { Source = Markers[0], Path = new PropertyPath(Marker.LocalDistanceProperty) });
                visBind2.Bindings.Add(new Binding { Source = datacanvas, Path = new PropertyPath(Canvas.ActualHeightProperty) });
                BindingOperations.SetBinding(Markers[1], Marker.IsVisibleProperty, visBind2);
            }
        }
        private void CheckpointsListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Checkpoint cp in e.NewItems)
                {
                    int num = Checkpoints.IndexOf(cp);
                    cp.Name = $"Checkpoint #{++num}";
                    cp.Draw(datacanvas);
                    MultiBinding visBind = new MultiBinding { Converter = new OutOfBoundConverter(), ConverterParameter = 1 };
                    visBind.Bindings.Add(new Binding { Source = datacanvas, Path = new PropertyPath(Canvas.ActualHeightProperty) });
                    visBind.Bindings.Add(new Binding { Source = cp, Path = new PropertyPath(Checkpoint.LocalDistanceProperty) });
                    BindingOperations.SetBinding(cp, Checkpoint.IsVisibleProperty, visBind);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Checkpoint cp in e.OldItems)
                {
                    cp.Undraw(datacanvas);
                }
            }
        }
        #endregion

        //Custom Event View Calls
        private void TypeUpdatedView()
        {
            if (Type == RouteLegType.Landing)
            {
                if (FuelCircles.Count == 1) FuelCircles.Add(new FuelCircle());
            }
            else
            {
                if (FuelCircles.Count == 2) FuelCircles.Remove(FuelCircles[1]);
            }
        }

        protected override void ConstructGeometry()
        {
            base.ConstructGeometry();
            NodesFilled = false;
            NodeRadius = 10;

            startnode.Tag = new object[] { this, 1 };
            line.Tag = new object[] { this, 0 };
            endnode.Tag = new object[] { this, 2 };

            //datacanvas definitions and bindings
            datacanvas = new Canvas
            {
                IsHitTestVisible = false,
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = new RotateTransform()
            };
            MultiBinding dcHeight = new MultiBinding() { Converter = new PanelHolderHeightConverter() };
            dcHeight.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.X1Property) });
            dcHeight.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.Y1Property) });
            dcHeight.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.X2Property) });
            dcHeight.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.Y2Property) });
            datacanvas.SetBinding(Canvas.HeightProperty, dcHeight);
            MultiBinding dcMargin = new MultiBinding() { Converter = new RectangleMarginConverter() };
            dcMargin.Bindings.Add(new Binding { Source = datacanvas, Path = new PropertyPath(Canvas.ActualWidthProperty) });
            dcMargin.Bindings.Add(new Binding { Source = datacanvas, Path = new PropertyPath(Canvas.ActualHeightProperty) });
            datacanvas.SetBinding(Canvas.MarginProperty, dcMargin);
            MultiBinding X = new MultiBinding { Converter = new DataCanvasConverterX() };
            X.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.X1Property) });
            X.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.X2Property) });
            datacanvas.SetBinding(Canvas.LeftProperty, X);
            MultiBinding Y = new MultiBinding { Converter = new DataCanvasConverterY() };
            Y.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.Y1Property) });
            Y.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.Y2Property) });
            datacanvas.SetBinding(Canvas.TopProperty, Y);
            MultiBinding θ = new MultiBinding { Converter = new DataCanvasConverterθ() };
            θ.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.X1Property) });
            θ.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.Y1Property) });
            θ.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.X2Property) });
            θ.Bindings.Add(new Binding { Source = line, Path = new PropertyPath(Line.Y2Property) });
            BindingOperations.SetBinding((RotateTransform)datacanvas.RenderTransform, RotateTransform.AngleProperty, θ);

            //arrow definitions and bindings
            arrow = new Path
            {
                Data = PathGeometry.Parse(@"M20,5L0,0L5,5L0,10Z"),
                Margin = new Thickness(-6, -5, -13, -5),
                StrokeThickness = 2,
                RenderTransform = new RotateTransform(-90)
            };
            if (ArrowColorBindsToLine)
            {
                arrow.Stroke = LineColor;
                arrow.Fill = LineColor;
            }
            else
            {
                arrow.Stroke = ArrowColor;
                arrow.Fill = ArrowColor;
            }
            Viewbox arrowholder = new Viewbox { Child = arrow };
            arrowholder.SetBinding(Canvas.LeftProperty, new Binding { Source = datacanvas, Path = new PropertyPath(Canvas.WidthProperty), Converter = new MiddlePositionConverter() });
            arrowholder.SetBinding(Canvas.BottomProperty, new Binding { Source = datacanvas, Path = new PropertyPath(Canvas.HeightProperty), Converter = new ArrowPositionConverter() });
            datacanvas.Children.Add(arrowholder);


            //panelholder definitions and bindings
            panelholder = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                RenderTransformOrigin = new Point(0, 0.5),
                RenderTransform = new ScaleTransform()
            };
            panelholder.SetBinding(StackPanel.MarginProperty, new Binding { Source = panelholder, Path = new PropertyPath(StackPanel.ActualHeightProperty), Converter = new DataPanelMarginConverter() });
            panelholder.SetBinding(Canvas.LeftProperty, new Binding { Source = datacanvas, Path = new PropertyPath(Canvas.ActualWidthProperty), Converter = new MiddlePositionConverter() });
            panelholder.SetBinding(Canvas.TopProperty, new Binding { Source = datacanvas, Path = new PropertyPath(Canvas.ActualHeightProperty), Converter = new MiddlePositionConverter() });
            datacanvas.Children.Add(panelholder);

            //datacanvas binding dependent to panelholder
            datacanvas.SetBinding(Canvas.WidthProperty, new Binding { Source = panelholder, Path = new PropertyPath(StackPanel.ActualWidthProperty), Converter = new DataPanelContainerWidthConverter() });

            //datapanel definitions and bindings
            datapanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Center,
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = new ScaleTransform()
            };
            datapanel.Margin = new Thickness((LineThickness / 2) + PanelOffsetX, PanelOffsetY, 0, 0);
            panelholder.Children.Add(datapanel);
        }

        private void CreateObject(Route parent)
        {
            Parent = parent;
            HeadingBoxes = new ObservableCollection<HeadingBox>();
            FuelCircles = new ObservableCollection<FuelCircle>();
            Markers = new ObservableCollection<Marker>();
            Checkpoints = new ObservableCollection<Checkpoint>();

            Segments = new List<RouteLegSegment>(3);

            //Segment Initialization
            for (int i = 0; i < Segments.Capacity; i++)
            {
                Segments.Add(new RouteLegSegment(this));
            }

            HeadingBoxes.CollectionChanged += HeadingBoxesListChanged;
            FuelCircles.CollectionChanged += FuelCirclesListChanged;
            Markers.CollectionChanged += MarkersListChanged;
            Checkpoints.CollectionChanged += CheckpointsListChanged;
            HeadingBox headingBox = new HeadingBox();
            HeadingBoxes.Add(headingBox);
            FuelCircles.Add(new FuelCircle());
            BindingOperations.SetBinding(headingBox, HeadingBox.TrackProperty, new Binding { Source = this, Path = new PropertyPath(TrackProperty) });
            BindingOperations.SetBinding(headingBox, HeadingBox.TimeProperty, new Binding { Source = this, Path = new PropertyPath(TimeProperty) });
            BindingOperations.SetBinding(headingBox, HeadingBox.FuelProperty, new Binding { Source = this, Path = new PropertyPath(FuelProperty) });
            BindingOperations.SetBinding(headingBox, HeadingBox.AltitudeProperty, new Binding { Source = this, Path = new PropertyPath(AltitudeProperty) });

            Marker marking1 = new Marker(this);
            BindingOperations.SetBinding(marking1, Marker.TypeProperty, new Binding { Source = Segments[0], Path = new PropertyPath(RouteLegSegment.TypeProperty), Converter = new MarkingTypeConverter() });
            BindingOperations.SetBinding(marking1, Marker.DistanceProperty, new Binding { Source = Segments[0], Path = new PropertyPath(RouteLegSegment.DistanceProperty) });
            BindingOperations.SetBinding(marking1, Marker.TimeProperty, new Binding { Source = Segments[0], Path = new PropertyPath(RouteLegSegment.TimeProperty) });

            Marker marking2 = new Marker(this);
            BindingOperations.SetBinding(marking2, Marker.TypeProperty, new Binding { Source = Segments[2], Path = new PropertyPath(RouteLegSegment.TypeProperty), Converter = new MarkingTypeConverter() });
            BindingOperations.SetBinding(marking2, Marker.DistanceProperty, new Binding { Source = Segments[2], Path = new PropertyPath(RouteLegSegment.DistanceProperty) });
            MultiBinding m2tc = new MultiBinding() { Converter = new TODMarkingParamConverter() };
            m2tc.Bindings.Add(new Binding { Source = Segments[0], Path = new PropertyPath(RouteLegSegment.TimeProperty) });
            m2tc.Bindings.Add(new Binding { Source = Segments[1], Path = new PropertyPath(RouteLegSegment.TimeProperty) });
            BindingOperations.SetBinding(marking2, Marker.TimeProperty, m2tc);

            Markers.Add(marking1);
            Markers.Add(marking2);
        }

        #endregion
    }
}
