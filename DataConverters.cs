using System;
using System.CodeDom;

namespace Mission_Assistant
{
    class DataConverters
    {
        public static double LengthUnits(double val, string from, string to)
        {
            switch (to)
            {
                case "KM":
                    switch (from)
                    {
                        case "NM":
                            return Math.Round(1.851999 * val, 3);
                        case "FT":
                            return Math.Round(0.0003048 * val, 3);
                        case "M":
                            return Math.Round(0.001 * val, 3);
                        default:
                            return Math.Round(val, 3);
                    }
                case "NM":
                    switch (from)
                    {
                        case "KM":
                            return Math.Round(0.539957 * val, 3);
                        case "FT":
                            return Math.Round(0.000164579 * val, 3);
                        case "M":
                            return Math.Round(0.000539957 * val, 3);
                        default:
                            return Math.Round(val, 3);
                    }
                case "FT":
                    switch (from)
                    {
                        case "KM":
                            return Math.Round(3280.84 * val, 3);
                        case "M":
                            return Math.Round(3.28084 * val, 3);
                        case "NM":
                            return Math.Round(6076.12 * val, 3);
                        default:
                            return Math.Round(val, 3);
                    }
                case "M":
                    switch (from)
                    {
                        case "NM":
                            return Math.Round(1852 * val, 3);
                        case "FT":
                            return Math.Round(0.3048 * val, 3);
                        case "KM":
                            return Math.Round(1000 * val, 3);
                        default:
                            return Math.Round(val, 3);
                    }
                default:
                    return Math.Round(val, 3);
            }
        }

        public static double SpeedUnits(double val, string from, string to)
        {
            switch (to)
            {
                case "KPH":
                    switch (from)
                    {
                        case "MACH":
                            return Math.Round(1234.8 * val, 3);
                        case "KTS":
                            return Math.Round(1.852 * val, 3);
                        default:
                            return Math.Round(val, 3);
                    }
                case "MACH":
                    switch (from)
                    {
                        case "KPH":
                            return Math.Round(0.000809848 * val, 3);
                        case "KTS":
                            return Math.Round(0.00149984 * val, 3);
                        default:
                            return Math.Round(val, 3);
                    }
                case "KTS":
                    switch (from)
                    {
                        case "MACH":
                            return Math.Round(666.739 * val, 3);
                        case "KPH":
                            return Math.Round(0.539957 * val, 3);
                        default:
                            return Math.Round(val, 3);
                    }
                default:
                    return Math.Round(val, 3);
            }
        }

        public static double MassUnits(double val, string from, string to)
        {
            switch (to)
            {
                case "KG":
                    switch (from)
                    {
                        case "LBS":
                            return Math.Round(0.453592 * val, 3);
                        case "LTR":
                            return Math.Round(1 * val, 3);
                        default:
                            return Math.Round(val, 3);
                    }
                case "LBS":
                    switch (from)
                    {
                        case "KG":
                            return Math.Round(2.20462 * val, 3);
                        case "LTR":
                            return Math.Round(2.20462 * val, 3);
                        default:
                            return Math.Round(val, 3);
                    }
                case "LTR":
                    switch (from)
                    {
                        case "KG":
                            return Math.Round(1 * val, 3);
                        case "LBS":
                            return Math.Round(0.453592 * val, 3);
                        default:
                            return Math.Round(val, 3);
                    }
                default:
                    return Math.Round(val, 3);
            }
        }

        public static double ConsumptionUnits(double val, string from, string to)
        {
            switch (to)
            {
                case "PERMIN":
                    switch (from)
                    {
                        case "PERKM":
                            return 0;
                        default:
                            return Math.Round(val, 3);
                    }
                case "PERKM":
                    switch (from)
                    {
                        case "PERMIN":
                            return 0;
                        default:
                            return Math.Round(val, 3);
                    }
                default:
                    return Math.Round(val, 3);
            }
        }
    }
}
