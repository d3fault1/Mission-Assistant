using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MissionAssistant
{
    partial class Polygon : DependencyObject
    {
        #region Dependency Properties
        //Dependency view properties
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Brush), typeof(Polygon), new PropertyMetadata(Brushes.White, new PropertyChangedCallback((d, e) => { ColorPropertyChanged((Polygon)d, e); })));
        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register("Thickness", typeof(double), typeof(Polygon), new PropertyMetadata(2.0, new PropertyChangedCallback((d, e) => { ThicknessPropertyChanged((Polygon)d, e); })));
        public static readonly DependencyProperty DashArrayProperty = DependencyProperty.Register("DashArray", typeof(DoubleCollection), typeof(Polygon), new PropertyMetadata(new DoubleCollection { 1, 0 }, new PropertyChangedCallback((d, e) => { DashArrayPropertyChanged((Polygon)d, e); })));
        public static readonly DependencyProperty NodeRadiusProperty = DependencyProperty.Register("NodeRadius", typeof(double), typeof(Polygon), new PropertyMetadata(5.0, new PropertyChangedCallback((d, e) => { NodeRadiusPropertyChanged((Polygon)d, e); })));
        public static readonly DependencyProperty NodeColorProperty = DependencyProperty.Register("NodeColor", typeof(Brush), typeof(Polygon), new PropertyMetadata(Brushes.Red, new PropertyChangedCallback((d, e) => { NodeColorPropertyChanged((Polygon)d, e); })));
        public static readonly DependencyProperty NodeColorBindsToLineProperty = DependencyProperty.Register("NodeColorBindsToLine", typeof(bool), typeof(Polygon), new PropertyMetadata(true, new PropertyChangedCallback((d, e) => { NodeColorBindsToLinePropertyChanged((Polygon)d, e); })));
        public static readonly DependencyProperty NodesFilledProperty = DependencyProperty.Register("NodesFilled", typeof(bool), typeof(Polygon), new PropertyMetadata(true, new PropertyChangedCallback((d, e) => { NodesFilledPropertyChanged((Polygon)d, e); })));
        public static readonly DependencyProperty RestrictPositionUpdateProperty = DependencyProperty.Register("RestrictPositionUpdate", typeof(bool), typeof(Polygon), new PropertyMetadata(false, new PropertyChangedCallback((d, e) => { RestrictPositionUpdatePropertyChanged((Polygon)d, e); })));
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
        private static void ColorPropertyChanged(Polygon obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                Brush stroke = (Brush)e.NewValue;
                foreach (PolygonLine line in obj.Lines)
                {
                    line.LineColor = stroke;
                }
            }
        }
        private static void ThicknessPropertyChanged(Polygon obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double thickness = (double)e.NewValue;
                foreach (PolygonLine line in obj.Lines)
                {
                    line.LineThickness = thickness;
                }
            }
        }
        private static void DashArrayPropertyChanged(Polygon obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                DoubleCollection dasharray = (DoubleCollection)e.NewValue;
                foreach (PolygonLine line in obj.Lines)
                {
                    line.LineDashArray = dasharray;
                }
            }
        }
        private static void NodeRadiusPropertyChanged(Polygon obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                double radius = (double)e.NewValue;
                foreach (PolygonLine line in obj.Lines)
                {
                    line.NodeRadius = radius;
                }
            }
        }
        private static void NodeColorPropertyChanged(Polygon obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                Brush stroke = (Brush)e.NewValue;
                foreach (PolygonLine line in obj.Lines)
                {
                    line.NodeColor = stroke;
                }
            }
        }
        private static void NodeColorBindsToLinePropertyChanged(Polygon obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                bool isbound = (bool)e.NewValue;
                foreach (PolygonLine line in obj.Lines)
                {
                    line.NodeColorBindsToLine = isbound;
                }
            }
        }
        private static void NodesFilledPropertyChanged(Polygon obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                bool isfilled = (bool)e.NewValue;
                foreach (PolygonLine line in obj.Lines)
                {
                    line.NodesFilled = isfilled;
                }
            }
        }
        private static void RestrictPositionUpdatePropertyChanged(Polygon obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.IsValidValue(e.NewValue))
            {
                bool isrestricted = (bool)e.NewValue;
                foreach (PolygonLine line in obj.Lines)
                {
                    line.RestrictPositionUpdate = isrestricted;
                }
            }
        }
        #endregion

        #region Public Fields
        public ObservableCollection<PolygonLine> Lines;
        #endregion

        #region Internal Fields
        private List<PolygonLine> DisposableLines;
        #endregion

        #region Member Methods
        public Polygon()
        {
            CreateObject();
        }

        //Public Methods
        public void Draw(Canvas canvas)
        {
            foreach (PolygonLine line in Lines) line.Draw(canvas);
            foreach (PolygonLine line in DisposableLines) line.Undraw(canvas);
            DisposableLines.Clear();
        }

        public void Undraw(Canvas canvas)
        {
            foreach (PolygonLine line in Lines) line.Undraw(canvas);
            foreach (PolygonLine line in DisposableLines) line.Undraw(canvas);
            Lines.Clear();
            DisposableLines.Clear();
        }

        public void UpdateLocalParameters()
        {
            foreach (var line in Lines)
            {
                line.UpdateLocalParameters();
            }
        }

        //Private Methods
        private void CreateObject()
        {
            Lines = new ObservableCollection<PolygonLine>();
            DisposableLines = new List<PolygonLine>();
            Lines.CollectionChanged += ItemsChanged;
        }
        private void ItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            for (int i = 0; i < Lines.Count - 1; i++)
            {
                BindingOperations.SetBinding(Lines[i], PolygonLine.LocalEndPointProperty, new Binding { Source = Lines[i + 1], Path = new PropertyPath(PolygonLine.LocalStartPointProperty), Mode = BindingMode.TwoWay });
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (PolygonLine line in e.NewItems)
                {
                    line.LineColor = Color;
                    line.LineThickness = Thickness;
                    line.LineDashArray = DashArray;
                    line.NodeRadius = NodeRadius;
                    line.NodeColor = NodeColor;
                    line.NodeColorBindsToLine = NodeColorBindsToLine;
                    line.NodesFilled = NodesFilled;
                    BindData(line);
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (PolygonLine line in e.OldItems)
                {
                    UnbindData(line);
                    DisposableLines.Add(line);
                }
            }
        }
        #endregion
    }
}
