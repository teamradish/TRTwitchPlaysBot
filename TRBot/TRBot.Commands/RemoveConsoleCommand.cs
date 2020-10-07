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
    /// A command that removes a game console.
    /// </summary>
    public sealed class RemoveConsoleCommand : BaseCommand
    {
        private string UsageMessage = $"Usage - \"console name\"";

        public RemoveConsoleCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            //Ignore with incorrect number of arguments
            if (arguments.Count != 1)
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
                    context.Consoles.Remove(console);

                    DataContainer.MessageHandler.QueueMessage($"Successfully removed console \"{consoleName}\"!");

                    Settings lastConsoleSetting = context.SettingCollection.FirstOrDefault(set => set.key == SettingsConstants.LAST_CONSOLE);
                    
                    //After removing the console, set the last console ID to the first console in the list
                    if (lastConsoleSetting.value_int == console.id)
                    {
                        GameConsole firstConsole = context.Consoles.FirstOrDefault();
                        if (firstConsole != null)
                        {
                            lastConsoleSetting.value_int = firstConsole.id;
                            DataContainer.MessageHandler.QueueMessage($"\"{consoleName}\" used to be the current console, so the current console has been set to \"{firstConsole.Name}\"!");
                        }
                        else
                        {
                            DataContainer.MessageHandler.QueueMessage($"\"{consoleName}\" used to be the current console, but there are no other consoles available. Please add a new console in order to play.");
                        }
                    }

                    context.SaveChanges();

                    return;
                }
            }

            DataContainer.MessageHandler.QueueMessage($"No console named \"{consoleName}\" exists.");
        }
    }
}
