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
using System.Collections.Generic;
using System.Text;

namespace TRBot.Connection
{
    /// <summary>
    /// Handles client interaction with a service.
    /// </summary>
    /// <remarks>Examples of such services are Twitch and Mixer.
    /// You can also create your own service, allowing the bot to run however you wish.
    /// </remarks>
    public interface IClientService
    {
        /// <summary>
        /// The event handler associated with the service.
        /// </summary>
        IEventHandler EventHandler { get; }

        /// <summary>
        /// Tells if the client is initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Tells the client's operation type.
        /// </summary>
        OperationTypes OperationType { get; }

        /// <summary>
        /// Tells if the client is connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// The channels the client joined.
        /// </summary>
        List<string> JoinedChannels { get; }

        /// <summary>
        /// Initializes the client.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Connects the client.
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnects the client.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Reconnects the client.
        /// </summary>
        void Reconnect();

        /// <summary>
        /// Send a message through the client.
        /// </summary>
        void SendMessage(string channel, string message);

        /// <summary>
        /// Cleans up the client.
        /// </summary>
        void CleanUp();
    }
}
