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

namespace TRBot
{
    /// <summary>
    /// The Nintendo Wii.
    /// </summary>
    public sealed class WiiConsole : ConsoleBase
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

        public override string[] ValidInputs { get; protected set; } = new string[]
        {
            "left", "right", "up", "down",
            "pleft", "pright", "pup", "pdown",
            "tleft", "tright", "tup", "tdown",
            "dleft", "dright", "dup", "ddown",
            "a", "b", "one", "two", "minus", "plus",
            "c", "z",
            "shake", "point",
            "ss1", "ss2", "ss3", "ss4", "ss5", "ss6",
            "ls1", "ls2", "ls3", "ls4", "ls5", "ls6",
            "#"
        };
        
        public override Dictionary<string, InputAxis> InputAxes { get; protected set; } = new Dictionary<string, InputAxis>()
        {
            { "left",       new InputAxis((int)GlobalAxisVals.AXIS_X, -1, 0) },
            { "right",      new InputAxis((int)GlobalAxisVals.AXIS_X, 0, 1) },
            { "up",         new InputAxis((int)GlobalAxisVals.AXIS_Y, -1, 0) },
            { "down",       new InputAxis((int)GlobalAxisVals.AXIS_Y, 0, 1) },
            { "tleft",      new InputAxis((int)GlobalAxisVals.AXIS_RX, -1, 0) },
            { "tright",     new InputAxis((int)GlobalAxisVals.AXIS_RX, 0, 1) },
            { "tforward",   new InputAxis((int)GlobalAxisVals.AXIS_RY, -1, 0) },
            { "tback",      new InputAxis((int)GlobalAxisVals.AXIS_RY, 0, 1) },
            { "pleft",      new InputAxis((int)GlobalAxisVals.AXIS_RZ, -1, 0) },
            { "pright",     new InputAxis((int)GlobalAxisVals.AXIS_RZ, 0, 1) },
            { "pup",        new InputAxis((int)GlobalAxisVals.AXIS_Z, -1, 0) },
            { "pdown",      new InputAxis((int)GlobalAxisVals.AXIS_Z, 0, 1) }
        };

        //Kimimaru: NOTE - Though some virtual controllers support up to 128 buttons, Dolphin supports only 32 max
        //The Wii Remote + Nunchuk has more than 32 inputs, so since we can't fit them all, we'll need some modes to toggle
        //and change the input map based on the input scheme
        //We might have to make some sacrifices, such as fewer savestates or disallowing use of the D-pad if a Nunchuk is being used
        public override Dictionary<string, InputButton> ButtonInputMap { get; protected set; } = new Dictionary<string, InputButton>() {
            { "left",       new InputButton((int)GlobalButtonVals.BTN1) }, { "c",          new InputButton((int)GlobalButtonVals.BTN1) },
            { "right",      new InputButton((int)GlobalButtonVals.BTN2) }, { "z",          new InputButton((int)GlobalButtonVals.BTN2) },
            { "up",         new InputButton((int)GlobalButtonVals.BTN3) }, { "tleft",      new InputButton((int)GlobalButtonVals.BTN3) },
            { "down",       new InputButton((int)GlobalButtonVals.BTN4) }, { "tright",     new InputButton((int)GlobalButtonVals.BTN4) },
            { "a",          new InputButton((int)GlobalButtonVals.BTN5) },
            { "b",          new InputButton((int)GlobalButtonVals.BTN6) },
            { "one",        new InputButton((int)GlobalButtonVals.BTN7) },
            { "two",        new InputButton((int)GlobalButtonVals.BTN8) },
            { "minus",      new InputButton((int)GlobalButtonVals.BTN9) },
            { "plus",       new InputButton((int)GlobalButtonVals.BTN10) },
            { "pleft",      new InputButton((int)GlobalButtonVals.BTN11) },
            { "pright",     new InputButton((int)GlobalButtonVals.BTN12) },
            { "pup",        new InputButton((int)GlobalButtonVals.BTN13) },
            { "pdown",      new InputButton((int)GlobalButtonVals.BTN14) },
            { "dleft",      new InputButton((int)GlobalButtonVals.BTN15) },
            { "dright",     new InputButton((int)GlobalButtonVals.BTN16) },
            { "dup",        new InputButton((int)GlobalButtonVals.BTN17) },
            { "ddown",      new InputButton((int)GlobalButtonVals.BTN18) },
            { "ss1",        new InputButton((int)GlobalButtonVals.BTN19) }, { "tforward",  new InputButton((int)GlobalButtonVals.BTN19) },
            { "ss2",        new InputButton((int)GlobalButtonVals.BTN20) }, { "tback",     new InputButton((int)GlobalButtonVals.BTN20) },
            { "ss3",        new InputButton((int)GlobalButtonVals.BTN21) },
            { "ss4",        new InputButton((int)GlobalButtonVals.BTN22) },
            { "ss5",        new InputButton((int)GlobalButtonVals.BTN23) },
            { "ss6",        new InputButton((int)GlobalButtonVals.BTN24) },
            { "ls1",        new InputButton((int)GlobalButtonVals.BTN25) },
            { "ls2",        new InputButton((int)GlobalButtonVals.BTN26) },
            { "ls3",        new InputButton((int)GlobalButtonVals.BTN27) },
            { "ls4",        new InputButton((int)GlobalButtonVals.BTN28) },
            { "ls5",        new InputButton((int)GlobalButtonVals.BTN29) },
            { "ls6",        new InputButton((int)GlobalButtonVals.BTN30) },
            { "shake",      new InputButton((int)GlobalButtonVals.BTN31) },
            { "point",      new InputButton((int)GlobalButtonVals.BTN32) }
        };
        
        public override void HandleArgsOnConsoleChange(List<string> arguments)
        {
            if (arguments?.Count == 0) return;

            for (int i = 0; i < arguments.Count; i++)
            {
                //Check for enum mode
                if (Enum.TryParse(arguments[i], true, out WiiInputExtensions extension) == false)
                {
                    BotProgram.MsgHandler.QueueMessage("Invalid input extension argument for the Wii.");
                    break;
                }

                int extensionInt = (int)extension;

                if (extensionInt < 0 || extensionInt > (int)WiiInputExtensions.TaikoDrum)
                {
                    BotProgram.MsgHandler.QueueMessage("Invalid input extension argument for the Wii.");
                    return;
                }

                InputExtension = extension;
                BotProgram.MsgHandler.QueueMessage($"Changed Wii input extension to {extension}!");
            }
        }

        public override bool GetAxis(in Parser.Input input, out InputAxis axis)
        {
            return InputAxes.TryGetValue(input.name, out axis);
        }

        public override bool IsAbsoluteAxis(in Parser.Input input)
        {
            return false;
        }

        public override bool IsAxis(in Parser.Input input)
        {
            return (InputAxes.ContainsKey(input.name) == true);
        }

        public override bool IsMinAxis(in Parser.Input input)
        {
            return (input.name == "left" || input.name == "up" || input.name == "tleft" || input.name == "tforward"
                || input.name == "pleft" || input.name == "pup");
        }

        public override bool IsButton(in Parser.Input input)
        {
            return (IsWait(input) == false && IsAxis(input) == false);
        }
    }
}
