using System.Collections.Generic;

namespace MissionAssistant
{
    class DataTable
    {
        public string aircraftName;

        public List<double> altitudes;
        public List<double> speeds;

        public Dictionary<string, double> startingFuel;
        public Dictionary<string, double> reductionFuel;

        public Dictionary<double, Dictionary<string, double>> climbPerformance;
        public Dictionary<double, Dictionary<string, double>> descendPerformance;
        public Dictionary<double, Dictionary<double, double>> lffc;

        public Dictionary<string, string> defaultUnits;

        public DataTable()
        {
            altitudes = new List<double>();
            speeds = new List<double>();

            startingFuel = new Dictionary<string, double>();
            reductionFuel = new Dictionary<string, double>();
            climbPerformance = new Dictionary<double, Dictionary<string, double>>();
            descendPerformance = new Dictionary<double, Dictionary<string, double>>();
            lffc = new Dictionary<double, Dictionary<double, double>>();
            defaultUnits = new Dictionary<string, string>(5);
        }
    }
}
