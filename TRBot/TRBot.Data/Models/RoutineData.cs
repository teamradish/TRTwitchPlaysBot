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
using System.Text;

namespace TRBot.Data
{
    /// <summary>
    /// Routine data.
    /// </summary>
    public class RoutineData
    {
        /// <summary>
        /// The routine's ID.
        /// </summary>
        public int ID { get; set; } = 0;

        /// <summary>
        /// The name of the routine.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The class name associated with the routine.
        /// </summary>
        public string ClassName { get; set; } = string.Empty;

        /// <summary>
        /// Whether the routine is enabled or not.
        /// </summary>
        public long Enabled { get; set; } = 1L;

        /// <summary>
        /// Whether the routine resets on a data reload or not.
        /// </summary>
        public long ResetOnReload { get; set; } = 0L;

        /// <summary>
        /// An additional value for the routine.
        /// </summary>
        public string ValueStr { get; set; } = string.Empty;

        public RoutineData()
        {

        }

        public RoutineData(string routineName, string className, in bool routineEnabled, in bool resetOnReload)
        {
            Name = routineName;
            ClassName = className;
            Enabled = routineEnabled == true ? 1 : 0;
            ResetOnReload = resetOnReload == true ? 1 : 0;
        }

        public RoutineData(string routineName, string className, in bool routineEnabled, in bool resetOnReload, string valueStr)
            : this(routineName, className, routineEnabled, resetOnReload)
        {
            ValueStr = valueStr;
        }
    }
}
