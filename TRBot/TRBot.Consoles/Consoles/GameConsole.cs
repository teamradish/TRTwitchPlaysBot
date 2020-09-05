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
        /// The console's identifier.
        /// </summary>
        public string Identifier { get; protected set; } = "GameConsole";

        /// <summary>
        /// The valid inputs for this console.
        /// </summary>
        public List<string> ValidInputs { get; protected set; } = new List<string>(8);

        /// <summary>
        /// The input axes this console supports.
        /// </summary>
        public Dictionary<string, InputAxis> InputAxes { get; protected set; } = new Dictionary<string, InputAxis>(8);

        /// <summary>
        /// The button input map for this console.
        /// Each value corresponds to a numbered button on a virtual controller.
        /// </summary>
        public Dictionary<string, InputButton> ButtonInputMap { get; protected set; } = new Dictionary<string, InputButton>(32);

        /// <summary>
        /// The input regex for the console.
        /// Update this with <see cref="UpdateInputRegex"/> to warm the regex expression for the parser.
        /// Modifying <see cref="ValidInputs"/> will also update this value.
        /// </summary>
        public string InputRegex { get; private set; } = string.Empty;

        #endregion

        public GameConsole()
        {
            
        }

        public GameConsole(string identifier, List<string> validInputs,
            Dictionary<string, InputAxis> inputAxes, Dictionary<string, InputButton> buttonInputMap)
        {
            Identifier = identifier;
            
            SetValidInputs(validInputs);
            SetInputAxes(inputAxes);
            SetButtonMap(buttonInputMap);

            UpdateInputRegex();
        }

        #region Modifications

        /// <summary>
        /// Sets the valid inputs for the console.
        /// This updates the input regex.
        /// </summary>
        /// <param name="inputName">The valid inputs to set.</param>
        /// <returns>true if the input was removed, otherwise false.</returns>
        public void SetValidInputs(List<string> validInputs)
        {
            ValidInputs = validInputs;

            UpdateInputRegex();
        }

        public void SetInputAxes(Dictionary<string, InputAxis> inputAxes)
        {
            InputAxes = inputAxes;
        }

        public void SetButtonMap(Dictionary<string, InputButton> buttonMap)
        {
            ButtonInputMap = buttonMap;
        }

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
        /// </summary>
        /// <param name="axisName">The name of the axis to add.</param>
        /// <param name="inputAxis">The InputAxis data for this axis.</param>
        /// <returns>true if the axis was added, otherwise false.</returns>
        public bool AddAxis(string axisName, in InputAxis inputAxis)
        {
            InputAxes[axisName] = inputAxis;

            return true;
        }

        /// <summary>
        /// Removes an axis from the console.
        /// </summary>
        /// <param name="axisName">The name of the axis to remove.</param>
        /// <returns>true if the axis was removed, otherwise false.</returns>
        public bool RemoveAxis(string axisName)
        {
            return InputAxes.Remove(axisName);
        }

        /// <summary>
        /// Adds a button to the console.
        /// </summary>
        /// <param name="buttonName">The name of the button to add.</param>
        /// <param name="inputButton">The InputButton data for this axis.</param>
        /// <returns>true if the button was added, otherwise false.</returns>
        public bool AddButton(string buttonName, in InputButton inputButton)
        {
            ButtonInputMap[buttonName] = inputButton;

            return true;
        }

        /// <summary>
        /// Removes a button from the console.
        /// </summary>
        /// <param name="buttonName">The name of the button to remove.</param>
        /// <returns>true if the button was removed, otherwise false.</returns>
        public bool RemoveButton(string buttonName)
        {
            return ButtonInputMap.Remove(buttonName);
        }

        #endregion

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
        public virtual bool GetAxis(in Input input, out InputAxis axis)
        {
            return InputAxes.TryGetValue(input.name, out axis);
        }

        /// <summary>
        /// Tells whether an input is an axis or not.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <returns>true if the input is an axis, otherwise false.</returns>
        public virtual bool IsAxis(in Input input)
        {
            return InputAxes.ContainsKey(input.name);
        }

        /// <summary>
        /// Tells whether the input is a button.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <returns>true if the input is a button, otherwise false.</returns>
        public virtual bool IsButton(in Input input)
        {
            return ButtonInputMap.ContainsKey(input.name) == true && IsAxis(input) == false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Tells whether the input is a blank input, an input without any specially defined function.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <returns>true if the input is not a button or axes, otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsBlankInput(in Input input)
        {
            return IsButton(input) == false && IsAxis(input) == false;
        }

        /// <summary>
        /// Updates the input regex for the console.
        /// </summary>
        public void UpdateInputRegex()
        {
            InputRegex = Parser.BuildInputRegex(Parser.DEFAULT_PARSE_REGEX_START, Parser.DEFAULT_PARSE_REGEX_END, ValidInputs);
        }

        #endregion
    }
}
