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
using System.Globalization;
using TRBot.Connection;
using TRBot.Misc;
using TRBot.Utilities;
using TRBot.Consoles;
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// A command that removes an input from a console.
    /// </summary>
    public sealed class RemoveInputCommand : BaseCommand
    {
        private string UsageMessage = $"Usage - \"console name\", \"input name\"";

        public RemoveInputCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            int argCount = arguments.Count;

            //Ignore with not enough arguments
            if (argCount != 2)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string consoleStr = arguments[0].ToLowerInvariant();

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                GameConsole console = context.Consoles.FirstOrDefault(c => c.Name == consoleStr);
                if (console == null)
                {
                    QueueMessage($"No console named \"{consoleStr}\" found.");
                    return;
                }

                string inputName = arguments[1].ToLowerInvariant();
                
                //Check if the input exists
                int index = console.InputList.FindIndex((inpData) => inpData.Name == inputName);
    
                if (index >= 0)
                {
                    console.InputList.RemoveAt(index);
    
                    //Save changes since it's removed
                    context.SaveChanges();
    
                    QueueMessage($"Removed input \"{inputName}\" from console \"{consoleStr}\"!");
                }
                else
                {
                    QueueMessage($"Input \"{inputName}\" does not exist in console \"{consoleStr}\".");
                }
            }
        }
    }
}
