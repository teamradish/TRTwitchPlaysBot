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
        private static readonly object BotDataLockObj = new object();
        private static readonly object SettingsLockObj = new object();

        private static BotProgram instance = null;

        public bool Initialized { get; private set; } = false;

        private LoginInfo LoginInformation = null;
        public static Settings BotSettings { get; private set; } = null;
        public static BotData BotData { get; private set; } = null;

        private TwitchClient Client;
        private ConnectionCredentials Credentials = null;
        private CrashHandler crashHandler = null;

        private CommandHandler CommandHandler = null;
        public static EventHandler EvtHandler { get; private set; } = new EventHandler();

        public static bool TryReconnect { get; private set; } = false;
        public static bool ChannelJoined { get; private set; } = false;

        public bool IsInChannel => (Client?.IsConnected == true && ChannelJoined == true);

        private DateTime CurQueueTime;

        /// <summary>
        /// Queued messages.
        /// </summary>
        private Queue<string> ClientMessages = new Queue<string>();

        private List<BaseRoutine> BotRoutines = new List<BaseRoutine>();

        /// <summary>
        /// Whether to ignore logging bot messages to the console based on potential console logs from the <see cref="ExecCommand"/>.
        /// </summary>
        public static bool IgnoreConsoleLog = false;

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

            for (int i = 0; i < BotRoutines.Count; i++)
            {
                BotRoutines[i].CleanUp(Client);
            }

            CommandHandler.CleanUp();
            EvtHandler.CleanUp(Client);

            ClientMessages.Clear();

            if (Client.IsConnected == true)
                Client.Disconnect();

            //Clean up and relinquish the devices when we're done
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
                    BotData.Users.Add(botName, botUser);

                    SaveBotData();
                }
            }

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

            Client = new TwitchClient();
            Client.Initialize(Credentials, LoginInformation.ChannelName, Globals.CommandIdentifier, Globals.CommandIdentifier, true);
            Client.OverrideBeingHostedCheck = true;

            UnsubscribeEvents();

            Client.OnJoinedChannel += OnJoinedChannel;
            Client.OnMessageReceived += OnMessageReceived;
            Client.OnWhisperReceived += OnWhisperReceived;
            Client.OnNewSubscriber += OnNewSubscriber;
            Client.OnReSubscriber += OnReSubscriber;
            Client.OnChatCommandReceived += OnChatCommandReceived;
            Client.OnBeingHosted += OnBeingHosted;
            
            Client.OnConnected += OnConnected;
            Client.OnConnectionError += OnConnectionError;
            Client.OnReconnected += OnReconnected;
            Client.OnDisconnected += OnDisconnected;

            AddRoutine(new PeriodicMessageRoutine());
            AddRoutine(new CreditsGiveRoutine());
            AddRoutine(new ReconnectRoutine());
            AddRoutine(new ChatBotResponseRoutine());

            //Initialize controller input - validate the controller type first
            if (InputGlobals.IsVControllerSupported((InputGlobals.VControllerTypes)BotData.LastVControllerType) == false)
            {
                BotData.LastVControllerType = (int)InputGlobals.GetDefaultSupportedVControllerType();
            }

            InputGlobals.VControllerTypes vCType = (InputGlobals.VControllerTypes)BotData.LastVControllerType;
            Console.WriteLine($"Setting up virtual controller {vCType}");
            
            InputGlobals.SetVirtualController(vCType);

            Initialized = true;
        }

        public void Run()
        {
            if (Client.IsConnected == true)
            {
                Console.WriteLine("Client is already connected and running!");
                return;
            }

            Client.Connect();

            //Run
            while (true)
            {
                DateTime now = DateTime.Now;

                TimeSpan queueDiff = now - CurQueueTime;

                //Queued messages
                if (ClientMessages.Count > 0 && queueDiff.TotalMilliseconds >= BotSettings.MessageCooldown)
                {
                    if (IsInChannel == true)
                    {
                        string message = ClientMessages.Dequeue();

                        //There's a chance the bot could be disconnected from the channel between the conditional and now
                        try
                        {
                            //Send the message
                            Client.SendMessage(LoginInformation.ChannelName, message);
                        }
                        catch (TwitchLib.Client.Exceptions.BadStateException e)
                        {
                            Console.WriteLine($"Could not send message due to bad state: {e.Message}");
                        }

                        if (IgnoreConsoleLog == false)
                        {
                            Console.WriteLine(message);
                        }

                        CurQueueTime = now;
                    }
                }

                //Update routines
                for (int i = 0; i < BotRoutines.Count; i++)
                {
                    if (BotRoutines[i] == null)
                    {
                        Console.WriteLine($"NULL BOT ROUTINE AT {i} SOMEHOW!!");
                        continue;
                    }

                    BotRoutines[i].UpdateRoutine(Client, now);
                }

                Thread.Sleep(BotSettings.MainThreadSleep);
            }
        }

        public static void QueueMessage(string message)
        {
            if (string.IsNullOrEmpty(message) == false)
            {
                instance.ClientMessages.Enqueue(message);
            }
        }

        public static void AddRoutine(BaseRoutine routine)
        {
            routine.Initialize(instance.Client);
            instance.BotRoutines.Add(routine);
        }

        public static void RemoveRoutine(BaseRoutine routine)
        {
            routine.CleanUp(instance.Client);
            instance.BotRoutines.Remove(routine);
        }

        public static BaseRoutine FindRoutine<T>()
        {
            return instance.BotRoutines.Find((routine) => routine is T);
        }

        private void UnsubscribeEvents()
        {
            Client.OnJoinedChannel -= OnJoinedChannel;
            Client.OnMessageReceived -= OnMessageReceived;
            Client.OnWhisperReceived -= OnWhisperReceived;
            Client.OnNewSubscriber -= OnNewSubscriber;
            Client.OnReSubscriber -= OnReSubscriber;
            Client.OnChatCommandReceived -= OnChatCommandReceived;
            Client.OnConnected -= OnConnected;
            Client.OnConnectionError -= OnConnectionError;
            Client.OnReconnected += OnReconnected;
            Client.OnDisconnected -= OnDisconnected;
            Client.OnBeingHosted -= OnBeingHosted;
        }

