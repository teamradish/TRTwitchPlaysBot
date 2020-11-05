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
        /// Returns a random float greater than or equal to <paramref name="minVal"/> and less than <paramref name="maxVal"/>.
        /// </summary>
        /// <param name="random">The Random instance.</param>
        /// <param name="minVal">The minimum range, inclusive.</param>
        /// <param name="maxVal">The maximum range, exclusive.</param>
        /// <returns>A float greater than or equal to <paramref name="minVal"/> and less than <paramref name="maxVal"/>.</returns>
        public static float RandomFloat(this Random random, in float minVal, in float maxVal)
        {
            return (float)((random.NextDouble() * (maxVal - minVal)) + minVal);
        }

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

        /// <summary>
        /// Makes a string singular or plural and lowercase or capitalized based on the context.
        /// </summary>
        /// <param name="str">The string to pluralize.</param>
        /// <param name="capital">Whether the string should be capitalized. false means lowercase.</param>
        /// <param name="count">A given number, used to indicate if the string should be singular or plural.</param>
        /// <returns>A pluralized string.</returns>
        public static string Pluralize(this string str, in bool capital, in long count)
        {
            //We can't do anything for an empty or null string
            if (string.IsNullOrEmpty(str) == true)
            {
                return str;
            }

            string newStr = str;

            //Get the character values so we can see if a character is uppercase or lowercase
            int firstCapital = (int)'A';
            int lastCapital = (int)'Z';
            int firstLower = (int)'a';
            int lastLower = (int)'z';

            int diff = firstLower - firstCapital;
            char firstChar = str[0];
            char lastChar = str[str.Length - 1];

            int firstCharVal = (int)firstChar;

            //Set capital
            if (capital == true)
            {
                //If it's 'a' through 'z', replace it with the capital version
                if (firstChar >= firstLower && firstChar <= lastLower)
                {
                    char capitalChar = (char)(firstCharVal - diff);
                    newStr = newStr.Remove(0, 1);
                    newStr = capitalChar.ToString() + newStr;
                }
            }
            else
            {
                //If it's 'A' through 'Z', replace it with the lowercase version
                if (firstChar >= firstCapital && firstChar <= lastCapital)
                {
                    char lowercaseChar = (char)(firstCharVal + diff);
                    newStr = newStr.Remove(0, 1);
                    newStr = lowercaseChar.ToString() + newStr;
                }
            }

            if (count == 1)
            {
                //If the string ends with a plural, remove it
                //This doesn't cover all possible plural forms
                if ((lastChar == 's' || lastChar == 'S') && newStr.Length > 1)
                {
                    newStr = newStr.Remove(str.Length - 1, 1);
                }
            }
            else
            {
                //If the string is singular, make it plural
                //This doesn't cover all possible plural forms
                if (lastChar != 's')
                {
                    newStr = newStr + "s";
                }
            }

            return newStr;
        }
    }
}
