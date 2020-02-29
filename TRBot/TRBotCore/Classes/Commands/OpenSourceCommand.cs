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
            BotProgram.QueueMessage("This bot is libre software, licensed under AGPL v3.0. You can find the repository, which includes the source code and full license terms, here: https://github.com/teamradish/TRTwitchPlaysBot Ask the streamer to learn more about their own modifications, if any, to the bot.");
        }
    }
}
