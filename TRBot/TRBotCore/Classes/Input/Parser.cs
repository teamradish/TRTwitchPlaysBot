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

namespace TRBot
{
    /// <summary>
    /// The parser for input.
    /// </summary>
    /// <remarks>
    /// Kimimaru: Constant Regex expressions are compiled to increase performance of subsequent Match calls.
    /// This is favorable since we have only a few Regex expressions that are run many times.
    /// </remarks>
    public static class Parser
    {
        /// <summary>
        /// The validation types for inputs.
        /// </summary>
        public enum InputValidationTypes
        {
            NormalMsg, Valid, Invalid
        }

        public const string ParseRegexPortInput = @"&";
        public const string ParseRegexHoldInput = @"_";
        public const string ParseRegexReleaseInput = @"-";
        public const string ParseRegexPlusInput = @"\+";
        public const string ParseRegexPercentInput = @"%";
        public const string ParseRegexMillisecondsInput = @"ms";
        public const string ParseRegexSecondsInput = @"s";

        public const int ParserDefaultPercent = 100;
        public const string ParserDefaultDurType = ParseRegexMillisecondsInput;

        /// <summary>
        /// The start of the input regex string.
        /// </summary>
        public const string ParseRegexStart = @"(" + ParseRegexPortInput + @"\d+)?" + "([" + ParseRegexHoldInput + ParseRegexReleaseInput + "])?(";

        /// <summary>
        /// The end of the input regex string.
        /// </summary>
        public const string ParseRegexEnd = @")(\d+" + ParseRegexPercentInput + @")?((\d+" + ParseRegexMillisecondsInput + @")|(\d+" + ParseRegexSecondsInput + @"))?(" + ParseRegexPlusInput + ")?";

        private static Comparison<(string, (int, int), List<string>)> SubCompare = SubComparison;

        static Parser()
        {
            //Set Regex cache size
            Regex.CacheSize = 32;
        }

        /// <summary>
        /// Expands out repeated inputs.
        /// </summary>
        public static string Expandify(string message)
        {
            string newMessage = message;
            StringBuilder strBuilder = null;

            const RegexOptions regexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;

            //Find text within brackets with a number of repetitions
            //Get the innermost matches first

            //Uncomment these stopwatch lines to time it
            //Stopwatch sw = Stopwatch.StartNew();

            const string regex = @"\[([^\[\]]*\])\*(\d{1,3})";
            Match m = Regex.Match(newMessage, regex, regexOptions);
            
            while (m.Success == true)
            {
                //Initialize StringBuilder if not initialized
                if (strBuilder == null)
                {
                    strBuilder = new StringBuilder(message, message.Length * 3);
                }

                //Get the group
                Group containedGroup = m.Groups[1];
                string containedStr = containedGroup.Value;
                if (containedGroup.Success == false)
                {
                    return newMessage;
                }

                //Get the number of repetitions
                Group repetitionsGroup = m.Groups[2];
                string repetitionsStr = repetitionsGroup.Value;
                if (repetitionsGroup.Success == false || string.IsNullOrEmpty(repetitionsStr) == true)
                {
                    return newMessage;
                }

                if (int.TryParse(repetitionsStr, out int num) == false)
                {
                    return newMessage;
                }

                //Remove opening bracket
                strBuilder.Remove(containedGroup.Index - 1, 1);
                //Console.WriteLine("Opening removed: " + strBuilder.ToString());
                
                //If repetitions is 0 or lower, remove the input entirely
                if (num <= 0)
                {
                    strBuilder.Remove(containedGroup.Index - 1, containedGroup.Length + 1 + repetitionsStr.Length);
                    //Console.WriteLine("Zero Str: " + strBuilder.ToString());
                }
                else
                {
                    //Console.WriteLine("Str Length: " + strBuilder.ToString().Length);

                    int contentsStart = containedGroup.Index - 1;
                    int contentsLength = containedGroup.Length - 1;

                    //Remove closing bracket, symbol, and repetitions
                    int start = contentsStart + contentsLength;
                    int removeAmt = 1 + 1 + repetitionsStr.Length;

                    //Console.WriteLine("start: " + strt + " | removeAmt: " + removeAmt);

                    strBuilder.Remove(start, 1 + 1 + repetitionsStr.Length);
                    //Console.WriteLine("CB, S, R removed: " + strBuilder.ToString());

                    //Trim the end of the string so we can add just the text within the brackets
                    string containedStrTrimmed = containedStr.Substring(0, containedStr.Length - 1);

                    //Repeat sequence if greater than 1 to expand it out
                    //Ex. "[a500ms .]*2" should expand out to "a500ms . a500ms ."
                    if (num > 1)
                    {
                        int newStart = contentsStart + (contentsLength * 1);
                        //Console.WriteLine("NEW START: " + newStart + " | INSERTING " + "\"" + containedStr + "\"");

                        //Insert string - the StringBuilder can accurately allocate enough additional memory with this overload
                        strBuilder.Insert(newStart, containedStrTrimmed, num - 1);
                    }

                    //Console.WriteLine("INSERTED: " + strBuilder.ToString());
                }

                newMessage = strBuilder.ToString();

                //Find the next match in case this is a nested repetition
                m = Regex.Match(newMessage, regex, regexOptions);
            }

            //sw.Stop();
            //Console.WriteLine($"SW MS for Expandify: {sw.ElapsedMilliseconds}");

            return newMessage;
        }

