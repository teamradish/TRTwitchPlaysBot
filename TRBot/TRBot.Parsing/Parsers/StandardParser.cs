/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, version 3 of the License.
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
using System.Linq;
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
        /// The default percentage value for inputs if no percentage is specified.
        /// </summary>
        public const double DEFAULT_PERCENT_VAL = 100d;

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
        private double DefaultPercentage = DEFAULT_PERCENT_VAL;

        /// <summary>
        /// The parser components used to build the parser.
        /// </summary>
        private List<IParserComponent> ParserComponents = null;

        /// <summary>
        /// The pre-parsers to run right before parsing.
        /// </summary>
        private List<IPreparser> Preparsers = null;

        /// <summary>
        /// Creates a parser with settings and utilizing given parser components.
        /// </summary>
        /// <param name="parserComponents">The parser components to use in this parser.</param>
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

        /// <summary>
        /// Creates a parser with settings and utilizing given parser components.
        /// </summary>
        /// <param name="parserComponents">The parser components to use in this parser.</param>
        /// <param name="validInputs">The available valid inputs.</param>
        /// <param name="defaultPortNum">The default controller port number.</param>
        /// <param name="maxPortNum">The maximum controller port number.</param>
        /// <param name="defaultInputDur">The default input duration for inputs, in milliseconds.</param>
        /// <param name="maxInputDur">The maximum duration of an input sequence, in milliseconds.</param>
        /// <param name="checkMaxDur">Whether to mark the input sequence as invalid if it surpasses the maximum duration.</param>
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

        /// <summary>
        /// Creates a parser with settings and utilizing given parser components and preparsers.
        /// </summary>
        /// <param name="preParsers">The preparsers to use to prepare the input message before parsing.</param>
        /// <param name="parserComponents">The parser components to use in this parser.</param>
        /// <param name="validInputs">The available valid inputs.</param>
        /// <param name="defaultPortNum">The default controller port number.</param>
        /// <param name="maxPortNum">The maximum controller port number.</param>
        /// <param name="defaultInputDur">The default input duration for inputs, in milliseconds.</param>
        /// <param name="maxInputDur">The maximum duration of an input sequence, in milliseconds.</param>
        /// <param name="checkMaxDur">Whether to mark the input sequence as invalid if it surpasses the maximum duration.</param>
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

            //Console.WriteLine("REGEX: " + ParserRegex);

            //Get all the matches from the regex
            MatchCollection matches = Regex.Matches(message, ParserRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            if (matches.Count == 0)
            {
                return new ParsedInputSequence(ParsedInputResults.NormalMsg, null, 0, "Parser: Message is a normal one, as there are no Regex matches.");
            }

            //Store the previous index - if there's anything in between that's not picked up by the regex, it's an invalid input
            int prevIndex = 0;

            //Console.WriteLine($"Match count for \"{message}\" is {matches.Count}");

            //Create our input sequence and inputs
            ParsedInputSequence inputSequence = new ParsedInputSequence();
            List<List<ParsedInput>> parsedInputList = new List<List<ParsedInput>>();

            //Track the total duration of the input sequence
            int totalDur = 0;
            
            //Track the longest subsequence duration
            int longestSubDur = 0;

            //The current parsed subsequence
            List<ParsedInput> subInputs = new List<ParsedInput>();

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];

                //Console.WriteLine($"Match index: {match.Index} | Value: {match.Value} | Length: {match.Length}");
                
                //for (int j = 0; j < match.Groups.Count; j++)
                //{
                //    Console.WriteLine($"{j + 1} group match: \"{match.Groups[j].Name}\" | Index: {match.Groups[j].Index} | Value: {match.Groups[j].Value} | Length: {match.Groups[j].Length}");
                //}

                //If there's no match, it should be a normal message
                if (match.Success == false)
                {
                    return new ParsedInputSequence(ParsedInputResults.NormalMsg, null, 0, "Parser: Message is a normal one, as there's no Regex match.");
                }

                //If there's a gap in matches (Ex. "a34ms hi b300ms"), this isn't a valid input and is likely a normal message
                if (match.Index != prevIndex)
                {
                    //Console.WriteLine($"PrevIndex: {prevIndex} | Match Index: {match.Index}");

                    return new ParsedInputSequence(ParsedInputResults.NormalMsg, null, 0, "Parser: Message is a normal one, as the indexes don't match.");
                }

                //Console.WriteLine($"Match value: {match.Value}");

                //Set up our input
                ParsedInput input = new ParsedInput();
                
                //Set default controller port
                input.ControllerPort = DefaultPortNum;

                //Console.WriteLine($"Group count: {match.Groups.Count}");

                //Look for the input - this is the only required field
                if (match.Groups.TryGetValue(InputParserComponent.INPUT_GROUP_NAME, out Group inpGroup) == false
                    || inpGroup.Success == false)
                {
                    return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Input is missing at around index {match.Index}.");
                }

                input.Name = inpGroup.Value;

                //Console.WriteLine($"FOUND INPUT NAME: \"{input.Name}\"");

                //Check holds and releases
                bool hasHold = match.Groups.TryGetValue(HoldParserComponent.HOLD_GROUP_NAME, out Group holdGroup) && holdGroup?.Success == true;
                bool hasRelease = match.Groups.TryGetValue(ReleaseParserComponent.RELEASE_GROUP_NAME, out Group releaseGroup) && releaseGroup?.Success == true;

                //Console.WriteLine($"HAS HOLD: {hasHold} | HAS RELEASE: {hasRelease}");

                //Can't have both a hold and a release
                if (hasHold == true && hasRelease == true)
                {
                    return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Contains both a hold and a release at around index {holdGroup.Index}.");
                }

                //Console.WriteLine("GOT PAST HOLD");

                input.Hold = hasHold;
                input.Release = hasRelease;

                //Check if a port number exists
                if (match.Groups.TryGetValue(PortParserComponent.PORT_GROUP_NAME, out Group portGroup) == true
                    && portGroup.Success == true)
                {
                    //Console.WriteLine($"Port group success: {portGroup.Success} | I: {portGroup.Index} - L: {portGroup.Length}");

                    //Find the number
                    if (match.Groups.TryGetValue(PortParserComponent.PORT_NUM_GROUP_NAME, out Group portNumGroup) == false
                        || portNumGroup.Success == false)
                    {
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Controller port number not specified at index {portGroup.Index}.");
                    }

                    //Console.WriteLine($"Port num group success: {portNumGroup.Success} | I: {portNumGroup.Index} - L: {portNumGroup.Length}");

                    //Parse the number
                    string portNumStr = portNumGroup.Value;
                    if (int.TryParse(portNumStr, out int cPort) == false)
                    {
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Controller port number is invalid at index {portNumGroup.Index}.");
                    }

                    //Subtract 1 from the port
                    //While the input syntax isn't zero-based, the internal numbers are
                    input.ControllerPort = cPort - 1;

                    if (input.ControllerPort < 0)
                    {
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Controller port number is less than 0 at index {portNumGroup.Index}.");
                    }
                }

                //Controller port is greater than max port number
                if (input.ControllerPort > MaxPortNum)
                {
                    return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Port number specified is greater than the max port of {MaxPortNum} at around index {portGroup.Index}.");
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
                    if (double.TryParse(percentStr, out double percent) == false)
                    {
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Percentage is invalid at index {percentNumGroup.Index}.");
                    }

                    //The percentage can't be less than 0 or greater than 100
                    if (percent < 0d || percent > 100d)
                    {
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Percentage is less than 0 or greater than 100 at index {percentNumGroup.Index}.");
                    }

                    input.Percent = percent;
                }
                else
                {
                    input.Percent = DefaultPercentage;
                }

                //Console.WriteLine("GOT PAST PERCENT");

                //Check for duration
                bool hasMs = match.Groups.TryGetValue(MillisecondParserComponent.MS_DUR_GROUP_NAME, out Group msGroup) && msGroup?.Success == true;
                bool hasSec = match.Groups.TryGetValue(SecondParserComponent.SEC_DUR_GROUP_NAME, out Group secGroup) && secGroup?.Success == true;

                //Console.WriteLine($"Has MS: {hasMs} | HasSec: {hasSec}");

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
                    if (double.TryParse(durStr, out double durVal) == false)
                    {
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Invalid duration at index {durGroup.Index}.");
                    }

                    input.DurationType = InputDurationTypes.Milliseconds;
                    input.Duration = (int)durVal;

                    //If we have seconds, increase the duration and change the duration type
                    if (hasSec == true)
                    {
                        input.Duration = (int)(durVal * 1000d);

                        //If there's no extra decimal value, mark it as a seconds input
                        //Otherwise, keep it as milliseconds
                        int decInt = (int)durVal;

                        if (durVal == decInt)
                        {
                            input.DurationType = InputDurationTypes.Seconds;
                        }
                    }

                    //Account for overflow
                    if (input.Duration < 0 && durVal >= 0d)
                    {
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Input duration integer overflow at index {durGroup.Index}. The max duration for a single input is {int.MaxValue} milliseconds.");
                    }
                }
                else
                {
                    input.DurationType = InputDurationTypes.Milliseconds;
                    input.Duration = DefaultInputDuration;
                }

                //Console.WriteLine("GOT PAST DURATION");

                //Check if this input is the longest in the subsequence
                if (input.Duration > longestSubDur)
                {
                    longestSubDur = input.Duration;
                }

                subInputs.Add(input);

                //If there's no simultaneous input, set up a new list
                if (match.Groups.TryGetValue(SimultaneousParserComponent.SIMULTANEOUS_GROUP_NAME, out Group simultaneousGroup) == false
                    || simultaneousGroup.Success == false)
                {
                    parsedInputList.Add(subInputs);
                    subInputs = new List<ParsedInput>();

                    //Add the longest subsequence duration to the total
                    totalDur += longestSubDur;
                    longestSubDur = 0;

                    //Handle overflow when adding very large numbers
                    if (totalDur < 0)
                    {
                        return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Total input sequence duration integer overflow at around index {match.Index}. The absolute max duration for an input sequence is {int.MaxValue} milliseconds.");
                    }

                    //Console.WriteLine($"Current total dur: {totalDur}");
                }

                //Exceeded duration
                if (CheckMaxDur == true && (totalDur + longestSubDur) > MaxInputDuration)
                {
                    return new ParsedInputSequence(ParsedInputResults.Invalid, null, 0, $"Parser error: Input sequence exceeds max input duration of {MaxInputDuration} at around index {match.Index}.");
                }

                //Console.WriteLine("GOT PAST MAX DUR CHECK");

                int prevPrev = prevIndex;
                prevIndex = match.Index + match.Length;

                //Console.WriteLine($"PrevIndex WAS: {prevPrev} | NOW: {prevIndex}");
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
                //Console.WriteLine($"PREVINDEX: {prevIndex} | MESSAGE LENGTH: {message.Length}");

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

        /// <summary>
        /// Creates a <see cref="StandardParser" /> with the standard parser configuration.
        /// </summary>
        /// <param name="macros">The InputMacro data to use.</param>
        /// <param name="synonyms">The InputSynonym data to use.</param>
        /// <param name="validInputs">The available valid inputs.</param>
        /// <param name="defaultPortNum">The default controller port number.</param>
        /// <param name="maxPortNum">The maximum controller port number.</param>
        /// <param name="defaultInputDur">The default input duration for inputs, in milliseconds.</param>
        /// <param name="maxInputDur">The maximum duration of an input sequence, in milliseconds.</param>
        /// <param name="checkMaxDur">Whether to mark the input sequence as invalid if it surpasses the maximum duration.</param>
        /// <returns>A <see cref="StandardParser" /> standard configuration, including preparsers and parser components.</returns>
        public static StandardParser CreateStandard(IQueryable<InputMacro> macros, IEnumerable<InputSynonym> synonyms,
            IList<string> validInputs, in int defaultPortNum, in int maxPortNum, in int defaultInputDur,
            in int maxInputDur, in bool checkMaxDur)
        {
            List<IPreparser> preparsers = new List<IPreparser>()
            {
                new RemoveWhitespacePreparser(),
                new LowercasePreparser(),
                new InputMacroPreparser(macros),
                new InputSynonymPreparser(synonyms),
                new ExpandPreparser(),
                new RemoveWhitespacePreparser(),
                new LowercasePreparser()
            };

            List<IParserComponent> components = new List<IParserComponent>()
            {
                new PortParserComponent(),
                new HoldParserComponent(),
                new ReleaseParserComponent(),
                new InputParserComponent(validInputs),
                new PercentParserComponent(),
                new MillisecondParserComponent(),
                new SecondParserComponent(),
                new SimultaneousParserComponent()
            };

            StandardParser standardParser = new StandardParser(preparsers, components, defaultPortNum, maxPortNum,
                defaultInputDur, maxInputDur, checkMaxDur);

            return standardParser;
        }
    }
}