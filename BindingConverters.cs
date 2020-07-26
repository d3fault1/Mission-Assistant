using System;
using System.Globalization;
using System.Windows.Data;

namespace Mission_Assistant
{
    class HeadingBoxConverterX : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double X1 = (double)values[0];
            double Y1 = (double)values[1];
            double X2 = (double)values[2];
            double Y2 = (double)values[3];
            double offset = (values[4] as RouteData).offset;
            double width = (double)values[5];
            return ((X1 + X2) / 2) + (width / 2 + offset) * Math.Cos((Math.PI / 180) * 90 + Math.Atan2((Y2 - Y1), (X2 - X1)));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class HeadingBoxConverterY : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double X1 = (double)values[0];
            double Y1 = (double)values[1];
            double X2 = (double)values[2];
            double Y2 = (double)values[3];
            double offset = (values[4] as RouteData).offset;
            double width = (double)values[5];
            return ((Y1 + Y2) / 2) + (width / 2 + offset) * Math.Sin((Math.PI / 180) * 90 + Math.Atan2((Y2 - Y1), (X2 - X1)));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class HeadingBoxConverterθ : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double X1 = (double)values[0];
            double Y1 = (double)values[1];
            double X2 = (double)values[2];
            double Y2 = (double)values[3];
            return (((180 / Math.PI) * (Math.Atan2((Y2 - Y1), (X2 - X1)))) + 90);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class ArrowConverterXY : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double XY1 = (double)values[0];
            double XY2 = (double)values[1];
            return XY1 + ((XY2 - XY1) * 4 / 5);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class ArrowConverterθ : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double X1 = (double)values[0];
            double Y1 = (double)values[1];
            double X2 = (double)values[2];
            double Y2 = (double)values[3];
            return Math.Atan2((Y2 - Y1), (X2 - X1)) * 180 / Math.PI;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class DistanceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double X1 = (double)values[0];
            double Y1 = (double)values[1];
            double X2 = (double)values[2];
            double Y2 = (double)values[3];
            return ((180 / Math.PI) * (Math.Atan2((Y2 - Y1), (X2 - X1))));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class CircleRadiusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double width = System.Convert.ToDouble(value);
            return -(width / 2);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan tval = TimeSpan.FromSeconds(System.Convert.ToDouble(value));
            return String.Format("{0:00}", tval.Minutes) + ":" + String.Format("{0:00}", tval.Seconds);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class HeadingBoxInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            RouteData info = value as RouteData;
            switch (parameter as String)
            {
                case "track":
                    return info.track;
                case "time":
                    TimeSpan ts = TimeSpan.FromSeconds(info.time);
                    return String.Format($"{ts.Hours.ToString().PadLeft(2, '0')}:{ts.Minutes.ToString().PadLeft(2, '0')}:{ts.Seconds.ToString().PadLeft(2, '0')}");
                case "fuel":
                    return Math.Round(info.fuel, 2);
                case "alt":
                    return info.alt;
            }
            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
