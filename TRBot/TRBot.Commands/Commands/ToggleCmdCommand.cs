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
using TRBot.Data;
using TRBot.Permissions;

namespace TRBot.Commands
{
    /// <summary>
    /// A command that toggles a command's enabled state.
    /// </summary>
    public sealed class ToggleCmdCommand : BaseCommand
    {
        private string UsageMessage = $"Usage - \"command name\", (\"true\" or \"false\")";

        public ToggleCmdCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            int argCount = arguments.Count;

            //Ignore with the incorrect number of arguments
            if (argCount != 2)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string commandName = arguments[0].ToLowerInvariant();
            string enabledStr = arguments[1];
            
            using BotDBContext context = DatabaseManager.OpenContext();

            //Find the command
            CommandData cmdData = context.Commands.FirstOrDefault(c => c.name == commandName);

            if (cmdData == null)
            {
                QueueMessage($"Cannot find a command named \"{commandName}\".");
                return;
            }

            if (bool.TryParse(enabledStr, out bool cmdEnabled) == false)
            {
                QueueMessage("Incorrect command enabled state specified.");
                return;
            }

            cmdData.enabled = (cmdEnabled == true) ? 1 : 0;

            context.SaveChanges();

            BaseCommand baseCmd = CmdHandler.GetCommand(commandName);
            baseCmd.Enabled = cmdEnabled;

            QueueMessage($"Successfully set the enabled state of command \"{commandName}\" to {cmdEnabled}!");
        }
    }
}
