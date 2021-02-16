/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
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

namespace TRBot.Commands
{
    /// <summary>
    /// A command that clears a user's stats.
    /// </summary>
    public class ClearUserStatsCommand : BaseCommand
    {
        private const string CONFIRM_CLEAR_STR = "yes";
        private const string CONFIRM_STOP_STR = "no";

        //Track which users are attempting to clear their stats
        //This gives them an additional confirmation
        private List<string> ConfirmClearedStatsUsers = new List<string>(8);

        public ClearUserStatsCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            string creditsName = DataHelper.GetCreditsName();
            string userName = args.Command.ChatMessage.Username.ToLowerInvariant();

            User user = DataHelper.GetOrAddUser(userName, out bool added);
            
            if (user == null)
            {
                QueueMessage("Huh? The user calling this doesn't exist in the database!");
                return;
            }

            if (ConfirmClearedStatsUsers.Contains(userName) == false)
            {
                QueueMessage($"WARNING {userName}: this clears all miscellaneous user stats, such as {creditsName.Pluralize(false, 0)}, message/input counts, and recent inputs. If you're sure, retype this command with \"{CONFIRM_CLEAR_STR}\" as an argument to clear or \"{CONFIRM_STOP_STR}\" to decline.");
                ConfirmClearedStatsUsers.Add(userName);
                return;
            }
        
            //Check for an argument
            List<string> arguments = args.Command.ArgumentsAsList;
            
            //Only accept the exact argument
            if (arguments.Count != 1)
            {
                QueueMessage($"If you're sure you want to clear your stats, retype this command with \"{CONFIRM_CLEAR_STR}\" as an argument to clear or \"{CONFIRM_STOP_STR}\" to decline.");
                return;
            }
            
            string confirmation = arguments[0];
            
            //Confirm - clear stats
            if (confirmation == CONFIRM_CLEAR_STR)
            {
                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    user = DataHelper.GetUserNoOpen(userName, context);

                    //Clear stats and save
                    user.RecentInputs.Clear();

                    user.Stats.Credits = 0L;
                    user.Stats.TotalMessageCount = 0L;
                    user.Stats.ValidInputCount = 0L;

                    context.SaveChanges();
                }

                ConfirmClearedStatsUsers.Remove(userName);

                QueueMessage("Successfully cleared stats!");
            }
            //Ignore
            else if (confirmation == CONFIRM_STOP_STR)
            {
                ConfirmClearedStatsUsers.Remove(userName);
                
                QueueMessage("Cancelled clearing stats!");
            }
        }
    }
}
