using GMap.NET;
using System.Windows;

namespace MissionAssistant
{
    partial class Circle
    {
        #region Dependency Property
        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register("Center", typeof(PointLatLng), typeof(Circle), new PropertyMetadata(PointLatLng.Empty, new PropertyChangedCallback((d, e) => { CenterPropertyChanged((Circle)d, e); })));
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(double), typeof(Circle), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { RadiusPropertyChanged((Circle)d, e); })));
        #endregion

        #region Property Fields
        public PointLatLng Center
        {
            get
            {
                return (PointLatLng)GetValue(CenterProperty);
            }
            set
            {
                SetValue(CenterProperty, value);
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
        #endregion

        #region Property Callback Functions
        private static void CenterPropertyChanged(Circle obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                PointLatLng pos = (PointLatLng)e.NewValue;

                //Update Local
                if (!obj.restrictCenterUpdate)
                {
                    obj.restrictCenterUpdate = !obj.restrictCenterUpdate;
                    obj.LocalCenter = DataCalculations.GetPhysicalFromLatLng(pos);
                }
                else obj.restrictCenterUpdate = !obj.restrictCenterUpdate;
            }
        }
        private static void RadiusPropertyChanged(Circle obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double radius = (double)e.NewValue;

                //Update Local
                if (!obj.restrictRadiusUpdate)
                {
                    obj.restrictRadiusUpdate = !obj.restrictRadiusUpdate;
                    PointLatLng perimeter = DataCalculations.GetOffset(obj.Center, radius, 0);
                    double localdistance = DataCalculations.GetLocalDistance(DataCalculations.GetPhysicalFromLatLng(obj.Center), DataCalculations.GetPhysicalFromLatLng(perimeter));
                    obj.LocalRadius = localdistance;
                }
                else obj.restrictRadiusUpdate = !obj.restrictRadiusUpdate;
            }
        }
        #endregion
    }
}
