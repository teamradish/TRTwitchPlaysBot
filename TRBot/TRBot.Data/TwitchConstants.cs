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
using System.Collections.Generic;
using System.Text;

namespace TRBot.Data
{
    /// <summary>
    /// Constants regarding Twitch.
    /// </summary>
    public static class TwitchConstants
    {
        /// <summary>
        /// The character limit for Twitch messages. 
        /// </summary>
        public const int MESSAGE_CHAR_LIMIT = 500;

        /// <summary>
        /// The file storing Twitch login settings.
        /// </summary>
        public const string LOGIN_SETTINGS_FILENAME = "TwitchLoginSettings.txt";
    }
}