#region Events

        private void OnConnected(object sender, OnConnectedArgs e)
        {
            TryReconnect = false;
            ChannelJoined = false;

            Console.WriteLine($"{LoginInformation.BotName} connected!");
        }

        private void OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            ChannelJoined = false;

            if (TryReconnect == false)
            {
                Console.WriteLine($"Failed to connect: {e.Error.Message}");

                TryReconnect = true;
            }
        }

        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            if (string.IsNullOrEmpty(BotSettings.ConnectMessage) == false)
            {
                string finalMsg = BotSettings.ConnectMessage.Replace("{0}", LoginInformation.BotName).Replace("{1}", Globals.CommandIdentifier.ToString());
                QueueMessage(finalMsg);
            }

            Console.WriteLine($"Joined channel \"{e.Channel}\"");

            TryReconnect = false;
            ChannelJoined = true;

            if (CommandHandler == null)
            {
                CommandHandler = new CommandHandler(Client);
            }
        }

        private void OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            CommandHandler.HandleCommand(sender, e);
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            User userData = GetOrAddUser(e.ChatMessage.Username, false);

            if (userData.OptedOut == false)
            {
                userData.IncrementMsgCount();
            }

            string possibleMeme = e.ChatMessage.Message.ToLower();
            if (BotProgram.BotData.Memes.TryGetValue(possibleMeme, out string meme) == true)
            {
                BotProgram.QueueMessage(meme);
            }
            else
            {
                //Ignore commands as inputs
                if (possibleMeme.StartsWith(Globals.CommandIdentifier) == true)
                {
                    return;
                }

                //If there are no valid inputs, don't attempt to parse
                if (InputGlobals.CurrentConsole.ValidInputs == null || InputGlobals.CurrentConsole.ValidInputs.Length == 0)
                {
                    return;
                }

                //Parser.InputSequence inputSequence = default;
                //(bool, List<List<Parser.Input>>, bool, int) parsedVal = default;
                Parser.InputSequence inputSequence = default;

                try
                {
                    string parse_message = Parser.Expandify(Parser.PopulateMacros(e.ChatMessage.Message));

                    inputSequence = Parser.ParseInputs(parse_message);

                    //parsedVal = Parser.Parse(parse_message);
                    //Console.WriteLine(inputSequence.ToString());
                }
                catch (Exception exception)
                {
                    //Kimimaru: Sanitize parsing exceptions
                    //Most of these are currently caused by differences in how C# and Python handle slicing strings (Substring() vs string[:])
                    //One example that throws this that shouldn't is "#mash(w234"
                    //BotProgram.QueueMessage($"ERROR: {exception.Message}");
                    inputSequence.InputValidationType = Parser.InputValidationTypes.Invalid;
                    //parsedVal.Item1 = false;
                }

                //Check for non-valid messages
                if (inputSequence.InputValidationType != Parser.InputValidationTypes.Valid)
                {
                    //Display error message for invalid inputs
                    if (inputSequence.InputValidationType == Parser.InputValidationTypes.Invalid)
                    {
                        BotProgram.QueueMessage(inputSequence.Error);
                    }
                }
                //It's a valid message, so process it
                else
                {
                    //Ignore if user is silenced
                    if (userData.Silenced == true)
                    {
                        return;
                    }

                    //Ignore based on user level and permissions
                    if (userData.Level < BotProgram.BotData.InputPermissions)
                    {
                        BotProgram.QueueMessage($"Inputs are restricted to levels {(AccessLevels.Levels)BotProgram.BotData.InputPermissions} and above");
                        return;
                    }

                    #region Parser Post-Process Validation
                    
                    //All this validation is very slow - find a way to speed it up, ideally without integrating it directly into the parser

                    //Kimimaru: Max pause validation is temporarily removed since it should be redundant with the new combo validation
                    //The primary purpose of it was to prevent players from resetting games with button combos, which the combo validation handles
                     
                    //if (ParserPostProcess.IsValidPauseInputDuration(inputSequence.Inputs, "start", BotData.MaxPauseHoldDuration) == false)
                    //{
                    //    BotProgram.QueueMessage($"Invalid input: Pause button held for longer than the max duration of {BotData.MaxPauseHoldDuration} milliseconds!");
                    //    return;
                    //}

                    //Check if the user has permission to perform all the inputs they attempted
                    ParserPostProcess.InputValidation inputValidation = ParserPostProcess.CheckInputPermissions(userData.Level, inputSequence.Inputs, BotData.InputAccess.InputAccessDict);

                    //If the input isn't valid, exit
                    if (inputValidation.IsValid == false)
                    {
                        if (string.IsNullOrEmpty(inputValidation.Message) == false)
                        {
                            QueueMessage(inputValidation.Message);
                        }

                        return;
                    }

                    //Lastly, check for invalid button combos given the current console
                    if (BotData.InvalidBtnCombos.InvalidCombos.TryGetValue((int)InputGlobals.CurrentConsoleVal, out List<string> invalidCombos) == true)
                    {
                        if (ParserPostProcess.ValidateButtonCombos(inputSequence.Inputs, invalidCombos, userData.Team) == false)
                        {
                            string msg = "Invalid input: buttons ({0}) are not allowed to be pressed at the same time.";
                            string combos = string.Empty;
                            
                            for (int i = 0; i < invalidCombos.Count; i++)
                            {
                                combos += "\"" + invalidCombos[i] + "\"";
                                
                                if (i < (invalidCombos.Count - 1))
                                {
                                    combos += ", ";
                                }
                            }
                            
                            msg = string.Format(msg, combos);
                            BotProgram.QueueMessage(msg);
                            
                            return;
                        }
                    }

                    #endregion

                    if (InputHandler.StopRunningInputs == false)
                    {
                        //Mark this as a valid input
                        if (userData.OptedOut == false)
                        {
                            userData.IncrementValidInputCount();
                        }

                        EvtHandler.InvokeUserMadeInputEvent(userData, inputSequence);

                        bool shouldPerformInput = true;

                        //Check the team the user is on for the controller they should be using
                        //Validate that the controller is acquired and exists
                        int controllerNum = userData.Team;

                        if (controllerNum < 0 || controllerNum >= InputGlobals.ControllerMngr.ControllerCount)
                        {
                            BotProgram.QueueMessage($"ERROR: Invalid joystick number {controllerNum + 1}. # of joysticks: {InputGlobals.ControllerMngr.ControllerCount}. Please change your controller port to a valid number to perform inputs.");
                            shouldPerformInput = false;
                        }
                        //Now verify that the controller has been acquired
                        else if (InputGlobals.ControllerMngr.GetController(controllerNum).IsAcquired == false)
                        {
                            BotProgram.QueueMessage($"ERROR: Joystick number {controllerNum + 1} with controller ID of {InputGlobals.ControllerMngr.GetController(controllerNum).ControllerID} has not been acquired! Ensure you (the streamer) have a virtual device set up at this ID.");
                            shouldPerformInput = false;
                        }

                        //We're okay to perform the input
                        if (shouldPerformInput == true)
                        {
                            InputHandler.CarryOutInput(inputSequence.Inputs, controllerNum);

                            //If auto whitelist is enabled, the user reached the whitelist message threshold,
                            //the user isn't whitelisted, and the user hasn't ever been whitelisted, whitelist them
                            if (BotSettings.AutoWhitelistEnabled == true && userData.Level < (int)AccessLevels.Levels.Whitelisted
                                && userData.AutoWhitelisted == false && userData.ValidInputs >= BotSettings.AutoWhitelistInputCount)
                            {
                                userData.Level = (int)AccessLevels.Levels.Whitelisted;
                                userData.SetAutoWhitelist(true);

                                if (string.IsNullOrEmpty(BotSettings.AutoWhitelistMsg) == false)
                                {
                                    //Replace the user's name with the message
                                    string msg = BotSettings.AutoWhitelistMsg.Replace("{0}", e.ChatMessage.Username);
                                    QueueMessage(msg);
                                }
                            }
                        }
                    }
                    else
                    {
                        QueueMessage("New inputs cannot be processed until all other inputs have stopped.");
                    }
                }
            }

            //Kimimaru: For testing this will work, but we shouldn't save after each message
            SaveBotData();
        }

        private void OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            
        }

        private void OnBeingHosted(object sender, OnBeingHostedArgs e)
        {
            QueueMessage($"Thank you for hosting, {e.BeingHostedNotification.HostedByChannel}!!");
        }

        private void OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            QueueMessage($"Thank you for subscribing, {e.Subscriber.DisplayName} :D !!");
        }

        private void OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            QueueMessage($"Thank you for subscribing for {e.ReSubscriber.Months} months, {e.ReSubscriber.DisplayName} :D !!");
        }

        private void OnReconnected(object sender, OnReconnectedEventArgs e)
        {
            QueueMessage("Successfully reconnected to chat!");

            TryReconnect = false;
        }

        private void OnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            Console.WriteLine("Disconnected!");

            TryReconnect = true;
        }

        public static User GetUser(string username, bool isLower = true)
        {
            if (isLower == false)
            {
                username = username.ToLowerInvariant();
            }

            User userData = null;

            BotData.Users.TryGetValue(username, out userData);

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
                BotData.Users.Add(username, userData);

                BotProgram.QueueMessage($"Welcome to the stream, {origName} :D ! We hope you enjoy your stay!");
            }

            return userData;
        }

        public static void SaveSettings()
        {
            //Make sure more than one thread doesn't try to save at the same time to prevent potential loss of data and access violations
            lock (SettingsLockObj)
            {
                string text = JsonConvert.SerializeObject(BotSettings, Formatting.Indented);
                if (string.IsNullOrEmpty(text) == false)
                {
                    if (Globals.SaveToTextFile(Globals.SettingsFilename, text) == false)
                    {
                        QueueMessage($"CRITICAL - Unable to save settings");
                    }
                }
            }
        }

        public static void SaveBotData()
        {
            //Make sure more than one thread doesn't try to save at the same time to prevent potential loss of data and access violations
            lock (BotDataLockObj)
            {
                string text = JsonConvert.SerializeObject(BotData, Formatting.Indented);
                if (string.IsNullOrEmpty(text) == false)
                {
                    if (Globals.SaveToTextFile(Globals.BotDataFilename, text) == false)
                    {
                        QueueMessage($"CRITICAL - Unable to save bot data");
                    }
                }
            }
        }

        public static void LoadSettingsAndBotData()
        {
            bool settingsChanged = false;
            
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
            
            //Write only once after checking all the changes
            if (settingsChanged == true)
            {
                SaveSettings();
            }

            string dataText = Globals.ReadFromTextFile(Globals.BotDataFilename);
            BotData = JsonConvert.DeserializeObject<BotData>(dataText);

            if (BotData == null)
            {
                Console.WriteLine("Not bot data found; initializing new bot data.");

                BotData = new BotData();
                SaveBotData();
            }

            //string achievementsText = Globals.ReadFromTextFileOrCreate(Globals.AchievementsFilename);
            //BotData.Achievements = JsonConvert.DeserializeObject<AchievementData>(achievementsText);
            //if (BotData.Achievements == null)
            //{
            //    Console.WriteLine("No achievement data found; initializing template.");
            //    BotData.Achievements = new AchievementData();

            //    //Add an example achievement
            //    BotData.Achievements.AchievementDict.Add("talkative", new Achievement("Talkative",
            //        "Say 500 messages in chat.", AchievementTypes.MsgCount, 500, 1000L)); 

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

        public class Settings
        {
            public int MessageTime = 30;
            public double MessageCooldown = 1000d;
            public double CreditsTime = 2d;
            public long CreditsAmount = 100L;
            
            /// <summary>
            /// How long to make the main thread sleep after each iteration.
            /// Higher values use less CPU at the expense of delaying queued messages and routines.
            /// </summary>
            public int MainThreadSleep = 100;

            /// <summary>
            /// The message to send when the bot connects to a channel. "{0}" is replaced with the name of the bot and "{1}" is replaced with the command identifier.
            /// </summary>
            public string ConnectMessage = "{0} has connected :D ! Use {1}help to display a list of commands and {1}tutorial to see how to play! Original input parser by Jdog, aka TwitchPlays_Everything, converted to C# & improved by the community.";

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
            public string AutoWhitelistMsg = "{0} has been whitelisted! New commands are available.";
            
            /// <summary>
            /// If true, will acknowledge that a chat bot is in use and allow interacting with it, provided it's set up.
            /// </summary>
            public bool UseChatBot = false;
        }
    }
}
