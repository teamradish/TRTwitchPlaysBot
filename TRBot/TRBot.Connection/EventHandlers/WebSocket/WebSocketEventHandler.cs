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
using WebSocketSharp;
using Newtonsoft.Json;
using TRBot.Logging;
using static TRBot.Connection.EventDelegates;

namespace TRBot.Connection.WebSocket
{
    /// <summary>
    /// Helps handle events from a WebSocket.
    /// </summary>
    public class WebSocketEventHandler : IEventHandler
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

        private WebSocketSharp.WebSocket EvtSocket = null;
        private char CmdIdentifier = '!';
        private string BotName = string.Empty;

        private JsonSerializerSettings SerializerSettings = null;

        public WebSocketEventHandler(WebSocketSharp.WebSocket evtSocket, char cmdIdentifier, string botName)
        {
            EvtSocket = evtSocket;
        }

        public void Initialize()
        {
            EvtSocket.OnOpen -= OnSocketOpened;
            EvtSocket.OnOpen += OnSocketOpened;

            EvtSocket.OnClose -= OnSocketClosed;
            EvtSocket.OnClose += OnSocketClosed;

            EvtSocket.OnError -= OnSocketError;
            EvtSocket.OnError += OnSocketError;

            EvtSocket.OnMessage -= OnSocketMessage;
            EvtSocket.OnMessage += OnSocketMessage;

            SerializerSettings = new JsonSerializerSettings();
            SerializerSettings.StringEscapeHandling = StringEscapeHandling.Default;
            SerializerSettings.ConstructorHandling = ConstructorHandling.Default;
            SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
        }

        public void CleanUp()
        {
            EvtSocket.OnOpen -= OnSocketOpened;
            EvtSocket.OnClose -= OnSocketClosed;
            EvtSocket.OnError -= OnSocketError;
            EvtSocket.OnMessage -= OnSocketMessage;

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

        private void OnSocketOpened(object sender, EventArgs e)
        {
            EvtConnectedArgs connectedArgs = new EvtConnectedArgs()
            {
                BotUsername = BotName,
                AutoJoinChannel = EvtSocket.Url.AbsoluteUri
            };

            OnConnectedEvent?.Invoke(connectedArgs);

            EvtJoinedChannelArgs joinedChannelArgs = new EvtJoinedChannelArgs()
            {
                BotUsername = BotName,
                Channel = EvtSocket.Url.AbsoluteUri
            };

            OnJoinedChannelEvent?.Invoke(joinedChannelArgs);
        }

        private void OnSocketClosed(object sender, CloseEventArgs e)
        {
            EvtDisconnectedArgs disconnectedArgs = new EvtDisconnectedArgs();

            OnDisconnectedEvent?.Invoke(disconnectedArgs);
        }

        private void OnSocketError(object sender, ErrorEventArgs e)
        {
            EvtErrorData errorData = new EvtErrorData($"{e.Message} | {e.Exception.Message}");
            EvtConnectionErrorArgs errorArgs = new EvtConnectionErrorArgs()
            {
                Error = errorData,
                BotUsername = BotName
            };

            OnConnectionErrorEvent?.Invoke(errorArgs);
        }
        
        private void OnSocketMessage(object sender, MessageEventArgs e)
        {
            string json = e.Data;

            //TRBotLogger.Logger.Information($"Received: {json}");

            WebSocketResponse response = null;

            try
            {
                response = JsonConvert.DeserializeObject<WebSocketResponse>(json, SerializerSettings);
            }
            catch (Exception exc)
            {
                TRBotLogger.Logger.Warning($"Invalid JSON received as a response. {exc.Message}");
                return;
            }

            if (response.Message == null)
            {
                TRBotLogger.Logger.Warning("Message JSON object is invalid as it couldn't be parsed.");
                return;
            }

            if (string.IsNullOrEmpty(response.Message.Text) == true)
            {
                TRBotLogger.Logger.Warning("null or empty message string.");
                return;
            }

            string msgWSReplaced = Utilities.Helpers.ReplaceAllWhitespaceWithSpace(response.Message.Text);

            string userName = string.Empty;

            if (response.User != null)
            {
                userName = response.User.Name;
            }

            //Send message event
            EvtUserMessageArgs umArgs = new EvtUserMessageArgs()
            {
                UsrMessage = new EvtUserMsgData(userName, userName, userName, EvtSocket.Url.AbsoluteUri, msgWSReplaced)
            };
            
            UserSentMessageEvent?.Invoke(umArgs);

            //Check for a command
            if (msgWSReplaced.Length > 0 && msgWSReplaced[0] == CmdIdentifier)
            {
                //Build args list
                List<string> argsList = new List<string>(msgWSReplaced.Split(' '));
                string argsAsStr = string.Empty;

                //Remove the command itself and the space from the string
                if (argsList.Count > 1)
                {
                    argsAsStr = msgWSReplaced.Remove(0, argsList[0].Length + 1);
                }
                
                //Remove command identifier
                string cmdText = argsList[0].Remove(0, 1);
                    
                //Now remove the first entry from the list, which is the command, retaining only the arguments
                argsList.RemoveAt(0);

                EvtChatCommandArgs chatcmdArgs = new EvtChatCommandArgs();
                EvtUserMsgData msgData = new EvtUserMsgData(userName, userName, userName,
                    string.Empty, msgWSReplaced);

                chatcmdArgs.Command = new EvtChatCommandData(argsList, argsAsStr, msgData, CmdIdentifier, cmdText);

                ChatCommandReceivedEvent?.Invoke(chatcmdArgs);
            }
        }
    }
}