using GMap.NET;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MissionAssistant
{
    class UIColorComboboxConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return typeof(Brushes).GetProperties().FirstOrDefault(b => b.GetValue(null) == value).Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    class UILineTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DoubleCollection type = (DoubleCollection)value;
            if (type.SequenceEqual<Double>(new DoubleCollection { 1, 0 })) return 0;
            if (type.SequenceEqual<Double>(new DoubleCollection { 1, 2 })) return 1;
            if (type.SequenceEqual<Double>(new DoubleCollection { 3, 2 })) return 2;
            if (type.SequenceEqual<Double>(new DoubleCollection { 3, 2, 1, 2 })) return 3;
            else return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int)value;
            switch (index)
            {
                case 0:
                    return new DoubleCollection { 1, 0 };
                case 1:
                    return new DoubleCollection { 1, 2 };
                case 2:
                    return new DoubleCollection { 3, 2 };
                case 3:
                    return new DoubleCollection { 3, 2, 1, 2 };
                default:
                    return Binding.DoNothing;
            }
        }
    }

    class UIDoubleAsIsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double num;
            return Double.TryParse(value.ToString(), out num) ? num : Binding.DoNothing;
        }
    }

    class UIDoubleAsIsComboBox : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cmb = (ComboBox)parameter;
            if (cmb.ItemsSource is List<double>)
            {
                if (!cmb.Items.Contains(value)) return cmb.Tag;
                else return value;
            }

            if (cmb.ItemsSource is Dictionary<string, double>)
            {
                var items = (Dictionary<string, double>)cmb.ItemsSource;
                if (!items.ContainsValue((double)value)) return cmb.Uid;
                else return value;
            }
            else return value;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double num;
            if (value == null) return Binding.DoNothing;
            return Double.TryParse(value.ToString(), out num) ? num : Binding.DoNothing;
        }
    }

    class UIDoubleDataFormat : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DataFormatter.FormatData((double)value, DataFormat.Long);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double num;
            return Double.TryParse(value.ToString(), out num) ? num : Binding.DoNothing;
        }
    }

    class UITimeDataFormat : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DataFormatter.FormatTime((double)value, TimeFormat.Standard);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    class UIRouteDistanceFormat : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double distance = (double)values[0];
            string unit = (string)values[1];
            distance = DataConverters.LengthUnits(distance, "KM", unit);
            return DataFormatter.FormatData(distance, DataFormat.Long);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { Binding.DoNothing, Binding.DoNothing };
        }
    }

    class UIStartingFuelSelectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (double)value;
            var cmb = (ComboBox)parameter;
            var dict = (Dictionary<string, double>)cmb.ItemsSource;
            if (val == 0) return 0;
            if (dict.ContainsValue(val))
            {
                cmb.SelectedValue = val;
            }
            else
            {
                cmb.Text = val.ToString();
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int)value;
            var cmb = (ComboBox)parameter;
            double val;
            if (index != -1) return cmb.SelectedValue;
            else if (Double.TryParse(cmb.Text, out val))
            {
                return val;
            }
            else return Binding.DoNothing;
        }
    }

    class UICoordFormat : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var point = (PointLatLng)value;
            if (point == null || point == PointLatLng.Empty) return String.Format($"0°0'0\"N 0°0'0\"E");
            string lat = DataFormatter.FormatGeo(point.Lat, GeoFormat.DMS);
            string lng = DataFormatter.FormatGeo(point.Lng, GeoFormat.DMS);
            return lat + "N" + " " + lng + "E";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    class UIInputCoordFormat : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string main = (string)value;
            int index = (int)parameter;
            if (String.IsNullOrEmpty(main)) return "00";
            switch (index)
            {
                case 1:
                    main = main.Split(' ')[0].TrimEnd('N');
                    return main.Split('°')[0];
                case 2:
                    main = main.Split(' ')[0].TrimEnd('N');
                    return main.Split('°')[1].Split('\'')[0];
                case 3:
                    main = main.Split(' ')[0].TrimEnd('N');
                    return main.Split('°')[1].Split('\'')[1].TrimEnd('\"');
                case 4:
                    main = main.Split(' ')[1].TrimEnd('E');
                    return main.Split('°')[0];
                case 5:
                    main = main.Split(' ')[1].TrimEnd('E');
                    return main.Split('°')[1].Split('\'')[0];
                case 6:
                    main = main.Split(' ')[1].TrimEnd('E');
                    return main.Split('°')[1].Split('\'')[1].TrimEnd('\"');
                default:
                    return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    class UILegTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (RouteLegType)value;
            switch ((int)type)
            {
                case 0:
                    return "Starting";
                case 1:
                    return "Enroute";
                case 2:
                    return "Landing";
                case 3:
                    return "Diversion";
                default:
                    return "Enroute";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string type = (string)value;
            switch (type)
            {
                case "Enroute":
                    return RouteLegType.Enroute;
                default:
                    return Binding.DoNothing;
            }
        }
    }

    class UIFuelAdjustConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (double)value;
            var selector = (ComboBox)parameter;
            if (val < 0)
            {
                selector.SelectedIndex = 1;
                return val * -1;
            }
            else
            {
                selector.SelectedIndex = 0;
                return val;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (double)value;
            var selector = (ComboBox)parameter;
            if (selector.SelectedIndex == 0) return val;
            if (selector.SelectedIndex == 1) return val * -1;
            else return Binding.DoNothing;
        }
    }

}
