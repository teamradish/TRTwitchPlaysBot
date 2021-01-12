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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TRBot.Parsing
{
    /// <summary>
    /// A standard parser.
    /// </summary>
    public class StandardParser : IParser
    {
        private List<IParserComponent> ParserComponents = null;

        public StandardParser(List<IParserComponent> parserComponents)
        {
            ParserComponents = new List<IParserComponent>(parserComponents);
        }

        /// <summary>
        /// Parses a message to return a parsed input sequence.
        /// </summary>
        /// <param name="message">The message to parse.</param>
        /// <returns>A <see cref="ParsedInputSequence" /> containing all the inputs parsed.</returns>
        public ParsedInputSequence ParseInputs(string message)
        {
            string regex = string.Empty;

            //Combine the regex for all parser components
            for (int i = 0; i < ParserComponents.Count; i++)
            {
                regex += ParserComponents[i].Regex;
            }

            Console.WriteLine("REGEX: " + regex);

            MatchCollection matches = Regex.Matches(message, regex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (matches.Count == 0)
            {
                return new ParsedInputSequence(ParsedInputResults.NormalMsg, null, 0, "Parser: Message is a normal one.");
            }

            Console.WriteLine($"Match count for \"{message}\" is {matches.Count}");

            ParsedInputSequence inputSequence = new ParsedInputSequence();
            int totalDur = 0;
            List<List<ParsedInput>> parsedInputList = new List<List<ParsedInput>>();

            List<ParsedInput> subInputs = new List<ParsedInput>();

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                Console.WriteLine($"Match value: {match.Value}");

                ParsedInput input = new ParsedInput();

                Console.WriteLine($"Group count: {match.Groups.Count}");

                if (match.Groups.TryGetValue("input", out Group inpGroup) == false)
                {
                    return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, "Parser error: Input is missing.");
                }

                input.name = inpGroup.Value;

                bool hasHold = match.Groups.TryGetValue("hold", out Group holdGroup);
                bool hasRelease = match.Groups.TryGetValue("release", out Group releaseGroup);

                if (hasHold == true && hasRelease == true)
                {
                    return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, "Parser error: Contains both a hold and a release.");
                }

                input.hold = hasHold;
                input.release = hasRelease;

                if (match.Groups.TryGetValue("port", out Group portGroup) == true)
                {
                    string portNumStr = portGroup.Value.Substring(1);
                    if (int.TryParse(portNumStr, out input.controllerPort) == false)
                    {
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, "Parser error: Port number is invalid.");
                    }
                }

                if (match.Groups.TryGetValue("percent", out Group percentGroup) == true)
                {
                    string percentParseStr = percentGroup.Value;

                    int percentIndex = percentParseStr.IndexOf("%");
                    if (percentIndex < 0)
                    {
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, "Parser error: '%' symbol not found in percentage.");
                    }

                    if (percentIndex != (percentParseStr.Length - 1))
                    {
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, "Parser error: '%' symbol not at end of percentage.");
                    }
                    
                    string percentStr = percentParseStr.Substring(0, percentParseStr.Length - percentIndex);

                    if (int.TryParse(percentStr, out int percent) == false)
                    {
                     
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, "Parser error: Percentage is invalid.");
                    }

                    input.percent = percent;
                }

                bool hasMs = match.Groups.TryGetValue("ms", out Group msGroup);
                bool hasSec = match.Groups.TryGetValue("s", out Group secGroup);

                if (hasMs == true && hasSec == true)
                {
                    return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, "Parser error: Contains both 'ms' and 's' for duration.");
                }
                else if (hasMs == true)
                {
                    string msParseStr = msGroup.Value;
                    int msIndex = msParseStr.IndexOf("ms");
                    if (msIndex < 0)
                    {
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, "Parser error: 'ms' not found.");
                    }

                    if (msIndex != (msParseStr.Length - 2))
                    {
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, "Parser error: 'ms' is not at end of duration.");
                    }

                    string msStr = msParseStr.Substring(0, msParseStr.Length - msIndex);
                    if (int.TryParse(msStr, out int msVal) == false)
                    {
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, "Parser error: Invalid millisecond duration.");
                    }

                    input.duration_type = "ms";
                    input.duration = msVal;
                }
                else if (hasSec == true)
                {
                    string secParseStr = msGroup.Value;
                    int secIndex = secParseStr.IndexOf('s');
                    if (secIndex < 0)
                    {
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, "Parser error: 's' not found.");
                    }

                    if (secIndex != (secParseStr.Length - 1))
                    {
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, "Parser error: 's' is not at end of duration.");
                    }

                    string secStr = secParseStr.Substring(0, secParseStr.Length - secIndex);
                    if (int.TryParse(secStr, out int secVal) == false)
                    {
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, "Parser error: Invalid second duration.");
                    }

                    input.duration_type = "s";
                    input.duration = secVal * 1000;
                }
                else
                {
                    input.duration_type = "ms";
                    input.duration = 200;
                }

                subInputs.Add(input);

                if (match.Groups.TryGetValue("simultaneous", out Group simultaneousGroup) == false)
                {
                    parsedInputList.Add(subInputs);
                    subInputs = new List<ParsedInput>();
                }

                totalDur += input.duration;
            }

            if (subInputs.Count > 0)
            {
                return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, "Parser error: Simultaneous specified with no input at end.");
            }

            inputSequence.ParsedInputResult = ParsedInputResults.Valid;
            inputSequence.Inputs = parsedInputList;
            inputSequence.TotalDuration = totalDur;
            inputSequence.Error = string.Empty;

            return inputSequence;
        }
    }
}