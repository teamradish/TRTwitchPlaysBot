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
    /// Describes a recent input sequence.
    /// </summary>
    public class RecentInput
    {
        public int ID { get; set; } = 0;

        /// <summary>
        /// The ID of the user the recent input belongs to.
        /// </summary>
        public int UserID { get; set; } = 0;

        /// <summary>
        /// The recent input sequence.
        /// </summary>
        public string InputSequence { get; set; } = string.Empty;

        /// <summary>
        /// The User associated with the recent input.
        /// This is used by the database and should not be assigned or modified manually.
        /// </summary>
        public virtual User user { get; set; } = null;

        public RecentInput()
        {
            
        }

        public RecentInput(string inputSequence)
        {
            InputSequence = inputSequence;
        }
    }
}