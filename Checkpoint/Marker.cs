using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MissionAssistant
{
    class Marker : Checkpoint
    {
        #region Dependency Properties
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(MarkerType), typeof(Marker), new PropertyMetadata(MarkerType.None, new PropertyChangedCallback((d, e) => { TypePropertyChanged((Marker)d, e); })));

        //Overriden Properties
        public static new readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Brush), typeof(Marker), new PropertyMetadata(Brushes.White, new PropertyChangedCallback((d, e) => { ColorPropertyChanged((Marker)d, e); })));
        public static new readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(Marker), new PropertyMetadata(Brushes.Transparent, new PropertyChangedCallback((d, e) => { BackgroundPropertyChanged((Marker)d, e); })));
        public static new readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(Marker), new PropertyMetadata(23.0, new PropertyChangedCallback((d, e) => { FontSizePropertyChanged((Marker)d, e); })));
        public static new readonly DependencyProperty FontWeightProperty = DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(Marker), new PropertyMetadata(FontWeights.Bold, new PropertyChangedCallback((d, e) => { FontWeightPropertyChanged((Marker)d, e); })));
        public static new readonly DependencyProperty FontProperty = DependencyProperty.Register("Font", typeof(FontFamily), typeof(Marker), new PropertyMetadata(new PropertyChangedCallback((d, e) => { FontPropertyChanged((Marker)d, e); })));
        public static new readonly DependencyProperty IsFlippedProperty = DependencyProperty.Register("IsFlipped", typeof(bool), typeof(Marker), new PropertyMetadata(true, new PropertyChangedCallback((d, e) => { IsFlippedPropertyChanged((Marker)d, e); })));
        public static new readonly DependencyProperty DistanceProperty = DependencyProperty.Register("Distance", typeof(double), typeof(Marker), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { DistancePropertyChanged((Marker)d, e); })));
        #endregion

        #region Property Fields
        public MarkerType Type
        {
            get
            {
                return (MarkerType)GetValue(TypeProperty);
            }
            set
            {
                SetValue(TypeProperty, value);
            }
        }
        public new Brush Color
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
        public new Brush Background
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
        public new double FontSize
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
        public new FontWeight FontWeight
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
        public new FontFamily Font
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
        public new bool IsFlipped
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
        public new double Distance
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
        #endregion

        #region Property Callback Functions
        private static void TypePropertyChanged(Marker obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                MarkerType type = (MarkerType)e.NewValue;
                switch ((int)type)
                {
                    case 0:
                        obj.type.Content = "";
                        break;
                    case 1:
                        obj.type.Content = "TOC";
                        break;
                    case 2:
                        obj.type.Content = "TOD";
                        break;
                    case 3:
                        obj.type.Content = "BOD";
                        break;
                    default:
                        return;
                }
            }
        }
        private static void ColorPropertyChanged(Marker obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                Brush color = (Brush)e.NewValue;
                obj.marking.Stroke = color;
                obj.type.Foreground = color;
                obj.time.Foreground = color;
            }
        }
        private static void BackgroundPropertyChanged(Marker obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                Brush color = (Brush)e.NewValue;
                obj.type.Background = color;
                obj.time.Background = color;
            }
        }
        private static void FontSizePropertyChanged(Marker obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double size = (double)e.NewValue;
                obj.type.FontSize = size;
                obj.time.FontSize = size;
            }
        }
        private static void FontWeightPropertyChanged(Marker obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                FontWeight weight = (FontWeight)e.NewValue;
                obj.type.FontWeight = weight;
                obj.time.FontWeight = weight;
            }
        }
        private static void FontPropertyChanged(Marker obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                FontFamily font = (FontFamily)e.NewValue;
                obj.type.FontFamily = font;
                obj.time.FontFamily = font;
            }
        }
        private static void IsFlippedPropertyChanged(Marker obj, DependencyPropertyChangedEventArgs e)
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
                (obj.type.RenderTransform as ScaleTransform).ScaleX *= -1;
                (obj.time.RenderTransform as ScaleTransform).ScaleX *= -1;
            }
        }
        private static void DistancePropertyChanged(Marker obj, DependencyPropertyChangedEventArgs e)
        {

        }
        #endregion

        #region Internal Members
        protected Label type;
        #endregion

        #region Public Members
        #endregion

        #region Member Functions
        public Marker(RouteLeg parent) : base(parent)
        {

        }

        public override bool isDrawn(Canvas canvas)
        {
            return base.isDrawn(canvas);
        }

        public override void Draw(Canvas canvas, bool fromTop)
        {
            if (!isDrawn(canvas))
            {
                checkpoint.SetBinding(Canvas.LeftProperty, new Binding { Source = canvas, Path = new PropertyPath(Canvas.ActualWidthProperty), Converter = new CheckpointXConverter() });
                MultiBinding posBind = new MultiBinding { Converter = new CheckpointOffsetConverter(), ConverterParameter = canvas };
                posBind.Bindings.Add(new Binding { Source = this, Path = new PropertyPath(Marker.DistanceProperty) });
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

        public override void Undraw(Canvas canvas)
        {
            base.Undraw(canvas);
        }

        protected override void ConstructGeometry()
        {
            base.ConstructGeometry();
            type = new Label { Foreground = Color, Background = Background, FontSize = FontSize, FontWeight = FontWeight, Margin = new Thickness(0, -6, 0, 0), RenderTransformOrigin = new Point(0.5, 0.5), RenderTransform = new ScaleTransform() };
            switch ((int)Type)
            {
                case 0:
                    type.Content = "";
                    break;
                case 1:
                    type.Content = "TOC";
                    break;
                case 2:
                    type.Content = "TOD";
                    break;
                case 3:
                    type.Content = "BOD";
                    break;
                default:
                    return;

            }
            checkpoint.Children.Insert(1, type);
            if (IsFlipped) (type.RenderTransform as ScaleTransform).ScaleX *= -1;
        }
        #endregion
    }

    public enum MarkerType
    {
        None,
        TOC,
        TOD,
        BOD
    }
}
