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
        private Dictionary<uint, string> ButtonToInputCache = new Dictionary<uint, string>(64);
        private InputGlobals.InputConsoles? CachedConsole = null;

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

            //Parse the controller index if we have another argument
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

                //Out of range
                if (value <= 0 || value > InputGlobals.ControllerMngr.ControllerCount)
                {
                    BotProgram.MsgHandler.QueueMessage($"value <= 0 or out of controller count range of {InputGlobals.ControllerMngr.ControllerCount}.");
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

            //Get the controller
            IVirtualController controller = InputGlobals.ControllerMngr.GetController(controllerIndex);

            if (controller.IsAcquired == false)
            {
                BotProgram.MsgHandler.QueueMessage($"Controller at index {controllerIndex} is not acquired.");
                return;
            }

            //Console.WriteLine("Found controller and acquired");

            //Set up cache
            if (CachedConsole == null || CachedConsole.Value != InputGlobals.CurrentConsoleVal)
            {
                CachedConsole = InputGlobals.CurrentConsoleVal;
                SetupBtnToInputCache();
            }
            
            StringBuilder stringBuilder = new StringBuilder(500);
            string startString = $"Pressed inputs for controller {controllerIndex + 1}: ";
            
            stringBuilder.Append(startString);

            foreach (KeyValuePair<uint, string> kvPair in ButtonToInputCache)
            {
                ButtonStates btnState = controller.GetButtonState(kvPair.Key);
                if (btnState == ButtonStates.Pressed)
                {
                    stringBuilder.Append(kvPair.Value).Append(',').Append(' ');
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

        private void SetupBtnToInputCache()
        {
            ButtonToInputCache.Clear();

            //NOTE: Look into improving performance here
            ConsoleBase curConsole = InputGlobals.CurrentConsole;
            List<GlobalButtonVals> vals = new List<GlobalButtonVals>(EnumUtility.GetValues<GlobalButtonVals>.EnumValues);

            foreach (KeyValuePair<string, uint> kvPair in curConsole.ButtonInputMap)
            {
                for (int i = vals.Count - 1; i >= 0; i--)
                {
                    uint val = (uint)vals[i];
                    //Console.WriteLine($"Iterating val {val} with {kvPair.Key}");
                    if (kvPair.Value == val)
                    {
                        ButtonToInputCache[val] = kvPair.Key;
                        vals.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }
}
