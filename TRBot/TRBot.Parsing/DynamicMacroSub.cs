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
using System.Collections;
using System.Collections.Generic;

namespace TRBot.Parsing
{
    /// <summary>
    /// Represents a dynamic macro substitution.
    /// </summary>
    public struct DynamicMacroSub
    {
        public string MacroName;
        public int StartIndex;
        public int EndIndex;
        public List<string> Variables;
        
        public DynamicMacroSub(string macroName, in int startIndex, in int endIndex, List<string> variables)
        {
            MacroName = macroName;
            StartIndex = startIndex;
            EndIndex = endIndex;
            Variables = variables;
        }

        public override bool Equals(object obj)
        {
            if (obj is DynamicMacroSub dynamicMacroSub)
            {
                return (this == dynamicMacroSub);
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 37) + ((MacroName == null) ? 0 : MacroName.GetHashCode());
                hash = (hash * 37) + StartIndex.GetHashCode();
                hash = (hash * 37) + EndIndex.GetHashCode();
                hash = (hash * 37) + ((Variables == null) ? 0 : Variables.GetHashCode());
                return hash;
            }
        }

        public static bool operator ==(DynamicMacroSub a, DynamicMacroSub b)
        {
            return (a.MacroName == b.MacroName && a.StartIndex == b.StartIndex
                    && a.EndIndex == b.EndIndex && a.Variables == b.Variables);
        }
        
        public static bool operator !=(DynamicMacroSub a, DynamicMacroSub b)
        {
            return !(a == b);
        }
    }
}