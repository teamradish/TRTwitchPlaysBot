/* Copyright (C) 2019-2020 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot,software for playing games through text.
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

            //Replace all whitespace with regex
            return Regex.Replace(originalStr, @"\s+", string.Empty, RegexOptions.Compiled);
        }
    }
}