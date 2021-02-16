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
using System.Text;
using System.Runtime.CompilerServices;
using TRBot.Logging;

namespace TRBot.VirtualControllers
{
    public class DummyControllerManager : IVirtualControllerManager
    {
        private DummyController[] Joysticks = null;

        public bool Initialized { get; private set; } = false;

        public int ControllerCount => Joysticks.Length;

        public int MinControllers { get; private set; } = 1;

        public int MaxControllers { get; private set; } = 16;

        public DummyControllerManager()
        {

        }

        ~DummyControllerManager()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        public void Initialize()
        {
            if (Initialized == true) return;

            MinControllers = 1;
            MaxControllers = 4;

            Initialized = true;
        }

        public void Dispose()
        {
            if (Initialized == false)
            {
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

            //Ensure count of min controllers
            if (count < MinControllers)
            {
                count = MinControllers;
                TRBotLogger.Logger.Information($"Joystick count of {count} is less than {nameof(MinControllers)}. Clamping value to this limit.");
            }

            //Check for max controller ID
            if (count > MaxControllers)
            {
                count = MaxControllers;

                TRBotLogger.Logger.Information($"Joystick count of {count} is greater than max {nameof(MaxControllers)} of {MaxControllers}. Clamping value to this limit.");
            }

            Joysticks = new DummyController[count];
            for (int i = 0; i < Joysticks.Length; i++)
            {
                Joysticks[i] = new DummyController(i);
            }

            int acquiredCount = 0;

            //Acquire the device IDs
            for (int i = 0; i < Joysticks.Length; i++)
            {
                DummyController joystick = Joysticks[i];

                joystick.Acquire();
                if (joystick.IsAcquired == false)
                {
                    TRBotLogger.Logger.Error($"Unable to acquire dummy device at index {joystick.ControllerIndex}...HUH?!?!");
                    continue;
                }

                acquiredCount++;
                TRBotLogger.Logger.Information($"Acquired dummy device ID {joystick.ControllerID} at index {joystick.ControllerIndex}!");

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
