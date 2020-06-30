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
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Lists all inputs currently pressed on a virtual controller.
    /// </summary>
    public sealed class ListPressedInputsCommand : BaseCommand
    {
        public ListPressedInputsCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            //Console.WriteLine("Executed");
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count > 1)
            {
                BotProgram.MsgHandler.QueueMessage("Usage: \"controller number\"");
                return;
            }

            int controllerIndex = 0;

            //Default to the user's controller port
            User user = BotProgram.GetUser(e.Command.ChatMessage.DisplayName, false);
            if (user != null)
            {
                controllerIndex = user.Team;
            }

            //Parse the controller port if we have another argument
            if (args.Count == 1)
            {
                //Console.WriteLine("Found arg");
                string arg = args[0];

                //Couldn't parse
                if (int.TryParse(arg, out int value) == false)
                {
                    BotProgram.MsgHandler.QueueMessage("Usage: \"controller number\"");
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
            if (controllerIndex < 0 || controllerIndex >= InputGlobals.ControllerMngr.ControllerCount)
            {
                BotProgram.MsgHandler.QueueMessage($"Controller port is out of the 1 to {InputGlobals.ControllerMngr.ControllerCount} range.");
                return;
            }

            //Get the controller
            IVirtualController controller = InputGlobals.ControllerMngr.GetController(controllerIndex);

            if (controller.IsAcquired == false)
            {
                BotProgram.MsgHandler.QueueMessage($"Controller at index {controllerIndex} is not acquired.");
                return;
            }

            //Console.WriteLine("Found controller and acquired");
            
            StringBuilder stringBuilder = new StringBuilder(500);
            string startString = $"Pressed inputs for controller {controllerIndex + 1}: ";
            
            stringBuilder.Append(startString);

            //Check which inputs are pressed
            string[] validInputs = InputGlobals.ValidInputs;
            for (int i = 0; i < validInputs.Length; i++)
            {
                string inputName = validInputs[i];
                ButtonStates btnState = controller.GetInputState(inputName);
                if (btnState == ButtonStates.Pressed)
                {
                    stringBuilder.Append(inputName).Append(',').Append(' ');
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

            BotProgram.MsgHandler.QueueMessage(finalStr);
        }
    }
}
