﻿/* This file is part of TRBot.
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
    /// The SNES, or Super Famicom.
    /// </summary>
    public sealed class SNESConsole : GameConsole
    {
        public SNESConsole()
        {
            Name = "snes";

            Initialize();

            UpdateInputRegex();
        }

        private void Initialize()
        {
            SetConsoleInputs(new Dictionary<string, InputData>(25)
            {
                { "left",       InputData.CreateButton("left", (int)GlobalButtonVals.BTN1) },
                { "right",      InputData.CreateButton("right", (int)GlobalButtonVals.BTN2) },
                { "up",         InputData.CreateButton("up", (int)GlobalButtonVals.BTN3) },
                { "down",       InputData.CreateButton("down", (int)GlobalButtonVals.BTN4) },
                { "a",          InputData.CreateButton("a", (int)GlobalButtonVals.BTN5) },
                { "b",          InputData.CreateButton("b", (int)GlobalButtonVals.BTN6) },
                { "l",          InputData.CreateButton("l", (int)GlobalButtonVals.BTN7) },
                { "r",          InputData.CreateButton("r", (int)GlobalButtonVals.BTN8) },
                { "select",     InputData.CreateButton("select", (int)GlobalButtonVals.BTN9) },
                { "start",      InputData.CreateButton("start", (int)GlobalButtonVals.BTN10) },
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
                { "x",          InputData.CreateButton("x", (int)GlobalButtonVals.BTN31) },
                { "y",          InputData.CreateButton("y", (int)GlobalButtonVals.BTN32) },
                { "#",          InputData.CreateBlank("#") }
            });
            
            /*InputAxesMap = new Dictionary<string, InputAxis>();

            InputButtonMap = new Dictionary<string, InputButton>(24)
            {
                { "left", new InputButton((int)GlobalButtonVals.BTN1) },
                { "right", new InputButton((int)GlobalButtonVals.BTN2) },
                { "up", new InputButton((int)GlobalButtonVals.BTN3) },
                { "down", new InputButton((int)GlobalButtonVals.BTN4) },
                { "a", new InputButton((int)GlobalButtonVals.BTN5) },
                { "b", new InputButton((int)GlobalButtonVals.BTN6) },
                { "l", new InputButton((int)GlobalButtonVals.BTN7) },
                { "r", new InputButton((int)GlobalButtonVals.BTN8) },
                { "select", new InputButton((int)GlobalButtonVals.BTN9) },
                { "start", new InputButton((int)GlobalButtonVals.BTN10) },
                { "ss1", new InputButton((int)GlobalButtonVals.BTN19) },
                { "ss2", new InputButton((int)GlobalButtonVals.BTN20) },
                { "ss3", new InputButton((int)GlobalButtonVals.BTN21) },
                { "ss4", new InputButton((int)GlobalButtonVals.BTN22) },
                { "ss5", new InputButton((int)GlobalButtonVals.BTN23) },
                { "ss6", new InputButton((int)GlobalButtonVals.BTN24) },
                { "ls1", new InputButton((int)GlobalButtonVals.BTN25) },
                { "ls2", new InputButton((int)GlobalButtonVals.BTN26) },
                { "ls3", new InputButton((int)GlobalButtonVals.BTN27) },
                { "ls4", new InputButton((int)GlobalButtonVals.BTN28) },
                { "ls5", new InputButton((int)GlobalButtonVals.BTN29) },
                { "ls6", new InputButton((int)GlobalButtonVals.BTN30) },
                { "x", new InputButton((int)GlobalButtonVals.BTN31) },
                { "y", new InputButton((int)GlobalButtonVals.BTN32) }
            };

            ValidInputs = new List<string>(25)
            {
                "up", "down", "left", "right", "a", "b", "x", "y", "l", "r", "select", "start",
                "ss1", "ss2", "ss3", "ss4", "ss5", "ss6",
                "ls1", "ls2", "ls3", "ls4", "ls5", "ls6",
                "#"
            };*/
        }
    }
}