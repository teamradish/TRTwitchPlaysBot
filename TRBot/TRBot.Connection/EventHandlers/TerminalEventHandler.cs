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
using System.Threading;
using TRBot.Utilities;
using static TRBot.Connection.EventDelegates;

namespace TRBot.Connection
{
    /// <summary>
    /// Handles events from input through a terminal.
    /// </summary>
    public class TerminalEventHandler : IEventHandler
    {
        /// <summary>
        /// The default name for the terminal user.
        /// </summary>
        public const string DEFAULT_TERMINAL_USERNAME = "terminaluser";

        public event UserSentMessage UserSentMessageEvent = null;

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

        private volatile bool StopConsoleThread = false;

        private char CommandIdentifier = '!';
        private string TerminalUsername = DEFAULT_TERMINAL_USERNAME;

        public TerminalEventHandler(char commandIdentifier)
        {
            CommandIdentifier = commandIdentifier;
        }

        public void Initialize()
        {
            WaitForMainInitialization();       
        }

        public void CleanUp()
        {
            StopConsoleThread = true;

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

        private void WaitForMainInitialization()
        {
            SetupStart();

            //Create a new thread to handle inputs through the console
            Thread consoleThread = new Thread(ReadWaitInputs);
            consoleThread.IsBackground = true;
            consoleThread.Start();
        }

        private void SetupStart()
        {
            Console.WriteLine($"\nPlease enter a name to use (no spaces)! This can be an existing name in the database. Skip to use \"{DEFAULT_TERMINAL_USERNAME}\" as the name.");

            string newName = Console.ReadLine();

            Console.WriteLine();

            if (string.IsNullOrEmpty(newName) == true)
            {
                newName = DEFAULT_TERMINAL_USERNAME;
            }
            else
            {
                //Remove all spaces
                newName = Helpers.RemoveAllWhitespace(newName);
            }

            //Set the name
            TerminalUsername = newName;

            EvtConnectedArgs conArgs = new EvtConnectedArgs
            {
                BotUsername = TerminalUsername,
                AutoJoinChannel = string.Empty
            };

            OnConnectedEvent?.Invoke(conArgs);

            EvtJoinedChannelArgs joinedChannelArgs = new EvtJoinedChannelArgs
            {
                BotUsername = TerminalUsername,
                Channel = string.Empty
            };

            OnJoinedChannelEvent?.Invoke(joinedChannelArgs);
        }

        private void ReadWaitInputs()
        {
            while (true)
            {
                if (StopConsoleThread == true)
                {
                    break;
                }

                string line = Console.ReadLine();

                //Send message event
                EvtUserMessageArgs umArgs = new EvtUserMessageArgs()
                {
                    UsrMessage = new EvtUserMsgData(TerminalUsername, TerminalUsername, TerminalUsername,
                        string.Empty, line)
                };

                UserSentMessageEvent?.Invoke(umArgs);

                //Check for a command
                if (line.Length > 0 && line[0] == CommandIdentifier)
                {
                    //Build args list
                    List<string> argsList = new List<string>(line.Split(' '));
                    string argsAsStr = string.Empty;

                    //Remove the command itself and the space from the string
                    if (argsList.Count > 1)
                    {
                        argsAsStr = line.Remove(0, argsList[0].Length + 1);
                    }

                    //Remove command identifier
                    string cmdText = argsList[0].Remove(0, 1);
                    
                    //Now remove the first entry from the list, which is the command, retaining only the arguments
                    argsList.RemoveAt(0);

                    EvtChatCommandArgs chatcmdArgs = new EvtChatCommandArgs();
                    EvtUserMsgData msgData = new EvtUserMsgData(TerminalUsername, TerminalUsername, TerminalUsername,
                        string.Empty, line);

                    chatcmdArgs.Command = new EvtChatCommandData(argsList, argsAsStr, msgData, CommandIdentifier, cmdText);

                    ChatCommandReceivedEvent?.Invoke(chatcmdArgs);
                }
            }
        }
    }
}
