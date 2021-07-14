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
    /// A command that adds an input to a console.
    /// </summary>
    public sealed class AddInputCommand : BaseCommand
    {
        /// <summary>
        /// The max name length for new inputs.
        /// </summary>
        public const int MAX_INPUT_NAME_LENGTH = 20;

        private string UsageMessage = $"Usage - \"console name\", \"input name\", \"buttonVal (int)\", \"axisVal (int)\" \"inputType (int: 0 = Blank, 1 = Button, 2 = Axis, 3 = Button+Axis)\" \"minAxis (-1.0 to 1.0)\" \"maxAxis (-1.0 to 1.0)\" \"maxAxis % (0.000 to 100.000)\"";

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

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                GameConsole console = context.Consoles.FirstOrDefault(c => c.Name == consoleStr);

                if (console == null)
                {
                    QueueMessage($"No console named \"{consoleStr}\" found.");
                    return;
                }
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

            if (GetDouble(minAxisStr, out double minAxisVal) == false || minAxisVal < -1d || minAxisVal > 1d)
            {
                QueueMessage("Invalid minimum axis value.");
                return;
            }

            //Ensure that the minimum axis value has at most 1 decimal point
            double minAxisDecimals = minAxisVal * 10d;
            if (((int)minAxisDecimals) != minAxisDecimals)
            {
                QueueMessage("Minimum axis value has more than 1 decimal point.");
                return;
            }

            if (GetDouble(maxAxisStr, out double maxAxisVal) == false || maxAxisVal < -1d || maxAxisVal > 1d)
            {
                QueueMessage("Invalid maximum axis value.");
                return;
            }

            //Ensure that the maximum axis value has at most 1 decimal point
            double maxAxisDecimals = maxAxisVal * 10d;
            if (((int)maxAxisDecimals) != maxAxisDecimals)
            {
                QueueMessage("Maximum axis value has more than 1 decimal point.");
                return;
            }

            if (GetDouble(maxAxisPercentStr, out double maxAxisPercent) == false || maxAxisPercent < 0d || maxAxisPercent > 100d)
            {
                QueueMessage("Invalid maximum axis percent.");
                return;
            }

            //Ensure that the max axis percent has at most 3 decimal points
            double numDecimals = maxAxisPercent * 1000d;
            if (((int)numDecimals) != numDecimals)
            {
                QueueMessage("Maximum axis percent has more than 3 decimal points.");
                return;
            }

            //Create the input and add it to the console
            InputData inputData = new InputData(inputName, buttonVal, axisVal, (InputTypes)inputType,
                minAxisVal, maxAxisVal, maxAxisPercent, (long)PermissionLevels.User);

            InputData existingInput = null;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                GameConsole console = context.Consoles.FirstOrDefault(c => c.Name == consoleStr);

                //Check if the input exists
                existingInput = console.InputList.FirstOrDefault((inpData) => inpData.Name == inputName);

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
            }

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

        private bool GetDouble(string value, out double num)
        {
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out num);
        }
    }
}
