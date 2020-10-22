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
    /// Command data.
    /// </summary>
    public class CommandData
    {
        /// <summary>
        /// The command's ID.
        /// </summary>
        public int id { get; set; } = 0;

        /// <summary>
        /// The name of the command.
        /// </summary>
        public string name { get; set; } = string.Empty;

        /// <summary>
        /// The class name associated with the command.
        /// </summary>
        public string class_name { get; set; } = string.Empty;

        /// <summary>
        /// The access level of the command.
        /// </summary>
        public long level { get; set; } = 0;

        /// <summary>
        /// Whether the command is enabled or not.
        /// </summary>
        public long enabled { get; set; } = 1;

        /// <summary>
        /// Whether to display the command in the command list.
        /// If the command is disabled, it won't be displayed regardless.
        /// </summary>
        public long display_in_list { get; set; } = 1;

        /// <summary>
        /// An additional value for the command.
        /// </summary>
        public string value_str { get; set; } = string.Empty;

        public CommandData()
        {

        }

        public CommandData(string cmdName, string className, in long lvl, in bool cmdEnabled, in bool displayInList)
        {
            name = cmdName;
            class_name = className;
            level = lvl;
            enabled = cmdEnabled == true ? 1 : 0;
            display_in_list = displayInList == true ? 1 : 0;
        }

        public CommandData(string cmdName, string className, in long lvl, in bool cmdEnabled, in bool displayInList,
            string valueStr)
            : this(cmdName, className, lvl, cmdEnabled, displayInList)
        {
            value_str = valueStr;
        }
    }
}
