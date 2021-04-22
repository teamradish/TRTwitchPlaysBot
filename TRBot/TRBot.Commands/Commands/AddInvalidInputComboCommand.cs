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
    /// A command that adds an input to the invalid input combo for a console.
    /// </summary>
    public sealed class AddInvalidInputComboCommand : BaseCommand
    {
        private string UsageMessage = $"Usage - \"console name (string)\", \"input name (string)\"";

        public AddInvalidInputComboCommand()
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
            string inputName = arguments[1].ToLowerInvariant();

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                GameConsole console = context.Consoles.FirstOrDefault(c => c.Name == consoleStr);

                if (console == null)
                {
                    QueueMessage($"No console named \"{consoleStr}\" found.");
                    return;
                }

                //Check if the input exists
                InputData existingInput = console.InputList.FirstOrDefault((inpData) => inpData.Name == inputName);

                if (existingInput == null)
                {
                    QueueMessage($"No input named \"{inputName}\" exists for the \"{console.Name}\" console!");
                    return;
                }

                //Check if it's already in the invalid input combo
                InvalidCombo existingCombo = console.InvalidCombos.FirstOrDefault((inpCombo) => inpCombo.Input.ID == existingInput.ID);

                if (existingCombo != null)
                {
                    QueueMessage($"Input \"{inputName}\" is already part of the invalid input combo for the \"{console.Name}\" console!");
                    return;
                }

                //Add the input to the invalid input combo for this console and save
                console.InvalidCombos.Add(new InvalidCombo(existingInput));

                context.SaveChanges();
            }

            QueueMessage($"Successfully added \"{inputName}\" to the invalid input combo for the \"{consoleStr}\" console!");
        }
    }
}
