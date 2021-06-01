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
            @"[^\#\(\s]+)(?<"+ MACRO_DYNAMIC_GROUP_NAME +
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

            string parsedMacroMsg = ParseMacros(message, regexOptions, 0);

            
            
            //sw.Stop();
            //Console.WriteLine($"SW MS for PopulateMacros: {sw.ElapsedMilliseconds}");

            return parsedMacroMsg;
        }

        private string ParseMacros(string message, in RegexOptions regexOptions, in int recursionDepth)
        {
            string parsedMsg = message;

            //Stop parsing if the recursion is too deep or the message is invalid
            if (recursionDepth >= MaxRecursions || string.IsNullOrEmpty(parsedMsg) == true)
            {
                return parsedMsg;
            }

            MatchCollection matches = Regex.Matches(parsedMsg, MacroRegex, regexOptions);

            //No matches, so return the original message
            if (matches.Count == 0)
            {
                Console.WriteLine($"No matches found in \"{parsedMsg}\".");
                return parsedMsg;
            }
            
            int origLength = parsedMsg.Length;
            int curLength = parsedMsg.Length;

            StringBuilder strBuilder = null;

            foreach (Match match in matches)
            {
                if (match.Groups.TryGetValue(MACRO_GROUP_NAME, out Group matchGroup) == false
                    || matchGroup.Success == false)
                {
                    Console.WriteLine($"No match with group \"{MACRO_GROUP_NAME}\" found.");
                    continue;
                }

                string macroNameMatch = matchGroup.Value;

                Console.WriteLine($"Found \"{MACRO_GROUP_NAME}\" group with value \"{macroNameMatch}\"");

                //Get the first character in the macro
                string macroStart = macroNameMatch.Substring(0, 2);

                //Find the longest macro with this name
                //Filter by macros equal or shorter in length than the picked up macro name, along with
                //macros that start with the first two characters found, using ordinal comparison for better performance
                //Sort by macro length in descending order to find longer macros first
                IQueryable<InputMacro> matchingMacros = MacroData
                .Where((m) => m.MacroName.Length <= macroNameMatch.Length && m.MacroName.StartsWith(macroStart, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(inpMacro => inpMacro.MacroName.Length);

                Console.WriteLine($"Looking for macro starting with: {macroStart}");

                InputMacro longestMacro = null;

                //Search for the longest macro that the match name starts with
                //For example, this would pick up "#hello" over "#he" in "#hello123" 
                foreach (InputMacro macro in matchingMacros)
                {
                    if (macroNameMatch.StartsWith(macro.MacroName, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        longestMacro = macro;
                        break;
                    }
                }

                //Macro not found
                if (longestMacro == null)
                {
                    Console.WriteLine($"Macro in name \"{macroNameMatch}\" not found!");
                    continue;
                }

                Console.WriteLine($"Found longest macro named \"{longestMacro.MacroName}\" with value \"{longestMacro.MacroValue}\"!");

                bool foundDynamic = match.Groups.TryGetValue(MACRO_DYNAMIC_GROUP_NAME, out Group dynamicGroup);
                bool foundDynamicArgs = match.Groups.TryGetValue(MACRO_DYNAMIC_ARGS_GROUP_NAME, out Group dynamicArgsGroup);
                
                //RETURN FOR NOW UNTIL IMPLEMENTING THIS!!!
                if (foundDynamic == true && dynamicGroup.Success == true)
                {
                    if (foundDynamicArgs == false || dynamicArgsGroup.Success == false)
                    {
                        Console.WriteLine("Found a dynamic macro but no arguments! Invalid macro!");
                        return parsedMsg;
                    }

                    Console.WriteLine("Found a dynamic macro - skipping");
                    return parsedMsg;
                }

                //Create StringBuilder if it doesn't exist - prevents allocations if we find no matches
                if (strBuilder == null)
                {
                    strBuilder = new StringBuilder(parsedMsg);
                }

                //Check for the existence of other macros within this one
                string parsedMacroVal = ParseMacros(longestMacro.MacroValue, regexOptions, recursionDepth + 1);

                int origStartIndex = match.Index;
                
                //#pressa#b = 9
                //a#b = 3
                //aaaaaaaa#b = 10

                //Adjust start index to account for replacements in the string
                int adjustedStartIndex = origStartIndex - (origLength - curLength);

                Console.WriteLine($"Replacing {longestMacro.MacroName.Length} characters in {longestMacro.MacroName} with {parsedMacroVal} starting at index {adjustedStartIndex}"); 

                strBuilder.Replace(longestMacro.MacroName, parsedMacroVal, adjustedStartIndex, longestMacro.MacroName.Length);

                Console.WriteLine($"Cur string: {strBuilder.ToString()}");

                curLength = strBuilder.Length;
            }

            if (strBuilder != null)
            {
                parsedMsg = strBuilder.ToString();
            }

            return parsedMsg;
        }

        private string PopulateVariables(string macroContents, List<string> macroVariables)
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