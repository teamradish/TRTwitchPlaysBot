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
    /// The interface for all virtual controllers.
    /// </summary>
    public interface IVirtualController : IDisposable
    {
        /// <summary>
        /// The ID of the controller.
        /// </summary>
        uint ControllerID { get; }

        /// <summary>
        /// The index of the controller (Ex. port 1).
        /// </summary>
        int ControllerIndex { get; }

        /// <summary>
        /// Tells whether the controller device was successfully acquired.
        /// If this is false, don't use this controller instance to make inputs.
        /// </summary>
        bool IsAcquired { get; }

        /// <summary>
        /// Acquires the controller device.
        /// Make sure the device is acquired before initializing.
        /// </summary>
        void Acquire();

        /// <summary>
        /// Closes the controller device.
        /// Make sure to call this when the device is no longer needed.
        /// </summary>
        void Close();

        /// <summary>
        /// Sets up the controller after it has been acquired.
        /// </summary>
        void Init();

        /// <summary>
        /// Resets the controller to its defaults, including all button and axes presses.
        /// </summary>
        void Reset();

        #region Inputs

        void PressInput(in Parser.Input input);
        void ReleaseInput(in Parser.Input input);

        void PressAxis(in int axis, in bool min, in int percent);
        void ReleaseAxis(in int axis);

        void PressAbsoluteAxis(in int axis, in int percent);
        void ReleaseAbsoluteAxis(in int axis);

        void PressButton(in uint buttonVal);
        void ReleaseButton(in uint buttonVal);

        ButtonStates GetButtonState(in uint buttonVal);

        /// <summary>
        /// Updates the virtual device by applying all changes.
        /// </summary>
        void UpdateController();

        #endregion
    }
}
