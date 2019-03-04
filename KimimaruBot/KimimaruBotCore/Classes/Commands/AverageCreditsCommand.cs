using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    public sealed class AverageCreditsCommand : BaseCommand
    {
        public AverageCreditsCommand()
        {

        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            long[] vals = CreditsCommand.UserCredits.Values.ToArray();

            long average = 0L;

            for (int i = 0; i < vals.Length; i++)
            {
                average += vals[i];
            }

            average /= vals.Length;

            BotProgram.QueueMessage($"The average number of credits in the database is {average}!");
        }
    }
}
