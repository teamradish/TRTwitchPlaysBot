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
    /// The invocation types determining when an input callback is invoked.
    /// <para>This is a bitwise field.</para>
    /// </summary>
    [Flags]
    public enum InputCBInvocation
    {
        None = 0,
        Press = 1 << 0,
        Hold = 1 << 1,
        Release = 1 << 2
    }

    /// <summary>
    /// The available callback types.
    /// </summary>
    public enum InputCBTypes
    {
        SavestateLog = 0,
        BotMessage = 1,
        SavestateSlotLog = 2,
        ChangeStateSlot = 3
    }
}
