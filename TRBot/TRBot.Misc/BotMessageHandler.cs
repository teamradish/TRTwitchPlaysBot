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
using TRBot.Connection;
using TRBot.Utilities;

namespace TRBot.Misc
{
    /// <summary>
    /// Handles messages with rate limiting.
    /// </summary>
    public class BotMessageHandler
    {
        private static BotMessageNoThrottle NoThrottleInstance = new BotMessageNoThrottle();

        /// <summary>
        /// The ClientService the message handler is using.
        /// </summary>
        public IClientService ClientService { get; private set; } = null;

        /// <summary>
        /// Whether to also log bot messages to the console.
        /// </summary>
        public bool LogToConsole { get; private set; } = true;

        /// <summary>
        /// How many messages are in the queue.
        /// </summary>
        public int ClientMessageCount => ClientMessages.Count;

        private string ChannelName = string.Empty;

        /// <summary>
        /// The message throttler that handles sending the messages when it's time.
        /// </summary>
        private BotMessageThrottler MessageThrottler = NoThrottleInstance;

        /// <summary>
        /// Queued messages.
        /// </summary>
        private readonly Queue<string> ClientMessages = new Queue<string>(16);

        public BotMessageHandler()
        {

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

        public void SetMessageThrottling(in MessageThrottlingOptions msgThrottleOption, in long maxMsgCount,
            in long msgCooldown)
        {
            MessageThrottler = InstantiateThrottler(msgThrottleOption, maxMsgCount, msgCooldown);
        }

        private BotMessageThrottler InstantiateThrottler(in MessageThrottlingOptions msgThrottleOption,
            in long maxMsgCount, in long msgCooldown)
        {
            switch (msgThrottleOption)
            {
                case MessageThrottlingOptions.MsgCountPerInterval:
                    return new BotMessagePerIntervalThrottler(maxMsgCount, msgCooldown);
                case MessageThrottlingOptions.TimeThrottled:
                    return new BotMessageTimeThrottler(msgCooldown);
                case MessageThrottlingOptions.None:
                default: return NoThrottleInstance;
            }
        }

        public void SetLogToConsole(in bool logToConsole)
        {
            LogToConsole = logToConsole;
        }

        public void Update(in DateTime nowUTC)
        {
            MessageThrottler.Update(nowUTC, this);
        }

        /// <summary>
        /// Sends the next queued message through the client service. This returns false if this fails.
        /// </summary>
        /// <returns>true if the message was successfully sent. false if the client service is disconnected or the message fails to send.</returns>
        public bool SendNextQueuedMessage()
        {
            //Ensure the client service has joined a channel, otherwise we can't send the message 
            if (ClientMessages.Count == 0 || ClientService.IsConnected == false || ClientService.JoinedChannels?.Count <= 0)
            {
                return false;
            }
            
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
                return false;
            }

            return true;
        }

        public void QueueMessage(string message)
        {
            if (string.IsNullOrEmpty(message) == false)
            {
                ClientMessages.Enqueue(message);
            }
        }

        public void QueueMessageSplit(string message, in int maxCharCount, string separator)
        {
            string sentMessage = Helpers.SplitStringWithinCharCount(message, maxCharCount, separator, out List<string> textList);

            //If the text fits within the character limit, print it all out at once
            if (textList == null)
            {
                QueueMessage(sentMessage);
            }
            else
            {
                //Otherwise, queue up the text in pieces
                for (int i = 0; i < textList.Count; i++)
                {
                    QueueMessage(textList[i]);
                }
            }
        }
    }
}
