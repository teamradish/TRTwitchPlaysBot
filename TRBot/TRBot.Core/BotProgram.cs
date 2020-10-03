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
using TRBot.ParserData;
using TRBot.Connection;
using TRBot.Consoles;
using TRBot.VirtualControllers;
using TRBot.Common;
using TRBot.Utilities;
using TRBot.Data;
using TRBot.Commands;
using Newtonsoft.Json;
using TwitchLib;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Client.Events;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Interfaces;

namespace TRBot.Core
{
    public sealed class BotProgram : IDisposable
    {
        //private static BotProgram instance = null;

        public bool Initialized { get; private set; } = false;

        public IClientService ClientService { get; private set; } = null;
        public IVirtualControllerManager ControllerMngr { get; private set; } = null;
        public GameConsole CurConsole { get; private set; } = null;
        
        public BotMessageHandler MsgHandler { get; private set; } = new BotMessageHandler();

        private InputMacroCollection MacroData = new InputMacroCollection();
        private InputSynonymCollection SynonymData = new InputSynonymCollection();

        private Parser InputParser = null;
        private CommandHandler CmdHandler = null;
        private DataReloader DataReloader = new DataReloader();

        //Store the function to reduce garbage, since this one is being called constantly
        private Func<Settings, bool> ThreadSleepFindFunc = null;

        public BotProgram()
        {
            //Below normal priority
            //NOTE: Have a setting to set the priority
            Process thisProcess = Process.GetCurrentProcess();
            thisProcess.PriorityBoostEnabled = false;
            thisProcess.PriorityClass = ProcessPriorityClass.BelowNormal;

            ThreadSleepFindFunc = FindThreadSleepTime;
        }

        //Clean up anything we need to here
        public void Dispose()
        {
            if (Initialized == false)
                return;

            UnsubscribeEvents();

            //RoutineHandler?.CleanUp();

            //CommandHandler.CleanUp();
            ClientService?.CleanUp();

            MsgHandler?.CleanUp();
            CmdHandler?.CleanUp();
            DataReloader?.CleanUp();

            if (ClientService?.IsConnected == true)
                ClientService.Disconnect();

            //Clean up and relinquish the virtual controllers when we're done
            ControllerMngr?.CleanUp();

            //instance = null;
        }

