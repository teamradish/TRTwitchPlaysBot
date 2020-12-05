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
using System.Threading;
using System.Threading.Tasks;
using Matrix;
using Matrix.Client;
using Matrix.Structures;

namespace TRBot.Connection
{
    /// <summary>
    /// Matrix client interaction.
    /// </summary>
    public class MatrixClientService : IClientService
    {
        private MatrixClient matrixClient = null;
        private MatrixConnectionSettings ConnectionSettings = null;

        /// <summary>
        /// The event handler associated with the service.
        /// </summary>
        public IEventHandler EventHandler { get; private set; } = null;

        /// <summary>
        /// Tells if the client is initialized.
        /// </summary>
        public bool IsInitialized => (matrixClient != null && matrixClient.Api != null);

        /// <summary>
        /// Tells the client's operation type.
        /// </summary>
        public OperationTypes OperationType => OperationTypes.Online;

        /// <summary>
        /// Tells if the client is connected.
        /// </summary>
        public bool IsConnected => (matrixClient != null && matrixClient.Api != null && matrixClient.Api.IsConnected);

        /// <summary>
        /// The channels the client has joined.
        /// </summary>
        public List<string> JoinedChannels { get; private set; } = new List<string>(8);

        public MatrixClientService(MatrixConnectionSettings connectionSettings)
        {
            ConnectionSettings = connectionSettings;
            matrixClient = new MatrixClient(connectionSettings.HomeServerURL);
        }

        /// <summary>
        /// Initializes the client.
        /// </summary>
        public void Initialize()
        {
            
        }

        /// <summary>
        /// Connects the client.
        /// </summary>
        public void Connect()
        {
            try
            {
                MatrixLoginResponse loginRes = matrixClient.LoginWithPassword(ConnectionSettings.Username,
                    ConnectionSettings.Password);

                Console.WriteLine("Starting sync to get updated room information.");
                
                matrixClient.StartSync();

                Console.WriteLine("Finished initial sync");
                
                foreach (var roomval in matrixClient.GetAllRooms())
                {
                    Console.WriteLine($"Found room: {roomval.ID}");
                }

                MatrixRoom room = matrixClient.GetRoom(ConnectionSettings.RoomID);
                JoinedChannels.Add(room.ID);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error connecting to Matrix: {e.Message}");
            }
        }

        /// <summary>
        /// Disconnects the client.
        /// </summary>
        public void Disconnect()
        {
            if (IsConnected == false)
            {
                Console.WriteLine("Attempting to disconnect while not connected!");
                return;
            }

            matrixClient.Api.StopSyncThreads();

            if (JoinedChannels?.Count > 0)
            {
                matrixClient.Api.RoomLeave(JoinedChannels[0]);
            }
            
            JoinedChannels?.Clear();
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
            MMessageText messageObj = new MMessageText();
            messageObj.body = message;

            Task<string> messageSend = matrixClient.Api.RoomMessageSend(channel,
                messageObj.msgtype, messageObj);

            messageSend.Wait();

            Console.WriteLine($"Message completed: {messageSend.Result}");
        }

        /// <summary>
        /// Cleans up the client.
        /// </summary>
        public void CleanUp()
        {
            matrixClient?.Dispose();
            matrixClient = null;
            
            JoinedChannels = null;

            EventHandler.CleanUp();
        }
    }
}