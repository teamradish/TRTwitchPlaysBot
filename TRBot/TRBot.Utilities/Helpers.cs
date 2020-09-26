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

        /// <summary>
        /// Splits a string into multiple parts with each part's length being less than or equal to a given max character count.
        /// <summary>
        /// <param name="text">The text to split.</param>
        /// <param name="maxCharCount">The maximum number of characters to have in each section.</param>
        /// <returns>A List containing the split strings.</returns>
        public static List<string> SplitStringWithinCharCount(string text, in int maxCharCount)
        {
            //No text, return empty
            if (string.IsNullOrEmpty(text) == true)
            {
                return new List<string>();
            }

            //All the text already fits in the character count, so simply return it
            if (text.Length <= maxCharCount)
            {
                return new List<string>(1) { text };
            }

            //Initialize list
            int listCapacity = (int)Math.Ceiling(text.Length / (double)maxCharCount);
            List<string> textList = new List<string>(listCapacity);

            int totalChars = text.Length;

            int startIndex = 0;
            StringBuilder strBuilder = new StringBuilder(maxCharCount);

            //Keep going until we reach the end of the string
            while (startIndex < totalChars)
            {
                int appendCount = maxCharCount;
                
                //If we end up going over the total number of characters, clamp it
                if ((startIndex + appendCount) >= totalChars)
                {
                    appendCount = totalChars - startIndex;
                }

                strBuilder.Append(text, startIndex, appendCount);
                
                startIndex += appendCount;

                //Add this section of the string to the list
                string splitStr = strBuilder.ToString();
                textList.Add(splitStr);

                strBuilder.Clear();
            }

            return textList;
        }

        /// <summary>
        /// Splits a string into multiple parts with each part's length being less than or equal to a given max character count.
        /// If it encounters the given character separator near the end of the max character count, it will cut the part there and continue.
        /// <summary>
        /// <param name="text">The text to split.</param>
        /// <param name="maxCharCount">The maximum number of characters to have in each section.</param>
        /// <param name="separatorChar">The character to determine the early cutoff.</param>
        /// <returns>A List containing the split strings.</returns>
        public static List<string> FitTextWithinCharCount(string text, in int maxCharCount, in char separatorChar)
        {
            //No text, return empty
            if (string.IsNullOrEmpty(text) == true)
            {
                return new List<string>();
            }

            //All the text already fits in the character count, so simply return it
            if (text.Length <= maxCharCount)
            {
                return new List<string>(1) { text };
            }

            //Initialize list
            int listCapacity = (int)Math.Ceiling(text.Length / (double)maxCharCount);
            List<string> textList = new List<string>(listCapacity);

            int totalChars = text.Length;

            int startIndex = 0;
            StringBuilder strBuilder = new StringBuilder(maxCharCount);

            //Keep going until we reach the end of the string
            while (startIndex < totalChars)
            {
                int appendCount = maxCharCount;
                int endIndex = startIndex + appendCount;

                //If we end up going over the total number of characters, clamp it and add the remaining characters
                if (endIndex >= totalChars)
                {
                    appendCount = totalChars - startIndex;
                    endIndex = totalChars - 1;
                }
                else
                {
                    //Find the last instance of the separator in our range
                    int separatorIndex = text.LastIndexOf(separatorChar, endIndex, appendCount);
                    if (separatorIndex >= 0)
                    {
                        //We found the separator, so adjust how many characters we add
                        endIndex = separatorIndex + 1;
                        appendCount = endIndex - startIndex;

                        //If the separator's additional length goes over the character count,
                        //we have no choice but to add fewer characters
                        if (appendCount > maxCharCount)
                        {
                            appendCount = maxCharCount;
                        }
                    }
                }

                strBuilder.Append(text, startIndex, appendCount);
                
                startIndex += appendCount;

                string splitStr = strBuilder.ToString();
                textList.Add(splitStr);

                strBuilder.Clear();
            }

            return textList;
        }

        /// <summary>
        /// Splits a string into multiple parts with each part's length being less than or equal to a given max character count.
        /// If it encounters the given string separator near the end of the max character count, it will cut the part there and continue.
        /// <summary>
        /// <param name="text">The text to split.</param>
        /// <param name="maxCharCount">The maximum number of characters to have in each section.</param>
        /// <param name="separatorStr">The string to determine the early cutoff</param>
        /// <returns>A List containing the split strings.</returns>
        public static List<string> FitTextWithinCharCount(string text, in int maxCharCount, string separatorStr)
        {
            //No text, return empty
            if (string.IsNullOrEmpty(text) == true)
            {
                return new List<string>();
            }

            //All the text already fits in the character count, so simply return it
            if (text.Length <= maxCharCount)
            {
                return new List<string>(1) { text };
            }

            //Initialize list
            int listCapacity = (int)Math.Ceiling(text.Length / (double)maxCharCount);
            List<string> textList = new List<string>(listCapacity);

            int totalChars = text.Length;

            int startIndex = 0;
            StringBuilder strBuilder = new StringBuilder(maxCharCount);

            //Keep going until we reach the end of the string
            while (startIndex < totalChars)
            {
                int appendCount = maxCharCount;
                int endIndex = startIndex + appendCount;

                //If we end up going over the total number of characters, clamp it and add the remaining characters
                if (endIndex >= totalChars)
                {
                    appendCount = totalChars - startIndex;
                    endIndex = totalChars - 1;
                }
                else
                {
                    //Find the last instance of the separator in our range
                    int separatorIndex = text.LastIndexOf(separatorStr, endIndex, appendCount);
                    if (separatorIndex >= 0)
                    {
                        //We found the separator, so adjust how many characters we add
                        endIndex = separatorIndex + separatorStr.Length;
                        appendCount = endIndex - startIndex;

                        //If the separator's additional length goes over the character count,
                        //we have no choice but to add fewer characters
                        if (appendCount > maxCharCount)
                        {
                            appendCount = maxCharCount;
                        }
                    }
                }

                strBuilder.Append(text, startIndex, appendCount);
                
                startIndex += appendCount;

                string splitStr = strBuilder.ToString();
                textList.Add(splitStr);

                strBuilder.Clear();
            }

            return textList;
        }
    }
}
