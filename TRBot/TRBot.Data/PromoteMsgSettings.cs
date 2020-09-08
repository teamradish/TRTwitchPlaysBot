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
using System.Text;

namespace TRBot.Data
{
    /// <summary>
    /// Settings for promoting users based on message count.
    /// </summary>
    public sealed class PromoteMsgSettings
    {
        /// <summary>
        /// If true, automatically promotes users to new levels if conditions are met.
        /// </summary>
        public bool AutoPromoteEnabled = false;

        /// <summary>
        /// The level to promote users to upon meeting promotion conditions.
        /// </summary>
        public int AutoPromoteLevel = 1;

        /// <summary>
        /// The number of valid inputs required to promote a user if they're not promoted and auto promote is enabled.
        /// </summary>
        public int AutoPromoteInputCount = 20;
    }
}
