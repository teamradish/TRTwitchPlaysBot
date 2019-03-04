using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    public sealed class HighestJumpRopeCommand : BaseCommand
    {
        private JumpRopeCommand JumpRope = null;

        public HighestJumpRopeCommand()
        {

        }

        public override void Initialize(CommandHandler commandHandler)
        {
            const string jumpRope = "jumprope";
            if (commandHandler.CommandDict.ContainsKey(jumpRope) == true)
            {
                JumpRope = commandHandler.CommandDict[jumpRope] as JumpRopeCommand;
            }
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            if (JumpRope != null && JumpRope.JRStreak != null)
            {
                BotProgram.QueueMessage($"The biggest jump rope streak is {JumpRope.JRStreak.Streak}!");
            }
            else
            {
                BotProgram.QueueMessage("Something went wrong - I can't find the highest Jump Rope streak!");
            }
        }
    }
}
