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
using System.Collections.Concurrent;
using System.Text;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using TRBot.Utilities;
using static TRBot.VirtualControllers.VirtualControllerDelegates;

namespace TRBot.VirtualControllers
{
    /// <summary>
    /// Utilizes xdotool to perform mouse and keyboard inputs. Linux only.
    /// EXPERIMENTAL
    /// </summary>
    public class XDotoolController : IVirtualController
    {
        private const string ProcessName = "xdotool";
        private const string MouseMoveRelArg = "mousemove_relative -- ";
        private const string MouseDownArg = "mousedown ";
        private const string MouseUpArg = "mouseup ";
        private const string KeyDownArg = "keydown ";
        private const string KeyUpArg = "keyup ";
        
        /// <summary>
        /// The input codes that are used on xdotool virtual controllers.
        /// </summary>
        private enum InputCodes
        {
            MLeft, MRight, MUp, MDown,
            LClick, RClick, MClick,
            
            //Keyboard
            Return, space, q, w, e, r, a, s, d, p
        }
        
        /// <summary>
        /// The axis codes that are used on xdotool virtual controllers.
        /// </summary>
        private enum AxisCodes
        {
            MouseX, MouseY
        }

        /// <summary>
        /// The mapping from button number to input code.
        /// </summary>
        private static readonly Dictionary<int, int> InputCodeMap = new Dictionary<int, int>(32)
        {
            { (int)GlobalButtonVals.BTN1,   (int)InputCodes.MLeft },
            { (int)GlobalButtonVals.BTN2,   (int)InputCodes.MRight },
            { (int)GlobalButtonVals.BTN3,   (int)InputCodes.MUp },
            { (int)GlobalButtonVals.BTN4,   (int)InputCodes.MDown },
            { (int)GlobalButtonVals.BTN5,   (int)InputCodes.LClick },
            { (int)GlobalButtonVals.BTN6,   (int)InputCodes.MClick },
            { (int)GlobalButtonVals.BTN7,   (int)InputCodes.RClick },
            { (int)GlobalButtonVals.BTN8,   (int)InputCodes.Return },
            { (int)GlobalButtonVals.BTN9,   (int)InputCodes.space },
            { (int)GlobalButtonVals.BTN10,  (int)InputCodes.q },
            { (int)GlobalButtonVals.BTN11,  (int)InputCodes.w },
            { (int)GlobalButtonVals.BTN12,  (int)InputCodes.e },
            { (int)GlobalButtonVals.BTN13,  (int)InputCodes.r },
            { (int)GlobalButtonVals.BTN14,  (int)InputCodes.a },
            { (int)GlobalButtonVals.BTN15,  (int)InputCodes.s },
            { (int)GlobalButtonVals.BTN16,  (int)InputCodes.d },
            { (int)GlobalButtonVals.BTN17,  (int)InputCodes.p }
        };

        /// <summary>
        /// The mapping from axis number to axis code.
        /// </summary>
        private static readonly Dictionary<int, int> AxisCodeMap = new Dictionary<int, int>(8)
        {
            { (int)GlobalAxisVals.AXIS_X,  (int)AxisCodes.MouseX },
            { (int)GlobalAxisVals.AXIS_Y,  (int)AxisCodes.MouseY }
        };
    
        private static readonly Dictionary<int, int> ClickMap = new Dictionary<int, int>(3)
        {
            { (int)InputCodes.LClick, 1 },
            { (int)InputCodes.MClick, 2 },
            { (int)InputCodes.RClick, 3 }
        };
    
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

        private Dictionary<int, (long AxisMin, long AxisMax)> MinMaxAxes = new Dictionary<int, (long, long)>(2);

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

        /// <summary>
        /// The built argument list passed into xdotool.
        /// </summary>
        private StringBuilder BuiltArgList = new StringBuilder(512);

        public XDotoolController(in int controllerIndex)
        {
            ControllerIndex = controllerIndex;
        }

        ~XDotoolController()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            if (IsAcquired == false)
                return;

            //Reset();
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
            BuiltArgList.Clear();
            IsAcquired = true;
        }

        public void Close()
        {
            BuiltArgList.Clear();
            IsAcquired = false;

            ControllerClosedEvent?.Invoke();
        }

        public void Init()
        {
            //Reset();

            //Initialize axes
            //Use the global axes values, which will be converted to xdotool values when needing to carry out the inputs
            GlobalAxisVals[] axes = EnumUtility.GetValues<GlobalAxisVals>.EnumValues;

            for (int i = 0; i < axes.Length; i++)
            {
                MinMaxAxes.Add((int)axes[i], (-500, 500));
            }

            InputTracker = new VControllerInputTracker(this);
        }

