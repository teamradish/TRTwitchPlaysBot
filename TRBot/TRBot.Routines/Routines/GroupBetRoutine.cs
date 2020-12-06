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
using TRBot.Utilities;

namespace TRBot.Routines
{
    /// <summary>
    /// A routine handling the group bet.
    /// </summary>
    public class GroupBetRoutine : BaseRoutine
    {
        public long MillisecondsForBet { get; private set; } = 0L;
        public int MinParticipants { get; private set; }= 3;

        private DateTime GroupBetStartTime = default;
        private int CurMinute = 0;
        private TimeSpan TotalTime = TimeSpan.Zero;
        private Random Rand = new Random();

        private Dictionary<string, long> Participants = new Dictionary<string, long>(4);

        /// <summary>
        /// The number of participants in the group bet.
        /// </summary>
        public int ParticipantCount => Participants.Count;

        public GroupBetRoutine(in long millisecondsForBet, in int minParticipants)
        {
            Identifier = RoutineConstants.GROUP_BET_ROUTINE_ID;

            MillisecondsForBet = millisecondsForBet;
            MinParticipants = minParticipants;

            TotalTime = TimeSpan.FromMilliseconds(MillisecondsForBet);
        }

        public override void Initialize()
        {
            base.Initialize();

            DateTime nowUTC = DateTime.UtcNow;
            GroupBetStartTime = nowUTC;

            CurMinute = 0;
        }

        public override void CleanUp()
        {
            CurMinute = 0;
            Participants.Clear();

            base.CleanUp();
        }

        /// <summary>
        /// Retrieves data about a participant from the group bet.
        /// </summary>
        /// <param name="username">The name of the participant in the group bet.</param>
        /// <param name="participantData">Returned data about the participant if found in the group bet.</param>
        /// <returns>true if the participant is in the group bet, otherwise false.</returns>
        public bool TryGetParticipant(string username, out ParticipantData participantData)
        {
            if (Participants.TryGetValue(username, out long betAmount) == true)
            {
                participantData = new ParticipantData(username, betAmount);
                return true;
            }

            participantData = default;
            return false;
        }

        /// <summary>
        /// Adds a participant to the group bet or updates their information if already participating.
        /// </summary>
        /// <param name="username">The name of the participant entering the group bet.</param>
        /// <param name="betAmount">The participant's bet amount.</param>
        public void AddOrUpdateParticipant(string username, in long betAmount)
        {
            bool added = !Participants.ContainsKey(username);

            Participants[username] = betAmount;

            //Update the time if the we added the last participant required to start
            if (added == true && ParticipantCount == MinParticipants)
            {
                DateTime nowUTC = DateTime.UtcNow;
                GroupBetStartTime = nowUTC;

                CurMinute = 0;
            }
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
            //Return if we don't have enough participants
            if (ParticipantCount < MinParticipants)
            {
                return;
            }

            //Get time difference and time remaining
            TimeSpan diff = currentTimeUTC - GroupBetStartTime;

            //Floor the total seconds so the millisecond component doesn't get in the way
            //This lets the time remaining second count stay in sync with the time difference second count
            //The millisecond component would otherwise make it one second off (Ex. saying 29 seconds remaining instead of 30)
            TimeSpan timeRemaining = TotalTime - TimeSpan.FromSeconds((long)Math.Floor(diff.TotalSeconds));

            //Console.WriteLine($"CurMinute: {CurMinute} | diff minutes: {diff.Minutes} Seconds: {diff.Seconds} | Remaining: Min: {timeRemaining.Minutes} Sec: {timeRemaining.Seconds}");

            //Remind users about the group bet every minute
            if (diff.Minutes > CurMinute && diff.TotalMilliseconds < MillisecondsForBet)
            {
                CurMinute = (int)diff.Minutes;

                long minutesRemaining = (long)timeRemaining.Minutes;
                long secondsRemaining = (long)timeRemaining.Seconds;

                //If there are minutes remaining state them, otherwise state only the seconds remaining
                if (minutesRemaining > 0)
                {
                    DataContainer.MessageHandler.QueueMessage($"{minutesRemaining} minute(s) and {secondsRemaining} second(s) remaining for the group bet! Join while you still can! Current number of participants: {Participants.Count}");
                }
                else
                {
                    DataContainer.MessageHandler.QueueMessage($"{secondsRemaining} second(s) remaining for the group bet! Join while you still can! Current number of participants: {Participants.Count}");
                }
            }

            //Not enough time passed - return
            if (diff.TotalMilliseconds < MillisecondsForBet)
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

            string creditsName = DataHelper.GetCreditsName();

            //Get all participants
            List<KeyValuePair<string, long>> participants = Participants.ToList();

            //Finish group bet; choose random winner
            int randWinner = Rand.Next(0, participants.Count);
            KeyValuePair<string, long> winnerKVPair = participants[randWinner];

            string winnerName = winnerKVPair.Key;

            string invalid = string.Empty;

            //Default win is the winner's bet
            long total = winnerKVPair.Value;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
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

                        //Save their loss
                        context.SaveChanges();
                    }
                }
            }

            //Remove last comma and space
            if (invalid != string.Empty)
            {
                invalid = invalid.Remove(invalid.Length - 2, 2);

                //Mention who was disqualified
                DataContainer.MessageHandler.QueueMessage($"{invalid} was/were disqualified from the group bet since either their {creditsName.Pluralize(false, 0)} are now lower than their bet(s) or they were removed from the database!");
            }

            if (total > 0)
            {
                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    //Add credits to the winner and save
                    User winnerUser = DataHelper.GetUserNoOpen(winnerName, context);

                    winnerUser.Stats.Credits += total;

                    context.SaveChanges();
                }
                
                DataContainer.MessageHandler.QueueMessage($"{winnerName} won the group bet and {total} {creditsName.Pluralize(false, total)} PogChamp! Nice!");

                //If there's more than one minimum participants and no one else had enough credits, mention the disappointment
                if (participants.Count == 1 && MinParticipants > 1)
                {
                    DataContainer.MessageHandler.QueueMessage("What a bummer! Everyone else was disqualified from the group bet, so the winner won only their bet!");
                }
            }

            //Update time just in case
            GroupBetStartTime = currentTimeUTC;

            //Clear participants
            Participants.Clear();

            //Stop group bet
            RemoveGroupBet();
        }

        private void RemoveGroupBet()
        {
            RoutineHandler.RemoveRoutine(Identifier);
        }

        /// <summary>
        /// Represents data about a participant in the group bet.
        /// </summary>
        public struct ParticipantData
        {
            /// <summary>
            /// The name of the participant.
            /// </summary>
            public string ParticipantName;
            
            /// <summary>
            /// The participant's bet amount.
            /// </summary>
            public long ParticipantBet;

            public ParticipantData(string participantName, in long participantBet)
            {
                ParticipantName = participantName;
                ParticipantBet = participantBet;
            }

            public override bool Equals(object obj)
            {
                if (obj is ParticipantData pData)
                {
                    return (this == pData);
                }

                return false;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 3;
                    hash = (hash * 37) + ((ParticipantName == null) ? 0 : ParticipantName.GetHashCode());
                    hash = (hash * 37) + ParticipantBet.GetHashCode();
                    return hash;
                } 
            }

            public static bool operator==(ParticipantData a, ParticipantData b)
            {
                return (a.ParticipantName == b.ParticipantName && a.ParticipantBet == b.ParticipantBet);
            }

            public static bool operator!=(ParticipantData a, ParticipantData b)
            {
                return !(a == b);
            }
        }
    }
}
