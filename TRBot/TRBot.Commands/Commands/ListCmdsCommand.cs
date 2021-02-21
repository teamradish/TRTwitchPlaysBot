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
using System.Text;
using System.Linq;
using TRBot.Connection;
using TRBot.Data;
using TRBot.Permissions;

namespace TRBot.Commands
{
    /// <summary>
    /// Lists all commands.
    /// </summary>
    public sealed class ListCmdsCommand : BaseCommand
    {
        private const string DISABLED_ARG = "disabled";
        private const string ALL_ARG = "all";

        private string UsageMessage = "Usage: no arguments (all enabled commands), \"disabled (optional)\" (only disabled commands), or \"all (optional)\" (enabled & disabled commands)";

        public ListCmdsCommand()
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

            string arg = string.Empty;

            if (arguments.Count > 0)
            {
                arg = arguments[0].ToLowerInvariant();

                //Validate argument
                if (arg != DISABLED_ARG && arg != ALL_ARG)
                {
                    QueueMessage(UsageMessage);
                    return;
                }
            }

            //Get the user so we can show only the commands they have access to
            string userName = args.Command.ChatMessage.Username;
            User infoUser = DataHelper.GetUser(userName);
            StringBuilder stringBuilder = null;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Show the commands that should be displayed based on our argument
                IQueryable<CommandData> commandList = null;

                //All commands
                if (arg == ALL_ARG)
                {
                    commandList = context.Commands.Where(c => c.DisplayInList > 0 && c.Level <= infoUser.Level);
                }
                //Disabled commands only
                else if (arg == DISABLED_ARG)
                {
                    commandList = context.Commands.Where(c => c.Enabled <= 0 && c.DisplayInList > 0 && c.Level <= infoUser.Level);
                }
                //Enabled commands only
                else
                {
                    commandList = context.Commands.Where(c => c.Enabled > 0 && c.DisplayInList > 0 && c.Level <= infoUser.Level);
                }
            
                //Order them alphabetically
                commandList = commandList.OrderBy(c => c.Name);
                int cmdCount = commandList.Count();

                if (cmdCount == 0)
                {
                    QueueMessage("There are no displayable commands that you have access to based on your argument!");
                    return;
                }

                //The capacity is estimated by the number of commands times the average string length of each one
                stringBuilder = new StringBuilder(cmdCount * 12);

                stringBuilder.Append("Hi ").Append(args.Command.ChatMessage.Username).Append(", here's the list of commands: ");

                foreach (CommandData cmd in commandList)
                {
                    stringBuilder.Append(cmd.Name);

                    //Note if the command is disabled
                    if (cmd.Enabled <= 0)
                    {
                        stringBuilder.Append(" (disabled)");
                    }

                    stringBuilder.Append(',').Append(' ');
                }
            }

            stringBuilder.Remove(stringBuilder.Length - 2, 2);

            int msgCharLimit = (int)DataHelper.GetSettingInt(SettingsConstants.BOT_MSG_CHAR_LIMIT, 500L);

            QueueMessageSplit(stringBuilder.ToString(), msgCharLimit, ", ");
        }
    }
}
