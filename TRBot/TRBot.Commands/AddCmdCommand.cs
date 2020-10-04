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
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// A command that adds a command.
    /// </summary>
    public sealed class AddCmdCommand : BaseCommand
    {
        private CommandHandler CmdHandler = null;
        private BotMessageHandler MessageHandler = null;
        private string UsageMessage = $"Usage - \"command name\", \"type w/namespace\", \"argString - optional\", \"level (int)\", \"enabled (bool)\" \"displayInHelp (bool)\"";

        public AddCmdCommand()
        {
            
        }

        public override void Initialize(CommandHandler cmdHandler, DataContainer dataContainer)
        {
            CmdHandler = cmdHandler;
            MessageHandler = dataContainer.MessageHandler;
        }

        public override void CleanUp()
        {
            CmdHandler = null;
            MessageHandler = null;
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            int argCount = arguments.Count;

            //Ignore with too few arguments
            if (argCount < 5)
            {
                MessageHandler.QueueMessage(UsageMessage);
                return;
            }

            int minArgCount = argCount - 3;

            string commandName = arguments[0].ToLowerInvariant();
            string className = arguments[1];
            string valueStr = string.Empty;
            string levelStr = arguments[minArgCount];
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

            if (int.TryParse(levelStr, out int levelNum) == false)
            {
                MessageHandler.QueueMessage("Incorrect level specified.");
                return;
            }

            if (bool.TryParse(enabledStr, out bool cmdEnabled) == false)
            {
                MessageHandler.QueueMessage("Incorrect command enabled state specified.");
                return;
            }

            if (bool.TryParse(displayInHelpStr, out bool displayInHelp) == false)
            {
                MessageHandler.QueueMessage("Incorrect command displayInHelp state specified.");
                return;
            }

            bool added = CmdHandler.AddCommand(commandName, className, valueStr, levelNum, cmdEnabled, displayInHelp);

            if (added == true)
            {
                //Add this command to the database
                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    //If this command already exists, remove it so it can be replaced with the new data
                    CommandData cmdData = context.Commands.FirstOrDefault((cmd) => cmd.name == commandName);
                    if (cmdData != null)
                    {
                        //Remove command
                        context.Commands.Remove(cmdData);
                    }

                    //Add the new one
                    context.Commands.Add(new CommandData(commandName, className, levelNum, cmdEnabled,
                        displayInHelp, valueStr));
                    
                    context.SaveChanges();
                }

                MessageHandler.QueueMessage($"Successfully added command \"{commandName}\"!");
            }
            else
            {
                MessageHandler.QueueMessage($"Failed to add command \"{commandName}\".");
            }
        }
    }
}
