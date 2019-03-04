using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KimimaruBot
{
    public static class Extensions
    {
        /// <summary>
        /// Returns a random double greater than or equal to <paramref name="minVal"/> and less than <paramref name="maxVal"/>.
        /// </summary>
        /// <param name="random">The Random instance.</param>
        /// <param name="minVal">The minimum range, inclusive.</param>
        /// <param name="maxVal">The maximum range, exclusive.</param>
        /// <returns>A double greater than or equal to <paramref name="minVal"/> and less than <paramref name="maxVal"/>.</returns>
        public static double RandomDouble(this Random random, double minVal, double maxVal)
        {
            return (random.NextDouble() * (maxVal - minVal)) + minVal;
        }
    }
}
