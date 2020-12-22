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
    /// The Game Boy Advance handheld.
    /// </summary>
    public sealed class GBAConsole : GameConsole
    {
        public GBAConsole()
        {
            Name = "gba";
            
            Initialize();

            UpdateInputRegex();
        }

        private void Initialize()
        {
            SetConsoleInputs(new Dictionary<string, InputData>(23)
            {
                { "left", InputData.CreateButton("left", (int)GlobalButtonVals.BTN1) },
                { "right", InputData.CreateButton("right", (int)GlobalButtonVals.BTN2) },
                { "up", InputData.CreateButton("up", (int)GlobalButtonVals.BTN3) },
                { "down", InputData.CreateButton("down", (int)GlobalButtonVals.BTN4) },
                { "a", InputData.CreateButton("a", (int)GlobalButtonVals.BTN5) },
                { "b", InputData.CreateButton("b", (int)GlobalButtonVals.BTN6) },
                { "select", InputData.CreateButton("select", (int)GlobalButtonVals.BTN7) },
                { "start", InputData.CreateButton("start", (int)GlobalButtonVals.BTN8) },
                { "ss1", InputData.CreateButton("ss1", (int)GlobalButtonVals.BTN19) },
                { "ss2", InputData.CreateButton("ss2", (int)GlobalButtonVals.BTN20) },
                { "ss3", InputData.CreateButton("ss3", (int)GlobalButtonVals.BTN21) },
                { "ss4", InputData.CreateButton("ss4", (int)GlobalButtonVals.BTN22) },
                { "ss5", InputData.CreateButton("ss5", (int)GlobalButtonVals.BTN23) },
                { "ss6", InputData.CreateButton("ss6", (int)GlobalButtonVals.BTN24) },
                { "ls1", InputData.CreateButton("ls1", (int)GlobalButtonVals.BTN25) },
                { "ls2", InputData.CreateButton("ls2", (int)GlobalButtonVals.BTN26) },
                { "ls3", InputData.CreateButton("ls3", (int)GlobalButtonVals.BTN27) },
                { "ls4", InputData.CreateButton("ls4", (int)GlobalButtonVals.BTN28) },
                { "ls5", InputData.CreateButton("ls5", (int)GlobalButtonVals.BTN29) },
                { "ls6", InputData.CreateButton("ls6", (int)GlobalButtonVals.BTN30) },
                { "l", InputData.CreateButton("l", (int)GlobalButtonVals.BTN31) },
                { "r", InputData.CreateButton("r", (int)GlobalButtonVals.BTN32) },
                { "#", InputData.CreateBlank("#") }
            });

            InvalidCombos = new List<InvalidCombo>(4)
            {
                new InvalidCombo(ConsoleInputs["a"]),
                new InvalidCombo(ConsoleInputs["b"]),
                new InvalidCombo(ConsoleInputs["start"]),
                new InvalidCombo(ConsoleInputs["select"]),
            };
        }
    }
}
