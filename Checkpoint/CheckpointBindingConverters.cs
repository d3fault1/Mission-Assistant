using GMap.NET;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;

namespace MissionAssistant
{
    class CheckpointMarginConverter2 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double width = (double)values[0];
            double height = (double)values[1];
            double linelength = (double)values[2];
            Line marker = (Line)values[3];
            StackPanel holder = (StackPanel)parameter;
            Point translated = marker.TranslatePoint(new Point(0, 0), holder);
            double offsetLeft = translated.X + (linelength / 2);
            double offsetTop = height / 2;
            double offsetBottom = offsetTop;
            double offsetRight = width - offsetLeft;
            return new Thickness(-offsetLeft, -offsetTop, -offsetRight, -offsetBottom);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
    class CheckpointRenderOriginConverter2 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double width = (double)values[0];
            double linelength = (double)values[1];
            Line marker = (Line)values[2];
            StackPanel holder = (StackPanel)parameter;
            Point translated = marker.TranslatePoint(new Point(0, 0), holder);
            double percentageX = (translated.X + (linelength / 2)) / width;
            return new Point(percentageX, 0.5);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    class CheckpointOffsetConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double distance = (double)values[0];
            double maxY = (double)values[1];
            Canvas holder = (Canvas)parameter;
            double mid = holder.ActualWidth / 2;
            var pos = GetPosition(holder, (int)mid, (int)maxY, distance);
            return (double)pos;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { Binding.DoNothing, Binding.DoNothing };
        }

        public int GetPosition(Canvas canvas, int midX, int range, double key)
        {
            int min = 0, max = range, mid;
            double res;
            var reference = DataCalculations.GetLatLngFromPhysical(new Point(midX, range), canvas);
            while (min <= max)
            {
                mid = (min + max) / 2;
                var point = DataCalculations.GetLatLngFromPhysical(new Point(midX, range - mid), canvas);
                res = DataCalculations.GetDistance(reference, point) - key;
                if (res < 0) min = mid + 1;
                else if (res > 0) max = mid - 1;
                else return mid;
            }
            return max;
        }
    }
    class OutOfBoundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is Marker)
            {
                double maxdist;
                double distance = (double)values[0];
                double otherdistance = (double)values[1];
                double legdistance = (double)values[2];
                maxdist = distance + otherdistance;
                if (distance <= 2 || distance >= (int)legdistance - 2) return false;
                else if ((int)legdistance > maxdist) return true;
                else return false;
            }
            else if ((int)parameter == 1)
            {
                double legdist = (double)values[0];
                double distance = (double)values[1];
                if (distance <= 2 || distance >= (int)legdist - 2) return false;
                else return true;
            }
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
