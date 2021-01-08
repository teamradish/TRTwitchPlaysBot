/* Copyright (C) 2019-2020 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot,software for playing games through text.
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
    /// The Nintendo 64.
    /// </summary>
    public sealed class N64Console : GameConsole
    {
        public N64Console()
        {
            Name = "n64";

            Initialize();

            UpdateInputRegex();
        }

        private void Initialize()
        {
            SetConsoleInputs(new Dictionary<string, InputData>(23)
            {
                { "left",       InputData.CreateAxis("left", (int)GlobalAxisVals.AXIS_X, 0, -1) },
                { "right",      InputData.CreateAxis("right", (int)GlobalAxisVals.AXIS_X, 0, 1) },
                { "up",         InputData.CreateAxis("up", (int)GlobalAxisVals.AXIS_Y, 0, -1) },
                { "down",       InputData.CreateAxis("down", (int)GlobalAxisVals.AXIS_Y, 0, 1) },

                { "a",          InputData.CreateButton("a", (int)GlobalButtonVals.BTN5) },
                { "b",          InputData.CreateButton("b", (int)GlobalButtonVals.BTN6) },
                { "l",          InputData.CreateButton("l", (int)GlobalButtonVals.BTN7) },
                { "r",          InputData.CreateButton("r", (int)GlobalButtonVals.BTN8) },
                { "z",          InputData.CreateButton("z", (int)GlobalButtonVals.BTN9) },
                { "start",      InputData.CreateButton("start", (int)GlobalButtonVals.BTN10) },
                { "cleft",      InputData.CreateButton("cleft", (int)GlobalButtonVals.BTN11) },
                { "cright",     InputData.CreateButton("cright", (int)GlobalButtonVals.BTN12) },
                { "cup",        InputData.CreateButton("cup", (int)GlobalButtonVals.BTN13) },
                { "cdown",      InputData.CreateButton("cdown", (int)GlobalButtonVals.BTN14) },
                { "dleft",      InputData.CreateButton("dleft", (int)GlobalButtonVals.BTN15) },
                { "dright",     InputData.CreateButton("dright", (int)GlobalButtonVals.BTN16) },
                { "dup",        InputData.CreateButton("dup", (int)GlobalButtonVals.BTN17) },
                { "ddown",      InputData.CreateButton("ddown", (int)GlobalButtonVals.BTN18) },
                { "ss1",        InputData.CreateButton("ss1", (int)GlobalButtonVals.BTN19) },
                { "ss2",        InputData.CreateButton("ss2", (int)GlobalButtonVals.BTN20) },
                { "ss3",        InputData.CreateButton("ss3", (int)GlobalButtonVals.BTN21) },
                { "ss4",        InputData.CreateButton("ss4", (int)GlobalButtonVals.BTN22) },
                { "ss5",        InputData.CreateButton("ss5", (int)GlobalButtonVals.BTN23) },
                { "ss6",        InputData.CreateButton("ss6", (int)GlobalButtonVals.BTN24) },
                { "ls1",        InputData.CreateButton("ls1", (int)GlobalButtonVals.BTN25) },
                { "ls2",        InputData.CreateButton("ls2", (int)GlobalButtonVals.BTN26) },
                { "ls3",        InputData.CreateButton("ls3", (int)GlobalButtonVals.BTN27) },
                { "ls4",        InputData.CreateButton("ls4", (int)GlobalButtonVals.BTN28) },
                { "ls5",        InputData.CreateButton("ls5", (int)GlobalButtonVals.BTN29) },
                { "ls6",        InputData.CreateButton("ls6", (int)GlobalButtonVals.BTN30) },
                { "#",          InputData.CreateBlank("#") }
            });

            InvalidCombos = new List<InvalidCombo>();
        }
    }
}
