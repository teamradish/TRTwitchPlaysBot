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
    /// Delegates for virtual controller events.
    /// </summary>
    public static class VirtualControllerDelegates
    {
        public delegate void OnInputPressed(in Parser.Input inputPressed);
        public delegate void OnInputReleased(in Parser.Input inputPressed);

        public delegate void OnAxisPressed(in int axis, in int percent);
        public delegate void OnAxisReleased(in int axis);

        public delegate void OnButtonPressed(in uint buttonVal);
        public delegate void OnButtonReleased(in uint buttonVal);

        public delegate void OnControllerUpdated();
        public delegate void OnControllerReset();

        public delegate void OnControllerClosed();
    }
}
