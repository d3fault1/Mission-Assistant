using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace MissionAssistant
{
    [Serializable]
    class RouteSerializationTemplate
    {
        //View Fields
        [NonSerialized]
        public Brush Color;
        public double Thickness;
        [NonSerialized]
        public DoubleCollection DashArray;

        //Data Fields
        public string Aircraft;
        public string Mission;
        public double StartingFuel;
        public double Minima;

        //Member Fields
        public List<RouteLegSerializationTemplate> Legs;
        public List<DiversionLegSerializationTemplate> DiversionLegs;


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

        public RouteSerializationTemplate()
        {
            Legs = new List<RouteLegSerializationTemplate>();
            DiversionLegs = new List<DiversionLegSerializationTemplate>();
        }
    }
}
