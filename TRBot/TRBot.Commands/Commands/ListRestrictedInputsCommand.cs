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
    /// A command that lists a user's restricted inputs.
    /// </summary>
    public sealed class ListRestrictedInputsCommand : BaseCommand
    {
        private string UsageMessage = $"Usage - \"username (optional)\"";

        public ListRestrictedInputsCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            int argCount = arguments.Count;

            //Ignore with not enough arguments
            if (argCount > 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            //Get this user if an argument wasn't given, otherwise get the user specified
            string username = (argCount > 0) ? arguments[0] : args.Command.ChatMessage.Username;

            StringBuilder strBuilder = null;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                
                User restrictedUser = DataHelper.GetUserNoOpen(username, context);

                //Check for the user
                if (restrictedUser == null)
                {
                    QueueMessage("A user with this name does not exist in the database!");
                    return;
                }

                //Find all unexpired restricted inputs
                IEnumerable<RestrictedInput> restrictedInputs = restrictedUser.RestrictedInputs.Where(r => r.HasExpired == false);

                int restrictedInputCount = restrictedInputs.Count();

                //Check for no restricted inputs
                if (restrictedInputCount == 0)
                {
                    QueueMessage($"{restrictedUser.Name} has no restricted inputs!");
                    return;
                }

                strBuilder = new StringBuilder(restrictedInputCount * 8);
                strBuilder.Append("Restricted inputs for ").Append(restrictedUser.Name).Append(':').Append(' ');

                foreach(RestrictedInput resInp in restrictedInputs)
                {
                    strBuilder.Append(resInp.inputData.Name).Append(" (").Append(resInp.inputData.Console.Name).Append(")");

                    if (resInp.HasExpiration == true)
                    {
                        strBuilder.Append(" (exp: ").Append(resInp.Expiration.Value.ToString()).Append(" UTC)");
                    }

                    strBuilder.Append(',').Append(' ');
                }
            }

            strBuilder.Remove(strBuilder.Length - 2, 2);

            int maxCharCount = (int)DataHelper.GetSettingInt(SettingsConstants.BOT_MSG_CHAR_LIMIT, 500L);

            QueueMessageSplit(strBuilder.ToString(), maxCharCount, ", ");
        }
    }
}
