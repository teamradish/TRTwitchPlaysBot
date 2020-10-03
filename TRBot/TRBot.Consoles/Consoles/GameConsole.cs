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
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using TRBot.Parsing;
using TRBot.Utilities;
using Newtonsoft.Json;

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
        /// The name of the console.
        /// </summary>
        public string Name { get; protected set; } = "GameConsole";

        /// <summary>
        /// All the input data for this console.
        /// </summary>
        public Dictionary<string, InputData> ConsoleInputs { get; protected set; } = new Dictionary<string, InputData>(8);

        // <summary>
        // The valid inputs for this console.
        // </summary>
        //public List<string> ValidInputs { get; protected set; } = new List<string>(8);

        // <summary>
        // The input axes map for this console.
        // Each value corresponds to a numbered axis on a virtual controller.
        // </summary>
        //public Dictionary<string, InputAxis> InputAxesMap { get; protected set; } = new Dictionary<string, InputAxis>(8);

        // <summary>
        // The button input map for this console.
        // Each value corresponds to a numbered button on a virtual controller.
        // </summary>
        //public Dictionary<string, InputButton> InputButtonMap { get; protected set; } = new Dictionary<string, InputButton>(32);

        /// <summary>
        /// The input regex for the console.
        /// Update this with <see cref="UpdateInputRegex"/> to warm the regex expression for the parser.
        /// Modifying <see cref="ConsoleInputs"/> will also update this value.
        /// </summary>
        [JsonIgnore]
        public string InputRegex { get; private set; } = string.Empty;

        #endregion

        public GameConsole()
        {

        }

        public GameConsole(string name, Dictionary<string, InputData> consoleInputs)
        {
            Name = name;

            SetConsoleInputs(consoleInputs);

            UpdateInputRegex();
        }

        //public GameConsole(string identifier, List<string> validInputs,
        //    Dictionary<string, InputAxis> inputAxes, Dictionary<string, InputButton> buttonInputMap)
        //{
        //    Name = identifier;
        //    
        //    SetValidInputs(validInputs);
        //    SetInputAxes(inputAxes);
        //    SetButtonMap(buttonInputMap);
        //
        //    UpdateInputRegex();
        //}

        #region Modifications

        public void SetConsoleInputs(Dictionary<string, InputData> consoleInputs)
        {
            ConsoleInputs = consoleInputs;

            UpdateInputRegex();
        }

        // <summary>
        // Sets the valid inputs for the console.
        // This updates the input regex.
        // </summary>
        // <param name="inputName">The valid inputs to set.</param>
        // <returns>true if the input was removed, otherwise false.</returns>
        /*public void SetValidInputs(List<string> validInputs)
        {
            ValidInputs = validInputs;

            UpdateInputRegex();
        }

        public void SetInputAxes(Dictionary<string, InputAxis> axesMap)
        {
            InputAxesMap = axesMap;
        }

        public void SetButtonMap(Dictionary<string, InputButton> buttonMap)
        {
            InputButtonMap = buttonMap;
        }*/

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
                UpdateInputRegex();
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
                UpdateInputRegex();
            }

            return removed;
        }

        /*
        /// <summary>
        /// Adds an input to the console. Duplicates are ignored.
        /// This updates the input regex if added.
        /// </summary>
        /// <param name="inputName">The name of the input to add.</param>
        /// <returns>true if the input was added, otherwise false.</returns>
        public bool AddValidInput(string inputName)
        {
            //Don't add if already in to avoid duplication
            if (ValidInputs.Contains(inputName) == true)
            {
                return false;
            }

            ValidInputs.Add(inputName);

            UpdateInputRegex();

            return true;
        }

        /// <summary>
        /// Removes an input from the console.
        /// This updates the input regex if removed.
        /// </summary>
        /// <param name="inputName">The name of the input to remove.</param>
        /// <returns>true if the input was removed, otherwise false.</returns>
        public bool RemoveValidInput(string inputName)
        {
            bool removed = ValidInputs.Remove(inputName);

            if (removed == true)
            {
                UpdateInputRegex();
            }

            return removed;
        }

        /// <summary>
        /// Adds an axis to the console.
        /// If the axis already exists, its value will be replaced.
        /// </summary>
        /// <param name="axisName">The name of the axis to add.</param>
        /// <param name="inputAxis">The InputAxis data for this axis.</param>
        /// <returns>true if the axis was added, otherwise false.</returns>
        public bool AddAxis(string axisName, in InputAxis inputAxis)
        {
            InputAxesMap[axisName] = inputAxis;

            return true;
        }

        /// <summary>
        /// Removes an axis from the console.
        /// </summary>
        /// <param name="axisName">The name of the axis to remove.</param>
        /// <returns>true if the axis was removed, otherwise false.</returns>
        public bool RemoveAxis(string axisName)
        {
            return InputAxesMap.Remove(axisName);
        }

        /// <summary>
        /// Adds a button to the console.
        /// If the button already exists, its value will be replaced.
        /// </summary>
        /// <param name="buttonName">The name of the button to add.</param>
        /// <param name="inputButton">The InputButton data for this axis.</param>
        /// <returns>true if the button was added, otherwise false.</returns>
        public bool AddButton(string buttonName, in InputButton inputButton)
        {
            InputButtonMap[buttonName] = inputButton;

            return true;
        }

        /// <summary>
        /// Removes a button from the console.
        /// </summary>
        /// <param name="buttonName">The name of the button to remove.</param>
        /// <returns>true if the button was removed, otherwise false.</returns>
        public bool RemoveButton(string buttonName)
        {
            return InputButtonMap.Remove(buttonName);
        }*/

        #endregion

        /// <summary>
        /// Tells if a given input exists for this console.
        /// </summary>
        /// <param name="inputName">The name of the input.</param>
        /// <returns>true if the input name is a valid input, otherwise false.</returns>
        public bool DoesInputExist(string inputName)
        {
            return ConsoleInputs.ContainsKey(inputName);
            //return ValidInputs.Contains(inputName);
        }

        /// <summary>
        /// Tells if a given axis value exists and returns it if so.
        /// </summary>
        /// <param name="axisName">The name of the axis to get the value for.</param>
        /// <param name="inputAxis">The returned InputAxis if found.</param>
        /// <returns>true if an axis with the given name exists, otherwise false.</returns>
        public bool GetAxisValue(string axisName, out InputAxis inputAxis)
        {
            if (ConsoleInputs.TryGetValue(axisName, out InputData inputData) == false
                || EnumUtility.HasEnumVal(inputData.InputType, (long)InputTypes.Axis) == false)
            {
                inputAxis = default;
                return false;
            }

            inputAxis = new InputAxis(inputData.AxisValue, inputData.MinAxisVal, inputData.MaxAxisVal, inputData.MaxAxisPercent);
            return true;
            //return InputAxesMap.TryGetValue(axisName, out inputAxis);
        }

        /// <summary>
        /// Tells if a given button value exists and returns it if so.
        /// </summary>
        /// <param name="buttonName">The name of the button to get the value for.</param>
        /// <param name="inputButton">The returned InputButton if found.</param>
        /// <returns>true if a button with the given name exists, otherwise false.</returns>
        public bool GetButtonValue(string buttonName, out InputButton inputButton)
        {
            if (ConsoleInputs.TryGetValue(buttonName, out InputData inputData) == false
                || EnumUtility.HasEnumVal(inputData.InputType, (long)InputTypes.Button) == false)
            {
                inputButton = default;
                return false;
            }

            inputButton = new InputButton((uint)inputData.ButtonValue);
            return true;
            //return InputButtonMap.TryGetValue(buttonName, out inputButton);
        }

        #region Virtual Methods

        // <summary>
        // Allows the console to handle additional arguments when its changed.
        // <para>For instance, an argument can include a specific controller mode for the Wii.</para>
        // </summary>
        // <param name="arguments">A list of arguments as strings.</param>
        //public virtual void HandleArgsOnConsoleChange(List<string> arguments)
        //{
        //
        //}

        /// <summary>
        /// A more efficient version of telling whether an input is an axis.
        /// Returns the axis if found to save a dictionary lookup if one is needed afterwards.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <param name="axis">The InputAxis value that is assigned. If no axis is found, the default value.</param>
        /// <returns>true if the input is an axis, otherwise false.</returns>
        public virtual bool GetAxis(in ParsedInput input, out InputAxis axis)
        {
            ConsoleInputs.TryGetValue(input.name, out InputData inputData);

            if (inputData == null || EnumUtility.HasEnumVal(inputData.InputType, (long)InputTypes.Axis) == false)
            {
                axis = default;
                return false;
            }

            if (input.percent <= inputData.MaxAxisPercent)
            {
                axis = new InputAxis(inputData.AxisValue, inputData.MinAxisVal, inputData.MaxAxisVal, inputData.MaxAxisPercent);
                return true;
            }

            axis = default;
            return false;
            //bool found = InputAxesMap.TryGetValue(input.name, out axis);
            //if (found == false)
            //{
            //    return false;
            //}

            ////Check if the percent pressed is less or equal to the max percentage allowed by the axis
            //if (input.percent <= axis.MaxPercentPressed)
            //{
            //    return true;
            //}

            //axis = default;
            //return false;
        }

        /// <summary>
        /// Tells whether an input is an axis or not.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <returns>true if the input is an axis, otherwise false.</returns>
        public virtual bool IsAxis(in ParsedInput input)
        {
            return GetAxis(input, out InputAxis axis);
        }

        /// <summary>
        /// Tells whether the input is a button.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <returns>true if the input is a button, otherwise false.</returns>
        public virtual bool IsButton(in ParsedInput input)
        {
            ConsoleInputs.TryGetValue(input.name, out InputData inputData);

            if (inputData == null)
            {
                return false;
            }

            return (EnumUtility.HasEnumVal(inputData.InputType, (long)InputTypes.Button) == true && IsAxis(input) == false);

            //return InputButtonMap.ContainsKey(input.name) == true && IsAxis(input) == false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Tells whether the input is a blank input, an input without any specially defined function.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <returns>true if the input is not a button or axes, otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsBlankInput(in ParsedInput input)
        {
            ConsoleInputs.TryGetValue(input.name, out InputData inputData);

            if (inputData == null || inputData.InputType == (int)InputTypes.None)
            {
                return true;
            }

            return false;
            //return IsButton(input) == false && IsAxis(input) == false;
        }

        /// <summary>
        /// Updates the input regex for the console.
        /// </summary>
        public void UpdateInputRegex()
        {
            InputRegex = Parser.BuildInputRegex(Parser.DEFAULT_PARSE_REGEX_START, Parser.DEFAULT_PARSE_REGEX_END, ConsoleInputs.Keys.ToArray());//ValidInputs);
        }

        #endregion
    }
}
