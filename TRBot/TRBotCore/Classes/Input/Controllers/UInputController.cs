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
using System.Collections.Concurrent;
using System.Text;
using System.Runtime.CompilerServices;

namespace TRBot
{
    public class UInputController : IVirtualController
    {
        /// <summary>
        /// The value for an invalid controller.
        /// </summary>
        public const int INVALID_CONTROLLER = -1;

        /// <summary>
        /// The button codes that are used on uinput virtual controllers.
        /// </summary>
        private enum ButtonCodes
        {
            BTN_A           = 0x130,
            BTN_B           = 0x131,
            BTN_C           = 0x132,
            BTN_X           = 0x133,
            BTN_Y           = 0x134,
            BTN_Z           = 0x135,
            BTN_TL          = 0x136,
            BTN_TR          = 0x137,
            BTN_TL2         = 0x138,
            BTN_TR2         = 0x139,
            BTN_SELECT      = 0x13a,
            BTN_START       = 0x13b,
            BTN_MODE        = 0x13c,
            BTN_THUMBL      = 0x13d,
            BTN_THUMBR      = 0x13e,
            BTN_DPAD_UP     = 0x220,
            BTN_DPAD_DOWN   = 0x221,
            BTN_DPAD_LEFT   = 0x222,
            BTN_DPAD_RIGHT  = 0x223,
            BTN_0           = 0x100,
            BTN_1           = 0x101,
            BTN_2           = 0x102,
            BTN_3           = 0x103,
            BTN_4           = 0x104,
            BTN_5           = 0x105,
            BTN_6           = 0x106,
            BTN_7           = 0x107,
            BTN_8           = 0x108,
            BTN_9           = 0x109,
            BTN_JOYSTICK    = 0x120,
            BTN_THUMB       = 0x121,
            BTN_THUMB2      = 0x122
        }
        
        /// <summary>
        /// The axis codes that are used on uinput virtual controllers.
        /// </summary>
        private enum AxisCodes
        {
            ABS_X       = 0x00,
            ABS_Y       = 0x01,
            ABS_Z       = 0x02,
            ABS_RX      = 0x03,
            ABS_RY      = 0x04,
            ABS_RZ      = 0x05,
            ABS_HAT0X   = 0x10,
            ABS_HAT0Y   = 0x11,
        }

