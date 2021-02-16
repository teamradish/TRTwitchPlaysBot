/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
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
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using TRBot.Parsing;
using TRBot.Utilities;

namespace TRBot.Consoles
{
    /// <summary>
    /// Base mutable console definition.
    /// Consoles have an identifier and define which inputs are available and how each input is classified.
    /// </summary>
    public class GameConsole
    {
        #region Properties

        /// <summary>
        /// The id of the console.
        /// </summary>
        public int ID { get; set; } 

        /// <summary>
        /// The name of the console.
        /// </summary>
        public string Name { get; set; } = "GameConsole";

        /// <summary>
        /// All the input data for this console.
        /// </summary>
        public Dictionary<string, InputData> ConsoleInputs { get; protected set; } = new Dictionary<string, InputData>(8);

        /// <summary>
        /// The input list for the console.
        /// This is used by the database and should not be assigned or modified manually.
        /// </summary>
        public virtual List<InputData> InputList { get; set; } = null;

        /// <summary>
        /// The invalid input combos for the console.
        /// This is used by the database and should not be assigned or modified manually.
        /// </summary>
        public virtual List<InvalidCombo> InvalidCombos { get; set; } = null;

        /// <summary>
        /// The input regex for the console.
        /// Update this with <see cref="UpdateInputRegex"/> to warm the regex expression for the parser.
        /// Modifying <see cref="ConsoleInputs"/> will also update this value.
        /// </summary>
        public string InputRegex { get; private set; } = string.Empty;

        #endregion

        public GameConsole()
        {

        }

        public GameConsole(string name)
        {
            Name = name;
        }

        public GameConsole(string name, Dictionary<string, InputData> consoleInputs)
        {
            Name = name;

            SetConsoleInputs(consoleInputs);
        }

        public GameConsole(string name, List<InputData> inputList)
        {
            Name = name;

            SetInputsFromList(inputList);
        }

        public GameConsole(string name, List<InputData> inputList, List<InvalidCombo> invalidCombos)
            : this(name, inputList)
        {
            InvalidCombos = new List<InvalidCombo>(invalidCombos);
        }

        #region Modifications

        public void SetConsoleInputs(Dictionary<string, InputData> consoleInputs)
        {
            ConsoleInputs = consoleInputs;

            if (InputList == null)
            {
                InputList = new List<InputData>(ConsoleInputs.Count);
            }
            
            InputList.Clear();
            InputList.AddRange(ConsoleInputs.Values.ToList());

            UpdateInputRegex();
        }

        public void SetInputsFromList(List<InputData> inputList)
        {
            ConsoleInputs.Clear();

            for (int i = 0; i < inputList.Count; i++)
            {
                InputData inputData = inputList[i];
                ConsoleInputs[inputData.Name] = inputData;
            }

            UpdateInputRegex();
        }

        /// <summary>
        /// Adds an input to the console. If the input already exists, it will be updated with the new value.
        /// This updates the input regex if the input did not previously exist.
        /// </summary>
        /// <param name="inputName">The name of the input to add.</param>
        /// <param name="inputData">The data corresponding to the input.</param>
        /// <returns>true if the input was added, otherwise false.</returns>
        public bool AddInput(string inputName, InputData inputData)
        {
            bool existed = ConsoleInputs.ContainsKey(inputName);

            ConsoleInputs[inputName] = inputData;

            if (existed == false)
            {
                InputList.Add(inputData);
                UpdateInputRegex();
            }
            else
            {
                int index = InputList.FindIndex((inpData) => inpData.Name == inputName);
                InputList.RemoveAt(index);
                InputList.Add(inputData);
            }

            return true;
        }

        /// <summary>
        /// Removes an input from the console.
        /// This updates the input regex if removed.
        /// </summary>
        /// <param name="inputName">The name of the input to remove.</param>
        /// <returns>true if the input was removed, otherwise false.</returns>
        public bool RemoveInput(string inputName)
        {
            bool removed = ConsoleInputs.Remove(inputName);

            if (removed == true)
            {
                int index = InputList.FindIndex((inpData) => inpData.Name == inputName);
                InputList.RemoveAt(index);

                UpdateInputRegex();
            }

            return removed;
        }

        #endregion

        /// <summary>
        /// Tells if a given input exists for this console.
        /// </summary>
        /// <param name="inputName">The name of the input.</param>
        /// <returns>true if the input name is a valid input, otherwise false.</returns>
        public bool DoesInputExist(string inputName)
        {
            return ConsoleInputs.ContainsKey(inputName);
        }

