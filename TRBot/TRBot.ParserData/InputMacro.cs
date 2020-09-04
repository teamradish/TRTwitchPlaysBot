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

namespace TRBot.ParserData
{
    /// <summary>
    /// Represents an input macro.
    /// </summary>
    public struct InputMacro
    {
        /// <summary>
        /// The name of the macro.
        /// </summary>
        public string MacroName;
        
        /// <summary>
        /// The value of the macro.
        /// </summary>
        public string MacroValue;

        public InputMacro(string macroName, string macroValue)
        {
            MacroName = macroName;
            MacroValue = macroValue;
        }

        public override bool Equals(object obj)
        {
            if (obj is InputMacro inpMacro)
            {
                return (this == inpMacro);
            }
            return false;
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

        public static bool operator ==(InputMacro a, InputMacro b)
        {
            return (a.MacroName == b.MacroName && a.MacroValue == b.MacroValue);
        }
        
        public static bool operator !=(InputMacro a, InputMacro b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return $"\"{MacroName}\" = \"{MacroValue}\"";
        }
    }
}
