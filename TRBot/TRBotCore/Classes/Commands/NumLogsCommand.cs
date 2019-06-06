using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Tells how many game logs exist.
    /// </summary>
    public sealed class NumLogsCommand : BaseCommand
    {
        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            int numLogs = BotProgram.BotData.Logs.Count;

            if (numLogs == 1)
            {
                BotProgram.QueueMessage("There is 1 game log!");
            }
            else if (numLogs > 0)
            {
                BotProgram.QueueMessage($"There are {numLogs} game logs!");
            }
            else
            {
                BotProgram.QueueMessage("There are no game logs!");
            }
        }
    }
}
