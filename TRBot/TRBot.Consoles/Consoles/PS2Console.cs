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
    /// The Playstation 2.
    /// </summary>
    public sealed class PS2Console : GameConsole
    {
        public PS2Console()
        {
            Name = "ps2";

            Initialize();

            UpdateInputRegex();
        }

        private void Initialize()
        {
            SetConsoleInputs(new Dictionary<string, InputData>(33)
            {
                { "left",       InputData.CreateAxis("left", (int)GlobalAxisVals.AXIS_X, 0, -1) },
                { "right",      InputData.CreateAxis("right", (int)GlobalAxisVals.AXIS_X, 0, 1) },
                { "up",         InputData.CreateAxis("up", (int)GlobalAxisVals.AXIS_Y, 0, -1) },
                { "down",       InputData.CreateAxis("down", (int)GlobalAxisVals.AXIS_Y, 0, 1) },
                { "rleft",      InputData.CreateAxis("rleft", (int)GlobalAxisVals.AXIS_RX, 0, -1) },
                { "rright",     InputData.CreateAxis("rright", (int)GlobalAxisVals.AXIS_RX, 0, 1) },
                { "rup",        InputData.CreateAxis("rup", (int)GlobalAxisVals.AXIS_RY, 0, -1) },
                { "rdown",      InputData.CreateAxis("rdown", (int)GlobalAxisVals.AXIS_RY, 0, 1) },

                { "square",     InputData.CreateButton("square", (int)GlobalButtonVals.BTN9) },
                { "triangle",   InputData.CreateButton("triangle", (int)GlobalButtonVals.BTN10) },
                { "circle",     InputData.CreateButton("circle", (int)GlobalButtonVals.BTN11) },
                { "cross",      InputData.CreateButton("cross", (int)GlobalButtonVals.BTN12) },
                { "select",     InputData.CreateButton("select", (int)GlobalButtonVals.BTN13) },
                { "start",      InputData.CreateButton("start", (int)GlobalButtonVals.BTN14) },
                { "l1",         InputData.CreateButton("l1", (int)GlobalButtonVals.BTN15) },
                { "r1",         InputData.CreateButton("r1", (int)GlobalButtonVals.BTN16) },
                { "l2",         InputData.CreateButton("l2", (int)GlobalButtonVals.BTN17) },
                { "r2",         InputData.CreateButton("r2", (int)GlobalButtonVals.BTN18) },
                { "l3",         InputData.CreateButton("l3", (int)GlobalButtonVals.BTN19) },
                { "r3",         InputData.CreateButton("r3", (int)GlobalButtonVals.BTN20) },
                { "dup",        InputData.CreateButton("dup", (int)GlobalButtonVals.BTN21) },
                { "ddown",      InputData.CreateButton("ddown", (int)GlobalButtonVals.BTN22) },
                { "dleft",      InputData.CreateButton("dleft", (int)GlobalButtonVals.BTN23) },
                { "dright",     InputData.CreateButton("dright", (int)GlobalButtonVals.BTN24) },
                { "ss1",        InputData.CreateButton("ss1", (int)GlobalButtonVals.BTN25) },
                { "ss2",        InputData.CreateButton("ss2", (int)GlobalButtonVals.BTN26) },
                { "ss3",        InputData.CreateButton("ss3", (int)GlobalButtonVals.BTN27) },
                { "ss4",        InputData.CreateButton("ss4", (int)GlobalButtonVals.BTN28) },
                { "ls1",        InputData.CreateButton("ls1", (int)GlobalButtonVals.BTN29) },
                { "ls2",        InputData.CreateButton("ls2", (int)GlobalButtonVals.BTN30) },
                { "ls3",        InputData.CreateButton("ls3", (int)GlobalButtonVals.BTN31) },
                { "ls4",        InputData.CreateButton("ls4", (int)GlobalButtonVals.BTN32) },
                { "#",          InputData.CreateBlank("#") }
            });

            /*InputButtonMap = new Dictionary<string, InputButton>(32)
            {
                { "left", new InputButton((int)GlobalButtonVals.BTN1) },
                { "right", new InputButton((int)GlobalButtonVals.BTN2) },
                { "up", new InputButton((int)GlobalButtonVals.BTN3) },
                { "down", new InputButton((int)GlobalButtonVals.BTN4) },
                { "rleft", new InputButton((int)GlobalButtonVals.BTN5) },
                { "rright", new InputButton((int)GlobalButtonVals.BTN6) },
                { "rup", new InputButton((int)GlobalButtonVals.BTN7) },
                { "rdown", new InputButton((int)GlobalButtonVals.BTN8) },
                { "square", new InputButton((int)GlobalButtonVals.BTN9) },
                { "triangle", new InputButton((int)GlobalButtonVals.BTN10) },
                { "circle", new InputButton((int)GlobalButtonVals.BTN11) },
                { "cross", new InputButton((int)GlobalButtonVals.BTN12) },
                { "select", new InputButton((int)GlobalButtonVals.BTN13) },
                { "start", new InputButton((int)GlobalButtonVals.BTN14) },
                { "l1", new InputButton((int)GlobalButtonVals.BTN15) },
                { "r1", new InputButton((int)GlobalButtonVals.BTN16) },
                { "l2", new InputButton((int)GlobalButtonVals.BTN17) },
                { "r2", new InputButton((int)GlobalButtonVals.BTN18) },
                { "l3", new InputButton((int)GlobalButtonVals.BTN19) },
                { "r3", new InputButton((int)GlobalButtonVals.BTN20) },
                { "dup", new InputButton((int)GlobalButtonVals.BTN21) },
                { "ddown", new InputButton((int)GlobalButtonVals.BTN22) },
                { "dleft", new InputButton((int)GlobalButtonVals.BTN23) },
                { "dright", new InputButton((int)GlobalButtonVals.BTN24) },
                { "ss1", new InputButton((int)GlobalButtonVals.BTN25) },
                { "ss2", new InputButton((int)GlobalButtonVals.BTN26) },
                { "ss3", new InputButton((int)GlobalButtonVals.BTN27) },
                { "ss4", new InputButton((int)GlobalButtonVals.BTN28) },
                { "ls1", new InputButton((int)GlobalButtonVals.BTN29) },
                { "ls2", new InputButton((int)GlobalButtonVals.BTN30) },
                { "ls3", new InputButton((int)GlobalButtonVals.BTN31) },
                { "ls4", new InputButton((int)GlobalButtonVals.BTN32) },
            };

            ValidInputs = new List<string>(33)
            {
                "up", "down", "left", "right", "rup", "rdown", "rleft", "rright",
                "square", "triangle", "circle", "cross", "l1", "r1", "l2", "r2", "l3", "r3", "dup", "ddown", "dleft", "dright", "select", "start",
                "ss1", "ss2", "ss3", "ss4",
                "ls1", "ls2", "ls3", "ls4",
                "#"
            };

            InputAxesMap = new Dictionary<string, InputAxis>(8)
            {
                { "left", new InputAxis((int)GlobalAxisVals.AXIS_X, 0, -1) },
                { "right", new InputAxis((int)GlobalAxisVals.AXIS_X, 0, 1) },
                { "up", new InputAxis((int)GlobalAxisVals.AXIS_Y, 0, -1) },
                { "down", new InputAxis((int)GlobalAxisVals.AXIS_Y, 0, 1) },
                { "rleft", new InputAxis((int)GlobalAxisVals.AXIS_RX, 0, -1) },
                { "rright", new InputAxis((int)GlobalAxisVals.AXIS_RX, 0, 1) },
                { "rup", new InputAxis((int)GlobalAxisVals.AXIS_RY, 0, -1) },
                { "rdown", new InputAxis((int)GlobalAxisVals.AXIS_RY, 0, 1) }
            };*/
        }
    }
}
