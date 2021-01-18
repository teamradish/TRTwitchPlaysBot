/* Copyright (C) 2019-2020 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot,software for playing games through text.
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
using TRBot.Logging;

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

            TRBotLogger.DisposeLogger();
        }

        public void Initialize()
        {
            if (Initialized == true)
                return;

            //Use invariant culture
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            //Set up the logger
            string logPath = Path.Combine(LoggingConstants.LogFolderPath, LoggingConstants.LOG_FILE_NAME);
            if (Utilities.FileHelpers.ValidatePathForFile(logPath) == false)
            {
                Console.WriteLine("Logger path cannot be validated. This is a problem! Double check the path is correct.");
            }

            //Set up the logger
            //Cap the size at 10 MB
            TRBotLogger.SetupLogger(logPath, Serilog.Events.LogEventLevel.Verbose,
                Serilog.RollingInterval.Day, 1024L * 1024L * 10L, TimeSpan.FromSeconds(60d));

            //Initialize database
            string databasePath = Path.Combine(DataConstants.DataFolderPath, DataConstants.DATABASE_FILE_NAME);

            TRBotLogger.Logger.Information($"Validating database at: {databasePath}");
            if (Utilities.FileHelpers.ValidatePathForFile(databasePath) == false)
            {
                TRBotLogger.Logger.Error($"Cannot create database path at {databasePath}. Check if you have permission to write to this directory. Aborting.");
                return;
            }

            TRBotLogger.Logger.Information("Database path validated! Initializing database and importing migrations.");

            DatabaseManager.SetDatabasePath(databasePath);
            DatabaseManager.InitAndMigrateContext();
            
            TRBotLogger.Logger.Information("Checking to initialize default values for missing database entries.");

            //Check for and initialize default values if the database was newly created or needs updating
            int addedDefaultEntries = DataHelper.InitDefaultData();

            if (addedDefaultEntries > 0)
            {
                TRBotLogger.Logger.Information($"Added {addedDefaultEntries} additional entries to the database.");
            }

            //Set the logger's log level
            long logLevel = DataHelper.GetSettingInt(SettingsConstants.LOG_LEVEL, (long)Serilog.Events.LogEventLevel.Information);
            TRBotLogger.SetLogLevel((Serilog.Events.LogEventLevel)logLevel);

            TRBotLogger.Logger.Information("Initializing client service");

            //Initialize client service
            InitClientService();

            //If the client service doesn't exist, we can't continue
            if (ClientService == null)
            {
                TRBotLogger.Logger.Error("Client service failed to initialize; please check your settings. Aborting.");
                return;
            }

            //Set client service and message cooldown
            MsgHandler.SetClientService(ClientService);

            MessageThrottlingOptions msgThrottleOption = (MessageThrottlingOptions)DataHelper.GetSettingInt(SettingsConstants.MESSAGE_THROTTLE_TYPE, 0L);
            long msgCooldown = DataHelper.GetSettingInt(SettingsConstants.MESSAGE_COOLDOWN, 30000L);
            long msgThrottleCount = DataHelper.GetSettingInt(SettingsConstants.MESSAGE_THROTTLE_COUNT, 20L);
            MsgHandler.SetMessageThrottling(msgThrottleOption, new MessageThrottleData(msgCooldown, msgThrottleCount));

            //Subscribe to events
            UnsubscribeEvents();
            SubscribeEvents();

            DataContainer.SetMessageHandler(MsgHandler);
            DataContainer.SetDataReloader(DataReloader);

            TRBotLogger.Logger.Information("Setting up virtual controller manager.");

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

            TRBotLogger.Logger.Information($"Setting up virtual controller {curVControllerType} and acquired {acquiredCount} controllers!");

            CmdHandler = new CommandHandler();
            CmdHandler.Initialize(DataContainer, RoutineHandler);

            DataReloader.SoftDataReloadedEvent -= OnSoftReload;
            DataReloader.SoftDataReloadedEvent += OnSoftReload;

            DataReloader.HardDataReloadedEvent -= OnHardReload;
            DataReloader.HardDataReloadedEvent += OnHardReload;

            //Initialize routines
            InitRoutines();

            //Cache our parser
            InputParser = new Parser();

            Initialized = true;
        }

        public void Run()
        {
            if (ClientService.IsConnected == true)
            {
                TRBotLogger.Logger.Information("Client is already connected and running!");
                return;
            }

            try
            {
                ClientService.Connect();
            }
            catch (Exception e)
            {
                TRBotLogger.Logger.Error($"Unable to connect to client service: {e.Message}\n{e.StackTrace}");
            }

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
            TRBotLogger.Logger.Information($"Bot connected!");
        }

        private void OnConnectionError(EvtConnectionErrorArgs e)
        {
            MsgHandler.QueueMessage($"Failed to connect: {e.Error.Message}");
        }

        private void OnJoinedChannel(EvtJoinedChannelArgs e)
        {
            TRBotLogger.Logger.Information($"Joined channel \"{e.Channel}\"");

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
                TRBotLogger.Logger.Error($"Ran into exception on command {e.Command.CommandText}: {exc.Message}\n{exc.StackTrace}"); 
            }
        }

        private void OnUserSentMessage(EvtUserMessageArgs e)
        {
            //Look for a user with this name
            string userName = e.UsrMessage.Username;

            if (string.IsNullOrEmpty(userName) == false)
            {
                User user = DataHelper.GetOrAddUser(userName, out bool added);
                
                if (added == true)
                {
                    //Get the new user message and replace the variable with their name
                    string newUserMessage = DataHelper.GetSettingString(SettingsConstants.NEW_USER_MESSAGE, $"Welcome to the stream, {userName}!");
                    newUserMessage = newUserMessage.Replace("{0}", userName);
                    
                    MsgHandler.QueueMessage(newUserMessage);
                }
                
                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    user = DataHelper.GetUserNoOpen(userName, context);
                    
                    //Increment message count and save
                    if (user.IsOptedOut == false)
                    {
                        user.Stats.TotalMessageCount++;
                        context.SaveChanges();
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
            TRBotLogger.Logger.Warning("Bot disconnected! Please check your internet connection.");
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
            string userName = e.UsrMessage.Username;
            int defaultDur = 200;
            int defaultPort = 0;

            try
            {
                int maxDur = 60000;

                string regexStr = usedConsole.InputRegex;

                string readyMessage = string.Empty;
                
                //Get default and max input durations
                //Use user overrides if they exist, otherwise use the global values
                User user = DataHelper.GetUser(userName);

                //Get default controller port
                defaultPort = (int)user.ControllerPort;

                defaultDur = (int)DataHelper.GetUserOrGlobalDefaultInputDur(userName);
                maxDur = (int)DataHelper.GetUserOrGlobalMaxInputDur(userName);

                //TRBotLogger.Logger.Information($"Default dur: {defaultDur} | Max dur: {maxDur}");

                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    //Get input synonyms for this console
                    IQueryable<InputSynonym> synonyms = context.InputSynonyms.Where(syn => syn.ConsoleID == lastConsoleID);

                    //Prepare the message for parsing
                    readyMessage = InputParser.PrepParse(e.UsrMessage.Message, context.Macros, synonyms);
                }

                //Parse inputs to get our parsed input sequence
                inputSequence = InputParser.ParseInputs(readyMessage, regexStr, new ParserOptions(defaultPort, defaultDur, true, maxDur));
                TRBotLogger.Logger.Debug(inputSequence.ToString());
                TRBotLogger.Logger.Debug("Reverse Parsed (on parse): " + ReverseParser.ReverseParse(inputSequence, usedConsole,
                    new ReverseParser.ReverseParserOptions(ReverseParser.ShowPortTypes.ShowNonDefaultPorts, defaultPort,
                    ReverseParser.ShowDurationTypes.ShowNonDefaultDurations, defaultDur)));
            }
            catch (Exception exception)
            {
                string excMsg = exception.Message;

                //Handle parsing exceptions
                MsgHandler.QueueMessage($"ERROR PARSING: {excMsg} | {exception.StackTrace}", Serilog.Events.LogEventLevel.Warning);
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

            long globalInputPermLevel = DataHelper.GetSettingInt(SettingsConstants.GLOBAL_INPUT_LEVEL, 0L);
            int userControllerPort = 0;
            long userLevel = 0;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetUserNoOpen(e.UsrMessage.Username, context);

                //Check if the user is silenced and ignore the message if so
                if (user.HasEnabledAbility(PermissionConstants.SILENCED_ABILITY) == true)
                {
                    return;
                }
            
                //Ignore based on user level and permissions
                if (user.Level < globalInputPermLevel)
                {
                    MsgHandler.QueueMessage($"Inputs are restricted to levels {(PermissionLevels)globalInputPermLevel} and above.");
                    return;
                }

                userControllerPort = (int)user.ControllerPort;
                userLevel = user.Level;
            }

            //First, add delays between inputs if we should
            //We do this first so we can validate the inserted inputs later
            //The blank inputs can have a different permission level
            if (DataHelper.GetUserOrGlobalMidInputDelay(e.UsrMessage.Username, out long midInputDelay) == true)
            {
                MidInputDelayData midInputDelayData = ParserPostProcess.InsertMidInputDelays(inputSequence, userControllerPort, (int)midInputDelay, usedConsole);

                //If it's successful, replace the input list and duration
                if (midInputDelayData.Success == true)
                {
                    int oldDur = inputSequence.TotalDuration;
                    inputSequence.Inputs = midInputDelayData.NewInputs;
                    inputSequence.TotalDuration = midInputDelayData.NewTotalDuration;

                    //TRBotLogger.Logger.Debug($"Mid input delay success. Message: {midInputDelayData.Message} | OldDur: {oldDur} | NewDur: {inputSequence.TotalDuration}\n{ReverseParser.ReverseParse(inputSequence, usedConsole, new ReverseParser.ReverseParserOptions(ReverseParser.ShowPortTypes.ShowAllPorts, 0))}");
                }
            }

            InputValidation validation = default;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetUserNoOpen(userName, context);

                //Check for restricted inputs on this user
                validation = ParserPostProcess.InputSequenceContainsRestrictedInputs(inputSequence, user.GetRestrictedInputs());

                if (validation.InputValidationType != InputValidationTypes.Valid)
                {
                    if (string.IsNullOrEmpty(validation.Message) == false)
                    {
                        MsgHandler.QueueMessage(validation.Message);
                    }
                    return;
                }

                //Check for invalid input combinations
                validation = ParserPostProcess.ValidateInputCombos(inputSequence, usedConsole.InvalidCombos,
                    DataContainer.ControllerMngr, usedConsole);
                
                if (validation.InputValidationType != InputValidationTypes.Valid)
                {
                    if (string.IsNullOrEmpty(validation.Message) == false)
                    {
                        MsgHandler.QueueMessage(validation.Message);
                    }
                    return;
                }
            }

            //Check for level permissions and ports
            validation = ParserPostProcess.ValidateInputLvlPermsAndPorts(userLevel, inputSequence,
                DataContainer.ControllerMngr, usedConsole.ConsoleInputs);
            if (validation.InputValidationType != InputValidationTypes.Valid)
            {
                if (string.IsNullOrEmpty(validation.Message) == false)
                {
                    MsgHandler.QueueMessage(validation.Message, Serilog.Events.LogEventLevel.Warning);
                }
                return;
            }

            #endregion

            //Make sure inputs aren't stopped
            if (InputHandler.InputsHalted == true)
            {
                //We can't process inputs because they're currently stopped
                MsgHandler.QueueMessage("New inputs cannot be processed until all other inputs have stopped.", Serilog.Events.LogEventLevel.Warning);
                return;
            }

            //Fetch these values ahead of time to avoid passing the database context through so many methods    
            long autoPromoteEnabled = DataHelper.GetSettingInt(SettingsConstants.AUTO_PROMOTE_ENABLED, 0L);
            long autoPromoteInputReq = DataHelper.GetSettingInt(SettingsConstants.AUTO_PROMOTE_INPUT_REQ, long.MaxValue);
            long autoPromoteLevel = DataHelper.GetSettingInt(SettingsConstants.AUTO_PROMOTE_LEVEL, -1L);
            string autoPromoteMsg = DataHelper.GetSettingString(SettingsConstants.AUTOPROMOTE_MESSAGE, string.Empty);

            bool addedInputCount = false;
            
            TRBotLogger.Logger.Debug($"Reverse Parsed (post-process): " + ReverseParser.ReverseParse(inputSequence, usedConsole,
                    new ReverseParser.ReverseParserOptions(ReverseParser.ShowPortTypes.ShowNonDefaultPorts, defaultPort,
                    ReverseParser.ShowDurationTypes.ShowNonDefaultDurations, defaultDur)));

            //Get the max recorded inputs per-user
            long maxUserRecInps = DataHelper.GetSettingInt(SettingsConstants.MAX_USER_RECENT_INPUTS, 0L);

            //It's a valid input - save it in the user's stats
            //Also record the input if we should
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetUserNoOpen(e.UsrMessage.Username, context);

                //Ignore if the user is opted out
                if (user.IsOptedOut == false)
                {
                    user.Stats.ValidInputCount++;
                    addedInputCount = true;

                    context.SaveChanges();

                    //If we should store recent user inputs, do so
                    if (maxUserRecInps > 0)
                    {
                        //Get the input sequence - we may have added mid input delays between
                        //As a result, we'll need to reverse parse it
                        string message = ReverseParser.ReverseParse(inputSequence, usedConsole,
                            new ReverseParser.ReverseParserOptions(ReverseParser.ShowPortTypes.ShowNonDefaultPorts, (int)user.ControllerPort,
                            ReverseParser.ShowDurationTypes.ShowNonDefaultDurations, defaultDur));

                        //Add the recorded input
                        user.RecentInputs.Add(new RecentInput(message));
                        context.SaveChanges();

                        int diff = user.RecentInputs.Count - (int)maxUserRecInps;

                        //If we're over the max after adding, remove
                        if (diff > 0)
                        {
                            //Order by ascending ID and take the difference
                            //Lower IDs = older entries
                            IEnumerable<RecentInput> shouldRemove = user.RecentInputs.OrderBy(r => r.UserID).Take(diff);

                            foreach (RecentInput rec in shouldRemove)
                            {
                                user.RecentInputs.Remove(rec);
                                context.SaveChanges();
                            }
                        }
                    }
                }
            }

            bool autoPromoted = false;

            //Check if auto promote is enabled and auto promote the user if applicable
            if (addedInputCount == true)
            {
                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    User user = DataHelper.GetUserNoOpen(e.UsrMessage.Username, context);
                    
                    //Check if the user was already autopromoted, autopromote is enabled,
                    //and if the user reached the autopromote input count requirement
                    if (user.Stats.AutoPromoted == 0 && autoPromoteEnabled > 0
                        && user.Stats.ValidInputCount >= autoPromoteInputReq)
                    {
                        //Only autopromote if this is a valid permission level
                        //We may not want to log or send a message for this, as it has potential to be very spammy,
                        //and it's not something the users can control
                        if (PermissionHelpers.IsValidPermissionValue(autoPromoteLevel) == true)
                        {
                            //Mark the user as autopromoted and save
                            user.Stats.AutoPromoted = 1;
                            autoPromoted = true;

                            context.SaveChanges();
                        }
                    }
                }
            }

            if (autoPromoted == true)
            {
                //If the user is already at or above this level, don't set them to it
                //Only set if the user is below
                if (userLevel < autoPromoteLevel)
                {
                    //Adjust abilities and promote to the new level
                    DataHelper.AdjustUserLvlAndAbilitiesOnLevel(userName, autoPromoteLevel);

                    if (string.IsNullOrEmpty(autoPromoteMsg) == false)
                    {
                        PermissionLevels permLvl = (PermissionLevels)autoPromoteLevel;
                        string finalMsg = autoPromoteMsg.Replace("{0}", userName).Replace("{1}", permLvl.ToString());
                        MsgHandler.QueueMessage(finalMsg);
                    } 
                }
            }

            InputModes inputMode = (InputModes)DataHelper.GetSettingInt(SettingsConstants.INPUT_MODE, 0L);

            //If the mode is Democracy, add it as a vote for this input
            if (inputMode == InputModes.Democracy)
            {
                //Set up the routine if it doesn't exist
                BaseRoutine foundRoutine = RoutineHandler.FindRoutine(RoutineConstants.DEMOCRACY_ROUTINE_ID, out int indexFound);
                DemocracyRoutine democracyRoutine = null;

                if (foundRoutine == null)
                {
                    long voteTime = DataHelper.GetSettingInt(SettingsConstants.DEMOCRACY_VOTE_TIME, 10000L);

                    democracyRoutine = new DemocracyRoutine(voteTime);
                    RoutineHandler.AddRoutine(democracyRoutine);
                }
                else
                {
                    democracyRoutine = (DemocracyRoutine)foundRoutine;
                }

                democracyRoutine.AddInputSequence(userName, inputSequence.Inputs);
            }
            //If it's Anarchy, carry out the input
            else
            {
                /************************************
                * Finally carry out the inputs now! *
                ************************************/

                InputHandler.CarryOutInput(inputSequence.Inputs, usedConsole, DataContainer.ControllerMngr);
            }
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
                    TRBotLogger.Logger.Warning($"Database does not contain the {SettingsConstants.CLIENT_SERVICE_TYPE} setting. It will default to {clientServiceType}.");
                }
            }

            TRBotLogger.Logger.Information($"Setting up client service: {clientServiceType}");

            if (clientServiceType == ClientServiceTypes.Terminal)
            {
                ClientService = new TerminalClientService(DataConstants.COMMAND_IDENTIFIER);

                MsgHandler.SetLogToLogger(false);
            }
            else if (clientServiceType == ClientServiceTypes.Twitch)
            {
                TwitchLoginSettings twitchSettings = ConnectionHelper.ValidateTwitchCredentialsPresent(DataConstants.DataFolderPath,
                    TwitchConstants.LOGIN_SETTINGS_FILENAME);

                //If either of these fields are empty, the data is invalid
                if (string.IsNullOrEmpty(twitchSettings.ChannelName) || string.IsNullOrEmpty(twitchSettings.Password)
                    || string.IsNullOrEmpty(twitchSettings.BotName))
                {
                    TRBotLogger.Logger.Error($"Twitch login settings are invalid. Please modify the data in the \"{TwitchConstants.LOGIN_SETTINGS_FILENAME}\" file.");
                    return;
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
            HandleReloadBoth();
        }

        private void OnHardReload()
        {
            HandleReloadBoth();
        }

        private void HandleReloadBoth()
        {
            //Check for changes in the log level
            Serilog.Events.LogEventLevel logLevel = (Serilog.Events.LogEventLevel)DataHelper.GetSettingInt(SettingsConstants.LOG_LEVEL, (long)Serilog.Events.LogEventLevel.Information);
            if (logLevel != TRBotLogger.MinLoggingLevel)
            {
                TRBotLogger.Logger.Information($"Detected change in logging level - changing the logging level from {TRBotLogger.MinLoggingLevel} to {logLevel}");
                TRBotLogger.SetLogLevel(logLevel);
            }

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
            }
            else
            {
                ReinitVControllerCount();
            }

            //Handle message throttling changes
            MessageThrottlingOptions msgThrottle = (MessageThrottlingOptions)DataHelper.GetSettingInt(SettingsConstants.MESSAGE_THROTTLE_TYPE, 0L);
            long msgTime = DataHelper.GetSettingInt(SettingsConstants.MESSAGE_COOLDOWN, 30000L);
            long msgThrottleCount = DataHelper.GetSettingInt(SettingsConstants.MESSAGE_THROTTLE_COUNT, 20L);

            if (msgThrottle != MsgHandler.CurThrottleOption)
            {
                TRBotLogger.Logger.Information("Detected change in message throttling type - changing message throttler.");
            }

            MsgHandler.SetMessageThrottling(msgThrottle, new MessageThrottleData(msgTime, msgThrottleCount));
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

            int prevJoystickCount = (int)DataHelper.GetSettingInt(SettingsConstants.JOYSTICK_COUNT, 1L);
            int newJoystickCount = prevJoystickCount;

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

            if (prevJoystickCount < minCount)
            {
                MsgHandler.QueueMessage($"New controller count of {prevJoystickCount} in database is invalid. Clamping to the min of {minCount}.");
                newJoystickCount = minCount;
            }
            else if (prevJoystickCount > maxCount)
            {
                MsgHandler.QueueMessage($"New controller count of {prevJoystickCount} in database is invalid. Clamping to the max of {maxCount}.");
                newJoystickCount = maxCount;
            }

            if (prevJoystickCount != newJoystickCount)
            {
                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    //Adjust the value and save
                    Settings joystickCountSetting = DataHelper.GetSettingNoOpen(SettingsConstants.JOYSTICK_COUNT, context);
                    joystickCountSetting.ValueInt = newJoystickCount;
                    
                    context.SaveChanges();
                }
            }

            int acquiredCount = DataContainer.ControllerMngr.InitControllers(newJoystickCount);

            //Resume inputs
            InputHandler.ResumeRunningInputs();

            MsgHandler.QueueMessage($"Changed to virtual controller {DataContainer.CurVControllerType} and acquired {acquiredCount} controllers!");
        }

        private void ReinitVControllerCount()
        {
            int curVControllerCount = DataContainer.ControllerMngr.ControllerCount;

            int prevJoystickCount = (int)DataHelper.GetSettingInt(SettingsConstants.JOYSTICK_COUNT, 1L);
            int newJoystickCount = prevJoystickCount;

            int minCount = DataContainer.ControllerMngr.MinControllers;
            int maxCount = DataContainer.ControllerMngr.MaxControllers;

            //Validate controller count
            if (prevJoystickCount < minCount)
            {
                MsgHandler.QueueMessage($"New controller count of {prevJoystickCount} in database is invalid. Clamping to the min of {minCount}.");
                newJoystickCount = minCount;
            }
            else if (prevJoystickCount > maxCount)
            {
                MsgHandler.QueueMessage($"New controller count of {prevJoystickCount} in database is invalid. Clamping to the max of {maxCount}.");
                newJoystickCount = maxCount;
            }

            if (prevJoystickCount != newJoystickCount)
            {
                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    //Adjust the value and save
                    Settings joystickCountSetting = DataHelper.GetSettingNoOpen(SettingsConstants.JOYSTICK_COUNT, context);
                    joystickCountSetting.ValueInt = newJoystickCount;

                    context.SaveChanges();
                }
            }

            //Same count, so ignore
            if (curVControllerCount == newJoystickCount)
            {
                return;
            }

            MsgHandler.QueueMessage("Found controller count change in database. Stopping all inputs and reinitializing virtual controllers.");

            //First, stop all inputs
            InputHandler.StopAndHaltAllInputs();

            //Re-initialize the new number of virtual controllers
            DataContainer.ControllerMngr.Dispose();
            DataContainer.ControllerMngr.Initialize();
            int acquiredCount = DataContainer.ControllerMngr.InitControllers(newJoystickCount);

            //Resume inputs
            InputHandler.ResumeRunningInputs();

            MsgHandler.QueueMessage($"Set up and acquired {acquiredCount} controllers!");
        }   
    }
}
