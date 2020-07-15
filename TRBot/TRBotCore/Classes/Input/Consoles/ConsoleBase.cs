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

namespace TRBot
{
    /// <summary>
    /// Base console definition.
    /// </summary>
    public abstract class ConsoleBase
    {
        #region Properties

        /// <summary>
        /// The valid inputs for this console.
        /// </summary>
        public abstract string[] ValidInputs { get; protected set; }

        /// <summary>
        /// The input axes this console supports.
        /// </summary>
        public abstract Dictionary<string, InputAxis> InputAxes { get; protected set; }

        /// <summary>
        /// The button input map for this console.
        /// Each value corresponds to a numbered button on a virtual controller.
        /// </summary>
        public abstract Dictionary<string, InputButton> ButtonInputMap { get; protected set; }

        public string InputRegex { get; private set; } = string.Empty;

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Allows the console to handle additional arguments when its changed.
        /// <para>For instance, an argument can include a specific controller mode for the Wii.</para>
        /// </summary>
        /// <param name="arguments">A list of arguments as strings.</param>
        public virtual void HandleArgsOnConsoleChange(List<string> arguments)
        {

        }

        /// <summary>
        /// A more efficient version of telling whether an input is an axis.
        /// Returns the axis if found to save a dictionary lookup if one is needed afterwards.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <param name="axis">The InputAxis value that is assigned. If no axis is found, the default value.</param>
        /// <returns>true if the input is an axis, otherwise false.</returns>
        public abstract bool GetAxis(in Parser.Input input, out InputAxis axis);

        /// <summary>
        /// Tells whether an input is an axis or not.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <returns>true if the input is an axis, otherwise false.</returns>
        public abstract bool IsAxis(in Parser.Input input);

        /// <summary>
        /// Tells whether the input is a button.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <returns>true if the input is a button, otherwise false.</returns>
        public abstract bool IsButton(in Parser.Input input);

        #endregion

        #region Methods

        /// <summary>
        /// Tells whether the input is a wait input.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <returns>true if the input is a wait character, otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsWait(in Parser.Input input) => (input.name == "#");

        public void Initialize()
        {
            //Set up the regex for the console
            //Add longer inputs first due to how the new parser works
            //This avoids picking up shorter inputs with the same characters first
            IOrderedEnumerable<string> sorted = from str in ValidInputs
                                                orderby str.Length descending
                                                select str;

            StringBuilder sb = new StringBuilder(Parser.ParseRegexStart.Length + Parser.ParseRegexEnd.Length);
            int i = 0;

            sb.Append(Parser.ParseRegexStart);
            foreach (string s in sorted)
            {
                sb.Append(System.Text.RegularExpressions.Regex.Escape(s));
                if (i != (ValidInputs.Length - 1))
                {
                    sb.Append('|');
                }
                i++;
            }
            sb.Append(Parser.ParseRegexEnd);

            InputRegex = sb.ToString();
        }

        #endregion

        public ConsoleBase()
        {
            Initialize();
        }
    }
}
