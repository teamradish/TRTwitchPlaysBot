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
    /// A command that adds an input to a console.
    /// </summary>
    public sealed class AddInputCommand : BaseCommand
    {
        /// <summary>
        /// The max name length for new inputs.
        /// </summary>
        public const int MAX_INPUT_NAME_LENGTH = 20;

        private string UsageMessage = $"Usage - \"console name\", \"input name\", \"buttonVal (int)\", \"axisVal (int)\" \"inputType (int)\" \"minAxis (-1 to 1)\" \"maxAxis (-1 to 1)\" \"maxAxis % (0 to 100)\"";

        public AddInputCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            int argCount = arguments.Count;

            //Ignore with not enough arguments
            if (argCount != 8)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string consoleStr = arguments[0].ToLowerInvariant();

            using BotDBContext context = DatabaseManager.OpenContext();

            GameConsole console = context.Consoles.FirstOrDefault(c => c.Name == consoleStr);
            if (console == null)
            {
                QueueMessage($"No console named \"{consoleStr}\" found.");
                return;
            }

            string inputName = arguments[1].ToLowerInvariant();

            //Check for name length
            if (inputName.Length > MAX_INPUT_NAME_LENGTH)
            {
                QueueMessage($"Inputs may have up to a max of {MAX_INPUT_NAME_LENGTH} characters as their name.");
                return;
            }

            string buttonStr = arguments[2].ToLowerInvariant();
            string axisStr = arguments[3].ToLowerInvariant();
            string inputTypeStr = arguments[4].ToLowerInvariant();
            string minAxisStr = arguments[5].ToLowerInvariant();
            string maxAxisStr = arguments[6].ToLowerInvariant();
            string maxAxisPercentStr = arguments[7].ToLowerInvariant();

            if (GetInt(buttonStr, out int buttonVal) == false)
            {
                QueueMessage("Invalid button value.");
                return;
            }

            if (GetInt(axisStr, out int axisVal) == false)
            {
                QueueMessage("Invalid axis value.");
                return;
            }

            if (GetInt(inputTypeStr, out int inputType) == false)
            {
                QueueMessage("Invalid input type.");
                return;
            }

            if (GetInt(minAxisStr, out int minAxisVal) == false || minAxisVal < -1 || minAxisVal > 1)
            {
                QueueMessage("Invalid minimum axis value.");
                return;
            }

            if (GetInt(maxAxisStr, out int maxAxisVal) == false || maxAxisVal < -1 || maxAxisVal > 1)
            {
                QueueMessage("Invalid maximum axis value.");
                return;
            }

            if (GetInt(maxAxisPercentStr, out int maxAxisPercent) == false || maxAxisPercent < 0 || maxAxisPercent > 100)
            {
                QueueMessage("Invalid maximum axis percent.");
                return;
            }

            //Create the input and add it to the console
            InputData inputData = new InputData(inputName, buttonVal, axisVal, (InputTypes)inputType,
                minAxisVal, maxAxisVal, maxAxisPercent, (long)PermissionLevels.User);

            //Check if the input exists
            InputData existingInput = console.InputList.FirstOrDefault((inpData) => inpData.Name == inputName);

            //Update if found
            if (existingInput != null)
            {    
                existingInput.UpdateData(inputData);
            }
            //Otherwise add it
            else
            {
                //Add the input to the console
                console.InputList.Add(inputData);
            }
            
            //Save database changes
            context.SaveChanges();

            string message = string.Empty;
            if (existingInput == null)
            {
                message = $"Added new input \"{inputName}\" to console \"{consoleStr}\"!";
            }
            else
            {
                message = $"Updated input \"{inputName}\" on console \"{consoleStr}\"!";
            }

            QueueMessage(message);
        }

        private bool GetInt(string value, out int num)
        {
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out num);
        }
    }
}
