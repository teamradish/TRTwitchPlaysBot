using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace TRBot
{
    public sealed class OpenSourceCommand : BaseCommand
    {
        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            BotProgram.QueueMessage("This bot is open source! You can find the repository here: https://github.com/teamradish/TRTwitchPlaysBot");
        }
    }
}