        /// <summary>
        /// The mapping from button number to button code.
        /// </summary>
        private static readonly Dictionary<int, int> ButtonCodeMap = new Dictionary<int, int>(32)
        {
            { (int)GlobalButtonVals.BTN1,  (int)ButtonCodes.BTN_A },
            { (int)GlobalButtonVals.BTN2,  (int)ButtonCodes.BTN_B },
            { (int)GlobalButtonVals.BTN3,  (int)ButtonCodes.BTN_C },
            { (int)GlobalButtonVals.BTN4,  (int)ButtonCodes.BTN_X },
            { (int)GlobalButtonVals.BTN5,  (int)ButtonCodes.BTN_Y },
            { (int)GlobalButtonVals.BTN6,  (int)ButtonCodes.BTN_Z },
            { (int)GlobalButtonVals.BTN7,  (int)ButtonCodes.BTN_TL },
            { (int)GlobalButtonVals.BTN8,  (int)ButtonCodes.BTN_TR },
            { (int)GlobalButtonVals.BTN9,  (int)ButtonCodes.BTN_TL2 },
            { (int)GlobalButtonVals.BTN10, (int)ButtonCodes.BTN_TR2 },
            { (int)GlobalButtonVals.BTN11, (int)ButtonCodes.BTN_SELECT },
            { (int)GlobalButtonVals.BTN12, (int)ButtonCodes.BTN_START },
            { (int)GlobalButtonVals.BTN13, (int)ButtonCodes.BTN_MODE },
            { (int)GlobalButtonVals.BTN14, (int)ButtonCodes.BTN_THUMBL },
            { (int)GlobalButtonVals.BTN15, (int)ButtonCodes.BTN_THUMBR },
            { (int)GlobalButtonVals.BTN16, (int)ButtonCodes.BTN_DPAD_UP },
            { (int)GlobalButtonVals.BTN17, (int)ButtonCodes.BTN_DPAD_DOWN },
            { (int)GlobalButtonVals.BTN18, (int)ButtonCodes.BTN_DPAD_LEFT },
            { (int)GlobalButtonVals.BTN19, (int)ButtonCodes.BTN_DPAD_RIGHT },
            { (int)GlobalButtonVals.BTN20, (int)ButtonCodes.BTN_0 },
            { (int)GlobalButtonVals.BTN21, (int)ButtonCodes.BTN_1 },
            { (int)GlobalButtonVals.BTN22, (int)ButtonCodes.BTN_2 },
            { (int)GlobalButtonVals.BTN23, (int)ButtonCodes.BTN_3 },
            { (int)GlobalButtonVals.BTN24, (int)ButtonCodes.BTN_4 },
            { (int)GlobalButtonVals.BTN25, (int)ButtonCodes.BTN_5 },
            { (int)GlobalButtonVals.BTN26, (int)ButtonCodes.BTN_6 },
            { (int)GlobalButtonVals.BTN27, (int)ButtonCodes.BTN_7 },
            { (int)GlobalButtonVals.BTN28, (int)ButtonCodes.BTN_8 },
            { (int)GlobalButtonVals.BTN29, (int)ButtonCodes.BTN_9 },
            { (int)GlobalButtonVals.BTN30, (int)ButtonCodes.BTN_JOYSTICK },
            { (int)GlobalButtonVals.BTN31, (int)ButtonCodes.BTN_THUMB },
            { (int)GlobalButtonVals.BTN32, (int)ButtonCodes.BTN_THUMB2 }
        };

        /// <summary>
        /// The mapping from axis number to axis code.
        /// </summary>
        private static readonly Dictionary<int, int> AxisCodeMap = new Dictionary<int, int>(8)
        {
            { (int)GlobalAxisVals.AXIS_X,  (int)AxisCodes.ABS_X },
            { (int)GlobalAxisVals.AXIS_Y,  (int)AxisCodes.ABS_Y },
            { (int)GlobalAxisVals.AXIS_Z,  (int)AxisCodes.ABS_Z },
            { (int)GlobalAxisVals.AXIS_RX, (int)AxisCodes.ABS_RX },
            { (int)GlobalAxisVals.AXIS_RY, (int)AxisCodes.ABS_RY },
            { (int)GlobalAxisVals.AXIS_RZ, (int)AxisCodes.ABS_RZ },
            { (int)GlobalAxisVals.AXIS_M1, (int)AxisCodes.ABS_HAT0X },
            { (int)GlobalAxisVals.AXIS_M2, (int)AxisCodes.ABS_HAT0Y }
        };
    
        /// <summary>
        /// The ID of the controller.
        /// </summary>
        public uint ControllerID { get; private set; } = 0;

        /// <summary>
        /// The native descriptor value for the device.
        /// </summary>
        public int ControllerDescriptor { get; private set; } = 0;

        public int ControllerIndex { get; private set; } = 0;

        /// <summary>
        /// Tells whether the controller device was successfully acquired.
        /// If this is false, don't use this controller instance to make inputs.
        /// </summary>
        public bool IsAcquired { get; private set; } = false;

        private Dictionary<int, (long AxisMin, long AxisMax)> MinMaxAxes = new Dictionary<int, (long, long)>(8);

        //Kimimaru: Ideally we get the input's state from the driver, but this should work well enough, for now at least
        private ConcurrentDictionary<uint, ButtonStates> ButtonPressStates = new ConcurrentDictionary<uint, ButtonStates>(Environment.ProcessorCount * 2, 32);
        private ConcurrentDictionary<uint, ButtonStates> TempBtnStates = new ConcurrentDictionary<uint, ButtonStates>(Environment.ProcessorCount * 2, 32);

