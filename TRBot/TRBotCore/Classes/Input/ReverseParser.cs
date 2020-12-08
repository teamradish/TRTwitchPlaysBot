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
            StringBuilder strBuilder = new StringBuilder(256);

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
                        strBuilder.Append(Parser.ParseRegexHoldInput);
                    }

                    //Add release string
                    if (input.release == true)
                    {
                        strBuilder.Append(Parser.ParseRegexReleaseInput);
                    }

                    strBuilder.Append(input.name);

                    //Add percent if it's an axis or the percent isn't the default
                    if (input.percent != Parser.ParserDefaultPercent
                        || InputGlobals.CurrentConsole.IsAxis(input) == true)
                    {
                        strBuilder.Append(input.percent).Append(Parser.ParseRegexPercentInput);
                    }
                    
                    //Divide by 1000 to display seconds properly
                    int duration = input.duration;
                    if (input.duration_type == Parser.ParseRegexSecondsInput)
                    {
                        duration /= 1000;
                    }

                    strBuilder.Append(duration);
                    strBuilder.Append(input.duration_type);

                    //Add plus string if there are more in the subsequence
                    if (j < (inputList.Count - 1))
                    {
                        strBuilder.Append(Parser.ParseRegexPlusInput);
                    }
                }

                //Add a space for readability if there are more subsequences
                if (i < (inputsDList.Count - 1))
                {
                    strBuilder.Append(' ');
                }
            }

            string strBuilderVal = strBuilder.ToString();
            string finalVal = Regex.Unescape(strBuilderVal);

            return finalVal;
        }

        /// <summary>
        /// Generates a natural sounding sentence given an <see cref="Parser.InputSequence"/>.
        /// </summary>
        /// <param name="inputSequence">The input sequence.</param>
        /// <returns>A string of the natural sentence.</returns>
        /// <remarks>Wording is cut back to balance readability with Twitch's character limit.</remarks>
        public static string ReverseParseNatural(in Parser.InputSequence inputSequence)
        {
            List<List<Parser.Input>> inputsDList = inputSequence.Inputs;

            //If the input isn't valid, say so
            if (inputSequence.InputValidationType != Parser.InputValidationTypes.Valid
                || inputsDList == null || inputsDList.Count == 0)
            {
                return "Invalid input!";
            }

            //Initialize string builder for building the string
            StringBuilder strBuilder = new StringBuilder(512);

            for (int i = 0; i < inputsDList.Count; i++)
            {
                List<Parser.Input> inputList = inputsDList[i];

                //Go through all inputs in the subsequence
                for (int j = 0; j < inputList.Count; j++)
                {
                    Parser.Input input = inputList[j];

                    //Handle hold
                    if (input.hold == true)
                    {
                        if (i == 0 && j ==0) strBuilder.Append("Hold ");
                        else strBuilder.Append("hold ");
                    }
                    //Handle release
                    else if (input.release == true)
                    {
                        if (i == 0 && j ==0) strBuilder.Append("Release ");
                        else strBuilder.Append("release ");
                    }
                    else if (InputGlobals.CurrentConsole.IsWait(input) == false)
                    {
                        if (i == 0 && j ==0) strBuilder.Append("Press ");
                        else strBuilder.Append("press ");
                    }

                    //If waiting, say we should wait
                    if (InputGlobals.CurrentConsole.IsWait(input) ==true)
                    {
                        if (i == 0 && j ==0) strBuilder.Append("Wait ");
                        else strBuilder.Append("wait ");
                    }
                    else
                    {
                        //Add input name
                        strBuilder.Append('\"').Append(input.name).Append('\"').Append(' ');

                        //Add percent if it's an axis, the percent isn't the default, and not releasing
                        if (input.release == false
                            && (input.percent != Parser.ParserDefaultPercent
                            || InputGlobals.CurrentConsole.IsAxis(input) == true))
                        {
                            strBuilder.Append(input.percent).Append("% ");
                        }
                    }
                    
                    //Divide by 1000 to display seconds properly
                    int duration = input.duration;
                    string durTypeStr = "msec";
                    if (input.duration_type == Parser.ParseRegexSecondsInput)
                    {
                        duration /= 1000;
                        durTypeStr = "sec";
                    }

                    strBuilder.Append(duration).Append(' ').Append(durTypeStr);

                    //Handle 1 (Ex. "second" instead of "seconds")
                    if (duration != 1)
                    {
                        strBuilder.Append('s');
                    }

                    //Add plus string if there are more in the subsequence
                    if (j < (inputList.Count - 1))
                    {
                        strBuilder.Append(" AND ");
                    }
                }

                //Add a space for readability if there are more subsequences
                if (i < (inputsDList.Count - 1))
                {
                    strBuilder.Append(", THEN ");
                }
                else
                {
                    strBuilder.Append('.');
                }
            }

            string strBuilderVal = strBuilder.ToString();
            string finalVal = Regex.Unescape(strBuilderVal);

            return finalVal;
        }
    }
}
