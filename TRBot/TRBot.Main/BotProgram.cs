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
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TRBot.Parsing;
using TRBot.Connection;
using TRBot.Consoles;
using TRBot.VirtualControllers;
using TRBot.Misc;
using TRBot.Utilities;
using TRBot.Data;
using TRBot.Commands;
using TRBot.Permissions;
using TRBot.Routines;
using Newtonsoft.Json;
using TwitchLib;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Client.Events;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Interfaces;

namespace TRBot.Main
{
    public sealed class BotProgram : IDisposable
    {
        public bool Initialized { get; private set; } = false;

        private IClientService ClientService = null;

        private Parser InputParser = null;
        private CommandHandler CmdHandler = null;
        
        private BotMessageHandler MsgHandler = new BotMessageHandler();
        private BotRoutineHandler RoutineHandler = new BotRoutineHandler();
        private DataReloader DataReloader = new DataReloader();
        private DataContainer DataContainer = new DataContainer();

        private CrashHandler crashHandler = null;

        //Store the function to reduce garbage, since this one is being called constantly
        private Func<Settings, bool> ThreadSleepFindFunc = null;

        public BotProgram()
        {
            crashHandler = new CrashHandler();

            //Below normal priority
            //NOTE: Have a setting to set the priority
            Process thisProcess = Process.GetCurrentProcess();
            thisProcess.PriorityBoostEnabled = false;
            thisProcess.PriorityClass = ProcessPriorityClass.BelowNormal;

            ThreadSleepFindFunc = FindThreadSleepTime;

            //Call this to set the application start time
            DateTime start = Application.ApplicationStartTimeUTC;
        }

        //Clean up anything we need to here
        public void Dispose()
        {
            if (Initialized == false)
                return;

            UnsubscribeEvents();

            ClientService?.CleanUp();

            MsgHandler?.CleanUp();
            RoutineHandler?.CleanUp();
            CmdHandler?.CleanUp();
            DataReloader?.CleanUp();

            if (ClientService?.IsConnected == true)
                ClientService.Disconnect();

            //Dispose and relinquish the virtual controllers when we're done
            DataContainer?.ControllerMngr?.Dispose();
        }

        public void Initialize()
        {
            if (Initialized == true)
                return;

            //Use invariant culture
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            InputParser = new Parser();

            //Initialize database
            string databasePath = Path.Combine(DataConstants.DataFolderPath, DataConstants.DATABASE_FILE_NAME);

            Console.WriteLine($"Validating database at: {databasePath}");
            if (Utilities.FileHelpers.ValidatePathForFile(databasePath) == false)
            {
                Console.WriteLine($"Cannot create database path at {databasePath}. Check if you have permission to write to this directory. Aborting.");
                return;
            }

            Console.WriteLine("Database path validated! Initializing database and importing migrations.");

            DatabaseManager.SetDatabasePath(databasePath);
            DatabaseManager.InitAndMigrateContext();
            
            Console.WriteLine("Checking to initialize default values for missing database entries.");

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Check for and initialize default values if the database was newly created or needs updating
                int addedDefaultEntries = DataHelper.InitDefaultData(context);

                if (addedDefaultEntries > 0)
                {
                    context.SaveChanges();
                    Console.WriteLine($"Added {addedDefaultEntries} additional entries to the database.");
                }
            }

            Console.WriteLine("Initializing client service");

            //Initialize client service
            InitClientService();

            //If the client service doesn't exist, we can't continue
            if (ClientService == null)
            {
                Console.WriteLine("Client service failed to initialize; please check your settings. Aborting.");
                return;
            }

            //Set client service and message cooldown
            MsgHandler.SetClientService(ClientService);

            long msgCooldown = DataHelper.GetSettingInt(SettingsConstants.MESSAGE_COOLDOWN, 1000L);
            MsgHandler.SetMessageCooldown(msgCooldown);

