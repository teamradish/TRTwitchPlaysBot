/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
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
using System.Collections.Concurrent;
using System.Text;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using TRBot.Utilities;
using static TRBot.VirtualControllers.VirtualControllerDelegates;

namespace TRBot.VirtualControllers
{
    /// <summary>
    /// A dummy virtual controller implementation. Used as a fallback.
    /// </summary>
    public class DummyController : IVirtualController
    {
        /// <summary>
        /// The ID of the controller.
        /// </summary>
        public uint ControllerID => (uint)ControllerIndex;

        public int ControllerIndex { get; private set; } = 0;

        /// <summary>
        /// Tells whether the controller device was successfully acquired.
        /// If this is false, don't use this controller instance to make inputs.
        /// </summary>
        public bool IsAcquired { get; private set; } = false;

        public event OnInputPressed InputPressedEvent = null;
        public event OnInputReleased InputReleasedEvent = null;

        public event OnAxisPressed AxisPressedEvent = null;
        public event OnAxisReleased AxisReleasedEvent = null;

        public event OnButtonPressed ButtonPressedEvent = null;
        public event OnButtonReleased ButtonReleasedEvent = null;

        public event OnControllerUpdated ControllerUpdatedEvent = null;
        public event OnControllerReset ControllerResetEvent = null;

        public event OnControllerClosed ControllerClosedEvent = null;

        //Kimimaru: Ideally we get the input's state from the driver, but this should work well enough, for now at least
        private VControllerInputTracker InputTracker = null;

        public DummyController(in int controllerIndex)
        {
            ControllerIndex = controllerIndex;
        }

        ~DummyController()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            if (IsAcquired == false)
                return;

            Close();

            InputPressedEvent = null;
            InputReleasedEvent = null;
            AxisPressedEvent = null;
            AxisReleasedEvent = null;
            ButtonPressedEvent = null;
            ButtonReleasedEvent = null;
            ControllerUpdatedEvent = null;
            ControllerResetEvent = null;
            ControllerClosedEvent = null;
        }

        public void Acquire()
        {
            IsAcquired = true;
        }

        public void Close()
        {
            IsAcquired = false;

            ControllerClosedEvent?.Invoke();
        }

        public void Init()
        {
            Reset();

            InputTracker = new VControllerInputTracker(this);
        }

        public void Reset()
        {
            if (IsAcquired == false)
                return;

            ControllerResetEvent?.Invoke();
        }

        public void SetInputNamePressed(in string inputName)
        {
            InputPressedEvent?.Invoke(inputName);
        }

        public void SetInputNameReleased(in string inputName)
        {
            InputReleasedEvent?.Invoke(inputName);
        }

        public void PressAxis(in int axis, in double minAxisVal, in double maxAxisVal, in double percent)
        {
            AxisPressedEvent?.Invoke(axis, percent);
        }

        public void ReleaseAxis(in int axis)
        {
            AxisReleasedEvent?.Invoke(axis);
        }

        public void PressAbsoluteAxis(in int axis, in int percent)
        {
            AxisPressedEvent?.Invoke(axis, percent);
        }

        public void ReleaseAbsoluteAxis(in int axis)
        {
            AxisReleasedEvent?.Invoke(axis);
        }

        public void PressButton(in uint buttonVal)
        {
            ButtonPressedEvent?.Invoke(buttonVal);
        }

        public void ReleaseButton(in uint buttonVal)
        {
            ButtonReleasedEvent?.Invoke(buttonVal);
        }

        public ButtonStates GetInputState(in string inputName)
        {
            return InputTracker.GetInputState(inputName);
        }

        public ButtonStates GetButtonState(in uint buttonVal)
        {
            return InputTracker.GetButtonState(buttonVal);
        }

        public double GetAxisState(in int axisVal)
        {
            return InputTracker.GetAxisState(axisVal);
        }

        public void UpdateController()
        {
            ControllerUpdatedEvent?.Invoke();
        }
    }
}
