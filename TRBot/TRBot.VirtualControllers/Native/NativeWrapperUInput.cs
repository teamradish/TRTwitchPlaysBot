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
using System.Runtime.InteropServices;

namespace TRBot.VirtualControllers
{
    /// <summary>
    /// Native wrapper for the uinput virtual controller implementation.
    /// </summary>
    public static class NativeWrapperUInput
    {
        private const string LIB_NAME = "SetupVController.so";

        [DllImport(LIB_NAME)]
        private static extern int GetMinControllers();

        [DllImport(LIB_NAME)]
        private static extern int GetMaxControllers();

        [DllImport(LIB_NAME)]
        private static extern int GetMinAxisVal();

        [DllImport(LIB_NAME)]
        private static extern int GetMaxAxisVal();

        [DllImport(LIB_NAME)]
        private static extern void UpdateJoystick(int fd);

        [DllImport(LIB_NAME)]
        private static extern void PressReleaseButton(int fd, int button, int press);

        [DllImport(LIB_NAME)]
        private static extern void PressAxis(int fd, int axis, int value);

        [DllImport(LIB_NAME)]
        private static extern int CreateController(int index);

        [DllImport(LIB_NAME)]
        private static extern void CloseController(int fd);

        /// <summary>
        /// Retrieves the minimum number of controllers allowed.
        /// </summary>
        /// <returns>An integer representing the minimum number of controllers.</returns>
        public static int GetMinControllerCount()
        {
            return GetMinControllers();
        }

        /// <summary>
        /// Retrieves the maximum number of controllers allowed.
        /// </summary>
        /// <returns>An integer representing the maximum number of controllers.</returns>
        public static int GetMaxControllerCount()
        {
            return GetMaxControllers();
        }

        /// <summary>
        /// Retrieves the minimum axis value on a controller.
        /// </summary>
        /// <returns>An integer representing the minimum axis value on a controller.</returns>
        public static int GetMinAxisValue()
        {
            return GetMinAxisVal();
        }

        /// <summary>
        /// Retrieves the maximum axis value on a controller.
        /// </summary>
        /// <returns>An integer representing the maximum axis value on a controller.</returns>
        public static int GetMaxAxisValue()
        {
            return GetMaxAxisVal();
        }

        /// <summary>
        /// Presses a button on the controller.
        /// </summary>
        /// <param name="fd">The controller description value.</param>
        /// <param name="button">The button to press.</param>
        public static void PressButton(in int fd, in int button)
        {
            PressReleaseButton(fd, button, 1);
        }

        /// <summary>
        /// Releases a button on the controller.
        /// </summary>
        /// <param name="fd">The controller description value.</param>
        /// <param name="button">The button to release.</param>
        public static void ReleaseButton(in int fd, in int button)
        {
            PressReleaseButton(fd, button, 0);
        }

        /// <summary>
        /// Sets an axis to a specified value on the controller.
        /// </summary>
        /// <param name="fd">The controller description value.</param>
        /// <param name="axis">The axis to set the value for.</param>
        /// <param name="value">The value to set the axis to.</param>
        public static void SetAxis(in int fd, in int axis, in int value)
        {
            PressAxis(fd, axis, value);
        }

        /// <summary>
        /// Updates the controller driver and applies all input changes.
        /// </summary>
        /// <param name="fd">The controller description value.</param>
        public static void UpdateController(in int fd)
        {
            UpdateJoystick(fd);
        }

        /// <summary>
        /// Creates a new virtual controller at the specified controller index (controller port).
        /// The return value is a controller description number to be used by uinput.
        /// </summary>
        /// <param name="index">The index to create the new virtual controller at.</param>
        /// <returns>An integer representing the controller description value. -1 if one failed to be created.</returns>
        public static int CreateVirtualController(in int index)
        {
            return CreateController(index);
        }

        /// <summary>
        /// Closes and destroys the virtual controller at the specified controller description number.
        /// </summary>
        /// <param name="fd">The controller description value.</param>
        public static void Close(in int fd)
        {
            CloseController(fd);
        }
    }
}
