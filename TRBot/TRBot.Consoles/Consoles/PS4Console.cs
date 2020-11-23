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
    /// The PlayStation 4.
    /// </summary>
    public sealed class PS4Console : GameConsole
    {
        public PS4Console()
        {
            Name = "ps4";

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
                { "swipeleft",  InputData.CreateAxis("swipeleft", (int)GlobalAxisVals.AXIS_M1, 0, -1) },
                { "swiperight", InputData.CreateAxis("swiperight", (int)GlobalAxisVals.AXIS_M1, 0, 1) },
                { "swipeup",    InputData.CreateAxis("swipeup", (int)GlobalAxisVals.AXIS_M2, 0, -1) },
                { "swipedown",  InputData.CreateAxis("swipedown", (int)GlobalAxisVals.AXIS_M2, 0, 1) },

                { "square",     InputData.CreateButton("square", (int)GlobalButtonVals.BTN9) },
                { "triangle",   InputData.CreateButton("triangle", (int)GlobalButtonVals.BTN10) },
                { "circle",     InputData.CreateButton("circle", (int)GlobalButtonVals.BTN11) },
                { "cross",      InputData.CreateButton("cross", (int)GlobalButtonVals.BTN12) },
                { "share",      InputData.CreateButton("share", (int)GlobalButtonVals.BTN13) },
                { "options",      InputData.CreateButton("options", (int)GlobalButtonVals.BTN14) },
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
                { "touchclick", InputData.CreateButton("touchclick", (int)GlobalButtonVals.BTN25) },
                { "#",          InputData.CreateBlank("#") },

                //Spare buttons
                { "sb1",        InputData.CreateButton("sb1", (int)GlobalButtonVals.BTN26) },
                { "sb2",        InputData.CreateButton("sb2", (int)GlobalButtonVals.BTN27) },
                { "sb3",        InputData.CreateButton("sb3", (int)GlobalButtonVals.BTN28) },
                { "sb4",        InputData.CreateButton("sb4", (int)GlobalButtonVals.BTN29) },
                { "sb5",        InputData.CreateButton("sb5", (int)GlobalButtonVals.BTN30) },
                { "sb6",        InputData.CreateButton("sb6", (int)GlobalButtonVals.BTN31) },
                { "sb7",        InputData.CreateButton("sb7", (int)GlobalButtonVals.BTN32) },
            });

            InvalidCombos = new List<InvalidCombo>();
        }
    }
}
