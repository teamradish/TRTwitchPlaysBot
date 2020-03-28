using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace TRBot
{
    public class XDotoolControllerManager : IVirtualControllerManager
    {
        private XDotoolController[] Joysticks = null;

        public bool Initialized { get; private set; } = false;

        public int ControllerCount => Joysticks.Length;

        public int MinControllers => 1;

        public int MaxControllers => 1;

        public void Initialize()
        {
            if (Initialized == true) return;

            Initialized = true;

            int acquiredCount = InitControllers(BotProgram.BotData.JoystickCount);
            Console.WriteLine($"Acquired {acquiredCount} controllers!");
        }

        public void CleanUp()
        {
            if (Initialized == false)
            {
                Console.WriteLine("XDotoolControllerManager not initialized; cannot clean up");
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

            //Ensure count of 1
            if (count < 1)
            {
                count = 1;
                Console.WriteLine($"Joystick count of {count} is less than 1. Clamping value to this limit.");
            }

            //Check for max device count to ensure we don't try to register more devices than it can support
            if (count > MaxControllers)
            {
                count = MaxControllers;

                Console.WriteLine($"Joystick count of {count} is greater than max {nameof(MaxControllers)} of {MaxControllers}. Clamping value to this limit.");
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
                    Console.WriteLine($"Unable to acquire xdotool device at index {joystick.ControllerIndex}");
                    continue;
                }

                acquiredCount++;
                Console.WriteLine($"Acquired xdotool device at index {joystick.ControllerIndex}!");

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
