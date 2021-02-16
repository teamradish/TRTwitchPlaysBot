/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
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
using TwitchLib.Communication.Events;
using static TRBot.Connection.EventDelegates;

namespace TRBot.Connection
{
    /// <summary>
    /// Helps handle events from Twitch.
    /// </summary>
    public class TwitchEventHandler : IEventHandler
    {
        public event UserSentMessage UserSentMessageEvent = null;

        //public event UserMadeInput UserMadeInputEvent = null;

        public event UserNewlySubscribed UserNewlySubscribedEvent = null;

        public event UserReSubscribed UserReSubscribedEvent = null;

        public event OnWhisperReceived WhisperReceivedEvent = null;

        public event ChatCommandReceived ChatCommandReceivedEvent = null;

        public event OnJoinedChannel OnJoinedChannelEvent = null;

        public event ChannelBeingHosted ChannelHostedEvent = null;

        public event OnConnected OnConnectedEvent = null;

        public event OnConnectionError OnConnectionErrorEvent = null;

        public event OnReconnected OnReconnectedEvent = null;

        public event OnDisconnected OnDisconnectedEvent = null;

        private TwitchClient twitchClient = null;

        public TwitchEventHandler(TwitchClient client)
        {
            twitchClient = client;
        }

        public void Initialize()
        {
            twitchClient.OnMessageReceived -= OnMessageReceived;
            twitchClient.OnMessageReceived += OnMessageReceived;

            twitchClient.OnNewSubscriber -= OnNewSubscriber;
            twitchClient.OnNewSubscriber += OnNewSubscriber;
            
            twitchClient.OnReSubscriber -= OnReSubscriber;
            twitchClient.OnReSubscriber += OnReSubscriber;

            twitchClient.OnWhisperReceived -= OnWhisperReceived;
            twitchClient.OnWhisperReceived += OnWhisperReceived;

            twitchClient.OnChatCommandReceived -= OnChatCommandReceived;
            twitchClient.OnChatCommandReceived += OnChatCommandReceived;

            twitchClient.OnJoinedChannel -= OnJoinedChannel;
            twitchClient.OnJoinedChannel += OnJoinedChannel;

            twitchClient.OnBeingHosted -= OnChannelHosted;
            twitchClient.OnBeingHosted += OnChannelHosted;

            twitchClient.OnConnected -= OnConnected;
            twitchClient.OnConnected += OnConnected;

            twitchClient.OnConnectionError -= OnConnectionError;
            twitchClient.OnConnectionError += OnConnectionError;

            twitchClient.OnReconnected -= OnReconnected;
            twitchClient.OnReconnected += OnReconnected;

            twitchClient.OnDisconnected -= OnDisconnected;
            twitchClient.OnDisconnected += OnDisconnected;
        }

        public void CleanUp()
        {
            twitchClient.OnMessageReceived -= OnMessageReceived;
            twitchClient.OnNewSubscriber -= OnNewSubscriber;
            twitchClient.OnReSubscriber -= OnReSubscriber;
            twitchClient.OnWhisperReceived -= OnWhisperReceived;
            twitchClient.OnChatCommandReceived -= OnChatCommandReceived;
            twitchClient.OnJoinedChannel -= OnJoinedChannel;
            twitchClient.OnBeingHosted -= OnChannelHosted;
            twitchClient.OnConnected -= OnConnected;
            twitchClient.OnConnectionError -= OnConnectionError;
            twitchClient.OnReconnected -= OnReconnected;
            twitchClient.OnDisconnected -= OnDisconnected;

            UserSentMessageEvent = null;
            UserNewlySubscribedEvent = null;
            UserReSubscribedEvent = null;
            WhisperReceivedEvent = null;
            ChatCommandReceivedEvent = null;
            OnJoinedChannelEvent = null;
            ChannelHostedEvent = null;
            OnConnectedEvent = null;
            OnConnectionErrorEvent = null;
            OnReconnectedEvent = null;
            OnDisconnectedEvent = null;
        }

