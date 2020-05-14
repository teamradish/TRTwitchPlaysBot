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
    /// The Playstation 2.
    /// </summary>
    public sealed class PS2Console : ConsoleBase
    {
        public override Dictionary<string, uint> ButtonInputMap { get; protected set; } = new Dictionary<string, uint>()
        {
            { "left", (int)GlobalButtonVals.BTN1 },
            { "right", (int)GlobalButtonVals.BTN2 },
            { "up", (int)GlobalButtonVals.BTN3 },
            { "down", (int)GlobalButtonVals.BTN4 },
            { "rleft", (int)GlobalButtonVals.BTN5 },
            { "rright", (int)GlobalButtonVals.BTN6 },
            { "rup", (int)GlobalButtonVals.BTN7 },
            { "rdown", (int)GlobalButtonVals.BTN8 },
            { "square", (int)GlobalButtonVals.BTN9 },
            { "triangle", (int)GlobalButtonVals.BTN10 },
            { "circle", (int)GlobalButtonVals.BTN11 },
            { "cross", (int)GlobalButtonVals.BTN12 },
            { "select", (int)GlobalButtonVals.BTN13 },
            { "start", (int)GlobalButtonVals.BTN14 },
            { "l1", (int)GlobalButtonVals.BTN15 },
            { "r1", (int)GlobalButtonVals.BTN16 },
            { "l2", (int)GlobalButtonVals.BTN17 },
            { "r2", (int)GlobalButtonVals.BTN18 },
            { "l3", (int)GlobalButtonVals.BTN19 },
            { "r3", (int)GlobalButtonVals.BTN20 },
            { "dup", (int)GlobalButtonVals.BTN21 },
            { "ddown", (int)GlobalButtonVals.BTN22 },
            { "dleft", (int)GlobalButtonVals.BTN23 },
            { "dright", (int)GlobalButtonVals.BTN24 },
            { "ss1", (int)GlobalButtonVals.BTN25 },
            { "ss2", (int)GlobalButtonVals.BTN26 },
            { "ss3", (int)GlobalButtonVals.BTN27 },
            { "ss4", (int)GlobalButtonVals.BTN28 },
            { "ls1", (int)GlobalButtonVals.BTN29 },
            { "ls2", (int)GlobalButtonVals.BTN30 },
            { "ls3", (int)GlobalButtonVals.BTN31 },
            { "ls4", (int)GlobalButtonVals.BTN32 },
        };

        public override string[] ValidInputs { get; protected set; } = new string[]
        {
            "up", "down", "left", "right", "rup", "rdown", "rleft", "rright",
            "square", "triangle", "circle", "cross", "l1", "r1", "l2", "r2", "l3", "r3", "dup", "ddown", "dleft", "dright", "select", "start",
            "ss1", "ss2", "ss3", "ss4",
            "ls1", "ls2", "ls3", "ls4",
            "#", "."
        };

        public override Dictionary<string, int> InputAxes { get; protected set; } = new Dictionary<string, int>()
        {
            { "left", (int)GlobalAxisVals.AXIS_X },
            { "right", (int)GlobalAxisVals.AXIS_X },
            { "up", (int)GlobalAxisVals.AXIS_Y },
            { "down", (int)GlobalAxisVals.AXIS_Y },
            { "rleft", (int)GlobalAxisVals.AXIS_RX },
            { "rright", (int)GlobalAxisVals.AXIS_RX },
            { "rup", (int)GlobalAxisVals.AXIS_RY },
            { "rdown", (int)GlobalAxisVals.AXIS_RY }
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
            return (input.name == "left" || input.name == "up" || input.name == "rleft" || input.name == "rup");
        }

        public override bool IsButton(in Parser.Input input)
        {
            return (IsWait(input) == false);
        }
    }
}
