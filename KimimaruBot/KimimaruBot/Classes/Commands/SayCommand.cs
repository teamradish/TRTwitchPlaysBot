using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    public sealed class SayCommand : BaseCommand
    {
        public SayCommand()
        {

        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            if (e.Command.ChatMessage.Message.Length > 5)
            {
                string realMsg = e.Command.ChatMessage.Message.Remove(0, 5).Trim();// + ". The statement said to me expresses the views of the one instructing me, not myself :D";
                if (realMsg.StartsWith("/") == true)
                {
                    BotProgram.QueueMessage("I can't say any Twitch chat commands for you - no hard feelings!");
                }
                else
                {
                    BotProgram.QueueMessage(realMsg);
                }
            }
        }
    }
}
