﻿/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, version 3 of the License.
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
    /// The NES, or Famicom.
    /// </summary>
    public sealed class NESConsole : GameConsole
    {
        public NESConsole()
        {
            Name = "nes";

            Initialize();
        }

        private void Initialize()
        {
            SetConsoleInputs(new Dictionary<string, InputData>(22)
            {
                { "left",       InputData.CreateButton("left", (int)GlobalButtonVals.BTN1) },
                { "right",      InputData.CreateButton("right", (int)GlobalButtonVals.BTN2) },
                { "up",         InputData.CreateButton("up", (int)GlobalButtonVals.BTN3) },
                { "down",       InputData.CreateButton("down", (int)GlobalButtonVals.BTN4) },
                { "a",          InputData.CreateButton("a", (int)GlobalButtonVals.BTN9) },
                { "b",          InputData.CreateButton("b", (int)GlobalButtonVals.BTN10) },
                { "select",     InputData.CreateButton("select", (int)GlobalButtonVals.BTN13) },
                { "start",      InputData.CreateButton("start", (int)GlobalButtonVals.BTN14) },
                { "ss1",        InputData.CreateButton("ss1", (int)GlobalButtonVals.BTN21) },
                { "ss2",        InputData.CreateButton("ss2", (int)GlobalButtonVals.BTN22) },
                { "ss3",        InputData.CreateButton("ss3", (int)GlobalButtonVals.BTN23) },
                { "ss4",        InputData.CreateButton("ss4", (int)GlobalButtonVals.BTN24) },
                { "ss5",        InputData.CreateButton("ss5", (int)GlobalButtonVals.BTN25) },
                { "ss6",        InputData.CreateButton("ss6", (int)GlobalButtonVals.BTN26) },
                { "ls1",        InputData.CreateButton("ls1", (int)GlobalButtonVals.BTN27) },
                { "ls2",        InputData.CreateButton("ls2", (int)GlobalButtonVals.BTN28) },
                { "ls3",        InputData.CreateButton("ls3", (int)GlobalButtonVals.BTN29) },
                { "ls4",        InputData.CreateButton("ls4", (int)GlobalButtonVals.BTN30) },
                { "ls5",        InputData.CreateButton("ls5", (int)GlobalButtonVals.BTN31) },
                { "ls6",        InputData.CreateButton("ls6", (int)GlobalButtonVals.BTN32) },
                { "#",          InputData.CreateBlank("#") },
                { ".",          InputData.CreateBlank(".") }
            });

            InvalidCombos = new List<InvalidCombo>();
        }
    }
}
