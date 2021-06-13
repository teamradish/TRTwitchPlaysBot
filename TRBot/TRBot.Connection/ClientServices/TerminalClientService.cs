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
using TRBot.Logging;

namespace TRBot.Connection
{
    /// <summary>
    /// Read inputs through a terminal.
    /// </summary>
    public class TerminalClientService : IClientService
    {
        /// <summary>
        /// The event handler associated with the service.
        /// </summary>
        public IEventHandler EventHandler { get; private set; } = null;

        /// <summary>
        /// Tells if the client is initialized.
        /// </summary>
        public bool IsInitialized => Initialized;

        /// <summary>
        /// Tells if the client is connected.
        /// </summary>
        public bool IsConnected => Connected;

        /// <summary>
        /// Whether the client is able to send messages.
        /// </summary>
        public bool CanSendMessages => Connected;

        private bool Initialized = false;
        private bool Connected = false;

        private char CommandIdentifier = '!';

        public TerminalClientService(char commandIdentifier)
        {
            CommandIdentifier = commandIdentifier;
        }

        /// <summary>
        /// Initializes the client.
        /// </summary>
        public void Initialize()
        {
            EventHandler = new TerminalEventHandler(CommandIdentifier);

            Initialized = true;
        }

        /// <summary>
        /// Connects the client.
        /// </summary>
        public void Connect()
        {
            EventHandler.Initialize();

            Connected = true;
        }

        /// <summary>
        /// Disconnects the client.
        /// </summary>
        public void Disconnect()
        {
            Connected = false;
        }

        /// <summary>
        /// Reconnects the client.
        /// </summary>
        public void Reconnect()
        {
            
        }

        /// <summary>
        /// Send a message through the client.
        /// </summary>
        public void SendMessage(string channel, string message)
        {
            TRBotLogger.Logger.Information(message);
        }

        /// <summary>
        /// Cleans up the client.
        /// </summary>
        public void CleanUp()
        {
            EventHandler.CleanUp();
        }
    }
}
