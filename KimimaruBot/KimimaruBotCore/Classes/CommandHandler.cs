using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Client.Events;
using Newtonsoft.Json;

namespace KimimaruBot
{
    /// <summary>
    /// Handles commands.
    /// </summary>
    public sealed class CommandHandler
    {
        public Dictionary<string, BaseCommand> CommandDict = new Dictionary<string, BaseCommand>();

        private readonly string[] ExemptUsers = new string[]
        {
            "mrmacrobot"
        };

        private TwitchClient Client = null;

        public CommandHandler(TwitchClient client)
        {
            Client = client;
            Initialize();
        }

        private void Initialize()
        {
            CommandDict.Add("help", new HelpCommand());
            //CommandDict.Add("schedule", new ScheduleCommand());
            //CommandDict.Add("suggestions", new SuggestionsCommand());
            CommandDict.Add("credits", new CreditsCommand());
            CommandDict.Add("transfer", new TransferCommand());
            CommandDict.Add("bet", new BetCommand());
            CommandDict.Add("duel", new DuelCommand());
            CommandDict.Add("accept", new AcceptCommand());
            CommandDict.Add("deny", new DenyCommand());
            CommandDict.Add("averagecredits", new AverageCreditsCommand());
            CommandDict.Add("mediancredits", new MedianCreditsCommand());
            CommandDict.Add("highestcredits", new HighestCreditsCommand());
            CommandDict.Add("say", new SayCommand());
            CommandDict.Add("randnum", new RandNumCommand());
            CommandDict.Add("inspiration", new InspirationCommand());
            CommandDict.Add("feed", new FeedCommand());
            CommandDict.Add("jumprope", new JumpRopeCommand());
            CommandDict.Add("jumpropestreak", new HighestJumpRopeCommand());
            CommandDict.Add("calculate", new CalculateCommand());
            CommandDict.Add("memes", new MemesCommand());
            CommandDict.Add("addmeme", new AddMemeCommand());
            CommandDict.Add("removememe", new RemoveMemeCommand());
            CommandDict.Add("highfive", new HighFiveCommand());
            CommandDict.Add("groupduel", new GroupDuelCommand());
            CommandDict.Add("outgroupduel", new OutGroupDuelCommand());
            CommandDict.Add("crashbot", new CrashBotCommand());
            CommandDict.Add("console", new ConsoleCommand());
            CommandDict.Add("stopall", new StopAllCommand());
            CommandDict.Add("macros", new MacrosCommand());
            CommandDict.Add("addmacro", new AddMacroCommand());
            CommandDict.Add("removemacro", new RemoveMacroCommand());
            CommandDict.Add("show", new ShowCommand());
            CommandDict.Add("savestate", new SavestateCommand());
            CommandDict.Add("loadstate", new LoadstateCommand());
            CommandDict.Add("viewstate", new ViewstateCommand());
            CommandDict.Add("ss", new SavestateCommand() { HiddenFromHelp = true });
            CommandDict.Add("ls", new LoadstateCommand() { HiddenFromHelp = true });
            CommandDict.Add("vs", new ViewstateCommand() { HiddenFromHelp = true });
            CommandDict.Add("setlevel", new SetLevelCommand());
            CommandDict.Add("level", new LevelCommand());
            CommandDict.Add("log", new LogCommand());
            CommandDict.Add("viewlog", new ViewLogCommand());
            CommandDict.Add("numlogs", new NumLogsCommand());
            CommandDict.Add("time", new TimeCommand());
            CommandDict.Add("length", new LengthCommand());
            CommandDict.Add("exec", new ExecCommand());

            foreach (KeyValuePair<string, BaseCommand> command in CommandDict)
            {
                command.Value.Initialize(this);
            }
        }

        public void HandleCommand(object sender, OnChatCommandReceivedArgs e)
        {
            if (e == null || e.Command == null || e.Command.ChatMessage == null)
            {
                Console.WriteLine($"{nameof(OnChatCommandReceivedArgs)} or its Command or ChatMessage is null! Not parsing command");
                return;
            }

            string userToLower = e.Command.ChatMessage.DisplayName.ToLower();

            //Don't handle certain users (Ex. MrMacroBot)
            if (ExemptUsers.Contains(userToLower) == true)
            {
                Console.WriteLine($"User {e.Command.ChatMessage.DisplayName} is exempt and I won't take commands from them");
                return;
            }

            User user = BotProgram.GetOrAddUser(userToLower);

            string commandToLower = e.Command.CommandText.ToLower();

            if (CommandDict.TryGetValue(commandToLower, out BaseCommand command) == true)
            {
                //Handle permissions
                if (user.Level >= command.AccessLevel)
                {
                    command.ExecuteCommand(sender, e);
                }
                else
                {
                    BotProgram.QueueMessage("You don't have permission to do that!");
                }
            }
        }
    }
}