        public static string PopulateVariables(string macro_contents, List<string> variables)
        {
            int count = variables.Count;
            for (int i = 0; i < count; i++)
            {
                string v = variables[i];
                macro_contents = Regex.Replace(macro_contents, "<" + i + ">", v);
            }
            return macro_contents;
        }

        public static string PopulateMacros(string message, ConcurrentDictionary<string, string> macros, Dictionary<char, List<string>> macroLookup)
        {   
            message = message.Replace(" ", string.Empty);
            message = Parser.Expandify(message);

            const RegexOptions regexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase;

            //Uncomment these stopwatch lines to time it
            //Stopwatch sw = Stopwatch.StartNew();

            const int MAX_RECURSION = 10;
            int count = 0;
            bool found_macro = true;
            Match macro_args = null;
            List<string> macro_argsarr = null;
            while (count < MAX_RECURSION && found_macro == true)
            {
                found_macro = false;
                MatchCollection possible_macros = Regex.Matches(message, @"#[a-zA-Z0-9\(\,\.]*", regexOptions);
                List<(string, (int, int), List<string>)> subs = null;
                foreach (Match p in possible_macros)
                {
                    string macro_name = Regex.Replace(message.Substring(p.Index, p.Length), @"\(.*\)", string.Empty, regexOptions);
                    string macro_name_generic = string.Empty;
                    int arg_index = macro_name.IndexOf("(");
                    if (arg_index != -1)
                    {
                        string sub = message.Substring(p.Index, p.Length + 1);
                        macro_args = Regex.Match(sub, @"\(.*\)", regexOptions);
                        if (macro_args.Success == true)
                        {
                            int start = p.Index + macro_args.Index + 1;
                            string substr = message.Substring(start, macro_args.Length - 2);
                            macro_argsarr = new List<string>(substr.Split(","));
                            macro_name += ")";
                            macro_name_generic = Regex.Replace(macro_name, @"\(.*\)", string.Empty, regexOptions) + "(";

                            int macroArgsCount = macro_argsarr.Count;
                            for (int i = 0; i < macroArgsCount; i++)
                            {
                                macro_name_generic += "*,";
                            }
                            macro_name_generic = macro_name_generic.Substring(0, macro_name_generic.Length - 1) + ")";
                        }
                    }
                    else
                    {
                        macro_argsarr = new List<string>();
                        macro_name_generic = macro_name;
                    }

                    string longest = string.Empty;
                    int end = 0;

                    //Look through the parser macro list for performance
                    //Handle no macro (Ex. "#" alone)
                    if (macro_name_generic.Length > 1
                        && macroLookup.TryGetValue(macro_name_generic[1], out List<string> macroList) == true)
                    {
                        for (int i = 0; i < macroList.Count; i++)
                        {
                            string macro = macroList[i];
                    
                            if (macro_name_generic.Contains(macro) == true)
                            {
                                if (macro.Length > longest.Length) longest = macro;
                            }
                            end = p.Index + longest.Length;
                        }
                    }

                    if (string.IsNullOrEmpty(longest) == false)
                    {
                        if (subs == null)
                            subs = new List<(string, (int, int), List<string>)>(4);

                        if (macro_argsarr.Count > 0)
                        {
                            subs.Add((longest, (p.Index, p.Index + p.Length + 1), macro_argsarr));
                        }
                        else
                        {
                            subs.Add((longest, (p.Index, end), macro_argsarr));
                        }
                    }
                }

                string str = string.Empty;
                if (subs?.Count > 0)
                {
                    //Sort by start of the macro index
                    subs.Sort(SubCompare);

                    found_macro = true;
                    str = message.Substring(0, subs[0].Item2.Item1);
                    (string, (int, int), List<string>) def = default;
                    (string, (int, int), List<string>) prev = default;
                    foreach (var current in subs)
                    {
                        if (prev != def) str += message.Substring(prev.Item2.Item2, current.Item2.Item1 - prev.Item2.Item2);
                        str += Parser.PopulateVariables(macros[current.Item1], current.Item3);
                        prev = current;
                    }
                    str += message.Substring(prev.Item2.Item2);
                    message = str;
                }
                count += 1;
            }

            //sw.Stop();
            //Console.WriteLine($"SW MS for PopulateMacros: {sw.ElapsedMilliseconds}");

            return message;
        }

