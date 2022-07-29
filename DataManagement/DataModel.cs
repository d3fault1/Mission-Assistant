using System.Collections.Generic;

namespace MissionAssistant
{
    static class DataModel
    {
        private static List<DataTable> dataArray;

        static DataModel()
        {
            dataArray = new List<DataTable>();
        }

        public static DataTable GetData(string aircraft)
        {
            foreach (DataTable d in dataArray)
            {
                if (d.aircraftName == aircraft) return d;
            }
            DataTable newTable = FetchData(aircraft);
            if (newTable != null) dataArray.Add(newTable);
            return newTable;
        }

        public static bool InvalidateData(string aircraft)
        {
            foreach (DataTable d in dataArray)
            {
                if (d.aircraftName == aircraft)
                {
                    dataArray.Remove(d);
                    return true;
                }
            }
            return false;
        }

        private static DataTable FetchData(string aircraft)
        {
            Dictionary<string, double> performanceDatagroup;
            Dictionary<double, double> speedValuegroup;
            dynamic pdata, fsdata, frdata, sudata;
            DatabaseIO.Fetch(aircraft, out pdata, out fsdata, out frdata, out sudata);
            if ((pdata as List<dynamic>).Count == 0) return null;
            DataTable newTable = new DataTable();
            newTable.aircraftName = aircraft;

            for (int i = 0; i < 5; i++)
            {
                if (sudata[i].Value != 0 && !newTable.speeds.Contains(sudata[i].Value))
                {
                    newTable.speeds.Add(sudata[i].Value);
                }
            }

            newTable.defaultUnits.Add("alt", sudata[0].Unit);
            newTable.defaultUnits.Add("distance", sudata[1].Unit);
            newTable.defaultUnits.Add("speed", sudata[2].Unit);
            newTable.defaultUnits.Add("fuel", sudata[3].Unit);
            newTable.defaultUnits.Add("lffc", sudata[4].Unit);

            foreach (dynamic d in pdata)
            {
                performanceDatagroup = new Dictionary<string, double>();
                speedValuegroup = new Dictionary<double, double>();

                bool altCheck = d.ALT != 0 && !newTable.altitudes.Contains(d.ALT);

                if (altCheck)
                {
                    newTable.altitudes.Add(d.ALT);

                    performanceDatagroup.Add("time", d.ClimbTime);
                    performanceDatagroup.Add("distance", d.ClimbDistance);
                    performanceDatagroup.Add("fuel", d.ClimbFuel);

                    newTable.climbPerformance.Add(d.ALT, performanceDatagroup);

                    performanceDatagroup = new Dictionary<string, double>();

                    performanceDatagroup.Add("time", d.DescendTime);
                    performanceDatagroup.Add("distance", d.DescendDistance);
                    performanceDatagroup.Add("fuel", d.DescendFuel);

                    newTable.descendPerformance.Add(d.ALT, performanceDatagroup);
                }

                if (sudata[0].Value != 0 && !speedValuegroup.ContainsKey(sudata[0].Value)) speedValuegroup.Add(sudata[0].Value, d.Speed01);
                if (sudata[1].Value != 0 && !speedValuegroup.ContainsKey(sudata[1].Value)) speedValuegroup.Add(sudata[1].Value, d.Speed02);
                if (sudata[2].Value != 0 && !speedValuegroup.ContainsKey(sudata[2].Value)) speedValuegroup.Add(sudata[2].Value, d.Speed03);
                if (sudata[3].Value != 0 && !speedValuegroup.ContainsKey(sudata[3].Value)) speedValuegroup.Add(sudata[3].Value, d.Speed04);
                if (sudata[4].Value != 0 && !speedValuegroup.ContainsKey(sudata[4].Value)) speedValuegroup.Add(sudata[4].Value, d.Speed05);

                if (altCheck) newTable.lffc.Add(d.ALT, speedValuegroup);
            }

            foreach (dynamic d in fsdata)
            {
                if (!newTable.startingFuel.ContainsKey(d.Label)) newTable.startingFuel.Add(d.Label, d.Value);
            }

            foreach (dynamic d in frdata)
            {
                if (!newTable.reductionFuel.ContainsKey(d.Label)) newTable.reductionFuel.Add(d.Label, d.Value);
            }

            return newTable;
        }
    }
}
