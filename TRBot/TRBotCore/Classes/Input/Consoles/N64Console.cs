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
    /// The Nintendo 64.
    /// </summary>
    public sealed class N64Console : ConsoleBase
    {
        public override string[] ValidInputs { get; protected set; } = new string[]
        {
            "left", "right", "up", "down",
            "dleft", "dright", "dup", "ddown",
            "cleft", "cright", "cup", "cdown",
            "a", "b", "l", "r", "z",
            "start",
            "savestate1", "savestate2", "savestate3", "savestate4", "savestate5", "savestate6", "ss1", "ss2", "ss3", "ss4", "ss5", "ss6",
            "loadstate1", "loadstate2", "loadstate3", "loadstate4", "loadstate5", "loadstate6", "ls1", "ls2", "ls3", "ls4", "ls5", "ls6",
            "#", "."
        };

        public override Dictionary<string, int> InputAxes { get; protected set; } = new Dictionary<string, int>()
        {
            { "left", (int)GlobalAxisVals.AXIS_X },
            { "right", (int)GlobalAxisVals.AXIS_X },
            { "up", (int)GlobalAxisVals.AXIS_Y },
            { "down", (int)GlobalAxisVals.AXIS_Y }
        };

        public override Dictionary<string, uint> ButtonInputMap { get; protected set; } = new Dictionary<string, uint>()
        {
            { "left", (int)GlobalButtonVals.BTN1 },
            { "right", (int)GlobalButtonVals.BTN2 },
            { "up", (int)GlobalButtonVals.BTN3 },
            { "down", (int)GlobalButtonVals.BTN4 },
            { "a", (int)GlobalButtonVals.BTN5 },
            { "b", (int)GlobalButtonVals.BTN6 },
            { "l", (int)GlobalButtonVals.BTN7 },
            { "r", (int)GlobalButtonVals.BTN8 },
            { "z", (int)GlobalButtonVals.BTN9 },
            { "start", (int)GlobalButtonVals.BTN10 },
            { "cleft", (int)GlobalButtonVals.BTN11 },
            { "cright", (int)GlobalButtonVals.BTN12 },
            { "cup", (int)GlobalButtonVals.BTN13 },
            { "cdown", (int)GlobalButtonVals.BTN14 },
            { "dleft", (int)GlobalButtonVals.BTN15 },
            { "dright", (int)GlobalButtonVals.BTN16 },
            { "dup", (int)GlobalButtonVals.BTN17 },
            { "ddown", (int)GlobalButtonVals.BTN18 },
            { "savestate1", (int)GlobalButtonVals.BTN19 }, { "ss1", (int)GlobalButtonVals.BTN19 },
            { "savestate2", (int)GlobalButtonVals.BTN20 }, { "ss2", (int)GlobalButtonVals.BTN20 },
            { "savestate3", (int)GlobalButtonVals.BTN21 }, { "ss3", (int)GlobalButtonVals.BTN21 },
            { "savestate4", (int)GlobalButtonVals.BTN22 }, { "ss4", (int)GlobalButtonVals.BTN22 },
            { "savestate5", (int)GlobalButtonVals.BTN23 }, { "ss5", (int)GlobalButtonVals.BTN23 },
            { "savestate6", (int)GlobalButtonVals.BTN24 }, { "ss6", (int)GlobalButtonVals.BTN24 },
            { "loadstate1", (int)GlobalButtonVals.BTN25 }, { "ls1", (int)GlobalButtonVals.BTN25 },
            { "loadstate2", (int)GlobalButtonVals.BTN26 }, { "ls2", (int)GlobalButtonVals.BTN26 },
            { "loadstate3", (int)GlobalButtonVals.BTN27 }, { "ls3", (int)GlobalButtonVals.BTN27 },
            { "loadstate4", (int)GlobalButtonVals.BTN28 }, { "ls4", (int)GlobalButtonVals.BTN28 },
            { "loadstate5", (int)GlobalButtonVals.BTN29 }, { "ls5", (int)GlobalButtonVals.BTN29 },
            { "loadstate6", (int)GlobalButtonVals.BTN30 }, { "ls6", (int)GlobalButtonVals.BTN30 },
        };

        public override bool GetAxis(in Parser.Input input, out int axis)
        {
            return InputAxes.TryGetValue(input.name, out axis);
        }

        public override bool IsAbsoluteAxis(in Parser.Input input)
        {
            return false;
        }

        public override bool IsAxis(in Parser.Input input)
        {
            return InputAxes.ContainsKey(input.name);
        }

        public override bool IsMinAxis(in Parser.Input input)
        {
            return (input.name == "up" || input.name == "left");
        }
        
        public override bool IsButton(in Parser.Input input)
        {
            return (IsWait(input) == false);
        }
    }
}
