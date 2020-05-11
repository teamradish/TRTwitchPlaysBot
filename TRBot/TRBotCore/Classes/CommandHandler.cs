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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Client.Events;
using Newtonsoft.Json;

namespace TRBot
{
    /// <summary>
    /// Handles commands.
    /// </summary>
    public sealed class CommandHandler
    {
        public Dictionary<string, BaseCommand> CommandDict = new Dictionary<string, BaseCommand>();
        public const string INVALID_ACCESS_MESSAGE = "You don't have permission to do that!";

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
            CommandDict.Add("chat", new ChatBotCommand());
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
            CommandDict.Add("groupbet", new GroupBetCommand());
            CommandDict.Add("outgroupbet", new OutGroupBetCommand());
            CommandDict.Add("crashbot", new CrashBotCommand());
            CommandDict.Add("console", new ConsoleCommand());
            CommandDict.Add("stopall", new StopAllCommand());
            CommandDict.Add("macros", new MacrosCommand());
            CommandDict.Add("addmacro", new AddMacroCommand());
            CommandDict.Add("removemacro", new RemoveMacroCommand());
            CommandDict.Add("show", new ShowCommand());
            CommandDict.Add("savestate", new SavestateCommand() { HiddenFromHelp = true });
            CommandDict.Add("loadstate", new LoadstateCommand() { HiddenFromHelp = true });
            CommandDict.Add("viewstate", new ViewstateCommand() { HiddenFromHelp = true });
            CommandDict.Add("ss", new SavestateCommand() { HiddenFromHelp = true });
            CommandDict.Add("ls", new LoadstateCommand() { HiddenFromHelp = true });
            CommandDict.Add("vs", new ViewstateCommand() { HiddenFromHelp = true });
            CommandDict.Add("info", new InfoCommand());
            CommandDict.Add("setinfo", new SetInfoCommand());
            CommandDict.Add("setlevel", new SetLevelCommand());
            CommandDict.Add("level", new LevelCommand());
            CommandDict.Add("log", new LogCommand());
            CommandDict.Add("viewlog", new ViewLogCommand());
            CommandDict.Add("numlogs", new NumLogsCommand());
            CommandDict.Add("time", new TimeCommand());
            CommandDict.Add("length", new LengthCommand());
            CommandDict.Add("exec", new ExecCommand());
            CommandDict.Add("sleep", new SetSleepCommand() { HiddenFromHelp = true });
            CommandDict.Add("setmessage", new SetMessageCommand());
            CommandDict.Add("viewmessage", new ViewMessageCommand());
            CommandDict.Add("silence", new SilenceCommand());
            CommandDict.Add("unsilence", new UnsilenceCommand());
            CommandDict.Add("viewsilence", new ViewSilencedCommand());
            CommandDict.Add("userinfo", new UserInfoCommand());
            CommandDict.Add("inputs", new ValidInputsCommand());
            CommandDict.Add("controllers", new ControllerCountCommand());
            CommandDict.Add("setcontrollers", new SetControllersCommand());
            CommandDict.Add("player", new ChangePlayerCommand());
            CommandDict.Add("defaultinputdur", new DefaultInputDurationCommand());
            CommandDict.Add("maxinputdur", new MaxInputDurationCommand());
            CommandDict.Add("sourcecode", new OpenSourceCommand());
            CommandDict.Add("reload", new ReloadCommand());
            //CommandDict.Add("maxpause", new MaxPauseDurationCommand());
            CommandDict.Add("vcontroller", new VirtualControllerCommand());
            CommandDict.Add("inputperms", new InputPermissionsCommand());
            CommandDict.Add("reverse", new ReverseCommand());
            CommandDict.Add("toggleoptstats", new OptStatsCommand());
            CommandDict.Add("clearuserstats", new ClearStatsCommand());
            //CommandDict.Add("achievements", new ListAchievementsCommand());
            //CommandDict.Add("achinfo", new AchievementInfoCommand());
            CommandDict.Add("exercise", new ExerciseCommand());
            CommandDict.Add("tutorial", new TutorialCommand());

            foreach (KeyValuePair<string, BaseCommand> command in CommandDict)
            {
                command.Value.Initialize(this);
            }
        }

        public void CleanUp()
        {
            foreach (KeyValuePair<string, BaseCommand> command in CommandDict)
            {
                command.Value.CleanUp();
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
                    BotProgram.QueueMessage(INVALID_ACCESS_MESSAGE);
                }
            }
        }
    }
}
