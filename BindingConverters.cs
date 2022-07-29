using System;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Diagnostics;
using System.Collections.Generic;

namespace MissionAssistant
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
            RouteData2 dat = values[1] as RouteData2;
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
            if (parameter == "MarkingCD")
            {
                TimeSpan tval = TimeSpan.FromSeconds((value as RouteData2).tocbodtime);
                if (tval == null || tval == TimeSpan.Zero) return String.Format($"N/A");
                return String.Format($"{(int)tval.TotalMinutes}'{tval.Seconds}\"");
            }
            else if (parameter == "MarkingL")
            {
                TimeSpan tval = TimeSpan.FromSeconds((value as RouteData2).landingtime);
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
            RouteData2 info = value as RouteData2;
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
            RouteData2 data = value as RouteData2;
            if (data.transition == "climb") return @"TOC";
            else if (data.transition == "descend") return @"BOD";
            else return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    class ComboBoxEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int)value;
            if (index == -1) return false;
            else return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
