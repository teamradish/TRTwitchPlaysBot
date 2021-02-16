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
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;
using TRBot.Parsing;
using TRBot.Consoles;

namespace TRBot.Misc
{
    /// <summary>
    /// A reverse parser that attempts to generate a string given a set of inputs.
    /// </summary>
    public static class ReverseParser
    {
        /// <summary>
        /// The ways to show controller ports in the reverse parser.
        /// </summary>
        public enum ShowPortTypes
        {
            None = 0,
            ShowAllPorts = 1,
            ShowNonDefaultPorts = 2
        }

        /// <summary>
        /// The ways to show durations in the reverse parser.
        /// </summary>
        public enum ShowDurationTypes
        {
            ShowAllDurations = 0,
            ShowNonDefaultDurations = 1
        }

        /// <summary>
        /// Generates an input string given an <see cref="ParsedInputSequence"/>.
        /// </summary>
        /// <param name="inputSequence">The input sequence.</param>
        /// <param name="gameConsole">The game console to reference.</param>
        /// <param name="options">Options for the reverse parser.</param>
        /// <returns>A string of the reverse parsed inputs.</returns>
        public static string ReverseParse(in ParsedInputSequence inputSequence, GameConsole gameConsole,
            in ReverseParserOptions options)
        {
            List<List<ParsedInput>> inputsDList = inputSequence.Inputs;

            //If the input isn't valid, then we can't reverse parse it because it couldn't be parsed!
            //Likewise for a lack of inputs
            if (inputSequence.ParsedInputResult != ParsedInputResults.Valid
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

                    //Show the port number if it's not a blank input,
                    //we should show all ports, or we should show a non-default port
                    //and the input's controller port is not the default port
                    if (gameConsole.IsBlankInput(input) == false
                        && options.ShowPortType == ShowPortTypes.ShowAllPorts
                        || (options.ShowPortType == ShowPortTypes.ShowNonDefaultPorts 
                            && input.ControllerPort != options.DefaultPortNum))
                    {
                        strBuilder.Append(Parser.DEFAULT_PARSE_REGEX_PORT_INPUT).Append(input.ControllerPort + 1);
                    }

                    //Add hold string
                    if (input.Hold == true)
                    {
                        strBuilder.Append(Parser.DEFAULT_PARSE_REGEX_HOLD_INPUT);
                    }

                    //Add release string
                    if (input.Release == true)
                    {
                        strBuilder.Append(Parser.DEFAULT_PARSE_REGEX_RELEASE_INPUT);
                    }

                    strBuilder.Append(input.Name);

                    //Add percent if it's an axis or the percent isn't the default
                    if (input.Percent != Parser.PARSER_DEFAULT_PERCENT
                        || gameConsole.IsAxis(input) == true)
                    {
                        strBuilder.Append(input.Percent).Append(Parser.DEFAULT_PARSE_REGEX_PERCENT_INPUT);
                    }
                    
                    int duration = input.Duration;

                    //Skip displaying the duration if we should show only non-default durations
                    //and the duration isn't the default
                    //Always show durations for blank inputs
                    if (options.ShowDurationType == ShowDurationTypes.ShowAllDurations
                       || gameConsole.IsBlankInput(input) == true
                       || (options.ShowDurationType == ShowDurationTypes.ShowNonDefaultDurations
                        && options.DefaultDuration != duration))
                    {
                        //Divide by 1000 to display seconds properly
                        if (input.DurationType == InputDurationTypes.Seconds)
                        {
                            duration /= 1000;
                        }

                        strBuilder.Append(duration);
                        strBuilder.Append(input.DurationType);
                    }

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
        /// <param name="options">Options for the reverse parser.</param>
        /// <returns>A string of the natural sentence.</returns>
        /// <remarks>Wording is cut back to balance readability with the bot's character limit.</remarks>
        public static string ReverseParseNatural(in ParsedInputSequence inputSequence, GameConsole gameConsole,
            in ReverseParserOptions options)
        {
            List<List<ParsedInput>> inputsDList = inputSequence.Inputs;

            //If the input isn't valid, say so
            if (inputSequence.ParsedInputResult != ParsedInputResults.Valid
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

                    bool isBlankInput = gameConsole.IsBlankInput(input);

                    //Handle hold
                    if (input.Hold == true)
                    {
                        if (i == 0 && j ==0) strBuilder.Append("Hold ");
                        else strBuilder.Append("hold ");
                    }
                    //Handle release
                    else if (input.Release == true)
                    {
                        if (i == 0 && j ==0) strBuilder.Append("Release ");
                        else strBuilder.Append("release ");
                    }
                    else if (isBlankInput == false)
                    {
                        if (i == 0 && j ==0) strBuilder.Append("Press ");
                        else strBuilder.Append("press ");
                    }

                    //If waiting, say we should wait
                    if (isBlankInput == true)
                    {
                        if (i == 0 && j ==0) strBuilder.Append("Wait ");
                        else strBuilder.Append("wait ");
                    }
                    else
                    {
                        //Add input name
                        strBuilder.Append('\"').Append(input.Name).Append('\"').Append(' ');

                        //Add percent if it's an axis, the percent isn't the default, and not releasing
                        if (input.Release == false
                            && (input.Percent != Parser.PARSER_DEFAULT_PERCENT
                            || gameConsole.IsAxis(input) == true))
                        {
                            strBuilder.Append(input.Percent).Append("% ");
                        }
                    }
                    
                    //Divide by 1000 to display seconds properly
                    int duration = input.Duration;
                    string durTypeStr = "msec";
                    if (input.DurationType == InputDurationTypes.Seconds)
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

                    //Show the port number if it's not a blank input,
                    //we should show all ports, or we should show a non-default port
                    //and the input's controller port is not the default port
                    if (isBlankInput == false && options.ShowPortType == ShowPortTypes.ShowAllPorts
                        || (options.ShowPortType == ShowPortTypes.ShowNonDefaultPorts 
                            && input.ControllerPort != options.DefaultPortNum))
                    {
                        strBuilder.Append(" on port ").Append(input.ControllerPort + 1);
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

        /// <summary>
        /// Options for the reverse parser.
        /// </summary>
        public struct ReverseParserOptions
        {
            public ShowPortTypes ShowPortType;
            public int DefaultPortNum;
            
            public ShowDurationTypes ShowDurationType;
            public int DefaultDuration;

            public ReverseParserOptions(in ShowPortTypes showPortType, in int defaultPortNum)
            {
                ShowPortType = showPortType;
                DefaultPortNum = defaultPortNum;

                ShowDurationType = ShowDurationTypes.ShowAllDurations;
                DefaultDuration = 200;
            }

            public ReverseParserOptions(in ShowPortTypes showPortType, in int defaultPortNum,
                in ShowDurationTypes showDurType, in int defaultDuration)
                : this(showPortType, defaultPortNum)
            {
                ShowDurationType = showDurType;
                DefaultDuration = defaultDuration;
            }

            public override bool Equals(object obj)
            {
                if (obj is ReverseParserOptions inpSeq)
                {
                    return (this == inpSeq);
                }
                return false;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 59;
                    hash = (hash * 37) + ShowPortType.GetHashCode();
                    hash = (hash * 37) + DefaultPortNum.GetHashCode();
                    hash = (hash * 37) + ShowDurationType.GetHashCode();
                    hash = (hash * 37) + DefaultDuration.GetHashCode();
                    return hash;
                }
            }

            public static bool operator ==(ReverseParserOptions a, ReverseParserOptions b)
            {
                return (a.ShowPortType == b.ShowPortType && a.DefaultPortNum == b.DefaultPortNum
                    && a.ShowDurationType == b.ShowDurationType && a.DefaultDuration == b.DefaultDuration);
            }

            public static bool operator !=(ReverseParserOptions a, ReverseParserOptions b)
            {
                return !(a == b);
            }
        }
    }
}
