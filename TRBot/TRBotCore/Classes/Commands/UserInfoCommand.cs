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
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Tells information about a user.
    /// </summary>
    public sealed class UserInfoCommand : BaseCommand
    {
        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count > 1)
            {
                BotProgram.QueueMessage("Usage: \"username\"");
                return;
            }

            string username = string.Empty;

            //If no arguments are specified, use the name of the user who performed the command
            if (args.Count == 0)
            {
                username = e.Command.ChatMessage.Username;
            }
            else
            {
                username = args[0];
            }

            User user = BotProgram.GetUser(username, false);

            if (user == null)
            {
                BotProgram.QueueMessage($"User does not exist in database!");
                return;
            }

            //Print the user's information
            BotProgram.QueueMessage($"User: {user.Name} | Level: {user.Level} | Total Inputs: {user.ValidInputs} | Total Messages: {user.TotalMessages}");
        }
    }
}
