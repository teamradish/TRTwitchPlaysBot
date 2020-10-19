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
using System.Text.RegularExpressions;
using System.Diagnostics;
using TRBot.Parsing;
using TRBot.ParserData;
using TRBot.Consoles;

namespace TRBot.Misc
{
    /// <summary>
    /// A reverse parser that attempts to generate a string given a set of inputs.
    /// </summary>
    public static class ReverseParser
    {
        /// <summary>
        /// Generates an input string given an <see cref="ParsedInputSequence"/>.
        /// </summary>
        /// <param name="inputSequence">The input sequence.</param>
        /// <param name="gameConsole">The game console to reference.</param>
        /// <returns>A string of the reverse parsed inputs.</returns>
        public static string ReverseParse(in ParsedInputSequence inputSequence, GameConsole gameConsole)
        {
            List<List<ParsedInput>> inputsDList = inputSequence.Inputs;

            //If the input isn't valid, then we can't reverse parse it because it couldn't be parsed!
            //Likewise for a lack of inputs
            if (inputSequence.InputValidationType != InputValidationTypes.Valid
                || inputsDList == null || inputsDList.Count == 0)
            {
                return string.Empty;
            }

            //Initialize string builder for building the string
            StringBuilder strBuilder = new StringBuilder(256);

            for (int i = 0; i < inputsDList.Count; i++)
            {
                List<ParsedInput> inputList = inputsDList[i];

                //Go through all inputs in the subsequence
                for (int j = 0; j < inputList.Count; j++)
                {
                    ParsedInput input = inputList[j];

                    //Add hold string
                    if (input.hold == true)
                    {
                        strBuilder.Append(Parser.DEFAULT_PARSE_REGEX_HOLD_INPUT);
                    }

                    //Add release string
                    if (input.release == true)
                    {
                        strBuilder.Append(Parser.DEFAULT_PARSE_REGEX_RELEASE_INPUT);
                    }

                    strBuilder.Append(input.name);

                    //Add percent if it's an axis or the percent isn't the default
                    if (input.percent != Parser.PARSER_DEFAULT_PERCENT
                        || gameConsole.IsAxis(input) == true)
                    {
                        strBuilder.Append(input.percent).Append(Parser.DEFAULT_PARSE_REGEX_PERCENT_INPUT);
                    }
                    
                    //Divide by 1000 to display seconds properly
                    int duration = input.duration;
                    if (input.duration_type == Parser.DEFAULT_PARSE_REGEX_SECONDS_INPUT)
                    {
                        duration /= 1000;
                    }

                    strBuilder.Append(duration);
                    strBuilder.Append(input.duration_type);

                    //Add plus string if there are more in the subsequence
                    if (j < (inputList.Count - 1))
                    {
                        strBuilder.Append(Parser.DEFAULT_PARSE_REGEX_PLUS_INPUT);
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
        /// Generates a natural sounding sentence given an <see cref="ParsedInputSequence"/>.
        /// </summary>
        /// <param name="inputSequence">The input sequence.</param>
        /// <param name="gameConsole">The game console to reference.</param>
        /// <returns>A string of the natural sentence.</returns>
        /// <remarks>Wording is cut back to balance readability with the bot's character limit.</remarks>
        public static string ReverseParseNatural(in ParsedInputSequence inputSequence, GameConsole gameConsole)
        {
            List<List<ParsedInput>> inputsDList = inputSequence.Inputs;

            //If the input isn't valid, say so
            if (inputSequence.InputValidationType != InputValidationTypes.Valid
                || inputsDList == null || inputsDList.Count == 0)
            {
                return "Invalid input!";
            }

            //Initialize string builder for building the string
            StringBuilder strBuilder = new StringBuilder(512);

            for (int i = 0; i < inputsDList.Count; i++)
            {
                List<ParsedInput> inputList = inputsDList[i];

                //Go through all inputs in the subsequence
                for (int j = 0; j < inputList.Count; j++)
                {
                    ParsedInput input = inputList[j];

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
                    else if (gameConsole.IsBlankInput(input) == false)
                    {
                        if (i == 0 && j ==0) strBuilder.Append("Press ");
                        else strBuilder.Append("press ");
                    }

                    //If waiting, say we should wait
                    if (gameConsole.IsBlankInput(input) ==true)
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
                            && (input.percent != Parser.PARSER_DEFAULT_PERCENT
                            || gameConsole.IsAxis(input) == true))
                        {
                            strBuilder.Append(input.percent).Append("% ");
                        }
                    }
                    
                    //Divide by 1000 to display seconds properly
                    int duration = input.duration;
                    string durTypeStr = "msec";
                    if (input.duration_type == Parser.DEFAULT_PARSE_REGEX_SECONDS_INPUT)
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
