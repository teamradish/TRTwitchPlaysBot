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
using TwitchLib.Client.Models;

namespace TRBot
{
    /// <summary>
    /// Read inputs through a terminal.
    /// </summary>
    public class TerminalClientService : IClientService
    {
        /// <summary>
        /// The event handler associated with the service.
        /// </summary>
        public IEventHandler EventHandler { get; private set; } = null;

        /// <summary>
        /// Tells if the client is initialized.
        /// </summary>
        public bool IsInitialized => Initialized;

        /// <summary>
        /// Tells if the client is connected.
        /// </summary>
        public bool IsConnected => Connected;

        /// <summary>
        /// The channels the client has joined.
        /// </summary>
        public List<string> JoinedChannels { get; private set; } = new List<string>(8);

        private bool Initialized = false;
        private bool Connected = false;

        public TerminalClientService()
        {

        }

        /// <summary>
        /// Initializes the client.
        /// </summary>
        public void Initialize()
        {
            User dummyUser = DummyUserSetup();

            EventHandler = new TerminalEventHandler(dummyUser);

            EventHandler.OnJoinedChannelEvent -= OnClientJoinedChannel;
            EventHandler.OnJoinedChannelEvent += OnClientJoinedChannel;

            Initialized = true;
        }

        private User DummyUserSetup()
        {
            //Set name
            string line = string.Empty;
            while (string.IsNullOrEmpty(line) == true)
            {
                Console.WriteLine("Please choose a valid name for your user. This can be a valid user in your BotData.");
                line = Console.ReadLine();
            }

            User user = BotProgram.GetUser(line, false);

            if (user != null)
            {
                return user;
            }

            string userName = line;

            //Set level
            int level = -1;
            line = string.Empty;

            while (level < 0)
            {
                Console.WriteLine("Please choose an access level for your user.");
                line = Console.ReadLine();

                AccessLevels.Levels[] levelArray = EnumUtility.GetValues<AccessLevels.Levels>.EnumValues;

                if (int.TryParse(line, out int levelNum) == false)
                {
                    Console.WriteLine("Invalid level specified.");
                    continue;
                }

                bool found = false;
                string lvlName = string.Empty;

                for (int i = 0; i < levelArray.Length; i++)
                {
                    if (levelNum == (int)levelArray[i])
                    {
                        found = true;
                        lvlName = levelArray[i].ToString();
                        break;
                    }
                }

                if (found == false)
                {
                    Console.WriteLine("Invalid level specified.");
                    continue;
                }

                level = levelNum;
            }

            //Return new user
            user = new User();
            user.Name = userName;
            user.Level = level;

            return user;
        }

        /// <summary>
        /// Connects the client.
        /// </summary>
        public void Connect()
        {
            BotProgram.MsgHandler.SetIgnoreConsoleLog(true);

            EventHandler.Initialize();

            Connected = true;
        }

        /// <summary>
        /// Disconnects the client.
        /// </summary>
        public void Disconnect()
        {
            BotProgram.MsgHandler.SetIgnoreConsoleLog(true);

            JoinedChannels?.Clear();
            Connected = false;
        }

        /// <summary>
        /// Reconnects the client.
        /// </summary>
        public void Reconnect()
        {
            
        }

        /// <summary>
        /// Send a message through the client.
        /// </summary>
        public void SendMessage(string channel, string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Cleans up the client.
        /// </summary>
        public void CleanUp()
        {
            JoinedChannels = null;

            EventHandler.OnJoinedChannelEvent -= OnClientJoinedChannel;

            EventHandler.CleanUp();
        }

        private void OnClientJoinedChannel(EvtJoinedChannelArgs e)
        {
            //When joining a channel, set the joined channels list
            if (JoinedChannels == null)
            {
                JoinedChannels = new List<string>(1);
            }
            
            JoinedChannels.Clear();

            JoinedChannels.Add(e.Channel);
        }
    }
}
