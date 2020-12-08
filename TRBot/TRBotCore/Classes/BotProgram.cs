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
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Client.Events;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Interfaces;
using Newtonsoft;
using Newtonsoft.Json;

namespace TRBot
{
    public sealed class BotProgram : IDisposable
    {
        private static BotProgram instance = null;

        public bool Initialized { get; private set; } = false;

        public static TimeSpan TotalUptime { get; private set;} = TimeSpan.Zero;
        private static DateTime StartUptime = DateTime.UtcNow;

        //NOTE: Potentially move all these to a separate data storage class
        private LoginInfo LoginInformation = null;
        public static Settings BotSettings { get; private set; } = null;
        public static BotData BotData { get; private set; } = null;
        public static InputCallbackData InputCBData { get; private set; } = null;

        public static IClientService ClientService { get; private set; } = null;
        private ConnectionCredentials Credentials = null;
        private CrashHandler crashHandler = null;

        private CommandHandler CommandHandler = null;
        public static BotMessageHandler MsgHandler { get; private set; } = null;
        public static BotRoutineHandler RoutineHandler { get; private set; } = null;

        public static string BotName
        {
            get
            {
                if (instance != null)
                {
                    if (instance.LoginInformation != null) return instance.LoginInformation.BotName;
                }

                return "N/A";
            }
        }

        public BotProgram()
        {
            crashHandler = new CrashHandler();
            
            instance = this;

            //Below normal priority
            //NOTE: Perhaps have a setting to set the priority
            Process thisProcess = Process.GetCurrentProcess();
            thisProcess.PriorityBoostEnabled = false;
            thisProcess.PriorityClass = ProcessPriorityClass.Idle;
        }

        //Clean up anything we need to here
        public void Dispose()
        {
            if (Initialized == false)
                return;

            UnsubscribeEvents();

            RoutineHandler?.CleanUp();

            CommandHandler.CleanUp();
            ClientService?.CleanUp();

            MsgHandler?.CleanUp();

            if (ClientService?.IsConnected == true)
                ClientService.Disconnect();

            //Clean up and relinquish the virtual controllers when we're done
            InputGlobals.ControllerMngr?.CleanUp();

            instance = null;
        }

        public void Initialize()
        {
            if (Initialized == true)
                return;

            //Kimimaru: Use invariant culture
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            //Load all the necessary data; if something doesn't exist, save out an empty object so it can be filled in manually
            string loginText = Globals.ReadFromTextFileOrCreate(Globals.LoginInfoFilename);
            LoginInformation = JsonConvert.DeserializeObject<LoginInfo>(loginText);

            if (LoginInformation == null)
            {
                Console.WriteLine("No login information found; attempting to create file template. If created, please manually fill out the information.");

                LoginInformation = new LoginInfo();
                string text = JsonConvert.SerializeObject(LoginInformation, Formatting.Indented);
                Globals.SaveToTextFile(Globals.LoginInfoFilename, text);
            }

            LoadSettingsAndBotData();

            //Kimimaru: If the bot itself isn't in the bot data, add it as an admin!
            if (string.IsNullOrEmpty(LoginInformation.BotName) == false)
            {
                string botName = LoginInformation.BotName.ToLowerInvariant();
                User botUser = null;
                if (BotData.Users.TryGetValue(botName, out botUser) == false)
                {
                    botUser = new User();
                    botUser.Name = botName;
                    botUser.Level = (int)AccessLevels.Levels.Admin;
                    BotData.Users.TryAdd(botName, botUser);

                    SaveBotData();
                }
            }

            ClientServiceTypes clientType = BotSettings.ClientSettings.ClientType;

            //Credentials don't matter if running through a terminal
            if (clientType != ClientServiceTypes.Terminal)
            {
                try
                {
                    Credentials = new ConnectionCredentials(LoginInformation.BotName, LoginInformation.Password);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Invalid credentials: {exception.Message}");
                    Console.WriteLine("Cannot proceed. Please double check the login information in the data folder");
                    return;
                }
            }
            
            Console.WriteLine($"Setting up client service: {clientType}");

            //Set up client service
            //NOTE: Replace with something more scalable later, such as a factory
            switch (clientType)
            {
                case ClientServiceTypes.Terminal:
                    ClientService = new TerminalClientService();
                    break;
                case ClientServiceTypes.Twitch:
                    ClientService = new TwitchClientService(Credentials, LoginInformation.ChannelName,
                        Globals.CommandIdentifier, Globals.CommandIdentifier, true);
                    break;
            }

            //Initialize service
            ClientService.Initialize();

            UnsubscribeEvents();
            SubscribeEvents();

            //Set up message handler
            MsgHandler = new BotMessageHandler(ClientService, LoginInformation.ChannelName, BotSettings.MsgSettings.MessageCooldown);

            RoutineHandler = new BotRoutineHandler(ClientService);
            RoutineHandler.AddRoutine(new PeriodicMessageRoutine());
            RoutineHandler.AddRoutine(new CreditsGiveRoutine());
            RoutineHandler.AddRoutine(new ReconnectRoutine());

            //Initialize controller input - validate the controller type first
            if (InputGlobals.IsVControllerSupported((InputGlobals.VControllerTypes)BotData.LastVControllerType) == false)
            {
                BotData.LastVControllerType = (int)InputGlobals.GetDefaultSupportedVControllerType();
            }

            InputGlobals.VControllerTypes vCType = (InputGlobals.VControllerTypes)BotData.LastVControllerType;
            Console.WriteLine($"Setting up virtual controller {vCType}");
            
            InputGlobals.SetVirtualController(vCType);

            StartUptime = DateTime.UtcNow;

            Initialized = true;
        }

