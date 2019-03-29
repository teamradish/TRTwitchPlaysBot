using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    /// <summary>
    /// Tells you the time!
    /// </summary>
    public sealed class TimeCommand : BaseCommand
    {
        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            DateTime curTime = DateTime.UtcNow;

            BotProgram.QueueMessage($"The current time is {curTime.ToLongTimeString()} on {curTime.ToShortDateString()} (UTC)!");
        }
    }
}