            //Subscribe to events
            UnsubscribeEvents();
            SubscribeEvents();

            DataContainer.SetMessageHandler(MsgHandler);
            DataContainer.SetDataReloader(DataReloader);

            Console.WriteLine("Setting up virtual controller manager.");

            VirtualControllerTypes lastVControllerType = (VirtualControllerTypes)DataHelper.GetSettingInt(SettingsConstants.LAST_VCONTROLLER_TYPE, 0L);

            VirtualControllerTypes curVControllerType = VControllerHelper.ValidateVirtualControllerType(lastVControllerType, TRBotOSPlatform.CurrentOS);

            //Show a message saying the previous value wasn't supported and save the changes
            if (VControllerHelper.IsVControllerSupported(lastVControllerType, TRBotOSPlatform.CurrentOS) == false)
            {
                MsgHandler.QueueMessage($"Current virtual controller {lastVControllerType} is not supported by the {TRBotOSPlatform.CurrentOS} platform. Switched it to the default of {curVControllerType} for this platform.");                

                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    Settings lastVControllerSetting = DataHelper.GetSettingNoOpen(SettingsConstants.LAST_VCONTROLLER_TYPE, context);
                    lastVControllerSetting.ValueInt = (long)curVControllerType;
                    context.SaveChanges();
                }
            }

            DataContainer.SetCurVControllerType(curVControllerType);

            IVirtualControllerManager controllerMngr = VControllerHelper.GetVControllerMngrForType(curVControllerType);

            DataContainer.SetControllerManager(controllerMngr);

            int controllerCount = 0;

            //Clamp the controller count to the min and max allowed by the virtual controller manager
            using (BotDBContext context = DatabaseManager.OpenContext())
            { 
                Settings joystickCountSetting = DataHelper.GetSettingNoOpen(SettingsConstants.JOYSTICK_COUNT, context);

                int minCount = DataContainer.ControllerMngr.MinControllers;
                int maxCount = DataContainer.ControllerMngr.MaxControllers;

                //Validate controller count
                if (joystickCountSetting.ValueInt < minCount)
                {
                    MsgHandler.QueueMessage($"Controller count of {joystickCountSetting.ValueInt} in database is invalid. Clamping to the min of {minCount}.");
                    joystickCountSetting.ValueInt = minCount;
                    context.SaveChanges();
                }
                else if (joystickCountSetting.ValueInt > maxCount)
                {
                    MsgHandler.QueueMessage($"Controller count of {joystickCountSetting.ValueInt} in database is invalid. Clamping to the max of {maxCount}.");
                    joystickCountSetting.ValueInt = maxCount;
                    context.SaveChanges();
                }

                controllerCount = (int)joystickCountSetting.ValueInt;
            }

            DataContainer.ControllerMngr.Initialize();
            int acquiredCount = DataContainer.ControllerMngr.InitControllers(controllerCount);

            Console.WriteLine($"Setting up virtual controller {curVControllerType} and acquired {acquiredCount} controllers!");

            CmdHandler = new CommandHandler();
            CmdHandler.Initialize(DataContainer, RoutineHandler);

            DataReloader.SoftDataReloadedEvent -= OnSoftReload;
            DataReloader.SoftDataReloadedEvent += OnSoftReload;

            DataReloader.HardDataReloadedEvent -= OnHardReload;
            DataReloader.HardDataReloadedEvent += OnHardReload;

            //Initialize routines
            InitRoutines();

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

                //Update queued messages
                MsgHandler.Update(utcNow);

                //Update routines
                RoutineHandler.Update(utcNow);

                int threadSleep = 500;

                //Get the thread sleep value from the database
                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    Settings threadSleepSetting = context.SettingCollection.FirstOrDefault(ThreadSleepFindFunc);