        public UInputController(in int controllerIndex)
        {
            ControllerIndex = controllerIndex;
        }

        public void Dispose()
        {
            if (IsAcquired == false)
                return;

            Reset();
            Close();
        }

        public void Acquire()
        {
            ControllerDescriptor = NativeWrapperUInput.CreateVirtualController(ControllerIndex);
            ControllerID = 0;

            //Check for valid controller
            if (ControllerDescriptor > INVALID_CONTROLLER)
            {
                IsAcquired = true;
                ControllerID = (uint)ControllerDescriptor;
            }
        }

        public void Close()
        {
            NativeWrapperUInput.Close(ControllerDescriptor);
            IsAcquired = false;
            
            ControllerDescriptor = INVALID_CONTROLLER;
            ControllerID = 0;
        }

        public void Init()
        {
            Reset();

            //Initialize axes
            //Use the global axes values, which will be converted to uinput ones when needing to carry out the inputs
            GlobalAxisVals[] axes = EnumUtility.GetValues<GlobalAxisVals>.EnumValues;

            int minAxis = NativeWrapperUInput.GetMinAxisValue();
            int maxAxis = NativeWrapperUInput.GetMaxAxisValue();

            for (int i = 0; i < axes.Length; i++)
            {
                MinMaxAxes.Add((int)axes[i], (minAxis, maxAxis));
            }
        }

        public void Reset()
        {
            if (IsAcquired == false)
                return;

            foreach (KeyValuePair<int, (long, long)> val in MinMaxAxes)
            {
                if (val.Key == (int)AxisCodes.ABS_Z || val.Key == (int)AxisCodes.ABS_RZ)
                {
                    ReleaseAbsoluteAxis(val.Key);
                }
                else
                {
                    ReleaseAxis(val.Key);
                }
            }

            //Reset all buttons
            GlobalButtonVals[] buttons = EnumUtility.GetValues<GlobalButtonVals>.EnumValues;

            for (int i = 0; i < buttons.Length; i++)
            {
                ReleaseButton((uint)buttons[i]);
            }

            UpdateController();
        }

        public void PressInput(in Parser.Input input)
        {
            if (InputGlobals.CurrentConsole.IsAbsoluteAxis(input) == true)
            {
                PressAbsoluteAxis(InputGlobals.CurrentConsole.InputAxes[input.name], input.percent);

                //Kimimaru: In the case of L and R buttons on GCN, when the axes are pressed, the buttons should be released
                ReleaseButton(InputGlobals.CurrentConsole.ButtonInputMap[input.name]);
            }
            else if (InputGlobals.CurrentConsole.GetAxis(input, out int axis) == true)
            {
                PressAxis(axis, InputGlobals.CurrentConsole.IsMinAxis(input), input.percent);
            }
            else if (InputGlobals.CurrentConsole.IsButton(input) == true)
            {
                PressButton(InputGlobals.CurrentConsole.ButtonInputMap[input.name]);

                //Kimimaru: In the case of L and R buttons on GCN, when the buttons are pressed, the axes should be released
                if (InputGlobals.CurrentConsole.InputAxes.TryGetValue(input.name, out int value) == true)
                {
                    ReleaseAbsoluteAxis(value);
                }
            }
        }

        public void ReleaseInput(in Parser.Input input)
        {
            if (InputGlobals.CurrentConsole.IsAbsoluteAxis(input) == true)
            {
                ReleaseAbsoluteAxis(InputGlobals.CurrentConsole.InputAxes[input.name]);

                //Kimimaru: In the case of L and R buttons on GCN, when the axes are released, the buttons should be too
                ReleaseButton(InputGlobals.CurrentConsole.ButtonInputMap[input.name]);
            }
            else if (InputGlobals.CurrentConsole.GetAxis(input, out int axis) == true)
            {
                ReleaseAxis(axis);
            }
            else if (InputGlobals.CurrentConsole.IsButton(input) == true)
            {
                ReleaseButton(InputGlobals.CurrentConsole.ButtonInputMap[input.name]);

                //Kimimaru: In the case of L and R buttons on GCN, when the buttons are released, the axes should be too
                if (InputGlobals.CurrentConsole.InputAxes.TryGetValue(input.name, out int value) == true)
                {
                    ReleaseAbsoluteAxis(value);
                }
            }
        }

