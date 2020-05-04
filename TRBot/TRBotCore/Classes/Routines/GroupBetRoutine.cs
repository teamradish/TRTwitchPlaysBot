/* This file is part of TRBot.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * TRBot is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with TRBot.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Client;

namespace TRBot
{
    public sealed class GroupBetRoutine : BaseRoutine
    {
        private Random Rand = new Random();
        private int MinutesForBet;

        private DateTime CurGroupBetTime;
        private DateTime CurMinute;

        public GroupBetRoutine(int minutesForDuel)
        {
            MinutesForBet = minutesForDuel;
        }
groupduel
        }

        public override void CleanUp(in TwitchClient client)
        {
            
        }

        public override void UpdateRoutine(in TwitchClient client, in DateTime currentTime)
        {
            TimeSpan diff = currentTime - CurGroupBetTime;
            TimeSpan minuteDiff = currentTime - CurMinute;

            if (minuteDiff.TotalMinutes >= 1)
            {
                int minutesRemaining =  MinutesForBet - (int)Math.Round(diff.TotalMinutes, 2);

                if (minutesRemaining >= 1)
                {
                    BotProgram.QueueMessage($"{minutesRemaining} minute(s) remaining for the group bet! Join while you still can!");
                }

                CurMinute = currentTime;
            }

            if (diff.TotalMinutes >= MinutesForBet)
            {
                //Get all participants
                List<KeyValuePair<string, long>> participants = GroupBetCommand.UsersInBet.ToList();

                //Finish group bet; choose random winner
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
                    if (BotProgram.BotData.Users[cur.Key].Credits < cur.Value)
                    {
                        invalid += cur.Key + ", ";

                        participants.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        //Subtract credits for those who do have enough, and add their bet to the total the winner earns
                        BotProgram.BotData.Users[cur.Key].SubtractCredits(cur.Value);
                        total += cur.Value;
                    }
                }

                //Remove last comma and space
                if (invalid != string.Empty)
                {
                    invalid = invalid.Remove(invalid.Length - 2, 2);

                    //Mention who was disqualified
                    BotProgram.QueueMessage($"{invalid} was/were disqualified from the group bet since their credits are now lower than their bet(s)!");
                }

                if (total > 0)
                {
                    //Add credits to the winner
                    BotProgram.BotData.Users[winner.Key].AddCredits(total);
                    BotProgram.SaveBotData();

                    BotProgram.QueueMessage($"{winner.Key} won the group bet and {total} credit(s) :D! Nice!");

                    if (participants.Count == 1)
                    {
                        BotProgram.QueueMessage("What a bummer! Everyone else was disqualified from the group bet, so the winner only won their bet!");
                    }
                }

                //Update time just in case
                CurGroupBetTime = currentTime;

                //Stop the group bet
                GroupBetCommand.UsersInBet.Clear();
                GroupBetCommand.StopGroupBet();
            }
        }
    }
}
