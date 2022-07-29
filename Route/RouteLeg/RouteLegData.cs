using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MissionAssistant
{
    partial class RouteLeg
    {
        #region Dependency Property
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(RouteLeg), new PropertyMetadata("", new PropertyChangedCallback((d, e) => { NamePropertyChanged((RouteLeg)d, e); })));
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(RouteLegType), typeof(RouteLeg), new PropertyMetadata(RouteLegType.Enroute, new PropertyChangedCallback((d, e) => { TypePropertyChanged((RouteLeg)d, e); })));
        public static readonly DependencyProperty AltitudeProperty = DependencyProperty.Register("Altitude", typeof(double), typeof(RouteLeg), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { AltitudePropertyChanged((RouteLeg)d, e); })));
        public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register("Speed", typeof(double), typeof(RouteLeg), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { SpeedPropertyChanged((RouteLeg)d, e); })));
        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register("Time", typeof(double), typeof(RouteLeg), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { TimePropertyChanged((RouteLeg)d, e); })));
        public static readonly DependencyProperty FuelProperty = DependencyProperty.Register("Fuel", typeof(double), typeof(RouteLeg), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { FuelPropertyChanged((RouteLeg)d, e); })));
        public static readonly DependencyProperty FuelAdjustmentProperty = DependencyProperty.Register("FuelAdjustment", typeof(double), typeof(RouteLeg), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { FuelAdjustmentPropertyChanged((RouteLeg)d, e); })));
        #endregion

        #region Property Fields
        public string Name
        {
            get
            {
                return (string)GetValue(NameProperty);
            }
            set
            {
                SetValue(NameProperty, value);
            }
        }
        public RouteLegType Type
        {
            get
            {
                return (RouteLegType)GetValue(TypeProperty);
            }
            set
            {
                SetValue(TypeProperty, value);
            }
        }
        public double Altitude
        {
            get
            {
                return (double)GetValue(AltitudeProperty);
            }
            set
            {
                SetValue(AltitudeProperty, value);
            }
        }
        public double Speed
        {
            get
            {
                return (double)GetValue(SpeedProperty);
            }
            set
            {
                SetValue(SpeedProperty, value);
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
        public double FuelAdjustment
        {
            get
            {
                return (double)GetValue(FuelAdjustmentProperty);
            }
            set
            {
                SetValue(FuelAdjustmentProperty, value);
            }
        }
        #endregion

        #region Property Callback Functions
        private static void NamePropertyChanged(RouteLeg obj, DependencyPropertyChangedEventArgs e)
        {
            
        }
        private static void TypePropertyChanged(RouteLeg obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                obj.TypeUpdatedView();
                obj.OnTypeUpdated();
            }
        }
        private static void AltitudePropertyChanged(RouteLeg obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                obj.OnAltitudeUpdated();
            }
        }
        private static void SpeedPropertyChanged(RouteLeg obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                obj.OnSpeedUpdated();
            }
        }
        private static void TimePropertyChanged(RouteLeg obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                obj.OnTimeUpdated();
            }
        }
        private static void FuelPropertyChanged(RouteLeg obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                obj.OnFuelUpdated();
            }
        }
        private static void FuelAdjustmentPropertyChanged(RouteLeg obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {

            }
        }
        #endregion

        #region Internal Fields
        #endregion

        #region Public Fields
        public Route Parent { get; private set; }

        public List<RouteLegSegment> Segments;

        #region Events and Delegates
        //Delegates
        public delegate void TypeUpdatedEventHandler(object sender, EventArgs e);
        public delegate void AltitudeUpdatedEventHandler(object sender, EventArgs e);
        public delegate void SpeedUpdatedEventHandler(object sender, EventArgs e);
        public delegate void TimeUpdatedEventHandler(object sender, EventArgs e);
        public delegate void FuelUpdatedEventHandler(object sender, EventArgs e);

        //Events
        public event TypeUpdatedEventHandler TypeUpdated;
        public event AltitudeUpdatedEventHandler AltitudeUpdated;
        public event SpeedUpdatedEventHandler SpeedUpdated;
        public event TimeUpdatedEventHandler TimeUpdated;
        public event FuelUpdatedEventHandler FuelUpdated;
        #endregion

        #endregion

        #region Member Functions
        public void CalculateData(double nextAlt = double.NaN)
        {
            //Pre
            if ((Type == RouteLegType.Starting) || (Type == RouteLegType.Landing && Parent.Legs.Count == 1))
            {
                Segments[0].InitialAlt = 0;
                Segments[0].FinalAlt = Altitude;
            }
            else
            {
                Segments[0].InitialAlt = Altitude;
                Segments[0].FinalAlt = Altitude;
            }
            Segments[0].Distance = DataCalculations.GetClimbDescendDistance(Segments[0].InitialAlt, Segments[0].FinalAlt, Parent.Aircraft);
            Segments[0].Time = DataCalculations.GetClimbDescendTime(Segments[0].InitialAlt, Segments[0].FinalAlt, Parent.Aircraft);
            Segments[0].Fuel = DataCalculations.GetClimbDescendFuel(Segments[0].InitialAlt, Segments[0].FinalAlt, Parent.Aircraft);

            //Post
            if (Type == RouteLegType.Landing)
            {
                Segments[2].InitialAlt = Altitude;
                Segments[2].FinalAlt = 0;
            }
            else
            {
                Segments[2].InitialAlt = Altitude;
                if (nextAlt == double.NaN) Segments[2].FinalAlt = Altitude;
                else Segments[2].FinalAlt = nextAlt;
            }
            Segments[2].Distance = DataCalculations.GetClimbDescendDistance(Segments[2].InitialAlt, Segments[2].FinalAlt, Parent.Aircraft);
            Segments[2].Time = DataCalculations.GetClimbDescendTime(Segments[2].InitialAlt, Segments[2].FinalAlt, Parent.Aircraft);
            Segments[2].Fuel = DataCalculations.GetClimbDescendFuel(Segments[2].InitialAlt, Segments[2].FinalAlt, Parent.Aircraft);

            //Level
            Segments[1].InitialAlt = Altitude;
            Segments[1].FinalAlt = Altitude;
            Segments[1].Distance = Distance - (Segments[0].Distance + Segments[2].Distance);
            Segments[1].Time = DataCalculations.GetLevelTime(Segments[1].Distance, Speed, Parent.Aircraft);
            Segments[1].Fuel = DataCalculations.GetLevelFuel(Segments[1].Distance, Segments[1].Time, Altitude, Speed, Parent.Aircraft);

            Time = 0;
            Fuel = 0;
            foreach (RouteLegSegment rls in Segments)
            {
                Time += rls.Time;
                Fuel += rls.Fuel;
            }
        }

        #region Event Functions
        private void OnTypeUpdated()
        {
            TypeUpdated?.Invoke(this, EventArgs.Empty);
        }
        private void OnAltitudeUpdated()
        {
            AltitudeUpdated?.Invoke(this, EventArgs.Empty);
        }
        private void OnSpeedUpdated()
        {
            SpeedUpdated?.Invoke(this, EventArgs.Empty);
        }
        private void OnTimeUpdated()
        {
            TimeUpdated?.Invoke(this, EventArgs.Empty);
        }
        private void OnFuelUpdated()
        {
            FuelUpdated?.Invoke(this, EventArgs.Empty);
        }
        #endregion
        #endregion
    }
    public enum RouteLegType
    {
        Starting,
        Enroute,
        Landing,
        Diversion
    }
}
