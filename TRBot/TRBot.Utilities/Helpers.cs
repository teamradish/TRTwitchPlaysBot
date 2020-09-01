/* This file is part of TRBot.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * TRBot is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with TRBot.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRBot.Utilities
{
    /// <summary>
    /// Various utilities and helper methods.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Linearly interpolates between two values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="amount">The amount to interpolate, ranging from 0 to 1, inclusive.</param>
        public static double Lerp(in double value1, in double value2, in double amount) => ((1 - amount) * value1) + (value2 * amount);

        /// <summary>
        /// Remaps a given value originally between <paramref name="lowNum1"> and <paramref name="highNum1">
        /// to be between <paramref name="lowNum2"> and <paramref name="highNum2">.
        /// </summary>
        public static double RemapNum(in double value, double lowNum1, double highNum1, double lowNum2, double highNum2)
        {
            return lowNum2 + (value - lowNum1) * (highNum2 - lowNum2) / (highNum1 - lowNum1);
        }
    }
}
