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
    /// A command that adds a restricted input to a user.
    /// </summary>
    public sealed class AddRestrictedInputCommand : BaseCommand
    {
        private const string NULL_EXPIRATION_ARG = "null";
        private string UsageMessage = $"Usage - \"username\", \"console name\", \"input name\", \"expiration from now (Ex. 30 ms/s/m/h/d) - \"null\" for no expiration\"";

        public AddRestrictedInputCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            int argCount = arguments.Count;

            //Ignore with not enough arguments
            if (argCount != 4)
            {
                QueueMessage(UsageMessage);
                return;
            }

            using BotDBContext context = DatabaseManager.OpenContext();

            string username = arguments[0];

            User restrictedUser = DataHelper.GetUserNoOpen(username, context);

            //Check for the user
            if (restrictedUser == null)
            {
                QueueMessage("A user with this name does not exist in the database!");
                return;
            }

            string consoleStr = arguments[1].ToLowerInvariant();

            GameConsole console = context.Consoles.FirstOrDefault(c => c.Name == consoleStr);
            if (console == null)
            {
                QueueMessage($"No console named \"{consoleStr}\" found.");
                return;
            }

            string inputName = arguments[2].ToLowerInvariant();
            
            //Check if the input exists
            InputData inputData = console.InputList.FirstOrDefault((inpData) => inpData.Name == inputName);

            if (inputData == null)
            {
                QueueMessage($"Input \"{inputName}\" does not exist in console \"{consoleStr}\".");
                return;
            }

            //Compare this user's level with the user they're trying to restrict
            User thisUser = DataHelper.GetUserNoOpen(args.Command.ChatMessage.Username.ToLowerInvariant(), context);

            if (thisUser == null)
            {
                QueueMessage("Huh? The user calling this doesn't exist in the database!");
                return;
            }

            if (thisUser.Level <= restrictedUser.Level)
            {
                QueueMessage("Cannot restrict inputs for users greater than or equal to you in level!");
                return;
            }

            DateTime nowUTC = DateTime.UtcNow;

            string expirationArg = arguments[3].ToLowerInvariant();

            DateTime? expiration = null;

            if (expirationArg != NULL_EXPIRATION_ARG)
            {
                if (Helpers.TryParseTimeModifierFromStr(expirationArg, out TimeSpan timeFromNow) == false)
                {
                    QueueMessage("Unable to parse expiration time from now.");
                    return;
                }

                //Set the time to this amount from now
                expiration = nowUTC + timeFromNow;
            }

            //See if a restricted input already exists
            RestrictedInput restInput = restrictedUser.RestrictedInputs.FirstOrDefault(r => r.inputData.Name == inputName && r.inputData.ConsoleID == console.ID);

            //Already restricted - update the expiration
            if (restInput != null)
            {
                restInput.Expiration = expiration;

                QueueMessage($"Updated \"{inputName}\" restriction for the \"{consoleStr}\" console on {restrictedUser.Name}! Expires in {expirationArg}!");
            }
            //Add a new restricted input
            else
            {
                //Add the restricted input
                RestrictedInput newRestrictedInput = new RestrictedInput(restrictedUser.ID, inputData.ID, expiration);
                restrictedUser.RestrictedInputs.Add(newRestrictedInput);

                QueueMessage($"Restricted {restrictedUser.Name} from inputting \"{inputName}\" for the \"{consoleStr}\" console! Expires in {expirationArg}!");
            }

            //Save            
            context.SaveChanges();
        }
    }
}
