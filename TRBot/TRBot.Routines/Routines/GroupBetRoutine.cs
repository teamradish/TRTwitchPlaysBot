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
using TRBot.Connection;
using TRBot.Data;

namespace TRBot.Routines
{
    /// <summary>
    /// A routine handling the group bet.
    /// </summary>
    public class GroupBetRoutine : BaseRoutine
    {
        private Random Rand = new Random();
        private int MinutesForBet = 0;

        private DateTime CurGroupBetTime = default;
        private DateTime CurMinute = default;

        private Dictionary<string, long> Participants = new Dictionary<string, long>(4);

        public GroupBetRoutine(in int minutesForBet)
        {
            Identifier = RoutineConstants.GROUP_BET_ROUTINE_ID;

            MinutesForBet = minutesForBet;
        }

        public override void Initialize(BotRoutineHandler routineHandler, DataContainer dataContainer)
        {
            base.Initialize(routineHandler, dataContainer);

            DateTime nowUTC = DateTime.UtcNow;
            CurGroupBetTime = nowUTC;
            CurMinute = nowUTC;
        }

        public override void CleanUp()
        {
            Participants.Clear();

            base.CleanUp();
        }

        /// <summary>
        /// Adds a participant to the group bet.
        /// </summary>
        /// <param name="username">The name of the participant entering the group bet.</param>
        /// <param name="betAmount">The participant's bet amount.</param>
        /// <param name="added">A return value indicating if the participant was newly added to the group bet.</param>
        public void AddParticipant(string username, in long betAmount, out bool added)
        {
            added = !Participants.ContainsKey(username);

            Participants[username] = betAmount;
        }

        /// <summary>
        /// Removes a participant from the group bet.
        /// </summary>
        /// <param name="username">The name of the participant leaving the group bet.</param>
        /// <returns>true if the participant is in the group bet and was removed from it, otherwise false.</returns>
        public bool RemoveParticipant(string username)
        {
            return Participants.Remove(username);
        }

        public override void UpdateRoutine(in DateTime currentTimeUTC)
        {
            TimeSpan diff = currentTimeUTC - CurGroupBetTime;
            TimeSpan minuteDiff = currentTimeUTC - CurMinute;

            //Remind users about the group bet every minute
            if (minuteDiff.TotalMinutes >= 1)
            {
                int minutesRemaining =  MinutesForBet - (int)Math.Round(diff.TotalMinutes, 2);

                if (minutesRemaining >= 1)
                {
                    DataContainer.MessageHandler.QueueMessage($"{minutesRemaining} minute(s) remaining for the group bet! Join while you still can! Current number of participants: {Participants.Count}");
                }

                CurMinute = currentTimeUTC;
            }

            //Not enough time passed - return
            if (diff.TotalMinutes < MinutesForBet)
            {
                return;
            }
        
            //Somehow, there are no participants by the time the group bet ends
            if (Participants.Count == 0)
            {
                DataContainer.MessageHandler.QueueMessage("Hmm, somehow there are no participants in the group bet. Which means no one wins! Oh well, try again next time (and report this as a bug)!");
                RemoveGroupBet();

                return;
            }

            //Open database context
            using BotDBContext context = DatabaseManager.OpenContext();

            //Get all participants
            List<KeyValuePair<string, long>> participants = Participants.ToList();

            //Finish group bet; choose random winner
            int randWinner = Rand.Next(0, participants.Count);
            KeyValuePair<string, long> winnerKVPair = participants[randWinner];

            string winnerName = winnerKVPair.Key;

            string invalid = string.Empty;

            //Default win is the winner's bet
            long total = winnerKVPair.Value;

            //Go through the list; make sure they still have enough credits
            for (int i = 0; i < participants.Count; i++)
            {
                KeyValuePair<string, long> curKVPair = participants[i];
                
                string curUserName = curKVPair.Key;
                long curUserBet = curKVPair.Value;

                //Skip over the winner
                if (curUserName == winnerName)
                {
                    continue;
                }

                User betUser = DataHelper.GetUserNoOpen(curUserName, context);

                //If they're no longer in the database or don't have enough credits, disqualify them
                if (betUser == null || betUser.Stats.Credits < curUserBet)
                {
                    invalid += curUserName + ", ";
                    participants.RemoveAt(i);
                    i--;
                }
                else
                {
                    //Subtract credits for those who do have enough, and add their bet to the total the winner earns
                    betUser.Stats.Credits -= curUserBet;
                    total += curUserBet;
                }
            }

            //Remove last comma and space
            if (invalid != string.Empty)
            {
                invalid = invalid.Remove(invalid.Length - 2, 2);

                //Mention who was disqualified
                DataContainer.MessageHandler.QueueMessage($"{invalid} was/were disqualified from the group bet since either their credits are now lower than their bet(s) or they were removed from the database!");
            }

            if (total > 0)
            {
                //Add credits to the winner and save
                User winnerUser = DataHelper.GetUserNoOpen(winnerName, context);

                winnerUser.Stats.Credits += total;
                
                context.SaveChanges();
                
                DataContainer.MessageHandler.QueueMessage($"{winnerName} won the group bet and {total} credit(s) PogChamp! Nice!");

                //No one else had enough credits
                if (participants.Count == 1)
                {
                    DataContainer.MessageHandler.QueueMessage("What a bummer! Everyone else was disqualified from the group bet, so the winner won only their bet!");
                }
            }

            //Update time just in case
            CurGroupBetTime = currentTimeUTC;

            //Clear participants
            Participants.Clear();

            //Stop group bet
            RemoveGroupBet();
        }

        private void RemoveGroupBet()
        {
            RoutineHandler.RemoveRoutine(Identifier);
        }
    }
}
