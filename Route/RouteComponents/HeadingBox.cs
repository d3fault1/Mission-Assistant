using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MissionAssistant
{
    class HeadingBox : DependencyObject
    {
        #region Dependency Properties
        //Internal Dependency Properties
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(HeadingBox), new PropertyMetadata(Brushes.DarkGray, new PropertyChangedCallback((d, e) => { BackgroundPropertyChanged((HeadingBox)d, e); })));
        public static readonly DependencyProperty BorderColorProperty = DependencyProperty.Register("BorderColor", typeof(Brush), typeof(HeadingBox), new PropertyMetadata(Brushes.Black, new PropertyChangedCallback((d, e) => { BorderColorPropertyChanged((HeadingBox)d, e); })));
        public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(double), typeof(HeadingBox), new PropertyMetadata(3.0, new PropertyChangedCallback((d, e) => { BorderThicknessPropertyChanged((HeadingBox)d, e); })));
        public static readonly DependencyProperty TriangleFillProperty = DependencyProperty.Register("TriangleFill", typeof(Brush), typeof(HeadingBox), new PropertyMetadata(Brushes.DarkRed, new PropertyChangedCallback((d, e) => { TriangleFillPropertyChanged((HeadingBox)d, e); })));
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(HeadingBox), new PropertyMetadata(120.0, new PropertyChangedCallback((d, e) => { WidthPropertyChanged((HeadingBox)d, e); })));
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(HeadingBox), new PropertyMetadata(140.0, new PropertyChangedCallback((d, e) => { HeightPropertyChanged((HeadingBox)d, e); })));
        public static readonly DependencyProperty TriangleHeightProperty = DependencyProperty.Register("TriangleHeight", typeof(double), typeof(HeadingBox), new PropertyMetadata(40.0, new PropertyChangedCallback((d, e) => { TriangleHeightPropertyChanged((HeadingBox)d, e); })));
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(HeadingBox), new PropertyMetadata(16.0, new PropertyChangedCallback((d, e) => { FontSizePropertyChanged((HeadingBox)d, e); })));
        public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(HeadingBox), new PropertyMetadata(FontWeights.Bold, new PropertyChangedCallback((d, e) => { FontWeightPropertyChanged((HeadingBox)d, e); })));
        public static readonly DependencyProperty FontProperty = DependencyProperty.Register("Font", typeof(FontFamily), typeof(HeadingBox), new PropertyMetadata(new PropertyChangedCallback((d, e) => { FontPropertyChanged((HeadingBox)d, e); })));
        public static readonly DependencyProperty TrackProperty = DependencyProperty.Register("Track", typeof(double), typeof(HeadingBox), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { TrackPropertyChanged((HeadingBox)d, e); })));
        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register("Time", typeof(double), typeof(HeadingBox), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { TimePropertyChanged((HeadingBox)d, e); })));
        public static readonly DependencyProperty FuelProperty = DependencyProperty.Register("Fuel", typeof(double), typeof(HeadingBox), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { FuelPropertyChanged((HeadingBox)d, e); })));
        public static readonly DependencyProperty AltitudeProperty = DependencyProperty.Register("Altitude", typeof(double), typeof(HeadingBox), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { AltitudePropertyChanged((HeadingBox)d, e); })));
        public static readonly DependencyProperty TrackColorProperty = DependencyProperty.Register("TrackColor", typeof(Brush), typeof(HeadingBox), new PropertyMetadata(Brushes.Red, new PropertyChangedCallback((d, e) => { TrackColorPropertyChanged((HeadingBox)d, e); })));
        public static readonly DependencyProperty TimeColorProperty = DependencyProperty.Register("TimeColor", typeof(Brush), typeof(HeadingBox), new PropertyMetadata(Brushes.Black, new PropertyChangedCallback((d, e) => { TimeColorPropertyChanged((HeadingBox)d, e); })));
        public static readonly DependencyProperty FuelColorProperty = DependencyProperty.Register("FuelColor", typeof(Brush), typeof(HeadingBox), new PropertyMetadata(Brushes.Red, new PropertyChangedCallback((d, e) => { FuelColorPropertyChanged((HeadingBox)d, e); })));
        public static readonly DependencyProperty AltitudeColorProperty = DependencyProperty.Register("AltitudeColor", typeof(Brush), typeof(HeadingBox), new PropertyMetadata(Brushes.Black, new PropertyChangedCallback((d, e) => { AltitudeColorPropertyChanged((HeadingBox)d, e); })));
        public static readonly DependencyProperty IsFlippedProperty = DependencyProperty.Register("IsFlipped", typeof(bool), typeof(HeadingBox), new PropertyMetadata(false, new PropertyChangedCallback((d, e) => { IsFlippedPropertyChanged((HeadingBox)d, e); })));
        #endregion

        #region Property Fields
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
        public Brush BorderColor
        {
            get
            {
                return (Brush)GetValue(BorderColorProperty);
            }
            set
            {
                SetValue(BorderColorProperty, value);
            }
        }
        public double BorderThickness
        {
            get
            {
                return (double)GetValue(BorderThicknessProperty);
            }
            set
            {
                SetValue(BorderThicknessProperty, value);
            }
        }
        public Brush TriangleFill
        {
            get
            {
                return (Brush)GetValue(TriangleFillProperty);
            }
            set
            {
                SetValue(TriangleFillProperty, value);
            }
        }
        public double Width
        {
            get
            {
                return (double)GetValue(WidthProperty);
            }
            set
            {
                SetValue(WidthProperty, value);
            }
        }
        public double Height
        {
            get
            {
                return (double)GetValue(HeightProperty);
            }
            set
            {
                SetValue(HeightProperty, value);
            }
        }
        public double TriangleHeight
        {
            get
            {
                return (double)GetValue(TriangleHeightProperty);
            }
            set
            {
                SetValue(TriangleHeightProperty, value);
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
        public double Track
        {
            get
            {
                return (double)GetValue(TrackProperty);
            }
            set
            {
                SetValue(TrackProperty, value);
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
        public Brush FuelColor
        {
            get
            {
                return (Brush)GetValue(FuelColorProperty);
            }
            set
            {
                SetValue(FuelColorProperty, value);
            }
        }
        public Brush AltitudeColor
        {
            get
            {
                return (Brush)GetValue(AltitudeColorProperty);
            }
            set
            {
                SetValue(AltitudeColorProperty, value);
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
        #endregion

        #region Property Callback Functions
        private static void BackgroundPropertyChanged(HeadingBox obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                Brush color = (Brush)e.NewValue;
                obj.track.Background = color;
                obj.time.Background = color;
                obj.fuel.Background = color;
                obj.alt.Background = color;
            }
        }
        private static void BorderColorPropertyChanged(HeadingBox obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                Brush stroke = (Brush)e.NewValue;
                obj.triangle.Stroke = stroke;
                obj.track.BorderBrush = stroke;
                obj.time.BorderBrush = stroke;
                obj.fuel.BorderBrush = stroke;
                obj.alt.BorderBrush = stroke;
            }
        }
        private static void BorderThicknessPropertyChanged(HeadingBox obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double thickness = (double)e.NewValue;
                obj.trianglethickness = thickness;
                obj.triangle.StrokeThickness = thickness;
                obj.triangle.Data = Geometry.Parse(String.Format("M{0},{0} L{0},{2} {1},{2} Z", obj.trianglethickness / 2, obj.trianglewidth - (obj.trianglethickness / 2), obj.triangleheight - (obj.trianglethickness / 2)));
                obj.track.BorderThickness = new Thickness(thickness, 0, 0, thickness);
                obj.time.BorderThickness = new Thickness(thickness, 0, 0, thickness);
                obj.fuel.BorderThickness = new Thickness(thickness, 0, 0, thickness);
                obj.alt.BorderThickness = new Thickness(thickness, 0, 0, thickness);
            }
        }
        private static void TriangleFillPropertyChanged(HeadingBox obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                Brush color = (Brush)e.NewValue;
                obj.triangle.Fill = color;
            }
        }
        private static void WidthPropertyChanged(HeadingBox obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double width = (double)e.NewValue;
                obj.headingBox.Width = width;
                obj.trianglewidth = width;
                obj.triangle.Data = Geometry.Parse(String.Format("M{0},{0} L{0},{2} {1},{2} Z", obj.trianglethickness / 2, obj.trianglewidth - (obj.trianglethickness / 2), obj.triangleheight - (obj.trianglethickness / 2)));
            }
        }
        private static void HeightPropertyChanged(HeadingBox obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double height = (double)e.NewValue;
                obj.headingBox.Height = height + obj.triangleheight;
                obj.track.Height = height / 4;
                obj.time.Height = height / 4;
                obj.fuel.Height = height / 4;
                obj.alt.Height = height / 4;
            }
        }
        private static void TriangleHeightPropertyChanged(HeadingBox obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double height = (double)e.NewValue;
                obj.triangleheight = height;
                obj.headingBox.Height = obj.triangleheight + (obj.track.Height * 4);
                obj.triangle.Data = Geometry.Parse(String.Format("M{0},{0} L{0},{2} {1},{2} Z", obj.trianglethickness / 2, obj.trianglewidth - (obj.trianglethickness / 2), obj.triangleheight - (obj.trianglethickness / 2)));
            }
        }
        private static void FontSizePropertyChanged(HeadingBox obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double size = (double)e.NewValue;
                obj.track.FontSize = size;
                obj.time.FontSize = size;
                obj.fuel.FontSize = size;
                obj.alt.FontSize = size;
            }
        }
        private static void FontWeightPropertyChanged(HeadingBox obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                FontWeight weight = (FontWeight)e.NewValue;
                obj.track.FontWeight = weight;
                obj.time.FontWeight = weight;
                obj.fuel.FontWeight = weight;
                obj.alt.FontWeight = weight;
            }
        }
        private static void FontPropertyChanged(HeadingBox obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                FontFamily font = (FontFamily)e.NewValue;
                obj.track.FontFamily = font;
                obj.time.FontFamily = font;
                obj.fuel.FontFamily = font;
                obj.alt.FontFamily = font;
            }
        }
        private static void TrackPropertyChanged(HeadingBox obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double track = (double)e.NewValue;
                obj.track.Content = DataFormatter.FormatData(track, DataFormat.Short);
            }
        }
        private static void TimePropertyChanged(HeadingBox obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double time = (double)e.NewValue;
                obj.time.Content = DataFormatter.FormatTime(time, TimeFormat.Short);
            }
        }
        private static void FuelPropertyChanged(HeadingBox obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double fuel = (double)e.NewValue;
                obj.fuel.Content = DataFormatter.FormatData(fuel, DataFormat.Short);
            }
        }
        private static void AltitudePropertyChanged(HeadingBox obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double alt = (double)e.NewValue;
                obj.alt.Content = DataFormatter.FormatData(alt, DataFormat.Short);
            }
        }
        private static void TrackColorPropertyChanged(HeadingBox obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                Brush color = (Brush)e.NewValue;
                obj.track.Foreground = color;
            }
        }
        private static void TimeColorPropertyChanged(HeadingBox obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                Brush color = (Brush)e.NewValue;
                obj.time.Foreground = color;
            }
        }
        private static void FuelColorPropertyChanged(HeadingBox obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                Brush color = (Brush)e.NewValue;
                obj.fuel.Foreground = color;
            }
        }
        private static void AltitudeColorPropertyChanged(HeadingBox obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                Brush color = (Brush)e.NewValue;
                obj.alt.Foreground = color;
            }
        }
        private static void IsFlippedPropertyChanged(HeadingBox obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                bool isflipped = (bool)e.NewValue;
                var transform = obj.triangle.RenderTransform as ScaleTransform;
                if (isflipped)
                {
                    if (transform.ScaleX >= 0)
                    {
                        transform.ScaleX *= -1;
                        obj.track.BorderThickness = new Thickness(0, 0, obj.BorderThickness, obj.BorderThickness);
                        obj.time.BorderThickness = new Thickness(0, 0, obj.BorderThickness, obj.BorderThickness);
                        obj.fuel.BorderThickness = new Thickness(0, 0, obj.BorderThickness, obj.BorderThickness);
                        obj.alt.BorderThickness = new Thickness(0, 0, obj.BorderThickness, obj.BorderThickness);
                        obj.track.HorizontalContentAlignment = HorizontalAlignment.Right;
                        obj.time.HorizontalContentAlignment = HorizontalAlignment.Right;
                        obj.fuel.HorizontalContentAlignment = HorizontalAlignment.Right;
                        obj.alt.HorizontalContentAlignment = HorizontalAlignment.Right;
                    }
                }
                else
                {
                    if (transform.ScaleX <= 0)
                    {
                        transform.ScaleX *= -1;
                        obj.track.BorderThickness = new Thickness(obj.BorderThickness, 0, 0, obj.BorderThickness);
                        obj.time.BorderThickness = new Thickness(obj.BorderThickness, 0, 0, obj.BorderThickness);
                        obj.fuel.BorderThickness = new Thickness(obj.BorderThickness, 0, 0, obj.BorderThickness);
                        obj.alt.BorderThickness = new Thickness(obj.BorderThickness, 0, 0, obj.BorderThickness);
                        obj.track.HorizontalContentAlignment = HorizontalAlignment.Left;
                        obj.time.HorizontalContentAlignment = HorizontalAlignment.Left;
                        obj.fuel.HorizontalContentAlignment = HorizontalAlignment.Left;
                        obj.alt.HorizontalContentAlignment = HorizontalAlignment.Left;
                    }
                }
            }
        }
        #endregion

        #region Internal Fields
        private StackPanel headingBox;
        private Path triangle;
        private Label track, time, fuel, alt;
        private double trianglewidth, triangleheight, trianglethickness;
        #endregion

        #region Member Functions
        public HeadingBox()
        {
            CreateObject();
        }

        //Public Methods
        public bool isDrawn(StackPanel datapanel)
        {
            return datapanel.Children.Contains(headingBox);
        }
        public void Draw(StackPanel datapanel, double offset, int index = -1)
        {
            if (!isDrawn(datapanel))
            {
                headingBox.Margin = new Thickness(0, 0, 0, offset);
                if (index < 0) datapanel.Children.Add(headingBox);
                else datapanel.Children.Insert(index, headingBox);
            }
        }
        public void Undraw(StackPanel datapanel)
        {
            if (isDrawn(datapanel))
            {
                datapanel.Children.Remove(headingBox);
            }
        }
        public int GetIndex(StackPanel datapanel)
        {
            if (!isDrawn(datapanel)) return -1;
            else return datapanel.Children.IndexOf(headingBox);
        }

        //Private Methods
        private void ConstructGeometry()
        {
            trianglethickness = BorderThickness;
            trianglewidth = Width;
            triangleheight = TriangleHeight;
            headingBox = new StackPanel { Orientation = Orientation.Vertical, Width = Width, Background = Brushes.Transparent };
            triangle = new Path { Stroke = BorderColor, StrokeThickness = trianglethickness, Fill = TriangleFill, Data = Geometry.Parse($"M{trianglethickness / 2},{trianglethickness / 2} L{trianglethickness / 2},{triangleheight - (trianglethickness / 2)} {trianglewidth - (trianglethickness / 2)},{triangleheight - (trianglethickness / 2)} Z"), RenderTransformOrigin = new Point(0.4685, 0.5), RenderTransform = new ScaleTransform() };
            track = new Label() { Height = Height / 4, Foreground = TrackColor, Background = Background, BorderThickness = new Thickness(BorderThickness, 0, 0, BorderThickness), BorderBrush = BorderColor, FontSize = FontSize, FontWeight = FontWeight, VerticalContentAlignment = VerticalAlignment.Center, HorizontalContentAlignment = HorizontalAlignment.Left, Content = DataFormatter.FormatData(Track, DataFormat.Short) };
            time = new Label() { Height = Height / 4, Foreground = TimeColor, Background = Background, BorderThickness = new Thickness(BorderThickness, 0, 0, BorderThickness), BorderBrush = BorderColor, FontSize = FontSize, FontWeight = FontWeight, VerticalContentAlignment = VerticalAlignment.Center, HorizontalContentAlignment = HorizontalAlignment.Left, Content = DataFormatter.FormatTime(Time, TimeFormat.Short) };
            fuel = new Label() { Height = Height / 4, Foreground = FuelColor, Background = Background, BorderThickness = new Thickness(BorderThickness, 0, 0, BorderThickness), BorderBrush = BorderColor, FontSize = FontSize, FontWeight = FontWeight, VerticalContentAlignment = VerticalAlignment.Center, HorizontalContentAlignment = HorizontalAlignment.Left, Content = DataFormatter.FormatData(Track, DataFormat.Short) };
            alt = new Label() { Height = Height / 4, Foreground = AltitudeColor, Background = Background, BorderThickness = new Thickness(BorderThickness, 0, 0, BorderThickness), BorderBrush = BorderColor, FontSize = FontSize, FontWeight = FontWeight, VerticalContentAlignment = VerticalAlignment.Center, HorizontalContentAlignment = HorizontalAlignment.Left, Content = DataFormatter.FormatData(Track, DataFormat.Short) };
            headingBox.Children.Add(triangle);
            headingBox.Children.Add(track);
            headingBox.Children.Add(time);
            headingBox.Children.Add(fuel);
            headingBox.Children.Add(alt);
        }

        private void CreateObject()
        {
            ConstructGeometry();
        }
        #endregion
    }
}
