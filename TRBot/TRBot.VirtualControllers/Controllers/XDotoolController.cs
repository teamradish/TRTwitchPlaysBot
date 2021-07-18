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
using System.Collections.Concurrent;
using System.Text;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using TRBot.Utilities;
using TRBot.Logging;
using static TRBot.VirtualControllers.VirtualControllerDelegates;

namespace TRBot.VirtualControllers
{
    /// <summary>
    /// Utilizes xdotool to perform mouse and keyboard inputs. Linux only.
    /// EXPERIMENTAL
    /// </summary>
    public class XDotoolController : IVirtualController
    {
        private readonly object StrBuilderLockObject = new object();

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
            Return, space,
            q, w, e, r, t, y, u, i, o, p,
            a, s, d, f, g, h, j, k, l,
            z, x, c, v, b, n, m,

            Up, Down, Left, Right
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
            { (int)GlobalButtonVals.BTN14,  (int)InputCodes.t },
            { (int)GlobalButtonVals.BTN15,  (int)InputCodes.y },
            { (int)GlobalButtonVals.BTN16,  (int)InputCodes.u },
            { (int)GlobalButtonVals.BTN17,  (int)InputCodes.i },
            { (int)GlobalButtonVals.BTN18,  (int)InputCodes.o },
            { (int)GlobalButtonVals.BTN19,  (int)InputCodes.p },

            { (int)GlobalButtonVals.BTN20,  (int)InputCodes.a },
            { (int)GlobalButtonVals.BTN21,  (int)InputCodes.s },
            { (int)GlobalButtonVals.BTN22,  (int)InputCodes.d },
            { (int)GlobalButtonVals.BTN23,  (int)InputCodes.f },
            { (int)GlobalButtonVals.BTN24,  (int)InputCodes.g },
            { (int)GlobalButtonVals.BTN25,  (int)InputCodes.h },
            { (int)GlobalButtonVals.BTN26,  (int)InputCodes.j },
            { (int)GlobalButtonVals.BTN27,  (int)InputCodes.k },
            { (int)GlobalButtonVals.BTN28,  (int)InputCodes.l },

            { (int)GlobalButtonVals.BTN29,  (int)InputCodes.z },
            { (int)GlobalButtonVals.BTN30,  (int)InputCodes.x },
            { (int)GlobalButtonVals.BTN31,  (int)InputCodes.c },
            { (int)GlobalButtonVals.BTN32,  (int)InputCodes.v },
            { (int)GlobalButtonVals.BTN33,  (int)InputCodes.b },
            { (int)GlobalButtonVals.BTN34,  (int)InputCodes.n },
            { (int)GlobalButtonVals.BTN35,  (int)InputCodes.m },

            { (int)GlobalButtonVals.BTN36,  (int)InputCodes.Up },
            { (int)GlobalButtonVals.BTN37,  (int)InputCodes.Down },
            { (int)GlobalButtonVals.BTN38,  (int)InputCodes.Left },
            { (int)GlobalButtonVals.BTN39,  (int)InputCodes.Right },
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

        private VControllerInputTracker InputTracker = null;

        /// <summary>
        /// The built argument list passed into xdotool.
        /// </summary>
        private StringBuilder BuiltArgList = new StringBuilder(1024);

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

        public void PressAxis(in int axis, in double minAxisVal, in double maxAxisVal, in double percent)
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

            //TRBotLogger.Logger.Information($"%: {percent} | Min/Max: {minAxisVal}/{maxAxisVal} | pressAmount: {pressAmount} | finalVal: {finalVal}");

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
            //Not a valid button - defaulting to 0 results in the wrong button being pressed/released
            if (InputCodeMap.TryGetValue((int)buttonVal, out int button) == false)
            {
                Console.WriteLine($"Didn't find buttonValue {buttonVal}");
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

        public double GetAxisState(in int axisVal)
        {
            return InputTracker.GetAxisState(axisVal);
        }

        public void UpdateController()
        {
            if (BuiltArgList.Length == 0)
            {
                return;
            }
            
            string argList = string.Empty;

            //Execute all the built up commands at once by passing them as arguments to xdotool
            lock (StrBuilderLockObject)
            {
                argList = BuiltArgList.ToString();
            }
            
            //TRBotLogger.Logger.Information($"BUILT ARG LIST: \"{argList}\"");
            
            //A lot can go wrong when trying to start the process, so catch exceptions
            try
            {
                using (Process p = Process.Start(ProcessName, argList))
                {
                    
                }
            }
            catch (Exception e)
            {
                TRBotLogger.Logger.Error($"Unable to carry out xdotool inputs: {e.Message}");
            }
            
            lock (StrBuilderLockObject)
            {
                BuiltArgList.Clear();
            }

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
            lock (StrBuilderLockObject)
            {
                BuiltArgList.Append(MouseUpArg).Append(mouseBtn).Append(" ");
            }
        }
        
        private void HandleMouseMove(int moveLeft, int moveUp)
        {
            lock (StrBuilderLockObject)
            {
                BuiltArgList.Append(MouseMoveRelArg).Append(moveLeft).Append(" ").Append(moveUp).Append(" ");
            }
        }
        
        private void HandleProcessKeyDown(string key)
        {
            lock (StrBuilderLockObject)
            {
                BuiltArgList.Append(KeyDownArg).Append(key).Append(" ");
            }
        }
        
        private void HandleProcessKeyUp(string key)
        {
            lock (StrBuilderLockObject)
            {
                BuiltArgList.Append(KeyUpArg).Append(key).Append(" ");
            }
        }
    }
}
