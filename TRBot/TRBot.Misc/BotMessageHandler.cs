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
using TRBot.Connection;
using TRBot.Utilities;
using TRBot.Logging;

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
        /// Whether to also log bot messages to the logger.
        /// </summary>
        public bool LogToLogger { get; private set; } = true;

        /// <summary>
        /// How many messages are in the queue.
        /// </summary>
        public int ClientMessageCount => ClientMessages.Count;

        private string ChannelName = string.Empty;

        /// <summary>
        /// The message throttler that restricts how often messages are sent.
        /// </summary>
        public BotMessageThrottler MessageThrottler { get; private set; } = NoThrottleInstance;

        /// <summary>
        /// The current message throttling option.
        /// </summary>
        public MessageThrottlingOptions CurThrottleOption { get; private set; } = MessageThrottlingOptions.None;

        /// <summary>
        /// Queued messages.
        /// </summary>
        private readonly Queue<QueuedMessage> ClientMessages = new Queue<QueuedMessage>(16);

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

        public void SetMessageThrottling(in MessageThrottlingOptions msgThrottleOption,
            in MessageThrottleData messageThrottleData)
        {
            InstantiateOrUpdateThrottler(msgThrottleOption, messageThrottleData);
            CurThrottleOption = msgThrottleOption;
        }

        private void InstantiateOrUpdateThrottler(in MessageThrottlingOptions msgThrottleOption,
            in MessageThrottleData messageThrottleData)
        {
            //Simply set data if the throttle option hasn't changed
            if (CurThrottleOption == msgThrottleOption && MessageThrottler != null)
            {
                MessageThrottler.SetData(messageThrottleData);
            }
            else
            {
                //Instantiate based on the throttle option we have
                switch (msgThrottleOption)
                {
                    case MessageThrottlingOptions.MsgCountPerInterval:
                        MessageThrottler = new BotMessagePerIntervalThrottler(messageThrottleData);
                        break;
                    case MessageThrottlingOptions.TimeThrottled:
                        MessageThrottler = new BotMessageTimeThrottler(messageThrottleData);
                        break;
                    case MessageThrottlingOptions.None:
                    default: 
                        MessageThrottler = NoThrottleInstance;
                        break;
                }
            }
        }

        public void SetLogToLogger(in bool logToLogger)
        {
            LogToLogger = logToLogger;
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
            QueuedMessage queuedMsg = ClientMessages.Peek();

            //There's a chance the bot could be disconnected from the channel between the conditional and now
            try
            {
                //Send the message
                ClientService.SendMessage(ChannelName, queuedMsg.Message);

                if (LogToLogger == true)
                {
                    TRBotLogger.Logger.Write(queuedMsg.LogLevel, queuedMsg.Message);
                }

                //Remove from queue
                ClientMessages.Dequeue();
            }
            catch (Exception e)
            {
                TRBotLogger.Logger.Error($"Could not send message: {e.Message}");
                return false;
            }

            return true;
        }

        public void QueueMessage(string message)
        {
            if (string.IsNullOrEmpty(message) == false)
            {
                QueuedMessage queuedMsg = new QueuedMessage(message);
                ClientMessages.Enqueue(queuedMsg);
            }
        }

        public void QueueMessage(string message, in Serilog.Events.LogEventLevel logLevel)
        {
            if (string.IsNullOrEmpty(message) == false)
            {
                QueuedMessage queuedMsg = new QueuedMessage(message, logLevel);
                ClientMessages.Enqueue(queuedMsg);
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

        public void QueueMessageSplit(string message, in Serilog.Events.LogEventLevel logLevel, in int maxCharCount, string separator)
        {
            string sentMessage = Helpers.SplitStringWithinCharCount(message, maxCharCount, separator, out List<string> textList);

            //If the text fits within the character limit, print it all out at once
            if (textList == null)
            {
                QueueMessage(sentMessage, logLevel);
            }
            else
            {
                //Otherwise, queue up the text in pieces
                for (int i = 0; i < textList.Count; i++)
                {
                    QueueMessage(textList[i], logLevel);
                }
            }
        }

        /// <summary>
        /// Represents a queued message.
        /// </summary>
        private struct QueuedMessage
        {
            public string Message;
            public Serilog.Events.LogEventLevel LogLevel;

            public QueuedMessage(string message)
            {
                Message = message;
                LogLevel = Serilog.Events.LogEventLevel.Information;
            }

            public QueuedMessage(string message, in Serilog.Events.LogEventLevel logLevel)
            {
                Message = message;
                LogLevel = logLevel;
            }

            public override bool Equals(object obj)
            {
                if (obj is QueuedMessage queuedMsg)
                {
                    return (this == queuedMsg);
                }

                return false;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 5;
                    hash = (hash * 31) + ((Message == null) ? 0 : Message.GetHashCode());
                    hash = (hash * 31) + LogLevel.GetHashCode();
                    return hash;
                }
            }

            public static bool operator==(QueuedMessage a, QueuedMessage b)
            {
                return (a.Message == b.Message && a.LogLevel == b.LogLevel);
            }

            public static bool operator!=(QueuedMessage a, QueuedMessage b)
            {
                return !(a == b);
            }
        }
    }
}
