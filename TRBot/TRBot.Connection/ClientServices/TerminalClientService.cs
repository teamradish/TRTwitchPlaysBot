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
        /// Tells the client's operation type.
        /// </summary>
        public OperationTypes OperationType => OperationTypes.Offline;

        /// <summary>
        /// Tells if the client is connected.
        /// </summary>
        public bool IsConnected => Connected;

        /// <summary>
        /// The channels the client has joined.
        /// </summary>
        public List<string> JoinedChannels { get; private set; } = new List<string>(8);

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

            EventHandler.OnJoinedChannelEvent -= OnClientJoinedChannel;
            EventHandler.OnJoinedChannelEvent += OnClientJoinedChannel;

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
            JoinedChannels?.Clear();
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
            JoinedChannels = null;

            EventHandler.OnJoinedChannelEvent -= OnClientJoinedChannel;

            EventHandler.CleanUp();
        }

        private void OnClientJoinedChannel(EvtJoinedChannelArgs e)
        {
            //When joining a channel, set the joined channels list
            if (JoinedChannels == null)
            {
                JoinedChannels = new List<string>(1);
            }
            
            JoinedChannels.Clear();

            JoinedChannels.Add(e.Channel);
        }
    }
}
