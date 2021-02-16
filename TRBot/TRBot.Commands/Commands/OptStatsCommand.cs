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
using TRBot.Permissions;
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// Has the user opt in or out of any sort of bot stats, such as dueling, credits, and more.
    /// For simplicity, this currently doesn't factor in existing processes, such as ongoing duels or group bets.
    /// </summary>
    public sealed class OptStatsCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"true or false (optional)\"";
        
        public OptStatsCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            //Invalid number of arguments
            if (arguments.Count > 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string name = args.Command.ChatMessage.Username;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetUserNoOpen(name, context);

                if (user == null)
                {
                    return;
                }

                //Display opt status
                if (arguments.Count == 0)
                {
                    string message = "You are opted ";

                    if (user.IsOptedOut == true)
                    {
                        message += "out of ";
                    }
                    else
                    {
                        message += "into ";
                    }

                    message += "bot stats. Enter \"true\" or \"false\" as an argument to change your opt status.";

                    QueueMessage(message);

                    return;
                }
            }

            string optStr = arguments[0];

            if (bool.TryParse(optStr, out bool optStatus) == false)
            {
                QueueMessage("Invalid opt status argument.");
                return;
            }

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetUserNoOpen(name, context);

                //Opt back into stats
                if (optStatus == true)
                {
                    if (user.IsOptedOut == false)
                    {
                        QueueMessage("You are already opted into bot stats!");
                        return;
                    }

                    QueueMessage("Opted back into bot stats!");
                }
                else
                {
                    if (user.IsOptedOut == true)
                    {
                        QueueMessage("You are already opted out of bot stats!");
                        return;
                    }

                    QueueMessage("Opted out of bot stats!");
                }

                //Set status and save
                user.SetOptStatus(optStatus);
                context.SaveChanges();
            }
        }
    }
}
