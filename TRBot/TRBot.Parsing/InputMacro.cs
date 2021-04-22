﻿/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
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
using System.Text;

namespace TRBot.Parsing
{
    /// <summary>
    /// Represents an input macro.
    /// </summary>
    public class InputMacro
    {
        /// <summary>
        /// The ID of the macro.
        /// </summary>
        public int ID { get; set; } = 0;

        /// <summary>
        /// The name of the macro.
        /// </summary>
        public string MacroName { get; set; } = string.Empty;
        
        /// <summary>
        /// The value of the macro.
        /// </summary>
        public string MacroValue { get; set; } = string.Empty;

        public InputMacro()
        {

        }

        public InputMacro(string macroName, string macroValue)
        {
            MacroName = macroName;
            MacroValue = macroValue;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 11;
                hash = (hash * 37) + MacroName.GetHashCode();
                hash = (hash * 37) + MacroValue.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return $"\"{MacroName}\" = \"{MacroValue}\"";
        }
    }
}
