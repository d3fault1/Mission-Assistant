using System;
using System.Collections.Generic;

namespace MissionAssistant
{
    [Serializable]
    class RootSerializationTemplate
    {
        public List<MapLineSerializationTemplate> MapLines;
        public List<CircleSerializationTemplate> Circles;
        public List<PolygonSerializationTemplate> Polygons;
        public List<RouteSerializationTemplate> Routes;

        public RootSerializationTemplate()
        {
            MapLines = new List<MapLineSerializationTemplate>();
            Circles = new List<CircleSerializationTemplate>();
            Polygons = new List<PolygonSerializationTemplate>();
            Routes = new List<RouteSerializationTemplate>();
        }
    }
}
