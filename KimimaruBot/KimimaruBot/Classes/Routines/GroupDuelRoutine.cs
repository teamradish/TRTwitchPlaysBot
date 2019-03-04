using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;

namespace KimimaruBot
{
    public sealed class GroupDuelRoutine : BaseRoutine
    {
        private Random Rand = new Random();
        private int MinutesForDuel;

        private DateTime CurGroupDuelTime;
        private DateTime CurMinute;

        public GroupDuelRoutine(int minutesForDuel)
        {
            MinutesForDuel = minutesForDuel;
        }

        public override void Initialize(in TwitchClient client)
        {
            CurGroupDuelTime = DateTime.Now;
            CurMinute = CurGroupDuelTime;
        }

        public override void CleanUp(in TwitchClient client)
        {
            
        }

        public override void UpdateRoutine(in TwitchClient client, in DateTime currentTime)
        {
            TimeSpan diff = currentTime - CurGroupDuelTime;
            TimeSpan minuteDiff = currentTime - CurMinute;

            if (minuteDiff.TotalMinutes >= 1)
            {
                int minutesRemaining =  MinutesForDuel - (int)Math.Round(diff.TotalMinutes, 2);

                if (minutesRemaining >= 1)
                {
                    BotProgram.QueueMessage($"{minutesRemaining} minute(s) remaining for the group duel! Join while you still can!");
                }

                CurMinute = currentTime;
            }

            if (diff.TotalMinutes >= MinutesForDuel)
            {
                //Get all participants
                List<KeyValuePair<string, long>> participants = GroupDuelCommand.UsersInDuel.ToList();

                //Finish group duel; choose random winner
                int randWinner = Rand.Next(0, participants.Count);

                KeyValuePair<string, long> winner = participants[randWinner];

                string invalid = string.Empty;

                //Default win is the winner's bet
                long total = winner.Value;

                //Go through the list; make sure they still have enough credits
                for (int i = 0; i < participants.Count; i++)
                {
                    KeyValuePair<string, long> cur = participants[i];

                    //Skip over the winner
                    if (cur.Key == winner.Key) continue;

                    //If they don't have enough credits, disqualify them
                    if (CreditsCommand.UserCredits[cur.Key] < cur.Value)
                    {
                        invalid += cur.Key + ", ";

                        participants.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        //Subtract credits for those who do have enough, and add their bet to the total the winner earns
                        CreditsCommand.UserCredits[cur.Key] -= cur.Value;
                        total += cur.Value;
                    }
                }

                //Remove last comma and space
                if (invalid != string.Empty)
                {
                    invalid = invalid.Remove(invalid.Length - 2, 2);

                    //Mention who was disqualified
                    BotProgram.QueueMessage($"{invalid} was/were disqualified from the group duel since their credits are now lower than their bet(s)!");
                }

                if (total > 0)
                {
                    //Add credits to the winner
                    CreditsCommand.UserCredits[winner.Key] += total;
                    CreditsCommand.SaveDict();

                    BotProgram.QueueMessage($"{winner.Key} won the group duel and {total} credit(s) :D! Nice!");

                    if (participants.Count == 1)
                    {
                        BotProgram.QueueMessage("What a bummer! Everyone else was disqualified from the group duel, so the winner only won their bet!");
                    }
                }

                //Update time just in case
                CurGroupDuelTime = currentTime;

                //Stop the group duel
                GroupDuelCommand.UsersInDuel.Clear();
                GroupDuelCommand.StopGroupDuel();
            }
        }
    }
}
