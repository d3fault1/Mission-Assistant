using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MissionAssistant
{
    partial class Polygon
    {
        #region Dependency Properties
        //Dependency Data Properties
        public static readonly DependencyProperty TotalDistanceProperty = DependencyProperty.Register("TotalDistance", typeof(double), typeof(Polygon), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => { TotalDistancePropertyChanged((Polygon)d, e); })));
        #endregion

        #region Property Fields
        public double TotalDistance
        {
            get
            {
                return (double)GetValue(TotalDistanceProperty);
            }
            private set
            {
                SetValue(TotalDistanceProperty, value);
            }
        }
        #endregion

        #region Property Callback Functions
        private static void TotalDistancePropertyChanged(Polygon obj, DependencyPropertyChangedEventArgs e)
        {
            
        }
        #endregion

        #region Member Functions
        //Private Methods
        private void BindData(PolygonLine line)
        {
            TotalDistance += line.Distance;
            line.DistanceUpdated += NotifyDistanceUpdate;
        }
        private void UnbindData(PolygonLine line)
        {
            TotalDistance -= line.Distance;
            line.DistanceUpdated -= NotifyDistanceUpdate;
        }
        private void NotifyDistanceUpdate(object sender, EventArgs e)
        {
            double res = 0;
            foreach(PolygonLine line in Lines)
            {
                res += line.Distance;
            }
            TotalDistance = res;
        }
        #endregion
    }
}
