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
using System.Collections.Generic;
using System.Text;
using TRBot.Parsing;
using TRBot.VirtualControllers;

namespace TRBot.Consoles
{
    /// <summary>
    /// A desktop computer.
    /// </summary>
    public sealed class PCConsole : GameConsole
    {
        public PCConsole()
        {
            Name = "pc";

            Initialize();
        }

        private void Initialize()
        {
            SetConsoleInputs(new Dictionary<string, InputData>(18)
            {
                { "mleft",       InputData.CreateAxis("mleft", (int)GlobalAxisVals.AXIS_X, 0, -1) },
                { "mright",      InputData.CreateAxis("mright", (int)GlobalAxisVals.AXIS_X, 0, 1) },
                { "mup",         InputData.CreateAxis("mup", (int)GlobalAxisVals.AXIS_Y, 0, -1) },
                { "mdown",       InputData.CreateAxis("mdown", (int)GlobalAxisVals.AXIS_Y, 0, 1) },

                { "lclick",     InputData.CreateButton("lclick", (int)GlobalButtonVals.BTN5) },
                { "mclick",     InputData.CreateButton("mclick", (int)GlobalButtonVals.BTN6) },
                { "rclick",     InputData.CreateButton("rclick", (int)GlobalButtonVals.BTN7) },
                { "return",     InputData.CreateButton("return", (int)GlobalButtonVals.BTN8) },
                { "space",      InputData.CreateButton("space", (int)GlobalButtonVals.BTN9) },
                { "q",          InputData.CreateButton("q", (int)GlobalButtonVals.BTN10) },
                { "w",          InputData.CreateButton("w", (int)GlobalButtonVals.BTN11) },
                { "e",          InputData.CreateButton("e", (int)GlobalButtonVals.BTN12) },
                { "r",          InputData.CreateButton("r", (int)GlobalButtonVals.BTN13) },
                { "a",          InputData.CreateButton("a", (int)GlobalButtonVals.BTN14) },
                { "s",          InputData.CreateButton("s", (int)GlobalButtonVals.BTN15) },
                { "d",          InputData.CreateButton("d", (int)GlobalButtonVals.BTN16) },
                { "p",          InputData.CreateButton("p", (int)GlobalButtonVals.BTN17) },
                { "#",          InputData.CreateBlank("#") },
                { ".",          InputData.CreateBlank(".") }
            });

            InvalidCombos = new List<InvalidCombo>();
        }
    }
}
