/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRBot.Misc
{
    /// <summary>
    /// The types of message throttling.
    /// </summary>
    public enum MessageThrottlingOptions
    {
        /// <summary>
        /// No throttling.
        /// </summary>
        None = 0,

        /// <summary>
        /// Only one message will be allowed within a specified period of time.
        /// </summary>
        TimeThrottled = 1,

        /// <summary>
        /// A specified number of messages will be allowed within a specified period of time.
        /// </summary>
        MsgCountPerInterval = 2,
    }
}
