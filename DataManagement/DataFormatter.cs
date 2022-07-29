using System;

namespace MissionAssistant
{
    public enum TimeFormat
    {
        Standard,
        Short,
        Extended
    }
    public enum DataFormat
    {
        Long,
        Short,
        Round
    }
    public enum GeoFormat
    {
        DecimalDegrees,
        DMS
    }
    static class DataFormatter
    {
        public static string FormatTime(double value, TimeFormat format)
        {
            string output = "";
            TimeSpan t = TimeSpan.FromSeconds(value);
            switch ((int)format)
            {
                case 0:
                    output = String.Format("{0:D2}:{1:D2}:{2:D2}", (int)t.TotalHours, t.Minutes, t.Seconds);
                    break;
                case 1:
                    output = String.Format("{0}'{1}\"", (int)t.TotalMinutes, t.Seconds);
                    break;
                case 2:
                    output = String.Format("{0:D2}:{1:D2}:{2:D2}:{3:D3}", (int)t.TotalHours, t.Minutes, t.Seconds, t.Milliseconds);
                    break;
                default:
                    output = value.ToString();
                    break;
            }
            return output;
        }

        public static string FormatData(double value, DataFormat format)
        {
            string output = "";
            switch ((int)format)
            {
                case 0:
                    output = Math.Round(value, 3, MidpointRounding.AwayFromZero).ToString();
                    break;
                case 1:
                    output = Math.Round(value, 2, MidpointRounding.AwayFromZero).ToString();
                    break;
                case 2:
                    output = Math.Round(value, 0, MidpointRounding.AwayFromZero).ToString();
                    break;
                default:
                    output = value.ToString();
                    break;
            }
            return output;
        }

        public static string FormatGeo(double value, GeoFormat format)
        {
            string output = "";
            switch ((int)format)
            {
                case 0:
                    output = Math.Round(value, 3, MidpointRounding.AwayFromZero).ToString();
                    break;
                case 1:
                    var res = DataConverters.CoordinateUnits(value, "DEGREE", "DMS");
                    output = String.Format($"{res[0]}°{res[1]}'{res[2]}\"");
                    break;
                default:
                    output = value.ToString();
                    break;
            }
            return output;
        }
    }
}
