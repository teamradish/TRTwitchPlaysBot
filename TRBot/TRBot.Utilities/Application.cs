/* Copyright (C) 2019-2020 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot,software for playing games through text.
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

namespace TRBot.Utilities
{
    /// <summary>
    /// Helpers that deal with the general application.
    /// </summary>
    public static class Application
    {
        /// <summary>
        /// The version number of the application.
        /// </summary>
        public const string VERSION_NUMBER = "2.1.1";

        /// <summary>
        /// The start time of the application.
        /// <para>To ensure this is accurate, call it at the start of the application to set the value.</para>
        /// </summary>
        public static readonly DateTime ApplicationStartTimeUTC = DateTime.UtcNow;
    }
}
