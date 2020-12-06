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
using TRBot.Consoles;
using TRBot.VirtualControllers;
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// Lists all inputs currently pressed on a virtual controller.
    /// </summary>
    public sealed class ListPressedInputsCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"controller port (int)\"";

        public ListPressedInputsCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            //Console.WriteLine("Executed");
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count > 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            int controllerIndex = 0;

            //Default to the user's controller port
            User user = DataHelper.GetUser(args.Command.ChatMessage.DisplayName);
            if (user != null)
            {
                controllerIndex = (int)user.ControllerPort;
            }

            //Parse the controller port if we have another argument
            if (arguments.Count == 1)
            {
                //Console.WriteLine("Found arg");
                string arg = arguments[0];

                //Couldn't parse
                if (int.TryParse(arg, out int value) == false)
                {
                    QueueMessage(UsageMessage);
                    return;
                }

                //Subtract 1 for consistency (Ex. player 1 is controller index 0)
                controllerIndex = value - 1;
                //Console.WriteLine($"Arg val set to {controllerIndex}");
            }
            else
            {
                //Console.WriteLine("No arg found");
            }

            //Console.WriteLine("CONTROLLER INDEX: " + controllerIndex);

            //Check if the controller port is out of range
            if (controllerIndex < 0 || controllerIndex >= DataContainer.ControllerMngr.ControllerCount)
            {
                QueueMessage($"Controller port is out of the 1 to {DataContainer.ControllerMngr.ControllerCount} range.");
                return;
            }

            //Get the controller
            IVirtualController controller = DataContainer.ControllerMngr.GetController(controllerIndex);

            if (controller.IsAcquired == false)
            {
                QueueMessage($"Controller at index {controllerIndex} is not acquired.");
                return;
            }
            
            StringBuilder stringBuilder = new StringBuilder(500);
            string startString = $"Pressed inputs for controller {controllerIndex + 1}: ";
            
            stringBuilder.Append(startString);

            //Get the console
            long lastConsoleID = DataHelper.GetSettingInt(SettingsConstants.LAST_CONSOLE, 1L);

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                GameConsole lastConsole = context.Consoles.FirstOrDefault(c => c.ID == lastConsoleID);

                if (lastConsole == null)
                {
                    QueueMessage("The current console is invalid!? No data is available.");
                    return;
                }

                //Check which inputs are pressed
                List<InputData> validInputs = lastConsole.InputList;
                for (int i = 0; i < validInputs.Count; i++)
                {
                    string inputName = validInputs[i].Name;
                    ButtonStates btnState = controller.GetInputState(inputName);
                    if (btnState == ButtonStates.Pressed)
                    {
                        stringBuilder.Append(inputName).Append(',').Append(' ');
                    }
                }
            }

            //If the controller doesn't have any pressed inputs, mention it
            if (stringBuilder.Length == startString.Length)
            {
                stringBuilder.Append("None!");
            }
            else
            {
                stringBuilder.Remove(stringBuilder.Length - 2, 2);
            }

            string finalStr = stringBuilder.ToString();

            int maxCharLength = (int)DataHelper.GetSettingInt(SettingsConstants.BOT_MSG_CHAR_LIMIT, 500L);

            QueueMessageSplit(finalStr, maxCharLength, ", ");
        }
    }
}
