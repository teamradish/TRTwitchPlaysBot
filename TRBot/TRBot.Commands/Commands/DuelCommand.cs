/* Copyright (C) 2019-2020 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot,software for playing games through text.
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
using TRBot.Misc;
using TRBot.Utilities;
using TRBot.Consoles;
using TRBot.Parsing;
using TRBot.Data;
using TRBot.Permissions;

namespace TRBot.Commands
{
    /// <summary>
    /// Lets a user duel another for credits.
    /// </summary>
    public class DuelCommand : BaseCommand
    {
        private const string ACCEPT_DUEL_ARG = "accept";
        private const string DENY_DUEL_ARG = "deny"; 

        /// <summary>
        /// The dictionary to hold data on pending duels.
        /// </summary>
        private Dictionary<string, DuelData> DuelRequests = null;

        private string UsageMessage = "Usage: \"user to duel - or (accept/deny)\" \"bet amount (int)\"";
        private Random Rand = new Random();

        public DuelCommand()
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();

            if (DuelRequests == null)
            {
                DuelRequests = new Dictionary<string, DuelData>();
            }
        }

        public override void CleanUp()
        {
            if (DuelRequests != null)
            {
                DuelRequests.Clear();
                DuelRequests = null;
            }

            base.CleanUp();
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count == 0 || arguments.Count > 2)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string userName = args.Command.ChatMessage.Username.ToLowerInvariant();
            string opponentArg = arguments[0].ToLowerInvariant();

            //Get the users
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User thisUser = DataHelper.GetUserNoOpen(userName, context);

                //Confirm this user is in the database
                if (thisUser == null)
                {
                    QueueMessage("You cannot duel because you are not in the database!");
                    return;
                }

                //Confirm the user has the ability to duel
                if (thisUser.HasEnabledAbility(PermissionConstants.DUEL_ABILITY) == false)
                {
                    QueueMessage("You do not have the ability to duel!");
                    return;
                }
            }

            //Handle accepting or denying a duel with only one argument
            if (arguments.Count == 1)
            {
                if (opponentArg == ACCEPT_DUEL_ARG || opponentArg == DENY_DUEL_ARG)
                {
                    AcceptDenyDuel(userName, opponentArg);
                }
                else
                {
                    QueueMessage(UsageMessage);
                }

                return;
            }

            //Prevent dueling yourself
            if (userName == opponentArg)
            {
                QueueMessage("You cannot duel yourself!");
                return;
            }

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User thisUser = DataHelper.GetUserNoOpen(userName, context);
                
                //Get the opponent
                User opponentUser = DataHelper.GetUserNoOpen(opponentArg, context);

                if (opponentUser == null)
                {
                    QueueMessage($"{opponentArg} is not in the database!");
                    return;
                }

                if (thisUser.IsOptedOut == true)
                {
                    QueueMessage("You can't duel if you're opted out of bot stats!");
                    return;
                }

                //Confirm the opponent has the ability to duel
                if (opponentUser.HasEnabledAbility(PermissionConstants.DUEL_ABILITY) == false)
                {
                    QueueMessage("The one you're attempting to duel does not have the ability to duel!");
                    return;
                }

                if (opponentUser.IsOptedOut == true)
                {
                    QueueMessage("The one you're attempting to duel is opted out of bot stats, so you can't duel them!");
                    return;
                }
            }

            long duelTimeout = DataHelper.GetSettingInt(SettingsConstants.DUEL_TIMEOUT, 60000L);
            string creditsName = DataHelper.GetCreditsName();

            //Check if the duel expired and replace it with this one if so
            if (DuelRequests.ContainsKey(userName) == true)
            {
                DuelData data = DuelRequests[userName];
                TimeSpan diff = DateTime.Now - data.DuelStartTimeUTC;
                if (diff.TotalMilliseconds >= duelTimeout)
                {
                    DuelRequests.Remove(userName);
                }
                else
                {
                    QueueMessage($"You're still waiting on a duel response from {opponentArg}!");
                    return;
                }
            }

            string betStr = arguments[1];

            long betAmount = -1L;
            bool success = long.TryParse(betStr, out betAmount);
            if (success == false || betAmount <= 0)
            {
                QueueMessage($"Please specify a positive whole number of {creditsName.Pluralize(false, 0)} greater than 0!");
                return;
            }

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User thisUser = DataHelper.GetUserNoOpen(userName, context);
                User opponentUser = DataHelper.GetUserNoOpen(opponentArg, context);

                //Validate credit amounts
                if (thisUser.Stats.Credits < betAmount || opponentUser.Stats.Credits < betAmount)
                {
                    QueueMessage($"Either you or the one you're dueling does not have enough {creditsName.Pluralize(false, 0)} for this duel!");
                    return;
                }
            }

            //Add to the duel requests
            DuelRequests.Add(opponentArg, new DuelData(userName, betAmount, DateTime.UtcNow));

            TimeSpan totalTime = TimeSpan.FromMilliseconds(duelTimeout);

            QueueMessage($"{userName} has requested to duel {opponentArg} for {betAmount} {creditsName.Pluralize(false, betAmount)}! Pass \"{ACCEPT_DUEL_ARG}\" as an argument to duel or \"{DENY_DUEL_ARG}\" as an argument to refuse. The duel expires in {totalTime.TotalMinutes} minute(s), {totalTime.Seconds} second(s)!");
        }

        private void AcceptDenyDuel(string thisUserName, string argument)
        {
            long duelTimeout = DataHelper.GetSettingInt(SettingsConstants.DUEL_TIMEOUT, 60000L);
            string creditsName = DataHelper.GetCreditsName();

            if (DuelRequests.ContainsKey(thisUserName) == false)
            {
                QueueMessage("You are not in a duel or your duel has expired!");
                return;
            }

            DuelData data = DuelRequests[thisUserName];
            DuelRequests.Remove(thisUserName);

            TimeSpan diff = DateTime.UtcNow - data.DuelStartTimeUTC;
            
            //The duel expired
            if (diff.TotalMilliseconds >= duelTimeout)
            {
                QueueMessage("You are not in a duel or your duel has expired!");
                return;
            }
            
            long betAmount = data.BetAmount;
            string dueled = data.UserDueling;

            //If the duel is denied, exit early
            if (argument == DENY_DUEL_ARG)
            {
                QueueMessage($"{thisUserName} has denied to duel with {data.UserDueling} and miss out on a potential {data.BetAmount} {creditsName.Pluralize(false, data.BetAmount)}!");
                return;
            }

            string message = string.Empty;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User thisUser = DataHelper.GetUserNoOpen(thisUserName, context);
                User opponentUser = DataHelper.GetUserNoOpen(data.UserDueling, context);

                if (opponentUser == null)
                {
                    QueueMessage($"Your opponent, {data.UserDueling}, is no longer in the database!");
                    return;
                }

                //First confirm both users have enough credits for the duel, as they could've lost some in that time
                if (thisUser.Stats.Credits < betAmount || opponentUser.Stats.Credits < betAmount)
                {
                    QueueMessage($"At least one user involved in the duel no longer has enough {creditsName.Pluralize(false, 0)} for the duel. The duel is off!");
                    return;
                }

                //Check if either user is now opted out
                if (thisUser.IsOptedOut == true || opponentUser.IsOptedOut == true)
                {
                    QueueMessage("At least one user involved in the duel is now opted out of bot stats. The duel is off!");
                    return;
                }

                //50/50 chance of either user winning
                int val = Rand.Next(0, 2);

                //This user won
                if (val == 0)
                {
                    thisUser.Stats.Credits += betAmount;
                    opponentUser.Stats.Credits -= betAmount;

                    message = $"{thisUser.Name} won the bet against {dueled} for {betAmount} {creditsName.Pluralize(false, betAmount)}!";
                }
                //The opponent won
                else
                {
                    thisUser.Stats.Credits -= betAmount;
                    opponentUser.Stats.Credits += betAmount;

                    message = $"{dueled} won the bet against {thisUser.Name} for {betAmount} {creditsName.Pluralize(false, betAmount)}!";
                }

                //Save the outcome of the duel
                context.SaveChanges();
            }
            
            QueueMessage(message);
        }

        /// <summary>
        /// Represents data about participants in a duel.
        /// </summary>
        public struct DuelData
        {
            public string UserDueling;
            public long BetAmount;
            public DateTime DuelStartTimeUTC;

            public DuelData(string userDueling, long betAmount, DateTime duelStartTimeUTC)
            {
                UserDueling = userDueling;
                BetAmount = betAmount;
                DuelStartTimeUTC = duelStartTimeUTC;
            }

            public override bool Equals(object obj)
            {
                if (obj is DuelData dd)
                {
                    return (this == dd);
                }

                return false;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 23;
                    hash = (hash * 37) + ((UserDueling == null) ? 0 : UserDueling.GetHashCode());
                    hash = (hash * 37) + BetAmount.GetHashCode();
                    hash = (hash * 37) + DuelStartTimeUTC.GetHashCode();
                    return hash;
                } 
            }

            public static bool operator==(DuelData a, DuelData b)
            {
                return (a.BetAmount == b.BetAmount && a.DuelStartTimeUTC == b.DuelStartTimeUTC
                        && a.UserDueling == b.UserDueling);
            }

            public static bool operator!=(DuelData a, DuelData b)
            {
                return !(a == b);
            }
        }
    }
}