        public void Run()
        {
            if (ClientService.IsConnected == true)
            {
                Console.WriteLine("Client is already connected and running!");
                return;
            }

            ClientService.Connect();

            //Run
            while (true)
            {
                //Store the bot's uptime
                DateTime utcNow = DateTime.UtcNow;
                TotalUptime = (utcNow - StartUptime);

                DateTime now = DateTime.Now;

                //Update queued messages
                MsgHandler.Update(now);

                //Update routines
                RoutineHandler.Update(now);

                Thread.Sleep(BotSettings.MainThreadSleep);
            }
        }

        private void UnsubscribeEvents()
        {
            ClientService.EventHandler.UserSentMessageEvent -= OnUserSentMessage;
            ClientService.EventHandler.UserMadeInputEvent -= OnUserMadeInput;
            ClientService.EventHandler.UserNewlySubscribedEvent -= OnNewSubscriber;
            ClientService.EventHandler.UserReSubscribedEvent -= OnReSubscriber;
            ClientService.EventHandler.WhisperReceivedEvent -= OnWhisperReceived;
            ClientService.EventHandler.ChatCommandReceivedEvent -= OnChatCommandReceived;
            ClientService.EventHandler.OnJoinedChannelEvent -= OnJoinedChannel;
            ClientService.EventHandler.ChannelHostedEvent -= OnBeingHosted;
            ClientService.EventHandler.OnConnectedEvent -= OnConnected;
            ClientService.EventHandler.OnConnectionErrorEvent -= OnConnectionError;
            ClientService.EventHandler.OnReconnectedEvent -= OnReconnected;
            ClientService.EventHandler.OnDisconnectedEvent -= OnDisconnected;
        }

        private void SubscribeEvents()
        {
            ClientService.EventHandler.UserSentMessageEvent += OnUserSentMessage;
            ClientService.EventHandler.UserMadeInputEvent += OnUserMadeInput;
            ClientService.EventHandler.UserNewlySubscribedEvent += OnNewSubscriber;
            ClientService.EventHandler.UserReSubscribedEvent += OnReSubscriber;
            ClientService.EventHandler.WhisperReceivedEvent += OnWhisperReceived;
            ClientService.EventHandler.ChatCommandReceivedEvent += OnChatCommandReceived;
            ClientService.EventHandler.OnJoinedChannelEvent += OnJoinedChannel;
            ClientService.EventHandler.ChannelHostedEvent += OnBeingHosted;
            ClientService.EventHandler.OnConnectedEvent += OnConnected;
            ClientService.EventHandler.OnConnectionErrorEvent += OnConnectionError;
            ClientService.EventHandler.OnReconnectedEvent += OnReconnected;
            ClientService.EventHandler.OnDisconnectedEvent += OnDisconnected;
        }

#region Events

