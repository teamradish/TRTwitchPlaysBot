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
                QueueMessage(UsageMessage);
                return;
            }

            string consoleName = arguments[0].ToLowerInvariant();

            long curConsoleID = DataHelper.GetSettingInt(SettingsConstants.LAST_CONSOLE, 1L);
            long removedConsoleID = 0L;
            bool removed = false;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                GameConsole console = context.Consoles.FirstOrDefault(c => c.Name == consoleName);

                if (console != null)
                {
                    removedConsoleID = console.ID;
                    removed = true;

                    context.Consoles.Remove(console);
                    context.SaveChanges();
                }
            }
                    
            if (removed == true)
            {
                QueueMessage($"Successfully removed console \"{consoleName}\"!");

                //After removing the console, check if that console was the current one
                //If so, set the last console ID to the first console in the list
                if (curConsoleID == removedConsoleID)
                {
                    using (BotDBContext context = DatabaseManager.OpenContext())
                    {
                        GameConsole firstConsole = context.Consoles.FirstOrDefault();
                        if (firstConsole != null)
                        {
                            //Change the setting and save
                            Settings lastConsoleSetting = DataHelper.GetSettingNoOpen(SettingsConstants.LAST_CONSOLE, context);
                            lastConsoleSetting.ValueInt = firstConsole.ID;

                            context.SaveChanges();

                            QueueMessage($"\"{consoleName}\" used to be the current console, so the current console has been set to \"{firstConsole.Name}\"!");
                        }
                        else
                        {
                            QueueMessage($"\"{consoleName}\" used to be the current console, but there are no other consoles available. Please add a new console in order to play.");
                        }
                    }
                }

                return;
            }

            QueueMessage($"No console named \"{consoleName}\" exists.");
        }
    }
}
