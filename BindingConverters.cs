using System;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Diagnostics;
using System.Collections.Generic;

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
            return (X1 + X2) / 2;
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
            return (Y1 + Y2) / 2;
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

    class ArrowPositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double length = System.Convert.ToDouble(value);
            return length * 4 / 5;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
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

    class EllipseMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double width = System.Convert.ToDouble(value);
            return new Thickness(-(width / 2), -(width / 2), -(width / 2), -(width / 2));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class RectangleMarginConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double width = System.Convert.ToDouble(values[0]);
            double height = System.Convert.ToDouble(values[1]);
            return new Thickness(-(width / 2), -(height / 2), -(width / 2), -(height / 2));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    class MiddlePositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double param = System.Convert.ToDouble(value);
            return param / 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class DataPanelMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double height = System.Convert.ToDouble(value);
            return new Thickness(0, -(height / 2), 0, -(height / 2));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class DataPanelContainerWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double width = System.Convert.ToDouble(value);
            return width * 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class DataPanelContainerHeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double X1 = System.Convert.ToDouble(values[0]);
            double Y1 = System.Convert.ToDouble(values[1]);
            double X2 = System.Convert.ToDouble(values[2]);
            double Y2 = System.Convert.ToDouble(values[3]);
            return Math.Sqrt(Math.Pow(X2 - X1, 2) + Math.Pow(Y2 - Y1, 2));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    class DataPanelOffsetConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double thickness = System.Convert.ToDouble(values[0]);
            RouteData dat = values[1] as RouteData;
            double offsetX = dat.offsetX;
            double offsetY = dat.offsetY;
            return new Thickness(thickness / 2 + offsetX, offsetY, 0, 0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == "Checkpoint")
            {
                TimeSpan tval = TimeSpan.FromSeconds((value as CheckpointData).time);
                if (tval == null || tval == TimeSpan.Zero) return String.Format($"N/A");
                return String.Format($"{(int)tval.TotalMinutes}'{tval.Seconds}\"");
            }
            else if (parameter == "MarkingCD")
            {
                TimeSpan tval = TimeSpan.FromSeconds((value as RouteData).tocbodtime);
                if (tval == null || tval == TimeSpan.Zero) return String.Format($"N/A");
                return String.Format($"{(int)tval.TotalMinutes}'{tval.Seconds}\"");
            }
            else if (parameter == "MarkingL")
            {
                TimeSpan tval = TimeSpan.FromSeconds((value as RouteData).landingtime);
                if (tval == null || tval == TimeSpan.Zero) return String.Format($"N/A");
                return String.Format($"{(int)tval.TotalMinutes}'{tval.Seconds}\"");
            }
            else
            {
                TimeSpan tval = TimeSpan.FromSeconds(System.Convert.ToDouble(value));
                return String.Format("{0:00}", tval.Minutes) + ":" + String.Format("{0:00}", tval.Seconds);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class RouteInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            RouteData info = value as RouteData;
            if (info == null) return String.Empty;
            switch (parameter as String)
            {
                case "track":
                    return String.Format($"{Math.Round(info.track, 2)}°");
                case "time":
                    TimeSpan ts = TimeSpan.FromSeconds(info.time);
                    if (ts == null || ts == TimeSpan.Zero) return String.Format($"N/A");
                    return String.Format($"{(int)ts.TotalMinutes}'{ts.Seconds}\"");
                case "fuel":
                    return String.Format($"{Math.Round(info.fuel, 0)} {info.baseFuelunit}");
                case "alt":
                    return String.Format($"{Math.Round(info.alt, 2)} {info.baseAltunit}");
                case "rem":
                    return Math.Round(info.remfuel, 0);
                case "frcs":
                    return Math.Round(info.frcsfuel, 0);
                case "rem2":
                    return Math.Round(info.landingrem, 0);
                case "frcs2":
                    return Math.Round(info.landingfrcs, 0);
            }
            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class CheckpointMarginConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double height = System.Convert.ToDouble(values[0]);
            double linelength = System.Convert.ToDouble(values[1]);
            return new Thickness(-linelength / 2, -height / 2, 0, -height / 2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    class CheckpointRenderOriginConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double linelength = System.Convert.ToDouble(values[0]);
            double Width = System.Convert.ToDouble(values[1]);
            return new Point(linelength / (2 * Width), 0.5);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    class CheckpointPositionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            GMapControl map = parameter as GMapControl;
            //double X1 = System.Convert.ToDouble(values[0]);
            //double Y1 = System.Convert.ToDouble(values[1]);
            //double X2 = System.Convert.ToDouble(values[2]);
            //double Y2 = System.Convert.ToDouble(values[3]);
            double d = (values[4] as CheckpointData).distance;
            //PointLatLng pos1 = map.FromLocalToLatLng((int)X1, (int)Y1);
            //PointLatLng pos2 = map.FromLocalToLatLng((int)X2, (int)Y2);
            //double R = 6378.1;
            //double track = Math.Atan2(Math.Cos(pos2.Lat * Math.PI / 180) * Math.Sin((pos2.Lng - pos1.Lng) * Math.PI / 180), Math.Cos(pos1.Lat * Math.PI / 180) * Math.Sin(pos2.Lat * Math.PI / 180) - Math.Sin(pos1.Lat * Math.PI / 180) * Math.Cos(pos2.Lat * Math.PI / 180) * Math.Cos((pos2.Lng - pos1.Lng) * Math.PI / 180));
            //double lat2 = Math.Asin(Math.Sin(pos1.Lat * Math.PI / 180) * Math.Cos(d / R) + Math.Cos(pos1.Lat * Math.PI / 180) * Math.Sin(d / R) * Math.Cos(track)) * 180 / Math.PI;
            //double lng2 = pos1.Lng + Math.Atan2(Math.Sin(track) * Math.Sin(d / R) * Math.Cos(pos1.Lat * Math.PI / 180), Math.Cos(d / R) - (Math.Sin(pos1.Lat * Math.PI / 180) * Math.Sin(lat2 * Math.PI / 180))) * 180 / Math.PI;
            //GPoint temp = map.FromLatLngToLocal(new PointLatLng(lat2, lng2));
            //Point local1 = new Point(X1, Y1);
            //Point local2 = new Point((int)temp.X, (int)temp.Y);
            //return Math.Sqrt(Math.Pow(local2.X - local1.X, 2) + Math.Pow(local2.Y - local1.Y, 2));
            PointLatLng pnt1 = map.FromLocalToLatLng(0, 20);
            PointLatLng pnt2 = map.FromLocalToLatLng(200, 20);
            double dist = new MapRoute(new List<PointLatLng>() { pnt1, pnt2 }, "B").Distance;
            double factor = 200 / dist;
            return d * factor;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    class CheckpointXConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double width = System.Convert.ToDouble(value);
            return width / 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    class CheckpointLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double thickness = System.Convert.ToDouble(value);
            return 20 + thickness;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    class CheckpointLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            RouteData data = value as RouteData;
            if (data.transition == "climb") return @"TOC";
            else if (data.transition == "descend") return @"BOD";
            else return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    class OutOfBoundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter.ToString() == "Checkpoint")
            {
                double pos = System.Convert.ToDouble(values[0]);
                double height = System.Convert.ToDouble(values[1]);
                if (pos <= 0 || pos >= height) return Visibility.Hidden;
                else return Visibility.Visible;
            }
            else if (parameter is GMapControl)
            {
                GMapControl map = parameter as GMapControl;
                double height = System.Convert.ToDouble(values[0]);
                RouteData dat = values[1] as RouteData;
                double pos = System.Convert.ToDouble(values[2]);
                double climb = dat.neffectivedst;
                double descend = DataConverters.LengthUnits(dat.distance, dat.baseDistunit, "KM") - dat.effectivedst;
                PointLatLng pnt1 = map.FromLocalToLatLng(0, 20);
                PointLatLng pnt2 = map.FromLocalToLatLng(200, 20);
                double dist = new MapRoute(new List<PointLatLng>() { pnt1, pnt2 }, "B").Distance;
                double factor = dist / 200;
                if (height * factor < climb + descend) return Visibility.Hidden;
                else if (pos == 0) return Visibility.Hidden;
                else return Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