        private void OnConnected(EvtConnectedArgs e)
        {
            Console.WriteLine($"{LoginInformation.BotName} connected!");
        }

        private void OnConnectionError(EvtConnectionErrorArgs e)
        {
            Console.WriteLine($"Failed to connect: {e.Error.Message}");
        }

        private void OnJoinedChannel(EvtJoinedChannelArgs e)
        {
            if (string.IsNullOrEmpty(BotSettings.MsgSettings.ConnectMessage) == false)
            {
                string finalMsg = BotSettings.MsgSettings.ConnectMessage.Replace("{0}", LoginInformation.BotName).Replace("{1}", Globals.CommandIdentifier.ToString());
                MsgHandler.QueueMessage(finalMsg);
            }

            Console.WriteLine($"Joined channel \"{e.Channel}\"");

            if (CommandHandler == null)
            {
                CommandHandler = new CommandHandler();
            }
        }

        private void OnChatCommandReceived(EvtChatCommandArgs e)
        {
            //If an exception is unhandled in a command, the entire bot will hang up (potential internal TwitchLib issue)
            try
            {
                CommandHandler.HandleCommand(e.UserData, e);
            }
            catch (Exception exc)
            {
                BotProgram.MsgHandler.QueueMessage($"Error handling command \"{e.Command.CommandText}\": {exc.Message}\n {exc.StackTrace}");
            }
        }

        private void OnUserSentMessage(EvtUserMessageArgs e)
        {
            if (e.UserData.OptedOut == false)
            {
                e.UserData.IncrementMsgCount();
            }

            string possibleMeme = e.UsrMessage.Message.ToLower();
            if (BotProgram.BotData.Memes.TryGetValue(possibleMeme, out string meme) == true)
            {
                BotProgram.MsgHandler.QueueMessage(meme);
            }
        }

        private void OnUserMadeInput(EvtUserInputArgs e)
        {
            User user = e.UserData;

            //Mark this as a valid input
            if (user.OptedOut == false)
            {
                user.IncrementValidInputCount();
            }

            //bool shouldPerformInput = true;

            ///NOTE: This validation is now performed beforehand in
            ///<see cref="ParserPostProcess.CheckInputPermissionsAndPorts"/>

            //Check the team the user is on for the controller they should be using
            //Validate that the controller is acquired and exists
            //int controllerNum = user.Team;
            //if (controllerNum < 0 || controllerNum >= InputGlobals.ControllerMngr.ControllerCount)
            //{
            //    BotProgram.MsgHandler.QueueMessage($"ERROR: Invalid joystick number {controllerNum + 1}. # of joysticks: {InputGlobals.ControllerMngr.ControllerCount}. Please change your controller port to a valid number to perform inputs.");
            //    shouldPerformInput = false;
            //}
            ////Now verify that the controller has been acquired
            //else if (InputGlobals.ControllerMngr.GetController(controllerNum).IsAcquired == false)
            //{
            //    BotProgram.MsgHandler.QueueMessage($"ERROR: Joystick number {controllerNum + 1} with controller ID of {InputGlobals.ControllerMngr.GetController(controllerNum).ControllerID} has not been acquired! Ensure you (the streamer) have a virtual device set up at this ID.");
            //    shouldPerformInput = false;
            //}

            //We're okay to perform the input
            //if (shouldPerformInput == true)
            //{
                InputHandler.CarryOutInput(e.ValidInputSeq.Inputs, InputGlobals.CurrentConsole, InputGlobals.ControllerMngr);

                //If auto whitelist is enabled, the user reached the whitelist message threshold,
                //the user isn't whitelisted, and the user hasn't ever been whitelisted, whitelist them
                if (BotSettings.AutoWhitelistEnabled == true && user.Level < (int)AccessLevels.Levels.Whitelisted
                    && user.AutoWhitelisted == false && user.ValidInputs >= BotSettings.AutoWhitelistInputCount)
                {
                    user.Level = (int)AccessLevels.Levels.Whitelisted;
                    user.SetAutoWhitelist(true);
                    if (string.IsNullOrEmpty(BotSettings.MsgSettings.AutoWhitelistMsg) == false)
                    {
                        //Replace the user's name with the message
                        string msg = BotSettings.MsgSettings.AutoWhitelistMsg.Replace("{0}", user.Name);
                        MsgHandler.QueueMessage(msg);
                    }
                }
            //}
        }

