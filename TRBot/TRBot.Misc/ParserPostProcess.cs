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
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;
using TRBot.Parsing;
using TRBot.Consoles;
using TRBot.VirtualControllers;

namespace TRBot.Misc
{
    /// <summary>
    /// Handles post-processing inputs.
    /// </summary>
    public static class ParserPostProcess
    {
        /// <summary>
        /// Tells if a <see cref="ParsedInputSequence"/> contains any restricted inputs.
        /// </summary>
        public static InputValidation InputSequenceContainsRestrictedInputs(in ParsedInputSequence inputSequence,
            Dictionary<string, int> restrictedInputNames)
        {
            //No restricted inputs
            if (restrictedInputNames == null || restrictedInputNames.Count == 0)
            {
                return new InputValidation(InputValidationTypes.Valid, string.Empty);
            }

            //Go through all inputs and find any restricted ones
            List<List<ParsedInput>> allParsedInputs = inputSequence.Inputs;
            for (int i = 0; i < allParsedInputs.Count; i++)
            {
                List<ParsedInput> inputs = allParsedInputs[i];

                for (int j = 0; j < inputs.Count; j++)
                {
                    ParsedInput parsedInput = inputs[i];
                    string inputName = parsedInput.name;

                    //Check if this input is restricted
                    if (restrictedInputNames.ContainsKey(inputName) == true)
                    {
                        return new InputValidation(InputValidationTypes.RestrictedInputs, $"No permission to use input \"{inputName}\".");
                    }
                }
            }

            //No restricted inputs
            return new InputValidation(InputValidationTypes.Valid, string.Empty);
        }

        //Think of a faster way to do this; it's rather slow
        //The idea is to look through and see the inputs that would be held at the same time and avoid the given combo
        //One list is for held inputs with another for pressed inputs - the sum of their counts is compared with the invalid combo list's count
        //Released inputs do not count
        public static InputValidation ValidateInputCombos(in ParsedInputSequence inputSequence, List<InvalidCombo> invalidCombo,
            IVirtualControllerManager vControllerMngr, GameConsole gameConsole)
        {
            if (vControllerMngr == null)
            {
                return new InputValidation(InputValidationTypes.Other, "Virtual controller manager is null; cannot validate combos.");
            }

            List<List<ParsedInput>> inputs = inputSequence.Inputs;

            int controllerCount = vControllerMngr.ControllerCount;

            //These dictionaries are for each controller port
            Dictionary<int, List<string>> currentComboDict = new Dictionary<int, List<string>>(controllerCount);
            Dictionary<int, List<string>> subComboDict = new Dictionary<int, List<string>>(controllerCount);
            
            for (int i = 0; i < controllerCount; i++)
            {
                IVirtualController controller = vControllerMngr.GetController(i);
                if (controller.IsAcquired == false)
                {
                    continue;
                }

                //Add already pressed inputs from all controllers
                for (int j = 0; j < invalidCombo.Count; j++)
                {
                    string inputName = invalidCombo[j].Input.Name;

                    //Check if the button exists and is pressed
                    if (gameConsole.GetButtonValue(inputName, out InputButton inputBtn) == true)
                    {
                        if (controller.GetButtonState(inputBtn.ButtonVal) == ButtonStates.Pressed)
                        {
                            if (currentComboDict.ContainsKey(i) == false)
                            {
                                currentComboDict[i] = new List<string>(invalidCombo.Count);
                            }

                            currentComboDict[i].Add(inputName);
                        }
                    }
                    //Check if the axis exists and is pressed in any capacity
                    else if (gameConsole.GetAxisValue(inputName, out InputAxis inputAxis) == true)
                    {
                        if (controller.GetAxisState(inputAxis.AxisVal) != 0)
                        {
                            if (currentComboDict.ContainsKey(i) == false)
                            {
                                currentComboDict[i] = new List<string>(invalidCombo.Count);
                            }

                            currentComboDict[i].Add(inputName);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Warning: \"{inputName}\" is part of an invalid input combo but doesn't exist for {gameConsole.Name}.");
                    }
                }
            }

            //If all these inputs are somehow pressed already, whatever we do now doesn't matter 
            //However, returning false here would prevent any further inputs from working, so
            //give a chance to check other inputs (such as releasing)
            
            for (int i = 0; i < inputs.Count; i++)
            {
                List<ParsedInput> inputList = inputs[i];

                //Clear sublists
                foreach (List<string> subList in subComboDict.Values)
                {
                    subList.Clear();
                }
                
                for (int j = 0; j < inputList.Count; j++)
                {
                    ParsedInput input = inputList[j];

                    //Get controller port and initialize
                    int port = input.controllerPort;

                    //Ensure a currentcombo entry is available for this port
                    if (currentComboDict.ContainsKey(port) == false)
                    {
                        currentComboDict[port] = new List<string>(invalidCombo.Count);
                    }

                    //Ensure a subcombo entry is available for this port
                    if (subComboDict.ContainsKey(port) == false)
                    {
                        subComboDict[port] = new List<string>(invalidCombo.Count);
                    }

                    //Current and sub combo lists
                    List<string> currentCombo = currentComboDict[port];

                    List<string> subCombo = subComboDict[port];

                    //Check if this input is in the invalid combo
                    if (InvalidComboContainsInputName(invalidCombo, input.name) == true)
                    {
                        //If it's not a release input and isn't in the held or current inputs, add it
                        if (input.release == false && subCombo.Contains(input.name) == false
                            && currentCombo.Contains(input.name) == false)
                        {
                            subCombo.Add(input.name);
                            
                            //Check the count after adding
                            if ((subCombo.Count + currentCombo.Count) == invalidCombo.Count)
                            {
                                //Make the message mention which inputs aren't allowed
                                StringBuilder strBuilder = new StringBuilder(128);
                                strBuilder.Append("Inputs (");

                                for (int k = 0; k < invalidCombo.Count; k++)
                                {
                                    strBuilder.Append('"').Append(invalidCombo[k].Input.Name).Append('"');

                                    if (k < (invalidCombo.Count - 1))
                                    {
                                        strBuilder.Append(',').Append(' ');
                                    }
                                }

                                strBuilder.Append(") are not allowed to be pressed at the same time.");

                                return new InputValidation(InputValidationTypes.InvalidCombo, strBuilder.ToString());
                            }
                        }
                        
                        //For holds, use the held combo
                        if (input.hold == true)
                        {
                            if (currentCombo.Contains(input.name) == false)
                            {
                                //Remove from the subcombo to avoid duplicates
                                currentCombo.Add(input.name);
                                subCombo.Remove(input.name);
                                
                                if ((currentCombo.Count + subCombo.Count) == invalidCombo.Count)
                                {
                                    //Make the message mention which inputs aren't allowed
                                    StringBuilder strBuilder = new StringBuilder(128);
                                    strBuilder.Append("Inputs (");

                                    for (int k = 0; k < invalidCombo.Count; k++)
                                    {
                                        strBuilder.Append('"').Append(invalidCombo[k]).Append('"');

                                        if (k < (invalidCombo.Count - 1))
                                        {
                                            strBuilder.Append(',').Append(' ');
                                        }
                                    }

                                    strBuilder.Append(") are not allowed to be pressed at the same time.");

                                    return new InputValidation(InputValidationTypes.InvalidCombo, strBuilder.ToString());
                                }
                            }
                        }
                        //If released, remove from the current combo
                        else if (input.release == true)
                        {
                            currentCombo.Remove(input.name);
                        }
                    }
                }
            }

            return new InputValidation(InputValidationTypes.Valid, string.Empty);
        }

