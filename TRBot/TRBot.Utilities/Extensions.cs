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
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRBot.Utilities
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
        public static double RandomDouble(this Random random, in double minVal, in double maxVal)
        {
            return (random.NextDouble() * (maxVal - minVal)) + minVal;
        }
        
        /// <summary>
        /// Copies unique keys and values from a <see cref="Dictionary{TKey, TValue}"/> into an existing <see cref="Dictionary{TKey, TValue}"/>.
        /// If the key already exists in the dictionary to copy to, it will replace it.
        /// </summary>
        /// <typeparam name="T">The type of the key.</typeparam>
        /// <typeparam name="U">The type of the value.</typeparam>
        /// <param name="dictCopiedTo">The Dictionary to copy values to.</param>
        /// <param name="dictCopiedFrom">The Dictionary to copy from.</param>
        public static void CopyDictionaryData<T, U>(this Dictionary<T, U> dictCopiedTo, Dictionary<T, U> dictCopiedFrom)
        {
            //Don't do anything if null, since there's nothing to copy from
            if (dictCopiedFrom == null) return;

            //Go through all keys and values
            foreach (KeyValuePair<T, U> kvPair in dictCopiedFrom)
            {
                dictCopiedTo[kvPair.Key] = kvPair.Value;
            }
        }

        /// <summary>
        /// Copies unique keys and values from a <see cref="ConcurrentDictionary{TKey, TValue}"/> into an existing <see cref="ConcurrentDictionary{TKey, TValue}"/>.
        /// If the key already exists in the dictionary to copy to, it will replace it.
        /// </summary>
        /// <typeparam name="T">The type of the key.</typeparam>
        /// <typeparam name="U">The type of the value.</typeparam>
        /// <param name="dictCopiedTo">The ConcurrentDictionary to copy values to.</param>
        /// <param name="dictCopiedFrom">The ConcurrentDictionary to copy from.</param>
        public static void CopyDictionaryData<T, U>(this ConcurrentDictionary<T, U> dictCopiedTo, ConcurrentDictionary<T, U> dictCopiedFrom)
        {
            //Don't do anything if null, since there's nothing to copy from
            if (dictCopiedFrom == null) return;

            //Go through all keys and values
            foreach (KeyValuePair<T, U> kvPair in dictCopiedFrom)
            {
                dictCopiedTo[kvPair.Key] = kvPair.Value;
            }
        }
    }
}
