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
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace TRBot.Parsing
{
    /// <summary>
    /// The parser for input.
    /// </summary>
    /// <remarks>
    /// Constant Regex expressions are compiled to increase performance of subsequent Match calls.
    /// This is favorable since we have only a few Regex expressions that are run many times.
    /// </remarks>
    [Obsolete("This parser is deprecated. Please use the more flexible StandardParser or create your own implementing the IParser interface.", false)]
    public class Parser
    {
        public const string DEFAULT_PARSE_REGEX_PORT_INPUT = @"&";
        public const string DEFAULT_PARSE_REGEX_HOLD_INPUT = @"_";
        public const string DEFAULT_PARSE_REGEX_RELEASE_INPUT = @"-";
        public const string DEFAULT_PARSE_REGEX_PLUS_INPUT = @"\+";
        public const string DEFAULT_PARSE_REGEX_PERCENT_INPUT = @"%";
        public const string DEFAULT_PARSE_REGEX_MILLISECONDS_INPUT = @"ms";
        public const string DEFAULT_PARSE_REGEX_SECONDS_INPUT = @"s";
        
        public const string DEFAULT_PARSER_REGEX_MACRO_INPUT = @"#";

        public const int PARSER_DEFAULT_PERCENT = 100;
        public const string PARSER_DEFAULT_DUR_TYPE = DEFAULT_PARSE_REGEX_MILLISECONDS_INPUT;

        private const string DEFAULT_PARSE_MACRO_REGEX = DEFAULT_PARSER_REGEX_MACRO_INPUT + @"[a-zA-Z0-9\(\,\.\+_\-&%!]*";

        /// <summary>
        /// The start of the input regex string.
        /// </summary>
        public const string DEFAULT_PARSE_REGEX_START = @"(" + DEFAULT_PARSE_REGEX_PORT_INPUT + @"\d+)?" + "([" + DEFAULT_PARSE_REGEX_HOLD_INPUT + DEFAULT_PARSE_REGEX_RELEASE_INPUT + "])?(";

        /// <summary>
        /// The end of the input regex string.
        /// </summary>
        public const string DEFAULT_PARSE_REGEX_END = @")(\d+" + DEFAULT_PARSE_REGEX_PERCENT_INPUT + @")?((\d+" + DEFAULT_PARSE_REGEX_MILLISECONDS_INPUT + @")|(\d+" + DEFAULT_PARSE_REGEX_SECONDS_INPUT + @"))?(" + DEFAULT_PARSE_REGEX_PLUS_INPUT + ")?";

        private static Comparison<DynamicMacroSub> SubCompare = SubComparison;

        private string ParseRegexStart = DEFAULT_PARSE_REGEX_START;
        private string ParseRegexEnd = DEFAULT_PARSE_REGEX_END;

        static Parser()
        {
            //Set Regex cache size
            Regex.CacheSize = 32;
        }

        public Parser()
        {

        }

        public Parser(string parseRegexStart, string parseRegexEnd)
        {
            ParseRegexStart = parseRegexStart;
            ParseRegexEnd = parseRegexEnd;
        }

        /// <summary>
        /// Expands out repeated inputs.
        /// </summary>
        public string Expandify(string message)
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

        public string PopulateVariables(string macro_contents, List<string> variables)
        {
            int count = variables.Count;
            for (int i = 0; i < count; i++)
            {
                string v = variables[i];
                macro_contents = Regex.Replace(macro_contents, "<" + i + ">", v);
                //Console.WriteLine($"Macro Contents: {macro_contents}");
            }
            return macro_contents;
        }

        public string PopulateMacros(string message, IQueryable<InputMacro> macroData)
        {   
            //There are no macros, so just return the original message
            if (macroData == null || macroData.Count() == 0)
            {
                return message;
            }

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
                MatchCollection possible_macros = Regex.Matches(message, DEFAULT_PARSE_MACRO_REGEX, regexOptions);

                //Console.WriteLine($"Possible macros: {possible_macros} | {possible_macros.Count}");

                List<DynamicMacroSub> subs = null;
                foreach (Match p in possible_macros)
                {
                    string macro_name = Regex.Replace(message.Substring(p.Index, p.Length), @"\(.*\)", string.Empty, regexOptions);

                    //Console.WriteLine($"Macro name: {macro_name}");

                    string macro_name_generic = string.Empty;
                    int arg_index = macro_name.IndexOf("(");

                    if (arg_index != -1)
                    {
                        //Console.WriteLine($"Arg Index: {arg_index} | P index: {p.Index} | P len: {p.Length} | message Len: {message.Length}");

                        int maxIndex = p.Index + p.Length;

                        //This doesn't parse correctly - there's a missing ')', so simply return
                        if (maxIndex >= message.Length)
                        {
                            return message;
                        }

                        string sub = message.Substring(p.Index, p.Length + 1);

                        //Console.WriteLine($"Sub: {sub}");

                        macro_args = Regex.Match(sub, @"\(.*\)", regexOptions);

                        //Console.WriteLine($"Macro Arg match: {macro_args.Value} | {macro_args.Length}");

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

                    //Console.WriteLine($"Macro name generic: \"{macro_name_generic}\" | Len: {macro_name_generic.Length}");

                    string longest = string.Empty;
                    int end = 0;

                    //Look through the parser macro list for performance
                    //Handle no macro (Ex. "#" alone)
                    if (macro_name_generic.Length > 1)
                    {
                        //Use string comparison for the first two characters
                        //If the IQueryable is from a database such as SQLite, we can't do character comparisons in a Where clause
                        //This may be a performance regression from the previous dictionary lookup,
                        //but at the same time there may be indexing on the data; conduct tests to make sure and note it
                        string startedWith = macro_name_generic.Substring(0, 2);
                        List<InputMacro> macroList = macroData.Where((m) => m.MacroName.StartsWith(startedWith)).ToList();

                        //Console.WriteLine($"Found {macroList.Count} macros starting with \"{startedWith}\"");

                        //Go through each macro and find the longest match
                        //This ensures it works with other inputs as well
                        //For instance, if "#test" is a macro and the input is "#testa",
                        //it should perform "#test" followed by "a" instead of stopping because it couldn't find "#testa"
                        for (int i = 0; i < macroList.Count; i++)
                        {
                            string macro = macroList[i].MacroName;
                    
                            if (macro_name_generic.Contains(macro) == true)
                            {
                                if (macro.Length > longest.Length) longest = macro;
                            }
                            end = p.Index + longest.Length;
                        }
                    }

                    if (string.IsNullOrEmpty(longest) == false)
                    {
                        //Console.WriteLine($"Longest match found is \"{longest}\"");

                        if (subs == null)
                            subs = new List<DynamicMacroSub>(4);

                        if (macro_argsarr.Count > 0)
                        {
                            subs.Add(new DynamicMacroSub(longest, p.Index, p.Index + p.Length + 1, macro_argsarr));
                        }
                        else
                        {
                            subs.Add(new DynamicMacroSub(longest, p.Index, end, macro_argsarr));
                        }
                    }
                }

                string str = string.Empty;
                if (subs?.Count > 0)
                {
                    //Sort by start of the macro index
                    subs.Sort(SubCompare);

                    found_macro = true;
                    str = message.Substring(0, subs[0].StartIndex);
                    DynamicMacroSub def = default;
                    DynamicMacroSub prev = default;
                    foreach (var current in subs)
                    {
                        InputMacro inpMacro = macroData.FirstOrDefault((mac) => mac.MacroName == current.MacroName);

                        //The collection must have been modified at some point in between parsing and now if this macro is null
                        //There's nothing we can do here since there are no valid inputs mapped to the macro, so return
                        if (inpMacro == null)
                        {
                            Console.WriteLine($"Error: Macro \"{current.MacroName}\" was removed while parsing - exiting macro parsing.");
                            return message;
                        }

                        if (prev != def)
                        {
                            str += message.Substring(prev.EndIndex, current.StartIndex - prev.EndIndex);
                        }

                        string macroValue = macroData.FirstOrDefault((mac) => mac.MacroName == current.MacroName).MacroValue;

                        str += PopulateVariables(macroValue, current.Variables);
                        prev = current;
                    }
                    str += message.Substring(prev.EndIndex);
                    message = str;
                }
                count += 1;
            }

            //sw.Stop();
            //Console.WriteLine($"SW MS for PopulateMacros: {sw.ElapsedMilliseconds}");

            return message;
        }

        public string PopulateSynonyms(string message, IEnumerable<InputSynonym> inputSynonyms)
        {
            if (inputSynonyms != null)
            {
                foreach (InputSynonym synonym in inputSynonyms)
                {
                    message = message.Replace(synonym.SynonymName, synonym.SynonymValue);
                }
            }

            return message;
        }

        /// <summary>
        /// A convenient helper method that modifies a message so it's ready to be parsed by the parser.
        /// </summary>
        /// <param name="message">The message to be prepared for parsing.</param>
        /// <param name="macroData">Data for input macros.</param>
        /// <param name="synonymData">Data for input synonyms.</param>
        /// <returns>A string ready to be parsed by the parser.</returns>
        public string PrepParse(string message, IQueryable<InputMacro> macroData, IEnumerable<InputSynonym> synonymData)
        {
            //Console.WriteLine("Message: " + message);

            //Replace whitespace, populate macros, then expand the string
            string noWhiteSpace = ParserUtilities.RemoveAllWhitespace(message);

            string macros = PopulateMacros(noWhiteSpace, macroData);
            //Console.WriteLine("Macros: " + macros);

            string synonyms = PopulateSynonyms(macros, synonymData);
            //Console.WriteLine("Synonyms: " + synonyms);

            string expanded = Expandify(synonyms);
            //Console.WriteLine("Expanded: " + expanded);

            //Replace whitespace after populating everything and convert to lowercase
            string readyLowered = ParserUtilities.RemoveAllWhitespace(expanded).ToLowerInvariant();
            //Console.WriteLine("Ready lowered: " + readyLowered);

            return readyLowered;
        }

        /// <summary>
        /// Parses inputs from an expanded message.
        /// </summary>
        /// <param name="message">The expanded message.</param>
        /// <param name="inputRegex">The input regex to use.</param>
        /// <param name="parserOptions">The <see cref="Parser"/> to use.</param>
        /// <returns>An InputSequence containing information about the parsed inputs.</returns>
        public ParsedInputSequence ParseInputs(string message, string inputRegex, in ParserOptions parserOptions)
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

                return new ParsedInputSequence(ParsedInputResults.NormalMsg, null, 0, "ERR_NORMAL_MSG");
            }

            //Set up the input sequence
            ParsedInputSequence inputSequence = new ParsedInputSequence(ParsedInputResults.Valid, new List<List<ParsedInput>>(matches.Count), 0, string.Empty);

            //Store the previous index - if there's anything in between that's not picked up by the regex, it's an invalid input
            int prevIndex = 0;
            bool hasPlus = false;
            int maxSubDuration = 0;
            int totalDuration = 0;

            List<ParsedInput> simultaneousInputs = new List<ParsedInput>(4);

            for (int i = 0; i < matches.Count; i++)
            {
                Match m = matches[i];

                //If there's a gap in matches (Ex. "a34ms hi b300ms"), this isn't a valid input and is likely a normal message
                if (m.Index != prevIndex)
                {
                    //Console.WriteLine($"INDEX GAP. CUR: {m.Index} | PREV: {prevIndex}");

                    inputSequence.ParsedInputResult = ParsedInputResults.NormalMsg;
                    inputSequence.Error = "ERR_NORMAL_MSG";
                    break;
                }

                //Get the input using the match information
                ParsedInput input = GetInputFast(m, parserOptions.DefaultControllerPort, parserOptions.DefaultInputDur, ref prevIndex, ref hasPlus);

                //Console.WriteLine($"REGEX MATCH INDEX: {m.Index} | LENGTH: {m.Length} | MATCH: {m.Value}");

                //Console.WriteLine(input.ToString());

                //There's an error, so something went wrong
                if (string.IsNullOrEmpty(input.Error) == false)
                {
                    inputSequence.ParsedInputResult = ParsedInputResults.Invalid;
                    inputSequence.Error = input.Error + " for: \"" + m.Value + "\"";

                    break;
                }

                //Check for the max sub-duration (Ex. "a300ms+b500ms" should be 500ms)
                if (input.Duration > maxSubDuration)
                {
                    maxSubDuration = input.Duration;
                }

                simultaneousInputs.Add(input);

                //There's a plus at the very end of the input sequence, which is invalid
                if (hasPlus == true && i == (matches.Count - 1))
                {
                    inputSequence.ParsedInputResult = ParsedInputResults.Invalid;
                    inputSequence.Error = "ERR_PLUS_AT_END";
                    break;
                }

                //If there's no plus, add all current simultaneous inputs to the final input list and start over
                if (hasPlus == false)
                {
                    inputSequence.Inputs.Add(simultaneousInputs);
                    simultaneousInputs = new List<ParsedInput>(4);

                    totalDuration += maxSubDuration;
                    maxSubDuration = 0;

                    inputSequence.TotalDuration = totalDuration;

                    //Check for max duration and break out early if so
                    if (parserOptions.CheckMaxDur == true && totalDuration > parserOptions.MaxInputDur)
                    {
                        inputSequence.ParsedInputResult = ParsedInputResults.Invalid;
                        inputSequence.Error = "ERR_MAX_DURATION";
                        break;
                    }
                }
            }

            //sw.Stop();
            //Console.WriteLine($"SW MS for ParseInputs: {sw.ElapsedMilliseconds}");

            //If there's more past what the regex caught, this isn't a valid input and is likely a normal message
            if (inputSequence.ParsedInputResult == ParsedInputResults.Valid && prevIndex != message.Length)
            {
                //Console.WriteLine($"PREVINDEX: {prevIndex} | MESSAGE LENGTH: {message.Length}");

                inputSequence.ParsedInputResult = ParsedInputResults.NormalMsg;
                inputSequence.Error = "ERR_NORMAL_MSG";
            }

            return inputSequence;
        }

        private ParsedInput GetInputFast(Match regexMatch, in int defControllerPort, in int defaultInputDur, ref int prevIndex, ref bool hasPlus)
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

            ParsedInput input = ParsedInput.Default(defaultInputDur);
            input.ControllerPort = defControllerPort;

            //Check the top level success - if no matches at all or there's a gap, this isn't a valid input
            if (regexMatch.Success == false || regexMatch.Index != prevIndex)
            {
                input.Error = "ERR_INVALID_INPUT";
                return input;
            }

            Group portGroup = regexMatch.Groups[portIndex];
            if (portGroup.Success == true && string.IsNullOrEmpty(portGroup.Value) == false)
            {
                string rawPortStr = portGroup.Value;
                string portNumStr = rawPortStr.Substring(Parser.DEFAULT_PARSE_REGEX_PORT_INPUT.Length, rawPortStr.Length - Parser.DEFAULT_PARSE_REGEX_PORT_INPUT.Length);

                if (int.TryParse(portNumStr, out int portnum) == false)
                {
                    input.Error = "ERR_INVALID_CONTROLLER_PORT";
                    return input;
                }

                //Set it to the port minus 1 (Ex. 1 returns port 0)
                input.ControllerPort = portnum - 1;
            }

            //Hold or release modifier
            Group holdSubGroup = regexMatch.Groups[holdSubIndex];
            if (holdSubGroup.Success == true)
            {
                if (holdSubGroup.Value == Parser.DEFAULT_PARSE_REGEX_HOLD_INPUT)
                    input.Hold = true;
                else if (holdSubGroup.Value == Parser.DEFAULT_PARSE_REGEX_RELEASE_INPUT)
                    input.Release = true;
            }

            //Input name
            Group inputGroup = regexMatch.Groups[inputIndex];

            if (inputGroup.Success == false || string.IsNullOrEmpty(inputGroup.Value) == true)
            {
                input.Error = "ERR_NO_INPUT";
                return input;
            }

            input.Name = inputGroup.Value;

            Group percentGroup = regexMatch.Groups[percentIndex];

            //Check the percentage
            if (percentGroup.Success == true && string.IsNullOrEmpty(percentGroup.Value) == false)
            {
                string rawPercentStr = percentGroup.Value;
                string percent = rawPercentStr.Substring(0, rawPercentStr.Length - Parser.DEFAULT_PARSE_REGEX_PERCENT_INPUT.Length);
                
                if (double.TryParse(percent, out double percentage) == false)
                {
                    input.Error = "ERR_INVALID_PERCENTAGE";
                    return input;
                }

                //The percentage can't be less than 0 or greater than 100
                if (percentage < 0 || percentage > 100)
                {
                    input.Error = "ERR_INVALID_PERCENTAGE";
                    return input;
                }

                input.Percent = percentage;
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
                    input.Error = "ERR_INVALID_MS_DURATION";
                    return input;
                }

                input.Duration = msDur;
                input.DurationType = InputDurationTypes.Milliseconds;
            }
            //Check seconds
            else if (durSecGroup.Success == true && string.IsNullOrEmpty(durSecGroup.Value) == false)
            {
                string rawsecStr = durSecGroup.Value;
                string secStr = rawsecStr.Substring(0, rawsecStr.Length - 1);

                if (int.TryParse(secStr, out int secDur) == false)
                {
                    input.Error = "ERR_INVALID_SEC_DURATION";
                    return input;
                }

                input.Duration = secDur * 1000;
                input.DurationType = InputDurationTypes.Seconds;
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
        public string BuildInputRegex(IList<string> validInputs)
        {
            return BuildInputRegex(ParseRegexStart, ParseRegexEnd, validInputs);
        }

        /// <summary>
        /// Builds a regex expression for the parser to use given a start regex, end regex, and a set of valid input names.
        /// </summary>
        /// <param name="parseRegexStart">The start regex expression to use</param>
        /// <param name="parseRegexEnd">The end regex expression to use.</param>
        /// <param name="validInputs">The valid input names.</param>
        /// <returns>A string containing a regex expression for the parser to use.</returns>
        public static string BuildInputRegex(string parseRegexStart, string parseRegexEnd, IList<string> validInputs)
        {
            //Set up the regex using the given values
            //Add longer inputs first due to how the parser works
            //This avoids picking up shorter inputs with the same characters first
            IOrderedEnumerable<string> sorted = from str in validInputs
                                                orderby str.Length descending
                                                select str;

            StringBuilder sb = new StringBuilder(parseRegexStart.Length + parseRegexEnd.Length);
            int i = 0;

            sb.Append(parseRegexStart);
            foreach (string s in sorted)
            {
                sb.Append(System.Text.RegularExpressions.Regex.Escape(s));
                if (i != (validInputs.Count - 1))
                {
                    sb.Append('|');
                }
                i++;
            }
            sb.Append(parseRegexEnd);

            string inputRegex = sb.ToString();
            
            return inputRegex;
        }

        private static int SubComparison(DynamicMacroSub val1, DynamicMacroSub val2)
        {
            return val1.StartIndex.CompareTo(val2.StartIndex);
        }
    }
}
