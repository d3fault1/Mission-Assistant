using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;

namespace MissionAssistant
{
    partial class Route
    {
        #region Dependency Property
        //Dependency data properties
        public static readonly DependencyProperty AircraftProperty = DependencyProperty.Register("Aircraft", typeof(string), typeof(Route), new PropertyMetadata(new PropertyChangedCallback((d, e) => { AircraftPropertyChanged((Route)d, e); })));
        public static readonly DependencyProperty MissionProperty = DependencyProperty.Register("Mission", typeof(string), typeof(Route), new PropertyMetadata(new PropertyChangedCallback((d, e) => { MissionPropertyChanged((Route)d, e); })));
        public static readonly DependencyProperty TotalDistanceProperty = DependencyProperty.Register("TotalDistance", typeof(double), typeof(Route), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { TotalDistancePropertyChanged((Route)d, e); })));
        public static readonly DependencyProperty TotalTimeProperty = DependencyProperty.Register("TotalTime", typeof(double), typeof(Route), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { TotalTimePropertyChanged((Route)d, e); })));
        public static readonly DependencyProperty TotalFuelProperty = DependencyProperty.Register("TotalFuel", typeof(double), typeof(Route), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { TotalFuelPropertyChanged((Route)d, e); })));
        public static readonly DependencyProperty StartingFuelProperty = DependencyProperty.Register("StartingFuel", typeof(double), typeof(Route), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { StartingFuelPropertyChanged((Route)d, e); })));
        public static readonly DependencyProperty ReductionFuelProperty = DependencyProperty.Register("ReductionFuel", typeof(double), typeof(Route), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { ReductionFuelPropertyChanged((Route)d, e); })));
        public static readonly DependencyProperty MinimaProperty = DependencyProperty.Register("Minima", typeof(double), typeof(Route), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { MinimaPropertyChanged((Route)d, e); })));
        #endregion

        #region Property Fields
        public string Aircraft
        {
            get
            {
                return (string)GetValue(AircraftProperty);
            }
            set
            {
                SetValue(AircraftProperty, value);
            }
        }
        public string Mission
        {
            get
            {
                return (string)GetValue(MissionProperty);
            }
            set
            {
                SetValue(MissionProperty, value);
            }
        }
        public double TotalDistance
        {
            get
            {
                return (double)GetValue(TotalDistanceProperty);
            }
            set
            {
                SetValue(TotalDistanceProperty, value);
            }
        }
        public double TotalTime
        {
            get
            {
                return (double)GetValue(TotalTimeProperty);
            }
            set
            {
                SetValue(TotalTimeProperty, value);
            }
        }
        public double TotalFuel
        {
            get
            {
                return (double)GetValue(TotalFuelProperty);
            }
            set
            {
                SetValue(TotalFuelProperty, value);
            }
        }
        public double StartingFuel
        {
            get
            {
                return (double)GetValue(StartingFuelProperty);
            }
            set
            {
                SetValue(StartingFuelProperty, value);
            }
        }
        public double ReductionFuel
        {
            get
            {
                return (double)GetValue(ReductionFuelProperty);
            }
            set
            {
                SetValue(ReductionFuelProperty, value);
            }
        }
        public double Minima
        {
            get
            {
                return (double)GetValue(MinimaProperty);
            }
            set
            {
                SetValue(MinimaProperty, value);
            }
        }
        #endregion

        #region Property Callback Functions
        private static void AircraftPropertyChanged(Route obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                obj.setReductionFuelData();

                foreach (RouteLeg l in obj.Legs)
                {
                    obj.calcLegHeadingBoxData(l);
                }

                foreach (DiversionLeg l in obj.DiversionLegs)
                {
                    l.CalculateData();
                }
            }
        }
        private static void MissionPropertyChanged(Route obj, DependencyPropertyChangedEventArgs e)
        {

        }
        private static void TotalDistancePropertyChanged(Route obj, DependencyPropertyChangedEventArgs e)
        {

        }
        private static void TotalTimePropertyChanged(Route obj, DependencyPropertyChangedEventArgs e)
        {

        }
        private static void TotalFuelPropertyChanged(Route obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                obj.calcLegFuelCircleData();
            }
        }
        private static void StartingFuelPropertyChanged(Route obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                obj.calcLegFuelCircleData();
            }
        }
        private static void ReductionFuelPropertyChanged(Route obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                obj.calcLegFuelCircleData();
            }
        }
        private static void MinimaPropertyChanged(Route obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                obj.calcLegFuelCircleData();
            }
        }
        #endregion

        #region Internal Fields
        #endregion

        #region Public Fields
        public ObservableCollection<RouteLeg> Legs;
        public ObservableCollection<DiversionLeg> DiversionLegs;
        #endregion

        #region Member Functions
        private void LegChangedData(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset) return;
            for (int i = 1; i < Legs.Count - 1; i++)
            {
                if (Legs[i].Type == RouteLegType.Starting || Legs[i].Type == RouteLegType.Landing) Legs[i].Type = RouteLegType.Enroute;
            }
            Legs[0].Type = RouteLegType.Starting;
            Legs[Legs.Count - 1].Type = RouteLegType.Landing;

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (RouteLeg l in e.NewItems)
                {
                    l.TypeUpdated += LegTypeUpdated;
                    l.AltitudeUpdated += LegAltitudeUpdated;
                    l.SpeedUpdated += LegSpeedUpdated;
                    l.DistanceUpdated += LegDistanceUpdated;
                    l.TimeUpdated += LegTimeUpdated;
                    l.FuelUpdated += LegFuelUpdated;
                    calcLegHeadingBoxData(l);

                    TotalDistance += l.Distance;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (RouteLeg l in e.OldItems)
                {
                    l.TypeUpdated -= LegTypeUpdated;
                    l.AltitudeUpdated -= LegAltitudeUpdated;
                    l.SpeedUpdated -= LegSpeedUpdated;
                    l.DistanceUpdated -= LegDistanceUpdated;
                    l.TimeUpdated -= LegTimeUpdated;
                    l.FuelUpdated -= LegFuelUpdated;

                    TotalDistance -= l.Distance;
                    TotalTime -= l.Time;
                    TotalFuel -= l.Fuel;
                }
            }
        }

        #region Leg Event Functions
        private void LegTypeUpdated(object sender, EventArgs e)
        {
            RouteLeg l = (RouteLeg)sender;
            calcLegHeadingBoxData(l);
        }
        private void LegAltitudeUpdated(object sender, EventArgs e)
        {
            RouteLeg l = (RouteLeg)sender;
            calcLegHeadingBoxData(l);
        }
        private void LegSpeedUpdated(object sender, EventArgs e)
        {
            RouteLeg l = (RouteLeg)sender;
            calcLegHeadingBoxData(l);
        }
        private void LegDistanceUpdated(object sender, EventArgs e)
        {
            RouteLeg l = (RouteLeg)sender;
            calcLegHeadingBoxData(l);

            TotalDistance = 0;
            foreach (RouteLeg leg in Legs)
            {
                TotalDistance += leg.Distance;
            }
        }
        private void LegTimeUpdated(object sender, EventArgs e)
        {
            TotalTime = 0;
            foreach (RouteLeg l in Legs)
            {
                TotalTime += l.Time;
            }
        }
        private void LegFuelUpdated(object sender, EventArgs e)
        {
            TotalFuel = 0;
            foreach (RouteLeg l in Legs)
            {
                TotalFuel += l.Fuel;
            }
        }
        #endregion

        //Route calculation functions
        private void calcLegHeadingBoxData(RouteLeg leg)
        {
            //Needs improvement
            if (Legs.IndexOf(leg) > 0)
            {
                var prev = Legs[Legs.IndexOf(leg) - 1];
                calcLegHeadingBoxData(prev);
            }

            if (leg.Type != RouteLegType.Landing) leg.CalculateData(Legs[Legs.IndexOf(leg) + 1].Altitude);
            else leg.CalculateData();
        }

        //Very static function... Needs improvement
        private void calcLegFuelCircleData()
        {
            double rem = StartingFuel - ReductionFuel;
            double sug = TotalFuel + Minima;
            foreach (RouteLeg leg in Legs)
            {
                if (leg.Type == RouteLegType.Landing)
                {
                    leg.FuelCircles[0].RemainingFuel = rem;
                    leg.FuelCircles[0].SuggestedFuel = sug;
                    rem -= leg.Fuel;
                    sug -= leg.Fuel;
                    leg.FuelCircles[1].RemainingFuel = rem;
                    leg.FuelCircles[1].SuggestedFuel = sug;
                }
                else
                {
                    leg.FuelCircles[0].RemainingFuel = rem;
                    leg.FuelCircles[0].SuggestedFuel = sug;
                }
                rem -= leg.Fuel;
                sug -= leg.Fuel;
            }
        }

        private void setReductionFuelData()
        {
            ReductionFuel = DataCalculations.GetReductionFuel(Aircraft);
        }

        public void UpdateData()
        {
            foreach (var l in Legs)
            {
                calcLegHeadingBoxData(l);
            }
        }
        #endregion
    }
}
