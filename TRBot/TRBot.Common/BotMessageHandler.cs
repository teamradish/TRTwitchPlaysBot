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
using TRBot.Connection;

namespace TRBot.Common
{
    /// <summary>
    /// Handles messages with rate limiting.
    /// </summary>
    public class BotMessageHandler
    {
        /// <summary>
        /// Queued messages.
        /// </summary>
        private readonly Queue<string> ClientMessages = new Queue<string>(16);

        private IClientService ClientService = null;

        private string ChannelName = string.Empty;
        private double MessageCooldown = 1000d;

        /// <summary>
        /// Whether to also log bot messages to the console.
        /// </summary>
        public bool LogToConsole { get; private set; } = true;

        private DateTime CurQueueTime = default;

        public BotMessageHandler()
        {

        }

        public BotMessageHandler(IClientService clientService, string channelName, in double messageCooldown)
        {
            SetClientService(clientService);
            SetChannelName(channelName);
            SetMessageCooldown(messageCooldown);
        }

        public void CleanUp()
        {
            ClientMessages.Clear();
        }

        public void SetClientService(IClientService clientService)
        {
            ClientService = clientService;
        }

        public void SetChannelName(string channelName)
        {
            ChannelName = channelName;
        }

        public void SetMessageCooldown(in double messageCooldown)
        {
            MessageCooldown = messageCooldown;
        }

        public void SetLogToConsole(in bool logToConsole)
        {
            LogToConsole = logToConsole;
        }

        public void Update(in DateTime now)
        {
            TimeSpan queueDiff = now - CurQueueTime;

            //Queued messages
            if (ClientMessages.Count > 0 && queueDiff.TotalMilliseconds >= MessageCooldown)
            {
                //Ensure the client service has joined a channel, otherwise we can't send the message 
                if (ClientService.IsConnected == true && ClientService.JoinedChannels?.Count >= 1)
                {
                    //See the message
                    string message = ClientMessages.Peek();

                    //There's a chance the bot could be disconnected from the channel between the conditional and now
                    try
                    {
                        //Send the message
                        ClientService.SendMessage(ChannelName, message);

                        if (LogToConsole == true)
                        {
                            Console.WriteLine(message);
                        }

                        //Remove from queue
                        ClientMessages.Dequeue();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Could not send message: {e.Message}");
                    }

                    CurQueueTime = now;
                }
            }
        }

        public void QueueMessage(string message)
        {
            if (string.IsNullOrEmpty(message) == false)
            {
                ClientMessages.Enqueue(message);
            }
        }
    }
}
