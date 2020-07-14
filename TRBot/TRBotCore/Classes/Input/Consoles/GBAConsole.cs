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
    /// The Game Boy Advance handheld.
    /// </summary>
    public sealed class GBAConsole : ConsoleBase
    {
        public override Dictionary<string, InputAxis> InputAxes { get; protected set; } = new Dictionary<string, InputAxis>();

        public override Dictionary<string, InputButton> ButtonInputMap { get; protected set; } = new Dictionary<string, InputButton>()
        {
            { "left", new InputButton((int)GlobalButtonVals.BTN1) },
            { "right", new InputButton((int)GlobalButtonVals.BTN2) },
            { "up", new InputButton((int)GlobalButtonVals.BTN3) },
            { "down", new InputButton((int)GlobalButtonVals.BTN4) },
            { "a", new InputButton((int)GlobalButtonVals.BTN5) },
            { "b", new InputButton((int)GlobalButtonVals.BTN6) },
            { "select", new InputButton((int)GlobalButtonVals.BTN7) },
            { "start", new InputButton((int)GlobalButtonVals.BTN8) },
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
            { "l", new InputButton((int)GlobalButtonVals.BTN31) },
            { "r", new InputButton((int)GlobalButtonVals.BTN32) }
        };

        public override string[] ValidInputs { get; protected set; } = new string[]
        {
            "up", "down", "left", "right", "a", "b", "select", "start", "l", "r",
            "ss1", "ss2", "ss3", "ss4", "ss5", "ss6",
            "ls1", "ls2", "ls3", "ls4", "ls5", "ls6",
            "#"
        };

        public override bool GetAxis(in Parser.Input input, out InputAxis axis)
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
