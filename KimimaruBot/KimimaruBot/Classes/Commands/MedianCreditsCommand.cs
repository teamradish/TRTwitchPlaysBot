using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    public sealed class MedianCreditsCommand : BaseCommand
    {
        public MedianCreditsCommand()
        {

        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<KeyValuePair<string, long>> allCredits = CreditsCommand.UserCredits.ToList();

            allCredits.Sort(CompareKeyVal);

            int medianIndex = allCredits.Count / 2;

            if (medianIndex >= allCredits.Count)
            {
                BotProgram.QueueMessage("Sorry, there's not enough data in the credits database for that!");
            }
            else
            {
                long median = allCredits[medianIndex].Value;

                BotProgram.QueueMessage($"The median number of credits in the database is {median}!");
            }
        }

        private int CompareKeyVal(KeyValuePair<string, long> val1, KeyValuePair<string, long> val2)
        {
            return val1.Value.CompareTo(val2.Value);
        }
    }
}
