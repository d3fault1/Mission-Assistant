using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace MissionAssistant
{
    [Serializable]
    class PolygonSerializationTemplate
    {
        [NonSerialized]
        public Brush Color;
        public double Thickness;
        [NonSerialized]
        public DoubleCollection DashArray;

        public List<PolygonLineSerializationTemplate> Lines;


        private string _color;
        private List<double> _strokedasharray;

        [OnSerializing]
        private void Convert(StreamingContext context)
        {
            var bc = new BrushConverter();
            _color = bc.ConvertToString(Color);

            _strokedasharray = new List<double>(DashArray);
        }

        [OnDeserialized]
        private void ConvertBack(StreamingContext context)
        {
            var bc = new BrushConverter();
            Color = (Brush)typeof(Brushes).GetProperties().FirstOrDefault(b => bc.ConvertToString(b.GetValue(null)) == _color).GetValue(null);
            DashArray = new DoubleCollection(_strokedasharray);
        }

        public PolygonSerializationTemplate()
        {
            Lines = new List<PolygonLineSerializationTemplate>();
        }
    }
}
