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
using System.Runtime.CompilerServices;
using TRBot.Logging;

namespace TRBot.VirtualControllers
{
    public class XDotoolControllerManager : IVirtualControllerManager
    {
        private XDotoolController[] Joysticks = null;

        public bool Initialized { get; private set; } = false;

        public int ControllerCount => Joysticks.Length;

        public int MinControllers => 1;

        public int MaxControllers => 1;

        public XDotoolControllerManager()
        {

        }

        ~XDotoolControllerManager()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        public void Initialize()
        {
            if (Initialized == true) return;

            Initialized = true;
        }

        public void Dispose()
        {
            if (Initialized == false)
            {
                TRBotLogger.Logger.Warning("XDotoolControllerManager not initialized; cannot clean up");
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

            //Check for max device count to ensure we don't try to register more devices than it can support
            if (count > MaxControllers)
            {
                count = MaxControllers;

                TRBotLogger.Logger.Information($"Joystick count of {count} is greater than {nameof(MaxControllers)} of {MaxControllers}. Clamping value to this limit.");
            }

            Joysticks = new XDotoolController[count];
            for (int i = 0; i < Joysticks.Length; i++)
            {
                Joysticks[i] = new XDotoolController(i);
            }

            int acquiredCount = 0;

            //Acquire the device IDs
            for (int i = 0; i < Joysticks.Length; i++)
            {
                XDotoolController joystick = Joysticks[i];

                joystick.Acquire();
                if (joystick.IsAcquired == false)
                {
                    TRBotLogger.Logger.Error($"Unable to acquire xdotool device at index {joystick.ControllerIndex}");
                    continue;
                }

                acquiredCount++;
                TRBotLogger.Logger.Information($"Acquired xdotool device at index {joystick.ControllerIndex}!");

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
