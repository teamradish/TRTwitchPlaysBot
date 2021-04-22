/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, version 3 of the License.
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

namespace TRBot.VirtualControllers
{
    /// <summary>
    /// The types of virtual controllers available.
    /// </summary>
    public enum VirtualControllerTypes
    {
        /// <summary>
        /// A dummy virtual controller used as a fallback.
        /// </summary>
        Dummy = 0,

        /// <summary>
        /// vJoy virtual controllers. Supported only on Windows platforms.
        /// </summary>
        vJoy = 1,

        /// <summary>
        /// uinput virtual controllers. Supported only on GNU/Linux platforms.
        /// </summary>
        uinput = 2,

        /// <summary>
        /// A virtual controller utilizing xdotool on GNU/Linux platforms running X11 to
        /// enable mouse and keyboard controls. EXPERIMENTAL.
        /// </summary>
        xdotool = 3
    }
}
