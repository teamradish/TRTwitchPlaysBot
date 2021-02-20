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
using System.Globalization;
using TRBot.Connection;
using TRBot.Misc;
using TRBot.Utilities;
using TRBot.Consoles;
using TRBot.Data;
using TRBot.Permissions;

namespace TRBot.Commands
{
    /// <summary>
    /// A command that removes a restricted input from a user.
    /// </summary>
    public sealed class RemoveRestrictedInputCommand : BaseCommand
    {
        private string UsageMessage = $"Usage - \"username\", \"console name\", \"input name\"";

        public RemoveRestrictedInputCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            int argCount = arguments.Count;

            //Ignore with not enough arguments
            if (argCount != 3)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string username = arguments[0];

            User restrictedUser = DataHelper.GetUser(username);

            //Check for the user
            if (restrictedUser == null)
            {
                QueueMessage("A user with this name does not exist in the database!");
                return;
            }

            //Compare this user's level with the user they're trying to restrict
            User thisUser = DataHelper.GetUser(args.Command.ChatMessage.Username);

            if (thisUser == null)
            {
                QueueMessage("Huh? The user calling this doesn't exist in the database!");
                return;
            }

            if (thisUser.Level <= restrictedUser.Level)
            {
                QueueMessage("Cannot remove restricted inputs for users greater than or equal to you in level!");
                return;
            }

            string consoleStr = arguments[1].ToLowerInvariant();
            long consoleID = 0L;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                GameConsole console = context.Consoles.FirstOrDefault(c => c.Name == consoleStr);
                if (console == null)
                {
                    QueueMessage($"No console named \"{consoleStr}\" found.");
                    return;
                }

                consoleID = console.ID;
            }

            string inputName = arguments[2].ToLowerInvariant();
            
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                restrictedUser = DataHelper.GetUserNoOpen(username, context);

                //Check if the restricted input exists for this console
                RestrictedInput restrictedInput = restrictedUser.RestrictedInputs.FirstOrDefault(r => r.inputData.Name == inputName && r.inputData.ConsoleID == consoleID);

                //Not restricted
                if (restrictedInput == null)
                {
                    QueueMessage($"{restrictedUser.Name} already has no restrictions on inputting \"{inputName}\" on the \"{consoleStr}\" console!");
                    return;
                }

                //Remove the restricted input and save
                restrictedUser.RestrictedInputs.Remove(restrictedInput);
                context.SaveChanges();
            }

            QueueMessage($"Lifted the restriction for {restrictedUser.Name} on inputting \"{inputName}\" on the \"{consoleStr}\" console!");
        }
    }
}
