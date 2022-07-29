using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MissionAssistant
{
    partial class Route : DependencyObject
    {
        #region Dependency Property
        //Dependency view properties
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Brush), typeof(Route), new PropertyMetadata(Brushes.White, new PropertyChangedCallback((d, e) => { ColorPropertyChanged((Route)d, e); })));
        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register("Thickness", typeof(double), typeof(Route), new PropertyMetadata(2.0, new PropertyChangedCallback((d, e) => { ThicknessPropertyChanged((Route)d, e); })));
        public static readonly DependencyProperty DashArrayProperty = DependencyProperty.Register("DashArray", typeof(DoubleCollection), typeof(Route), new PropertyMetadata(new DoubleCollection { 1, 0 }, new PropertyChangedCallback((d, e) => { DashArrayPropertyChanged((Route)d, e); })));
        public static readonly DependencyProperty NodeRadiusProperty = DependencyProperty.Register("NodeRadius", typeof(double), typeof(Route), new PropertyMetadata(10.0, new PropertyChangedCallback((d, e) => { NodeRadiusPropertyChanged((Route)d, e); })));
        public static readonly DependencyProperty NodeColorProperty = DependencyProperty.Register("NodeColor", typeof(Brush), typeof(Route), new PropertyMetadata(Brushes.Red, new PropertyChangedCallback((d, e) => { NodeColorPropertyChanged((Route)d, e); })));
        public static readonly DependencyProperty NodeColorBindsToLineProperty = DependencyProperty.Register("NodeColorBindsToLine", typeof(bool), typeof(Route), new PropertyMetadata(true, new PropertyChangedCallback((d, e) => { NodeColorBindsToLinePropertyChanged((Route)d, e); })));
        public static readonly DependencyProperty NodesFilledProperty = DependencyProperty.Register("NodesFilled", typeof(bool), typeof(Route), new PropertyMetadata(false, new PropertyChangedCallback((d, e) => { NodesFilledPropertyChanged((Route)d, e); })));
        public static readonly DependencyProperty RestrictPositionUpdateProperty = DependencyProperty.Register("RestrictPositionUpdate", typeof(bool), typeof(Route), new PropertyMetadata(false, new PropertyChangedCallback((d, e) => { RestrictPositionUpdatePropertyChanged((Route)d, e); })));
        #endregion

        #region Property Fields
        //Property fields of this class
        public Brush Color
        {
            get
            {
                return (Brush)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }
        public double Thickness
        {
            get
            {
                return (double)GetValue(ThicknessProperty);
            }
            set
            {
                SetValue(ThicknessProperty, value);
            }
        }
        public DoubleCollection DashArray
        {
            get
            {
                return (DoubleCollection)GetValue(DashArrayProperty);
            }
            set
            {
                SetValue(DashArrayProperty, value);
            }
        }
        public double NodeRadius
        {
            get
            {
                return (double)GetValue(NodeRadiusProperty);
            }
            set
            {
                SetValue(NodeRadiusProperty, value);
            }
        }
        public Brush NodeColor
        {
            get
            {
                return (Brush)GetValue(NodeColorProperty);
            }
            set
            {
                SetValue(NodeColorProperty, value);
            }
        }
        public bool NodeColorBindsToLine
        {
            get
            {
                return (bool)GetValue(NodeColorBindsToLineProperty);
            }
            set
            {
                SetValue(NodeColorBindsToLineProperty, value);
            }
        }
        public bool NodesFilled
        {
            get
            {
                return (bool)GetValue(NodesFilledProperty);
            }
            set
            {
                SetValue(NodesFilledProperty, value);
            }
        }
        public bool RestrictPositionUpdate
        {
            get
            {
                return (bool)GetValue(RestrictPositionUpdateProperty);
            }
            set
            {
                SetValue(RestrictPositionUpdateProperty, value);
            }
        }
        #endregion

        #region Property Callback Functions
        private static void ColorPropertyChanged(Route obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                Brush stroke = (Brush)e.NewValue;
                foreach (RouteLeg leg in obj.Legs)
                {
                    leg.LineColor = stroke;
                }
            }
        }
        private static void ThicknessPropertyChanged(Route obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double thickness = (double)e.NewValue;
                foreach (RouteLeg leg in obj.Legs)
                {
                    leg.LineThickness = thickness;
                }
            }
        }
        private static void DashArrayPropertyChanged(Route obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                DoubleCollection dasharray = (DoubleCollection)e.NewValue;
                foreach (RouteLeg leg in obj.Legs)
                {
                    leg.LineDashArray = dasharray;
                }
            }
        }
        private static void NodeRadiusPropertyChanged(Route obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double radius = (double)e.NewValue;
                foreach (RouteLeg leg in obj.Legs)
                {
                    leg.NodeRadius = radius;
                }
                foreach (DiversionLeg leg in obj.DiversionLegs)
                {
                    leg.NodeRadius = radius;
                }
            }
        }
        private static void NodeColorPropertyChanged(Route obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                Brush stroke = (Brush)e.NewValue;
                foreach (RouteLeg leg in obj.Legs)
                {
                    leg.NodeColor = stroke;
                }
            }
        }
        private static void NodeColorBindsToLinePropertyChanged(Route obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                bool isbound = (bool)e.NewValue;
                foreach (RouteLeg leg in obj.Legs)
                {
                    leg.NodeColorBindsToLine = isbound;
                }
            }
        }
        private static void NodesFilledPropertyChanged(Route obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                bool isfilled = (bool)e.NewValue;
                foreach (RouteLeg leg in obj.Legs)
                {
                    leg.NodesFilled = isfilled;
                }
                foreach (DiversionLeg leg in obj.DiversionLegs)
                {
                    leg.NodesFilled = isfilled;
                }
            }
        }
        private static void RestrictPositionUpdatePropertyChanged(Route obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                bool isrestricted = (bool)e.NewValue;
                foreach (RouteLeg leg in obj.Legs)
                {
                    leg.RestrictPositionUpdate = isrestricted;
                }
                foreach (DiversionLeg leg in obj.DiversionLegs)
                {
                    leg.RestrictPositionUpdate = isrestricted;
                }
            }
        }
        #endregion

        #region Internal Fields
        private List<MapLine> DisposableLegs;
        #endregion

        #region Public Fields
        #endregion

        #region Member Functions
        public Route()
        {
            CreateObject();
        }

        public void Draw(Canvas canvas)
        {
            foreach (RouteLeg leg in Legs) leg.Draw(canvas);
            foreach (DiversionLeg leg in DiversionLegs) leg.Draw(canvas);
            foreach (MapLine leg in DisposableLegs) leg.Undraw(canvas);
            DisposableLegs.Clear();
        }

        public void Undraw(Canvas canvas)
        {
            foreach (RouteLeg leg in Legs) leg.Undraw(canvas);
            foreach (DiversionLeg leg in DiversionLegs) leg.Undraw(canvas);
            foreach (MapLine leg in DisposableLegs) leg.Undraw(canvas);
            Legs.Clear();
            DiversionLegs.Clear();
            DisposableLegs.Clear();
        }

        public void UpdateLocalParameters()
        {
            foreach (var leg in Legs)
            {
                leg.UpdateLocalParameters();
            }
            foreach (var leg in DiversionLegs)
            {
                leg.UpdateLocalParameters();
            }
        }

        public void CreateObject()
        {
            Legs = new ObservableCollection<RouteLeg>();
            DiversionLegs = new ObservableCollection<DiversionLeg>();
            DisposableLegs = new List<MapLine>();
            Legs.CollectionChanged += LegChangedView;
            Legs.CollectionChanged += LegChangedData;
            DiversionLegs.CollectionChanged += DiversionLegChangedView;
        }

        private void LegChangedView(object sender, NotifyCollectionChangedEventArgs e)
        {
            for (int i = 0; i < Legs.Count - 1; i++)
            {
                BindingOperations.SetBinding(Legs[i], RouteLeg.LocalEndPointProperty, new Binding { Source = Legs[i + 1], Path = new PropertyPath(RouteLeg.LocalStartPointProperty), Mode = BindingMode.TwoWay });
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (RouteLeg l in e.NewItems)
                {
                    l.LineColor = Color;
                    l.LineThickness = Thickness;
                    l.LineDashArray = DashArray;
                    l.NodeRadius = NodeRadius;
                    l.NodeColor = NodeColor;
                    l.NodeColorBindsToLine = NodeColorBindsToLine;
                    l.NodesFilled = NodesFilled;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (RouteLeg l in e.OldItems)
                {
                    DisposableLegs.Add(l);
                }
            }
        }

        private void DiversionLegChangedView(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (DiversionLeg l in e.NewItems)
                {
                    if (l.LinkingLegIndex < Legs.Count)
                    {
                        if (l.LinkingNode == 2) BindingOperations.SetBinding(l, DiversionLeg.LocalStartPointProperty, new Binding { Source = Legs[l.LinkingLegIndex], Path = new PropertyPath(RouteLeg.LocalEndPointProperty) });
                        else BindingOperations.SetBinding(l, DiversionLeg.LocalStartPointProperty, new Binding { Source = Legs[l.LinkingLegIndex], Path = new PropertyPath(RouteLeg.LocalStartPointProperty) });
                    }
                    else
                    {
                        DiversionLegs.CollectionChanged -= DiversionLegChangedView;
                        DiversionLegs.Remove(l);
                        DiversionLegs.CollectionChanged += DiversionLegChangedView;
                    }
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (DiversionLeg l in e.OldItems)
                {
                    DisposableLegs.Add(l);
                }
            }
        }
        #endregion
    }
}
