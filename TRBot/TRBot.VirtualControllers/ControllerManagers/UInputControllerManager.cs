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
using System.Text;
using System.Runtime.CompilerServices;
using TRBot.Logging;

namespace TRBot.VirtualControllers
{
    public class UInputControllerManager : IVirtualControllerManager
    {
        private UInputController[] Joysticks = null;

        public bool Initialized { get; private set; } = false;

        public int ControllerCount => Joysticks.Length;

        public int MinControllers { get; private set; } = 1;

        public int MaxControllers { get; private set; } = 16;

        public UInputControllerManager()
        {

        }

        ~UInputControllerManager()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        public void Initialize()
        {
            if (Initialized == true) return;

            //Acquire min and max controller counts
            MinControllers = NativeWrapperUInput.GetMinControllerCount();
            MaxControllers = NativeWrapperUInput.GetMaxControllerCount();

            Initialized = true;
        }

        public void Dispose()
        {
            if (Initialized == false)
            {
                TRBotLogger.Logger.Warning("UInputControllerManager not initialized; cannot clean up");
                return;
            }

            if (Joysticks != null)
            {
                for (int i = 0; i < Joysticks.Length; i++)
                {
                    Joysticks[i]?.Dispose();
                }
            }
        }

        public int InitControllers(in int controllerCount)
        {
            if (Initialized == false) return 0;

            int count = controllerCount;

            //Ensure the number isn't lower than the min controllers supported
            if (count < MinControllers)
            {
                count = MinControllers;
                TRBotLogger.Logger.Information($"Joystick count of {count} is less than {nameof(MinControllers)} of {MinControllers}. Clamping value to this limit.");
            }

            //Check for max uinput device ID to ensure we don't try to register more devices than it can support
            if (count > MaxControllers)
            {
                count = MaxControllers;

                TRBotLogger.Logger.Information($"Joystick count of {count} is greater than {nameof(MaxControllers)} of {MaxControllers}. Clamping value to this limit.");
            }

            Joysticks = new UInputController[count];
            for (int i = 0; i < Joysticks.Length; i++)
            {
                Joysticks[i] = new UInputController(i);
            }

            int acquiredCount = 0;

            //Acquire the device IDs
            for (int i = 0; i < Joysticks.Length; i++)
            {
                UInputController joystick = Joysticks[i];

                joystick.Acquire();
                if (joystick.IsAcquired == false)
                {
                    TRBotLogger.Logger.Error($"Unable to acquire uinput device at index {joystick.ControllerIndex}. Make sure you have permissions set with \"sudo chmod a+rw /dev/uinput\"");
                    continue;
                }

                acquiredCount++;
                TRBotLogger.Logger.Information($"Acquired uinput device ID {joystick.ControllerID} at index {joystick.ControllerIndex} with descriptor {joystick.ControllerDescriptor}!");

                //Initialize the joystick
                joystick.Init();

                //Reset the joystick
                joystick.Reset();
            }

            return acquiredCount;
        }

        public IVirtualController GetController(in int controllerPort) => Joysticks[controllerPort];
    }
}