        public static string PopulateSynonyms(string message, Dictionary<string, string> inputSynonyms)
        {
            if (inputSynonyms != null)
            {
                foreach (string synonym in inputSynonyms.Keys)
                {
                    message = message.Replace(synonym, inputSynonyms[synonym]);
                }
            }

            return message;
        }

        //This is a C# version of the original Python parser for TPE written by Jdog, aka TwitchPlays_Everything
        //Returns Input object
        /*[Obsolete("Use GetInputFast with ParseInputs for greatly improved performance and readability.", false)]
        private static Input GetInput(string message)
        {
            //Create a default input instance
            Input current_input = Input.Default;

            const string regex = @"^[_-]";
            Match m = Regex.Match(message, regex, RegexOptions.Compiled);

            //If there's a match, trim the message
            if (m.Success == true)
            {
                string c = message.Substring(m.Index, m.Length);
                message = message.Substring(m.Length);

                if (c == "_")
                {
                    current_input.hold = true;
                    current_input.length += 1;
                }
                else if (c == "-")
                {
                    current_input.release = true;
                    current_input.length += 1;
                }
            }

            //Try to match one input, prioritizing the longest match
            int max = 0;
            string valid_input = string.Empty;

            foreach (string button in InputGlobals.ValidInputs)
            {
                if (button == ".")
                    m = Regex.Match(message, @"^\.", RegexOptions.Compiled);
                else
                    m = Regex.Match(message, $"^{button}");

                if (m.Success == true)
                {
                    int length = (m.Index + m.Length) - m.Index;

                    if (length > max)
                    {
                        max = length;
                        current_input.name = message.Substring(m.Index, m.Length);
                    }
                }
            }

            //If not a valid input, break parsing
            if (string.IsNullOrEmpty(current_input.name) == true)
            {
                current_input.error = "ERR_INVALID_INPUT";

                return current_input;
            }
            else
                current_input.length += max;

            //Trim the input from the message
            message = message.Substring(max);

            //Try to match a percent
            const string percentRegex = @"^\d+%";
            m = Regex.Match(message, percentRegex, RegexOptions.Compiled);

            if (m.Success == true)
            {
                current_input.percent = int.Parse(message.Substring(m.Index, m.Length - 1));
                message = message.Substring(m.Length);
                current_input.length += current_input.percent.ToString().Length + 1;

                if (current_input.percent > 100)
                {
                    current_input.error = "ERR_INVALID_PERCENTAGE";
                    return current_input;
                }
            }

            //Try to match a duration
            const string durationRegex = @"^\d+";
            m = Regex.Match(message, durationRegex, RegexOptions.Compiled);

            if (m.Success == true)
            {
                current_input.duration = int.Parse(message.Substring(m.Index, m.Length));
                message = message.Substring(m.Length);
                current_input.length += current_input.duration.ToString().Length;

                //Determine the type of duration
                const string durTypeRegex = @"(s|ms)";
                m = Regex.Match(message, durTypeRegex, RegexOptions.Compiled);

                if (m.Success == true)
                {
                    current_input.duration_type = message.Substring(m.Index, m.Length);
                    message = message.Substring(m.Length);

                    if (current_input.duration_type == "s")
                    {
                        current_input.duration *= 1000;
                        current_input.length += 1;
                    }
                    else
                        current_input.length += 2;
                }
                else
                {
                    current_input.error = "ERR_DURATION_TYPE_UNSPECIFIED";
                    return current_input;
                }
            }
            return current_input;
        }*/

