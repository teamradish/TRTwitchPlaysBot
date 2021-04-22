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
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace TRBot.Integrations.LiveSplitOne
{
    /// <summary>
    /// WebSocketBehavior for LiveSplitOne.
    /// This exists only to open the connection. The command handles sending messages to clients.
    /// </summary>
    public class LiveSplitOneSocketBehavior : WebSocketBehavior
    {
        public LiveSplitOneSocketBehavior()
        {
            
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            
        }

        protected override void OnOpen()
        {
            
        }
    }
}