        public void Reset()
        {
            if (IsAcquired == false)
                return;

            foreach (KeyValuePair<int, (long, long)> val in MinMaxAxes)
            {
                ReleaseAxis(val.Key);
            }

            //Reset all buttons
            GlobalButtonVals[] buttons = EnumUtility.GetValues<GlobalButtonVals>.EnumValues;

            for (int i = 0; i < buttons.Length; i++)
            {
                ReleaseButton((uint)buttons[i]);
            }

            UpdateController();

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

        /*public void PressInput(in Parser.Input input)
        {
            ConsoleBase curConsole = InputGlobals.CurrentConsole;

            if (curConsole.IsWait(input) == true)
            {
                return;
            }

            if (curConsole.GetAxis(input, out InputAxis axis) == true)
            {
                PressAxis(axis.AxisVal, axis.MinAxisVal, axis.MaxAxisVal, input.percent);

                //Release a button with the same name (Ex. L/R buttons on GCN)
                if (curConsole.ButtonInputMap.TryGetValue(input.name, out InputButton btnVal) == true)
                {
                    ReleaseButton(btnVal.ButtonVal);
                }
            }
            else if (curConsole.IsButton(input) == true)
            {
                PressButton(curConsole.ButtonInputMap[input.name].ButtonVal);

                //Release an axis with the same name (Ex. L/R buttons on GCN)
                if (curConsole.InputAxes.TryGetValue(input.name, out InputAxis value) == true)
                {
                    ReleaseAxis(value.AxisVal);
                }
            }

            InputPressedEvent?.Invoke(input);
        }

        public void ReleaseInput(in Parser.Input input)
        {
            ConsoleBase curConsole = InputGlobals.CurrentConsole;

            if (curConsole.IsWait(input) == true)
            {
                return;
            }

            if (curConsole.GetAxis(input, out InputAxis axis) == true)
            {
                ReleaseAxis(axis.AxisVal);

                //Release a button with the same name (Ex. L/R buttons on GCN)
                if (curConsole.ButtonInputMap.TryGetValue(input.name, out InputButton btnVal) == true)
                {
                    ReleaseButton(btnVal.ButtonVal);
                }
            }
            else if (curConsole.IsButton(input) == true)
            {
                ReleaseButton(curConsole.ButtonInputMap[input.name].ButtonVal);

                //Release an axis with the same name (Ex. L/R buttons on GCN)
                if (curConsole.InputAxes.TryGetValue(input.name, out InputAxis value) == true)
                {
                    ReleaseAxis(value.AxisVal);
                }
            }

            InputReleasedEvent?.Invoke(input);
        }*/

        public void PressAxis(in int axis, in double minAxisVal, in double maxAxisVal, in int percent)
        {
            //Not a valid axis - defaulting to 0 results in the wrong axis being set
            if (AxisCodeMap.TryGetValue(axis, out int inputAxis) == false)
            {
                return;
            }
            
            if (MinMaxAxes.TryGetValue(axis, out (long, long) axisVals) == false)
            {
                return;
            }

            //Get the pressed amount between the min and max values for the axis
            double pressAmount = Helpers.Lerp(minAxisVal, maxAxisVal, (percent / 100d));

            //Map the pressed amount to the virtual controller's range
            //Ex. 0 to 1 min/max for axis
            //0 to 32767 for virtual controller
            //50% results in 0.5, which maps to 16383 (truncated)
            int finalVal = (int)Helpers.RemapNum(pressAmount, minAxisVal, maxAxisVal,
                minAxisVal * axisVals.Item1, maxAxisVal * axisVals.Item2);

            //Console.WriteLine($"%: {percent} | Min/Max: {minAxisVal}/{maxAxisVal} | pressAmount: {pressAmount} | finalVal: {finalVal}");

            if (inputAxis == (int)AxisCodes.MouseX)
            {
                HandleMouseMove(finalVal, 0);
            }
            else
            {
                HandleMouseMove(0, finalVal);
            }

            AxisPressedEvent?.Invoke(axis, percent);
        }

        public void ReleaseAxis(in int axis)
        {
            AxisReleasedEvent?.Invoke(axis);

            //Not a valid axis - defaulting to 0 results in the wrong axis being set
            //if (AxisCodeMap.TryGetValue(axis, out int xdotoolAxis) == false)
            //{
            //    return;
            //}
            //
            //if (MinMaxAxes.TryGetValue(axis, out (long, long) axisVals) == false)
            //{
            //    return;
            //}
            //
            //Neutral is halfway between the min and max axes
            //long half = (axisVals.Item2 - axisVals.Item1) / 2L;
            //int val = (int)(axisVals.Item1 + half);
            //
            //SetAxis(uinputAxis, val);
        }

        public void PressAbsoluteAxis(in int axis, in int percent)
        {
            AxisPressedEvent?.Invoke(axis, percent);

            ////Not a valid axis - defaulting to 0 results in the wrong axis being set
            //if (AxisCodeMap.TryGetValue(axis, out int xdotoolAxis) == false)
            //{
            //    return;
            //}
            //
            //if (MinMaxAxes.TryGetValue(axis, out (long, long) axisVals) == false)
            //{
            //    return;
            //}
            //
            //int val = (int)(axisVals.Item2 * (percent / 100f));
            //
            //SetAxis(uinputAxis, val);
        }

        public void ReleaseAbsoluteAxis(in int axis)
        {
            AxisReleasedEvent?.Invoke(axis);

            ////Not a valid axis - defaulting to 0 results in the wrong axis being set
            //if (AxisCodeMap.TryGetValue(axis, out int xdotoolAxis) == false)
            //{
            //    return;
            //}
            //
            //if (MinMaxAxes.ContainsKey(axis) == false)
            //{
            //    return;
            //}
            //
            //SetAxis(uinputAxis, 0);
        }

        public void PressButton(in uint buttonVal)
        {
            //Not a valid button - defaulting to 0 results in the wrong button being pressed/released
            if (InputCodeMap.TryGetValue((int)buttonVal, out int button) == false)
            {
                return;
            }
            
            if (ClickMap.TryGetValue(button, out int mouseBtn) == true)
            {
                HandleMouseDown(mouseBtn);
            }
            else
            {
                //It should be a keyboard key then
                HandleProcessKeyDown(((InputCodes)button).ToString());
            }

            ButtonPressedEvent?.Invoke(buttonVal);
        }

        public void ReleaseButton(in uint buttonVal)
        {
            //Not a valid button - defaulting to 0 results in the wrong button being pressed/released
            if (InputCodeMap.TryGetValue((int)buttonVal, out int button) == false)
            {
                return;
            }
            
            if (ClickMap.TryGetValue(button, out int mouseBtn) == true)
            {
                HandleMouseUp(mouseBtn);
            }
            else
            {
                //It should be a keyboard key then
                HandleProcessKeyUp(((InputCodes)button).ToString());
            }

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

        public int GetAxisState(in int axisVal)
        {
            return InputTracker.GetAxisState(axisVal);
        }

        public void UpdateController()
        {
            if (BuiltArgList.Length == 0)
            {
                return;
            }
            
            //Execute all the built up commands at once by passing them as arguments to xdotool
            string argList = BuiltArgList.ToString();
            
            //Console.WriteLine($"BUILT ARG LIST: \"{argList}\"");
            
            //A lot can go wrong when trying to start the process, so catch exceptions
            try
            {
                using (Process p = Process.Start(ProcessName, argList))
                {
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to carry out xdotool inputs: {e.Message}");
            }
            
            BuiltArgList.Clear();

            ControllerUpdatedEvent?.Invoke();
        }
        
        //Kimimaru: We need to find a way to smoothly move the mouse over time so durations work
        
        private void HandleMouseDown(int mouseBtn)
        {
            BuiltArgList.Append(MouseDownArg).Append(mouseBtn).Append(" ");
            //Process.Start(ProcessName, MouseDownArg + mouseBtn.ToString());
        }
        
        private void HandleMouseUp(int mouseBtn)
        {
            BuiltArgList.Append(MouseUpArg).Append(mouseBtn).Append(" ");
            //Process.Start(ProcessName, MouseUpArg + mouseBtn.ToString());
        }
        
        private void HandleMouseMove(int moveLeft, int moveUp)
        {
            BuiltArgList.Append(MouseMoveRelArg).Append(moveLeft).Append(" ").Append(moveUp).Append(" ");
            //Process.Start(ProcessName, MouseMoveRelArg + moveLeft.ToString() + " " + moveUp.ToString());
        }
        
        private void HandleProcessKeyDown(string key)
        {
            BuiltArgList.Append(KeyDownArg).Append(key).Append(" ");
            //Process.Start(ProcessName, KeyDownArg + key);
        }
        
        private void HandleProcessKeyUp(string key)
        {
            BuiltArgList.Append(KeyUpArg).Append(key).Append(" ");
            //Process.Start(ProcessName, KeyUpArg + key);
        }
    }
}
