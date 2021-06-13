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

namespace TRBot.Connection.WebSocket
{
    /// <summary>
    /// Represents a WebSocket response.
    /// </summary>
    public class WebSocketResponse
    {
        /// <summary>
        /// The response user object.
        /// </summary>
        public WebSocketUserObject User = null;

        /// <summary>
        /// The response message object.
        /// </summary>
        public WebSocketMsgObject Message = null;

        public WebSocketResponse()
        {

        }
    }

    public class WebSocketUserObject
    {
        /// <summary>
        /// The name of the user.
        /// </summary>
        public string Name = string.Empty;

        public WebSocketUserObject()
        {

        }
    }

    public class WebSocketMsgObject
    {
        /// <summary>
        /// The text of the message.
        /// </summary>
        public string Text = string.Empty;

        public WebSocketMsgObject()
        {

        }
    }
}
