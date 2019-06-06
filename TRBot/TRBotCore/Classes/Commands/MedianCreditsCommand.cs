using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace TRBot
{
    public sealed class MedianCreditsCommand : BaseCommand
    {
        private Comparison<(string, long)> CreditsCompare = null;

        public MedianCreditsCommand()
        {

        }

        public override void Initialize(CommandHandler commandHandler)
        {
            base.Initialize(commandHandler);

            //Set here to reduce garbage since it normally creates a new delegate instance each time you pass in the method name directly
            CreditsCompare = CompareKeyVal;
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<(string, long)> allCredits = new List<(string, long)>(BotProgram.BotData.Users.Count);

            foreach (var kvPair in BotProgram.BotData.Users)
            {
                allCredits.Add((kvPair.Key, kvPair.Value.Credits));
            }

            allCredits.Sort(CreditsCompare);

            int medianIndex = allCredits.Count / 2;

            if (medianIndex >= allCredits.Count)
            {
                BotProgram.QueueMessage("Sorry, there's not enough data in the credits database for that!");
            }
            else
            {
                long median = allCredits[medianIndex].Item2;

                BotProgram.QueueMessage($"The median number of credits in the database is {median}!");
            }
        }

        private int CompareKeyVal((string, long) val1, (string, long) val2)
        {
            return val1.Item2.CompareTo(val2.Item2);
        }
    }
}
