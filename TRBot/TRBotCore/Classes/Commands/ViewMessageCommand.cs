using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Views the game message.
    /// </summary>
    public sealed class ViewMessageCommand : BaseCommand
    {
        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            BotProgram.QueueMessage($"Current game message: \"{BotProgram.BotData.GameMessage}\"");
        }
    }
}
