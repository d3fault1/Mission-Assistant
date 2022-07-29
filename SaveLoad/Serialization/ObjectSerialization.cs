using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MissionAssistant
{
    static class ObjectSerialization
    {
        #region Serializers
        public static void SerializeData(List<object> Items, string filepath)
        {
            RootSerializationTemplate template = new RootSerializationTemplate();
            foreach (var item in Items)
            {
                if (item is MapLine) template.MapLines.Add(GetMaplineTemplate((MapLine)item));
                else if (item is Circle) template.Circles.Add(GetCircleTemplate((Circle)item));
                else if (item is Polygon) template.Polygons.Add(GetPolygonTemplate((Polygon)item));
                else if (item is Route) template.Routes.Add(GetRouteTemplate((Route)item));
            }

            using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, template);
            }
        }

        private static MapLineSerializationTemplate GetMaplineTemplate(MapLine obj)
        {
            MapLineSerializationTemplate template = new MapLineSerializationTemplate();
            template.LineColor = obj.LineColor;
            template.LineThickness = obj.LineThickness;
            template.LineDashArray = obj.LineDashArray;
            template.StartPoint = obj.StartPoint;
            template.EndPoint = obj.EndPoint;
            return template;
        }

        private static CircleSerializationTemplate GetCircleTemplate(Circle obj)
        {
            CircleSerializationTemplate template = new CircleSerializationTemplate();
            template.Color = obj.Color;
            template.Thickness = obj.Thickness;
            template.StrokeDashArray = obj.StrokeDashArray;
            template.Center = obj.Center;
            template.Radius = obj.Radius;
            return template;
        }

        private static PolygonSerializationTemplate GetPolygonTemplate(Polygon obj)
        {
            PolygonSerializationTemplate template = new PolygonSerializationTemplate();
            template.Color = obj.Color;
            template.Thickness = obj.Thickness;
            template.DashArray = obj.DashArray;

            foreach (var line in obj.Lines)
            {
                PolygonLineSerializationTemplate line_template = new PolygonLineSerializationTemplate();
                line_template.LineColor = line.LineColor;
                line_template.LineThickness = line.LineThickness;
                line_template.LineDashArray = line.LineDashArray;
                line_template.StartPoint = line.StartPoint;
                line_template.EndPoint = line.EndPoint;
                template.Lines.Add(line_template);
            }

            return template;
        }

        private static RouteSerializationTemplate GetRouteTemplate(Route obj)
        {
            RouteSerializationTemplate template = new RouteSerializationTemplate();
            template.Color = obj.Color;
            template.Thickness = obj.Thickness;
            template.DashArray = obj.DashArray;
            template.Aircraft = obj.Aircraft;
            template.Mission = obj.Mission;
            template.StartingFuel = obj.StartingFuel;
            template.Minima = obj.Minima;

            foreach (var leg in obj.Legs)
            {
                RouteLegSerializationTemplate leg_template = new RouteLegSerializationTemplate();
                leg_template.LineColor = leg.LineColor;
                leg_template.LineThickness = leg.LineThickness;
                leg_template.LineDashArray = leg.LineDashArray;
                leg_template.PanelOffsetX = leg.PanelOffsetX;
                leg_template.PanelOffsetY = leg.PanelOffsetY;
                leg_template.IsFlipped = leg.IsFlipped;
                leg_template.Name = leg.Name;
                leg_template.StartPoint = leg.StartPoint;
                leg_template.EndPoint = leg.EndPoint;
                leg_template.Altitude = leg.Altitude;
                leg_template.Speed = leg.Speed;
                leg_template.Type = leg.Type;
                leg_template.FuelAdjustment = leg.FuelAdjustment;

                foreach (var checkpoint in leg.Checkpoints)
                {
                    CheckpointSerializationTemplate checkpoint_template = new CheckpointSerializationTemplate();
                    checkpoint_template.IsFlipped = checkpoint.IsFlipped;
                    checkpoint_template.Distance = checkpoint.Distance;
                    leg_template.Checkpoints.Add(checkpoint_template);
                }

                template.Legs.Add(leg_template);
            }

            foreach (var divleg in obj.DiversionLegs)
            {
                DiversionLegSerializationTemplate divleg_template = new DiversionLegSerializationTemplate();
                divleg_template.LineColor = divleg.LineColor;
                divleg_template.LineThickness = divleg.LineThickness;
                divleg_template.LineDashArray = divleg.LineDashArray;
                divleg_template.ContentOrientation = divleg.ContentOrientation;
                divleg_template.ContentOffset = divleg.ContentOffset;
                divleg_template.ContentFlip = divleg.ContentFlip;
                divleg_template.LinkingLegIndex = divleg.LinkingLegIndex;
                divleg_template.LinkingNode = divleg.LinkingNode;
                divleg_template.Name = divleg.Name;
                divleg_template.StartPoint = divleg.StartPoint;
                divleg_template.EndPoint = divleg.EndPoint;
                divleg_template.Altitude = divleg.Altitude;
                divleg_template.Speed = divleg.Speed;

                //foreach(var checkpoint in divleg.Checkpoints)
                //{
                //    CheckpointSerializationTemplate checkpoint_template = new CheckpointSerializationTemplate();
                //    checkpoint_template.IsFlipped = checkpoint.IsFlipped;
                //    checkpoint_template.Distance = checkpoint.Distance;
                //    divleg_template.Checkpoints.Add(checkpoint_template);
                //}

                template.DiversionLegs.Add(divleg_template);
            }

            return template;
        }
        #endregion

        #region Deserializers
        public static List<object> DeSerializeData(string filepath)
        {
            RootSerializationTemplate template;
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter bf = new BinaryFormatter();
                template = (RootSerializationTemplate)bf.Deserialize(fs);
            }

            List<object> drawables = new List<object>();

            foreach (var mapline in template.MapLines) drawables.Add(GetMapLine(mapline));
            foreach (var circle in template.Circles) drawables.Add(GetCircle(circle));
            foreach (var polygon in template.Polygons) drawables.Add(GetPolygon(polygon));
            foreach (var route in template.Routes) drawables.Add(GetRoute(route));

            return drawables;
        }

        public static MapLine GetMapLine(MapLineSerializationTemplate template)
        {
            MapLine obj = new MapLine();
            obj.LineColor = template.LineColor;
            obj.LineThickness = template.LineThickness;
            obj.LineDashArray = template.LineDashArray;
            obj.StartPoint = template.StartPoint;
            obj.EndPoint = template.EndPoint;
            return obj;
        }

        public static Circle GetCircle(CircleSerializationTemplate template)
        {
            Circle obj = new Circle();
            obj.Color = template.Color;
            obj.Thickness = template.Thickness;
            obj.StrokeDashArray = template.StrokeDashArray;
            obj.Center = template.Center;
            obj.Radius = template.Radius;
            return obj;
        }

        public static Polygon GetPolygon(PolygonSerializationTemplate template)
        {
            Polygon obj = new Polygon();
            obj.Color = template.Color;
            obj.Thickness = template.Thickness;
            obj.DashArray = template.DashArray;

            foreach (var line in template.Lines)
            {
                PolygonLine line_obj = new PolygonLine(obj);
                line_obj.LineColor = line.LineColor;
                line_obj.LineThickness = line.LineThickness;
                line_obj.LineDashArray = line.LineDashArray;
                line_obj.StartPoint = line.StartPoint;
                line_obj.EndPoint = line.EndPoint;
                obj.Lines.Add(line_obj);
            }

            return obj;
        }

        public static Route GetRoute(RouteSerializationTemplate template)
        {
            Route obj = new Route();
            obj.Color = template.Color;
            obj.Thickness = template.Thickness;
            obj.DashArray = template.DashArray;
            obj.Aircraft = template.Aircraft;
            obj.Mission = template.Mission;
            obj.StartingFuel = template.StartingFuel;
            obj.Minima = template.Minima;

            foreach (var leg in template.Legs)
            {
                RouteLeg leg_obj = new RouteLeg(obj);
                leg_obj.LineColor = leg.LineColor;
                leg_obj.LineThickness = leg.LineThickness;
                leg_obj.LineDashArray = leg.LineDashArray;
                leg_obj.PanelOffsetX = leg.PanelOffsetX;
                leg_obj.PanelOffsetY = leg.PanelOffsetY;
                leg_obj.IsFlipped = leg.IsFlipped;
                leg_obj.Name = leg.Name;
                leg_obj.StartPoint = leg.StartPoint;
                leg_obj.EndPoint = leg.EndPoint;
                leg_obj.Altitude = leg.Altitude;
                leg_obj.Speed = leg.Speed;
                leg_obj.Type = leg.Type;
                leg_obj.FuelAdjustment = leg.FuelAdjustment;

                foreach (var checkpoint in leg.Checkpoints)
                {
                    Checkpoint checkpoint_obj = new Checkpoint(leg_obj);
                    checkpoint_obj.IsFlipped = checkpoint.IsFlipped;
                    checkpoint_obj.Distance = checkpoint.Distance;
                    leg_obj.Checkpoints.Add(checkpoint_obj);
                }

                obj.Legs.Add(leg_obj);
            }

            foreach (var divleg in template.DiversionLegs)
            {
                DiversionLeg divleg_obj = new DiversionLeg(obj);
                divleg_obj.LineColor = divleg.LineColor;
                divleg_obj.LineThickness = divleg.LineThickness;
                divleg_obj.LineDashArray = divleg.LineDashArray;
                divleg_obj.ContentOrientation = divleg.ContentOrientation;
                divleg_obj.ContentOffset = divleg.ContentOffset;
                divleg_obj.ContentFlip = divleg.ContentFlip;
                divleg_obj.LinkingLegIndex = divleg.LinkingLegIndex;
                divleg_obj.LinkingNode = divleg.LinkingNode;
                divleg_obj.Name = divleg.Name;
                divleg_obj.StartPoint = divleg.StartPoint;
                divleg_obj.EndPoint = divleg.EndPoint;
                divleg_obj.Altitude = divleg.Altitude;
                divleg_obj.Speed = divleg.Speed;

                //foreach (var checkpoint in divleg.Checkpoints)
                //{
                //    Checkpoint checkpoint_obj = new Checkpoint(divleg_obj);
                //    checkpoint_obj.IsFlipped = checkpoint.IsFlipped;
                //    checkpoint_obj.Distance = checkpoint.Distance;
                //    divleg_obj.Checkpoints.Add(checkpoint_obj);
                //}

                obj.DiversionLegs.Add(divleg_obj);
            }

            return obj;
        }
        #endregion
    }
}
