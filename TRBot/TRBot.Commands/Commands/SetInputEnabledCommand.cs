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
using TRBot.Permissions;

namespace TRBot.Commands
{
    /// <summary>
    /// A command that enables or disables an input on a console.
    /// </summary>
    public sealed class SetInputEnabledCommand : BaseCommand
    {
        private string UsageMessage = $"Usage - \"console name\", \"input name\", \"true or false\"";

        public SetInputEnabledCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            int argCount = arguments.Count;

            //Ignore with not enough arguments
            if (argCount != 3)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string consoleStr = arguments[0].ToLowerInvariant();
            string inputName = arguments[1].ToLowerInvariant();
            long inputLevel = 0;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                GameConsole console = context.Consoles.FirstOrDefault(c => c.Name == consoleStr);
                if (console == null)
                {
                    QueueMessage($"No console named \"{consoleStr}\" found.");
                    return;
                }

                //Check if the input exists
                InputData inputData = console.InputList.FirstOrDefault((inpData) => inpData.Name == inputName);

                if (inputData == null)
                {
                    QueueMessage($"Input \"{inputName}\" does not exist in console \"{consoleStr}\".");
                    return;
                }

                inputLevel = inputData.Level;
            }

            string enabledStr = arguments[2].ToLowerInvariant();

            //Parse the enabled state
            if (bool.TryParse(enabledStr, out bool inputEnabled) == false)
            {
                QueueMessage("Invalid enable state specified.");
                return;
            }

            //Compare this user's level with the input's current level
            User user = DataHelper.GetUser(args.Command.ChatMessage.Username);

            if (user != null)
            {
                long curInputLvl = inputLevel;

                //Your level is less than the current input's level - cannot change state
                if (user.Level < curInputLvl)
                {
                    QueueMessage("Cannot change this input's enabled state because your level is lower than it.");
                    return;
                }
            }

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Grab the console and input again
                GameConsole console = context.Consoles.FirstOrDefault(c => c.Name == consoleStr);

                InputData inputData = console.InputList.FirstOrDefault((inpData) => inpData.Name == inputName);
                inputData.Enabled = (inputEnabled == false) ? 0L : 1L;

                context.SaveChanges();
            }

            QueueMessage($"Set the enabled state of input \"{inputName}\" on \"{consoleStr}\" to {inputEnabled}!");
        }
    }
}
