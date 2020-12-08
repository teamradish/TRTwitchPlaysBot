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
using System.Data.SQLite;
using Microsoft.EntityFrameworkCore;

namespace TRBot.Data
{
    /// <summary>
    /// Represents bot settings.
    /// </summary>
    public class Settings
    {
        public int ID { get; set; } = 0;
        public string Key { get; set; } = string.Empty;
        public string ValueStr { get; set; } = string.Empty;
        public long ValueInt { get; set; } = 0L;

        public bool GetBool => (ValueInt == 0L) ? false : true;

        public Settings()
        {

        }

        public Settings(string Key, string Value_str, in long Value_int)
        {
            this.Key = Key;
            ValueStr = Value_str;
            ValueInt = Value_int;
        }

        public Settings(in int Id, string Key, string Value_str, in long Value_int)
        {
            ID = Id;
            this.Key = Key;
            ValueStr = Value_str;
            ValueInt = Value_int;
        }
    }
}