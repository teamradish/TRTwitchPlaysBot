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
using System.Diagnostics;

namespace TRBot
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

        //Kimimaru: Ideally we get the input's state from the driver, but this should work well enough, for now at least
        private ConcurrentDictionary<uint, ButtonStates> ButtonPressStates = new ConcurrentDictionary<uint, ButtonStates>(Environment.ProcessorCount * 2, 32);
        private ConcurrentDictionary<uint, ButtonStates> TempBtnStates = new ConcurrentDictionary<uint, ButtonStates>(Environment.ProcessorCount * 2, 32);

        /// <summary>
        /// The built argument list passed into xdotool.
        /// </summary>
        private StringBuilder BuiltArgList = new StringBuilder(512);

        public XDotoolController(in int controllerIndex)
        {
            ControllerIndex = controllerIndex;
        }

        public void Dispose()
        {
            if (IsAcquired == false)
                return;

            //Reset();
            Close();
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

            //Invoke input callbacks
            if (BotProgram.InputCBData.Callbacks.TryGetValue(input.name, out InputCallback cbData) == true)
            {
                long invocation = (long)cbData.InvocationType;

                //Invoke on press
                if (input.hold == false && EnumUtility.HasEnumVal(invocation, (long)InputCBInvocation.Press) == true)
                {
                    cbData.Callback?.Invoke();
                }
                //Invoke on hold
                else if (input.hold == true && EnumUtility.HasEnumVal(invocation,(long)InputCBInvocation.Hold) == true)
                {
                    cbData.Callback?.Invoke();
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

            //Invoke input callbacks
            if (BotProgram.InputCBData.Callbacks.TryGetValue(input.name, out InputCallback cbData) == true)
            {
                //Invoke on release
                if (EnumUtility.HasEnumVal((long)cbData.InvocationType, (long)InputCBInvocation.Release) == true)
                {
                    cbData.Callback?.Invoke();
                }
            }
        }

        public void PressAxis(in int axis, in bool min, in int percent)
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

            if (inputAxis == (int)AxisCodes.MouseX)
            {
                HandleMouseMove(val, 0);
            }
            else
            {
                HandleMouseMove(0, val);
            }
        }

        public void ReleaseAxis(in int axis)
        {
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
            
            TempBtnStates[buttonVal] = ButtonStates.Pressed;
            
            if (ClickMap.TryGetValue(button, out int mouseBtn) == true)
            {
                HandleMouseDown(mouseBtn);
                return;
            }

            //It should be a keyboard key then
            HandleProcessKeyDown(((InputCodes)button).ToString());
        }

        public void ReleaseButton(in uint buttonVal)
        {
            //Not a valid button - defaulting to 0 results in the wrong button being pressed/released
            if (InputCodeMap.TryGetValue((int)buttonVal, out int button) == false)
            {
                return;
            }
            
            TempBtnStates[buttonVal] = ButtonStates.Released;
            
            if (ClickMap.TryGetValue(button, out int mouseBtn) == true)
            {
                HandleMouseUp(mouseBtn);
                return;
            }

            //It should be a keyboard key then
            HandleProcessKeyUp(((InputCodes)button).ToString());
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
            if (BuiltArgList.Length == 0)
            {
                return;
            }
            
            //Copy button states over
            ButtonPressStates.CopyDictionaryData(TempBtnStates);
            
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
                Console.WriteLine("Unable to carry out xdotool inputs: " + e.Message);
            }
            
            BuiltArgList.Clear();
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
