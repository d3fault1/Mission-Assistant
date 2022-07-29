using GMap.NET;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Linq;

namespace MissionAssistant
{
    [Serializable]
    class MapLineSerializationTemplate
    {
        [NonSerialized]
        public Brush LineColor;
        public double LineThickness;
        [NonSerialized]
        public DoubleCollection LineDashArray;

        public PointLatLng StartPoint;
        public PointLatLng EndPoint;

        private string _color;
        private List<double> _strokedasharray;

        [OnSerializing]
        private void Convert(StreamingContext context)
        {
            var bc = new BrushConverter();
            _color = bc.ConvertToString(LineColor);

            _strokedasharray = new List<double>(LineDashArray);
        }

        [OnDeserialized]
        private void ConvertBack(StreamingContext context)
        {
            var bc = new BrushConverter();
            LineColor = (Brush)typeof(Brushes).GetProperties().FirstOrDefault(b => bc.ConvertToString(b.GetValue(null)) == _color).GetValue(null);
            LineDashArray = new DoubleCollection(_strokedasharray);
        }
    }
}
