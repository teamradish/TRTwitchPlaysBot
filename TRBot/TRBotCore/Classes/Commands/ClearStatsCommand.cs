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
    public sealed class ClearStatsCommand : BaseCommand
    {
        private const string ConfirmClearStr = "yes";
        private const string ConfirmStopStr = "no";

        private List<string> ConfirmClearedStatsUsers = new List<string>(8);

        public ClearStatsCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            string name = e.Command.ChatMessage.DisplayName;
            string nameToLower = name.ToLower();

            User user = BotProgram.GetUser(nameToLower);
            
            if (ConfirmClearedStatsUsers.Contains(nameToLower) == false)
            {
                BotProgram.QueueMessage($"WARNING {name}: this clears all miscellaneous user stats, such as credits and message/input counts. If you're sure, retype this command with \"{ConfirmClearStr}\" as an argument to clear or \"{ConfirmStopStr}\" to ignore.");
                ConfirmClearedStatsUsers.Add(nameToLower);
                return;
            }
            else
            {
                //Check for an argument
                List<string> args = e.Command.ArgumentsAsList;

                //Only accept the exact argument
                if (args.Count != 1)
                {
                    return;
                }

                string confirmation = args[0];

                //Confirm - clear stats
                if (confirmation == ConfirmClearStr)
                {
                    ClearUserStats(user);

                    BotProgram.SaveBotData();
                    ConfirmClearedStatsUsers.Remove(nameToLower);
                    BotProgram.QueueMessage("Successfully cleared stats!");
                }
                //Ignore
                else if (confirmation == ConfirmStopStr)
                {
                    ConfirmClearedStatsUsers.Remove(nameToLower);
                    BotProgram.QueueMessage("Cancelled clearing stats!");
                }
            }
        }

        /// <summary>
        /// Clears the user's stats.
        /// </summary>
        /// <param name="user">The user whose stats to clear.</param>
        private void ClearUserStats(User user)
        {
            user.Credits = 0;
            user.TotalMessages = 0;
            user.ValidInputs = 0;
        }
    }
}
