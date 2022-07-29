using System;
using System.Windows;

namespace MissionAssistant
{
    class RouteLegSegment : DependencyObject
    {
        #region Dependency Properties
        //Dependency properties
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(SegmentType), typeof(RouteLegSegment), new PropertyMetadata(SegmentType.Level, new PropertyChangedCallback((d, e) => { TypePropertyChanged((RouteLegSegment)d, e); })));
        public static readonly DependencyProperty InitialAltProperty = DependencyProperty.Register("InitialAlt", typeof(double), typeof(RouteLegSegment), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { InitialAltPropertyChanged((RouteLegSegment)d, e); })));
        public static readonly DependencyProperty FinalAltProperty = DependencyProperty.Register("FinalAlt", typeof(double), typeof(RouteLegSegment), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { FinalAltPropertyChanged((RouteLegSegment)d, e); })));
        public static readonly DependencyProperty DistanceProperty = DependencyProperty.Register("Distance", typeof(double), typeof(RouteLegSegment), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { DistancePropertyChanged((RouteLegSegment)d, e); })));
        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register("Time", typeof(double), typeof(RouteLegSegment), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { TimePropertyChanged((RouteLegSegment)d, e); })));
        public static readonly DependencyProperty FuelProperty = DependencyProperty.Register("Fuel", typeof(double), typeof(RouteLegSegment), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { FuelPropertyChanged((RouteLegSegment)d, e); })));
        #endregion

        #region Property Fields
        public SegmentType Type
        {
            get
            {
                return (SegmentType)GetValue(TypeProperty);
            }
            set
            {
                SetValue(TypeProperty, value);
            }
        }
        public double InitialAlt
        {
            get
            {
                return (double)GetValue(InitialAltProperty);
            }
            set
            {
                SetValue(InitialAltProperty, value);
            }
        }
        public double FinalAlt
        {
            get
            {
                return (double)GetValue(FinalAltProperty);
            }
            set
            {
                SetValue(FinalAltProperty, value);
            }
        }
        public double Distance
        {
            get
            {
                return (double)GetValue(DistanceProperty);
            }
            set
            {
                SetValue(DistanceProperty, value);
            }
        }
        public double Time
        {
            get
            {
                return (double)GetValue(TimeProperty);
            }
            set
            {
                SetValue(TimeProperty, value);
            }
        }
        public double Fuel
        {
            get
            {
                return (double)GetValue(FuelProperty);
            }
            set
            {
                SetValue(FuelProperty, value);
            }
        }
        #endregion

        #region Property Callback Functions
        private static void TypePropertyChanged(RouteLegSegment obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                
            }
        }
        private static void InitialAltPropertyChanged(RouteLegSegment obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double alt = (double)e.NewValue;
                if (alt < obj.FinalAlt) obj.Type = SegmentType.Ascend;
                else if (alt > obj.FinalAlt) obj.Type = SegmentType.Descend;
                else obj.Type = SegmentType.Level;
            }
        }
        private static void FinalAltPropertyChanged(RouteLegSegment obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double alt = (double)e.NewValue;
                if (alt > obj.InitialAlt) obj.Type = SegmentType.Ascend;
                else if (alt < obj.InitialAlt) obj.Type = SegmentType.Descend;
                else obj.Type = SegmentType.Level;
            }
        }
        private static void DistancePropertyChanged(RouteLegSegment obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {

            }
        }
        private static void TimePropertyChanged(RouteLegSegment obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {

            }
        }
        private static void FuelPropertyChanged(RouteLegSegment obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {

            }
        }
        #endregion

        #region Internal Fields

        #endregion

        #region Public Fields
        public RouteLeg Parent { get; private set; } = null;
        #endregion

        #region Member Functions
        public RouteLegSegment(RouteLeg parent)
        {
            Parent = parent;
        }
        #endregion
    }

    public enum SegmentType
    {
        Ascend,
        Descend,
        Level
    }
}
