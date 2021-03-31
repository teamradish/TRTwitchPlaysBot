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
using System.IO;

namespace TRBot.Data
{
    /// <summary>
    /// Constants regarding general data.
    /// </summary>
    public static class DataConstants
    {
        /// <summary>
        /// The name of the data folder.
        /// </summary>
        public const string DATA_FOLDER_NAME = "Data";

        /// <summary>
        /// The name of the database file.
        /// </summary>
        public const string DATABASE_FILE_NAME = "TRBotData.db";

        /// <summary>
        /// The identifier character for commands.
        /// </summary>
        public const char COMMAND_IDENTIFIER = '!';

        /// <summary>
        /// The path to the data folder.
        /// </summary>
        public static readonly string DataFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DATA_FOLDER_NAME);
    }
}
