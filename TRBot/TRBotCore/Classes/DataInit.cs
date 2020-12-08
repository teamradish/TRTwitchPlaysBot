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
using System.Text;

namespace TRBot
{
    /// <summary>
    /// Handles initializing various parts of data.
    /// </summary>
    public static class DataInit
    {
        public static void PopulateMacrosToParserList(ConcurrentDictionary<string, string> macros, Dictionary<char, List<string>> parserMacroLookup)
        {
            parserMacroLookup.Clear();

            //Add all macros in the data to the parser list on initialization
            foreach (var macroName in macros.Keys)
            {
                AddMacroToParserList(macroName, parserMacroLookup);
            }
        }

        public static void AddMacroToParserList(string macroName, Dictionary<char, List<string>> parserMacroLookup)
        {
            char macroFirstChar = macroName[1];

            //Add to the parsed macro list for quicker lookup
            if (parserMacroLookup.TryGetValue(macroFirstChar, out List<string> macroList) == false)
            {
                macroList = new List<string>(16);
                parserMacroLookup.Add(macroFirstChar, macroList);
            }

            macroList.Add(macroName);
        }

        public static void RemoveMacroFromParserList(string macroName, Dictionary<char, List<string>> parserMacroLookup)
        {
            char macroFirstChar = macroName[1];

            //Remove from the parser macro list
            List<string> parserMacroList = parserMacroLookup[macroFirstChar];
            parserMacroList.Remove(macroName);
            if (parserMacroList.Count == 0)
            {
                parserMacroLookup.Remove(macroFirstChar);
            }
        }
    }
}
