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
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace TRBot.Connection
{
    /// <summary>
    /// Twitch client interaction.
    /// </summary>
    public class TwitchClientService : IClientService
    {
        private TwitchClient twitchClient = null;

        private ConnectionCredentials Credentials = null;
        private string ChannelName = string.Empty;
        private char ChatCommandIdentifier = '!';
        private char WhisperCommandIdentifier = '!';
        private bool AutoRelistenOnExceptions = true;

        /// <summary>
        /// The event handler associated with the service.
        /// </summary>
        public IEventHandler EventHandler { get; private set; } = null;

        /// <summary>
        /// Tells if the client is initialized.
        /// </summary>
        public bool IsInitialized => (twitchClient?.IsInitialized == true);

        /// <summary>
        /// Tells if the client is connected.
        /// </summary>
        public bool IsConnected => (twitchClient?.IsConnected == true);

        /// <summary>
        /// The channels the client has joined.
        /// </summary>
        public List<string> JoinedChannels { get; private set; } = new List<string>(8);

        public TwitchClientService(ConnectionCredentials credentials, string channelName, in char chatCommandIdentifier,
            in char whisperCommandIdentifier, in bool autoRelistenOnExceptions)
        {
            twitchClient = new TwitchClient();

            Credentials = credentials;
            ChannelName = channelName;
            ChatCommandIdentifier = chatCommandIdentifier;
            WhisperCommandIdentifier = whisperCommandIdentifier;
            AutoRelistenOnExceptions = autoRelistenOnExceptions;
        }

        /// <summary>
        /// Initializes the client.
        /// </summary>
        public void Initialize()
        {
            twitchClient.Initialize(Credentials, ChannelName, ChatCommandIdentifier,
                WhisperCommandIdentifier, AutoRelistenOnExceptions);
            twitchClient.OverrideBeingHostedCheck = true;

            EventHandler = new TwitchEventHandler(twitchClient);
            EventHandler.Initialize();

            EventHandler.OnJoinedChannelEvent -= OnClientJoinedChannel;
            EventHandler.OnJoinedChannelEvent += OnClientJoinedChannel;
        }

        /// <summary>
        /// Connects the client.
        /// </summary>
        public void Connect()
        {
            if (twitchClient.IsConnected == true)
            {
                Console.WriteLine("Attempting to connect while already connected!");
                return;
            }
            
            twitchClient.Connect();
        }

        /// <summary>
        /// Disconnects the client.
        /// </summary>
        public void Disconnect()
        {
            if (twitchClient.IsConnected == false)
            {
                Console.WriteLine("Attempting to disconnect while not connected!");
                return;
            }

            twitchClient.Disconnect();
            JoinedChannels?.Clear();
        }

        /// <summary>
        /// Reconnects the client.
        /// </summary>
        public void Reconnect()
        {
            if (twitchClient.IsConnected == false)
            {
                Console.WriteLine("Attempting to reconnect while not connected!");
                return;
            }

            twitchClient.Reconnect();
        }

        /// <summary>
        /// Send a message through the client.
        /// </summary>
        public void SendMessage(string channel, string message)
        {
            if (twitchClient.IsConnected == true && JoinedChannels?.Count > 0)
            {
                twitchClient.SendMessage(channel, message);
            }
        }

        /// <summary>
        /// Cleans up the client.
        /// </summary>
        public void CleanUp()
        {
            if (twitchClient.IsConnected == true)
                twitchClient.Disconnect();
            
            JoinedChannels = null;

            EventHandler.OnJoinedChannelEvent -= OnClientJoinedChannel;

            EventHandler.CleanUp();
        }

        private void OnClientJoinedChannel(EvtJoinedChannelArgs e)
        {
            //When joining a channel, set the joined channels list
            IReadOnlyList<JoinedChannel> twitchJoinedChannels = twitchClient.JoinedChannels;

            //Instantiate if needed
            if (JoinedChannels == null)
            {
                JoinedChannels = new List<string>(twitchJoinedChannels.Count);
            }
            
            JoinedChannels.Clear();

            foreach (JoinedChannel channel in twitchJoinedChannels)
            {
                JoinedChannels.Add(channel.Channel);
            }
        }
    }
}