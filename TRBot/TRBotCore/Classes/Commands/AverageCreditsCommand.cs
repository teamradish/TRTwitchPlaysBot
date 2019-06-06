using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace TRBot
{
    public sealed class AverageCreditsCommand : BaseCommand
    {
        public AverageCreditsCommand()
        {

        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            User[] users = BotProgram.BotData.Users.Values.ToArray();

            long average = 0L;

            for (int i = 0; i < users.Length; i++)
            {
                average += users[i].Credits;
            }

            average /= users.Length;

            BotProgram.QueueMessage($"The average number of credits in the database is {average}!");
        }
    }
}
