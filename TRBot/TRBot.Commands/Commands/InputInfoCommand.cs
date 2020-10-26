﻿/* This file is part of TRBot.
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

namespace TRBot.Commands
{
    /// <summary>
    /// Shows all the inputs for a console or information about a specific input.
    /// </summary>
    public sealed class InputInfoCommand : BaseCommand
    {
        private string UsageMessage = $"Usage - \"console name (optional)\" \"input name (optional)\"";

        public InputInfoCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            using BotDBContext context = DatabaseManager.OpenContext();
            GameConsole console = null;
            bool arg1FoundConsole = false;

            //For no arguments, view all inputs on the current console
            //For one argument, check if it's a console and view all inputs, and if not, view the input data
            //For two arguments, view the input data for the given console
            switch (arguments.Count)
            {
                //First find the console
                case 0:
                    Settings lastSetting = context.SettingCollection.FirstOrDefault(set => set.key == SettingsConstants.LAST_CONSOLE);
                    console = context.Consoles.FirstOrDefault(console => console.id == lastSetting.value_int);
                    break;
                case 1:
                    console = context.Consoles.FirstOrDefault(console => console.Name == arguments[0]);
                    if (console == null)
                    {
                        goto case 0;
                    }

                    //We found the console in the first argument
                    arg1FoundConsole = true;
                    break;
                case 2:
                    goto case 1;
                default:
                    QueueMessage(UsageMessage);
                    return;
            }
            
            //Invalid console
            if (console == null)
            {
                QueueMessage("The current console is invalid!? No inputs or input data are available.");
                return;
            }

            //Check for the input data
            if (arguments.Count == 1 || arguments.Count == 2)
            {
                string inputName = arguments[arguments.Count - 1].ToLowerInvariant();
                InputData inpData = console.InputList.Find(inp => inp.Name == inputName);

                if (inpData != null)
                {
                    QueueMessage(inpData.ToString());
                    return;
                }
                else
                {
                    //For two arguments, end here since we didn't find an input for this console
                    //For one argument, the value was a console so we should continue to print all the inputs for it
                    //If we didn't find the console in the first argument, that means it's an input and it wasn't found
                    if (arguments.Count == 2 || arg1FoundConsole == false)
                    {
                        QueueMessage($"No input named \"{inputName}\" exists for the {console.Name} console.");
                        return;
                    }
                }
            }

            //List all inputs for this console
            StringBuilder strBuilder = new StringBuilder(300);
            strBuilder.Append("Valid inputs for ");
            strBuilder.Append(console.Name).Append(':').Append(' ');
            
            foreach (InputData inputData in console.InputList)
            {
                strBuilder.Append(inputData.Name);

                //Note if the input is disabled
                if (inputData.enabled == 0)
                {
                    strBuilder.Append(" (disabled)");
                }

                strBuilder.Append(", ");
            }
            
            Settings charCount = context.SettingCollection.FirstOrDefault(set => set.key == SettingsConstants.BOT_MSG_CHAR_LIMIT);
            int maxCharCount = (int)charCount.value_int;
            strBuilder.Remove(strBuilder.Length - 2, 2);
            
            QueueMessageSplit(strBuilder.ToString(), maxCharCount, ", ");
        }
    }
}
