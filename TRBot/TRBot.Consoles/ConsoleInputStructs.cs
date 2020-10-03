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

namespace TRBot.Consoles
{
    /// <summary>
    /// The types of inputs.
    /// This is a bitwise field.
    /// </summary>
    [Flags]
    public enum InputTypes
    {
        None = 0,
        Button = 1 << 0,
        Axis = 1 << 1
    }

    /// <summary>
    /// Represents input data.
    /// </summary>
    public class InputData
    {
        public string Name { get; set; } = string.Empty;
        public int ButtonValue { get; set; } = 0;
        public int AxisValue { get; set; } = 0;
        public int InputType { get; set; } = 0;
        public int MinAxisVal { get; set; } = 0;
        public int MaxAxisVal { get; set; } = 1;
        public int MaxAxisPercent { get; set; } = 100;

        public InputData()
        {

        }

        public InputData(string name, in int buttonValue, in int axisValue, in int inputType,
            in int minAxisVal, in int maxAxisVal, in int maxAxisPercent)
        {
            Name = name;
            ButtonValue = buttonValue;
            AxisValue = axisValue;
            InputType = inputType;
            MinAxisVal = minAxisVal;
            MaxAxisVal = maxAxisVal;
            MaxAxisPercent = maxAxisPercent;
        }

        public static InputData CreateBlank(string name)
        {
            InputData blankInput = new InputData();
            blankInput.Name = name;
            blankInput.InputType = (int)InputTypes.None;

            return blankInput;
        }

        public static InputData CreateButton(string name, in int buttonValue)
        {
            InputData btnData = new InputData();
            btnData.Name = name;
            btnData.ButtonValue = buttonValue;
            btnData.InputType = (int)InputTypes.Button;

            return btnData;
        }

        public static InputData CreateAxis(string name, in int axisValue, in int minAxisVal, in int maxAxisVal)
        {
            InputData btnData = new InputData();
            btnData.Name = name;
            btnData.AxisValue = axisValue;
            btnData.InputType = (int)InputTypes.Axis;
            btnData.MinAxisVal = minAxisVal;
            btnData.MaxAxisVal = maxAxisVal;
            btnData.MaxAxisPercent = 100;

            return btnData;
        }

        public static InputData CreateAxis(string name, in int axisValue, in int minAxisVal, in int maxAxisVal, in int maxAxisPercent)
        {
            InputData btnData = new InputData();
            btnData.Name = name;
            btnData.AxisValue = axisValue;
            btnData.InputType = (int)InputTypes.Axis;
            btnData.MinAxisVal = minAxisVal;
            btnData.MaxAxisVal = maxAxisVal;
            btnData.MaxAxisPercent = maxAxisPercent;

            return btnData;
        }
    }

    /// <summary>
    /// Represents an input axis. Min and max axis values are normalized in the range -1.0 to 1.0.
    /// </summary>
    public struct InputAxis
    {
        /// <summary>
        /// The value of the axis.
        /// </summary>
        public int AxisVal;

        /// <summary>
        /// The minimum value of the axis, normalized.
        /// </summary>
        public double MinAxisVal;

        /// <summary>
        /// The maximum value of the axis, normalized.
        /// </summary>
        public double MaxAxisVal;

        /// <summary>
        /// The maximum percent this axis can be pressed. If pressed above this amount, it's considered a button press.
        /// <para>This is useful only for axes and buttons with shared names. 
        /// For example, the GameCube's "L" and "R" inputs function both as axes and buttons.</para>
        /// </summary>
        public int MaxPercentPressed;

        /// <summary>
        /// Constructs an input axis.
        /// </summary>
        /// <param name="axisVal">The value of the input axis.</param>
        /// <param name="minAxisVal">The normalized minimum value of the axis. This is clamped from -1 to 1.</param>
        /// <param name="maxAxisVal">The normalized maximum value of the axis. This is clamped from -1 to 1.</param>
        public InputAxis(in int axisVal, in double minAxisVal, in double maxAxisVal)
        {
            AxisVal = axisVal;
            MinAxisVal = Math.Clamp(minAxisVal, -1d, 1d);
            MaxAxisVal = Math.Clamp(maxAxisVal, -1d, 1d);
            MaxPercentPressed = 100;
        }

        /// <summary>
        /// Constructs an input axis.
        /// </summary>
        /// <param name="axisVal">The value of the input axis.</param>
        /// <param name="minAxisVal">The normalized minimum value of the axis. This is clamped from -1 to 1.</param>
        /// <param name="maxAxisVal">The normalized maximum value of the axis. This is clamped from -1 to 1.</param>
        /// <param name="maxPercentPressed">The maximum percent the axis can be pressed. This is clamped from 0 to 100.</param>
        public InputAxis(in int axisVal, in double minAxisVal, in double maxAxisVal, in int maxPercentPressed)
        {
            AxisVal = axisVal;
            MinAxisVal = Math.Clamp(minAxisVal, -1d, 1d);
            MaxAxisVal = Math.Clamp(maxAxisVal, -1d, 1d);
            MaxPercentPressed = Math.Clamp(maxPercentPressed, 0, 100);
        }

        public override bool Equals(object obj)
        {
            if (obj is InputAxis inputAxis)
            {
                return (this == inputAxis);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 23) + AxisVal.GetHashCode();
                hash = (hash * 23) + MinAxisVal.GetHashCode();
                hash = (hash * 23) + MaxAxisVal.GetHashCode();
                hash = (hash * 23) + MaxPercentPressed.GetHashCode();
                return hash;
            }
        }

        public static bool operator==(InputAxis a, InputAxis b)
        {
            return (a.AxisVal == b.AxisVal && a.MinAxisVal == b.MinAxisVal && a.MaxAxisVal == b.MaxAxisVal
                && a.MaxPercentPressed == b.MaxPercentPressed);
        }

        public static bool operator!=(InputAxis a, InputAxis b)
        {
            return !(a == b);
        }
    }

    /// <summary>
    /// Represents an input button.
    /// </summary>
    public struct InputButton
    {
        /// <summary>
        /// The value of the button.
        /// </summary>
        public uint ButtonVal;

        public InputButton(in uint buttonVal)
        {
            ButtonVal = buttonVal;
        }

        public override bool Equals(object obj)
        {
            if (obj is InputButton inputBtn)
            {
                return (this == inputBtn);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 17) + ButtonVal.GetHashCode();
                return hash;
            }
        }

        public static bool operator==(InputButton a, InputButton b)
        {
            return (a.ButtonVal == b.ButtonVal);
        }

        public static bool operator!=(InputButton a, InputButton b)
        {
            return !(a == b);
        }
    }
}
