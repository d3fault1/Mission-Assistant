using GMap.NET;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MissionAssistant
{
    static class DataCalculations
    {
        const double earthRadius = 6371009.0;

        private static GMapControl map;
        private static Canvas mainCanvas;

        private static PointLatLng tempPointLatLng = PointLatLng.Empty;
        private static Point tempPoint = new Point(-1, -1);
        private static Thread background_thread;
        private static Dispatcher backgroundThread;
        static DataCalculations()
        {
            //var resetevent = new ManualResetEvent(false);
            //background_thread = new Thread(() =>
            //{
            //    backgroundThread = Dispatcher.CurrentDispatcher;
            //    var syncCtxt = new DispatcherSynchronizationContext(backgroundThread);
            //    SynchronizationContext.SetSynchronizationContext(syncCtxt);

            //    resetevent.Set();
            //    Dispatcher.Run();
            //});
            //background_thread.Start();
            //resetevent.WaitOne();
            //resetevent.Dispose();

            map = (Application.Current.MainWindow as MainWindow).Gmap;
            mainCanvas = (Application.Current.MainWindow as MainWindow).drawCanvas;
        }

        /// <summary>
        /// Gets the local position from geolocations.
        /// </summary>
        /// <param name="pos">position</param>
        /// <returns>local coordinate</returns>
        public static Point GetPhysicalFromLatLng(PointLatLng pos)
        {
            //Experimental
            tempPointLatLng = pos;
            GPoint gp = map.FromLatLngToLocal(pos);
            Point outp = new Point(gp.X, gp.Y);
            tempPoint = outp;
            return outp;

        }
        //Overload for calculation on a different canvas
        public static Point GetPhysicalFromLatLng(PointLatLng pos, Canvas canvas)
        {
            Point outP = GetPhysicalFromLatLng(pos);
            return mainCanvas.TranslatePoint(outP, canvas);
        }

        /// <summary>
        /// Gets geolocation from a local point.
        /// </summary>
        /// <param name="point">local point</param>
        /// <returns>geolocation</returns>
        public static PointLatLng GetLatLngFromPhysical(Point point)
        {
            //Experimental
            PointLatLng outp = map.FromLocalToLatLng((int)point.X, (int)point.Y);
            if (point == tempPoint) return tempPointLatLng;
            else return outp;
        }

        //Overload for calculation on a different canvas
        public static PointLatLng GetLatLngFromPhysical(Point point, Canvas canvas)
        {
            Point inP = canvas.TranslatePoint(point, mainCanvas);
            return GetLatLngFromPhysical(inP);
        }

        /// Returns the LatLng resulting from moving a distance from an origin
        /// in the specified heading (expressed in degrees clockwise from north).
        /// @param from     The LatLng from which to start.
        /// @param distance The distance to travel.
        /// @param heading  The heading in degrees clockwise from north.
        public static PointLatLng GetOffset(PointLatLng from, double distance, double heading)
        {
            distance *= 1000;
            distance /= earthRadius;
            heading = heading * Math.PI / 180;
            double fromLat = from.Lat * Math.PI / 180;
            double fromLng = from.Lng * Math.PI / 180;
            double cosDistance = Math.Cos(distance);
            double sinDistance = Math.Sin(distance);
            double sinFromLat = Math.Sin(fromLat);
            double cosFromLat = Math.Cos(fromLat);
            double sinLat = cosDistance * sinFromLat + sinDistance * cosFromLat * Math.Cos(heading);
            double dLng = Math.Atan2(sinDistance * cosFromLat * Math.Sin(heading), cosDistance - sinFromLat * sinLat);

            return new PointLatLng(Math.Asin(sinLat) * 180 / Math.PI, (fromLng + dLng) * 180 / Math.PI);
        }

        /// <summary>
        /// Gets the track between two geolocations.
        /// </summary>
        /// <param name="from">position 1 in Latitude, Longitude</param>
        /// <param name="to">position 2 in Latitude, Longitude</param>
        /// <returns>angle in degrees</returns>
        public static double GetTrack(PointLatLng from, PointLatLng to)
        {
            var res = Math.Atan2(Math.Cos(to.Lat * Math.PI / 180) * Math.Sin((to.Lng - from.Lng) * Math.PI / 180), Math.Cos(from.Lat * Math.PI / 180) * Math.Sin(to.Lat * Math.PI / 180) - Math.Sin(from.Lat * Math.PI / 180) * Math.Cos(to.Lat * Math.PI / 180) * Math.Cos((to.Lng - from.Lng) * Math.PI / 180)) * 180 / Math.PI;
            if (res < 0) res += 360;
            return res;
        }

        /// <summary>
        /// Gets the distance between two geolocations.
        /// </summary>
        /// <param name="from">position 1 in Latitude, Longitude</param>
        /// <param name="to">position 2 in Latitude, Longitude</param>
        /// <returns>distance in kilometers</returns>
        public static double GetDistance(PointLatLng from, PointLatLng to)
        {
            return new MapRoute(new List<PointLatLng>() { from, to }, "L").Distance;
        }

        /// <summary>
        /// Gets local distance from given two local points
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns>local distance</returns>
        public static double GetLocalDistance(Point point1, Point point2)
        {
            return Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2));
        }

        /// <summary>
        /// Gets the distance required to perform a climb or a descend
        /// </summary>
        /// <param name="initialAlt">the initial altitude</param>
        /// <param name="finalAlt">the final altitude</param>
        /// <param name="aircraft">the aircraft key</param>
        /// <returns>the distance in specified unit</returns>
        public static double GetClimbDescendDistance(double initialAlt, double finalAlt, string aircraft)
        {
            var dataset = DataModel.GetData(aircraft);
            if (dataset == null) return 0;
            if (finalAlt > initialAlt)
            {
                if (initialAlt == 0) return dataset.climbPerformance[finalAlt]["distance"];
                else return dataset.climbPerformance[finalAlt]["distance"] - dataset.climbPerformance[initialAlt]["distance"];
            }
            else if (initialAlt > finalAlt)
            {
                if (finalAlt == 0) return dataset.descendPerformance[initialAlt]["distance"];
                else return dataset.descendPerformance[initialAlt]["distance"] - dataset.descendPerformance[finalAlt]["distance"];
            }
            else return 0;
        }

        /// <summary>
        /// Gets the time required to perform a climb or a descend
        /// </summary>
        /// <param name="initialAlt">the initial altitude</param>
        /// <param name="finalAlt">the final altitude</param>
        /// <param name="aircraft">the aircraft key</param>
        /// <returns>the time in seconds</returns>
        public static double GetClimbDescendTime(double initialAlt, double finalAlt, string aircraft)
        {
            var dataset = DataModel.GetData(aircraft);
            if (dataset == null) return 0;
            if (finalAlt > initialAlt)
            {
                if (initialAlt == 0) return dataset.climbPerformance[finalAlt]["time"];
                else return dataset.climbPerformance[finalAlt]["time"] - dataset.climbPerformance[initialAlt]["time"];
            }
            else if (initialAlt > finalAlt)
            {
                if (finalAlt == 0) return dataset.descendPerformance[initialAlt]["time"];
                else return dataset.descendPerformance[initialAlt]["time"] - dataset.descendPerformance[finalAlt]["time"];
            }
            else return 0;
        }

        /// <summary>
        /// Gets the fuel required to perform a climb or a descend
        /// </summary>
        /// <param name="initialAlt">the initial altitude</param>
        /// <param name="finalAlt">the final altitude</param>
        /// <param name="aircraft">the aircraft key</param>
        /// <returns>the fuel in specified unit</returns>
        public static double GetClimbDescendFuel(double initialAlt, double finalAlt, string aircraft)
        {
            var dataset = DataModel.GetData(aircraft);
            if (dataset == null) return 0;
            if (finalAlt > initialAlt)
            {
                if (initialAlt == 0) return dataset.climbPerformance[finalAlt]["fuel"];
                else return dataset.climbPerformance[finalAlt]["fuel"] - dataset.climbPerformance[initialAlt]["fuel"];
            }
            else if (initialAlt > finalAlt)
            {
                if (finalAlt == 0) return dataset.descendPerformance[initialAlt]["fuel"];
                else return dataset.descendPerformance[initialAlt]["fuel"] - dataset.descendPerformance[finalAlt]["fuel"];
            }
            else return 0;
        }

        /// <summary>
        /// Gets the level travelling distance of a leg
        /// </summary>
        /// <param name="altitudes">list of altitudes in a leg</param>
        /// <param name="distance">the distance of a leg</param>
        /// <param name="aircraft">the aircraft name</param>
        /// <returns>the level travelling distance of the leg</returns>
        public static double GetLevelDistance(double[] altitudes, double distance, string aircraft)
        {
            double total = 0;
            for (int i = 1; i < altitudes.Length; i++)
            {
                total += GetClimbDescendDistance(altitudes[i - 1], altitudes[i], aircraft);
            }
            return distance - total;
        }

        /// <summary>
        /// Gets the level travelling time of an aircraft with a speed at a certein altitude
        /// Independent of the altitude of the route
        /// </summary>
        /// <param name="distance">the level distance of the route leg</param>
        /// <param name="speed">the speed the aircraft is maintaining</param>
        /// <param name="aircraft">the aircraft key</param>
        /// <returns>the travelling time in seconds</returns>
        public static double GetLevelTime(double distance, double speed, string aircraft)
        {
            if (speed == 0) return 0;
            var dataset = DataModel.GetData(aircraft);
            if (dataset == null) return 0;
            distance = DataConverters.LengthUnits(distance, dataset.defaultUnits["distance"], "KM");
            speed = DataConverters.SpeedUnits(speed, dataset.defaultUnits["speed"], "KPH");
            return (distance / speed) * 3600;
        }

        /// <summary>
        /// Gets the fuel consumed for level flight
        /// </summary>
        /// <param name="param">the distance or time based on the default unit of the lffc</param>
        /// <param name="altitude">the altitude of the leg</param>
        /// <param name="speed">the speed of the aircraft</param>
        /// <param name="aircraft">the aircraft key</param>
        /// <returns>the level travelling fuel of a leg</returns>
        public static double GetLevelFuel(double distance, double time, double altitude, double speed, string aircraft)
        {
            var dataset = DataModel.GetData(aircraft);
            if (dataset == null) return 0;
            if (!dataset.lffc.ContainsKey(altitude)) return 0;
            if (!dataset.lffc[altitude].ContainsKey(speed)) return 0;

            double lffc = dataset.lffc[altitude][speed];
            if (dataset.defaultUnits["lffc"] == "PER KM")
            {
                distance = DataConverters.LengthUnits(distance, dataset.defaultUnits["distance"], "KM");
                return distance * lffc;
            }
            else if (dataset.defaultUnits["lffc"] == "PER MIN")
            {
                time = time / 60;
                return time * lffc;
            }
            else return 0;
        }

        /// <summary>
        /// Gets the default units of given aircraft
        /// </summary>
        /// <param name="aircraft">The aircraft name</param>
        /// <param name="parameter">The parameter for what the unit should be returned</param>
        /// <returns>The default unit for the given aircraft and parameter</returns>
        public static string GetAircraftUnits(string aircraft, string parameter)
        {
            var dataset = DataModel.GetData(aircraft);
            if (dataset == null) return "";
            return dataset.defaultUnits[parameter];
        }

        /// <summary>
        /// Gets the aircraft starting fuel configurations
        /// </summary>
        /// <param name="aircraft"></param>
        /// <returns>a dictionary containing the starting fuel configurations</returns>
        public static Dictionary<string, double> GetAircraftFuelConfigurations(string aircraft)
        {
            var dataset = DataModel.GetData(aircraft);
            if (dataset == null) return new Dictionary<string, double>();
            return dataset.startingFuel;
        }

        /// <summary>
        /// Gets list of available altitudes for an aircraft
        /// </summary>
        /// <param name="aircraft"></param>
        /// <returns></returns>
        public static List<double> GetAircraftAltitudes(string aircraft)
        {
            var dataset = DataModel.GetData(aircraft);
            if (dataset == null) return new List<double>();
            return dataset.altitudes;
        }

        public static List<double> GetAircraftSpeeds(string aircraft)
        {
            var dataset = DataModel.GetData(aircraft);
            if (dataset == null) return new List<double>();
            return dataset.speeds;
        }

        public static double GetReductionFuel(string aircraft)
        {
            var dataset = DataModel.GetData(aircraft);
            if (dataset == null) return 0;
            double retval = 0;
            foreach (var value in dataset.reductionFuel.Values)
            {
                retval += value;
            }
            return retval;
        }
    }
}
