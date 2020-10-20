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
using TRBot.ParserData;
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
        public static InputValidation ValidateInputCombos(in ParsedInputSequence inputSequence, List<string> invalidCombo,
            IVirtualControllerManager vControllerMngr, GameConsole gameConsole)
        {
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
                    string inputName = invalidCombo[j];

                    //Check if the button exists and is pressed
                    if (gameConsole.GetButtonValue(inputName, out InputButton inputBtn) == false)
                    {
                        Console.WriteLine($"Warning: \"{inputName}\" is part of an invalid input combo but doesn't exist for {gameConsole.Name}.");
                        continue;
                    }
                    else if (controller.GetButtonState(inputBtn.ButtonVal) == ButtonStates.Pressed)
                    {
                        if (currentComboDict.ContainsKey(i) == false)
                        {
                            currentComboDict[i] = new List<string>(invalidCombo.Count);
                        }

                        currentComboDict[i].Add(inputName);
                        continue;
                    }

                    //Check if the axis exists and is pressed in any capacity
                    if (gameConsole.GetAxisValue(inputName, out InputAxis inputAxis) == false)
                    {
                        Console.WriteLine($"Warning: \"{inputName}\" is part of an invalid input combo but doesn't exist for {gameConsole.Name}.");
                        continue;
                    }
                    else if (controller.GetAxisState(inputAxis.AxisVal) != 0)
                    {
                        if (currentComboDict.ContainsKey(i) == false)
                        {
                            currentComboDict[i] = new List<string>(invalidCombo.Count);
                        }
                        
                        currentComboDict[i].Add(inputName);
                        continue;
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
                    if (invalidCombo.Contains(input.name) == true)
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
    }
}
