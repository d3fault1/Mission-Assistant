using System.ComponentModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using GMap.NET;

namespace MissionAssistant
{
    partial class Circle : DependencyObject
    {
        #region Dependency Properties
        public static readonly DependencyProperty LocalCenterProperty = DependencyProperty.Register("LocalCenter", typeof(Point), typeof(Circle), new PropertyMetadata(new PropertyChangedCallback((d, e) => { LocalCenterPropertyChanged((Circle)d, e); })));
        public static readonly DependencyProperty LocalRadiusProperty = DependencyProperty.Register("LocalRadius", typeof(double), typeof(Circle), new PropertyMetadata(new PropertyChangedCallback((d, e) => { LocalRadiusPropertyChanged((Circle)d, e); })));
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Brush), typeof(Circle), new PropertyMetadata(Brushes.White, new PropertyChangedCallback((d, e) => { ColorPropertyChanged((Circle)d, e); })));
        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register("Thickness", typeof(double), typeof(Circle), new PropertyMetadata(2.0, new PropertyChangedCallback((d, e) => { ThicknessPropertyChanged((Circle)d, e); })));
        public static readonly DependencyProperty StrokeDashArrayProperty = DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(Circle), new PropertyMetadata(new DoubleCollection { 1, 0 }, new PropertyChangedCallback((d, e) => { StrokeDashArrayPropertyChanged((Circle)d, e); })));
        public static readonly DependencyProperty RestrictPositionUpdateProperty = DependencyProperty.Register("RestrictPositionUpdate", typeof(bool), typeof(Circle), new PropertyMetadata(false, new PropertyChangedCallback((d, e) => { RestrictPositionUpdatePropertyChanged((Circle)d, e); })));
        #endregion

        #region Property Fields
        public Point LocalCenter
        {
            get
            {
                return (Point)GetValue(LocalCenterProperty);
            }
            set
            {
                SetValue(LocalCenterProperty, value);
            }
        }
        public double LocalRadius
        {
            get
            {
                return (double)GetValue(LocalRadiusProperty);
            }
            set
            {
                SetValue(LocalRadiusProperty, value);
            }
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
        public double Thickness
        {
            get
            {
                return (double)GetValue(ThicknessProperty);
            }
            set
            {
                SetValue(ThicknessProperty, value);
            }
        }
        public DoubleCollection StrokeDashArray
        {
            get
            {
                return (DoubleCollection)GetValue(StrokeDashArrayProperty);
            }
            set
            {
                SetValue(StrokeDashArrayProperty, value);
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
        private static void LocalCenterPropertyChanged(Circle obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidType(e.NewValue))
            {
                Point pos = (Point)e.NewValue;

                //Update Canvas
                if (!obj.restrictCanvasCenterLoop)
                {
                    obj.restrictCanvasCenterLoop = !obj.restrictCanvasCenterLoop;
                    Canvas.SetLeft(obj.circle, pos.X);
                    obj.restrictCanvasCenterLoop = !obj.restrictCanvasCenterLoop;
                    Canvas.SetTop(obj.circle, pos.Y);
                }
                else obj.restrictCanvasCenterLoop = !obj.restrictCanvasCenterLoop;

                //Update Data
                if (!obj.RestrictPositionUpdate)
                {
                    if (!obj.restrictCenterUpdate)
                    {
                        obj.restrictCenterUpdate = !obj.restrictCenterUpdate;
                        obj.Center = DataCalculations.GetLatLngFromPhysical(pos);
                    }
                    else obj.restrictCenterUpdate = !obj.restrictCenterUpdate;
                }
            }
        }
        private static void LocalRadiusPropertyChanged(Circle obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double radius = (double)e.NewValue;

                obj.circle.Width = radius * 2;

                //Update Data
                if (!obj.restrictRadiusUpdate)
                {
                    obj.restrictRadiusUpdate = !obj.restrictRadiusUpdate;
                    Point perimeter = new Point(obj.LocalCenter.X, obj.LocalCenter.Y - radius);
                    obj.Radius = DataCalculations.GetDistance(obj.Center, DataCalculations.GetLatLngFromPhysical(perimeter));
                }
                else obj.restrictRadiusUpdate = !obj.restrictRadiusUpdate;
            }
        }
        private static void ColorPropertyChanged(Circle obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                Brush color = (Brush)e.NewValue;
                obj.circle.Stroke = color;
            }
        }
        private static void ThicknessPropertyChanged(Circle obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double thickness = (double)e.NewValue;
                obj.circle.StrokeThickness = thickness;
            }
        }
        private static void StrokeDashArrayPropertyChanged(Circle obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                DoubleCollection pattern = (DoubleCollection)e.NewValue;
                obj.circle.StrokeDashArray = pattern;
            }
        }
        private static void RestrictPositionUpdatePropertyChanged(Circle obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                bool isrestricted = (bool)e.NewValue;
                if (!isrestricted)
                {
                    //Update CenterPoint
                    if (!obj.restrictCenterUpdate)
                    {
                        obj.restrictCenterUpdate = !obj.restrictCenterUpdate;
                        obj.Center = DataCalculations.GetLatLngFromPhysical(obj.LocalCenter);
                    }
                    else obj.restrictCenterUpdate = !obj.restrictCenterUpdate;
                }
                else
                {

                }
            }
        }
        #endregion

        #region Internal Members
        private Ellipse circle;
        private bool restrictCenterUpdate = false, restrictRadiusUpdate = false, restrictCanvasCenterLoop = false;
        #endregion

        #region Member Functions
        public Circle()
        {
            CreateObject();
        }

        public virtual void Draw(Canvas canvas)
        {
            if (!isDrawn(canvas))
            {
                canvas.Children.Add(circle);
            }
        }
        public virtual void Undraw(Canvas canvas)
        {
            if (isDrawn(canvas))
            {
                canvas.Children.Remove(circle);
            }
        }
        public virtual bool isDrawn(Canvas canvas)
        {
            return canvas.Children.Contains(circle);
        }

        public void UpdateLocalParameters()
        {
            restrictCenterUpdate = true;
            restrictRadiusUpdate = true;
            LocalCenter = DataCalculations.GetPhysicalFromLatLng(Center);

            PointLatLng perimeter = DataCalculations.GetOffset(Center, Radius, 0);
            double localdistance = DataCalculations.GetLocalDistance(DataCalculations.GetPhysicalFromLatLng(Center), DataCalculations.GetPhysicalFromLatLng(perimeter));
            LocalRadius = localdistance;

            restrictCenterUpdate = false;
            restrictRadiusUpdate = false;
        }
        protected virtual void ConstructGeometry()
        {
            circle = new Ellipse
            {
                Stroke = Color,
                StrokeThickness = Thickness,
                StrokeDashArray = StrokeDashArray
            };
            circle.SetBinding(Ellipse.HeightProperty, new Binding { Source = circle, Path = new PropertyPath(Ellipse.WidthProperty), Converter = new NullHandleDoubleConverter() });
            circle.SetBinding(Ellipse.MarginProperty, new Binding { Source = circle, Path = new PropertyPath(Ellipse.WidthProperty), Converter = new EllipseMarginConverter() });

            var left = DependencyPropertyDescriptor.FromProperty(Canvas.LeftProperty, typeof(Canvas));
            var top = DependencyPropertyDescriptor.FromProperty(Canvas.TopProperty, typeof(Canvas));
            left.AddValueChanged(circle, new EventHandler(CanvasCenterPosChanged));
            top.AddValueChanged(circle, new EventHandler(CanvasCenterPosChanged));
        }

        private void CreateObject()
        {
            ConstructGeometry();
            circle.Tag = new object[] { this, 0 };
        }
        private void CanvasCenterPosChanged(object sender, EventArgs e)
        {
            if (!restrictCanvasCenterLoop)
            {
                restrictCanvasCenterLoop = !restrictCanvasCenterLoop;
                LocalCenter = new Point(Canvas.GetLeft((UIElement)sender), Canvas.GetTop((UIElement)sender));
            }
            else restrictCanvasCenterLoop = !restrictCanvasCenterLoop;
        }
        #endregion
    }
}
