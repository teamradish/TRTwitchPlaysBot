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
    /// Collection for input synonyms.
    /// </summary>
    public sealed class InputSynonymCollection
    {
        /// <summary>
        /// The collection of synonyms available.
        /// </summary>
        public ConcurrentDictionary<string, InputSynonym> Synonyms = null;

        public InputSynonymCollection()
        {
            
        }

        public InputSynonymCollection(ConcurrentDictionary<string, InputSynonym> synonyms)
        {
            SetSynonymData(synonyms);
        }

        /// <summary>
        /// Sets the synonym data.
        /// </summary>
        /// <param name="synonyms">The input synonyms to set.</param>
        public void SetSynonymData(ConcurrentDictionary<string, InputSynonym> synonyms)
        {
            Synonyms = synonyms;
        }

        /// <summary>
        /// Adds a synonym to the collection.
        /// </summary>
        /// <param name="inputSynonym">The synonym to add.</param>
        public void AddSynonym(in InputSynonym inputSynonym)
        {
            Synonyms[inputSynonym.SynonymName] = inputSynonym;
        }

        /// <summary>
        /// Removes a synonym from the collection.
        /// </summary>
        /// <returns>true if the synonym was successfully removed, otherwise false.</returns>
        public bool RemoveSynonym(in InputSynonym inputSynonym)
        {
            return RemoveSynonym(inputSynonym.SynonymName);
        }

        /// <summary>
        /// Removes a synonym from the collection.
        /// </summary>
        /// <param name="synonymName">The name of the synonym to remove.</param>
        /// <returns>true if the synonym was successfully removed, otherwise false.</returns>
        public bool RemoveSynonym(string synonymName)
        {
            if (Synonyms.ContainsKey(synonymName) == false)
            {
                return false;
            }

            if (Synonyms.TryRemove(synonymName, out InputSynonym synonymValue) == false)
            {
                return false;
            }

            return true;
        }
    }
}
