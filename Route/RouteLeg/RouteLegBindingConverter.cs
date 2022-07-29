using System;
using System.Windows;
using System.Globalization;
using System.Windows.Data;

namespace MissionAssistant
{
    class PanelHolderHeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double X1 = (double)values[0];
            double Y1 = (double)values[1];
            double X2 = (double)values[2];
            double Y2 = (double)values[3];
            return DataCalculations.GetLocalDistance(new Point(X1, Y1), new Point(X2, Y2));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    class DataCanvasConverterX : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double X1 = (double)values[0];
            double X2 = (double)values[1];
            return (X1 + X2) / 2;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class DataCanvasConverterY : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double Y1 = (double)values[0];
            double Y2 = (double)values[1];
            return (Y1 + Y2) / 2;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class DataCanvasConverterθ : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double X1 = (double)values[0];
            double Y1 = (double)values[1];
            double X2 = (double)values[2];
            double Y2 = (double)values[3];
            return (180 / Math.PI * Math.Atan2(Y2 - Y1, X2 - X1)) + 90;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class TODMarkingParamConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var val1 = (double)values[0];
            var val2 = (double)values[1];
            return val1 + val2;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class MarkingTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SegmentType type = (SegmentType)value;
            if (type == SegmentType.Ascend) return MarkerType.TOC;
            else if (type == SegmentType.Descend) return MarkerType.TOD;
            else return MarkerType.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class DiversionLegFlipConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var width = (double)values[0];
            var height = (double)values[1];
            double x = (height / 2) / width;
            double y = 0.5;
            return new Point(x, y);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