        /// <summary>
        /// Validates the controller ports used and whether the user has permission to perform an input sequence.
        /// </summary>
        /// <param name="userLevel">The level of the user.</param>
        /// <param name="inputs">The inputs to check.</param>
        /// <param name="inputPermissionLevels">The dictionary of inputs.</param>
        /// <returns>An InputValidation object specifying the InputValidationType and a message, if any.</returns>
        public static InputValidation ValidateInputLvlPermsAndPorts(in long userLevel, in ParsedInputSequence inputSequence,
            IVirtualControllerManager vControllerMngr, Dictionary<string, InputData> inputPermissionLevels)
        {
            if (vControllerMngr == null)
            {
                return new InputValidation(InputValidationTypes.Other, "Virtual controller manager is null; cannot validate ports.");
            }

            List<List<ParsedInput>> inputs = inputSequence.Inputs;

            for (int i = 0; i < inputs.Count; i++)
            {
                for (int j = 0; j < inputs[i].Count; j++)
                {
                    ParsedInput input = inputs[i][j];

                    //Check for a valid port
                    if (input.controllerPort >= 0 && input.controllerPort < vControllerMngr.ControllerCount)
                    {
                        //Check if the controller is acquired
                        IVirtualController controller = vControllerMngr.GetController(input.controllerPort);
                        if (controller.IsAcquired == false)
                        {
                            return new InputValidation(InputValidationTypes.InvalidPort, $"ERROR: Joystick number {input.controllerPort + 1} with controller ID of {controller.ControllerID} has not been acquired! Ensure you (the streamer) have a virtual device set up at this ID.");
                        }
                    }
                    //Invalid port
                    else
                    {
                        return new InputValidation(InputValidationTypes.InvalidPort, $"ERROR: Invalid joystick number {input.controllerPort + 1}. # of joysticks: {vControllerMngr.ControllerCount}. Please change yours or your input's controller port to a valid number to perform inputs.");
                    }

                    //Check if the user has permission to enter this input
                    if (inputPermissionLevels.TryGetValue(input.name, out InputData inputData) == true
                        && userLevel < inputData.Level)
                    {
                        return new InputValidation(InputValidationTypes.InsufficientAccess,
                            $"No permission to use input \"{input.name}\", which requires at least level {inputData.Level}.");
                    }
                }
            }

            return new InputValidation(InputValidationTypes.Valid, string.Empty);
        }

        private static bool InvalidComboContainsInputName(List<InvalidCombo> invalidCombo, string inputName)
        {
            for (int i = 0; i < invalidCombo.Count; i++)
            {
                if (invalidCombo[i].Input.Name == inputName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
