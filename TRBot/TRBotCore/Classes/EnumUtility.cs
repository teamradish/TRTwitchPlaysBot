using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRBot
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
    }
}