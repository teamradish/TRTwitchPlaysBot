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
    /// Command data.
    /// </summary>
    public class CommandData
    {
        /// <summary>
        /// The command's ID.
        /// </summary>
        public int ID { get; set; } = 0;

        /// <summary>
        /// The name of the command.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The class name associated with the command.
        /// </summary>
        public string ClassName { get; set; } = string.Empty;

        /// <summary>
        /// The access level of the command.
        /// </summary>
        public long Level { get; set; } = 0;

        /// <summary>
        /// Whether the command is enabled or not.
        /// </summary>
        public long Enabled { get; set; } = 1;

        /// <summary>
        /// Whether to display the command in the command list.
        /// If the command is disabled, it won't be displayed regardless.
        /// </summary>
        public long DisplayInList { get; set; } = 1;

        /// <summary>
        /// An additional value for the command.
        /// </summary>
        public string ValueStr { get; set; } = string.Empty;

        public CommandData()
        {

        }

        public CommandData(string cmdName, string className, in long lvl, in bool cmdEnabled, in bool displayInList)
        {
            Name = cmdName;
            ClassName = className;
            Level = lvl;
            Enabled = cmdEnabled == true ? 1 : 0;
            DisplayInList = displayInList == true ? 1 : 0;
        }

        public CommandData(string cmdName, string className, in long lvl, in bool cmdEnabled, in bool displayInList,
            string valueStr)
            : this(cmdName, className, lvl, cmdEnabled, displayInList)
        {
            ValueStr = valueStr;
        }
    }
}