        /// <summary>
        /// Tells if a given input exists and is enabled for this console.
        /// </summary>
        /// <param name="inputName">The name of the input.</param>
        /// <returns>true if the input name is a valid input and the input is enabled, otherwise false.</returns>
        public bool IsInputEnabled(string inputName)
        {
            ConsoleInputs.TryGetValue(inputName, out InputData inputData);
            
            return (inputData != null && inputData.Enabled != 0);
        }

        /// <summary>
        /// Tells if a given axis value exists and returns it if so.
        /// </summary>
        /// <param name="axisName">The name of the axis to get the value for.</param>
        /// <param name="inputAxis">The returned InputAxis if found.</param>
        /// <returns>true if an enabled axis with the given name exists, otherwise false.</returns>
        public bool GetAxisValue(string axisName, out InputAxis inputAxis)
        {
            if (ConsoleInputs.TryGetValue(axisName, out InputData inputData) == false
                || inputData.Enabled == 0
                || EnumUtility.HasEnumVal((long)inputData.InputType, (long)InputTypes.Axis) == false)
            {
                inputAxis = default;
                return false;
            }

            inputAxis = new InputAxis(inputData.AxisValue, inputData.MinAxisVal, inputData.MaxAxisVal, inputData.MaxAxisPercent);
            return true;
        }

        /// <summary>
        /// Tells if a given button value exists and returns it if so.
        /// </summary>
        /// <param name="buttonName">The name of the button to get the value for.</param>
        /// <param name="inputButton">The returned InputButton if found.</param>
        /// <returns>true if an enabled button with the given name exists, otherwise false.</returns>
        public bool GetButtonValue(string buttonName, out InputButton inputButton)
        {
            if (ConsoleInputs.TryGetValue(buttonName, out InputData inputData) == false
                || inputData.Enabled == 0
                || EnumUtility.HasEnumVal((long)inputData.InputType, (long)InputTypes.Button) == false)
            {
                inputButton = default;
                return false;
            }

            inputButton = new InputButton((uint)inputData.ButtonValue);
            return true;
        }

        #region Virtual Methods

        /// <summary>
        /// A more efficient version of telling whether an input is an axis.
        /// Returns the axis if found to save a dictionary lookup if one is needed afterwards.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <param name="axis">The InputAxis value that is assigned. If no axis is found, the default value.</param>
        /// <returns>true if the input is an enabled axis, otherwise false.</returns>
        public virtual bool GetAxis(in ParsedInput input, out InputAxis axis)
        {
            ConsoleInputs.TryGetValue(input.Name, out InputData inputData);

            if (inputData == null || inputData.Enabled == 0 || EnumUtility.HasEnumVal((long)inputData.InputType, (long)InputTypes.Axis) == false)
            {
                axis = default;
                return false;
            }

            if (input.Percent <= inputData.MaxAxisPercent)
            {
                axis = new InputAxis(inputData.AxisValue, inputData.MinAxisVal, inputData.MaxAxisVal, inputData.MaxAxisPercent);
                return true;
            }

            axis = default;
            return false;
        }

        /// <summary>
        /// Tells whether an input is an axis or not.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <returns>true if the input is an enabled axis, otherwise false.</returns>
        public virtual bool IsAxis(in ParsedInput input)
        {
            return GetAxis(input, out InputAxis axis);
        }

        /// <summary>
        /// Tells whether the input is a button.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <returns>true if the input is an enabled button, otherwise false.</returns>
        public virtual bool IsButton(in ParsedInput input)
        {
            ConsoleInputs.TryGetValue(input.Name, out InputData inputData);

            //Not a button if null or not enabled
            if (inputData == null || inputData.Enabled == 0)
            {
                return false;
            }

            return (EnumUtility.HasEnumVal((long)inputData.InputType, (long)InputTypes.Button) == true && IsAxis(input) == false);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Tells whether the input is a blank input, an input without any specially defined function.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <returns>true if the input is enabled and is not a button or axes, otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsBlankInput(in ParsedInput input)
        {
            ConsoleInputs.TryGetValue(input.Name, out InputData inputData);

            //Not a blank input if null, disabled, or not Blank
            if (inputData == null || inputData.Enabled == 0 || inputData.InputType != (int)InputTypes.Blank)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Updates the input regex for the console. This excludes disabled inputs.
        /// </summary>
        public void UpdateInputRegex()
        {
            List<string> validInputs = new List<string>(ConsoleInputs.Count);

            foreach (KeyValuePair<string, InputData> kvPair in ConsoleInputs)
            {
                //Don't add if disabled
                if (kvPair.Value.Enabled == 0)
                {
                    continue;
                }

                validInputs.Add(kvPair.Key);
            }

            InputRegex = Parser.BuildInputRegex(Parser.DEFAULT_PARSE_REGEX_START, Parser.DEFAULT_PARSE_REGEX_END, validInputs);
        }

        #endregion
    }
}
