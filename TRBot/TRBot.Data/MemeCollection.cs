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

namespace TRBot.Data
{
    /// <summary>
    /// A collection of memes.
    /// </summary>
    public sealed class MemeCollection
    {
        /// <summary>
        /// The collection of memes available.
        /// </summary>
        public ConcurrentDictionary<string, Meme> Memes = new ConcurrentDictionary<string, Meme>();

        public MemeCollection()
        {
            
        }

        /// <summary>
        /// Sets the meme data.
        /// </summary>
        /// <param name="memes">The memes to set.</param>
        public void SetMemeData(ConcurrentDictionary<string, Meme> memes)
        {
            Memes = memes;
        }

        /// <summary>
        /// Adds a meme to the collection.
        /// </summary>
        /// <param name="meme">The meme to add.</param>
        public void AddMeme(in Meme meme)
        {
            Memes[meme.MemeName] = meme;
        }

        /// <summary>
        /// Removes a meme from the collection.
        /// </summary>
        /// <returns>true if the meme was successfully removed, otherwise false.</returns>
        public bool RemoveMeme(in Meme meme)
        {
            return RemoveMeme(meme.MemeName);
        }

        /// <summary>
        /// Removes a meme from the collection.
        /// </summary>
        /// <param name="memeName">The name of the meme to remove.</param>
        /// <returns>true if the meme was successfully removed, otherwise false.</returns>
        public bool RemoveMeme(string memeName)
        {
            if (Memes.ContainsKey(memeName) == false)
            {
                return false;
            }

            if (Memes.TryRemove(memeName, out Meme meme) == false)
            {
                return false;
            }

            return true;
        }
    }
}
