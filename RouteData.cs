﻿using GMap.NET;
using System;
using System.Collections.Generic;
using System.Windows;
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
        private double sutto = 0;
        private double climbtime = 0;
        private double climbdist = 0;
        private double climbfuel = 0;
        private double descendtime = 0;
        private double descenddist = 0;
        private double descendfuel = 0;
        private double prev_climbtime = 0;
        private double prev_climbdist = 0;
        private double prev_climbfuel = 0;
        private double prev_descendtime = 0;
        private double prev_descenddist = 0;
        private double prev_descendfuel = 0;
        private double prev_remfuel = 0;
        private double prev_fuel = 0;
        private double prev_alt = 0;
        private double next_frcsfuel = 0;
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
                    if (type == "Diversion") calcDiversionData();
                    else calcData();
                }
            }
        }

        public Canvas parent;
        public string objType = String.Empty;
        public int objID = -1;
        public int componentID = -1;
        public bool rev = false;


        public string aircraft = String.Empty;
        public string mission = String.Empty;
        public double totaldistance = 0;
        public string totaldistanceUnit;
        public double totaltime = 0;
        public double totalfuel = 0;
        public string name = String.Empty;
        public double alt = 0;
        public double distance = 0;
        public string distanceUnit;
        public double speed = 0;
        public double track = 0;
        public double time = 0;
        public double fuel = 0;
        public double remfuel = 0;
        public double landingrem = 0;
        public double frcsfuel = 0;
        public double landingfrcs = 0;
        public float offsetX = 20;
        public float offsetY = 0;
        public double checkpointconst = 0;
        public double neffectivedst = 0;
        public double effectivedst = 0;
        public bool isDraggable;
        public string transition = @"flat";

        private PointLatLng _pos1 = new PointLatLng();
        private PointLatLng _pos2 = new PointLatLng();
        private string _type = String.Empty;
        private double _startingfuel = 0;
        private double _minima = 0;
        private double _rdfuel = 0;
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
                    if (type == "Diversion") calcDiversionData();
                    else calcData();
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
                    if (type == "Diversion") calcDiversionData();
                    else calcData();
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
                    if (type == "Diversion") calcDiversionData();
                    else calcData();
                }
            }
        }
        public double startingfuel
        {
            get
            {
                return _startingfuel;
            }
            set
            {
                _startingfuel = value;
                if (!pos1.IsEmpty && !pos2.IsEmpty && lfft != -1)
                {
                    calcData();
                }
            }
        }
        public double minima
        {
            get
            {
                return _minima;
            }
            set
            {
                _minima = value;
                if (!pos1.IsEmpty && !pos2.IsEmpty && lfft != -1)
                {
                    calcData();
                }
            }
        }
        public double rdfuel
        {
            get
            {
                return _rdfuel;
            }
            set
            {
                _rdfuel = value;
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

        public void calcTime()
        {
            if (type == "Starting")
            {
                if (speed == 0) time = 0;
                else if (distance < climbdist)
                {
                    time = 0;
                }
                else
                {
                    transition = @"climb";
                    neffectivedst = DataConverters.LengthUnits(climbdist, baseDistunit, "KM");
                    effectivedst = DataConverters.LengthUnits(distance, baseDistunit, "KM");
                    checkpointconst = climbtime - TimeSpan.FromHours(DataConverters.LengthUnits(climbdist, baseDistunit, "KM") / DataConverters.SpeedUnits(speed, baseSpeedunit, "KPH")).TotalSeconds;
                    time = climbtime + TimeSpan.FromHours(DataConverters.LengthUnits(distance - climbdist, baseDistunit, "KM") / DataConverters.SpeedUnits(speed, baseSpeedunit, "KPH")).TotalSeconds;
                }
            }
            else if (type == "Origin")
            {
                time = 0;
            }
            else if (type == "Landing" || type == "Diversion")
            {
                if (speed == 0) time = 0;
                else
                {
                    pullprevData();
                    if (type == "Landing" && componentID == 1)
                    {
                        if (distance < descenddist + climbdist) time = 0;
                        else
                        {
                            transition = @"climb";
                            neffectivedst = DataConverters.LengthUnits(climbdist, baseDistunit, "KM");
                            effectivedst = DataConverters.LengthUnits(distance - descenddist, baseDistunit, "KM");
                            checkpointconst = climbtime + descendtime - TimeSpan.FromHours(DataConverters.LengthUnits(descenddist + climbdist, baseDistunit, "KM") / DataConverters.SpeedUnits(speed, baseSpeedunit, "KPH")).TotalSeconds;
                            time = climbtime + descendtime + TimeSpan.FromHours(DataConverters.LengthUnits(distance - (descenddist + climbdist), baseDistunit, "KM") / DataConverters.SpeedUnits(speed, baseSpeedunit, "KPH")).TotalSeconds;
                        }
                        return;
                    }
                    if (alt > prev_alt)
                    {
                        if (distance < descenddist + (climbdist - prev_climbdist))
                        {
                            time = 0;
                        }
                        else
                        {
                            transition = @"climb";
                            neffectivedst = DataConverters.LengthUnits(climbdist - prev_climbdist, baseDistunit, "KM");
                            effectivedst = DataConverters.LengthUnits(distance - descenddist, baseDistunit, "KM");
                            checkpointconst = (climbtime - prev_climbtime) + descendtime - TimeSpan.FromHours(DataConverters.LengthUnits(descenddist + (climbdist - prev_climbdist), baseDistunit, "KM") / DataConverters.SpeedUnits(speed, baseSpeedunit, "KPH")).TotalSeconds;
                            time = (climbtime - prev_climbtime) + descendtime + TimeSpan.FromHours(DataConverters.LengthUnits(distance - (descenddist + (climbdist - prev_climbdist)), baseDistunit, "KM") / DataConverters.SpeedUnits(speed, baseSpeedunit, "KPH")).TotalSeconds;
                        }
                    }
                    else if (alt < prev_alt)
                    {
                        if (distance < descenddist + (prev_descenddist - descenddist))
                        {
                            time = 0;
                        }
                        else
                        {
                            transition = @"descend";
                            neffectivedst = DataConverters.LengthUnits(prev_descenddist - descenddist, baseDistunit, "KM");
                            effectivedst = DataConverters.LengthUnits(distance - descenddist, baseDistunit, "KM");
                            checkpointconst = (prev_descendtime - descendtime) + descendtime - TimeSpan.FromHours(DataConverters.LengthUnits(descenddist + (prev_descenddist - descenddist), baseDistunit, "KM") / DataConverters.SpeedUnits(speed, baseSpeedunit, "KPH")).TotalSeconds;
                            time = (prev_descendtime - descendtime) + descendtime + TimeSpan.FromHours(DataConverters.LengthUnits(distance - (descenddist + (prev_descenddist - descenddist)), baseDistunit, "KM") / DataConverters.SpeedUnits(speed, baseSpeedunit, "KPH")).TotalSeconds;
                        }
                    }
                    else
                    {
                        if (distance < descenddist)
                        {
                            time = 0;
                        }
                        else
                        {
                            transition = @"flat";
                            neffectivedst = DataConverters.LengthUnits(0, baseDistunit, "KM");
                            effectivedst = DataConverters.LengthUnits(distance - descenddist, baseDistunit, "KM");
                            checkpointconst = descendtime - TimeSpan.FromHours(DataConverters.LengthUnits(descenddist, baseDistunit, "KM") / DataConverters.SpeedUnits(speed, baseSpeedunit, "KPH")).TotalSeconds;
                            time = descendtime + TimeSpan.FromHours(DataConverters.LengthUnits(distance - descenddist, baseDistunit, "KM") / DataConverters.SpeedUnits(speed, baseSpeedunit, "KPH")).TotalSeconds;
                        }
                    }
                }
            }
            else
            {
                if (speed == 0) time = 0;
                else
                {
                    pullprevData();
                    if (alt > prev_alt)
                    {
                        if (distance < climbdist - prev_climbdist)
                        {
                            time = 0;
                        }
                        else
                        {
                            transition = @"climb";
                            neffectivedst = DataConverters.LengthUnits(climbdist - prev_climbdist, baseDistunit, "KM");
                            effectivedst = DataConverters.LengthUnits(distance, baseDistunit, "KM");
                            checkpointconst = (climbtime - prev_climbtime) - TimeSpan.FromHours(DataConverters.LengthUnits(climbdist - prev_climbdist, baseDistunit, "KM") / DataConverters.SpeedUnits(speed, baseSpeedunit, "KPH")).TotalSeconds;
                            time = (climbtime - prev_climbtime) + TimeSpan.FromHours(DataConverters.LengthUnits(distance - (climbdist - prev_climbdist), baseDistunit, "KM") / DataConverters.SpeedUnits(speed, baseSpeedunit, "KPH")).TotalSeconds;
                        }
                    }
                    else if (alt < prev_alt)
                    {
                        if (distance < prev_descenddist - descenddist)
                        {
                            time = 0;
                        }
                        else
                        {
                            transition = @"descend";
                            neffectivedst = DataConverters.LengthUnits(prev_descenddist - descenddist, baseDistunit, "KM");
                            effectivedst = DataConverters.LengthUnits(distance, baseDistunit, "KM");
                            checkpointconst = (prev_descendtime - descendtime) - TimeSpan.FromHours(DataConverters.LengthUnits(prev_descenddist - descenddist, baseDistunit, "KM") / DataConverters.SpeedUnits(speed, baseSpeedunit, "KPH")).TotalSeconds;
                            time = (prev_descendtime - descendtime) + TimeSpan.FromHours(DataConverters.LengthUnits(distance - (prev_descenddist - descenddist), baseDistunit, "KM") / DataConverters.SpeedUnits(speed, baseSpeedunit, "KPH")).TotalSeconds;
                        }
                    }
                    else
                    {
                        transition = @"flat";
                        neffectivedst = DataConverters.LengthUnits(0, baseDistunit, "KM");
                        effectivedst = DataConverters.LengthUnits(distance, baseDistunit, "KM");
                        checkpointconst = 0;
                        time = TimeSpan.FromHours(DataConverters.LengthUnits(distance, baseDistunit, "KM") / DataConverters.SpeedUnits(speed, baseSpeedunit, "KPH")).TotalSeconds;
                    }
                }
            }
        }

        public void calcFuel()
        {
            if (baseLfftunit == "PER KM")
            {
                if (type == "Starting")
                {
                    if (rev)
                    {
                        pullnextData();
                        frcsfuel = next_frcsfuel + fuel + sutto;
                        return;
                    }
                    if (distance < climbdist) fuel = 0;
                    else fuel = climbfuel + rdfuel + DataConverters.LengthUnits(distance - climbdist, baseDistunit, "KM") * lfft;
                    remfuel = startingfuel - sutto;
                }
                else if (type == "Origin")
                {
                    fuel = 0;
                    remfuel = startingfuel - sutto;
                    frcsfuel = 0;
                }
                else if (type == "Landing")
                {
                    if (rev)
                    {
                        landingfrcs = minima;
                        frcsfuel = landingfrcs + fuel;
                        return;
                    }
                    if (componentID == 1)
                    {
                        if (distance < climbdist + descenddist) fuel = 0;
                        else fuel = climbfuel + descendfuel + rdfuel + DataConverters.LengthUnits(distance - (climbdist + descenddist), baseDistunit, "KM") * lfft;
                        remfuel = startingfuel - sutto;
                        landingrem = remfuel - fuel;
                        return;
                    }
                    if (alt > prev_alt)
                    {
                        if (distance < (climbdist - prev_climbdist) + descenddist) fuel = 0;
                        else fuel = (climbfuel - prev_climbfuel) + descendfuel + rdfuel + DataConverters.LengthUnits(distance - ((climbdist - prev_climbdist) + descenddist), baseDistunit, "KM") * lfft;
                    }
                    else if (alt < prev_alt)
                    {
                        if (distance < (prev_descenddist - descenddist) + descenddist) fuel = 0;
                        else fuel = (prev_descendfuel - descendfuel) + descendfuel + rdfuel + DataConverters.LengthUnits(distance - ((prev_descenddist - descenddist) + descenddist), baseDistunit, "KM") * lfft;
                    }
                    else
                    {
                        if (distance < descenddist) fuel = 0;
                        else fuel = descendfuel + rdfuel + DataConverters.LengthUnits(distance - descenddist, baseDistunit, "KM") * lfft;
                    }
                    remfuel = prev_remfuel - prev_fuel;
                    landingrem = remfuel - fuel;
                }
                else
                {
                    if (rev)
                    {
                        pullnextData();
                        frcsfuel = next_frcsfuel + fuel;
                        return;
                    }
                    if (alt > prev_alt)
                    {
                        if (distance < climbdist - prev_climbdist) fuel = 0;
                        else fuel = (climbfuel - prev_climbfuel) + rdfuel + DataConverters.LengthUnits(distance - (climbdist - prev_climbdist), baseDistunit, "KM") * lfft;
                    }
                    else if (alt < prev_alt)
                    {
                        if (distance < prev_descenddist - descenddist) fuel = 0;
                        else fuel = (prev_descendfuel - descendfuel) + rdfuel + DataConverters.LengthUnits(distance - (prev_descenddist - descenddist), baseDistunit, "KM") * lfft;
                    }
                    else fuel = rdfuel + DataConverters.LengthUnits(distance, baseDistunit, "KM") * lfft;
                    remfuel = prev_remfuel - prev_fuel;
                }
            }
            else if (baseLfftunit == "PER MIN")
            {
                if (type == "Starting")
                {
                    if (rev)
                    {
                        pullnextData();
                        frcsfuel = next_frcsfuel + fuel + sutto;
                        return;
                    }
                    if (time < climbtime) fuel = 0;
                    else fuel = climbfuel + rdfuel + TimeSpan.FromSeconds(time - climbtime).TotalMinutes * lfft;
                    remfuel = startingfuel - sutto;
                }
                else if (type == "Origin")
                {
                    fuel = 0;
                    remfuel = startingfuel - sutto;
                    frcsfuel = 0;
                }
                else if (type == "Landing")
                {
                    if (rev)
                    {
                        landingfrcs = minima;
                        frcsfuel = landingfrcs + fuel;
                        return;
                    }
                    if (componentID == 1)
                    {
                        if (time < climbtime + descendtime) fuel = 0;
                        else fuel = climbfuel + descendfuel + rdfuel + TimeSpan.FromSeconds(time - (climbtime + descendtime)).TotalMinutes * lfft;
                        remfuel = startingfuel - sutto;
                        landingrem = remfuel - fuel;
                        if (rev)
                        {
                            landingfrcs = minima;
                            frcsfuel = landingfrcs + fuel;
                        }
                        return;
                    }
                    if (alt > prev_alt)
                    {
                        if (time < (climbtime - prev_climbtime) + descendtime) fuel = 0;
                        else fuel = (climbfuel - prev_climbfuel) + descendfuel + rdfuel + TimeSpan.FromSeconds(time - ((climbtime - prev_climbtime) + descendtime)).TotalMinutes * lfft;
                    }
                    else if (alt < prev_alt)
                    {
                        if (time < (prev_descendtime - descendtime) + descendtime) fuel = 0;
                        else fuel = (prev_descendfuel - descendfuel) + descendfuel + rdfuel + TimeSpan.FromSeconds(time - ((prev_descendtime - descendtime) + descendtime)).TotalMinutes * lfft;
                    }
                    else
                    {
                        if (time < descendtime) fuel = 0;
                        else fuel = descendfuel + rdfuel + TimeSpan.FromSeconds(time - descendtime).TotalMinutes * lfft;
                    }
                    remfuel = prev_remfuel - prev_fuel;
                    landingrem = remfuel - fuel;
                }
                else
                {
                    if (rev)
                    {
                        pullnextData();
                        frcsfuel = next_frcsfuel + fuel;
                        return;
                    }
                    if (alt > prev_alt)
                    {
                        if (time < climbtime - prev_climbtime) fuel = 0;
                        else fuel = (climbfuel - prev_climbfuel) + rdfuel + TimeSpan.FromSeconds(time - (climbtime - prev_climbtime)).TotalMinutes * lfft;
                    }
                    else if (alt < prev_alt)
                    {
                        if (time < prev_descendtime - descendtime) fuel = 0;
                        else fuel = (prev_descendfuel - descendfuel) + rdfuel + TimeSpan.FromSeconds(time - (prev_descendtime - descendtime)).TotalMinutes * lfft;
                    }
                    else fuel = rdfuel + TimeSpan.FromSeconds(time).TotalMinutes * lfft;
                    remfuel = prev_remfuel - prev_fuel;
                }
            }
        }

        private void calcData()
        {
            totaldistance = 0;
            totaltime = 0;
            totalfuel = 0;
            track = Math.Atan2(Math.Cos(pos2.Lat * Math.PI / 180) * Math.Sin((pos2.Lng - pos1.Lng) * Math.PI / 180), Math.Cos(pos1.Lat * Math.PI / 180) * Math.Sin(pos2.Lat * Math.PI / 180) - Math.Sin(pos1.Lat * Math.PI / 180) * Math.Cos(pos2.Lat * Math.PI / 180) * Math.Cos((pos2.Lng - pos1.Lng) * Math.PI / 180)) * 180 / Math.PI;
            if (track < 0) track += 360;
            for (int i = 0; i < parent.Children.Count; i++)
            {
                if (parent.Children[i] is Ellipse)
                {
                    if (((parent.Children[i] as Ellipse).Tag as RouteData).objType == objType && ((parent.Children[i] as Ellipse).Tag as RouteData).objID == objID)
                    {
                        if (((parent.Children[i] as Ellipse).Tag as RouteData).type == "Origin") continue;
                        if (((parent.Children[i] as Ellipse).Tag as RouteData).type == "Diversion")
                        {
                            ((parent.Children[i] as Ellipse).Tag as RouteData).calcDiversionData();
                            continue;
                        }
                        ((parent.Children[i] as Ellipse).Tag as RouteData).distance = DataConverters.LengthUnits((new MapRoute(new List<PointLatLng>() { ((parent.Children[i] as Ellipse).Tag as RouteData).pos1, ((parent.Children[i] as Ellipse).Tag as RouteData).pos2 }, "L")).Distance, "KM", baseDistunit);
                        ((parent.Children[i] as Ellipse).Tag as RouteData).calcTime();
                        ((parent.Children[i] as Ellipse).Tag as RouteData).calcFuel();
                        totaldistance += ((parent.Children[i] as Ellipse).Tag as RouteData).distance;
                        totaltime += ((parent.Children[i] as Ellipse).Tag as RouteData).time;
                        totalfuel += ((parent.Children[i] as Ellipse).Tag as RouteData).fuel;
                        foreach (UIElement cnv in parent.Children)
                        {
                            if (cnv is Canvas)
                            {
                                foreach (UIElement stp in (cnv as Canvas).Children)
                                {
                                    if (stp is StackPanel)
                                    {
                                        if ((stp as StackPanel).Tag is CheckpointData)
                                        {
                                            if (((stp as StackPanel).Tag as CheckpointData).routenum == ((parent.Children[i] as Ellipse).Tag as RouteData).objID && ((stp as StackPanel).Tag as CheckpointData).linenum == ((parent.Children[i] as Ellipse).Tag as RouteData).componentID - 1)
                                            {
                                                ((stp as StackPanel).Tag as CheckpointData).updateData();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        ((parent.Children[i] as Ellipse).Tag as RouteData).rev = true;
                    }
                }
            }
            for (int i = parent.Children.Count - 1; i >= 0; i--)
            {
                if (parent.Children[i] is Ellipse)
                {
                    if (((parent.Children[i] as Ellipse).Tag as RouteData).objType == objType && ((parent.Children[i] as Ellipse).Tag as RouteData).objID == objID)
                    {
                        ((parent.Children[i] as Ellipse).Tag as RouteData).totaldistance = totaldistance;
                        ((parent.Children[i] as Ellipse).Tag as RouteData).totaltime = totaltime;
                        ((parent.Children[i] as Ellipse).Tag as RouteData).totalfuel = totalfuel;
                        if (((parent.Children[i] as Ellipse).Tag as RouteData).type == "Origin" || ((parent.Children[i] as Ellipse).Tag as RouteData).type == "Diversion") continue;
                        ((parent.Children[i] as Ellipse).Tag as RouteData).calcFuel();
                        ((parent.Children[i] as Ellipse).Tag as RouteData).rev = false;
                    }
                }
            }
        }

        private void calcDiversionData()
        {
            track = Math.Atan2(Math.Cos(pos2.Lat * Math.PI / 180) * Math.Sin((pos2.Lng - pos1.Lng) * Math.PI / 180), Math.Cos(pos1.Lat * Math.PI / 180) * Math.Sin(pos2.Lat * Math.PI / 180) - Math.Sin(pos1.Lat * Math.PI / 180) * Math.Cos(pos2.Lat * Math.PI / 180) * Math.Cos((pos2.Lng - pos1.Lng) * Math.PI / 180)) * 180 / Math.PI;
            if (track < 0) track += 360;
            distance = DataConverters.LengthUnits((new MapRoute(new List<PointLatLng>() { pos1, pos2 }, "L")).Distance, "KM", baseDistunit);
            calcTime();
            foreach (UIElement cnv in parent.Children)
            {
                if (cnv is Canvas)
                {
                    foreach (UIElement stp in (cnv as Canvas).Children)
                    {
                        if (stp is StackPanel)
                        {
                            if ((stp as StackPanel).Tag is CheckpointData)
                            {
                                if (((stp as StackPanel).Tag as CheckpointData).routenum == objID && ((stp as StackPanel).Tag as CheckpointData).linenum == componentID)
                                {
                                    ((stp as StackPanel).Tag as CheckpointData).updateData();
                                }
                            }
                        }
                    }
                }
            }
        }

        public void setpData(PerformanceData pd, int index, double spd)
        {
            speed = spd;
            if (index == -1)
            {
                alt = pd.alt;
                climbtime = pd.climbtime;
                climbdist = pd.climbdist;
                climbfuel = pd.climbfuel;
                descenddist = pd.descenddist;
                descendtime = pd.descendtime;
                descendfuel = pd.descendfuel;
                lfft = 0;
                return;
            }
            List<double> speeds = new List<double>() { pd.spd1, pd.spd2, pd.spd3, pd.spd4, pd.spd5 };
            int spid = 0;
            for (int i = 0; i < 5; i++)
            {
                if (speeds[i] == spd)
                {
                    spid = i + 1;
                    break;
                }
            }
            aircraft = pd.performanceDatas[index].aircraft;
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
                    lfft = pd.performanceDatas[index].spd1;
                    break;
                case 2:
                    lfft = pd.performanceDatas[index].spd2;
                    break;
                case 3:
                    lfft = pd.performanceDatas[index].spd3;
                    break;
                case 4:
                    lfft = pd.performanceDatas[index].spd4;
                    break;
                case 5:
                    lfft = pd.performanceDatas[index].spd5;
                    break;
                default:
                    lfft = 0;
                    break;
            }
        }

        public void setfData(FuelReduceData frd)
        {
            foreach (FuelReduceData fd in frd.fuelReduceDatas)
            {
                if (fd.reductionlabel.ToLower() == "sutto")
                {
                    sutto = fd.reductionval;
                }
            }
        }

        public void updateBaseUnits(string aUnit, string dUnit, string sUnit, string fUnit, string lUnit)
        {
            startingfuel = DataConverters.MassUnits(startingfuel, baseFuelunit, fUnit);
            minima = DataConverters.MassUnits(minima, baseFuelunit, fUnit);
            rdfuel = DataConverters.MassUnits(rdfuel, baseFuelunit, fUnit);
            baseAltunit = aUnit;
            baseDistunit = dUnit;
            baseSpeedunit = sUnit;
            baseFuelunit = fUnit;
            baseLfftunit = lUnit;
        }

        private void pullprevData()
        {
            for (int i = 0; i < parent.Children.Count; i++)
            {
                if (parent.Children[i] is Ellipse)
                {
                    if (((parent.Children[i] as Ellipse).Tag as RouteData).objType == objType && ((parent.Children[i] as Ellipse).Tag as RouteData).objID == objID && ((parent.Children[i] as Ellipse).Tag as RouteData).componentID == componentID - 1)
                    {
                        if (((parent.Children[i] as Ellipse).Tag as RouteData).type == "Origin") return;
                        RouteData prev = (parent.Children[i] as Ellipse).Tag as RouteData;
                        prev_alt = prev.alt;
                        prev_climbtime = prev.climbtime;
                        prev_climbdist = prev.climbdist;
                        prev_climbfuel = prev.climbfuel;
                        prev_descendtime = prev.descendtime;
                        prev_descenddist = prev.descenddist;
                        prev_descendfuel = prev.descendfuel;
                        prev_remfuel = prev.remfuel;
                        prev_fuel = prev.fuel;
                    }
                }
            }
        }

        private void pullnextData()
        {
            for (int i = 0; i < parent.Children.Count; i++)
            {
                if (parent.Children[i] is Ellipse)
                {
                    if (((parent.Children[i] as Ellipse).Tag as RouteData).objType == objType && ((parent.Children[i] as Ellipse).Tag as RouteData).objID == objID && ((parent.Children[i] as Ellipse).Tag as RouteData).componentID == componentID + 1)
                    {
                        if (((parent.Children[i] as Ellipse).Tag as RouteData).type == "Origin") return;
                        RouteData next = (parent.Children[i] as Ellipse).Tag as RouteData;
                        next_frcsfuel = next.frcsfuel;
                    }
                }
            }
        }

    }
}
