﻿/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
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
    /// Constants regarding WebSockets.
    /// </summary>
    public static class WebSocketConstants
    {
        /// <summary>
        /// The file storing WebSocket connection settings.
        /// </summary>
        public const string CONNECTION_SETTINGS_FILENAME = "WebSocketConnectionSettings.txt";

        /// <summary>
        /// The protocol string for WebSocket connections.
        /// </summary>
        public const string WEBSOCKET_PROTOCOL = @"ws://";

        /// <summary>
        /// The protocol string for secure WebSocket connections.
        /// </summary>
        public const string WEBSOCKET_SECURE_PROTOCOL = @"wss://";
    }
}
