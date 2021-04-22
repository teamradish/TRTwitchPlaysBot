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
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;

namespace TRBot.Data
{
    /// <summary>
    /// Represents a meme.
    /// </summary>
    public class Meme
    {
        /// <summary>
        /// The ID of the meme.
        /// </summary>
        public int ID { get; set; } = 0;

        /// <summary>
        /// The name of the meme.
        /// </summary>
        public string MemeName { get; set; } = string.Empty;

        /// <summary>
        /// The value of the meme.
        /// </summary>
        public string MemeValue { get; set; } = string.Empty;

        public Meme()
        {

        }

        public Meme(string memeName, string memeValue)
        {
            MemeName = memeName;
            MemeValue = memeValue;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 29;
                hash = (hash * 41) + MemeName.GetHashCode();
                hash = (hash * 41) + MemeValue.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return $"\"{MemeName}\":\"{MemeValue}\"";
        }
    }

    /*/// <summary>
    /// Represents a meme.
    /// </summary>
    public struct Meme
    {
        /// <summary>
        /// The name of the meme.
        /// </summary>
        public string MemeName;
        
        /// <summary>
        /// The value of the meme.
        /// </summary>
        public string MemeValue;

        public Meme(string memeName, string memeValue)
        {
            MemeName = memeName;
            MemeValue = memeValue;
        }

        public override bool Equals(object obj)
        {
            if (obj is Meme meme)
            {
                return (this == meme);
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 29;
                hash = (hash * 41) + MemeName.GetHashCode();
                hash = (hash * 41) + MemeValue.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(Meme a, Meme b)
        {
            return (a.MemeName == b.MemeName && a.MemeValue == b.MemeValue);
        }
        
        public static bool operator !=(Meme a, Meme b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return $"\"{MemeName}\":\"{MemeValue}\"";
        }
    }*/
}
