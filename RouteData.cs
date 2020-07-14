using GMap.NET;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Mission_Assistant
{
    class RouteData
    {
        public string baseAltunit;
        public string baseDistunit;
        public string baseFuelunit;
        public string baseSpeedunit;
        public string baseLfftunit;
        private double climbtime = -1;
        private double climbdist = -1;
        private double climbfuel = -1;
        private double descendtime = -1;
        private double descenddist = -1;
        private double descendfuel = -1;
        private double _lfft = -1;
        private double lfft
        {
            get
            {
                return _lfft;
            }
            set
            {
                _lfft = value;
                if (!pos1.IsEmpty && !pos2.IsEmpty)
                {
                    calcData();
                }
            }
        }

        public Canvas parent;
        public string objType = String.Empty;
        public int objID = -1;
        public int componentID = -1;
        public string aircraft = String.Empty;
        public string mission = String.Empty;
        public double totaldistance = 0;
        public string totaldistanceUnit;
        public double totaltime = 0;
        public double totalfuel = 0;
        public double startingfuel = 0;
        public double minima = 0;
        public string name = String.Empty;
        public double alt = 0;
        public double distance = 0;
        public string distanceUnit;
        public double speed = 0;
        public double track = 0;
        public double time = 0;
        public double fuel = 0;
        public double rdfuel = 0;
        public bool isDraggable;

        private PointLatLng _pos1 = new PointLatLng();
        private PointLatLng _pos2 = new PointLatLng();
        private string _type = String.Empty;
        public PointLatLng pos1
        {
            get
            {
                return _pos1;
            }
            set
            {
                _pos1 = value;
                if (!_pos2.IsEmpty && lfft != -1)
                {
                    calcData();
                }
            }
        }
        public PointLatLng pos2
        {
            get
            {
                return _pos2;
            }
            set
            {
                _pos2 = value;
                if (!_pos1.IsEmpty && lfft != -1)
                {
                    calcData();
                }
            }
        }
        public string type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                if (!pos1.IsEmpty && !pos2.IsEmpty && lfft != -1)
                {
                    calcData();
                }
            }
        }

        public RouteData()
        {
            
        }

        public RouteData(Canvas input, string aUnit, string dUnit, string sUnit, string fUnit, string lUnit)
        {
            parent = input;
            baseAltunit = aUnit;
            baseDistunit = dUnit;
            baseSpeedunit = sUnit;
            baseFuelunit = fUnit;
            baseLfftunit = lUnit;
        }

        private void calcTime()
        {
            if (type == "Starting")
            {
                if (speed == 0) time = 0;
                else time = climbtime + TimeSpan.FromHours(DataConverters.LengthUnits(distance - climbdist, baseDistunit, "KM") / DataConverters.SpeedUnits(speed, baseSpeedunit, "KPH")).TotalSeconds;
            }
            else if (type == "Landing")
            {
                if (speed == 0) time = 0;
                else time = descendtime + TimeSpan.FromHours(DataConverters.LengthUnits(distance - descenddist, baseDistunit, "KM") / DataConverters.SpeedUnits(speed, baseSpeedunit, "KPH")).TotalSeconds;
            }
            else
            {
                if (speed == 0) time = 0;
                else time = TimeSpan.FromHours(DataConverters.LengthUnits(distance, baseDistunit, "KM") / DataConverters.SpeedUnits(speed, baseSpeedunit, "KPH")).TotalSeconds;
            }
        }

        private void calcFuel()
        {
            if (baseLfftunit == "PER KM")
            {
                if (type == "Starting")
                {
                    fuel = climbfuel + DataConverters.LengthUnits(distance - climbdist, baseDistunit, "KM") * lfft;
                }
                else if (type == "Landing")
                {
                    fuel = descendfuel + DataConverters.LengthUnits(distance - descenddist, baseDistunit, "KM") * lfft;
                }
                else
                {
                    fuel = DataConverters.LengthUnits(distance, baseDistunit, "KM") * lfft;
                }
            }
            else if (baseLfftunit == "PER MIN")
            {
                if (type == "Starting")
                {
                    fuel = climbfuel + TimeSpan.FromSeconds(time - climbtime).TotalMinutes * lfft;
                }
                else if (type == "Landing")
                {
                    fuel = descendfuel + DataConverters.LengthUnits(distance - descenddist, baseDistunit, "KM") * lfft;
                }
                else
                {
                    fuel = TimeSpan.FromSeconds(time).TotalMinutes * lfft;
                }
            }
        }

        private void calcData()
        {
            distance = DataConverters.LengthUnits((new MapRoute(new List<PointLatLng>() { pos1, pos2 }, "L")).Distance, "KM", baseDistunit);
            calcTime();
            calcFuel();
            totaldistance = 0;
            totaltime = 0;
            totalfuel = 0;
            for (int i = 0; i < parent.Children.Count; i++)
            {
                if (parent.Children[i] is Ellipse)
                {
                    if (((parent.Children[i] as Ellipse).Tag as RouteData).objType == objType && ((parent.Children[i] as Ellipse).Tag as RouteData).objID == objID)
                    {
                        totaldistance += ((parent.Children[i] as Ellipse).Tag as RouteData).distance;
                        totaltime += ((parent.Children[i] as Ellipse).Tag as RouteData).time;
                        totalfuel += ((parent.Children[i] as Ellipse).Tag as RouteData).fuel;
                    }
                }
            }
            for (int i = 0; i < parent.Children.Count; i++)
            {
                if (parent.Children[i] is Line)
                {
                    if (((parent.Children[i] as Line).Tag as RouteData).objType == objType && ((parent.Children[i] as Line).Tag as RouteData).objID == objID)
                    {
                        ((parent.Children[i] as Line).Tag as RouteData).totaldistance = totaldistance;
                        ((parent.Children[i] as Line).Tag as RouteData).totaltime = totaltime;
                        ((parent.Children[i] as Line).Tag as RouteData).totalfuel = totalfuel;
                    }
                }
                else if (parent.Children[i] is Ellipse)
                {
                    if (((parent.Children[i] as Ellipse).Tag as RouteData).objType == objType && ((parent.Children[i] as Ellipse).Tag as RouteData).objID == objID)
                    {
                        ((parent.Children[i] as Ellipse).Tag as RouteData).totaldistance = totaldistance;
                        ((parent.Children[i] as Ellipse).Tag as RouteData).totaltime = totaltime;
                        ((parent.Children[i] as Ellipse).Tag as RouteData).totalfuel = totalfuel;
                    }
                }
            }
        }

        public void setpData(PerformanceData pd, int index, int spid)
        {
            List<double> speeds = new List<double>() { pd.spd1, pd.spd2, pd.spd3, pd.spd4, pd.spd5 };
            for (int i = 0; i < spid; i++)
            {
                if (spid == 6) break;
                if (speeds[i] == 0) spid++;
            }
            alt = pd.performanceDatas[index].alt;
            climbtime = pd.performanceDatas[index].climbtime;
            climbdist = pd.performanceDatas[index].climbdist;
            climbfuel = pd.performanceDatas[index].climbfuel;
            descenddist = pd.performanceDatas[index].descenddist;
            descendtime = pd.performanceDatas[index].descendtime;
            descendfuel = pd.performanceDatas[index].descendfuel;
            switch (spid)
            {
                case 1:
                    speed = pd.spd1;
                    lfft = pd.performanceDatas[index].spd1;
                    break;
                case 2:
                    speed = pd.spd2;
                    lfft = pd.performanceDatas[index].spd2;
                    break;
                case 3:
                    speed = pd.spd3;
                    lfft = pd.performanceDatas[index].spd3;
                    break;
                case 4:
                    speed = pd.spd4;
                    lfft = pd.performanceDatas[index].spd4;
                    break;
                case 5:
                    speed = pd.spd5;
                    lfft = pd.performanceDatas[index].spd5;
                    break;
                default:
                    speed = 0;
                    lfft = 0;
                    break;
            }
        }
    }
}
