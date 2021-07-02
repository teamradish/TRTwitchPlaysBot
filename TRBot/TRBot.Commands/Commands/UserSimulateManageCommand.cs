/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, version 3 of the License.
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
using TRBot.Permissions;
using TRBot.Data;
using TRBot.Logging;

namespace TRBot.Commands
{
    /// <summary>
    /// Helps a user manage their simulate data by clearing it or opting in/out.
    /// </summary>
    public sealed class UserSimulateManageCommand : BaseCommand
    {
        private const string CLEAR_ARG = "clear";

        private readonly string UsageMessage = $"Usage: (\"true\", \"false\", or \"{CLEAR_ARG}\") (optional)";
        private readonly string UsageMessage2 = $"Pass \"true\" or \"false\" to change simulate opt status, or \"{CLEAR_ARG}\" to clear your simulate data.";

        public UserSimulateManageCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count > 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string thisUserName = args.Command.ChatMessage.Username;
            
            bool optedInSimulate = false;
            int simulateLength = 0;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User thisUser = DataHelper.GetUserNoOpen(thisUserName, context);
                
                if (thisUser == null)
                {
                    QueueMessage("You're not in the database!");
                    return;
                }

                if (thisUser.IsOptedOut == true)
                {
                    QueueMessage($"You're opted out of bot stats, so you can't manage your simulation data.");
                    return;
                }

                optedInSimulate = thisUser.IsOptedIntoSimulate;
                simulateLength = thisUser.Stats.SimulateHistory.Length;
            }

            //No arguments - display information
            if (arguments.Count == 0)
            {
                QueueMessage($"You're {(optedInSimulate == false ? "not " : string.Empty)}opted into simulate data, and your simulate data is {simulateLength} characters. {UsageMessage2}");
                return;
            }

            string firstArg = arguments[0].ToLowerInvariant();

            bool isOpt = bool.TryParse(firstArg, out bool optStatus);

            //Changing opt status
            if (isOpt == true)
            {
                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    User user = DataHelper.GetUserNoOpen(thisUserName, context);

                    if (user == null)
                    {
                        QueueMessage("You're not in the database!");
                        return;
                    }

                    //Opt back into simulate data
                    if (optStatus == true)
                    {
                        if (user.IsOptedIntoSimulate == true)
                        {
                            QueueMessage("You are already opted into simulate data!");
                            return;
                        }

                        QueueMessage("Opted into simulate data!");
                    }
                    else
                    {
                        if (user.IsOptedIntoSimulate == false)
                        {
                            QueueMessage("You are already opted out of simulate data!");
                            return;
                        }

                        QueueMessage("Opted out of simulate data!");
                    }

                    //Set status and save
                    user.SetOptSimulate(optStatus);
                    context.SaveChanges();
                }

                return;
            }

            //Check for clear
            if (firstArg != CLEAR_ARG)
            {
                QueueMessage(UsageMessage);
                return;
            }

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetUserNoOpen(thisUserName, context);

                if (user == null)
                {
                    QueueMessage("You're not in the database!");
                    return;
                }

                user.Stats.SimulateHistory = string.Empty;
                context.SaveChanges();
            }

            QueueMessage("Cleared all simulate data!");
        }
    }
}
