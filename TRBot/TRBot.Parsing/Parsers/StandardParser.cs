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
        /// <summary>
        /// The default port number to use for inputs with unspecified ports.
        /// </summary>
        public int DefaultPortNum = 0;
        
        /// <summary>
        /// The maximum port number available for inputs.
        /// </summary>
        public int MaxPortNum = 1;

        /// <summary>
        /// The default input duration for inputs with unspecified durations.
        /// </summary>
        public int DefaultInputDuration = 200;

        /// <summary>
        /// The maximum duration for the input sequence.
        /// </summary>
        public int MaxInputDuration = 60000;

        /// <summary>
        /// Whether to check if the input sequence will exceed the maximum input duration.
        /// </summary>
        public bool CheckMaxDur = true;

        /// <summary>
        /// The combined parser regex generated from the parser components.
        /// </summary>
        public string ParserRegex { get; private set; } = string.Empty; 

        /// <summary>
        /// The default percentage for inputs with unspecified percentages.
        /// </summary>
        private int DefaultPercentage = 100;

        /// <summary>
        /// The parser components used to build the parser.
        /// </summary>
        private List<IParserComponent> ParserComponents = null;

        /// <summary>
        /// The pre-parsers to run right before parsing.
        /// </summary>
        private List<IPreparser> Preparsers = null;

        public StandardParser(List<IParserComponent> parserComponents)
        {
            ParserComponents = parserComponents;

            string regex = string.Empty;

            //Generate the input regex for all the parser components
            for (int i = 0; i < ParserComponents.Count; i++)
            {
                regex += ParserComponents[i].ComponentRegex;
            }

            ParserRegex = regex;
        }

        public StandardParser(List<IParserComponent> parserComponents, in int defaultPortNum,
            in int maxPortNum, in int defaultInputDur, in int maxInputDur, in bool checkMaxDur)
                : this(parserComponents)
        {
            DefaultPortNum = defaultPortNum;
            MaxPortNum = maxPortNum;
            DefaultInputDuration = defaultInputDur;
            MaxInputDuration = maxInputDur;
            CheckMaxDur = checkMaxDur;
        }

        public StandardParser(List<IPreparser> preParsers, List<IParserComponent> parserComponents,
            in int defaultPortNum, in int maxPortNum, in int defaultInputDur, in int maxInputDur,
            in bool checkMaxDur)
                : this(parserComponents, defaultPortNum, maxPortNum, defaultInputDur, maxInputDur, checkMaxDur)
        {
            Preparsers = preParsers;
        }

        /// <summary>
        /// Parses a message to return a parsed input sequence.
        /// </summary>
        /// <param name="message">The message to parse.</param>
        /// <returns>A <see cref="ParsedInputSequence" /> containing all the inputs parsed.</returns>
        public ParsedInputSequence ParseInputs(string message)
        {
            //Run pre-parsers if we have any
            if (Preparsers != null)
            {
                for (int i = 0; i < Preparsers.Count; i++)
                {
                    message = Preparsers[i].Preparse(message);
                }
            }

            Console.WriteLine("REGEX: " + ParserRegex);

            //Get all the matches from the regex
            MatchCollection matches = Regex.Matches(message, ParserRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            if (matches.Count == 0)
            {
                return new ParsedInputSequence(ParsedInputResults.NormalMsg, null, 0, "Parser: Message is a normal one.");
            }

            //Store the previous index - if there's anything in between that's not picked up by the regex, it's an invalid input
            int prevIndex = 0;

            Console.WriteLine($"Match count for \"{message}\" is {matches.Count}");

            //Create our input sequence and inputs
            ParsedInputSequence inputSequence = new ParsedInputSequence();
            List<List<ParsedInput>> parsedInputList = new List<List<ParsedInput>>();

            //Track the total duration of the input sequence
            int totalDur = 0;

            //The current parsed subsequence
            List<ParsedInput> subInputs = new List<ParsedInput>();

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];

                Console.WriteLine($"Match index: {match.Index} | Value: {match.Value} | Length: {match.Length}");
                
                for (int j = 0; j < match.Groups.Count; j++)
                {
                    Console.WriteLine($"{j + 1} group match: \"{match.Groups[j].Name}\" | Index: {match.Groups[j].Index} | Value: {match.Groups[j].Value} | Length: {match.Groups[j].Length}");
                }

                //If there's no match, it should be a normal message
                if (match.Success == false)
                {
                    return new ParsedInputSequence(ParsedInputResults.NormalMsg, null, 0, "Parser: Message is a normal one.");
                }

                //If there's a gap in matches (Ex. "a34ms hi b300ms"), this isn't a valid input and is likely a normal message
                if (match.Index != prevIndex)
                {
                    Console.WriteLine($"PrevIndex: {prevIndex} | Match Index: {match.Index}");

                    return new ParsedInputSequence(ParsedInputResults.NormalMsg, null, 0, "Parser: Message is a normal one, as the indexes don't match.");
                }

                Console.WriteLine($"Match value: {match.Value}");

                //Set up our input
                ParsedInput input = new ParsedInput();
                
                //Set default controller port
                input.controllerPort = DefaultPortNum;

                Console.WriteLine($"Group count: {match.Groups.Count}");

                //Look for the input - this is the only required field
                if (match.Groups.TryGetValue(InputParserComponent.INPUT_GROUP_NAME, out Group inpGroup) == false
                    || inpGroup.Success == false)
                {
                    return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Input is missing at around index {match.Index}.");
                }

                input.name = inpGroup.Value;

                Console.WriteLine($"FOUND INPUT NAME: \"{input.name}\"");

                bool hasHold = match.Groups.TryGetValue(HoldParserComponent.HOLD_GROUP_NAME, out Group holdGroup) && holdGroup?.Success == true;
                bool hasRelease = match.Groups.TryGetValue(ReleaseParserComponent.RELEASE_GROUP_NAME, out Group releaseGroup) && releaseGroup?.Success == true;

                Console.WriteLine($"HAS HOLD: {hasHold} | HAS RELEASE: {hasRelease}");

                //Can't have both a hold and a release
                if (hasHold == true && hasRelease == true)
                {
                    return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Contains both a hold and a release at around index {holdGroup.Index}.");
                }

                //Console.WriteLine("GOT PAST HOLD");

                input.hold = hasHold;
                input.release = hasRelease;

                //Check if a port number exists
                if (match.Groups.TryGetValue(PortParserComponent.PORT_GROUP_NAME, out Group portGroup) == true
                    && portGroup.Success == true)
                {
                    Console.WriteLine($"Port group success: {portGroup.Success} | I: {portGroup.Index} - L: {portGroup.Length}");

                    //Find the number
                    if (match.Groups.TryGetValue(PortParserComponent.PORT_NUM_GROUP_NAME, out Group portNumGroup) == false
                        || portNumGroup.Success == false)
                    {
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Controller port number not specified at index {portGroup.Index}.");
                    }

                    Console.WriteLine($"Port num group success: {portNumGroup.Success} | I: {portNumGroup.Index} - L: {portNumGroup.Length}");

                    string portNumStr = portNumGroup.Value;
                    if (int.TryParse(portNumStr, out int cPort) == false)
                    {
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Controller port number is invalid at index {portNumGroup.Index}.");
                    }

                    //Subtract 1 from the port
                    //While the input syntax isn't zero-based, the internal numbers are
                    input.controllerPort = cPort - 1;

                    if (input.controllerPort < 0)
                    {
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Controller port number is too low at index {portNumGroup.Index}.");
                    }
                }

                //Controller port is greater than max port number
                if (input.controllerPort > MaxPortNum)
                {
                    return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Port number specified is greater than the max port at around index {portGroup.Index}.");
                }

                //Console.WriteLine("GOT PAST PORT");

                //Check for percentage
                if (match.Groups.TryGetValue(PercentParserComponent.PERCENT_GROUP_NAME, out Group percentGroup) == true
                    && percentGroup.Success == true)
                {
                    string percentParseStr = percentGroup.Value;

                    if (match.Groups.TryGetValue(PercentParserComponent.PERCENT_NUM_GROUP_NAME, out Group percentNumGroup) == false
                        || percentNumGroup.Success == false)
                    {
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Percentage number not found at index {percentGroup.Index}.");
                    }

                    //Parse the percentage value
                    string percentStr = percentNumGroup.Value;

                    //Validate this as a number
                    if (int.TryParse(percentStr, out int percent) == false)
                    {
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Percentage is invalid at index {percentNumGroup.Index}.");
                    }

                    //The percentage can't be less than 0 or greater than 100
                    if (percent < 0 || percent > 100)
                    {
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Percentage is less than 0 or greater than 100 at index {percentNumGroup.Index}.");
                    }

                    input.percent = percent;
                }
                else
                {
                    input.percent = DefaultPercentage;
                }

                //Console.WriteLine("GOT PAST PERCENT");

                //Check for duration
                bool hasMs = match.Groups.TryGetValue(MillisecondParserComponent.MS_DUR_GROUP_NAME, out Group msGroup) && msGroup?.Success == true;
                bool hasSec = match.Groups.TryGetValue(SecondParserComponent.SEC_DUR_GROUP_NAME, out Group secGroup) && secGroup?.Success == true;

                Console.WriteLine($"Has MS: {hasMs} | HasSec: {hasSec}");

                //Can't have both durations
                if (hasMs == true && hasSec == true)
                {
                    return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Contains both 'ms' and 's' for duration at around index {msGroup.Index}.");
                }

                if (hasMs == true || hasSec == true)
                {
                    //Get the duration
                    if (match.Groups.TryGetValue(MillisecondParserComponent.DUR_NUM_GROUP_NAME, out Group durGroup) == false
                        || durGroup.Success == false)
                    {
                        //Show a different error message based on which duration type is missing
                        string errMsg = (hasMs == true)
                            ? $"Parser error: Millisecond duration not found at around index {msGroup.Index}."
                            : $"Parser error: Second duration not found at around index {secGroup.Index}.";
                        
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, errMsg);
                    }

                    string durStr = durGroup.Value;

                    //Check for a duration
                    if (int.TryParse(durStr, out int durVal) == false)
                    {
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Invalid duration at index {durGroup.Index}.");
                    }

                    input.duration_type = "ms";
                    input.duration = durVal;

                    //If we have seconds, increase the duration and change the duration type
                    if (hasSec == true)
                    {
                        input.duration_type = "s";
                        input.duration = durVal * 1000;
                    }
                }
                else
                {
                    input.duration_type = "ms";
                    input.duration = DefaultInputDuration;
                }

                //Console.WriteLine("GOT PAST DURATION");

                subInputs.Add(input);

                //If there's no simultaneous input, set up a new list
                if (match.Groups.TryGetValue(SimultaneousParserComponent.SIMULTANEOUS_GROUP_NAME, out Group simultaneousGroup) == false
                    || simultaneousGroup.Success == false)
                {
                    parsedInputList.Add(subInputs);
                    subInputs = new List<ParsedInput>();
                }

                totalDur += input.duration;

                //Exceeded duration
                if (CheckMaxDur == true && totalDur > MaxInputDuration)
                {
                    return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Input sequence exceeds max input duration at around index {match.Index}.");
                }

                Console.WriteLine("GOT PAST MAX DUR CHECK");

                int prevPrev = prevIndex;
                prevIndex = match.Index + match.Length;

                Console.WriteLine($"PrevIndex WAS: {prevPrev} | NOW: {prevIndex}");
            }

            //We still have a subinput in the list, meaning a simultaneous input wasn't closed
            if (subInputs.Count > 0)
            {
                return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Simultaneous specified with no input at end at index {message.Length - 1}.");
            }

            //Console.WriteLine("GOT PAST SIMULTANEOUS NOT CLOSED");

            //If there's more past what the regex caught, this isn't a valid input and is likely a normal message
            if (prevIndex != message.Length)
            {
                Console.WriteLine($"PREVINDEX: {prevIndex} | MESSAGE LENGTH: {message.Length}");

                return new ParsedInputSequence(ParsedInputResults.NormalMsg, null, 0, "Parser: Message is a normal one.");
            }

            //Console.WriteLine("EVERYTHING IS GOOD");

            //We're good at this point, so set all the values and return the result
            inputSequence.ParsedInputResult = ParsedInputResults.Valid;
            inputSequence.Inputs = parsedInputList;
            inputSequence.TotalDuration = totalDur;
            inputSequence.Error = string.Empty;

            return inputSequence;
        }
    }
}