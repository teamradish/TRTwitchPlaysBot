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
    /// The PS4.
    /// </summary>
    public sealed class PS4Console : ConsoleBase
    {
        public override Dictionary<string, InputAxis> InputAxes { get; protected set; } = new Dictionary<string, InputAxis>();

        public override Dictionary<string, InputButton> ButtonInputMap { get; protected set; } = new Dictionary<string, InputButton>()
        {
            { "left", new InputButton((int)GlobalButtonVals.BTN1) },
            { "right", new InputButton((int)GlobalButtonVals.BTN2) },
            { "up", new InputButton((int)GlobalButtonVals.BTN3) },
            { "down", new InputButton((int)GlobalButtonVals.BTN4) },
            { "cross", new InputButton((int)GlobalButtonVals.BTN5) },
            { "circle", new InputButton((int)GlobalButtonVals.BTN6) },
            { "triangle", new InputButton((int)GlobalButtonVals.BTN7) },
            { "square", new InputButton((int)GlobalButtonVals.BTN8) },
            { "touchclick", new InputButton((int)GlobalButtonVals.BTN9) },
            { "options", new InputButton((int)GlobalButtonVals.BTN10) },
            { "dleft", new InputButton((int)GlobalButtonVals.BTN11) },
            { "dright", new InputButton((int)GlobalButtonVals.BTN12) },
            { "dup", new InputButton((int)GlobalButtonVals.BTN13) },
            { "ddown", new InputButton((int)GlobalButtonVals.BTN14) },
            { "rsleft", new InputButton((int)GlobalButtonVals.BTN15) },
            { "rsright", new InputButton((int)GlobalButtonVals.BTN16) },
            { "rsup", new InputButton((int)GlobalButtonVals.BTN17) },
            { "rsdown", new InputButton((int)GlobalButtonVals.BTN18) },
            { "l1", new InputButton((int)GlobalButtonVals.BTN19) },
            { "r1", new InputButton((int)GlobalButtonVals.BTN20) },
            { "l2", new InputButton((int)GlobalButtonVals.BTN21) },
            { "r2", new InputButton((int)GlobalButtonVals.BTN22) },
            { "l3", new InputButton((int)GlobalButtonVals.BTN23) },
            { "r3", new InputButton((int)GlobalButtonVals.BTN24) },
            { "touchpad", new InputButton((int)GlobalButtonVals.BTN25) },
            { "pausesplit", new InputButton((int)GlobalButtonVals.BTN26) },
            { "resetsplit", new InputButton((int)GlobalButtonVals.BTN27) },
            { "sparebutton", new InputButton((int)GlobalButtonVals.BTN28) },
        };

        public override string[] ValidInputs { get; protected set; } = new string[]
        {
            "up", "down", "left", "right", "circle", "cross", "square", "triangle", "l1", "r1", "l2", "r2","l3", "r3", "touchclick", "options",
            "dleft", "dright", "dup", "ddown", "rsleft", "rsright",
            "rsup", "rsdown", "touchpad",
            "#"
        };

        public override bool GetAxis(in Parser.Input input, out InputAxis axis)
        {
            axis = default;
            return false;
        }

        public override bool IsAxis(in Parser.Input input) => false;

        public override bool IsButton(in Parser.Input input)
        {
            return (IsWait(input) == false);
        }
    }
}