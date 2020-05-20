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

namespace TRBot
{
    /// <summary>
    /// Interface for handling events.
    /// </summary>
    public interface IEventHandler
    {
        /// <summary>
        /// An event invoked whenever a user sends a message to chat.
        /// </summary>
        event EventDelegates.UserSentMessage UserSentMessageEvent;

        /// <summary>
        /// An event invoked whenever a user makes a valid input.
        /// This should be invoked after all post processing validation.
        /// </summary>
        event EventDelegates.UserMadeInput UserMadeInputEvent;

        /// <summary>
        /// An event invoked whenever a user newly subscribed to the channel.
        /// </summary>
        event EventDelegates.UserNewlySubscribed UserNewlySubscribedEvent;

        /// <summary>
        /// An event invoked whenever a user resubscribed to the channel.
        /// </summary>
        event EventDelegates.UserReSubscribed UserReSubscribedEvent;

        /// <summary>
        /// Initializes the IEventHandler.
        /// </summary>
        void Initialize(TwitchClient client);

        /// <summary>
        /// Cleans up the IEventHandler.
        /// </summary>
        void CleanUp(TwitchClient client);
    }
}
