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
using TwitchLib.Client.Events;

namespace TRBot
{
    public sealed class GroupBetCommand : BaseCommand
    {
        public const int MinUsersForBet = 3;
        private const int GroupBetMinutes = 2;
        public static readonly Dictionary<string, long> UsersInBet = new Dictionary<string, long>();

        public static bool BetStarted = false;

        public GroupBetCommand()
        {

        }

        public override void ExecuteCommand(OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count != 1)
            {
                BotProgram.QueueMessage("Please specify a bet amount!");
                return;
            }

            string displayName = e.Command.ChatMessage.DisplayName;
            string displayLower = displayName.ToLower();

            //If the user participating in the group bet isn't in the database, add them
            User user = BotProgram.GetOrAddUser(displayLower, true);
            if (user == null)
            {
                return;
            }

            //Check if we can parse the bet amount
            long betAmount = -1L;
            bool success = long.TryParse(args[0], out betAmount);
            if (success == false || betAmount <= 0)
            {
                BotProgram.QueueMessage("Please specify a positive whole number of credits greater than 0!");
                return;
            }

            if (user.Credits < betAmount)
            {
                BotProgram.QueueMessage("You don't have enough credits to bet this much!");
                return;
            }

            string message = string.Empty;

            bool prevStarted = BetStarted;

            //Check if the user isn't in the bet
            if (UsersInBet.ContainsKey(displayLower) == false)
            {
                if (user.OptedOut == true)
                {
                    BotProgram.QueueMessage("You can't participate in the group bet since you opted out of bot stats.");
                    return;
                }
                
                //Add them
                UsersInBet.Add(displayLower, betAmount);
                message = $"{displayName} entered the group bet with {betAmount} credit(s)!";

                int count = UsersInBet.Count;
                if (count < MinUsersForBet)
                {
                    int diff = MinUsersForBet - count;

                    message += $" {diff} more user(s) are required to start the group bet!";
                }
                else
                {
                    //Start bet
                    if (BetStarted == false)
                    {
                        StartGroupBet();
                    }
                }
            }
            //The user is already in the group bet, so adjust their bet
            else
            {
                long prevBet = UsersInBet[displayLower];

                message = $"{displayName} adjusted their group bet from {prevBet} to {betAmount} credit(s)!";

                UsersInBet[displayLower] = betAmount;
            }

            BotProgram.QueueMessage(message);

            if (prevStarted == false && BetStarted == true)
            {
                BotProgram.QueueMessage($"The group bet has enough participants and will start in {GroupBetMinutes} minute(s), so join before then if you want in!");
            }
        }

        public static void StartGroupBet()
        {
            if (BetStarted == true)
            {
                Console.WriteLine("****Can't start group bet since it already started!****");
                return;
            }

            BotProgram.AddRoutine(new GroupBetRoutine(GroupBetMinutes));

            BetStarted = true;
        }

        public static void StopGroupBet()
        {
            if (BetStarted == false)
            {
                Console.WriteLine("****Can't stop group bet since it hasn't started!****");
                return;
            }

            BotProgram.RemoveRoutine(BotProgram.FindRoutine<GroupBetRoutine>());

            BetStarted = false;
        }
    }
}
