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
        private static BotProgram instance = null;

        private LoginInfo LoginInformation = null;
        public static Settings BotSettings = null;

        private TwitchClient Client;
        private ConnectionCredentials Credentials = null;
        private CrashHandler crashHandler = null;

        private CommandHandler CommandHandler = null;

        private bool TryReconnect = false;

        private Dictionary<string, bool> UsersTalked = new Dictionary<string, bool>();

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

            instance = null;
        }

        public void Initialize()
        {
            string loginText = File.ReadAllText(Globals.GetDataFilePath("LoginInfo.txt"));
            LoginInformation = JsonConvert.DeserializeObject<LoginInfo>(loginText);

            string settingsText = File.ReadAllText(Globals.GetDataFilePath("Settings.txt"));
            BotSettings = JsonConvert.DeserializeObject<Settings>(settingsText);

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
            QueueMessage($"{LoginInformation.BotName} has connected :D ! Use {Globals.CommandIdentifier}help to display a list of commands!");

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
            string possibleMeme = e.ChatMessage.Message.ToLower();
            if (MemesCommand.Memes.ContainsKey(possibleMeme) == true)
            {
                BotProgram.QueueMessage(MemesCommand.Memes[possibleMeme]);
            }
            else
            {
                try
                {
                    Parser.Input input = new Parser.Input();
                    string parse_message = input.expandify(input.populate_macros(e.ChatMessage.Message));
                    (bool valid, List<Parser.Input> inputList, bool containsStartInput, int durationCounter)
                        parsedData = input.Parse(parse_message);

                    if (parsedData.valid == false)
                    {
                        if (string.IsNullOrEmpty(input.error) == false)
                            BotProgram.QueueMessage($"Invalid input: {input.error}");
                    }
                    else
                    {
                        BotProgram.QueueMessage("Valid input!");
                        string thing = "Valid input(s): ";

                        for (int i = 0; i < parsedData.inputList.Count; i++)
                        {
                            Parser.Input thing2 = parsedData.inputList[i];

                            thing += thing2.ToString() + "\n";
                        }
                        Console.WriteLine(thing);
                    }
                }
                catch (Exception exception)
                {
                    //Kimimaru: Sanitize parsing exceptions for now
                    //Most of these are currently caused by differences in how C# and Python handle slicing strings (Substring() vs string[:])
                    //One example that throws this that shouldn't is "#mash(w234"
                    //BotProgram.QueueMessage($"ERROR: {exception.Message}");
                }
            }
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
