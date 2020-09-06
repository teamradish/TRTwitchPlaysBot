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
using Newtonsoft.Json;

namespace TRBot.ParserData
{
    /// <summary>
    /// Collection for input macros.
    /// </summary>
    public sealed class InputMacroCollection
    {
        /// <summary>
        /// The collection of macros available.
        /// </summary>
        public ConcurrentDictionary<string, InputMacro> Macros = null;

        /// <summary>
        /// This dictionary is used to improve the speed of macro lookups for the parser, which needs to iterate through them since there are no spaces.
        /// This is populated based on the macro data and should not be saved to disk.
        /// </summary>
        [JsonIgnore]
        public Dictionary<char, List<InputMacro>> ParserMacroLookup = null;

        public InputMacroCollection()
        {
            
        }

        public InputMacroCollection(ConcurrentDictionary<string, InputMacro> macros)
        {
            SetMacroData(macros);
        }

        /// <summary>
        /// Sets the macro data.
        /// </summary>
        /// <param name="macros">The input macros to set.</param>
        public void SetMacroData(ConcurrentDictionary<string, InputMacro> macros)
        {
            Macros = macros;
            UpdateMacroLookupDict();
        }

        /// <summary>
        /// Updates the macro lookup dictionary.
        /// </summary>
        public void UpdateMacroLookupDict()
        {
            ValidateMacroLookupDict();

            ParserMacroLookup.Clear();

            foreach (KeyValuePair<string, InputMacro> macro in Macros)
            {
                AddMacroToParserList(macro.Value);
            }
        }

        /// <summary>
        /// Adds a macro to the collection.
        /// </summary>
        /// <param name="inputMacro">The macro to add.</param>
        public void AddMacro(in InputMacro inputMacro)
        {
            Macros[inputMacro.MacroName] = inputMacro;

            AddMacroToParserList(inputMacro);
        }

        /// <summary>
        /// Removes a macro from the collection.
        /// </summary>
        /// <returns>true if the macro was successfully removed, otherwise false.</returns>
        public bool RemoveMacro(in InputMacro inputMacro)
        {
            return RemoveMacro(inputMacro.MacroName);
        }

        /// <summary>
        /// Removes a macro from the collection.
        /// </summary>
        /// <param name="macroName">The name of the macro to remove.</param>
        /// <returns>true if the macro was successfully removed, otherwise false.</returns>
        public bool RemoveMacro(string macroName)
        {
            if (Macros.ContainsKey(macroName) == false)
            {
                return false;
            }

            if (Macros.TryRemove(macroName, out InputMacro macroValue) == false)
            {
                return false;
            }

            RemoveMacroFromParserList(macroName);

            return true;
        }

        /// <summary>
        /// Adds a macro to the quick macro lookup list.
        /// </summary>
        /// <param name="macroName">The name of the macro to add.</param>
        public void AddMacroToParserList(in InputMacro inputMacro)
        {
            ValidateMacroLookupDict();

            char macroFirstChar = inputMacro.MacroName[1];

            //Add to the parsed macro list for quicker lookup
            if (ParserMacroLookup.TryGetValue(macroFirstChar, out List<InputMacro> macroList) == false)
            {
                macroList = new List<InputMacro>(16);
                ParserMacroLookup.Add(macroFirstChar, macroList);
            }

            macroList.Add(inputMacro);
        }

        /// <summary>
        /// Removes a macro from the quick macro lookup list.
        /// </summary>
        /// <param name="inputMacro">The macro to remove.</param>
        public void RemoveMacroFromParserList(in InputMacro inputMacro)
        {
            RemoveMacroFromParserList(inputMacro.MacroName);
        }

        /// <summary>
        /// Removes a macro from the quick macro lookup list.
        /// </summary>
        /// <param name="macroName">The name of the macro to remove.</param>
        public void RemoveMacroFromParserList(string macroName)
        {
            ValidateMacroLookupDict();

            char macroFirstChar = macroName[1];

            //Remove from the parser macro list
            List<InputMacro> parserMacroList = ParserMacroLookup[macroFirstChar];

            //Find and remove the macro
            for (int i = parserMacroList.Count - 1; i >= 0; i--)
            {
                InputMacro macro = parserMacroList[i];
                if (macro.MacroName == macroName)
                {
                    parserMacroList.RemoveAt(i);
                    break;
                }
            }
            
            //Remove if none are remaining
            if (parserMacroList.Count == 0)
            {
                ParserMacroLookup.Remove(macroFirstChar);
            }
        }

        private void ValidateMacroLookupDict()
        {
            if (ParserMacroLookup == null)
            {
                ParserMacroLookup = new Dictionary<char, List<InputMacro>>(Macros.Count);
            }
        }
    }
}