        //Break up much of the message handling by sending events
        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            EvtUserMessageArgs umArgs = new EvtUserMessageArgs()
            {
                //UserData = user,
                UsrMessage = new EvtUserMsgData(e.ChatMessage.UserId, e.ChatMessage.Username,
                    e.ChatMessage.DisplayName, e.ChatMessage.Channel, e.ChatMessage.Message)
            };

            UserSentMessageEvent?.Invoke(umArgs);
        }

        private void OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            EvtOnSubscriptionArgs subArgs = new EvtOnSubscriptionArgs
            {
                SubscriptionData = new EvtSubscriptionData(e.Subscriber.UserId, e.Subscriber.DisplayName,
                    e.Subscriber.DisplayName)
            };

            UserNewlySubscribedEvent?.Invoke(subArgs);
        }

        private void OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            EvtOnReSubscriptionArgs reSubArgs = new EvtOnReSubscriptionArgs
            {
                ReSubscriptionData = new EvtReSubscriptionData(e.ReSubscriber.UserId, e.ReSubscriber.DisplayName,
                    e.ReSubscriber.DisplayName, e.ReSubscriber.Months)
            };

            UserReSubscribedEvent?.Invoke(reSubArgs);
        }

        private void OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            EvtWhisperMessageArgs whisperMsg = new EvtWhisperMessageArgs()
            {
                WhsprMessage = new EvtWhisperMsgData(e.WhisperMessage.UserId, e.WhisperMessage.Username,
                    e.WhisperMessage.DisplayName, e.WhisperMessage.MessageId, e.WhisperMessage.ThreadId,
                    e.WhisperMessage.Message)
            };

            WhisperReceivedEvent?.Invoke(whisperMsg);
        }

        private void OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            ChatMessage cMsg = e.Command.ChatMessage;

            EvtUserMsgData msgData = new EvtUserMsgData(cMsg.UserId, cMsg.Username, cMsg.DisplayName,
                cMsg.Channel, cMsg.Message);

            EvtChatCommandArgs chatCmdArgs = new EvtChatCommandArgs
            {
                Command = new EvtChatCommandData(e.Command.ArgumentsAsList, e.Command.ArgumentsAsString,
                    msgData, e.Command.CommandIdentifier, e.Command.CommandText)
            };

            ChatCommandReceivedEvent?.Invoke(chatCmdArgs);
        }

        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            EvtJoinedChannelArgs jcArgs = new EvtJoinedChannelArgs()
            {
                BotUsername = e.BotUsername,
                Channel = e.Channel
            };

            OnJoinedChannelEvent?.Invoke(jcArgs);
        }

        private void OnChannelHosted(object sender, OnBeingHostedArgs e)
        {
            BeingHostedNotification bHNotif = e.BeingHostedNotification;

            EvtOnHostedArgs hostedArgs = new EvtOnHostedArgs
            {
                HostedData = new EvtHostedData(bHNotif.Channel, bHNotif.HostedByChannel,
                bHNotif.Viewers, bHNotif.IsAutoHosted)
            };

            ChannelHostedEvent?.Invoke(hostedArgs);
        }

        private void OnConnected(object sender, OnConnectedArgs e)
        {
            EvtConnectedArgs connectedArgs = new EvtConnectedArgs()
            {
                BotUsername = e.BotUsername,
                AutoJoinChannel = e.AutoJoinChannel
            };

            OnConnectedEvent?.Invoke(connectedArgs);
        }

        private void OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            EvtConnectionErrorArgs cErrArgs = new EvtConnectionErrorArgs()
            {
                Error = new EvtErrorData(e.Error.Message),
                BotUsername = e.BotUsername
            };

            OnConnectionErrorEvent?.Invoke(cErrArgs);
        }

        private void OnReconnected(object sender, OnReconnectedEventArgs e)
        {
            EvtReconnectedArgs recArgs = new EvtReconnectedArgs()
            {

            };

            OnReconnectedEvent?.Invoke(recArgs);
        }

        private void OnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            EvtDisconnectedArgs disArgs = new EvtDisconnectedArgs()
            {

            };

            OnDisconnectedEvent?.Invoke(disArgs);
        }
    }
}