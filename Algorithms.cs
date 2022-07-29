using System;

namespace MissionAssistant
{
    public static class Algorithms
    {
        /// <summary>
        /// Delegate for comparing values for the search algorithms.
        /// Return in (num - key) format to get the desired results.
        /// </summary>
        /// <param name="num"></param>
        /// <param name="key"></param>
        /// <returns>Returns the processed comparision results in (num - key) format.</returns>
        public delegate double CompareScript(double num, double key);

        /// <summary>
        /// The binary search algorithm for searching in a double array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="key"></param>
        /// <param name="work"></param>
        /// <returns>The index of the member searched for. -1 if member not found</returns>
        public static int BinarySearch(double[] array, double key, CompareScript script)
        {
            int min = 0, max = array.Length - 1, mid;
            double res;
            while (min <= max)
            {
                mid = (min + max) / 2;
                res = script.Invoke(mid, key);
                if (res == 0)
                {
                    return ++mid;
                }
                else if (res > 0)
                {
                    max = mid - 1;
                }
                else
                {
                    min = mid + 1;
                }
            }
            return -1;
        }
    }
}