        private void OnWhisperReceived(EvtWhisperMessageArgs e)
        {
            
        }

        private void OnBeingHosted(EvtOnHostedArgs e)
        {
            if (string.IsNullOrEmpty(BotSettings.MsgSettings.BeingHostedMsg) == false)
            {
                string finalMsg = BotSettings.MsgSettings.BeingHostedMsg.Replace("{0}", e.HostedData.HostedByChannel);
                MsgHandler.QueueMessage(finalMsg);
            }
        }

        private void OnNewSubscriber(EvtOnSubscriptionArgs e)
        {
            if (string.IsNullOrEmpty(BotSettings.MsgSettings.NewSubscriberMsg) == false)
            {
                string finalMsg = BotSettings.MsgSettings.NewSubscriberMsg.Replace("{0}", e.SubscriptionData.DisplayName);
                MsgHandler.QueueMessage(finalMsg);
            }
        }

        private void OnReSubscriber(EvtOnReSubscriptionArgs e)
        {
            if (string.IsNullOrEmpty(BotSettings.MsgSettings.ReSubscriberMsg) == false)
            {
                string finalMsg = BotSettings.MsgSettings.ReSubscriberMsg.Replace("{0}", e.ReSubscriptionData.DisplayName).Replace("{1}", e.ReSubscriptionData.Months.ToString());
                MsgHandler.QueueMessage(finalMsg);
            }
        }

        private void OnReconnected(EvtReconnectedArgs e)
        {
            if (string.IsNullOrEmpty(BotSettings.MsgSettings.ReconnectedMsg) == false)
            {
                MsgHandler.QueueMessage(BotSettings.MsgSettings.ReconnectedMsg);
            }
        }

        private void OnDisconnected(EvtDisconnectedArgs e)
        {
            Console.WriteLine("Bot disconnected! Please check your internet connection.");
        }

        public static User GetUser(string username, bool isLower = true)
        {
            if (isLower == false)
            {
                username = username.ToLowerInvariant();
            }

            BotData.Users.TryGetValue(username, out User userData);

            return userData;
        }

        /// <summary>
        /// Gets a user object by username and adds the user object if the username isn't found.
        /// </summary>
        /// <param name="username">The name of the user.</param>
        /// <param name="isLower">Whether the username is all lower-case or not.
        /// If false, will make the username lowercase before checking the name.</param>
        /// <returns>A User object associated with <paramref name="username"/>.</returns>
        public static User GetOrAddUser(string username, bool isLower = true)
        {
            //Cannot use a user with no valid name
            if (string.IsNullOrEmpty(username) == true)
            {
                return null;
            }

            string origName = username;
            if (isLower == false)
            {
                username = username.ToLowerInvariant();
            }

            User userData = null;

            //Check to add a user that doesn't exist
            if (BotData.Users.TryGetValue(username, out userData) == false)
            {
                userData = new User();
                userData.Name = username;
                if (BotData.Users.TryAdd(username, userData) == false)
                {
                    Console.WriteLine($"An error occurred - failed to add user {username}");
                    return null;
                }

                if (string.IsNullOrEmpty(BotSettings.MsgSettings.NewUserMsg) == false)
                {
                    string finalMsg = BotSettings.MsgSettings.NewUserMsg.Replace("{0}", origName).Replace("{1}", Globals.CommandIdentifier.ToString());
                    BotProgram.MsgHandler.QueueMessage(finalMsg);
                }
            }

            return userData;
        }

