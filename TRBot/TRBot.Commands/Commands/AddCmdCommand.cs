/* Copyright (C) 2019-2020 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot,software for playing games through text.
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
using TRBot.Data;
using TRBot.Permissions;
using TRBot.Logging;

namespace TRBot.Commands
{
    /// <summary>
    /// A command that adds a command.
    /// </summary>
    public sealed class AddCmdCommand : BaseCommand
    {
        private string UsageMessage = $"Usage - \"command name\", \"type w/namespace\", \"argString - optional\", \"level (string/int)\", \"enabled (bool)\" \"displayInHelp (bool)\"";

        public AddCmdCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            int argCount = arguments.Count;

            //Ignore with too few arguments
            if (argCount < 5)
            {
                QueueMessage(UsageMessage);
                return;
            }

            int minArgCount = argCount - 3;

            string commandName = arguments[0].ToLowerInvariant();
            string className = arguments[1];
            string valueStr = string.Empty;
            string levelStr = arguments[minArgCount].ToLowerInvariant();
            string enabledStr = arguments[minArgCount + 1];
            string displayInHelpStr = arguments[minArgCount + 2];

            //Combine all the arguments in between as the value string
            for (int i = 2; i < minArgCount; i++)
            {
                valueStr += arguments[i];
                if (i < (minArgCount - 1))
                {
                    valueStr += " ";
                }
            }

            //Parse the level
            if (PermissionHelpers.TryParsePermissionLevel(levelStr, out PermissionLevels permLevel) == false)
            {
                QueueMessage("Invalid level specified.");
                return;
            }

            long levelNum = (long)permLevel;

            if (bool.TryParse(enabledStr, out bool cmdEnabled) == false)
            {
                QueueMessage("Incorrect command enabled state specified.");
                return;
            }

            if (bool.TryParse(displayInHelpStr, out bool displayInHelp) == false)
            {
                QueueMessage("Incorrect command displayInHelp state specified.");
                return;
            }

            bool added = CmdHandler.AddCommand(commandName, className, valueStr, levelNum, cmdEnabled, displayInHelp);

            if (added == true)
            {
                //Add this command to the database
                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    //If this command already exists, remove it so it can be replaced with the new data
                    CommandData cmdData = context.Commands.FirstOrDefault((cmd) => cmd.Name == commandName);
                    if (cmdData != null)
                    {
                        //Remove command
                        context.Commands.Remove(cmdData);
                    }

                    //Add the new one
                    context.Commands.Add(new CommandData(commandName, className, levelNum,
                    cmdEnabled, displayInHelp, valueStr));
                    
                    context.SaveChanges();
                }

                QueueMessage($"Successfully added command \"{commandName}\"!");
            }
            else
            {
                QueueMessage($"Failed to add command \"{commandName}\".");
            }
        }
    }
}
