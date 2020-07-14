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
    /// A desktop computer.
    /// </summary>
    public sealed class PCConsole : ConsoleBase
    {
        public override string[] ValidInputs { get; protected set; } = new string[]
        {
            "left", "right", "up", "down",
            "lclick", "mclick", "rclick",
            "return", "space",
            "q", "w", "e", "r", "a", "s", "d", "p",
            "#"
        };

        public override Dictionary<string, InputAxis> InputAxes { get; protected set; } = new Dictionary<string, InputAxis>()
        {
            { "left", new InputAxis((int)GlobalAxisVals.AXIS_X, -1, 0) },
            { "right", new InputAxis((int)GlobalAxisVals.AXIS_X, 0, 1) },
            { "up", new InputAxis((int)GlobalAxisVals.AXIS_Y, -1, 0) },
            { "down", new InputAxis((int)GlobalAxisVals.AXIS_Y, 0, 1) }
        };

        public override Dictionary<string, InputButton> ButtonInputMap { get; protected set; } = new Dictionary<string, InputButton>()
        {
            { "left", new InputButton((int)GlobalButtonVals.BTN1) },
            { "right", new InputButton((int)GlobalButtonVals.BTN2) },
            { "up", new InputButton((int)GlobalButtonVals.BTN3) },
            { "down", new InputButton((int)GlobalButtonVals.BTN4) },
            { "lclick", new InputButton((int)GlobalButtonVals.BTN5) },
            { "mclick", new InputButton((int)GlobalButtonVals.BTN6) },
            { "rclick", new InputButton((int)GlobalButtonVals.BTN7) },
            { "return", new InputButton((int)GlobalButtonVals.BTN8) },
            { "space", new InputButton((int)GlobalButtonVals.BTN9) },
            { "q", new InputButton((int)GlobalButtonVals.BTN10) },
            { "w", new InputButton((int)GlobalButtonVals.BTN11) },
            { "e", new InputButton((int)GlobalButtonVals.BTN12) },
            { "r", new InputButton((int)GlobalButtonVals.BTN13) },
            { "a", new InputButton((int)GlobalButtonVals.BTN14) },
            { "s", new InputButton((int)GlobalButtonVals.BTN15) },
            { "d", new InputButton((int)GlobalButtonVals.BTN16) },
            { "p", new InputButton((int)GlobalButtonVals.BTN17) }
        };

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
