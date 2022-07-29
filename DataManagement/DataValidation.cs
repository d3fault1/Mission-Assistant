using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionAssistant
{
    static class DataValidation
    {
        /// <summary>
        /// Checks if a double number is valid
        /// </summary>
        /// <param name="inp">The Number</param>
        /// <returns>true if number is valid. else false</returns>
        public static bool isNumberValid(double inp)
        {
            return !Double.IsNaN(inp) && !Double.IsInfinity(inp);
        }
    }
}
