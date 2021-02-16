/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRBot.Utilities
{
    /// <summary>
    /// Enum utility class.
    /// </summary>
    public static class EnumUtility
    {
        /// <summary>
        /// Gets the values for an Enum of a particular type in an array and caches it.
        /// </summary>
        /// <typeparam name="T">The Enum type.</typeparam>
        public static class GetValues<T> where T : Enum
        {
            /// <summary>
            /// The cached enum array containing all the values for the Enum type.
            /// </summary>
            public static T[] EnumValues { get; private set; } = null;

            static GetValues()
            {
                EnumValues = (T[])Enum.GetValues(typeof(T));
            }
        }

        /// <summary>
        /// Gets the names for an Enum of a particular type in an array and caches it.
        /// </summary>
        /// <typeparam name="T">The Enum type.</typeparam>
        public static class GetNames<T> where T : Enum
        {
            /// <summary>
            /// The cached string array containing all the names in the Enum type.
            /// </summary>
            public static string[] EnumNames { get; private set; } = null;

            static GetNames()
            {
                EnumNames = Enum.GetNames(typeof(T));
            }
        }

        /* Adding flags: flag1 |= flag2            ; 10 | 01 = 11
         * Checking flags: (flag1 & flag2) != 0    ; 11 & 10 = 10
         * Removing flags: (flag1 & (~flag2))      ; 1111 & (~0010) = 1111 & 1101 = 1101
         * */

        /// <summary>
        /// Tells if one set of values contains another set of values. This is for values that represent a collection of flags.
        /// <para>This takes in longs to avoid boxing.</para>
        /// </summary>
        /// <param name="flags">The set of values as a long.</param>
        /// <param name="checkFlags">The set of values to test against, as a long.</param>
        /// <returns>true if <paramref name="flags"/> contains <paramref name="checkFlags"/>, otherwise false.</returns>
        public static bool HasEnumVal(in long flags, in long checkFlags)
        {
            long check = (flags & checkFlags);

            return (check != 0);
        }

        /// <summary>
        /// Adds values onto a set of other values. This is for values that represent a collection of flags.
        /// <para>This takes in longs to avoid boxing.</para>
        /// </summary>
        /// <param name="flags">The set of values as a long.</param>
        /// <param name="newFlags">The new values to add, as a long.</param>
        /// <returns>A long with the new values added.</returns>
        public static long AddEnumVal(in long flags, in long checkFlags)
        {
            long add = (flags | checkFlags);

            return add;
        }

        /// <summary>
        /// Removes values from a set of other values. This is for values that represent a collection of flags.
        /// <para>This takes in longs to avoid boxing.</para>
        /// </summary>
        /// <param name="flags">The set of values as a long.</param>
        /// <param name="flagsToRemove">The values to remove, as a long.</param>
        /// <returns>A long with the values removed.</returns>
        public static long RemoveEnumVal(in long flags, in long flagsToRemove)
        {
            long remove = (flags & (~flagsToRemove));

            return remove;
        }

        /// <summary>
        /// Converts the string representation of an Enum to its value equivalent.
        /// This fails if a valid Enum value isn't found.
        /// </summary>
        /// <param name="enumStr">The string to convert.</param>
        /// <param name="enumValue">The Enum value returned. Defaults to the first element if not found.</param>
        /// <returns>A bool indicating if the conversion was successful.</returns>
        public static bool TryParseEnumValue<T>(string enumStr, out T enumValue) where T : struct, Enum
        {
            //Try to parse into a number
            if (long.TryParse(enumStr, out long num) == false)
            {
                //Failed to parse into a number, so check if the string passed in is a valid name
                if (Enum.TryParse<T>(enumStr, true, out T result) == false)
                {
                    enumValue = result;
                    return false;
                }
                
                enumValue = result;
                return true;
            }

            //Check if the given number matches any of these values
            T[] enumValArray = EnumUtility.GetValues<T>.EnumValues;

            for (int i = 0; i < enumValArray.Length; i++)
            {
                //We can't cast generic enums directly to an integer, and enums can be of different integer types
                //Use Convert, which uses the IConvertible interface to convert the incompatible types.
                //Note that this may cause some boxing and thus garbage
                long enumNumVal = Convert.ToInt64(enumValArray[i]);

                //Check if the values match
                if (num == enumNumVal)
                {
                    enumValue = enumValArray[i];
                    return true;
                }
            }

            enumValue = enumValArray[0];
            return false;
        }
    }
}
