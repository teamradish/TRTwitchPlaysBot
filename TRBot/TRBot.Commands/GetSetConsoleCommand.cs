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
using TRBot.Common;
using TRBot.Utilities;
using TRBot.Consoles;
using TRBot.Data;

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
                    Settings lastSetting = context.SettingCollection.FirstOrDefault(set => set.key == SettingsConstants.LAST_CONSOLE);

                    GameConsole curConsole = context.Consoles.FirstOrDefault(console => console.id == lastSetting.value_int);
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

                    Settings charCount = context.SettingCollection.FirstOrDefault(set => set.key == SettingsConstants.BOT_MSG_CHAR_LIMIT);
                    maxCharCount = (int)charCount.value_int;
                }

                strBuilder.Remove(strBuilder.Length - 2, 2);

                string message = Helpers.SplitStringWithinCharCount(strBuilder.ToString(), maxCharCount, out List<string> textList);

                //If the text fits within the character limit, print it all out at once
                if (textList == null)
                {
                    DataContainer.MessageHandler.QueueMessage(message);
                }
                else
                {
                    //Otherwise, queue up the text in pieces
                    for (int i = 0; i < textList.Count; i++)
                    {
                        DataContainer.MessageHandler.QueueMessage(textList[i]);
                    }
                }

                return;
            }

            //Ignore with greater than 1 argument
            if (arguments.Count > 1)
            {
                DataContainer.MessageHandler.QueueMessage(UsageMessage);
                return;
            }

            string consoleName = arguments[0].ToLowerInvariant();

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                GameConsole console = context.Consoles.FirstOrDefault(c => c.Name == consoleName);

                if (console != null)
                {
                    Settings lastSetting = context.SettingCollection.FirstOrDefault(set => set.key == SettingsConstants.LAST_CONSOLE);
                    lastSetting.value_int = console.id;

                    context.SaveChanges();

                    DataContainer.MessageHandler.QueueMessage($"Set the current console to \"{consoleName}\"!");

                    return;
                }
            }

            DataContainer.MessageHandler.QueueMessage($"No console named \"{consoleName}\" exists.");
        }
    }
}
