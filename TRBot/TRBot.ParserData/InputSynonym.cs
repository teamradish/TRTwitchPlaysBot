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
    /// Represents an input synonym.
    /// </summary>
    public struct InputSynonym
    {
        /// <summary>
        /// The name of the synonym.
        /// </summary>
        public string SynonymName;
        
        /// <summary>
        /// The value of the synonym.
        /// </summary>
        public string SynonymValue;

        public InputSynonym(string synonymName, string synonymValue)
        {
            SynonymName = synonymName;
            SynonymValue = synonymValue;
        }

        public override bool Equals(object obj)
        {
            if (obj is InputSynonym inputSynonym)
            {
                return (this == inputSynonym);
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 29;
                hash = (hash * 37) + SynonymName.GetHashCode();
                hash = (hash * 37) + SynonymValue.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(InputSynonym a, InputSynonym b)
        {
            return (a.SynonymName == b.SynonymName && a.SynonymValue == b.SynonymValue);
        }
        
        public static bool operator !=(InputSynonym a, InputSynonym b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return $"\"{SynonymName}\":\"{SynonymValue}\"";
        }
    }
}
