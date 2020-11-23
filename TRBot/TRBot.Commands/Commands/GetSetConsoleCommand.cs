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
using TRBot.Connection;
using TRBot.Misc;
using TRBot.Utilities;
using TRBot.Consoles;
using TRBot.Data;
using TRBot.Permissions;

namespace TRBot.Commands
{
    /// <summary>
    /// A command that gets or sets the current game console.
    /// </summary>
    public sealed class GetSetConsoleCommand : BaseCommand
    {
        private string UsageMessage = $"Usage - \"console name\"";

        public GetSetConsoleCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            //For no arguments, show the current and available consoles
            if (arguments.Count == 0)
            {
                StringBuilder strBuilder = new StringBuilder(300);
                strBuilder.Append("The current console is: ");

                int maxCharCount = 500;

                //List the current console and available consoles
                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    Settings lastSetting = context.SettingCollection.FirstOrDefault(set => set.Key == SettingsConstants.LAST_CONSOLE);

                    GameConsole curConsole = context.Consoles.FirstOrDefault(console => console.ID == lastSetting.ValueInt);
                    if (curConsole == null)
                    {
                        strBuilder.Append("Invalid!? Set a different console to fix this. ");
                    }
                    else
                    {
                        strBuilder.Append(curConsole.Name).Append('.').Append(' ');
                    }

                    strBuilder.Append("To set the console, add one as an argument: ");
                    foreach (GameConsole gameConsole in context.Consoles)
                    {
                        strBuilder.Append(gameConsole.Name).Append(", ");
                    }

                    Settings charCount = context.SettingCollection.FirstOrDefault(set => set.Key == SettingsConstants.BOT_MSG_CHAR_LIMIT);
                    maxCharCount = (int)charCount.ValueInt;
                }

                strBuilder.Remove(strBuilder.Length - 2, 2);

                QueueMessageSplit(strBuilder.ToString(), maxCharCount, ", ");

                return;
            }

            //Ignore with greater than 1 argument
            if (arguments.Count > 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string consoleName = string.Empty;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Check if this user has the ability to set the console
                User user = DataHelper.GetUserNoOpen(args.Command.ChatMessage.Username.ToLowerInvariant(), context);

                if (user != null && user.HasEnabledAbility(PermissionConstants.SET_CONSOLE_ABILITY) == false)
                {
                    QueueMessage("You do not have permission to change the console!");
                    return;
                } 

                consoleName = arguments[0].ToLowerInvariant();
                
                GameConsole console = context.Consoles.FirstOrDefault(c => c.Name == consoleName);

                if (console != null)
                {
                    Settings lastSetting = context.SettingCollection.FirstOrDefault(set => set.Key == SettingsConstants.LAST_CONSOLE);
                    lastSetting.ValueInt = console.ID;

                    context.SaveChanges();

                    QueueMessage($"Set the current console to \"{consoleName}\"!");

                    return;
                }
            }

            QueueMessage($"No console named \"{consoleName}\" exists.");
        }
    }
}
