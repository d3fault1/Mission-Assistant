using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace MissionAssistant
{
    [Serializable]
    class CircleSerializationTemplate
    {
        [NonSerialized]
        public Brush Color;
        public double Thickness;
        [NonSerialized]
        public DoubleCollection StrokeDashArray;

        public PointLatLng Center;
        public double Radius;


        private string _color;
        private List<double> _strokedasharray;

        [OnSerializing]
        private void Convert(StreamingContext context)
        {
            var bc = new BrushConverter();
            _color = bc.ConvertToString(Color);

            _strokedasharray = new List<double>(StrokeDashArray);
        }

        [OnDeserialized]
        private void ConvertBack(StreamingContext context)
        {
            var bc = new BrushConverter();
            Color = (Brush)typeof(Brushes).GetProperties().FirstOrDefault(b => bc.ConvertToString(b.GetValue(null)) == _color).GetValue(null);
            StrokeDashArray = new DoubleCollection(_strokedasharray);
        }

    }
}
