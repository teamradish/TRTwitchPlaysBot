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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using TRBot.Data;
using TRBot.Commands;
using TRBot.Connection;
using TRBot.Logging;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace TRBot.Integrations.LiveSplitOne
{
    /// <summary>
    /// A command that controls a local LiveSplitOne instance through a WebSocket.
    /// </summary>
    public class LiveSplitOneCommand : BaseCommand
    {
        private const int DEFAULT_SOCKET_PORT = 4347;
        private const string DEFAULT_SOCKET_PATH = "/";

        /// <summary>
        /// The available LiveSplitOne commands.
        /// </summary>
        private readonly Dictionary<string, int> AvailableCommands = new Dictionary<string, int>()
        {
            { "start", 0 },
            { "split", 0 },
            { "splitorstart", 0 },
            { "reset", 0 },
            { "togglepause", 0 },
            { "undo", 0 },
            { "skip", 0 },
            { "initgametime", 0 },
            { "setgametime", 1 },
            { "setloadingtimes", 1 },
            { "pausegametime", 0 },
            { "resumegametime", 0 },
        };

        private WebSocketServer SocketServer = null;

        //Configuration for the WebSocket
        private int SocketPort = DEFAULT_SOCKET_PORT;
        private string SocketPath = DEFAULT_SOCKET_PATH;

        private string CachedCmdsStr = string.Empty;

        public LiveSplitOneCommand()
        {

        }

        public override void Initialize()
        {
            base.Initialize();

            InitializeCmdMessage();

            //Get database values for the port and path
            SocketPort = (int)DataHelper.GetSettingInt(LiveSplitOneSettingsConstants.LIVESPLITONE_WEBSOCKET_PORT_NUM, DEFAULT_SOCKET_PORT);
            SocketPath = DataHelper.GetSettingString(LiveSplitOneSettingsConstants.LIVESPLITONE_WEBSOCKET_PATH, DEFAULT_SOCKET_PATH);

            //Start up the WebSocket server on localhost with the given port and path
            try
            {
                SocketServer = new WebSocketServer(IPAddress.Parse("127.0.0.1"), SocketPort, false);
                SocketServer.AddWebSocketService<LiveSplitOneSocketBehavior>(SocketPath);

                //Increase wait time to give some more leniency
                SocketServer.WaitTime = TimeSpan.FromSeconds(60);

                SocketServer.Start();
            }
            catch (Exception e)
            {
                string message = $"Issue setting up WebSocket server: {e.Message}";
                if (e.InnerException != null)
                {
                    message += $" - {e.InnerException.Message}";
                }

                TRBotLogger.Logger.Error(message);
            }
        }

        public override void CleanUp()
        {
            try
            {
                if (SocketServer.IsListening == true)
                {
                    SocketServer.Stop();
                }
            }
            catch (Exception e)
            {
                string message = $"Issue stopping WebSocket server: {e.Message}";
                if (e.InnerException != null)
                {
                    message += $" - {e.InnerException.Message}";
                }

                TRBotLogger.Logger.Error(message);
            }

            base.CleanUp();
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            if (SocketServer == null || SocketServer.IsListening == false)
            {
                QueueMessage("ERROR: Cannot send command because the WebSocket server isn't running!", Serilog.Events.LogEventLevel.Warning);
                return;
            }

            //Validate arguments
            List<string> argList = args.Command.ArgumentsAsList;

            if (argList.Count == 0)
            {
                QueueMessage($"Please specify a command: {CachedCmdsStr}");
                return;
            }

            string cmdArg = argList[0].ToLowerInvariant();

            //Invalid command
            if (AvailableCommands.TryGetValue(cmdArg, out int argCount) == false)
            {
                QueueMessage($"\"{cmdArg}\" is an invalid command. Please specify a valid command: {CachedCmdsStr}");
                return;
            }

            //Invalid number of arguments for this LiveSplitOne command
            if ((argList.Count - 1) != argCount)
            {
                if (argCount <= 0)
                {
                    QueueMessage("\"{cmdArg}\" doesn't take any arguments!");
                }
                else
                {
                    QueueMessage($"\"{cmdArg}\" requires {argCount} argument(s)!");
                }

                return;
            }

            try
            {
                //Send the data over the websocket
                if (SocketServer.WebSocketServices.TryGetServiceHost(SocketPath, out WebSocketServiceHost host) == true)
                {
                    host.Sessions.Broadcast(args.Command.ArgumentsAsString.ToLowerInvariant());
                }
            }
            catch (Exception e)
            {
                QueueMessage($"Issue broadcasting WebSocket message: {e.Message}", Serilog.Events.LogEventLevel.Warning);
            }
        }

        private void InitializeCmdMessage()
        {
            //Build the message
            StringBuilder strBuilder = new StringBuilder(256);

            foreach (KeyValuePair<string, int> kvPair in AvailableCommands)
            {
                strBuilder.Append('"').Append(kvPair.Key).Append('"').Append(',').Append(' ');
            }

            if (strBuilder.Length > 1)
            {
                strBuilder.Remove(strBuilder.Length - 2, 2);

                CachedCmdsStr = strBuilder.ToString();
            }
        }
    }
}