        public static void SaveDataToFile(object value, string filename)
        {
            //Make sure more than one thread doesn't try to save at the same time to prevent potential loss of data and access violations
            lock (Globals.BotDataLockObj)
            {
                string text = JsonConvert.SerializeObject(value, Formatting.Indented);
                if (string.IsNullOrEmpty(text) == false)
                {
                    if (Globals.SaveToTextFile(filename, text) == false)
                    {
                        MsgHandler.QueueMessage($"CRITICAL - Unable to save data");
                    }
                }
            }
        }

        public static void SaveSettings()
        {
            SaveDataToFile(BotSettings, Globals.SettingsFilename);
        }

        public static void SaveBotData()
        {
            SaveDataToFile(BotData, Globals.BotDataFilename);
        }

        public static void LoadSettingsAndBotData()
        {
            bool settingsChanged = false;
            
            ClientServiceTypes? prevClientService = null;

            if (BotSettings != null && BotSettings.ClientSettings != null)
            {
                prevClientService = BotSettings.ClientSettings.ClientType;
            }

            string settingsText = Globals.ReadFromTextFileOrCreate(Globals.SettingsFilename);
            BotSettings = JsonConvert.DeserializeObject<Settings>(settingsText);

            if (BotSettings == null)
            {
                Console.WriteLine("No settings found; attempting to create file template. If created, please manually fill out the information.");

                BotSettings = new Settings();
                settingsChanged = true;
            }

            if (BotSettings.MainThreadSleep < Globals.MinSleepTime)
            {
                BotSettings.MainThreadSleep = Globals.MinSleepTime;
                Console.WriteLine($"Clamped sleep time to the minimum of {Globals.MinSleepTime}ms!");
                settingsChanged = true;
            }
            else if (BotSettings.MainThreadSleep > Globals.MaxSleepTime)
            {
                BotSettings.MainThreadSleep = Globals.MaxSleepTime;
                Console.WriteLine($"Clamped sleep time to the maximum of {Globals.MinSleepTime}ms!");
                settingsChanged = true;
            }

            //Convert to new message settings
            if (BotSettings.MsgSettings == null)
            {
                BotSettings.MsgSettings = new MessageSettings();
                BotSettings.MsgSettings.PeriodicMessage = BotSettings.PeriodicMessage;
                BotSettings.MsgSettings.ConnectMessage = BotSettings.ConnectMessage;
                BotSettings.MsgSettings.MessageCooldown = BotSettings.MessageCooldown;
                BotSettings.MsgSettings.MessageTime = BotSettings.MessageTime;
                BotSettings.MsgSettings.AutoWhitelistMsg = BotSettings.AutoWhitelistMsg;
                
                Console.WriteLine("Converted some settings to new MessageSettings data!");

                settingsChanged = true;
            }

            if (BotSettings.ClientSettings == null)
            {
                BotSettings.ClientSettings = new ClientSettings();
                settingsChanged = true;
            }

            if (BotSettings.BingoSettings == null)
            {
                BotSettings.BingoSettings = new BingoSettings();
                settingsChanged = true;
            }
            
            //Write only once after checking all the changes
            if (settingsChanged == true)
            {
                SaveSettings();
            }

            //Notify that the client type was changed
            if (prevClientService != null && prevClientService.Value != BotSettings.ClientSettings.ClientType)
            {
                Console.WriteLine("Client service type changed in settings. To apply the change, restart the bot.");
            }

            string dataText = Globals.ReadFromTextFile(Globals.BotDataFilename);
            BotData = JsonConvert.DeserializeObject<BotData>(dataText);

            if (BotData == null)
            {
                Console.WriteLine("Not bot data found; initializing new bot data.");

                BotData = new BotData();
                SaveBotData();
            }

            //Input callbacks
            string inputCBText = Globals.ReadFromTextFileOrCreate(Globals.InputCallbacksFileName);
            InputCBData = JsonConvert.DeserializeObject<InputCallbackData>(inputCBText);

            if (InputCBData == null)
            {
                InputCBData = new InputCallbackData();
                SaveDataToFile(InputCBData, Globals.InputCallbacksFileName);
            }

            //Populate callbacks using the given data
            InputCBData.PopulateCBWithData();

            if (MsgHandler != null)
            {
                MsgHandler.SetMessageCooldown(BotSettings.MsgSettings.MessageCooldown);
            }

            //Re-populate macros
            DataInit.PopulateMacrosToParserList(BotProgram.BotData.Macros, BotProgram.BotData.ParserMacroLookup);

            //string achievementsText = Globals.ReadFromTextFileOrCreate(Globals.AchievementsFilename);
            //BotData.Achievements = JsonConvert.DeserializeObject<AchievementData>(achievementsText);
            //if (BotData.Achievements == null)
            //{
            //    Console.WriteLine("No achievement data found; initializing template.");
            //    BotData.Achievements = new AchievementData();
            //        
            //    //Add an example achievement
            //    BotData.Achievements.AchievementDict.Add("talkative", new Achievement("Talkative",
            //        "Say 500 messages in chat.", AchievementTypes.MsgCount, 500, 1000L)); 
            //        
            //    //Save the achievement template
            //    string text = JsonConvert.SerializeObject(BotData.Achievements, Formatting.Indented);
            //    if (string.IsNullOrEmpty(text) == false)
            //    {
            //        if (Globals.SaveToTextFile(Globals.AchievementsFilename, text) == false)
            //        {
            //            QueueMessage($"CRITICAL - Unable to save achievement data");
            //        }
            //    }
            //}
        }

#endregion

