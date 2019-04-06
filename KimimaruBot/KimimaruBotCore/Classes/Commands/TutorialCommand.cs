using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    public sealed class TutorialCommand : BaseCommand
    {
        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            BotProgram.QueueMessage($"Hi {e.Command.ChatMessage.Username}, here's how to play: https://twitchplays.fandom.com/wiki/Welcome_to_TPE");
        }
    }
}
