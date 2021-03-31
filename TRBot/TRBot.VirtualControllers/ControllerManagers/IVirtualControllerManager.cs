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
    /// The interface for all virtual controller managers.
    /// </summary>
    public interface IVirtualControllerManager : IDisposable
    {
        /// <summary>
        /// Tells if the virtual controller manager is initialized.
        /// </summary>
        bool Initialized { get; }

        /// <summary>
        /// Tells how many virtual controllers are managed by the virtual controller manager.
        /// Note that this doesn't correspond to how many virtual controllers are acquired and usable.
        /// </summary>
        int ControllerCount { get; }

        /// <summary>
        /// The minimum number of controllers the manager can handle.
        /// </summary>
        int MinControllers { get; }

        /// <summary>
        /// The maximum number of controllers the manager can handle.
        /// </summary>
        int MaxControllers { get; }

        /// <summary>
        /// Initializes the virtual controller manager.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Initializes a certain number of virtual controllers, returning how many were successfully initialized.
        /// </summary>
        /// <param name="controllerCount">The number of virtual controllers to initialize.</param>
        /// <returns>The number of virtual controllers successfully initialized.</returns>
        int InitControllers(in int controllerCount);

        /// <summary>
        /// Retrieves a virtual controller at a certain index (controller port).
        /// </summary>
        /// <param name="controllerPort">The controller port of the virtual controller.</param>
        /// <returns>A virtual controller at the controller port if found, otherwise null.</returns>
        IVirtualController GetController(in int controllerPort);
    }
}
