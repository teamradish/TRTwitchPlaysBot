using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace TRBot
{
    public sealed class SuggestionsCommand : BaseCommand
    {
        public SuggestionsCommand()
        {

        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            BotProgram.QueueMessage("Game Suggestions: https://docs.google.com/document/d/1Em-Lq4BKyvBICX1RF-4Ndt-P2mZeY4x9VZ1k63miMb8/edit?usp=sharing");
        }
    }
}
