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
        public int level { get; set; } = 0;

        /// <summary>
        /// An additional value for the command.
        /// </summary>
        public string value_str { get; set; } = string.Empty;

        public CommandData()
        {

        }

        public CommandData(string cmdName, string className, in int lvl)
        {
            name = cmdName;
            class_name = className;
            level = lvl;
        }

        public CommandData(string cmdName, string className, in int lvl, string valueStr) : this(cmdName, className, lvl)
        {
            value_str = valueStr;
        }
    }
}
