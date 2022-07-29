using GMap.NET;
using System;
using System.Windows;

namespace MissionAssistant
{
    partial class MapLine
    {
        #region Dependency Properties
        //Dependency data properties
        public static readonly DependencyProperty StartPointProperty = DependencyProperty.Register("StartPoint", typeof(PointLatLng), typeof(MapLine), new PropertyMetadata(new PropertyChangedCallback((d, e) => { StartPointPropertyChanged((MapLine)d, e); })));
        public static readonly DependencyProperty EndPointProperty = DependencyProperty.Register("EndPoint", typeof(PointLatLng), typeof(MapLine), new PropertyMetadata(new PropertyChangedCallback((d, e) => { EndPointPropertyChanged((MapLine)d, e); })));
        public static readonly DependencyProperty DistanceProperty = DependencyProperty.Register("Distance", typeof(double), typeof(MapLine), new PropertyMetadata(new PropertyChangedCallback((d, e) => { DistancePropertyChanged((MapLine)d, e); })));
        public static readonly DependencyProperty TrackProperty = DependencyProperty.Register("Track", typeof(double), typeof(MapLine), new PropertyMetadata(new PropertyChangedCallback((d, e) => { TrackPropertyChanged((MapLine)d, e); })));
        #endregion

        #region Property Fields
        public PointLatLng StartPoint
        {
            get
            {
                return (PointLatLng)GetValue(StartPointProperty);
            }
            set
            {
                SetValue(StartPointProperty, value);
            }
        }
        public PointLatLng EndPoint
        {
            get
            {
                return (PointLatLng)GetValue(EndPointProperty);
            }
            set
            {
                SetValue(EndPointProperty, value);
            }
        }
        public double Distance
        {
            get
            {
                return (double)GetValue(DistanceProperty);
            }
            private set
            {
                SetValue(DistanceProperty, value);
            }
        }
        public double Track
        {
            get
            {
                return (double)GetValue(TrackProperty);
            }
            private set
            {
                SetValue(TrackProperty, value);
            }
        }
        #endregion

        #region Property Callback Functions
        private static void StartPointPropertyChanged(MapLine obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                PointLatLng pos = (PointLatLng)e.NewValue;

                //Update Local
                if (!obj.restrictStartUpdate)
                {
                    obj.restrictStartUpdate = true;
                    obj.LocalStartPoint = DataCalculations.GetPhysicalFromLatLng(pos);

                    if (obj.RestrictPositionUpdate) obj.restrictStartUpdate = false;
                }
                else obj.restrictStartUpdate = false;

                //Calculate Data
                obj.CalculateDistance();
                obj.CalculateTrack();
            }
        }
        private static void EndPointPropertyChanged(MapLine obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                PointLatLng pos = (PointLatLng)e.NewValue;

                //Update Local
                if (!obj.restrictEndUpdate)
                {
                    obj.restrictEndUpdate = true;
                    obj.LocalEndPoint = DataCalculations.GetPhysicalFromLatLng(pos);

                    if (obj.RestrictPositionUpdate) obj.restrictEndUpdate = false;
                }
                else obj.restrictEndUpdate = false;

                //Calculate Data
                obj.CalculateDistance();
                obj.CalculateTrack();
            }
        }
        private static void DistancePropertyChanged(MapLine obj, DependencyPropertyChangedEventArgs e)
        {
            obj.OnDistanceUpdated();
        }
        private static void TrackPropertyChanged(MapLine obj, DependencyPropertyChangedEventArgs e)
        {
            obj.OnTrackUpdated();
        }
        #endregion

        #region Internal Fields
        #endregion

        #region Public Fields
        public delegate void DistanceUpdatedEventHandler(object sender, EventArgs e);
        public delegate void TrackUpdatedEventHandler(object sender, EventArgs e);
        public event DistanceUpdatedEventHandler DistanceUpdated;
        public event TrackUpdatedEventHandler TrackUpdated;
        #endregion

        #region Member Functions
        private void CalculateDistance()
        {
            Distance = DataCalculations.GetDistance(StartPoint, EndPoint);
        }
        private void CalculateTrack()
        {
            Track = DataCalculations.GetTrack(StartPoint, EndPoint);
        }

        protected virtual void OnDistanceUpdated()
        {
            DistanceUpdated?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnTrackUpdated()
        {
            TrackUpdated?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
