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
using System.Text;
using TRBot.Parsing;
using TRBot.VirtualControllers;

namespace TRBot.Consoles
{
    /// <summary>
    /// A custom console.
    /// </summary>
    public sealed class CustomConsole : GameConsole
    {
        public CustomConsole()
        {
            Identifier = "Custom";

            Initialize();

            UpdateInputRegex();
        }

        private void Initialize()
        {
            InputAxes = new Dictionary<string, InputAxis>();

            ButtonInputMap = new Dictionary<string, InputButton>(21)
            {
                { "left", new InputButton((int)GlobalButtonVals.BTN3) },        { "l", new InputButton((int)GlobalButtonVals.BTN3) },
                { "right", new InputButton((int)GlobalButtonVals.BTN4) },       { "r", new InputButton((int)GlobalButtonVals.BTN4) },
                { "up", new InputButton((int)GlobalButtonVals.BTN1) },          { "u", new InputButton((int)GlobalButtonVals.BTN1) },
                { "down", new InputButton((int)GlobalButtonVals.BTN2) },        { "d", new InputButton((int)GlobalButtonVals.BTN2) },
                { "grab", new InputButton((int)GlobalButtonVals.BTN5) },        { "g", new InputButton((int)GlobalButtonVals.BTN5) },
                { "select", new InputButton((int)GlobalButtonVals.BTN6) },      { "s", new InputButton((int)GlobalButtonVals.BTN6) },
                { "pause", new InputButton((int)GlobalButtonVals.BTN7) },       { "p", new InputButton((int)GlobalButtonVals.BTN7) }, { "start", new InputButton((int)GlobalButtonVals.BTN7) },
                { "restart", new InputButton((int)GlobalButtonVals.BTN8) },
                { "undo", new InputButton((int)GlobalButtonVals.BTN9) },
                { "back", new InputButton((int)GlobalButtonVals.BTN10) },       { "b", new InputButton((int)GlobalButtonVals.BTN10) },
                { "viewmap", new InputButton((int)GlobalButtonVals.BTN11) },    { "v", new InputButton((int)GlobalButtonVals.BTN11) },
            };

            ValidInputs = new List<string>(22)
            {
                "up", "u", "down", "d", "left", "l", "right", "r", "grab", "g",
                "select", "s", "pause", "p", "start", "restart", "undo", "back", "b", "viewmap", "v", 
                "#"
            };
        }
    }
}
