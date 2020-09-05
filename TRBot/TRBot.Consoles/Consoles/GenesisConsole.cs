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
    /// The Sega Genesis/Mega Drive.
    /// </summary>
    public sealed class GenesisConsole : GameConsole
    {
        public GenesisConsole()
        {
            Identifier = "Genesis";

            Initialize();

            UpdateInputRegex();
        }

        private void Initialize()
        {
            InputAxesMap = new Dictionary<string, InputAxis>();

            InputButtonMap = new Dictionary<string, InputButton>(16)
            {
                { "left", new InputButton((int)GlobalButtonVals.BTN1) },
                { "right", new InputButton((int)GlobalButtonVals.BTN2) },
                { "up", new InputButton((int)GlobalButtonVals.BTN3) },
                { "down", new InputButton((int)GlobalButtonVals.BTN4) },
                { "a", new InputButton((int)GlobalButtonVals.BTN5) },
                { "b", new InputButton((int)GlobalButtonVals.BTN6) },
                { "c", new InputButton((int)GlobalButtonVals.BTN7) },
                { "x", new InputButton((int)GlobalButtonVals.BTN8) },
                { "y", new InputButton((int)GlobalButtonVals.BTN9) },
                { "z", new InputButton((int)GlobalButtonVals.BTN10) },
                { "ss", new InputButton((int)GlobalButtonVals.BTN11) },
                { "ls", new InputButton((int)GlobalButtonVals.BTN12) },
                { "incs", new InputButton((int)GlobalButtonVals.BTN13) },
                { "decs", new InputButton((int)GlobalButtonVals.BTN14) },
                //{ "ss1", (int)GlobalButtonVals.BTN19 },
                //{ "ss2", (int)GlobalButtonVals.BTN20 },
                //{ "ss3", (int)GlobalButtonVals.BTN21 },
                //{ "ss4", (int)GlobalButtonVals.BTN22 },
                //{ "ss5", (int)GlobalButtonVals.BTN23 },
                //{ "ss6", (int)GlobalButtonVals.BTN24 },
                //{ "ls1", (int)GlobalButtonVals.BTN25 },
                //{ "ls2", (int)GlobalButtonVals.BTN26 },
                //{ "ls3", (int)GlobalButtonVals.BTN27 },
                //{ "ls4", (int)GlobalButtonVals.BTN28 },
                //{ "ls5", (int)GlobalButtonVals.BTN29 },
                //{ "ls6", (int)GlobalButtonVals.BTN30 },
                { "start", new InputButton((int)GlobalButtonVals.BTN31) },
                { "mode", new InputButton((int)GlobalButtonVals.BTN32) }
            };

            ValidInputs = new List<string>(17)
            {
                "up", "down", "left", "right", "a", "b", "c", "x", "y", "z", "start", "mode",
                "ss", "ls", "incs", "decs",
                //"ss1", "ss2", "ss3", "ss4", "ss5", "ss6",
                //"ls1", "ls2", "ls3", "ls4", "ls5", "ls6",
                "#"
            };
        }
    }
}
