using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    public sealed class ScheduleCommand : BaseCommand
    {
        public ScheduleCommand()
        {

        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            BotProgram.QueueMessage("http://twitchplays.wikia.com/wiki/Hiatus_Programming");
        }
    }
}
