/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, version 3 of the License.
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
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TRBot.Parsing
{
    /// <summary>
    /// Various parser utilities and helper methods.
    /// </summary>
    public static class ParserUtilities
    {
        /// <summary>
        /// Removes all whitespace from a string, including tabs and spaces.
        /// </summary>
        /// <param name="originalStr">The string to remove whitespace from.</param>
        /// <returns>A string with all whitespace removed. If <paramref name="originalStr"> is null or empty, it is returned instead.</returns>
        public static string RemoveAllWhitespace(string originalStr)
        {
            //Return the original string if null or empty
            if (string.IsNullOrEmpty(originalStr) == true)
            {
                return originalStr;
            }

            //Replace all whitespace via regex
            return Regex.Replace(originalStr, @"\s+", string.Empty, RegexOptions.Compiled);
        }

        /// <summary>
        /// Tells if a double is approximately equal to another one.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="comparison">The value to compare to.</param>
        /// <param name="error">The threshold of error to account for.</param>
        /// <returns>true if (<paramref name="value"/> - <paramref name="comparison"/>) is within <paramref name="error"/>.</returns>
        public static bool IsApproximate(in double value, in double comparison, in double error)
        {
            double check = Math.Abs(value - comparison);
            double absError = Math.Abs(error);

            return (check <= absError);
        }
    }
}