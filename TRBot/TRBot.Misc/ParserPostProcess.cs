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
            //give a chance to check other inputs, such as releasing
            
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
        /// Validates the controller ports used in an input sequence.
        /// </summary>
        /// <param name="userLevel">The level of the user.</param>
        /// <param name="inputSequence">The input sequence to check.</param>
        /// <returns>An InputValidation object specifying the InputValidationType and a message, if any.</returns>
        public static InputValidation ValidateInputPorts(in ParsedInputSequence inputSequence,
            IVirtualControllerManager vControllerMngr)
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
                            return new InputValidation(InputValidationTypes.InvalidPort, $"ERROR: Joystick number {input.controllerPort + 1} with controller ID of {controller.ControllerID} has not been acquired! Ensure you, the streamer, have a virtual controller set up at this ID (double check permissions).");
                        }
                    }
                    //Invalid port
                    else
                    {
                        return new InputValidation(InputValidationTypes.InvalidPort, $"ERROR: Invalid joystick number {input.controllerPort + 1}. # of joysticks: {vControllerMngr.ControllerCount}. Please change yours or your input's controller port to a valid number to perform inputs.");
                    }
                }
            }

            return new InputValidation(InputValidationTypes.Valid, string.Empty);
        }

        /// <summary>
        /// Validates the controller ports used and whether the user has permission to perform an input sequence.
        /// </summary>
        /// <param name="userLevel">The level of the user.</param>
        /// <param name="inputSequence">The input sequence to check.</param>
        /// <param name="inputPermissionLevels">The dictionary of input permissions.</param>
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
                            return new InputValidation(InputValidationTypes.InvalidPort, $"ERROR: Joystick number {input.controllerPort + 1} with controller ID of {controller.ControllerID} has not been acquired! Ensure you, the streamer, have a virtual controller set up at this ID (double check permissions).");
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

        /// <summary>
        /// Inserts artificial blank inputs after input subsequences if blank inputs do not exist or aren't the longest duration in the subsequence.
        /// This is used to add artificial delays after each input.
        /// The given game console must have a blank input defined for this to work.
        /// <para>
        /// This is expected to go over the maximum input duration, as the delays are inserted after the input.
        /// Otherwise, it'll be cumbersome as players will have to account for the delays when writing input sequences.
        /// That said, aim to keep the <paramref name="midInputDelay"> a low value.
        /// </para>
        /// </summary>
        /// <param name="inputSequence">The parsed input sequence.</param>
        /// <param name="defaultPort">The default controller port to use for the new inputs.</param>
        /// <param name="midInputDelay">The duration of the inserted blank inputs, in milliseconds.</param>
        /// <param name="gameConsole">The game console to check for blank inputs.</param>
        /// <returns>Data regarding the mid input delays.</returns>
        public static MidInputDelayData InsertMidInputDelays(in ParsedInputSequence inputSequence,
            in int defaultPort, in int midInputDelay, GameConsole gameConsole)
        {
            //There aren't enough inputs to add delays to
            if (inputSequence.Inputs == null || inputSequence.Inputs.Count < 2)
            {
                return new MidInputDelayData(null, inputSequence.TotalDuration, false, "There aren't enough inputs to insert delays.");
            }

            //The next thing we do is check if the console has any blank inputs at all
            //If it doesn't, we can't add a delay
            InputData firstBlankInput = null;

            foreach (KeyValuePair<string, InputData> consoleInputs in gameConsole.ConsoleInputs)
            {
                InputData inpData = consoleInputs.Value;
                if (inpData != null && inpData.InputType == InputTypes.Blank && inpData.Enabled > 0)
                {
                    firstBlankInput = inpData;
                    break;
                }
            }

            //No blank inputs, so return
            if (firstBlankInput == null)
            {
                return new MidInputDelayData(null, inputSequence.TotalDuration, false, $"Console {gameConsole.Name} does not have any blank inputs available.");
            }

            //Copy the input sequence
            List<List<ParsedInput>> parsedInputs = new List<List<ParsedInput>>(inputSequence.Inputs.Count);
            
            bool lastIndexBlankLongestDur = true;
            int additionalDelay = 0;

            //Insert sequences in the same loop as copying to improve speed
            //Check if there are any waits in between two input subsequences
            //If not, add the delay first then the next subsequence
            for (int i = 0; i < inputSequence.Inputs.Count; i++)
            {
                List<ParsedInput> curList = inputSequence.Inputs[i];
                List<ParsedInput> newListCopy = new List<ParsedInput>(curList.Count);
                int longestDur = 0;
                bool blankHasLongestDur = false;

                for (int j = 0; j < curList.Count; j++)
                {
                    ParsedInput curInput = curList[j];
                    newListCopy.Add(curInput);

                    //Check for the same duration for consistent behavior
                    //If the blank input lasts as long as another input, it should still add a delay
                    if (curInput.duration >= longestDur)
                    {
                        longestDur = curInput.duration;
                        blankHasLongestDur = false;

                        if (gameConsole.IsBlankInput(curInput) == true)
                        {
                            blankHasLongestDur = true;
                        }
                    }
                }

                //Console.WriteLine($"Index {i} | LastBlankLongest: {lastIndexBlankLongestDur} | CurBlankLongest: {blankHasLongestDur}"); 

                //Add a delay input in between
                if (blankHasLongestDur == false && lastIndexBlankLongestDur == false)
                {
                    ParsedInput newDelayInput = new ParsedInput(firstBlankInput.Name, false, false, 100, midInputDelay, Parser.DEFAULT_PARSE_REGEX_MILLISECONDS_INPUT, defaultPort, string.Empty);
                    
                    parsedInputs.Add(new List<ParsedInput>(1) { newDelayInput });

                    additionalDelay += midInputDelay;
                }

                parsedInputs.Add(newListCopy);

                lastIndexBlankLongestDur = blankHasLongestDur;
            }

            return new MidInputDelayData(parsedInputs, inputSequence.TotalDuration + additionalDelay, true, string.Empty);
        }
    }
}
