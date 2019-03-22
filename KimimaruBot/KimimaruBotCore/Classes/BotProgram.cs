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

namespace KimimaruBot
{
    public sealed class BotProgram : IDisposable
    {
        private static object LockObj = new object();

        private static BotProgram instance = null;

        private LoginInfo LoginInformation = null;
        public static Settings BotSettings = null;
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
            UnsubscribeEvents();

            for (int i = 0; i < BotRoutines.Count; i++)
            {
                BotRoutines[i].CleanUp(Client);
            }

            ClientMessages.Clear();
            Client.Disconnect();

            //Clean up the device when the driver is finished
            VJoyController.CleanUp();

            instance = null;
        }

        public void Initialize()
        {
            string loginText = File.ReadAllText(Globals.GetDataFilePath("LoginInfo.txt"));
            LoginInformation = JsonConvert.DeserializeObject<LoginInfo>(loginText);

            string settingsText = File.ReadAllText(Globals.GetDataFilePath("Settings.txt"));
            BotSettings = JsonConvert.DeserializeObject<Settings>(settingsText);

            string dataText = File.ReadAllText(Globals.GetDataFilePath("BotData.txt"));
            BotData = JsonConvert.DeserializeObject<BotData>(dataText);

            if (BotData == null)
            {
                BotData = new BotData();
                SaveBotData();
            }

            Credentials = new ConnectionCredentials(LoginInformation.BotName, LoginInformation.Password);

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
        }

        public void Run()
        {
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
                        Console.WriteLine(message);
                
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
            QueueMessage($"{LoginInformation.BotName} has connected :D ! Use {Globals.CommandIdentifier}help to display a list of commands! Input parser by Jdog, aka TwitchPlays_Everything, converted & modified by Kimimaru");

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
                (bool valid, List<List<Parser.Input>> inputList, bool containsStartInput, int durationCounter)
                        parsedData = default;

                try
                {
                    string parse_message = Parser.expandify(Parser.populate_macros(e.ChatMessage.Message));

                    parsedData = Parser.Parse(parse_message);
                }
                catch (Exception exception)
                {
                    //Kimimaru: Sanitize parsing exceptions for now
                    //Most of these are currently caused by differences in how C# and Python handle slicing strings (Substring() vs string[:])
                    //One example that throws this that shouldn't is "#mash(w234"
                    //BotProgram.QueueMessage($"ERROR: {exception.Message}");
                    parsedData.valid = false;
                }

                if (parsedData.valid == false)
                {
                    //Kimimaru: Currently this also shows this for commands - keep commented until we find a better way to differentiate them
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

                    if (InputHandler.StopRunningInputs == false)
                    {
                        userData.ValidInputs++;

                        InputHandler.CarryOutInput(parsedData.inputList);
                    }
                    else
                    {
                        QueueMessage("New inputs cannot be processed until all other inputs have stopped.");
                    }

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
            QueueMessage($"Thank you for subscribing, {e.Subscriber.DisplayName}!!");
        }

        private void OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            QueueMessage($"Thank you for subscribing for {e.ReSubscriber.Months} months, {e.ReSubscriber.DisplayName}!!");
        }

        private void OnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            Console.WriteLine("Disconnected!");
        }

        public static User GetUser(string username, bool isLower = true)
        {
            if (isLower == false)
            {
                username = username.ToLower();
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
                username = username.ToLower();
            }

            User userData = null;

            //Check to add a user that doesn't exist
            if (BotData.Users.TryGetValue(username, out userData) == false)
            {
                userData = new User();
                BotData.Users.Add(username, userData);

                BotProgram.QueueMessage($"{origName} added to database!");
            }

            return userData;
        }

        public static void SaveBotData()
        {
            //Make sure more than one thread doesn't try to save at the same time to prevent potential loss of data
            lock (LockObj)
            {
                string text = JsonConvert.SerializeObject(BotData, Formatting.Indented);
                if (string.IsNullOrEmpty(text) == false)
                {
                    try
                    {
                        File.WriteAllText(Globals.GetDataFilePath("BotData.txt"), text);
                    }
                    catch (Exception e)
                    {
                        QueueMessage($"CRITICAL - Unable to save bot data: {e.Message}");
                    }
                }
            }
        }

        #endregion

        private class LoginInfo
        {
            public string BotName = null;
            public string Password = null;
            public string ChannelName = null;
        }

        public class Settings
        {
            public int MessageTime = 0;
            public double MessageCooldown = 0d;
            public double CreditsTime = 0d;
            public long CreditsAmount = 50L;
            public string OwnerName = string.Empty;
        }
    }
}
