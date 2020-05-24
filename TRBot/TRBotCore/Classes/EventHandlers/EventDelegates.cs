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
using TwitchLib.Communication.Events;

namespace TRBot
{
    /// <summary>
    /// Delegates for event handlers.
    /// </summary>
    public static class EventDelegates
    {
        public delegate void UserSentMessage(User user, EvtUserMessageArgs e);

        public delegate void UserMadeInput(User user, EvtUserMessageArgs e, in Parser.InputSequence validInputSeq);

        public delegate void UserNewlySubscribed(User user, EvtOnSubscriptionArgs e);

        public delegate void UserReSubscribed(User user, EvtOnReSubscriptionArgs e);

        public delegate void OnWhisperReceived(EvtWhisperMessageArgs e);

        public delegate void ChatCommandReceived(EvtChatCommandArgs e);

        public delegate void OnJoinedChannel(EvtJoinedChannelArgs e);

        public delegate void ChannelBeingHosted(EvtOnHostedArgs e);
            
        public delegate void OnConnected(EvtConnectedArgs e);

        public delegate void OnConnectionError(EvtConnectionErrorArgs e);

        public delegate void OnReconnected(EvtReconnectedArgs e);

        public delegate void OnDisconnected(EvtDisconnectedArgs e);
    }
}
