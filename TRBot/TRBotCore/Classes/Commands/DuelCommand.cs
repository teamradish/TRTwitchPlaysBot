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
    public sealed class DuelCommand : BaseCommand
    {
        public const int DUEL_MINUTES = 1;
        public static Dictionary<string, DuelData> DuelRequests = new Dictionary<string, DuelData>();

        public DuelCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args == null || args.Count != 2)
            {
                BotProgram.QueueMessage("Please specify a single user to duel followed by the bet amount!");
                return;
            }

            string dueler = e.Command.ChatMessage.DisplayName;
            string duelerToLower = dueler.ToLower();

            string user = args[0];
            string usertoLower = user.ToLower();

            if (duelerToLower == usertoLower)
            {
                BotProgram.QueueMessage("You cannot duel yourself!");
                return;
            }

            //If the user dueling isn't in the database, add them
            User duelerUser = BotProgram.GetOrAddUser(duelerToLower);

            User cmdUser = BotProgram.GetUser(usertoLower);

            if (cmdUser == null)
            {
                BotProgram.QueueMessage($"{user} is not in the database!");
                return;
            }

            if (duelerUser.OptedOut == true)
            {
                BotProgram.QueueMessage("You can't duel if you're opted out of bot stats!");
                return;
            }

            if (cmdUser.OptedOut == true)
            {
                BotProgram.QueueMessage("The one you're attempting to duel is opted out of bot stats!");
                return;
            }

            //Check if the duel expired and replace it with this one if so
            if (DuelRequests.ContainsKey(usertoLower) == true)
            {
                DuelData data = DuelRequests[usertoLower];
                TimeSpan diff = DateTime.Now - data.CurDuelTime;
                if (diff.TotalMinutes >= DUEL_MINUTES)
                {
                    DuelRequests.Remove(usertoLower);
                }
                else
                {
                    BotProgram.QueueMessage($"You're still waiting on a duel response from {user}!");
                    return;
                }
            }

            long betAmount = -1L;
            bool success = long.TryParse(args[1], out betAmount);
            if (success == false || betAmount <= 0)
            {
                BotProgram.QueueMessage("Please specify a positive whole number of credits greater than 0!");
                return;
            }

            if (duelerUser.Credits < betAmount || cmdUser.Credits < betAmount)
            {
                BotProgram.QueueMessage("Either you or the one you're dueling does not have enough credits for this duel!");
                return;
            }

            //Add to the duel requests
            DuelRequests.Add(usertoLower, new DuelData(dueler, betAmount, DateTime.Now));

            BotProgram.QueueMessage($"{dueler} has requested to duel {user} for {betAmount} credit(s)! Type {Globals.CommandIdentifier}accept to duel or {Globals.CommandIdentifier}deny to refuse. The duel expires in {DUEL_MINUTES} minute(s)!");
        }

        public struct DuelData
        {
            public string UserDueling;
            public long BetAmount;
            public DateTime CurDuelTime;

            public DuelData(string userDueling, long betAmount, DateTime curDuelTime)
            {
                UserDueling = userDueling;
                BetAmount = betAmount;
                CurDuelTime = curDuelTime;
            }
        }
    }
}
