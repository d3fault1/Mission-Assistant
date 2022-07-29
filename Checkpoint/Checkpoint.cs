using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MissionAssistant
{
    class Checkpoint : DependencyObject
    {
        #region Dependency Properties
        //Internal Dependency Properties
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Checkpoint), new PropertyMetadata(new PropertyChangedCallback((d, e) => { NamePropertyChanged((Checkpoint)d, e); })));
        public static readonly DependencyProperty LocalDistanceProperty = DependencyProperty.Register("LocalDistance", typeof(double), typeof(Checkpoint), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { LocalDistancePropertyChanged((Checkpoint)d, e); })));
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Brush), typeof(Checkpoint), new PropertyMetadata(Brushes.White, new PropertyChangedCallback((d, e) => { ColorPropertyChanged((Checkpoint)d, e); })));
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(Checkpoint), new PropertyMetadata(Brushes.Transparent, new PropertyChangedCallback((d, e) => { BackgroundPropertyChanged((Checkpoint)d, e); })));
        public static readonly DependencyProperty MarkingLengthProperty = DependencyProperty.Register("MarkingLength", typeof(double), typeof(Checkpoint), new PropertyMetadata(20.0, new PropertyChangedCallback((d, e) => { MarkingLengthPropertyChanged((Checkpoint)d, e); })));
        public static readonly DependencyProperty TextOffsetProperty = DependencyProperty.Register("TextOffset", typeof(double), typeof(Checkpoint), new PropertyMetadata(5.0, new PropertyChangedCallback((d, e) => { TextOffsetPropertyChanged((Checkpoint)d, e); })));
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(Checkpoint), new PropertyMetadata(23.0, new PropertyChangedCallback((d, e) => { FontSizePropertyChanged((Checkpoint)d, e); })));
        public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(Checkpoint), new PropertyMetadata(FontWeights.Bold, new PropertyChangedCallback((d, e) => { FontWeightPropertyChanged((Checkpoint)d, e); })));
        public static readonly DependencyProperty FontProperty = DependencyProperty.Register("Font", typeof(FontFamily), typeof(Checkpoint), new PropertyMetadata(new PropertyChangedCallback((d, e) => { FontPropertyChanged((Checkpoint)d, e); })));
        public static readonly DependencyProperty MarkingThicknessProperty = DependencyProperty.Register("MarkingThickness", typeof(double), typeof(Checkpoint), new PropertyMetadata(3.0, new PropertyChangedCallback((d, e) => { MarkingThicknessPropertyChanged((Checkpoint)d, e); })));
        public static readonly DependencyProperty IsFlippedProperty = DependencyProperty.Register("IsFlipped", typeof(bool), typeof(Checkpoint), new PropertyMetadata(true, new PropertyChangedCallback((d, e) => { IsFlippedPropertyChanged((Checkpoint)d, e); })));
        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register("Time", typeof(double), typeof(Checkpoint), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { TimePropertyChanged((Checkpoint)d, e); })));
        public static readonly DependencyProperty DistanceProperty = DependencyProperty.Register("Distance", typeof(double), typeof(Checkpoint), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { DistancePropertyChanged((Checkpoint)d, e); })));
        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.Register("IsVisible", typeof(bool), typeof(Checkpoint), new PropertyMetadata(true, new PropertyChangedCallback((d, e) => { IsVisiblePropertyChanged((Checkpoint)d, e); })));
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
        public double LocalDistance
        {
            get
            {
                return (double)GetValue(LocalDistanceProperty);
            }
            //set
            //{
            //    SetValue(LocalDistanceProperty, value);
            //}
        }
        public Brush Color
        {
            get
            {
                return (Brush)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }
        public Brush Background
        {
            get
            {
                return (Brush)GetValue(BackgroundProperty);
            }
            set
            {
                SetValue(BackgroundProperty, value);
            }
        }
        public double MarkingLength
        {
            get
            {
                return (double)GetValue(MarkingLengthProperty);
            }
            set
            {
                SetValue(MarkingLengthProperty, value);
            }
        }
        public double TextOffset
        {
            get
            {
                return (double)GetValue(TextOffsetProperty);
            }
            set
            {
                SetValue(TextOffsetProperty, value);
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
        public double MarkingThickness
        {
            get
            {
                return (double)GetValue(MarkingThicknessProperty);
            }
            set
            {
                SetValue(MarkingThicknessProperty, value);
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
        public double Distance
        {
            get
            {
                return (double)GetValue(DistanceProperty);
            }
            set
            {
                SetValue(DistanceProperty, value);
            }
        }
        public bool IsVisible
        {
            get
            {
                return (bool)GetValue(IsVisibleProperty);
            }
            set
            {
                SetValue(IsVisibleProperty, value);
            }
        }
        #endregion

        #region Property Callback Functions
        private static void NamePropertyChanged(Checkpoint obj, DependencyPropertyChangedEventArgs e)
        {

        }
        private static void LocalDistancePropertyChanged(Checkpoint obj, DependencyPropertyChangedEventArgs e)
        {

        }
        private static void ColorPropertyChanged(Checkpoint obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                Brush color = (Brush)e.NewValue;
                obj.marking.Stroke = color;
                obj.time.Foreground = color;
            }
        }
        private static void BackgroundPropertyChanged(Checkpoint obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                Brush color = (Brush)e.NewValue;
                obj.time.Background = color;
            }
        }
        private static void MarkingLengthPropertyChanged(Checkpoint obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double length = (double)e.NewValue;
                obj.marking.X2 = length + obj.MarkingThickness;
            }
        }
        private static void TextOffsetPropertyChanged(Checkpoint obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double offset = (double)e.NewValue;
                Thickness margin = obj.marking.Margin;
                obj.marking.Margin = new Thickness(margin.Left, margin.Top, offset, margin.Bottom);
            }
        }
        private static void FontSizePropertyChanged(Checkpoint obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double size = (double)e.NewValue;
                obj.time.FontSize = size;
            }
        }
        private static void FontWeightPropertyChanged(Checkpoint obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                FontWeight weight = (FontWeight)e.NewValue;
                obj.time.FontWeight = weight;
            }
        }
        private static void FontPropertyChanged(Checkpoint obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                FontFamily font = (FontFamily)e.NewValue;
                obj.time.FontFamily = font;
            }
        }
        private static void MarkingThicknessPropertyChanged(Checkpoint obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double thickness = (double)e.NewValue;
                obj.marking.StrokeThickness = thickness;
                obj.marking.X2 = obj.MarkingLength + thickness;
            }
        }
        private static void IsFlippedPropertyChanged(Checkpoint obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                bool flipped = (bool)e.NewValue;
                var transform = obj.checkpoint.RenderTransform as ScaleTransform;
                if (flipped)
                {
                    if (transform.ScaleX < 0) return;
                }
                else
                {
                    if (transform.ScaleX > 0) return;
                }
                (obj.checkpoint.RenderTransform as ScaleTransform).ScaleX *= -1;
                (obj.time.RenderTransform as ScaleTransform).ScaleX *= -1;
            }
        }
        private static void TimePropertyChanged(Checkpoint obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double time = (double)e.NewValue;
                if (time < 0) obj.time.Content = @"N\A";
                else obj.time.Content = DataFormatter.FormatTime(time, TimeFormat.Short);
            }
        }
        private static void DistancePropertyChanged(Checkpoint obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double distance = (double)e.NewValue;

                //Time Calculation
                if (distance <= obj.Parent.Segments[0].Distance || distance >= (obj.Parent.Segments[0].Distance + obj.Parent.Segments[1].Distance))
                {
                    obj.Time = -1;
                }
                else
                {
                    var level_time = DataCalculations.GetLevelTime(distance, obj.Parent.Speed, obj.Parent.Parent.Aircraft);
                    obj.Time = obj.Parent.Segments[0].Time + level_time;
                }
            }
        }
        private static void IsVisiblePropertyChanged(Checkpoint obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                bool visible = (bool)e.NewValue;
                if (visible)
                {
                    obj.checkpoint.Visibility = Visibility.Visible;
                }
                else
                {
                    obj.checkpoint.Visibility = Visibility.Hidden;
                }
            }
        }
        #endregion

        #region Internal Fields
        protected Line marking;
        protected Label time;
        protected StackPanel checkpoint;
        #endregion

        #region Public Fields
        public RouteLeg Parent { get; private set; }
        #endregion

        #region Member Functions
        public Checkpoint(RouteLeg parent)
        {
            CreateObject(parent);
        }

        //Public Methods
        public virtual bool isDrawn(Canvas canvas)
        {
            return canvas.Children.Contains(checkpoint);
        }
        public virtual void Draw(Canvas canvas, bool fromTop = false)
        {
            if (!isDrawn(canvas))
            {
                checkpoint.SetBinding(Canvas.LeftProperty, new Binding { Source = canvas, Path = new PropertyPath(Canvas.ActualWidthProperty), Converter = new CheckpointXConverter() });
                MultiBinding posBind = new MultiBinding { Converter = new CheckpointOffsetConverter(), ConverterParameter = canvas };
                posBind.Bindings.Add(new Binding { Source = this, Path = new PropertyPath(Checkpoint.DistanceProperty) });
                posBind.Bindings.Add(new Binding { Source = canvas, Path = new PropertyPath(Canvas.ActualHeightProperty) });
                posBind.Bindings.Add(new Binding { Source = canvas.RenderTransform, Path = new PropertyPath(RotateTransform.AngleProperty) });

                if (fromTop)
                {
                    checkpoint.SetBinding(Canvas.TopProperty, posBind);
                    BindingOperations.SetBinding(this, LocalDistanceProperty, new Binding { Source = checkpoint, Path = new PropertyPath(Canvas.TopProperty) });
                }
                else
                {
                    checkpoint.SetBinding(Canvas.BottomProperty, posBind);
                    BindingOperations.SetBinding(this, LocalDistanceProperty, new Binding { Source = checkpoint, Path = new PropertyPath(Canvas.BottomProperty) });
                }

                canvas.Children.Add(checkpoint);
            }
        }
        public virtual void Undraw(Canvas canvas)
        {
            if (isDrawn(canvas))
            {
                canvas.Children.Remove(checkpoint);
            }
        }

        //Private Methods
        protected virtual void ConstructGeometry()
        {
            checkpoint = new StackPanel { Orientation = Orientation.Horizontal, RenderTransform = new ScaleTransform() };
            marking = new Line { X1 = 0, Y1 = 0, Y2 = 0, VerticalAlignment = VerticalAlignment.Center, StrokeThickness = MarkingThickness, Stroke = Color, Margin = new Thickness(0, 0, TextOffset, 0) };
            time = new Label { Content = DataFormatter.FormatTime(Time, TimeFormat.Short), Foreground = Color, Background = Background, FontSize = FontSize, FontWeight = FontWeight, Margin = new Thickness(5, -6, 0, 0), RenderTransformOrigin = new Point(0.5, 0.5), RenderTransform = new ScaleTransform() };
            marking.SetBinding(Line.X2Property, new Binding { Source = marking, Path = new PropertyPath(Line.StrokeThicknessProperty), Converter = new CheckpointLengthConverter() });
            checkpoint.Children.Add(marking);
            checkpoint.Children.Add(time);

            //Margin Binding
            MultiBinding checkpointmarginbinding = new MultiBinding { Converter = new CheckpointMarginConverter2(), ConverterParameter = checkpoint };
            checkpointmarginbinding.Bindings.Add(new Binding { Source = checkpoint, Path = new PropertyPath(StackPanel.ActualWidthProperty) });
            checkpointmarginbinding.Bindings.Add(new Binding { Source = checkpoint, Path = new PropertyPath(StackPanel.ActualHeightProperty) });
            checkpointmarginbinding.Bindings.Add(new Binding { Source = marking, Path = new PropertyPath(Line.X2Property) });
            checkpointmarginbinding.Bindings.Add(new Binding { Source = marking });
            checkpoint.SetBinding(StackPanel.MarginProperty, checkpointmarginbinding);

            //Render Origin Binding
            MultiBinding checkpointoriginbinding = new MultiBinding { Converter = new CheckpointRenderOriginConverter2(), ConverterParameter = checkpoint };
            checkpointoriginbinding.Bindings.Add(new Binding { Source = checkpoint, Path = new PropertyPath(StackPanel.ActualWidthProperty) });
            checkpointoriginbinding.Bindings.Add(new Binding { Source = marking, Path = new PropertyPath(Line.X2Property) });
            checkpointoriginbinding.Bindings.Add(new Binding { Source = marking });
            checkpoint.SetBinding(StackPanel.RenderTransformOriginProperty, checkpointoriginbinding);
            if (IsFlipped)
            {
                (checkpoint.RenderTransform as ScaleTransform).ScaleX *= -1;
                (time.RenderTransform as ScaleTransform).ScaleX *= -1;
            }
            if (IsVisible) checkpoint.Visibility = Visibility.Visible;
            else checkpoint.Visibility = Visibility.Collapsed;
        }
        private void CreateObject(RouteLeg parent)
        {
            this.Parent = parent;
            ConstructGeometry();
        }
        #endregion
    }
}
