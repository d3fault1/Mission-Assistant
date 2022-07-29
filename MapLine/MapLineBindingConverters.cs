using GMap.NET;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MissionAssistant
{
    class NullHandleDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DataValidation.isNumberValid((double)value))
            {
                return value;
            }
            else return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DataValidation.isNumberValid((double)value))
            {
                return value;
            }
            else return Binding.DoNothing;
        }
    }

    class CanvasPosToLocalPointConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            object retval;
            if (DataValidation.isNumberValid((double)values[0]) && DataValidation.isNumberValid((double)values[0]))
            {
                double X = (double)values[0];
                double Y = (double)values[1];
                retval = new Point(X, Y);
            }
            else retval = Binding.DoNothing;
            return retval;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            object[] retval;
            if ((Point)value != new Point(0, 0))
            {
                Point local = (Point)value;
                retval = new object[] { local.X, local.Y };
            }
            else retval = new object[] { Binding.DoNothing, Binding.DoNothing };
            return retval;
        }
    }

    class LocalToMapPointTwoWayConverter : IValueConverter
    {
        bool restrictUpdate = false;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object retval;
            if ((Point)value != new Point(0, 0))
            {
                if (!restrictUpdate)
                {
                    Point local = (Point)value;
                    retval = DataCalculations.GetLatLngFromPhysical(local);
                }
                else retval = Binding.DoNothing;
                restrictUpdate = !restrictUpdate;
            }
            else retval = Binding.DoNothing;
            return retval;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object retval;
            if ((PointLatLng)value != PointLatLng.Empty)
            {
                if (!restrictUpdate)
                {
                    PointLatLng geo = (PointLatLng)value;
                    retval = DataCalculations.GetPhysicalFromLatLng(geo);
                }
                else retval = Binding.DoNothing;
                restrictUpdate = !restrictUpdate;
            }
            else retval = Binding.DoNothing;
            return retval;
        }
    }
}
