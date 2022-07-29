using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MissionAssistant
{
    class FuelCircle : DependencyObject
    {
        #region Dependency Properties
        //Internal Dependency Properties
        public static readonly DependencyProperty BorderColorProperty = DependencyProperty.Register("BorderColor", typeof(Brush), typeof(FuelCircle), new PropertyMetadata(Brushes.Red, new PropertyChangedCallback((d, e) => { BorderColorPropertyChanged((FuelCircle)d, e); })));
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(FuelCircle), new PropertyMetadata(Brushes.DarkGray, new PropertyChangedCallback((d, e) => { BackgroundPropertyChanged((FuelCircle)d, e); })));
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(double), typeof(FuelCircle), new PropertyMetadata(60.0, new PropertyChangedCallback((d, e) => { RadiusPropertyChanged((FuelCircle)d, e); })));
        public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(double), typeof(FuelCircle), new PropertyMetadata(2.0, new PropertyChangedCallback((d, e) => { BorderThicknessPropertyChanged((FuelCircle)d, e); })));
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(FuelCircle), new PropertyMetadata(22.0, new PropertyChangedCallback((d, e) => { FontSizePropertyChanged((FuelCircle)d, e); })));
        public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(FuelCircle), new PropertyMetadata(FontWeights.Bold, new PropertyChangedCallback((d, e) => { FontWeightPropertyChanged((FuelCircle)d, e); })));
        public static readonly DependencyProperty FontProperty = DependencyProperty.Register("Font", typeof(FontFamily), typeof(FuelCircle), new PropertyMetadata(new PropertyChangedCallback((d, e) => { FontPropertyChanged((FuelCircle)d, e); })));
        public static readonly DependencyProperty RemainingFuelProperty = DependencyProperty.Register("RemainingFuel", typeof(double), typeof(FuelCircle), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { RemainingFuelPropertyChanged((FuelCircle)d, e); })));
        public static readonly DependencyProperty SuggestedFuelProperty = DependencyProperty.Register("SuggestedFuel", typeof(double), typeof(FuelCircle), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { SuggestedFuelPropertyChanged((FuelCircle)d, e); })));
        public static readonly DependencyProperty RemainingFuelColorProperty = DependencyProperty.Register("RemainingFuelColor", typeof(Brush), typeof(FuelCircle), new PropertyMetadata(Brushes.Black, new PropertyChangedCallback((d, e) => { RemainingFuelColorPropertyChanged((FuelCircle)d, e); })));
        public static readonly DependencyProperty SuggestedFuelColorProperty = DependencyProperty.Register("SuggestedFuelColor", typeof(Brush), typeof(FuelCircle), new PropertyMetadata(Brushes.Red, new PropertyChangedCallback((d, e) => { SuggestedFuelColorPropertyChanged((FuelCircle)d, e); })));
        #endregion

        #region Property Fields
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
        public double Radius
        {
            get
            {
                return (double)GetValue(RadiusProperty);
            }
            set
            {
                SetValue(RadiusProperty, value);
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
        public double RemainingFuel
        {
            get
            {
                return (double)GetValue(RemainingFuelProperty);
            }
            set
            {
                SetValue(RemainingFuelProperty, value);
            }
        }
        public double SuggestedFuel
        {
            get
            {
                return (double)GetValue(SuggestedFuelProperty);
            }
            set
            {
                SetValue(SuggestedFuelProperty, value);
            }
        }
        public Brush RemainingFuelColor
        {
            get
            {
                return (Brush)GetValue(RemainingFuelColorProperty);
            }
            set
            {
                SetValue(RemainingFuelColorProperty, value);
            }
        }
        public Brush SuggestedFuelColor
        {
            get
            {
                return (Brush)GetValue(SuggestedFuelColorProperty);
            }
            set
            {
                SetValue(SuggestedFuelColorProperty, value);
            }
        }
        #endregion

        #region Property Callback Functions
        private static void BorderColorPropertyChanged(FuelCircle obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                Brush stroke = (Brush)e.NewValue;
                obj.border.Stroke = stroke;
                obj.remfuel.BorderBrush = stroke;
                obj.recfuel.BorderBrush = stroke;
            }
        }
        private static void BackgroundPropertyChanged(FuelCircle obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                Brush background = (Brush)e.NewValue;
                obj.border.Fill = background;
            }
        }
        private static void RadiusPropertyChanged(FuelCircle obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double diameter = ((double)e.NewValue) * 2;
                obj.fuelcircle.Width = diameter;
                obj.fuelcircle.Height = diameter;
            }
        }
        private static void BorderThicknessPropertyChanged(FuelCircle obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double thickness = (double)e.NewValue;
                obj.border.StrokeThickness = thickness;
                obj.remfuel.BorderThickness = new Thickness(0, 0, 0, thickness / 2);
                obj.recfuel.BorderThickness = new Thickness(0, thickness / 2, 0, 0);
            }
        }
        private static void FontSizePropertyChanged(FuelCircle obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double size = (double)e.NewValue;
                obj.remfuel.FontSize = size;
                obj.recfuel.FontSize = size;
            }
        }
        private static void FontWeightPropertyChanged(FuelCircle obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                FontWeight weight = (FontWeight)e.NewValue;
                obj.remfuel.FontWeight = weight;
                obj.recfuel.FontWeight = weight;
            }
        }
        private static void FontPropertyChanged(FuelCircle obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                FontFamily font = (FontFamily)e.NewValue;
                obj.remfuel.FontFamily = font;
                obj.recfuel.FontFamily = font;
            }
        }
        private static void RemainingFuelPropertyChanged(FuelCircle obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double val = (double)e.NewValue;
                obj.remfuel.Content = DataFormatter.FormatData(val, DataFormat.Round);
            }
        }
        private static void SuggestedFuelPropertyChanged(FuelCircle obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double val = (double)e.NewValue;
                obj.recfuel.Content = DataFormatter.FormatData(val, DataFormat.Round);
            }
        }
        private static void RemainingFuelColorPropertyChanged(FuelCircle obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                Brush color = (Brush)e.NewValue;
                obj.remfuel.Foreground = color;
            }
        }
        private static void SuggestedFuelColorPropertyChanged(FuelCircle obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                Brush color = (Brush)e.NewValue;
                obj.recfuel.Foreground = color;
            }
        }
        #endregion

        #region Internal Fields
        private Grid fuelcircle;
        private Ellipse border;
        private StackPanel contentpanel;
        private Label remfuel, recfuel;
        #endregion

        #region Public Fields
        #endregion

        #region Member Functions
        public FuelCircle()
        {
            CreateObject();
        }

        //Public Methods
        public bool isDrawn(StackPanel datapanel)
        {
            return datapanel.Children.Contains(fuelcircle);
        }
        public void Draw(StackPanel datapanel, double offset, int index = -1)
        {
            fuelcircle.Margin = new Thickness(0, 0, 0, offset);
            if (index < 0) datapanel.Children.Add(fuelcircle);
            else datapanel.Children.Insert(index, fuelcircle);
        }
        public void Undraw(StackPanel datapanel)
        {
            if (isDrawn(datapanel))
            {
                datapanel.Children.Remove(fuelcircle);
            }
        }
        public int GetIndex(StackPanel datapanel)
        {
            if (!isDrawn(datapanel)) return -1;
            else return datapanel.Children.IndexOf(fuelcircle);
        }

        //Private Methods
        private void ConstructGeometry()
        {
            double diameter = Radius * 2;
            fuelcircle = new Grid() { Background = Brushes.Transparent, Width = diameter, Height = diameter };
            fuelcircle.ColumnDefinitions.Add(new ColumnDefinition());
            fuelcircle.RowDefinitions.Add(new RowDefinition());
            contentpanel = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Center };
            remfuel = new Label { Content = RemainingFuel, HorizontalContentAlignment = HorizontalAlignment.Center, Foreground = RemainingFuelColor, FontSize = FontSize, FontWeight = FontWeight, Margin = new Thickness(0, 5, 0, 0), BorderBrush = BorderColor, BorderThickness = new Thickness(0, 0, 0, BorderThickness / 2) };
            recfuel = new Label { Content = SuggestedFuel, HorizontalContentAlignment = HorizontalAlignment.Center, Foreground = SuggestedFuelColor, FontSize = FontSize, FontWeight = FontWeight, Margin = new Thickness(0, 0, 0, 5), BorderBrush = BorderColor, BorderThickness = new Thickness(0, BorderThickness / 2, 0, 0) };
            contentpanel.Children.Add(remfuel);
            contentpanel.Children.Add(recfuel);
            border = new Ellipse { Stroke = BorderColor, Fill = Background, StrokeThickness = BorderThickness };
            Grid.SetRow(border, 0);
            Grid.SetColumn(border, 0);
            Grid.SetRow(contentpanel, 0);
            Grid.SetColumn(contentpanel, 0);
            fuelcircle.Children.Add(border);
            fuelcircle.Children.Add(contentpanel);
        }

        private void CreateObject()
        {
            ConstructGeometry();
        }
        #endregion
    }
}
