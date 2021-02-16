/* Copyright (C) 2019-2020 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot,software for playing games through text.
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
using static TRBot.VirtualControllers.VirtualControllerDelegates;

namespace TRBot.VirtualControllers
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

        void SetInputNamePressed(in string inputName);
        void SetInputNameReleased(in string inputName);

        void PressAxis(in int axis, in double minAxisVal, in double maxAxisVal, in double percent);
        void ReleaseAxis(in int axis);

        void PressButton(in uint buttonVal);
        void ReleaseButton(in uint buttonVal);

        ButtonStates GetInputState(in string inputName);

        ButtonStates GetButtonState(in uint buttonVal);

        double GetAxisState(in int axisVal);

        /// <summary>
        /// Updates the virtual device by applying all changes.
        /// </summary>
        void UpdateController();

        #endregion

        #region Events

        /// <summary>
        /// An event invoked after an input is pressed on the controller.
        /// </summary>
        event OnInputPressed InputPressedEvent;

        /// <summary>
        /// An event invoked after an input is released on the controller.
        /// </summary>
        event OnInputReleased InputReleasedEvent;

        /// <summary>
        /// An event invoked after a button is pressed on the controller.
        /// </summary>
        event OnButtonPressed ButtonPressedEvent;

        /// <summary>
        /// An event invoked after a button is released on the controller.
        /// </summary>
        event OnButtonReleased ButtonReleasedEvent;

        /// <summary>
        /// An event invoked after an axis is pressed on the controller.
        /// </summary>
        event OnAxisPressed AxisPressedEvent;

        /// <summary>
        /// An event invoked after an axis is released on the controller.
        /// </summary>
        event OnAxisReleased AxisReleasedEvent;

        /// <summary>
        /// An event invoked after the virtual device is updated with all changes.
        /// </summary>
        event OnControllerUpdated ControllerUpdatedEvent;

        /// <summary>
        /// An event invoked after the controller is reset to its defaults.
        /// </summary>
        event OnControllerReset ControllerResetEvent;

        /// <summary>
        /// An event invoked after the controller has been closed and is no longer available.
        /// </summary>
        event OnControllerClosed ControllerClosedEvent;

        #endregion
    }
}
