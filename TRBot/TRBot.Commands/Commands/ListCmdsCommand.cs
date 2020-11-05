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
        public ListCmdsCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            using BotDBContext context = DatabaseManager.OpenContext();

            //Get the user so we can show only the commands they have access to
            string userName = args.Command.ChatMessage.Username;
            User infoUser = DataHelper.GetUserNoOpen(userName, context);

            //Show all enabled commands that should be displayed, and order them alphabetically
            IQueryable<CommandData> commandList = context.Commands.Where(c => c.Enabled != 0 && c.DisplayInList != 0 && c.Level <= infoUser.Level).OrderBy(c => c.Name);

            if (commandList.Count() == 0)
            {
                QueueMessage("There are no displayable commands that you have access to! This might be a mistake, considering you got here to begin with.");
                return;
            }

            //The capacity is estimated by the number of commands times the average string length of each one
            StringBuilder stringBuilder = new StringBuilder(commandList.Count() * 12);

            stringBuilder.Append("Hi ").Append(args.Command.ChatMessage.Username).Append(", here's the list of commands: ");

            foreach (CommandData cmd in commandList)
            {
                stringBuilder.Append(cmd.Name).Append(',').Append(' ');
            }

            stringBuilder.Remove(stringBuilder.Length - 2, 2);

            int msgCharLimit = (int)DataHelper.GetSettingIntNoOpen(SettingsConstants.BOT_MSG_CHAR_LIMIT, context, 500L);

            QueueMessageSplit(stringBuilder.ToString(), msgCharLimit, ", ");
        }
    }
}