        /// <summary>
        /// Parses inputs from an expanded message.
        /// </summary>
        /// <param name="message">The expanded message.</param>
        /// <param name="inputRegex">The input regex to use.</param>
        /// <param name="parserOptions">The <see cref="Parser"/> to use.</param>
        /// <returns>An InputSequence containing information about the parsed inputs.</returns>
        public static InputSequence ParseInputs(string message, string inputRegex, in ParserOptions parserOptions)
        {
            //Remove all whitespace
            message = message.Replace(" ", string.Empty).ToLower();

            //Full Regex:
            // (&\d)?([_-])?(left|right|a)(\d+%)?((\d+ms)|(\d+s))?(\+)?
            //Replace "left", "right", etc. with all the inputs for the console
            //Group 1 = controller port to send the input to
            //Group 2 = zero or one of '_' or '-' for hold and subtract, respectively
            //Group 3 = the input - exactly one
            //Group 4 = the percentage, including the amount
            //Group 6 = milliseconds, including duration
            //Group 7 = seconds, including duration
            //Group 8 = + sign for performing inputs simultaneously

            //Console.WriteLine(regex);

            //Uncomment these stopwatch lines to time it
            //Stopwatch sw = Stopwatch.StartNew();

            //New method: Get ALL the matches at once and parse them as we go, instead of matching each time and parsing
            //The caveat is all longer inputs with the same characters must be before shorter ones at the start of the input sequence
            //For example: "ls1" must be before "l" or it'll pick up "l" first
            MatchCollection matches = Regex.Matches(message, inputRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            //No matches, so invalid input
            if (matches.Count <= 0)
            {
                //Console.WriteLine("NO MATCHES");

                return new InputSequence(InputValidationTypes.NormalMsg, null, 0, "ERR_NORMAL_MSG");
            }

            //Set up the input sequence
            InputSequence inputSequence = new InputSequence(InputValidationTypes.Valid, new List<List<Input>>(matches.Count), 0, string.Empty);

            //Store the previous index - if there's anything in between that's not picked up by the regex, it's an invalid input
            int prevIndex = 0;
            bool hasPlus = false;
            int maxSubDuration = 0;
            int totalDuration = 0;

            List<Input> simultaneousInputs = new List<Input>(4);

            for (int i = 0; i < matches.Count; i++)
            {
                Match m = matches[i];

                //If there's a gap in matches (Ex. "a34ms hi b300ms"), this isn't a valid input and is likely a normal message
                if (m.Index != prevIndex)
                {
                    //Console.WriteLine($"INDEX GAP. CUR: {m.Index} | PREV: {prevIndex}");

                    inputSequence.InputValidationType = InputValidationTypes.NormalMsg;
                    inputSequence.Error = "ERR_NORMAL_MSG";
                    break;
                }

                //Get the input using the match information
                Input input = GetInputFast(m, parserOptions.DefaultControllerPort, parserOptions.DefaultInputDur, ref prevIndex, ref hasPlus);

                //Console.WriteLine($"REGEX MATCH INDEX: {m.Index} | LENGTH: {m.Length} | MATCH: {m.Value}");

                //Console.WriteLine(input.ToString());

                //There's an error, so something went wrong
                if (string.IsNullOrEmpty(input.error) == false)
                {
                    inputSequence.InputValidationType = InputValidationTypes.Invalid;
                    inputSequence.Error = input.error + " for: \"" + m.Value + "\"";

                    break;
                }

                //Check for the max sub-duration (Ex. "a300ms+b500ms" should be 500ms)
                if (input.duration > maxSubDuration)
                {
                    maxSubDuration = input.duration;
                }

                simultaneousInputs.Add(input);

                //There's a plus at the very end of the input sequence, which is invalid
                if (hasPlus == true && i == (matches.Count - 1))
                {
                    inputSequence.InputValidationType = InputValidationTypes.Invalid;
                    inputSequence.Error = "ERR_PLUS_AT_END";
                    break;
                }

                //If there's no plus, add all current simultaneous inputs to the final input list and start over
                if (hasPlus == false)
                {
                    inputSequence.Inputs.Add(simultaneousInputs);
                    simultaneousInputs = new List<Input>(4);

                    totalDuration += maxSubDuration;
                    maxSubDuration = 0;

                    inputSequence.TotalDuration = totalDuration;

                    //Check for max duration and break out early if so
                    if (parserOptions.CheckMaxDur == true && totalDuration > parserOptions.MaxInputDur)
                    {
                        inputSequence.InputValidationType = InputValidationTypes.Invalid;
                        inputSequence.Error = "ERR_MAX_DURATION";
                        break;
                    }
                }
            }

            //sw.Stop();
            //Console.WriteLine($"SW MS for ParseInputs: {sw.ElapsedMilliseconds}");

            //If there's more past what the regex caught, this isn't a valid input and is likely a normal message
            if (inputSequence.InputValidationType == InputValidationTypes.Valid && prevIndex != message.Length)
            {
                //Console.WriteLine($"PREVINDEX: {prevIndex} | MESSAGE LENGTH: {message.Length}");

                inputSequence.InputValidationType = InputValidationTypes.NormalMsg;
                inputSequence.Error = "ERR_NORMAL_MSG";
            }

            return inputSequence;
        }

        private static Input GetInputFast(Match regexMatch, in int defControllerPort, in int defaultInputDur, ref int prevIndex, ref bool hasPlus)
        {
            //Full Regex:
            // (&\d)?([_-])?(left|right|a)(\d+%)?((\d+ms)|(\d+s))?(\+)?
            //Replace "left", "right", etc. with all the inputs for the console
            //Group 1 = controller port to send the input to
            //Group 2 = zero or one of '_' or '-' for hold and subtract, respectively
            //Group 3 = the input - exactly one
            //Group 4 = the percentage, including the amount
            //Group 6 = milliseconds, including duration
            //Group 7 = seconds, including duration
            //Group 8 = + sign for performing inputs simultaneously
            const int portIndex = 1;
            const int holdSubIndex = 2;
            const int inputIndex = 3;
            const int percentIndex = 4;
            const int msIndex = 6;
            const int secIndex = 7;
            const int plusIndex = 8;

            Input input = Input.Default(defaultInputDur);
            input.controllerPort = defControllerPort;

            //Check the top level success - if no matches at all or there's a gap, this isn't a valid input
            if (regexMatch.Success == false || regexMatch.Index != prevIndex)
            {
                input.error = "ERR_INVALID_INPUT";
                return input;
            }

            Group portGroup = regexMatch.Groups[portIndex];
            if (portGroup.Success == true && string.IsNullOrEmpty(portGroup.Value) == false)
            {
                string rawPortStr = portGroup.Value;
                string portNumStr = rawPortStr.Substring(Parser.ParseRegexPortInput.Length, rawPortStr.Length - Parser.ParseRegexPortInput.Length);

                if (int.TryParse(portNumStr, out int portnum) == false)
                {
                    input.error = "ERR_INVALID_CONTROLLER_PORT";
                    return input;
                }

                //Set it to the port minus 1 (Ex. 1 returns port 0)
                input.controllerPort = portnum - 1;
            }

            //Hold or release modifier
            Group holdSubGroup = regexMatch.Groups[holdSubIndex];
            if (holdSubGroup.Success == true)
            {
                if (holdSubGroup.Value == Parser.ParseRegexHoldInput)
                    input.hold = true;
                else if (holdSubGroup.Value == Parser.ParseRegexReleaseInput)
                    input.release = true;
            }

            //Input name
            Group inputGroup = regexMatch.Groups[inputIndex];

            if (inputGroup.Success == false || string.IsNullOrEmpty(inputGroup.Value) == true)
            {
                input.error = "ERR_NO_INPUT";
                return input;
            }

            input.name = inputGroup.Value;

            Group percentGroup = regexMatch.Groups[percentIndex];

            //Check the percentage
            if (percentGroup.Success == true && string.IsNullOrEmpty(percentGroup.Value) == false)
            {
                string rawPercentStr = percentGroup.Value;
                string percent = rawPercentStr.Substring(0, rawPercentStr.Length - Parser.ParseRegexPercentInput.Length);
                
                if (int.TryParse(percent, out int percentage) == false)
                {
                    input.error = "ERR_INVALID_PERCENTAGE";
                    return input;
                }

                //The percentage can't be less than 0 or greater than 100
                if (percentage < 0 || percentage > 100)
                {
                    input.error = "ERR_INVALID_PERCENTAGE";
                    return input;
                }

                input.percent = percentage;
            }

            Group durMsGroup = regexMatch.Groups[msIndex];
            Group durSecGroup = regexMatch.Groups[secIndex];

            //Check milliseconds
            if (durMsGroup.Success == true && string.IsNullOrEmpty(durMsGroup.Value) == false)
            {
                string rawmsStr = durMsGroup.Value;
                string msStr = rawmsStr.Substring(0, rawmsStr.Length - 2);
                
                if (int.TryParse(msStr, out int msDur) == false)
                {
                    input.error = "ERR_INVALID_MS_DURATION";
                    return input;
                }

                input.duration = msDur;
                input.duration_type = ParseRegexMillisecondsInput;
            }
            //Check seconds
            else if (durSecGroup.Success == true && string.IsNullOrEmpty(durSecGroup.Value) == false)
            {
                string rawsecStr = durSecGroup.Value;
                string secStr = rawsecStr.Substring(0, rawsecStr.Length - 1);

                if (int.TryParse(secStr, out int secDur) == false)
                {
                    input.error = "ERR_INVALID_SEC_DURATION";
                    return input;
                }

                input.duration = secDur * 1000;
                input.duration_type = ParseRegexSecondsInput;
            }

            Group plusGroup = regexMatch.Groups[plusIndex];

            //Check for a plus sign to perform the next input simultaneously
            hasPlus = (plusGroup.Success == true && string.IsNullOrEmpty(plusGroup.Value) == false);

            prevIndex = regexMatch.Index + regexMatch.Length;

            return input;
        }

        /// <summary>
        /// Builds a regex expression for the parser to use given a set of valid input names.
        /// </summary>
        /// <param name="validInputs">The valid input names.</param>
        /// <returns>A string containing a regex expression for the parser to use.</returns>
        public static string BuildInputRegex(string[] validInputs)
        {
            //Set up the regex using the given values
            //Add longer inputs first due to how the parser works
            //This avoids picking up shorter inputs with the same characters first
            IOrderedEnumerable<string> sorted = from str in validInputs
                                                orderby str.Length descending
                                                select str;

            StringBuilder sb = new StringBuilder(Parser.ParseRegexStart.Length + Parser.ParseRegexEnd.Length);
            int i = 0;

            sb.Append(Parser.ParseRegexStart);
            foreach (string s in sorted)
            {
                sb.Append(System.Text.RegularExpressions.Regex.Escape(s));
                if (i != (validInputs.Length - 1))
                {
                    sb.Append('|');
                }
                i++;
            }
            sb.Append(Parser.ParseRegexEnd);

            string inputRegex = sb.ToString();
            
            return inputRegex;
        }

        //Returns list containing: [Valid, input_sequence]
        //Or: [Invalid, input that it failed on]
        /*[Obsolete("Use ParseInputs instead for better type safety and greatly improved performance and readability.", false)]
        public static (bool, List<List<Input>>, bool, int) Parse(string message)
        {
            bool contains_start_input = false;
            message = message.Replace(" ", string.Empty).ToLower();
            List<Input> input_subsequence = null;
            List<List<Input>> input_sequence = new List<List<Input>>(8);
            int duration_counter = 0;

            message = PopulateSynonyms(message);

            //System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            while (message.Length > 0)
            {
                input_subsequence = new List<Input>(8);
                int subduration_max = 0;
                Input current_input = GetInput(message);
                
                if (string.IsNullOrEmpty(current_input.error) == false)
                    return (false, new List<List<Input>>(1) { new List<Input>(1) { current_input } }, false, subduration_max);

                message = message.Substring(current_input.length);
                input_subsequence.Add(current_input);

                if (current_input.duration > subduration_max)
                    subduration_max = current_input.duration;

                if (message.Length > 0)
                {
                    while (message[0] == '+')
                    {
                        if (message.Length > 0)
                            message = message.Substring(1);
                        else
                            break;

                        current_input = GetInput(message);

                        //
                        // if (current_input.name == "plus")
                        //     contains_start_input = true;
                        //

                        if (string.IsNullOrEmpty(current_input.error) == false)
                            return (false, new List<List<Input>>(1) { new List<Input>(1) { current_input }  }, false, subduration_max);

                        message = message.Substring(current_input.length);
                        input_subsequence.Add(current_input);

                        if (current_input.duration > subduration_max)
                            subduration_max = current_input.duration;

                        if (message.Length == 0)
                            break;
                    }
                }

                duration_counter += subduration_max;

                if (duration_counter > BotProgram.BotData.MaxInputDuration)
                {
                    current_input.error = "ERR_DURATION_MAX";
                    return (false, new List<List<Input>>(1) { new List<Input>(1) { current_input }  }, false, subduration_max);
                }

                input_sequence.Add(input_subsequence);
            }

            //sw.Stop();
            //Console.WriteLine($"Elapsed: {sw.ElapsedMilliseconds}");

            return (true, input_sequence, contains_start_input, duration_counter);
        }*/

        private static int SubComparison((string, (int, int), List<string>) val1, (string, (int, int), List<string>) val2)
        {
            return val1.Item2.Item1.CompareTo(val2.Item2.Item1);
        }

        /// <summary>
        /// Contains input data.
        /// </summary>
        public struct Input
        {
            public string name;
            public bool hold;
            public bool release;
            public int percent;
            public int duration;
            public string duration_type;
            public int controllerPort;
            //[Obsolete("length is the total string length of the input, which is no longer necessary for the new parser.", false)]
            //public int length;
            public string error;

            /// <summary>
            /// Returns a default Input.
            /// </summary>
            public static Input Default(in int defaultInputDur) => new Input(string.Empty, false, false, Parser.ParserDefaultPercent, defaultInputDur, Parser.ParserDefaultDurType, 0, /*0,*/ string.Empty);

            public Input(string nme, in bool hld, in bool relse, in int percnt, in int dur, string durType, in int contPort, /*in int len,*/ in string err)
            {
                this.name = nme;
                this.hold = hld;
                this.release = relse;
                this.percent = percnt;
                this.duration = dur;
                this.duration_type = durType;
                this.controllerPort = contPort;
                //this.length = 0;
                this.error = string.Empty;
            }

            public override bool Equals(object obj)
            {
                if (obj is Input inp)
                {
                    return (this == inp);
                }

                return false;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 19;
                    hash = (hash * 37) + ((name == null) ? 0 : name.GetHashCode());
                    hash = (hash * 37) + hold.GetHashCode();
                    hash = (hash * 37) + release.GetHashCode();
                    hash = (hash * 37) + percent.GetHashCode();
                    hash = (hash * 37) + duration.GetHashCode();
                    hash = (hash * 37) + ((duration_type == null) ? 0 : duration_type.GetHashCode());
                    hash = (hash * 37) + controllerPort.GetHashCode();
                    //hash = (hash * 37) + length.GetHashCode();
                    hash = (hash * 37) + ((error == null) ? 0 : error.GetHashCode());
                    return hash;
                }
            }

            public static bool operator ==(Input a, Input b)
            {
                return (a.hold == b.hold && a.release == b.release && a.percent == b.percent
                        && a.duration_type == b.duration_type && a.duration_type == b.duration_type
                        && a.name == b.name && a.controllerPort == b.controllerPort /*&& a.length == b.length*/ && a.error == b.error);
            }

            public static bool operator !=(Input a, Input b)
            {
                return !(a == b);
            }

            public override string ToString()
            {
                return $"\"{name}\" {duration}{duration_type} | H:{hold} | R:{release} | P:{percent} | CPort:{controllerPort} | Err:{error}";
            }
        }

