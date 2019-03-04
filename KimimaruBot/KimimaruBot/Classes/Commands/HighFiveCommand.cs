using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    public sealed class HighFiveCommand : BaseCommand
    {
        public HighFiveCommand()
        {

        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args == null || args.Count <= 0)
            {
                BotProgram.QueueMessage("Choose one or more people to high five!");
                return;
            }

            string highFive = string.Empty;
            for (int i = 0; i < args.Count; i++)
            {
                int nextInd = i + 1;

                if (i > 0)
                {
                    highFive += ", ";
                    if (nextInd == args.Count)
                    {
                        highFive += "and ";
                    }
                }

                highFive += args[i];
            }

            BotProgram.QueueMessage($"{e.Command.ChatMessage.DisplayName} high fives {highFive}!");
        }
    }
}