        private class LoginInfo
        {
            public string BotName = string.Empty;
            public string Password = string.Empty;
            public string ChannelName = string.Empty;
        }

        public class MessageSettings
        {
            /// <summary>
            /// The time, in minutes, for outputting the periodic message.
            /// </summary>
            public int MessageTime = 30;

            /// <summary>
            /// The time, in milliseconds, before each queued message will be sent.
            /// This is used as a form of rate limiting.
            /// </summary>
            public double MessageCooldown = 1000d;

            /// <summary>
            /// The message to send when the bot connects to a channel. "{0}" is replaced with the name of the bot and "{1}" is replaced with the command identifier.
            /// </summary>
            /// <para>Set empty to display no message upon connecting.</para>
            public string ConnectMessage = "{0} has connected :D ! Use {1}help to display a list of commands and {1}tutorial to see how to play! Original input parser by Jdog, aka TwitchPlays_Everything, converted to C# & improved by the community.";

            /// <summary>
            /// The message to send when the bot reconnects to chat.
            /// </summary>
            public string ReconnectedMsg = "Successfully reconnected to chat!";

            /// <summary>
            /// The message to send periodically according to <see cref="MessageTime"/>.
            /// "{0}" is replaced with the name of the bot and "{1}" is replaced with the command identifier.
            /// </summary>
            /// <para>Set empty to display no messages in the interval.</para>
            public string PeriodicMessage = "Hi! I'm {0} :D ! Use {1}help to display a list of commands!";

            /// <summary>
            /// The message to send when a user is auto whitelisted. "{0}" is replaced with the name of the user whitelisted.
            /// </summary>
            public string AutoWhitelistMsg = "{0} has been whitelisted! New commands are available.";

            /// <summary>
            /// The message to send when a new user is added to the database.
            /// "{0}" is replaced with the name of the user and "{1}" is replaced with the command identifier.
            /// </summary>
            public string NewUserMsg = "Welcome to the stream, {0} :D ! We hope you enjoy your stay!";

            /// <summary>
            /// The message to send when another channel hosts the one the bot is on.
            /// "{0}" is replaced with the name of the channel hosting the one the bot is on.
            /// </summary>
            public string BeingHostedMsg = "Thank you for hosting, {0}!!";