        /// <summary>
        /// Represents a fully parsed input sequence.
        /// </summary>
        public struct InputSequence
        {
            public InputValidationTypes InputValidationType;
            public List<List<Input>> Inputs;
            public int TotalDuration;
            public string Error;

            public InputSequence(in InputValidationTypes inputValidationType, List<List<Input>> inputs, in int totalDuration, string error)
            {
                InputValidationType = inputValidationType;
                Inputs = inputs;
                TotalDuration = totalDuration;
                Error = error;
            }

            public override bool Equals(object obj)
            {
                if (obj is InputSequence inpSeq)
                {
                    return (this == inpSeq);
                }

                return false;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = (hash * 37) + InputValidationType.GetHashCode();
                    hash = (hash * 37) + ((Inputs == null) ? 0 : Inputs.GetHashCode());
                    hash = (hash * 37) + TotalDuration.GetHashCode();
                    hash = (hash * 37) + ((Error == null) ? 0 : Error.GetHashCode());
                    return hash;
                }
            }

            public static bool operator ==(InputSequence a, InputSequence b)
            {
                return (a.InputValidationType == b.InputValidationType
                        && a.Inputs == b.Inputs && a.TotalDuration == b.TotalDuration && a.Error == b.Error);
            }

            public static bool operator !=(InputSequence a, InputSequence b)
            {
                return !(a == b);
            }

