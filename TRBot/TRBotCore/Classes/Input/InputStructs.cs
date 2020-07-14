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
                return hash;
            }
        }

        public static bool operator==(InputAxis a, InputAxis b)
        {
            return (a.AxisVal == b.AxisVal && a.MinAxisVal == b.MinAxisVal && a.MaxAxisVal == b.MaxAxisVal);
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
