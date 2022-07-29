using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MissionAssistant
{
    class DiversionLeg : MapLine
    {
        #region Dependency Properties
        //Internal Dependency Properties
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(DiversionLeg), new PropertyMetadata("", new PropertyChangedCallback((d, e) => { NamePropertyChanged((DiversionLeg)d, e); })));
        public static readonly DependencyProperty ArrowColorProperty = DependencyProperty.Register("ArrowColor", typeof(Brush), typeof(DiversionLeg), new PropertyMetadata(Brushes.Red, new PropertyChangedCallback((d, e) => { ArrowColorPropertyChanged((DiversionLeg)d, e); })));
        public static readonly DependencyProperty TrackColorProperty = DependencyProperty.Register("TrackColor", typeof(Brush), typeof(DiversionLeg), new PropertyMetadata(Brushes.Red, new PropertyChangedCallback((d, e) => { TrackColorPropertyChanged((DiversionLeg)d, e); })));
        public static readonly DependencyProperty TimeColorProperty = DependencyProperty.Register("TimeColor", typeof(Brush), typeof(DiversionLeg), new PropertyMetadata(Brushes.Red, new PropertyChangedCallback((d, e) => { TimeColorPropertyChanged((DiversionLeg)d, e); })));
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(DiversionLeg), new PropertyMetadata(22.0, new PropertyChangedCallback((d, e) => { FontSizePropertyChanged((DiversionLeg)d, e); })));
        public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(DiversionLeg), new PropertyMetadata(FontWeights.Bold, new PropertyChangedCallback((d, e) => { FontWeightPropertyChanged((DiversionLeg)d, e); })));
        public static readonly DependencyProperty FontProperty = DependencyProperty.Register("Font", typeof(FontFamily), typeof(DiversionLeg), new PropertyMetadata(new PropertyChangedCallback((d, e) => { FontPropertyChanged((DiversionLeg)d, e); })));
        public static readonly DependencyProperty ContentOffsetProperty = DependencyProperty.Register("ContentOffset", typeof(double), typeof(DiversionLeg), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { ContentOffsetPropertyChanged((DiversionLeg)d, e); })));
        public static readonly DependencyProperty ContentOrientationProperty = DependencyProperty.Register("ContentOrientation", typeof(DiversionContentOrientation), typeof(DiversionLeg), new PropertyMetadata(DiversionContentOrientation.OneSided, new PropertyChangedCallback((d, e) => { ContentOrientationPropertyChanged((DiversionLeg)d, e); })));
        public static readonly DependencyProperty ContentFlipProperty = DependencyProperty.Register("ContentFlip", typeof(bool), typeof(DiversionLeg), new PropertyMetadata(false, new PropertyChangedCallback((d, e) => { ContentFlipPropertyChanged((DiversionLeg)d, e); })));
        public static readonly DependencyProperty ArrowColorBindsToLineProperty = DependencyProperty.Register("ArrowColorBindsToLine", typeof(bool), typeof(DiversionLeg), new PropertyMetadata(true, new PropertyChangedCallback((d, e) => { ArrowColorBindsToLinePropertyChanged((DiversionLeg)d, e); })));
        public static readonly DependencyProperty LabelsColorBindsToLineProperty = DependencyProperty.Register("LabelsColorBindsToLine", typeof(bool), typeof(DiversionLeg), new PropertyMetadata(true, new PropertyChangedCallback((d, e) => { LabelsColorBindsToLinePropertyChanged((DiversionLeg)d, e); })));
        public static readonly DependencyProperty LinkingLegIndexProperty = DependencyProperty.Register("LinkingLegIndex", typeof(int), typeof(DiversionLeg), new PropertyMetadata(new PropertyChangedCallback((d, e) => { LinkingLegPropertyChanged((DiversionLeg)d, e); })));
        public static readonly DependencyProperty LinkingNodeProperty = DependencyProperty.Register("LinkingNode", typeof(int), typeof(DiversionLeg), new PropertyMetadata(new PropertyChangedCallback((d, e) => { LinkingNodePropertyChanged((DiversionLeg)d, e); })));
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(RouteLegType), typeof(DiversionLeg), new PropertyMetadata(RouteLegType.Diversion, new PropertyChangedCallback((d, e) => { TypePropertyChanged((DiversionLeg)d, e); })));
        public static readonly DependencyProperty AltitudeProperty = DependencyProperty.Register("Altitude", typeof(double), typeof(DiversionLeg), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { AltitudePropertyChanged((DiversionLeg)d, e); })));
        public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register("Speed", typeof(double), typeof(DiversionLeg), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { SpeedPropertyChanged((DiversionLeg)d, e); })));
        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register("Time", typeof(double), typeof(DiversionLeg), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { TimePropertyChanged((DiversionLeg)d, e); })));
        public static readonly DependencyProperty FuelProperty = DependencyProperty.Register("Fuel", typeof(double), typeof(DiversionLeg), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { FuelPropertyChanged((DiversionLeg)d, e); })));

        //Modified inherited view properties
        public static new readonly DependencyProperty LineColorProperty = DependencyProperty.Register("LineColor", typeof(Brush), typeof(DiversionLeg), new PropertyMetadata(Brushes.Red, new PropertyChangedCallback((d, e) => { LineColorPropertyChanged((DiversionLeg)d, e); })));
        #endregion

        #region Property Fields
        public string Name
        {
            get
            {
                return (string)GetValue(NameProperty);
            }
            set
            {
                SetValue(NameProperty, value);
            }
        }
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
        public Brush TrackColor
        {
            get
            {
                return (Brush)GetValue(TrackColorProperty);
            }
            set
            {
                SetValue(TrackColorProperty, value);
            }
        }
        public Brush TimeColor
        {
            get
            {
                return (Brush)GetValue(TimeColorProperty);
            }
            set
            {
                SetValue(TimeColorProperty, value);
            }
        }
        public double FontSize
        {
            get
            {
                return (double)GetValue(FontSizeProperty);
            }
            set
            {
                SetValue(FontSizeProperty, value);
            }
        }
        public FontWeight FontWeight
        {
            get
            {
                return (FontWeight)GetValue(FontWeightProperty);
            }
            set
            {
                SetValue(FontWeightProperty, value);
            }
        }
        public FontFamily Font
        {
            get
            {
                return (FontFamily)GetValue(FontProperty);
            }
            set
            {
                SetValue(FontProperty, value);
            }
        }
        public double ContentOffset
        {
            get { return (double)GetValue(ContentOffsetProperty); }
            set
            {
                SetValue(ContentOffsetProperty, value);
            }
        }
        public DiversionContentOrientation ContentOrientation
        {
            get
            {
                return (DiversionContentOrientation)GetValue(ContentOrientationProperty);
            }
            set
            {
                SetValue(ContentOrientationProperty, value);
            }
        }
        public bool ContentFlip
        {
            get
            {
                return (bool)GetValue(ContentFlipProperty);
            }
            set
            {
                SetValue(ContentFlipProperty, value);
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
        public bool LabelsColorBindsToLine
        {
            get
            {
                return (bool)GetValue(LabelsColorBindsToLineProperty);
            }
            set
            {
                SetValue(LabelsColorBindsToLineProperty, value);
            }
        }
        public int LinkingLegIndex
        {
            get
            {
                return (int)GetValue(LinkingLegIndexProperty);
            }
            set
            {
                SetValue(LinkingLegIndexProperty, value);
            }
        }
        public int LinkingNode
        {
            get
            {
                return (int)GetValue(LinkingNodeProperty);
            }
            set
            {
                SetValue(LinkingNodeProperty, value);
            }
        }
        public RouteLegType Type
        {
            get
            {
                return (RouteLegType)GetValue(TypeProperty);
            }
            //set
            //{
            //    SetValue(TypeProperty, value);
            //}
        }
        public double Altitude
        {
            get
            {
                return (double)GetValue(AltitudeProperty);
            }
            set
            {
                SetValue(AltitudeProperty, value);
            }
        }
        public double Speed
        {
            get
            {
                return (double)GetValue(SpeedProperty);
            }
            set
            {
                SetValue(SpeedProperty, value);
            }
        }
        public double Time
        {
            get
            {
                return (double)GetValue(TimeProperty);
            }
            set
            {
                SetValue(TimeProperty, value);
            }
        }
        public double Fuel
        {
            get
            {
                return (double)GetValue(FuelProperty);
            }
            set
            {
                SetValue(FuelProperty, value);
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
        #endregion

        #region Property Callback Functions
        private static void NamePropertyChanged(DiversionLeg obj, DependencyPropertyChangedEventArgs e)
        {

        }
        private static void ArrowColorPropertyChanged(DiversionLeg obj, DependencyPropertyChangedEventArgs e)
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
        private static void TrackColorPropertyChanged(DiversionLeg obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                if (!obj.LabelsColorBindsToLine)
                {
                    Brush color = (Brush)e.NewValue;
                    obj.track.Foreground = color;
                    if (color != obj.TimeColor) obj.divider.Foreground = Brushes.White;
                    else obj.divider.Foreground = color;
                }
            }
        }
        private static void TimeColorPropertyChanged(DiversionLeg obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                if (!obj.LabelsColorBindsToLine)
                {
                    Brush color = (Brush)e.NewValue;
                    obj.time.Foreground = color;
                    if (color != obj.TrackColor) obj.divider.Foreground = Brushes.White;
                    else obj.divider.Foreground = color;
                }
            }
        }
        private static void FontSizePropertyChanged(DiversionLeg obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double size = (double)e.NewValue;
                obj.track.FontSize = size;
                obj.time.FontSize = size;
                obj.divider.FontSize = size;
            }
        }
        private static void FontWeightPropertyChanged(DiversionLeg obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                FontWeight weight = (FontWeight)e.NewValue;
                obj.track.FontWeight = weight;
                obj.time.FontWeight = weight;
                obj.divider.FontWeight = weight;
            }
        }
        private static void FontPropertyChanged(DiversionLeg obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                FontFamily font = (FontFamily)e.NewValue;
                obj.track.FontFamily = font;
                obj.time.FontFamily = font;
                obj.divider.FontFamily = font;
            }
        }
        private static void ContentOffsetPropertyChanged(DiversionLeg obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double offset = (double)e.NewValue;
                obj.datapanel1.Margin = new Thickness(offset, 0, 0, 0);
                obj.datapanel2.Margin = new Thickness(offset, 0, 0, 0);
            }
        }
        private static void ContentOrientationPropertyChanged(DiversionLeg obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                DiversionContentOrientation orientation = (DiversionContentOrientation)e.NewValue;
                if (orientation == DiversionContentOrientation.OneSided)
                {
                    obj.datapanel2.Children.Remove(obj.time);
                    obj.datapanel1.Children.Add(obj.divider);
                    obj.datapanel1.Children.Add(obj.time);
                }
                else
                {
                    obj.datapanel1.Children.Remove(obj.time);
                    obj.datapanel1.Children.Remove(obj.divider);
                    obj.datapanel2.Children.Add(obj.time);
                }
            }
        }
        private static void ContentFlipPropertyChanged(DiversionLeg obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                bool isflipped = (bool)e.NewValue;
                if (isflipped)
                {
                    (obj.panelholder1.RenderTransform as ScaleTransform).ScaleX = -1;
                    (obj.datapanel1.RenderTransform as ScaleTransform).ScaleX = -1;
                    (obj.panelholder2.RenderTransform as ScaleTransform).ScaleX = 1;
                    (obj.datapanel2.RenderTransform as ScaleTransform).ScaleX = 1;
                }
                else
                {
                    (obj.panelholder1.RenderTransform as ScaleTransform).ScaleX = 1;
                    (obj.datapanel1.RenderTransform as ScaleTransform).ScaleX = 1;
                    (obj.panelholder2.RenderTransform as ScaleTransform).ScaleX = -1;
                    (obj.datapanel2.RenderTransform as ScaleTransform).ScaleX = -1;
                }
            }
        }
        private static void ArrowColorBindsToLinePropertyChanged(DiversionLeg obj, DependencyPropertyChangedEventArgs e)
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
        private static void LabelsColorBindsToLinePropertyChanged(DiversionLeg obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                bool isbound = (bool)e.NewValue;
                if (isbound)
                {
                    obj.track.Foreground = obj.LineColor;
                    obj.divider.Foreground = obj.LineColor;
                    obj.time.Foreground = obj.LineColor;
                }
                else
                {
                    obj.track.Foreground = obj.TrackColor;
                    obj.time.Foreground = obj.TimeColor;
                    if (obj.TrackColor == obj.TimeColor) obj.divider.Foreground = obj.TrackColor;
                    else obj.divider.Foreground = Brushes.White;
                }
            }
        }
        private static void LinkingLegPropertyChanged(DiversionLeg obj, DependencyPropertyChangedEventArgs e)
        {

        }
        private static void LinkingNodePropertyChanged(DiversionLeg obj, DependencyPropertyChangedEventArgs e)
        {

        }
        private static void TypePropertyChanged(DiversionLeg obj, DependencyPropertyChangedEventArgs e)
        {

        }
        private static void AltitudePropertyChanged(DiversionLeg obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                obj.CalculateData();
            }
        }
        private static void SpeedPropertyChanged(DiversionLeg obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                obj.CalculateData();
            }
        }
        private static void TimePropertyChanged(DiversionLeg obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double time = (double)e.NewValue;
                obj.time.Content = DataFormatter.FormatTime(time, TimeFormat.Short);
            }
        }
        private static void FuelPropertyChanged(DiversionLeg obj, DependencyPropertyChangedEventArgs e)
        {

        }

        //Overriden property callbacks
        private static void LineColorPropertyChanged(DiversionLeg obj, DependencyPropertyChangedEventArgs e)
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

                if (obj.LabelsColorBindsToLine)
                {
                    obj.track.Foreground = stroke;
                    obj.divider.Foreground = stroke;
                    obj.time.Foreground = stroke;
                }
            }
        }
        #endregion

        #region Internal Fields
        Canvas datacanvas;
        StackPanel panelholder1, panelholder2, datapanel1, datapanel2;
        Path arrow;
        Label track, time, divider;
        #endregion

        #region Public Fields
        public Route Parent { get; private set; }

        public ObservableCollection<Checkpoint> Checkpoints;
        #endregion

        #region Member Functions
        public DiversionLeg(Route Parent) : base()
        {
            CreateObject(Parent);
        }

        public override bool isDrawn(Canvas canvas)
        {
            return canvas.Children.Contains(datacanvas) && base.isDrawn(canvas);
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

        //Data Functions
        public void CalculateData()
        {
            Time = DataCalculations.GetLevelTime(Distance, Speed, Parent.Aircraft);
            Fuel = DataCalculations.GetLevelFuel(Distance, Time, Altitude, Speed, Parent.Aircraft);
        }

        //Private Functions
        protected override void OnDistanceUpdated()
        {
            base.OnDistanceUpdated();
            CalculateData();
        }
        protected override void OnTrackUpdated()
        {
            base.OnTrackUpdated();
            track.Content = DataFormatter.FormatData(Track, DataFormat.Short);
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

        protected override void ConstructGeometry()
        {
            base.ConstructGeometry();
            NodesFilled = false;
            NodeRadius = 10;
            LineDashArray = new DoubleCollection { 3, 2 };

            //Needs Work
            base.LineColor = LineColor;

            startnode.Tag = new object[] { this, 1 };
            line.Tag = new object[] { this, 0 };
            endnode.Tag = new object[] { this, 2 };

            Panel.SetZIndex(startnode, 3);
            Panel.SetZIndex(line, 2);
            Panel.SetZIndex(endnode, 3);

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

            //panelholder1 definitions and bindings
            panelholder1 = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                RenderTransformOrigin = new Point(0, 0.5),
                RenderTransform = new ScaleTransform()
            };
            panelholder1.SetBinding(StackPanel.MarginProperty, new Binding { Source = panelholder1, Path = new PropertyPath(StackPanel.ActualHeightProperty), Converter = new DataPanelMarginConverter() });
            panelholder1.SetBinding(Canvas.LeftProperty, new Binding { Source = datacanvas, Path = new PropertyPath(Canvas.ActualWidthProperty), Converter = new MiddlePositionConverter() });
            panelholder1.SetBinding(Canvas.TopProperty, new Binding { Source = datacanvas, Path = new PropertyPath(Canvas.ActualHeightProperty), Converter = new MiddlePositionConverter() });
            datacanvas.Children.Add(panelholder1);

            //panelholder2 definitions and bindings
            panelholder2 = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                RenderTransformOrigin = new Point(0, 0.5),
                RenderTransform = new ScaleTransform() { ScaleX = -1 }
            };
            panelholder2.SetBinding(StackPanel.MarginProperty, new Binding { Source = panelholder2, Path = new PropertyPath(StackPanel.ActualHeightProperty), Converter = new DataPanelMarginConverter() });
            panelholder2.SetBinding(Canvas.LeftProperty, new Binding { Source = datacanvas, Path = new PropertyPath(Canvas.ActualWidthProperty), Converter = new MiddlePositionConverter() });
            panelholder2.SetBinding(Canvas.TopProperty, new Binding { Source = datacanvas, Path = new PropertyPath(Canvas.ActualHeightProperty), Converter = new MiddlePositionConverter() });
            datacanvas.Children.Add(panelholder2);

            //datacanvas binding dependent to panelholder
            datacanvas.SetBinding(Canvas.WidthProperty, new Binding { Source = panelholder1, Path = new PropertyPath(StackPanel.ActualWidthProperty), Converter = new DataPanelContainerWidthConverter() });

            //datapanel1 definitions and bindings
            datapanel1 = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                LayoutTransform = new RotateTransform(-90),
                RenderTransform = new ScaleTransform()
            };
            MultiBinding org1 = new MultiBinding { Converter = new DiversionLegFlipConverter(), Mode = BindingMode.OneWay };
            org1.Bindings.Add(new Binding { Source = datapanel1, Path = new PropertyPath(StackPanel.ActualWidthProperty), Mode = BindingMode.OneWay });
            org1.Bindings.Add(new Binding { Source = datapanel1, Path = new PropertyPath(StackPanel.ActualHeightProperty), Mode = BindingMode.OneWay });
            datapanel1.SetBinding(StackPanel.RenderTransformOriginProperty, org1);
            datapanel1.Margin = new Thickness(ContentOffset, 0, 0, 0);
            panelholder1.Children.Add(datapanel1);

            //datapanel2 definitions and bindings
            datapanel2 = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                LayoutTransform = new RotateTransform(-90),
                RenderTransform = new ScaleTransform() { ScaleX = -1 }
            };
            MultiBinding org2 = new MultiBinding { Converter = new DiversionLegFlipConverter(), Mode = BindingMode.OneWay };
            org2.Bindings.Add(new Binding { Source = datapanel2, Path = new PropertyPath(StackPanel.ActualWidthProperty), Mode = BindingMode.OneWay });
            org2.Bindings.Add(new Binding { Source = datapanel2, Path = new PropertyPath(StackPanel.ActualHeightProperty), Mode = BindingMode.OneWay });
            datapanel2.SetBinding(StackPanel.RenderTransformOriginProperty, org2);
            datapanel2.Margin = new Thickness(ContentOffset, 0, 0, 0);
            panelholder2.Children.Add(datapanel2);

            //labels definitions and bindings
            time = new Label
            {
                FontSize = FontSize,
                FontWeight = FontWeight,
                Content = DataFormatter.FormatTime(Time, TimeFormat.Short)
            };

            track = new Label
            {
                FontSize = FontSize,
                FontWeight = FontWeight,
                Content = DataFormatter.FormatData(Track, DataFormat.Short)
            };

            divider = new Label
            {
                FontSize = FontSize,
                FontWeight = FontWeight,
                Content = "/"
            };

            datapanel1.Children.Add(track);

            if (LabelsColorBindsToLine)
            {
                track.Foreground = LineColor;
                divider.Foreground = LineColor;
                time.Foreground = LineColor;
            }
            else
            {
                track.Foreground = TrackColor;
                time.Foreground = TimeColor;
                if (TrackColor == TimeColor) divider.Foreground = TrackColor;
                else divider.Foreground = Brushes.White;
            }

            if (ContentFlip)
            {
                (panelholder1.RenderTransform as ScaleTransform).ScaleX = -1;
                (datapanel1.RenderTransform as ScaleTransform).ScaleX = -1;
                (panelholder2.RenderTransform as ScaleTransform).ScaleX = 1;
                (datapanel2.RenderTransform as ScaleTransform).ScaleX = 1;
            }

            if (ContentOrientation == DiversionContentOrientation.OneSided)
            {
                datapanel1.Children.Add(divider);
                datapanel1.Children.Add(time);
            }
            else
            {
                datapanel2.Children.Add(time);
            }

        }

        private void CreateObject(Route Parent)
        {
            this.Parent = Parent;
            Checkpoints = new ObservableCollection<Checkpoint>();
            Checkpoints.CollectionChanged += CheckpointsListChanged;
        }

        #endregion
    }
    public enum DiversionContentOrientation
    {
        OneSided,
        TwoSided
    }
}
