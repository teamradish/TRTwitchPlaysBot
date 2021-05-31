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
    public class InputMacroPreparserNew : IPreparser
    {
        public const string DEFAULT_MACRO_START = @"#";
        
        public const string MACRO_GROUP_NAME = "macro";
        public const string MACRO_DYNAMIC_GROUP_NAME = "dynamic";
        public const string MACRO_DYNAMIC_ARGS_GROUP_NAME = "args";

        private IQueryable<InputMacro> MacroData = null;
        private int MaxRecursions = 10;

        private readonly string MacroRegex = @"(?<"+ MACRO_GROUP_NAME + @">\" + DEFAULT_MACRO_START +
            @"[^\#\(\s]*)(?<"+ MACRO_DYNAMIC_GROUP_NAME +
            @">\((?<"+ MACRO_DYNAMIC_ARGS_GROUP_NAME + @">([^,\s],?)+)\))?";

        public InputMacroPreparserNew(IQueryable<InputMacro> macroData)
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
            //There are no macros, so just return the original message
            if (MacroData == null || MacroData.Count() == 0)
            {
                return message;
            }

            foreach (var thing in MacroData)
            {
                Console.WriteLine($"MACRO = {thing.MacroName}");
            }

            const RegexOptions regexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase;

            MatchCollection matches = Regex.Matches(message, MacroRegex, regexOptions);

            //No matches, so return the original message
            if (matches.Count == 0)
            {
                Console.WriteLine("No matches found.");
                return message;
            }

            //Use a ReadOnlySpan to splice and check the string - better performance + no GC overhead
            ReadOnlySpan<char> msgSpan = message;
            
            foreach (Match match in matches)
            {
                if (match.Groups.TryGetValue(MACRO_GROUP_NAME, out Group matchGroup) == false
                    || matchGroup.Success == false)
                {
                    Console.WriteLine($"No match with group \"{MACRO_GROUP_NAME}\" found.");
                    continue;
                }

                string macroNameVal = matchGroup.Value;

                Console.WriteLine($"Found \"{MACRO_GROUP_NAME}\" group with value \"{macroNameVal}\"");

                //Find the longest macro with this name
                InputMacro longestMacro = MacroData
                .Where((m) => m.MacroName.StartsWith(macroNameVal))
                .OrderBy(inpMacro => inpMacro.MacroName.Length).FirstOrDefault();

                //Console.WriteLine($"Count for starting: {longestMacro.Count()}");

                //Macro not found
                if (longestMacro == null)
                {
                    Console.WriteLine($"Macro with name \"{macroNameVal}\" not found!");
                    continue;
                }

                Console.WriteLine($"Found longest macro named \"{longestMacro.MacroName}\" with value \"{longestMacro.MacroValue}\"!");
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
    }
}