using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;
using GMap.NET;
using GMap.NET.WindowsPresentation;

namespace Mission_Assistant
{
    class CheckpointData
    {
        private RouteData parentData;
        private string spdunit;
        private double speed = 0;
        private double chkconst = 0;
        private double neffdst = 0;
        private double effdst = 0;

        public int routenum = -1;
        public int linenum = -1;
        public int checkpointnum = -1;
        public double distance = 0;
        public double time = 0;
        public bool isMark = false;

        public CheckpointData(GMapControl map, Line ml, Ellipse node, Point setPoint)
        {
            parentData = node.Tag as RouteData;
            routenum = parentData.objID;
            linenum = (ml.Tag as RouteData).componentID;
            spdunit = parentData.baseSpeedunit;
            PointLatLng point1 = map.FromLocalToLatLng((int)ml.X1, (int)ml.Y1);
            PointLatLng point2 = map.FromLocalToLatLng((int)setPoint.X, (int)setPoint.Y);
            distance = new MapRoute(new List<PointLatLng>() { point1, point2 }, "D").Distance;
            updateData();
        }

        public CheckpointData(Line ml, Ellipse node, string mark)
        {
            isMark = true;
            parentData = node.Tag as RouteData;
            routenum = parentData.objID;
            linenum = (ml.Tag as RouteData).componentID;
            if (mark == "TOC") distance = parentData.neffectivedst;
            else if (mark == "TOD") distance = DataConverters.LengthUnits(parentData.distance, parentData.baseDistunit, "KM") - parentData.effectivedst;
        }

        private void pullData()
        {
            speed = parentData.speed;
            neffdst = parentData.neffectivedst;
            effdst = parentData.effectivedst;
            chkconst = parentData.checkpointconst;
        }

        private void calcTime()
        {
            if (speed == 0) time = 0;
            else if (distance < neffdst || distance > effdst) time = 0;
            else time = chkconst + TimeSpan.FromHours((distance - neffdst) / DataConverters.SpeedUnits(speed, spdunit, "KPH")).TotalSeconds;
        }

        public void updateData()
        {
            pullData();
            calcTime();
        }

        public void updateData(string mark)
        {
            if (mark == "TOC")
            {
                double temp = parentData.neffectivedst;
                if (temp < 0) return;
                else distance = temp;
            }
            else if (mark == "TOD")
            {
                double temp = DataConverters.LengthUnits(parentData.distance, parentData.baseDistunit, "KM") - parentData.effectivedst;
                if (temp < 0) return;
                else distance = temp;
            }
        }
    }
}
