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
    /// The Nintendo GameCube.
    /// </summary>
    public sealed class GCConsole : GameConsole
    {
        public GCConsole()
        {
            Name = "GC";

            Initialize();

            UpdateInputRegex();
        }

        private void Initialize()
        {
            ConsoleInputs = new Dictionary<string, InputData>(33)
            {
                { "left", InputData.CreateAxis("left", (int)GlobalAxisVals.AXIS_X, 0, -1) },
                { "right", InputData.CreateAxis("right", (int)GlobalAxisVals.AXIS_X, 0, 1) },
                { "up", InputData.CreateAxis("up", (int)GlobalAxisVals.AXIS_Y, 0, -1) },
                { "down", InputData.CreateAxis("down", (int)GlobalAxisVals.AXIS_Y, 0, 1) },
                { "cleft", InputData.CreateAxis("cleft", (int)GlobalAxisVals.AXIS_RX, 0, -1) },
                { "cright", InputData.CreateAxis("cright", (int)GlobalAxisVals.AXIS_RX, 0, 1) },
                { "cup", InputData.CreateAxis("cup", (int)GlobalAxisVals.AXIS_RY, 0, -1) },
                { "cdown", InputData.CreateAxis("cdown", (int)GlobalAxisVals.AXIS_RY, 0, 1) },
                
                { "l", new InputData("l", (int)GlobalButtonVals.BTN7, (int)GlobalAxisVals.AXIS_RZ, (InputTypes.Button | InputTypes.Axis), 0, 1, 99) },
                { "r", new InputData("r", (int)GlobalButtonVals.BTN8, (int)GlobalAxisVals.AXIS_Z, (InputTypes.Button | InputTypes.Axis), 0, 1, 99) },

                { "a", InputData.CreateButton("a", (int)GlobalButtonVals.BTN5) },
                { "b", InputData.CreateButton("b", (int)GlobalButtonVals.BTN6) },
                { "z", InputData.CreateButton("z", (int)GlobalButtonVals.BTN9) },
                { "start", InputData.CreateButton("start", (int)GlobalButtonVals.BTN10) },
                { "dleft", InputData.CreateButton("dleft", (int)GlobalButtonVals.BTN15) },
                { "dright", InputData.CreateButton("dright", (int)GlobalButtonVals.BTN16) },
                { "dup", InputData.CreateButton("dup", (int)GlobalButtonVals.BTN17) },
                { "ddown", InputData.CreateButton("ddown", ((int)GlobalButtonVals.BTN18)) },
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
                { "x", InputData.CreateButton("x", (int)GlobalButtonVals.BTN31) },
                { "y", InputData.CreateButton("y", (int)GlobalButtonVals.BTN32) },

                { "#", InputData.CreateBlank("#") }
            };

            /*ValidInputs = new List<string>(33)
            {
                "left", "right", "up", "down",
                "dleft", "dright", "dup", "ddown",
                "cleft", "cright", "cup", "cdown",
                "a", "b", "l", "r", "x", "y", "z",
                "start",
                "ss1", "ss2", "ss3", "ss4", "ss5", "ss6",
                "ls1", "ls2", "ls3", "ls4", "ls5", "ls6",
                "#"
            };

            InputAxesMap = new Dictionary<string, InputAxis>(10)
            {
                { "left", new InputAxis((int)GlobalAxisVals.AXIS_X, 0, -1) },
                { "right", new InputAxis((int)GlobalAxisVals.AXIS_X, 0, 1) },
                { "up", new InputAxis((int)GlobalAxisVals.AXIS_Y, 0, -1) },
                { "down", new InputAxis((int)GlobalAxisVals.AXIS_Y, 0, 1) },
                { "cleft", new InputAxis((int)GlobalAxisVals.AXIS_RX, 0, -1) },
                { "cright", new InputAxis((int)GlobalAxisVals.AXIS_RX, 0, 1) },
                { "cup", new InputAxis((int)GlobalAxisVals.AXIS_RY, 0, -1) },
                { "cdown", new InputAxis((int)GlobalAxisVals.AXIS_RY, 0, 1) },
                { "l", new InputAxis((int)GlobalAxisVals.AXIS_RZ, 0, 1, 99) },
                { "r", new InputAxis((int)GlobalAxisVals.AXIS_Z, 0, 1, 99) }
            };

            InputButtonMap = new Dictionary<string, InputButton>(32)
            {
                { "left", new InputButton((int)GlobalButtonVals.BTN1) },
                { "right", new InputButton((int)GlobalButtonVals.BTN2) },
                { "up", new InputButton((int)GlobalButtonVals.BTN3) },
                { "down", new InputButton((int)GlobalButtonVals.BTN4) },
                { "a", new InputButton((int)GlobalButtonVals.BTN5) },
                { "b", new InputButton((int)GlobalButtonVals.BTN6) },
                { "l", new InputButton((int)GlobalButtonVals.BTN7) },
                { "r", new InputButton((int)GlobalButtonVals.BTN8) },
                { "z", new InputButton((int)GlobalButtonVals.BTN9) },
                { "start", new InputButton((int)GlobalButtonVals.BTN10) },
                { "cleft", new InputButton((int)GlobalButtonVals.BTN11) },
                { "cright", new InputButton((int)GlobalButtonVals.BTN12) },
                { "cup", new InputButton((int)GlobalButtonVals.BTN13) },
                { "cdown", new InputButton((int)GlobalButtonVals.BTN14) },
                { "dleft", new InputButton((int)GlobalButtonVals.BTN15) },
                { "dright", new InputButton((int)GlobalButtonVals.BTN16) },
                { "dup", new InputButton((int)GlobalButtonVals.BTN17) },
                { "ddown", new InputButton((int)GlobalButtonVals.BTN18) },
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
                { "y", new InputButton((int)GlobalButtonVals.BTN32) },
            };*/
        }
    }
}