                    //Clamp to avoid an exception - the code that changes this should handle values below 0,
                    //but the database can manually be changed to have lower values 
                    threadSleep = Math.Clamp((int)threadSleepSetting.ValueInt, 0, int.MaxValue);
                }

                Thread.Sleep(threadSleep);
            }
        }

        private void UnsubscribeEvents()
        {
            ClientService.EventHandler.UserSentMessageEvent -= OnUserSentMessage;
            //ClientService.EventHandler.UserMadeInputEvent -= OnUserMadeInput;
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
            //ClientService.EventHandler.UserMadeInputEvent += OnUserMadeInput;
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

        private void InitRoutines()
        {
            RoutineHandler.SetDataContainer(DataContainer);

            RoutineHandler.AddRoutine(new CreditsGiveRoutine());
            RoutineHandler.AddRoutine(new PeriodicMessageRoutine());
            RoutineHandler.AddRoutine(new ReconnectRoutine());

            //Add the periodic input routine if it's enabled
            long periodicInputEnabled = DataHelper.GetSettingInt(SettingsConstants.PERIODIC_INPUT_ENABLED, 0L);
            if (periodicInputEnabled > 0L)
            {
                RoutineHandler.AddRoutine(new PeriodicInputRoutine());
            }
        }

#region Events

        private void OnConnected(EvtConnectedArgs e)
        {
            Console.WriteLine($"Bot connected!");
        }

        private void OnConnectionError(EvtConnectionErrorArgs e)
        {
            MsgHandler.QueueMessage($"Failed to connect: {e.Error.Message}");
        }

        private void OnJoinedChannel(EvtJoinedChannelArgs e)
        {
            Console.WriteLine($"Joined channel \"{e.Channel}\"");

            MsgHandler.SetChannelName(e.Channel);

            string connectMessage = DataHelper.GetSettingString(SettingsConstants.CONNECT_MESSAGE, "Connected!");
            MsgHandler.QueueMessage(connectMessage);
        }

        private void OnChatCommandReceived(EvtChatCommandArgs e)
        {
            //MsgHandler.QueueMessage($"Received command \"{e.Command.CommandText}\"");

            try
            {
                CmdHandler.HandleCommand(e);
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Ran into exception on command {e.Command.CommandText}: {exc.Message}\n{exc.StackTrace}"); 
            }
        }

        private void OnUserSentMessage(EvtUserMessageArgs e)
        {
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Look for a user with this name
                User user = null;
                string username = e.UsrMessage.Username;

                if (string.IsNullOrEmpty(username) == false)
                {
                    user = DataHelper.GetOrAddUserNoOpen(username, context, out bool added);

                    if (added == true)
                    {
                        //Get the new user message and replace the variable with their name
                        string newUserMessage = DataHelper.GetSettingStringNoOpen(SettingsConstants.NEW_USER_MESSAGE, context, $"Welcome to the stream, {username}!");
                        newUserMessage = newUserMessage.Replace("{0}", username);
                        
                        MsgHandler.QueueMessage(newUserMessage);
                    }

                    //Increment message count
                    if (user.IsOptedOut == false)
                    {
                        user.Stats.TotalMessageCount++;

                        context.SaveChanges();
                    }
                }

                //Check for memes if the user isn't ignoring them
                if (user.Stats.IgnoreMemes == 0)
                {
                    string possibleMeme = e.UsrMessage.Message.ToLower();
                    Meme meme = context.Memes.FirstOrDefault((m) => m.MemeName == possibleMeme);
                    if (meme != null)
                    {
                        MsgHandler.QueueMessage(meme.MemeValue);
                    }
                }
            }

            ProcessMsgAsInput(e);
        }

        private void OnWhisperReceived(EvtWhisperMessageArgs e)
        {
            
        }

        private void OnBeingHosted(EvtOnHostedArgs e)
        {
            string hostedMessage = DataHelper.GetSettingString(SettingsConstants.BEING_HOSTED_MESSAGE, string.Empty);

            if (string.IsNullOrEmpty(hostedMessage) == false)
            {
                string finalMsg = hostedMessage.Replace("{0}", e.HostedData.HostedByChannel);
                MsgHandler.QueueMessage(finalMsg);
            }
        }

        private void OnNewSubscriber(EvtOnSubscriptionArgs e)
        {
            string newSubscriberMessage = DataHelper.GetSettingString(SettingsConstants.NEW_SUBSCRIBER_MESSAGE, string.Empty);

            if (string.IsNullOrEmpty(newSubscriberMessage) == false)
            {
                string finalMsg = newSubscriberMessage.Replace("{0}", e.SubscriptionData.DisplayName);
                MsgHandler.QueueMessage(finalMsg);
            }
        }

        private void OnReSubscriber(EvtOnReSubscriptionArgs e)
        {
            string resubscriberMessage = DataHelper.GetSettingString(SettingsConstants.RESUBSCRIBER_MESSAGE, string.Empty);

            if (string.IsNullOrEmpty(resubscriberMessage) == false)
            {
                string finalMsg = resubscriberMessage.Replace("{0}", e.ReSubscriptionData.DisplayName).Replace("{1}", e.ReSubscriptionData.Months.ToString());
                MsgHandler.QueueMessage(finalMsg);
            }
        }

        private void OnReconnected(EvtReconnectedArgs e)
        {
            string reconnectedMessage = DataHelper.GetSettingString(SettingsConstants.RECONNECTED_MESSAGE, string.Empty);

            if (string.IsNullOrEmpty(reconnectedMessage) == false)
            {
                MsgHandler.QueueMessage(reconnectedMessage);
            }
        }

        private void OnDisconnected(EvtDisconnectedArgs e)
        {
            Console.WriteLine("Bot disconnected! Please check your internet connection.");
        }

        private void ProcessMsgAsInput(EvtUserMessageArgs e)
        {
            //Ignore commands as inputs
            if (e.UsrMessage.Message.StartsWith(DataConstants.COMMAND_IDENTIFIER) == true)
            {
                return;
            }

            GameConsole usedConsole = null;

            int lastConsoleID = 1;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                lastConsoleID = (int)DataHelper.GetSettingIntNoOpen(SettingsConstants.LAST_CONSOLE, context, 1L);
                GameConsole lastConsole = context.Consoles.FirstOrDefault(c => c.ID == lastConsoleID);

                if (lastConsole != null)
                {
                    //Create a new console using data from the database
                    usedConsole = new GameConsole(lastConsole.Name, lastConsole.InputList, lastConsole.InvalidCombos);
                }
            }

            //If there are no valid inputs, don't attempt to parse
            if (usedConsole == null)
            {
                MsgHandler.QueueMessage($"The current console does not point to valid data. Please set a different console to use, or if none are available, add one.");
                return;
            }

            if (usedConsole.ConsoleInputs.Count == 0)
            {
                MsgHandler.QueueMessage($"The current console, \"{usedConsole.Name}\", does not have any available inputs.");
            }

            ParsedInputSequence inputSequence = default;

            try
            {
                int defaultDur = 200;
                int maxDur = 60000;
                int defaultPort = 0;

                string regexStr = usedConsole.InputRegex;

                string readyMessage = string.Empty;
                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    //Get default and max input durations
                    //Use user overrides if they exist, otherwise use the global values
                    User user = DataHelper.GetUserNoOpen(e.UsrMessage.Username, context);

                    //Get default controller port
                    defaultPort = (int)user.ControllerPort;

                    defaultDur = (int)DataHelper.GetUserOrGlobalDefaultInputDur(user, context);
                    maxDur = (int)DataHelper.GetUserOrGlobalMaxInputDur(user, context);

                    //Console.WriteLine($"Default dur: {defaultDur} | Max dur: {maxDur}");

                    //Get input synonyms for this console
                    IQueryable<InputSynonym> synonyms = context.InputSynonyms.Where(syn => syn.ConsoleID == lastConsoleID);

                    //Prepare the message for parsing
                    readyMessage = InputParser.PrepParse(e.UsrMessage.Message, context.Macros, synonyms);
                }

                //Parse inputs to get our parsed input sequence
                inputSequence = InputParser.ParseInputs(readyMessage, regexStr, new ParserOptions(defaultPort, defaultDur, true, maxDur));
                //Console.WriteLine(inputSequence.ToString());
                //Console.WriteLine("\nReverse Parsed: " + ReverseParser.ReverseParse(inputSequence));
                //Console.WriteLine("\nReverse Parsed Natural:\n" + ReverseParser.ReverseParseNatural(inputSequence));
            }
            catch (Exception exception)
            {
                string excMsg = exception.Message;

                //Handle parsing exceptions
                MsgHandler.QueueMessage($"ERROR: {excMsg} | {exception.StackTrace}");
                inputSequence.ParsedInputResult = ParsedInputResults.Invalid;
            }

            //Check for non-valid messages
            if (inputSequence.ParsedInputResult != ParsedInputResults.Valid)
            {
                //Display error message for invalid inputs
                if (inputSequence.ParsedInputResult == ParsedInputResults.Invalid)
                {
                    MsgHandler.QueueMessage(inputSequence.Error);
                }

                return;
            }

            #region Parser Post-Process Validation
            
            /* All this validation may be able to be performed faster.
             * Find a way to speed it up.
             */

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetOrAddUserNoOpen(e.UsrMessage.Username, context, out bool added);
                
                //Check if the user is silenced and ignore the message if so
                if (user.HasEnabledAbility(PermissionConstants.SILENCED_ABILITY) == true)
                {
                    return;
                }

                long globalInputPermLevel = DataHelper.GetSettingIntNoOpen(SettingsConstants.GLOBAL_INPUT_LEVEL, context, 0L);

                //Ignore based on user level and permissions
                if (user.Level < globalInputPermLevel)
                {
                    MsgHandler.QueueMessage($"Inputs are restricted to levels {(PermissionLevels)globalInputPermLevel} and above.");
                    return;
                }

                //Check for restricted inputs on this user
                InputValidation validation = ParserPostProcess.InputSequenceContainsRestrictedInputs(inputSequence, user.GetRestrictedInputs());

                if (validation.InputValidationType != InputValidationTypes.Valid)
                {
                    if (string.IsNullOrEmpty(validation.Message) == false)
                    {
                        MsgHandler.QueueMessage(validation.Message);
                    }
                    return;
                }

                //Check for invalid input combinations
                validation = ParserPostProcess.ValidateInputCombos(inputSequence, usedConsole.InvalidCombos, DataContainer.ControllerMngr, usedConsole);

                if (validation.InputValidationType != InputValidationTypes.Valid)
                {
                    if (string.IsNullOrEmpty(validation.Message) == false)
                    {
                        MsgHandler.QueueMessage(validation.Message);
                    }
                    return;
                }

                //Check for level permissions and ports
                validation = ParserPostProcess.ValidateInputLvlPermsAndPorts(user.Level, inputSequence,
                    DataContainer.ControllerMngr, usedConsole.ConsoleInputs);

                if (validation.InputValidationType != InputValidationTypes.Valid)
                {
                    if (string.IsNullOrEmpty(validation.Message) == false)
                    {
                        MsgHandler.QueueMessage(validation.Message);
                    }
                    return;
                }
            }

            #endregion

            //Make sure inputs aren't stopped
            if (InputHandler.InputsHalted == true)
            {
                //We can't process inputs because they're currently stopped
                MsgHandler.QueueMessage("New inputs cannot be processed until all other inputs have stopped.");
                return;
            }
                
            //It's a valid input - save it in the user's stats
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetOrAddUserNoOpen(e.UsrMessage.Username, context, out bool added);
                
                //Ignore if the user is opted out
                if (user.IsOptedOut == false)
                {
                    user.Stats.ValidInputCount++;
                    
                    //Check for auto promote is enabled and auto promote if applicable
                    if (user.Stats.AutoPromoted == 0)
                    {
                        long autoPromoteEnabled = DataHelper.GetSettingIntNoOpen(SettingsConstants.AUTO_PROMOTE_ENABLED, context, 0L);
                        
                        //Check if autopromote is enabled
                        if (autoPromoteEnabled > 0)
                        {
                            long autoPromoteInputReq = DataHelper.GetSettingIntNoOpen(SettingsConstants.AUTO_PROMOTE_INPUT_REQ, context, long.MaxValue);
                            
                            //Check if the user reached the autopromote input count requirement
                            if (user.Stats.ValidInputCount >= autoPromoteInputReq)
                            {
                                long autoPromoteLevel = DataHelper.GetSettingIntNoOpen(SettingsConstants.AUTO_PROMOTE_LEVEL, context, -1L);
                                
                                //Only autopromote if this is a valid permission level
                                //We may not want to log or send a message for this, as it has potential to be very spammy,
                                //and it's not something the users can control
                                if (PermissionHelpers.IsValidPermissionValue(autoPromoteLevel) == true)
                                {
                                    long prevLevel = user.Level;

                                    //Mention that the user was autopromoted
                                    user.Stats.AutoPromoted = 1;   

                                    //If the user is already at or above this level, don't set them to it
                                    //Only set if the user is below
                                    if (user.Level < autoPromoteLevel)
                                    {
                                        //Adjust abilities and promote to the new level
                                        DataHelper.AdjustUserAbilitiesOnLevel(user, autoPromoteLevel, context);

                                        user.Level = autoPromoteLevel;

                                        string autoPromoteMsg = DataHelper.GetSettingStringNoOpen(SettingsConstants.AUTOPROMOTE_MESSAGE, context, string.Empty);
                                        if (string.IsNullOrEmpty(autoPromoteMsg) == false)
                                        {
                                            PermissionLevels permLvl = (PermissionLevels)autoPromoteLevel;

                                            string finalMsg = autoPromoteMsg.Replace("{0}", user.Name).Replace("{1}", permLvl.ToString());
                                            MsgHandler.QueueMessage(finalMsg);
                                        } 
                                    }
                                }
                            }
                        }
                    }
                    
                    //Save changes
                    context.SaveChanges();
                }
            }

            /************************************
            * Finally carry out the inputs now! *
            ************************************/

            InputHandler.CarryOutInput(inputSequence.Inputs, usedConsole, DataContainer.ControllerMngr);
        }

