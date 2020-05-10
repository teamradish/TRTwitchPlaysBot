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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace TRBot
{
    /// <summary>
    /// A reverse parser that attempts to generate a string given a set of inputs.
    /// </summary>
    public static class ReverseParser
    {
        /// <summary>
        /// Generates an input string given an <see cref="Parser.InputSequence"/>.
        /// </summary>
        /// <param name="inputSequence">The input sequence.</param>
        /// <returns>A string of the reverse parsed inputs.</returns>
        public static string ReverseParse(in Parser.InputSequence inputSequence)
        {
            List<List<Parser.Input>> inputsDList = inputSequence.Inputs;

            //If the input isn't valid, then we can't reverse parse it because it couldn't be parsed!
            //Likewise for a lack of inputs
            if (inputSequence.InputValidationType != Parser.InputValidationTypes.Valid
                || inputsDList == null || inputsDList.Count == 0)
            {
                return string.Empty;
            }

            //Initialize string builder for building the string
            StringBuilder stringBuilder = new StringBuilder(256);

            for (int i = 0; i < inputsDList.Count; i++)
            {
                List<Parser.Input> inputList = inputsDList[i];

                //Go through all inputs in the subsequence
                for (int j = 0; j < inputList.Count; j++)
                {
                    Parser.Input input = inputList[j];

                    //Add hold string
                    if (input.hold == true)
                    {
                        stringBuilder.Append(Parser.ParseRegexHoldInput);
                    }

                    //Add release string
                    if (input.release == true)
                    {
                        stringBuilder.Append(Parser.ParseRegexReleaseInput);
                    }

                    stringBuilder.Append(input.name);

                    //Add percent only if it's an axis
                    if (InputGlobals.CurrentConsole.IsAxis(input) == true || InputGlobals.CurrentConsole.IsAbsoluteAxis(input) == true)
                    {
                        stringBuilder.Append(input.percent).Append(Parser.ParseRegexPercentInput);
                    }
                    
                    //Divide by 1000 to display seconds properly
                    int duration = input.duration;
                    if (input.duration_type == Parser.ParseRegexSecondsInput)
                    {
                        duration /= 1000;
                    }

                    stringBuilder.Append(duration);
                    stringBuilder.Append(input.duration_type);

                    //Add plus string if there are more in the subsequence
                    if (j < (inputList.Count - 1))
                    {
                        stringBuilder.Append(Parser.ParseRegexPlusInput);
                    }
                }

                //Add a space for readability if there are more subsequences
                if (i < (inputsDList.Count - 1))
                {
                    stringBuilder.Append(' ');
                }
            }

            string strBuilderVal = stringBuilder.ToString();
            string finalVal = Regex.Unescape(strBuilderVal);

            return finalVal;
        }
    }
}
