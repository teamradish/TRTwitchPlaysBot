/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, version 3 of the License.
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

namespace TRBot.Commands
{
    /// <summary>
    /// A command that removes a command.
    /// </summary>
    public sealed class RemoveCmdCommand : BaseCommand
    {
        private string UsageMessage = $"Usage - \"command name\"";

        public RemoveCmdCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            //Ignore with incorrect number of arguments
            if (arguments.Count != 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string commandName = arguments[0].ToLowerInvariant();

            bool removed = CmdHandler.RemoveCommand(commandName);

            if (removed == true)
            {
                //Remove this command from the database
                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    CommandData cmdData = context.Commands.FirstOrDefault((cmd) => cmd.Name == commandName);
                    if (cmdData != null)
                    {
                        context.Commands.Remove(cmdData);

                        context.SaveChanges();
                    }
                    else
                    {
                        QueueMessage($"Error: Command \"{commandName}\" was not removed from the database because it cannot be found.");
                        return;
                    }
                }

                QueueMessage($"Successfully removed command \"{commandName}\"!");
            }
            else
            {
                QueueMessage($"Failed to remove command \"{commandName}\". It likely does not exist.");
            }
        }
    }
}