            /// <summary>
            /// The message to send when a user newly subscribes to the channel.
            /// "{0}" is replaced with the name of the subscriber.
            /// </summary>
            public string NewSubscriberMsg = "Thank you for subscribing, {0} :D !!";

            /// <summary>
            /// The message to send when a user resubscribes to the channel.
            /// "{0}" is replaced with the name of the subscriber and "{1}" is replaced with the number of months subscribed for.
            /// </summary>
            public string ReSubscriberMsg = "Thank you for subscribing for {1} months, {0} :D !!";
        }

        public class ClientSettings
        {
            public ClientServiceTypes ClientType = ClientServiceTypes.Twitch;
        }

        public class BingoSettings
        {
            public bool UseBingo = false;
            public string BingoPipeFilePath = Globals.GetDataFilePath("BingoPipe");
        }

        public class Settings
        {
            public ClientSettings ClientSettings = null;
            public MessageSettings MsgSettings = null;
            public BingoSettings BingoSettings = null;

            /// <summary>
            /// The time, in minutes, for outputting the periodic message.
            /// </summary>
            [Obsolete("Use the value in MsgSettings instead.", false)]
            public int MessageTime = 30;
            [Obsolete("Use the value in MsgSettings instead.", false)]
            public double MessageCooldown = 1000d;
            public double CreditsTime = 2d;
            public long CreditsAmount = 100L;

            /// <summary>
            /// The character limit for bot messages. The default is the client service's character limit (Ex. Twitch).
            /// <para>Some messages that naturally go over this limit will be split into multiple messages.
            /// Examples include listing memes and macros.</para>
            /// </summary>
            public int BotMessageCharLimit = Globals.TwitchCharacterLimit;
            
            /// <summary>
            /// How long to make the main thread sleep after each iteration.
            /// Higher values use less CPU at the expense of delaying queued messages and routines.
            /// </summary>
            public int MainThreadSleep = 100;

            /// <summary>
            /// The message to send when the bot connects to a channel. "{0}" is replaced with the name of the bot and "{1}" is replaced with the command identifier.
            /// </summary>
            /// <para>Set empty to display no message upon connecting.</para>
            [Obsolete("Use the value in MsgSettings instead.", false)]
            public string ConnectMessage = "{0} has connected :D ! Use {1}help to display a list of commands and {1}tutorial to see how to play! Original input parser by Jdog, aka TwitchPlays_Everything, converted to C# & improved by the community.";

            /// <summary>
            /// The message to send periodically according to <see cref="MessageSettings.MessageTime"/>.
            /// "{0}" is replaced with the name of the bot and "{1}" is replaced with the command identifier.
            /// </summary>
            /// <para>Set empty to display no messages in the interval.</para>
            [Obsolete("Use the value in MsgSettings instead.", false)]
            public string PeriodicMessage = "Hi! I'm {0} :D ! Use {1}help to display a list of commands!";

            /// <summary>
            /// If true, automatically whitelists users if conditions are met, including the command count.
            /// </summary>
            public bool AutoWhitelistEnabled = false;

            /// <summary>
            /// The number of valid inputs required to whitelist a user if they're not whitelisted and auto whitelist is enabled.
            /// </summary>
            public int AutoWhitelistInputCount = 20;

            /// <summary>
            /// The message to send when a user is auto whitelisted. "{0}" is replaced with the name of the user whitelisted.
            /// </summary>
            [Obsolete("Use the value in MsgSettings instead.", false)]
            public string AutoWhitelistMsg = "{0} has been whitelisted! New commands are available.";
            
            /// <summary>
            /// If true, will acknowledge that a chat bot is in use and allow interacting with it, provided it's set up.
            /// </summary>
            public bool UseChatBot = false;

            /// <summary>
            /// The name of the file for the chatbot's socket in the data directory.
            /// </summary>
            public string ChatBotSocketFilename = "ChatterBotSocket";
        }
    }
}