        public void PressAxis(in int axis, in bool min, in int percent)
        {
            //Not a valid axis - defaulting to 0 results in the wrong axis being set
            if (AxisCodeMap.TryGetValue(axis, out int uinputAxis) == false)
            {
                return;
            }
            
            if (MinMaxAxes.TryGetValue(axis, out (long, long) axisVals) == false)
            {
                return;
            }

            //Neutral is halfway between the min and max axes 
            long half = (axisVals.Item2 - axisVals.Item1) / 2L;
            int mid = (int)(axisVals.Item1 + half);
            int val = 0;

            if (min)
            {
                val = (int)(mid - ((percent / 100f) * half));
            }
            else
            {
                val = (int)(mid + ((percent / 100f) * half));
            }

            SetAxis(uinputAxis, val);
        }

        public void ReleaseAxis(in int axis)
        {
            //Not a valid axis - defaulting to 0 results in the wrong axis being set
            if (AxisCodeMap.TryGetValue(axis, out int uinputAxis) == false)
            {
                return;
            }
            
            if (MinMaxAxes.TryGetValue(axis, out (long, long) axisVals) == false)
            {
                return;
            }

            //Neutral is halfway between the min and max axes
            long half = (axisVals.Item2 - axisVals.Item1) / 2L;
            int val = (int)(axisVals.Item1 + half);

            SetAxis(uinputAxis, val);
        }

        public void PressAbsoluteAxis(in int axis, in int percent)
        {
            //Not a valid axis - defaulting to 0 results in the wrong axis being set
            if (AxisCodeMap.TryGetValue(axis, out int uinputAxis) == false)
            {
                return;
            }
            
            if (MinMaxAxes.TryGetValue(axis, out (long, long) axisVals) == false)
            {
                return;
            }

            int val = (int)(axisVals.Item2 * (percent / 100f));
            
            SetAxis(uinputAxis, val);
        }

        public void ReleaseAbsoluteAxis(in int axis)
        {
            //Not a valid axis - defaulting to 0 results in the wrong axis being set
            if (AxisCodeMap.TryGetValue(axis, out int uinputAxis) == false)
            {
                return;
            }
            
            if (MinMaxAxes.ContainsKey(axis) == false)
            {
                return;
            }

            SetAxis(uinputAxis, 0);
        }

        public void PressButton(in uint buttonVal)
        {
            //Not a valid button - defaulting to 0 results in the wrong button being pressed/released
            if (ButtonCodeMap.TryGetValue((int)buttonVal, out int button) == false)
            {
                return;
            }

            TempBtnStates[buttonVal] = ButtonStates.Pressed;

            NativeWrapperUInput.PressButton(ControllerDescriptor, button);
        }

        public void ReleaseButton(in uint buttonVal)
        {
            //Not a valid button - defaulting to 0 results in the wrong button being pressed/released
            if (ButtonCodeMap.TryGetValue((int)buttonVal, out int button) == false)
            {
                return;
            }

            TempBtnStates[buttonVal] = ButtonStates.Released;

            NativeWrapperUInput.ReleaseButton(ControllerDescriptor, button);
        }

        public ButtonStates GetButtonState(in uint buttonVal)
        {
            if (ButtonPressStates.TryGetValue(buttonVal, out ButtonStates btnState) == true)
            {
                return btnState;
            }
            
            return ButtonStates.Released;
        }

        public void UpdateController()
        {
            //Copy button states over
            ButtonPressStates.CopyDictionaryData(TempBtnStates);
            
            NativeWrapperUInput.UpdateController(ControllerDescriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetAxis(in int axis, in int value)
        {
            NativeWrapperUInput.SetAxis(ControllerDescriptor, axis, value);
        }
    }
}