        public void Initialize()
        {
            if (Initialized == true)
                return;

            //Kimimaru: Use invariant culture
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            InputParser = new Parser();

            CurConsole = new GCConsole();

            //Initialize database
            string databasePath = Path.Combine(DataConstants.DataFolderPath, "TRBotData.db");

            Console.WriteLine($"Validating database at: {databasePath}");
            if (Utilities.FileHelpers.ValidatePathForFile(databasePath) == false)
            {
                Console.WriteLine($"Cannot create database path at {databasePath}. Check if you have permission to write to this directory. Aborting.");
                return;
            }
            
            Console.WriteLine("Database path validated! Initializing database and importing migrations.");

            DatabaseManager.SetDatabasePath(databasePath);
            DatabaseManager.InitAndMigrateContext();
            
            Console.WriteLine("Adding default values for non-existent database settings.");

            //Check for and initialize default values if the database was newly created or needs updating
            InitDefaultData(out int entries);

            if (entries > 0)
            {
                Console.WriteLine($"Added {entries} additional entries to the database."); 
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

            Console.WriteLine("Setting up virtual controller manager.");

            VirtualControllerTypes lastVControllerType = ValidateVirtualControllerType();

            ControllerMngr = VControllerHelper.GetVControllerMngrForType(lastVControllerType);

            //No controller manager - there's a problem
            if (ControllerMngr == null)
            {
                Console.WriteLine($"Virtual controller manager failed to initialize. This indicates an invalid {SettingsConstants.LAST_VCONTROLLER_TYPE} setting in the database or an unimplemented platform. Aborting.");
                return;
            }

            ControllerMngr.Initialize();
            int acquiredCount = ControllerMngr.InitControllers(1);

            Console.WriteLine($"Setting up virtual controller {lastVControllerType} and acquired {acquiredCount} controllers!");

            MacroData = new InputMacroCollection(new ConcurrentDictionary<string, InputMacro>());
            MacroData.AddMacro(new InputMacro("#mash(*)", "[<0>34ms #34ms]*20"));
            MacroData.AddMacro(new InputMacro("#test", "b500ms #200ms up"));
            MacroData.AddMacro(new InputMacro("#test2", "a #200ms #test"));

            SynonymData = new InputSynonymCollection(new ConcurrentDictionary<string, InputSynonym>());
            SynonymData.AddSynonym(new InputSynonym(".", "#"));
            SynonymData.AddSynonym(new InputSynonym("aandup", "a+up"));

            CmdHandler = new CommandHandler();
            CmdHandler.Initialize(MsgHandler, DataReloader);

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
                //TotalUptime = (utcNow - StartUptime);

                DateTime now = DateTime.Now;

                //Update queued messages
                MsgHandler.Update(now);

                //Update routines
                //RoutineHandler.Update(now);

                int threadSleep = 100;

                //Get the thread sleep value from the database
                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    Settings threadSleepSetting = context.SettingCollection.FirstOrDefault(ThreadSleepFindFunc);

                    //Clamp to avoid an exception - the code that changes this should handle values below 0,
                    //but the database can manually be changed to have lower values 
                    threadSleep = Math.Clamp((int)threadSleepSetting.value_int, 0, int.MaxValue);
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
            MsgHandler.QueueMessage($"Received command \"{e.Command.CommandText}\"");

            CmdHandler.HandleCommand(e);            
        }

        private void OnUserSentMessage(EvtUserMessageArgs e)
        {
            ProcessMsgAsInput(e);
        }

        private void OnUserMadeInput(EvtUserInputArgs e)
        {
            //InputHandler.CarryOutInput(e.ValidInputSeq.Inputs, CurConsole, ControllerMngr);

            //If auto whitelist is enabled, the user reached the whitelist message threshold,
            //the user isn't whitelisted, and the user hasn't ever been whitelisted, whitelist them
            //if (BotSettings.AutoWhitelistEnabled == true && user.Level < (int)AccessLevels.Levels.Whitelisted
            //    && user.AutoWhitelisted == false && user.ValidInputs >= BotSettings.AutoWhitelistInputCount)
            //{
            //    user.Level = (int)AccessLevels.Levels.Whitelisted;
            //    user.SetAutoWhitelist(true);
            //    if (string.IsNullOrEmpty(BotSettings.MsgSettings.AutoWhitelistMsg) == false)
            //    {
            //        //Replace the user's name with the message
            //        string msg = BotSettings.MsgSettings.AutoWhitelistMsg.Replace("{0}", user.Name);
            //        MsgHandler.QueueMessage(msg);
            //    }
            //}
        }

        private void OnWhisperReceived(EvtWhisperMessageArgs e)
        {
            
        }

        private void OnBeingHosted(EvtOnHostedArgs e)
        {
            //if (string.IsNullOrEmpty(BotSettings.MsgSettings.BeingHostedMsg) == false)
            //{
            //    string finalMsg = BotSettings.MsgSettings.BeingHostedMsg.Replace("{0}", e.HostedData.HostedByChannel);
            //    MsgHandler.QueueMessage(finalMsg);
            //}
        }

        private void OnNewSubscriber(EvtOnSubscriptionArgs e)
        {
            //if (string.IsNullOrEmpty(BotSettings.MsgSettings.NewSubscriberMsg) == false)
            //{
            //    string finalMsg = BotSettings.MsgSettings.NewSubscriberMsg.Replace("{0}", e.SubscriptionData.DisplayName);
            //    MsgHandler.QueueMessage(finalMsg);
            //}
        }

        private void OnReSubscriber(EvtOnReSubscriptionArgs e)
        {
            //if (string.IsNullOrEmpty(BotSettings.MsgSettings.ReSubscriberMsg) == false)
            //{
            //    string finalMsg = BotSettings.MsgSettings.ReSubscriberMsg.Replace("{0}", e.ReSubscriptionData.DisplayName).Replace("{1}", e.ReSubscriptionData.Months.ToString());
            //    MsgHandler.QueueMessage(finalMsg);
            //}
        }

        private void OnReconnected(EvtReconnectedArgs e)
        {
            //if (string.IsNullOrEmpty(BotSettings.MsgSettings.ReconnectedMsg) == false)
            //{
            //    MsgHandler.QueueMessage(BotSettings.MsgSettings.ReconnectedMsg);
            //}
        }

        private void OnDisconnected(EvtDisconnectedArgs e)
        {
            Console.WriteLine("Bot disconnected! Please check your internet connection.");
        }

        private void ProcessMsgAsInput(EvtUserMessageArgs e)
        {
            //User userData = e.UserData;

            //Ignore commands as inputs
            if (e.UsrMessage.Message.StartsWith(DataConstants.COMMAND_IDENTIFIER) == true)
            {
                return;
            }

            //If there are no valid inputs, don't attempt to parse
            if (CurConsole.ConsoleInputs == null || CurConsole.ConsoleInputs.Count == 0)//if (CurConsole.ValidInputs == null || CurConsole.ValidInputs.Count == 0)
            {
                return;
            }

            //Parser.InputSequence inputSequence = default;
            //(bool, List<List<Parser.Input>>, bool, int) parsedVal = default;
            ParsedInputSequence inputSequence = default;

            try
            {
                string regexStr = CurConsole.InputRegex;

                string readyMessage = InputParser.PrepParse(e.UsrMessage.Message, MacroData, SynonymData);

                //parse_message = InputParser.PopulateSynonyms(parse_message, InputGlobals.InputSynonyms);
                inputSequence = InputParser.ParseInputs(readyMessage, regexStr, new ParserOptions(0, 200, true, 60000));
                //Console.WriteLine(inputSequence.ToString());
                //Console.WriteLine("\nReverse Parsed: " + ReverseParser.ReverseParse(inputSequence));
                //Console.WriteLine("\nReverse Parsed Natural:\n" + ReverseParser.ReverseParseNatural(inputSequence));
            }
            catch (Exception exception)
            {
                string excMsg = exception.Message;

                //Kimimaru: Sanitize parsing exceptions
                //Most of these are currently caused by differences in how C# and Python handle slicing strings (Substring() vs string[:])
                //One example that throws this that shouldn't is "#mash(w234"
                //BotProgram.MsgHandler.QueueMessage($"ERROR: {excMsg}");
                inputSequence.InputValidationType = InputValidationTypes.Invalid;
                //parsedVal.Item1 = false;
            }

            //Check for non-valid messages
            if (inputSequence.InputValidationType != InputValidationTypes.Valid)
            {
                //Display error message for invalid inputs
                if (inputSequence.InputValidationType == InputValidationTypes.Invalid)
                {
                    MsgHandler.QueueMessage(inputSequence.Error);
                }

                return;
            }

            //It's a valid message, so process it
                
            //Ignore if user is silenced
            //if (userData.Silenced == true)
            //{
            //    return;
            //}

            //Ignore based on user level and permissions
            //if (userData.Level < -1)//BotProgram.BotData.InputPermissions)
            //{
            //    BotProgram.MsgHandler.QueueMessage($"Inputs are restricted to levels {(AccessLevels.Levels)BotProgram.BotData.InputPermissions} and above");
            //    return;
            //}

            #region Parser Post-Process Validation
            
            /* All this validation is very slow
             * Find a way to speed it up, ideally without integrating it directly into the parser
             */
            
            //Check if the user has permission to perform all the inputs they attempted
            //Also validate that the controller ports they're inputting for are valid
            //ParserPostProcess.InputValidation inputValidation = ParserPostProcess.CheckInputPermissionsAndPorts(userData.Level, inputSequence.Inputs,
            //    BotProgram.BotData.InputAccess.InputAccessDict);

            //If the input isn't valid, exit
            //if (inputValidation.IsValid == false)
            //{
            //    if (string.IsNullOrEmpty(inputValidation.Message) == false)
            //    {
            //        BotProgram.MsgHandler.QueueMessage(inputValidation.Message);
            //    }
            //    return;
            //}

            //Lastly, check for invalid button combos given the current console
            /*if (BotProgram.BotData.InvalidBtnCombos.InvalidCombos.TryGetValue((int)InputGlobals.CurrentConsoleVal, out List<string> invalidCombos) == true)
            {
                bool buttonCombosValidated = ParserPostProcess.ValidateButtonCombos(inputSequence.Inputs, invalidCombos);

                if (buttonCombosValidated == false)
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
                    BotProgram.MsgHandler.QueueMessage(msg);
                    
                    return;
                }
            }*/

            #endregion

            /*if (true)//InputHandler.StopRunningInputs == false)
            {
                EvtUserInputArgs userInputArgs = new EvtUserInputArgs()
                {
                    //UserData = e.UserData,
                    UsrMessage = e.UsrMessage,
                    ValidInputSeq = inputSequence
                };

                //Invoke input event
                UserMadeInputEvent?.Invoke(userInputArgs);
            }
            else
            {
                //BotProgram.MsgHandler.QueueMessage("New inputs cannot be processed until all other inputs have stopped.");
            }*/

            InputHandler.CarryOutInput(inputSequence.Inputs, CurConsole, ControllerMngr);
        }

#endregion

        /// <summary>
        /// Initializes default values for data.
        /// </summary>
        private void InitDefaultData(out int entriesAdded)
        {
            entriesAdded = 0;
            
            using (BotDBContext dbContext = DatabaseManager.OpenContext())
            {
                //Check all settings with the defaults
                List<Settings> settings = DefaultData.GetDefaultSettings();
                if (dbContext.SettingCollection.Count() < settings.Count)
                {
                    for (int i = 0; i < settings.Count; i++)
                    {
                        Settings setting = settings[i];

                        //See if the setting exists
                        Settings foundSetting = dbContext.SettingCollection.FirstOrDefault((set) => set.key == setting.key);
                        if (foundSetting == null)
                        {
                            //Default setting does not exist, so add it
                            dbContext.SettingCollection.Add(setting);

                            entriesAdded++;
                        }
                    }
                }

                List<CommandData> cmdData = DefaultData.GetDefaultCommands();
                if (dbContext.Commands.Count() < cmdData.Count)
                {
                    for (int i = 0; i < cmdData.Count; i++)
                    {
                        CommandData commandData = cmdData[i];

                        //See if the command data exists
                        CommandData foundCommand = dbContext.Commands.FirstOrDefault((cmd) => cmd.name == commandData.name);
                        if (foundCommand == null)
                        {
                            //Default command does not exist, so add it
                            dbContext.Commands.Add(commandData);

                            entriesAdded++;
                        }
                    }
                }

                dbContext.SaveChanges();
            }
        }

        private VirtualControllerTypes ValidateVirtualControllerType()
        {
            VirtualControllerTypes lastVControllerType = VirtualControllerTypes.Invalid;
            using (BotDBContext dbContext = DatabaseManager.OpenContext())
            {
                //Validate the last virtual controller type
                Settings vControllerSetting = dbContext.SettingCollection.FirstOrDefault((set) => set.key == SettingsConstants.LAST_VCONTROLLER_TYPE);
                if (vControllerSetting != null)
                {
                    lastVControllerType = (VirtualControllerTypes)vControllerSetting.value_int;
                }

                //Check if the virtual controller type is supported on this platform
                if (VControllerHelper.IsVControllerSupported(lastVControllerType, OSPlatform.CurrentOS) == false)
                {
                    //It's not supported, so switch it to prevent issues on this platform
                    VirtualControllerTypes defaultVContType = VControllerHelper.GetDefaultVControllerTypeForPlatform(OSPlatform.CurrentOS);
                    if (vControllerSetting != null)
                    {
                        vControllerSetting.value_int = (long)defaultVContType;
                    }

                    Console.WriteLine($"Current virtual controller {lastVControllerType} is not supported by the {OSPlatform.CurrentOS} platform. Switched it to the default of {defaultVContType} for this platform.");
                    lastVControllerType = defaultVContType;

                    dbContext.SaveChanges();
                }
            }

            return lastVControllerType;
        }

        private void InitClientService()
        {
            ClientServiceTypes clientServiceType = ClientServiceTypes.Terminal;
            using (BotDBContext dbContext = DatabaseManager.OpenContext())
            {
                Settings setting = dbContext.SettingCollection.FirstOrDefault((set) => set.key == SettingsConstants.CLIENT_SERVICE_TYPE);

                if (setting != null)
                {
                    clientServiceType = (ClientServiceTypes)setting.value_int;
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
            return (setting.key == SettingsConstants.MAIN_THREAD_SLEEP);
        }
    }
}
