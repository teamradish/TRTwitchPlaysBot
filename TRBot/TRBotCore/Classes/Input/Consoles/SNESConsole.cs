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
    /// The SNES, or Super Famicom.
    /// </summary>
    public sealed class SNESConsole : ConsoleBase
    {
        public override Dictionary<string, int> InputAxes { get; protected set; } = new Dictionary<string, int>();

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
            { "select", (int)GlobalButtonVals.BTN9 },
            { "start", (int)GlobalButtonVals.BTN10 },
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
            { "x", (int)GlobalButtonVals.BTN31 },
            { "y", (int)GlobalButtonVals.BTN32 }
        };

        public override string[] ValidInputs { get; protected set; } = new string[]
        {
            "up", "down", "left", "right", "a", "b", "x", "y", "l", "r", "select", "start",
            "savestate1", "savestate2", "savestate3", "savestate4", "savestate5", "savestate6", "ss1", "ss2", "ss3", "ss4", "ss5", "ss6",
            "loadstate1", "loadstate2", "loadstate3", "loadstate4", "loadstate5", "loadstate6", "ls1", "ls2", "ls3", "ls4", "ls5", "ls6",
            "#", "."
        };

        public override bool GetAxis(in Parser.Input input, out int axis)
        {
            axis = default;
            return false;
        }

        public override bool IsAbsoluteAxis(in Parser.Input input) => false;

        public override bool IsAxis(in Parser.Input input) => false;

        public override bool IsMinAxis(in Parser.Input input) => false;

        public override bool IsButton(in Parser.Input input)
        {
            return (IsWait(input) == false);
        }
    }
}