#endregion

        private void InitClientService()
        {
            ClientServiceTypes clientServiceType = ClientServiceTypes.Terminal;
            using (BotDBContext dbContext = DatabaseManager.OpenContext())
            {
                Settings setting = dbContext.SettingCollection.FirstOrDefault((set) => set.Key == SettingsConstants.CLIENT_SERVICE_TYPE);

                if (setting != null)
                {
                    clientServiceType = (ClientServiceTypes)setting.ValueInt;
                }
                else
                {
                    Console.WriteLine($"Database does not contain the {SettingsConstants.CLIENT_SERVICE_TYPE} setting. It will default to {clientServiceType}.");
                }
            }

            Console.WriteLine($"Setting up client service: {clientServiceType}");

            if (clientServiceType == ClientServiceTypes.Terminal)
            {
                ClientService = new TerminalClientService(DataConstants.COMMAND_IDENTIFIER);

                MsgHandler.SetLogToConsole(false);
            }
            else if (clientServiceType == ClientServiceTypes.Twitch)
            {
                TwitchLoginSettings twitchSettings = ConnectionHelper.ValidateTwitchCredentialsPresent(DataConstants.DataFolderPath,
                    TwitchConstants.LOGIN_SETTINGS_FILENAME);

                //If either of these fields are empty, the data is invalid
                if (string.IsNullOrEmpty(twitchSettings.ChannelName) || string.IsNullOrEmpty(twitchSettings.Password)
                    || string.IsNullOrEmpty(twitchSettings.BotName))
                {
                    Console.WriteLine($"Twitch login settings are invalid. Please modify the data in the {TwitchConstants.LOGIN_SETTINGS_FILENAME} file.");
                }

                TwitchClient client = new TwitchClient();
                ConnectionCredentials credentials = ConnectionHelper.MakeCredentialsFromTwitchLogin(twitchSettings);
                ClientService = new TwitchClientService(credentials, twitchSettings.ChannelName, DataConstants.COMMAND_IDENTIFIER, DataConstants.COMMAND_IDENTIFIER, true);
            }

            //Initialize service
            ClientService?.Initialize();
        }

        private bool FindThreadSleepTime(Settings setting)
        {
            return (setting.Key == SettingsConstants.MAIN_THREAD_SLEEP);
        }

        private void OnSoftReload()
        {
            Console.WriteLine("Reloaded soft");
            HandleReloadBoth();
        }

        private void OnHardReload()
        {
            Console.WriteLine("Reloaded hard");
            HandleReloadBoth();
        }

        private void HandleReloadBoth()
        {
            //Check if the periodic input value changed, and enable/disable the routine if we should
            long periodicEnabled = DataHelper.GetSettingInt(SettingsConstants.PERIODIC_INPUT_ENABLED, 0L);
            if (periodicEnabled == 0)
            {
                RoutineHandler.FindRoutine(RoutineConstants.PERIODIC_INPUT_ROUTINE_ID, out int rIndex);

                //Remove the routine if it exists
                if (rIndex >= 0)
                {
                    RoutineHandler.RemoveRoutine(rIndex);
                }
            }
            else
            {
                RoutineHandler.FindRoutine(RoutineConstants.PERIODIC_INPUT_ROUTINE_ID, out int rIndex);

                //Add the routine if it doesn't exist
                if (rIndex < 0)
                {
                    RoutineHandler.AddRoutine(new PeriodicInputRoutine()); 
                }
            }

            //Check if the virtual controller type was changed
            if (LastVControllerTypeChanged() == true)
            {
                VirtualControllerTypes lastVControllerType = (VirtualControllerTypes)DataHelper.GetSettingInt(SettingsConstants.LAST_VCONTROLLER_TYPE, 0L);
                VirtualControllerTypes supportedVCType = VControllerHelper.ValidateVirtualControllerType(lastVControllerType, TRBotOSPlatform.CurrentOS);

                //Show a message saying the previous value wasn't supported
                if (VControllerHelper.IsVControllerSupported(lastVControllerType, TRBotOSPlatform.CurrentOS) == false)
                {
                    MsgHandler.QueueMessage($"Current virtual controller {lastVControllerType} is not supported by the {TRBotOSPlatform.CurrentOS} platform. Switched it to the default of {supportedVCType} for this platform.");                
                    using (BotDBContext context = DatabaseManager.OpenContext())
                    {
                        Settings lastVControllerSetting = DataHelper.GetSettingNoOpen(SettingsConstants.LAST_VCONTROLLER_TYPE, context);
                        lastVControllerSetting.ValueInt = (long)supportedVCType;
                        context.SaveChanges();
                    }
                }

                ChangeVControllerType(supportedVCType);

                return; 
            }
        
            ReinitVControllerCount();
        }

        private bool LastVControllerTypeChanged()
        {
            VirtualControllerTypes vContType = (VirtualControllerTypes)DataHelper.GetSettingInt(SettingsConstants.LAST_VCONTROLLER_TYPE, 0L);

            return (vContType != DataContainer.CurVControllerType);
        }

        private void ChangeVControllerType(in VirtualControllerTypes newVControllerType)
        {
            MsgHandler.QueueMessage("Found virtual controller manager change in database. Stopping all inputs and reinitializing virtual controllers.");

            //Fetch the new controller manager
            IVirtualControllerManager controllerMngr = VControllerHelper.GetVControllerMngrForType(newVControllerType);

            using BotDBContext context = DatabaseManager.OpenContext();

            Settings joystickCountSetting = DataHelper.GetSettingNoOpen(SettingsConstants.JOYSTICK_COUNT, context);

            //First, stop all inputs
            InputHandler.StopAndHaltAllInputs();

            //Dispose the controller manager
            DataContainer.ControllerMngr.Dispose();

            DataContainer.SetCurVControllerType(newVControllerType);
            DataContainer.SetControllerManager(controllerMngr);

            DataContainer.ControllerMngr.Initialize();

            //Validate virtual controller count for this virtual controller manager
            int minCount = DataContainer.ControllerMngr.MinControllers;
            int maxCount = DataContainer.ControllerMngr.MaxControllers;

            if (joystickCountSetting.ValueInt < minCount)
            {
                MsgHandler.QueueMessage($"New controller count of {joystickCountSetting.ValueInt} in database is invalid. Clamping to the min of {minCount}.");
                joystickCountSetting.ValueInt = minCount;
                context.SaveChanges();
            }
            else if (joystickCountSetting.ValueInt > maxCount)
            {
                MsgHandler.QueueMessage($"New controller count of {joystickCountSetting.ValueInt} in database is invalid. Clamping to the max of {maxCount}.");
                joystickCountSetting.ValueInt = maxCount;
                context.SaveChanges();
            }

            int acquiredCount = DataContainer.ControllerMngr.InitControllers((int)joystickCountSetting.ValueInt);

            //Resume inputs
            InputHandler.ResumeRunningInputs();

            MsgHandler.QueueMessage($"Changed to virtual controller {DataContainer.CurVControllerType} and acquired {acquiredCount} controllers!");
        }

        private void ReinitVControllerCount()
        {
            int curVControllerCount = DataContainer.ControllerMngr.ControllerCount;

            using BotDBContext context = DatabaseManager.OpenContext();

            Settings joystickCountSetting = DataHelper.GetSettingNoOpen(SettingsConstants.JOYSTICK_COUNT, context);

            int minCount = DataContainer.ControllerMngr.MinControllers;
            int maxCount = DataContainer.ControllerMngr.MaxControllers;

            //Validate controller count
            if (joystickCountSetting.ValueInt < minCount)
            {
                MsgHandler.QueueMessage($"New controller count of {joystickCountSetting.ValueInt} in database is invalid. Clamping to the min of {minCount}.");
                joystickCountSetting.ValueInt = minCount;
                context.SaveChanges();
            }
            else if (joystickCountSetting.ValueInt > maxCount)
            {
                MsgHandler.QueueMessage($"New controller count of {joystickCountSetting.ValueInt} in database is invalid. Clamping to the max of {maxCount}.");
                joystickCountSetting.ValueInt = maxCount;
                context.SaveChanges();
            }

            //Same count, so ignore
            if (curVControllerCount == joystickCountSetting.ValueInt)
            {
                return;
            }

            MsgHandler.QueueMessage("Found controller count change in database. Stopping all inputs and reinitializing virtual controllers.");

            //First, stop all inputs
            InputHandler.StopAndHaltAllInputs();

            //Re-initialize the new number of virtual controllers
            DataContainer.ControllerMngr.Dispose();
            DataContainer.ControllerMngr.Initialize();
            int acquiredCount = DataContainer.ControllerMngr.InitControllers((int)joystickCountSetting.ValueInt);

            //Resume inputs
            InputHandler.ResumeRunningInputs();

            MsgHandler.QueueMessage($"Set up and acquired {acquiredCount} controllers!");
        }   
    }
}
