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
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using TRBot.Logging;
using WebSocketSharp;

namespace TRBot.Connection.WebSocket
{
    /// <summary>
    /// Read inputs through a WebSocket.
    /// </summary>
    public class WebSocketClientService : IClientService
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
        public bool IsConnected => (Socket != null && Socket.ReadyState == WebSocketState.Open && Socket.IsAlive == true);

        /// <summary>
        /// Whether the client is able to send messages.
        /// </summary>
        public bool CanSendMessages => (IsConnected == true && JoinedChannels?.Count > 0);

        /// <summary>
        /// The channels the client has joined.
        /// </summary>
        public List<string> JoinedChannels { get; private set; } = new List<string>(8);

        private bool Initialized = false;

        private char CommandIdentifier = '!';
        private string ConnectURL = string.Empty;
        private string BotName = string.Empty;

        private WebSocketSharp.WebSocket Socket = null;

        public WebSocketClientService(string connectURL, char commandIdentifier, string botName)
        {
            ConnectURL = connectURL;
            CommandIdentifier = commandIdentifier;
            BotName = botName;
        }

        /// <summary>
        /// Initializes the client.
        /// </summary>
        public void Initialize()
        {
            Socket = new WebSocketSharp.WebSocket(ConnectURL);

            //Specify SSL configuration on a secure WebSocket
            if (Socket.IsSecure == true)
            {
                Socket.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls12;

                //Subscribe to validate SSL certificates for secure websockets
                ServicePointManager.ServerCertificateValidationCallback -= WebSocketSSLCertValidation;
                ServicePointManager.ServerCertificateValidationCallback += WebSocketSSLCertValidation;
            }

            EventHandler = new WebSocketEventHandler(Socket, CommandIdentifier, BotName);
            EventHandler.Initialize();

            EventHandler.OnJoinedChannelEvent -= OnClientJoinedChannel;
            EventHandler.OnJoinedChannelEvent += OnClientJoinedChannel;

            Initialized = true;
        }

        /// <summary>
        /// Connects the client.
        /// </summary>
        public void Connect()
        {
            if (IsConnected == true)
            {
                TRBotLogger.Logger.Warning("Attempting to connect while already connected!");
                return;
            }

            Socket.Connect();
        }

        /// <summary>
        /// Disconnects the client.
        /// </summary>
        public void Disconnect()
        {
            if (IsConnected == false)
            {
                TRBotLogger.Logger.Warning("Attempting to disconnect while not connected!");
                return;
            }

            Socket.Close(CloseStatusCode.Normal);
            JoinedChannels?.Clear();
        }

        /// <summary>
        /// Reconnects the client.
        /// </summary>
        public void Reconnect()
        {
            if (IsConnected == false)
            {
                TRBotLogger.Logger.Warning("Attempting to reconnect while not connected!");
                return;
            }

            Socket.Connect();
        }

        /// <summary>
        /// Send a message through the client.
        /// </summary>
        public void SendMessage(string channel, string message)
        {
            Socket.Send(message);
        }

        /// <summary>
        /// Cleans up the client.
        /// </summary>
        public void CleanUp()
        {
            if (IsConnected == true)
            {
                Socket.Close(CloseStatusCode.Normal);
            }

            JoinedChannels = null;

            EventHandler.OnJoinedChannelEvent -= OnClientJoinedChannel;

            EventHandler.CleanUp();

            ServicePointManager.ServerCertificateValidationCallback -= WebSocketSSLCertValidation;
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

        private bool WebSocketSSLCertValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //Since this can connect to any server, simply return true if there are no SSL errors
            return sslPolicyErrors == SslPolicyErrors.None;

            //If you intend to use this with a specific set of servers, modify this
            //There are many ways to do this - one option is to validate against a set of certificate hashes
            //that can either be hardcoded or read from disk
        }
    }
}
