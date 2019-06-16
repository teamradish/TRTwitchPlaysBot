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
        private static readonly object LockObj = new object();

        private static BotProgram instance = null;

        public bool Initialized { get; private set; } = false;

        private LoginInfo LoginInformation = null;
        public static Settings BotSettings { get; private set; } = null;
        public static BotData BotData { get; private set; } = null;

        private TwitchClient Client;
        private ConnectionCredentials Credentials = null;
        private CrashHandler crashHandler = null;

        private CommandHandler CommandHandler = null;

        private bool TryReconnect = false;

        private DateTime CurQueueTime;

        /// <summary>
        /// Queued messages.
        /// </summary>
        private Queue<string> ClientMessages = new Queue<string>();

        private List<BaseRoutine> BotRoutines = new List<BaseRoutine>();

        //Throttler
        private Stopwatch Throttler = new Stopwatch();

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

        public void Dispose()
        {
            if (Initialized == false)
                return;

            UnsubscribeEvents();

            for (int i = 0; i < BotRoutines.Count; i++)
            {
                BotRoutines[i].CleanUp(Client);
            }

            ClientMessages.Clear();
            Client.Disconnect();

            //Clean up and relinquish the devices when we're done
            VJoyController.CleanUp();

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
            Client.OnDisconnected += OnDisconnected;

            AddRoutine(new PeriodicMessageRoutine());
            AddRoutine(new CreditsGiveRoutine());

            //Initialize controller input
            VJoyController.Initialize();
            VJoyController.CheckButtonCount(1);

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

            const int CPUPercentLimit = 10;

            //Run
            while (true)
            {
                Throttler.Reset();
                Throttler.Start();

                long start = Throttler.ElapsedTicks;

                DateTime now = DateTime.Now;

                TimeSpan queueDiff = now - CurQueueTime;

                //Queued messages
                if (ClientMessages.Count > 0 && queueDiff.TotalMilliseconds >= BotSettings.MessageCooldown)
                {
                    if (Client.IsConnected == true)
                    {
                        string message = ClientMessages.Dequeue();
                        //Send the message
                        Client.SendMessage(LoginInformation.ChannelName, message);

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

                long end = Throttler.ElapsedTicks;
                long dur = end - start;

                long relativeWaitTime = (int)((1 / (double)CPUPercentLimit) * dur);

                Thread.Sleep((int)((relativeWaitTime / (double)Stopwatch.Frequency) * 1000));
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
            Client.OnDisconnected -= OnDisconnected;
            Client.OnBeingHosted -= OnBeingHosted;
        }

        #region Events

        private void OnConnected(object sender, OnConnectedArgs e)
        {
            TryReconnect = false;

            Console.WriteLine($"{LoginInformation.BotName} connected!");
        }

        private void OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            if (TryReconnect == false)
            {
                Console.WriteLine($"Failed to connect: {e.Error.Message}");

                TryReconnect = true;
            }
        }

        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            QueueMessage($"{LoginInformation.BotName} has connected :D ! Use {Globals.CommandIdentifier}help to display a list of commands and {Globals.CommandIdentifier}tutorial to see how to play! Input parser by Jdog, aka TwitchPlays_Everything, converted & modified by Kimimaru");

            Console.WriteLine($"Joined channel \"{e.Channel}\"");

            TryReconnect = false;

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

            userData.TotalMessages++;

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

                (bool valid, List<List<Parser.Input>> inputList, bool containsStartInput, int durationCounter)
                        parsedData = default;

                try
                {
                    string parse_message = Parser.Expandify(Parser.PopulateMacros(e.ChatMessage.Message));

                    parsedData = Parser.Parse(parse_message);
                }
                catch
                {
                    //Kimimaru: Sanitize parsing exceptions for now
                    //Most of these are currently caused by differences in how C# and Python handle slicing strings (Substring() vs string[:])
                    //One example that throws this that shouldn't is "#mash(w234"
                    //BotProgram.QueueMessage($"ERROR: {exception.Message}");
                    parsedData.valid = false;
                }

                if (parsedData.valid == false)
                {
                    //Kimimaru: Currently this also shows this for any message - keep commented until we find a better way to differentiate them
                    //Parser.Input input = parsedData.inputList[0][0];
                    //if (string.IsNullOrEmpty(input.error) == false)
                    //    BotProgram.QueueMessage($"Invalid input: {input.error}");
                }
                else
                {
                    //Ignore if user is silenced
                    if (userData.Silenced == true)
                    {
                        return;
                    }

                    if (InputGlobals.IsValidPauseInputDuration(parsedData.inputList, "start", BotData.MaxPauseHoldDuration) == false)
                    {
                        BotProgram.QueueMessage($"Invalid input: Pause button held for longer than the max duration of {BotData.MaxPauseHoldDuration} milliseconds!");
                        return;
                    }

                    if (InputHandler.StopRunningInputs == false)
                    {
                        //Mark this as a valid input
                        userData.ValidInputs++;

                        bool shouldPerformInput = true;

                        //Check the team the user is on for the controller they should be using
                        //Validate that the controller is acquired and exists
                        int controllerNum = userData.Team;

                        if (controllerNum < 0 || controllerNum >= VJoyController.Joysticks.Length)
                        {
                            BotProgram.QueueMessage($"ERROR: Invalid joystick number {controllerNum + 1}. # of joysticks: {VJoyController.Joysticks.Length}. Please change your controller port to a valid number to perform inputs.");
                            shouldPerformInput = false;
                        }
                        //Now verify that the controller has been acquired by vJoy
                        else if (VJoyController.Joysticks[controllerNum].IsAcquired == false)
                        {
                            BotProgram.QueueMessage($"ERROR: Joystick number {controllerNum + 1} with controller ID of {VJoyController.Joysticks[controllerNum].ControllerID} has not been acquired by the vJoy driver! Ensure you (the streamer) have a vJoy device set up at this ID.");
                            shouldPerformInput = false;
                        }

                        //We're okay to perform the input
                        if (shouldPerformInput == true)
                        {
                            InputHandler.CarryOutInput(parsedData.inputList, controllerNum);
                        }
                    }
                    else
                    {
                        QueueMessage("New inputs cannot be processed until all other inputs have stopped.");
                    }

                    //Debug info
                    //BotProgram.QueueMessage("Valid input!");
                    //string thing = "Valid input(s): ";
                    //
                    //for (int i = 0; i < parsedData.inputList.Count; i++)
                    //{
                    //    for (int j = 0; j < parsedData.inputList[i].Count; j++)
                    //    {
                    //        Parser.Input thing2 = parsedData.inputList[i][j];
                    //
                    //        thing += thing2.ToString() + "\n";
                    //    }
                    //}
                    //Console.WriteLine(thing);
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

        private void OnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            Console.WriteLine("Disconnected!");
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

                BotProgram.QueueMessage($"{origName} added to database!");
            }

            return userData;
        }

        public static void SaveBotData()
        {
            //Make sure more than one thread doesn't try to save at the same time to prevent potential loss of data and access violations
            lock (LockObj)
            {
                string text = JsonConvert.SerializeObject(BotData, Formatting.Indented);
                if (string.IsNullOrEmpty(text) == false)
                {
                    if (Globals.SaveToTextFile("BotData.txt", text) == false)
                    {
                        QueueMessage($"CRITICAL - Unable to save bot data");
                    }
                }
            }
        }

        public static void LoadSettingsAndBotData()
        {
            string settingsText = Globals.ReadFromTextFileOrCreate(Globals.SettingsFilename);
            BotSettings = JsonConvert.DeserializeObject<Settings>(settingsText);

            if (BotSettings == null)
            {
                Console.WriteLine("No settings found; attempting to create file template. If created, please manually fill out the information.");

                BotSettings = new Settings();
                string text = JsonConvert.SerializeObject(BotSettings, Formatting.Indented);
                Globals.SaveToTextFile(Globals.SettingsFilename, text);
            }

            string dataText = Globals.ReadFromTextFile(Globals.BotDataFilename);
            BotData = JsonConvert.DeserializeObject<BotData>(dataText);

            if (BotData == null)
            {
                Console.WriteLine("Not bot data found; initializing new bot data.");

                BotData = new BotData();
                SaveBotData();
            }
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
        }
    }
}
