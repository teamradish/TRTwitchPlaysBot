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

namespace TRBot.Parsing
{
    /// <summary>
    /// Represents an input synonym.
    /// </summary>
    public class InputSynonym
    {
        /// <summary>
        /// The ID of the synonym.
        /// </summary>
        public int ID { get; set; } = 0;

        /// <summary>
        /// The console ID of the synonym.
        /// </summary>
        public int ConsoleID { get; set; } = 0;

        /// <summary>
        /// The name of the synonym.
        /// </summary>
        public string SynonymName { get; set; } = string.Empty;
        
        /// <summary>
        /// The value of the synonym.
        /// </summary>
        public string SynonymValue { get; set; } = string.Empty;

        public InputSynonym()
        {

        }

        public InputSynonym(string synonymName, string synonymValue)
        {
            SynonymName = synonymName;
            SynonymValue = synonymValue;
        }

        public InputSynonym(in int consoleID, string synonymName, string synonymValue)
            : this(synonymName, synonymValue)
        {
            ConsoleID = consoleID;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 29;
                hash = (hash * 37) + SynonymName.GetHashCode();
                hash = (hash * 37) + SynonymValue.GetHashCode();
                hash = (hash * 37) + ConsoleID.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return $"\"{SynonymName}\":\"{SynonymValue}\"";
        }
    }
}
