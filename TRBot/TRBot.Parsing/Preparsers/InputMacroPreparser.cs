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
using System.Diagnostics;

namespace TRBot.Parsing
{
    /// <summary>
    /// A pre-parser that populates macros based on given information.
    /// </summary>
    [Obsolete("This preparser is deprecated. Use InputMacroPreparserNew for a more performant and accurate macro preparser.", false)]
    public class InputMacroPreparser : IPreparser
    {
        public const string DEFAULT_MACRO_START = @"#";

        private static Comparison<DynamicMacroSub> SubCompare = SubComparison;

        private IQueryable<InputMacro> MacroData = null;

        private readonly string MacroRegex = DEFAULT_MACRO_START + @"[a-zA-Z0-9\(\,\.\+_\-&%!]*";

        public InputMacroPreparser(IQueryable<InputMacro> macroData)
        {
            MacroData = macroData;
        }

        /// <summary>
        /// Pre-parses a string to prepare it for the parser.
        /// </summary>
        /// <param name="message">The message to pre-parse.</param>
        /// <returns>A string containing the modified message.</returns>
        public string Preparse(string message)
        {
            /* NOTE: There is a bug with this that we need to fix!
             * For example, take a macro named "#b" with a value of "b300ms"
             * A message of "# #b" will fail to parse since it expands out to "#b300ms" then keeps looping
             * Finally, it will find "#b" as a macro again and will parse that out to "b300ms300ms", causing it to fail
            */

            //There are no macros, so just return the original message
            if (MacroData == null || MacroData.Count() == 0)
            {
                return message;
            }

            const RegexOptions regexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase;

            //Uncomment these stopwatch lines to time it
            //Stopwatch sw = Stopwatch.StartNew();

            const int MAX_RECURSION = 10;
            int count = 0;
            bool foundMacro = true;
            Match macroArgs = null;
            List<string> macroArgsList = null;
            while (count < MAX_RECURSION && foundMacro == true)
            {
                foundMacro = false;
                MatchCollection macroMatches = Regex.Matches(message, MacroRegex, regexOptions);

                //Console.WriteLine($"Possible macros: {macroMatches} | {macroMatches.Count}");

                List<DynamicMacroSub> subs = null;
                foreach (Match p in macroMatches)
                {
                    //Console.WriteLine($"Match Value: {p.Value} | Index: {p.Index} | Len: {p.Length}");

                    string macroName = Regex.Replace(message.Substring(p.Index, p.Length), @"\(.*\)", string.Empty, regexOptions);

                    //Console.WriteLine($"Macro name: {macroName}");

                    string macroNameGeneric = string.Empty;
                    int argIndex = macroName.IndexOf("(");

                    if (argIndex != -1)
                    {
                        //Console.WriteLine($"Arg Index: {argIndex} | P index: {p.Index} | P len: {p.Length} | message Len: {message.Length}");

                        int maxIndex = p.Index + p.Length;

                        //This doesn't parse correctly - there's a missing ')', so simply return
                        if (maxIndex >= message.Length)
                        {
                            return message;
                        }

                        string sub = message.Substring(p.Index, p.Length + 1);

                        //Console.WriteLine($"Sub: {sub}");

                        macroArgs = Regex.Match(sub, @"\(.*\)", regexOptions);

                        //Console.WriteLine($"Macro Arg match: {macroArgs.Value} | {macroArgs.Length}");

                        if (macroArgs.Success == true)
                        {
                            int start = p.Index + macroArgs.Index + 1;
                            string substr = message.Substring(start, macroArgs.Length - 2);
                            macroArgsList = new List<string>(substr.Split(","));
                            macroName += ")";
                            macroNameGeneric = Regex.Replace(macroName, @"\(.*\)", string.Empty, regexOptions) + "(";

                            int macroArgsCount = macroArgsList.Count;
                            for (int i = 0; i < macroArgsCount; i++)
                            {
                                macroNameGeneric += "*,";
                            }
                            macroNameGeneric = macroNameGeneric.Substring(0, macroNameGeneric.Length - 1) + ")";
                        }
                    }
                    else
                    {
                        macroArgsList = new List<string>();
                        macroNameGeneric = macroName;
                    }

                    //Console.WriteLine($"Macro name generic: \"{macroNameGeneric}\" | Len: {macroNameGeneric.Length}");

                    string longest = string.Empty;
                    int end = 0;

                    //Look through the parser macro list for performance
                    //Handle no macro (Ex. "#" alone)
                    if (macroNameGeneric.Length > 1)
                    {
                        //Use string comparison for the first two characters
                        //If the IQueryable is from a database such as SQLite, we can't do character comparisons in a Where clause
                        //This may be a performance regression from the previous dictionary lookup,
                        //but at the same time there may be indexing on the data; conduct tests to make sure and note it
                        string startedWith = macroNameGeneric.Substring(0, 2);
                        List<InputMacro> macroList = MacroData.Where((m) => m.MacroName.StartsWith(startedWith)).ToList();

                        //Console.WriteLine($"Found {macroList.Count} macros starting with \"{startedWith}\"");

                        //Go through each macro and find the longest match
                        //This ensures it works with other inputs as well
                        //For instance, if "#test" is a macro and the input is "#testa",
                        //it should perform "#test" followed by "a" instead of stopping because it couldn't find "#testa"
                        for (int i = 0; i < macroList.Count; i++)
                        {
                            string macro = macroList[i].MacroName;
                    
                            if (macroNameGeneric.Contains(macro) == true)
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

                        if (macroArgsList.Count > 0)
                        {
                            subs.Add(new DynamicMacroSub(longest, p.Index, p.Index + p.Length + 1, macroArgsList));
                        }
                        else
                        {
                            subs.Add(new DynamicMacroSub(longest, p.Index, end, macroArgsList));
                        }
                    }
                }

                string str = string.Empty;
                if (subs?.Count > 0)
                {
                    //Sort by start of the macro index
                    subs.Sort(SubCompare);

                    foundMacro = true;
                    str = message.Substring(0, subs[0].StartIndex);
                    DynamicMacroSub def = default;
                    DynamicMacroSub prev = default;
                    foreach (var current in subs)
                    {
                        InputMacro inpMacro = MacroData.FirstOrDefault((mac) => mac.MacroName == current.MacroName);

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

                        string macroValue = MacroData.FirstOrDefault((mac) => mac.MacroName == current.MacroName).MacroValue;

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

        public string PopulateVariables(string macroContents, List<string> macroVariables)
        {
            int count = macroVariables.Count;
            for (int i = 0; i < count; i++)
            {
                string v = macroVariables[i];
                macroContents = Regex.Replace(macroContents, "<" + i + ">", v);
                //Console.WriteLine($"Macro Contents: {macroContents}");
            }
            return macroContents;
        }

        private static int SubComparison(DynamicMacroSub val1, DynamicMacroSub val2)
        {
            return val1.StartIndex.CompareTo(val2.StartIndex);
        }
    }
}