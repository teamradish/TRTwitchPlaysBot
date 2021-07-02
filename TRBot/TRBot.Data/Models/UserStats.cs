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
using System.Text;
using Newtonsoft.Json;

namespace TRBot.Data
{
    /// <summary>
    /// Represents user stats.
    /// </summary>
    public class UserStats
    {
        public int ID { get; set; } = 0;
        public int UserID { get; set; } = 0;
        
        public long Credits { get; set; } = 0;
        public long TotalMessageCount { get; set; } = 0;
        public long ValidInputCount { get; set; } = 0;
        public long IsSubscriber { get; set; } = 0;
        public long BetCounter { get; set; } = 0;
        public long AutoPromoted { get; set; } = 0;
        public long OptedOut { get; set; } = 0;
        public long IgnoreMemes { get; set; } = 0;
        public string SimulateHistory { get; set; } = string.Empty;
        public long OptedInSimulate { get; set; } = 0;

        /// <summary>
        /// The User associated with the user stats.
        /// This is used by the database and should not be assigned or modified manually. 
        /// </summary>
        public virtual User user { get; set; } = null;

        public UserStats()
        {

        }
    }
}