            public override string ToString()
            {
                int inputCount = (Inputs == null) ? 0 : Inputs.Count;
                return $"VType:{InputValidationType} | SubInputs:{inputCount} | Duration:{TotalDuration} | Err:{Error}";
            }
        }

        public struct ParserOptions
        {
            public int DefaultControllerPort;
            public int DefaultInputDur;
            public bool CheckMaxDur;
            public int MaxInputDur;

            public ParserOptions(in int defControllerPort, in int defaultInputDur, in bool checkMaxDur)
                : this(defControllerPort, defaultInputDur, checkMaxDur, 0)
            {

            }

            public ParserOptions(in int defControllerPort, in int defaultInputDur,
                in bool checkMaxDur, in int maxInputDur)
            {
                DefaultControllerPort = defControllerPort;
                DefaultInputDur = defaultInputDur;
                CheckMaxDur = checkMaxDur;
                MaxInputDur = maxInputDur;
            }

            public override bool Equals(object obj)
            {
                if (obj is ParserOptions inpSeq)
                {
                    return (this == inpSeq);
                }

                return false;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 19;
                    hash = (hash * 37) + DefaultControllerPort.GetHashCode();
                    hash = (hash * 37) + DefaultInputDur.GetHashCode();
                    hash = (hash * 37) + CheckMaxDur.GetHashCode();
                    hash = (hash * 37) + MaxInputDur.GetHashCode();
                    return hash;
                }
            }

            public static bool operator ==(ParserOptions a, ParserOptions b)
            {
                return (a.DefaultControllerPort == b.DefaultControllerPort && a.DefaultInputDur == b.DefaultInputDur
                        && a.CheckMaxDur == b.CheckMaxDur && a.MaxInputDur == b.MaxInputDur);
            }

            public static bool operator !=(ParserOptions a, ParserOptions b)
            {
                return !(a == b);
            }
        }
    }
}
