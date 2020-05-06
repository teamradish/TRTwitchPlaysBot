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
    /// Helps handle events.
    /// </summary>
    public class EventHandler
    {
        public delegate void UserSentMessage(User user, OnMessageReceivedArgs e);
        public event UserSentMessage UserSentMessageEvent = null;

        public delegate void UserMadeInput(User user, in Parser.InputSequence validInputSeq);
        public event UserMadeInput UserMadeInputEvent = null;

        public delegate void UserNewlySubscribed(User user, OnNewSubscriberArgs e);
        public event UserNewlySubscribed UserNewlySubscribedEvent = null;

        public delegate void UserReSubscribed(User user, OnReSubscriberArgs e);
        public event UserReSubscribed UserReSubscribedEvent = null;

        public void Initialize(TwitchClient client)
        {
            client.OnMessageReceived -= OnMessageReceived;
            client.OnMessageReceived += OnMessageReceived;

            client.OnNewSubscriber -= OnNewSubscriber;
            client.OnNewSubscriber += OnNewSubscriber;
            
            client.OnReSubscriber -= OnReSubscriber;
            client.OnReSubscriber += OnReSubscriber;
        }

        public void CleanUp(TwitchClient client)
        {
            client.OnMessageReceived -= OnMessageReceived;
            client.OnNewSubscriber -= OnNewSubscriber;
            client.OnReSubscriber -= OnReSubscriber;

            UserSentMessageEvent = null;
            UserMadeInputEvent = null;
            UserNewlySubscribedEvent = null;
            UserReSubscribedEvent = null;
        }

        //NOTE: This is in place until we create better infrastructure for managing this
        public void InvokeUserMadeInputEvent(User user, in Parser.InputSequence validInputSeq)
        {
            UserMadeInputEvent?.Invoke(user, validInputSeq);
        }

        //Break up much of the message handling by sending events
        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            User user = BotProgram.GetOrAddUser(e.ChatMessage.DisplayName, false);

            UserSentMessageEvent?.Invoke(user, e);
        }

        private void OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            User user = BotProgram.GetOrAddUser(e.Subscriber.DisplayName, false);

            UserNewlySubscribedEvent?.Invoke(user, e);
        }

        private void OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            User user = BotProgram.GetOrAddUser(e.ReSubscriber.DisplayName, false);

            UserReSubscribedEvent?.Invoke(user, e);
        }
    }
}
