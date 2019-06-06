using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Client.Events;

namespace TRBot
{
    public sealed class HighestJumpRopeCommand : BaseCommand
    {
        public HighestJumpRopeCommand()
        {

        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            if (BotProgram.BotData != null && BotProgram.BotData.JRData != null)
            {
                BotProgram.QueueMessage($"The biggest jump rope streak is {BotProgram.BotData.JRData.Streak}!");
            }
            else
            {
                BotProgram.QueueMessage("Something went wrong - I can't find the highest Jump Rope streak!");
            }
        }
    }
}
