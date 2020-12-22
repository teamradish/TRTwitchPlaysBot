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
using TwitchLib.Client;
using TwitchLib.Client.Events;
using static TRBot.Connection.EventDelegates;

namespace TRBot.Connection
{
    /// <summary>
    /// Interface for handling events.
    /// </summary>
    public interface IEventHandler
    {
        /// <summary>
        /// An event invoked whenever a user sends a message to chat.
        /// </summary>
        event UserSentMessage UserSentMessageEvent;

        // <summary>
        // An event invoked whenever a user makes a valid input.
        // This should be invoked after all post processing validation.
        // </summary>
        //event UserMadeInput UserMadeInputEvent;

        /// <summary>
        /// An event invoked whenever a user newly subscribed to the channel.
        /// </summary>
        event UserNewlySubscribed UserNewlySubscribedEvent;

        /// <summary>
        /// An event invoked whenever a user resubscribed to the channel.
        /// </summary>
        event UserReSubscribed UserReSubscribedEvent;

        /// <summary>
        /// An event invoked when the bot receives a whisper.
        /// </summary>
        event OnWhisperReceived WhisperReceivedEvent;

        /// <summary>
        /// An event invoked when the bot receives a chat command.
        /// </summary>
        event ChatCommandReceived ChatCommandReceivedEvent;

        /// <summary>
        /// An event invoked when the bot joins a channel.
        /// </summary>
        event OnJoinedChannel OnJoinedChannelEvent;

        /// <summary>
        /// An event invoked when a channel the bot is on is being hosted by another.
        /// </summary>
        event ChannelBeingHosted ChannelHostedEvent;

        /// <summary>
        /// An event invoked when the bot connects to the service.
        /// </summary>
        event OnConnected OnConnectedEvent;

        /// <summary>
        /// An event invoked when the bot fails to connect to the service.
        /// </summary>
        event OnConnectionError OnConnectionErrorEvent;

        /// <summary>
        /// An event invoked when the bot reconnects to the service.
        /// </summary>
        event OnReconnected OnReconnectedEvent;

        /// <summary>
        /// An event invoked when the bot disconnects from the service.
        /// </summary>
        event OnDisconnected OnDisconnectedEvent;

        /// <summary>
        /// Initializes the IEventHandler.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Cleans up the IEventHandler.
        /// </summary>
        void CleanUp();
    }
}
