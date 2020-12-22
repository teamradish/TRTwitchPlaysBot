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
    /// The Nintendo Wii.
    /// </summary>
    public sealed class WiiConsole : GameConsole
    {
        /// <summary>
        /// The various extensions for the Wii.
        /// </summary>
        /// <remarks>Some peripherals, such as the Wii Balance Board, currently cannot be emulated at all (see Dolphin wiki).</remarks>
        public enum WiiInputExtensions
        {
            None = 0,
            Nunchuk = 1,
            ClassicController = 2,
            Guitar = 3,
            DrumKit = 4,
            DJTurntable = 5,
            uDrawGameTablet = 6,
            DrawsomeTablet = 7,
            TaikoDrum = 8
        }

        /// <summary>
        /// The current input mode for the Wii.
        /// </summary>
        public WiiInputExtensions InputExtension = WiiInputExtensions.Nunchuk;

        public WiiConsole()
        {
            Name = "wii";

            Initialize();

            UpdateInputRegex();
        }

        private void Initialize()
        {
            SetConsoleInputs(new Dictionary<string, InputData>(39) {
                { "left",       InputData.CreateAxis("left", (int)GlobalAxisVals.AXIS_X, 0, -1) },
                { "right",      InputData.CreateAxis("right", (int)GlobalAxisVals.AXIS_X, 0, 1) },
                { "up",         InputData.CreateAxis("up", (int)GlobalAxisVals.AXIS_Y, 0, -1) },
                { "down",       InputData.CreateAxis("down", (int)GlobalAxisVals.AXIS_Y, 0, 1) },
                { "tleft",      InputData.CreateAxis("tleft", (int)GlobalAxisVals.AXIS_RX, 0, -1) },
                { "tright",     InputData.CreateAxis("tright", (int)GlobalAxisVals.AXIS_RX, 0, 1) },
                { "tforward",   InputData.CreateAxis("tforward", (int)GlobalAxisVals.AXIS_RY, 0, -1) },
                { "tback",      InputData.CreateAxis("tback", (int)GlobalAxisVals.AXIS_RY, 0, 1) },
                { "pleft",      InputData.CreateAxis("pleft", (int)GlobalAxisVals.AXIS_RZ, 0, -1) },
                { "pright",     InputData.CreateAxis("pright", (int)GlobalAxisVals.AXIS_RZ, 0, 1) },
                { "pup",        InputData.CreateAxis("pup", (int)GlobalAxisVals.AXIS_Z, 0, -1) },
                { "pdown",      InputData.CreateAxis("pdown", (int)GlobalAxisVals.AXIS_Z, 0, 1) },
                
                { "c",          InputData.CreateButton("c", (int)GlobalButtonVals.BTN1) },
                { "z",          InputData.CreateButton("z", (int)GlobalButtonVals.BTN2) },                
                { "a",          InputData.CreateButton("a", (int)GlobalButtonVals.BTN5) },
                { "b",          InputData.CreateButton("b", (int)GlobalButtonVals.BTN6) },
                { "one",        InputData.CreateButton("one", (int)GlobalButtonVals.BTN7) },
                { "two",        InputData.CreateButton("two", (int)GlobalButtonVals.BTN8) },
                { "minus",      InputData.CreateButton("minus", (int)GlobalButtonVals.BTN9) },
                { "plus",       InputData.CreateButton("plus", (int)GlobalButtonVals.BTN10) },
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
                { "shake",      InputData.CreateButton("shake", (int)GlobalButtonVals.BTN31) },
                { "point",      InputData.CreateButton("point", (int)GlobalButtonVals.BTN32) },
                { "#",          InputData.CreateBlank("#") }
            });

            InvalidCombos = new List<InvalidCombo>();
        }
    }
}
