using System;
using System.CodeDom;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace MissionAssistant
{
    static class DataConverters
    {
        public static double LengthUnits(double val, string from, string to)
        {
            switch (to)
            {
                case "KM":
                    switch (from)
                    {
                        case "NM":
                            return 1.851999 * val;
                        case "FT":
                            return 0.0003048 * val;
                        case "M":
                            return 0.001 * val;
                        default:
                            return val;
                    }
                case "NM":
                    switch (from)
                    {
                        case "KM":
                            return 0.539957 * val;
                        case "FT":
                            return 0.000164579 * val;
                        case "M":
                            return 0.000539957 * val;
                        default:
                            return val;
                    }
                case "FT":
                    switch (from)
                    {
                        case "KM":
                            return 3280.84 * val;
                        case "M":
                            return 3.28084 * val;
                        case "NM":
                            return 6076.12 * val;
                        default:
                            return val;
                    }
                case "M":
                    switch (from)
                    {
                        case "NM":
                            return 1852 * val;
                        case "FT":
                            return 0.3048 * val;
                        case "KM":
                            return 1000 * val;
                        default:
                            return val;
                    }
                default:
                    return val;
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
                            return 1234.8 * val;
                        case "KTS":
                            return 1.852 * val;
                        default:
                            return val;
                    }
                case "MACH":
                    switch (from)
                    {
                        case "KPH":
                            return 0.000809848 * val;
                        case "KTS":
                            return 0.00149984 * val;
                        default:
                            return val;
                    }
                case "KTS":
                    switch (from)
                    {
                        case "MACH":
                            return 666.739 * val;
                        case "KPH":
                            return 0.539957 * val;
                        default:
                            return val;
                    }
                default:
                    return val;
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
                            return 0.453592 * val;
                        case "LTR":
                            return 1 * val;
                        default:
                            return val;
                    }
                case "LBS":
                    switch (from)
                    {
                        case "KG":
                            return 2.20462 * val;
                        case "LTR":
                            return 2.20462 * val;
                        default:
                            return val;
                    }
                case "LTR":
                    switch (from)
                    {
                        case "KG":
                            return 1 * val;
                        case "LBS":
                            return 0.453592 * val;
                        default:
                            return val;
                    }
                default:
                    return val;
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
                            return val;
                    }
                case "PERKM":
                    switch (from)
                    {
                        case "PERMIN":
                            return 0;
                        default:
                            return val;
                    }
                default:
                    return val;
            }
        }

        public static double[] CoordinateUnits(double val, string from, string to)
        {
            switch (to)
            {
                case "DMS":
                    switch (from)
                    {
                        case "DEGREE":
                            double[] dms = new double[3];
                            for(int i = 0; i < dms.Length; i++)
                            {
                                dms[i] = (int)val;
                                val = (val - (int)val) * 60;
                            }
                            return dms;
                        default:
                            double[] asIs = new double[1];
                            asIs[0] = val;
                            return asIs;
                    }
                default:
                    double[] asis = new double[1];
                    asis[0] = val;
                    return asis;
            }
        }
    }
}
