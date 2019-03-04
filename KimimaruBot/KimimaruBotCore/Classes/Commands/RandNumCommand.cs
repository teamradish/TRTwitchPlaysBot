using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    public sealed class RandNumCommand : BaseCommand
    {
        public Random Rand = new Random();

        public RandNumCommand()
        {

        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> argList = e.Command.ArgumentsAsList;

            if (argList.Count <= 1 || argList.Count >= 3)
            {
                BotProgram.QueueMessage("I'm sorry; please enter a range of two whole numbers!");
            }
            else
            {
                if (int.TryParse(argList[0], out int num1) && int.TryParse(argList[1], out int num2))
                {
                    int min = Math.Min(num1, num2);
                    int max = Math.Max(num1, num2);

                    int randNum = Rand.Next(min, max);
                    BotProgram.QueueMessage($"The random number between {min} and {max} is: {randNum}! Play again!");
                }
                else
                {
                    BotProgram.QueueMessage("I'm sorry; please enter a range of two whole numbers!");
                }
            }
        }
    }
}
