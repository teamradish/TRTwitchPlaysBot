using System;
using System.Collections.Generic;
using System.Text;
using vJoyInterfaceWrap;
using System.Runtime.CompilerServices;
using static vJoyInterfaceWrap.vJoy;

namespace TRBot
{
    public class VJoyControllerManager : IVirtualControllerManager
    {
        /// <summary>
        /// Minimum acceptable vJoy device ID.
        /// </summary>
        public const uint MIN_VJOY_DEVICE_ID = 1;

        /// <summary>
        /// Maximum acceptable vJoy device ID.
        /// </summary>
        public const uint MAX_VJOY_DEVICE_ID = 16;

        private vJoy VJoyInstance = null;
        private VJoyController[] Joysticks = null;

        public bool Initialized => (VJoyInstance != null);

        public int ControllerCount => Joysticks.Length;

        public int MinControllers => (int)MIN_VJOY_DEVICE_ID;

        public int MaxControllers => (int)MAX_VJOY_DEVICE_ID;

        public void Initialize()
        {
            if (Initialized == true) return;

            VJoyInstance = new vJoy();

            if (VJoyInstance.vJoyEnabled() == false)
            {
                Console.WriteLine("vJoy driver not enabled!");
                return;
            }

            //Kimimaru: The data object passed in here will be returned in the callback
            //This can be used to update the object's data when a vJoy device is connected or disconnected from the computer
            //This does not get fired when the vJoy device is acquired or relinquished
            VJoyInstance.RegisterRemovalCB(OnDeviceRemoved, null);

            string vendor = VJoyInstance.GetvJoyManufacturerString();
            string product = VJoyInstance.GetvJoyProductString();
            string serialNum = VJoyInstance.GetvJoySerialNumberString();
            Console.WriteLine($"vJoy driver found - Vendor: {vendor} | Product: {product} | Version: {serialNum}");

            uint dllver = 0;
            uint drvver = 0;
            bool match = VJoyInstance.DriverMatch(ref dllver, ref drvver);

            Console.WriteLine($"Using vJoy DLL version {dllver} and vJoy driver version {drvver} | Version Match: {match}");

            int acquiredCount = InitControllers(BotProgram.BotData.JoystickCount);
            Console.WriteLine($"Acquired {acquiredCount} controllers!");
        }

        public void CleanUp()
        {
            if (Initialized == false)
            {
                Console.WriteLine("VJoyControllerManager not initialized; cannot clean up");
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

            //Check for max vJoy device ID to ensure we don't try to register more devices than it can support
            if (count > MAX_VJOY_DEVICE_ID)
            {
                count = (int)MAX_VJOY_DEVICE_ID;

                Console.WriteLine($"Joystick count of {count} is greater than max {nameof(MAX_VJOY_DEVICE_ID)} of {MAX_VJOY_DEVICE_ID}. Clamping value to this limit.");
            }

            Joysticks = new VJoyController[count];
            for (int i = 0; i < Joysticks.Length; i++)
            {
                Joysticks[i] = new VJoyController((uint)i + MIN_VJOY_DEVICE_ID, VJoyInstance);
            }

            int acquiredCount = 0;

            //Acquire the device IDs
            for (int i = 0; i < Joysticks.Length; i++)
            {
                VJoyController joystick = Joysticks[i];
                VjdStat controlStatus = VJoyInstance.GetVJDStatus(joystick.ControllerID);

                switch (controlStatus)
                {
                    case VjdStat.VJD_STAT_FREE:
                        joystick.Acquire();
                        if (joystick.IsAcquired == false) goto default;

                        acquiredCount++;
                        Console.WriteLine($"Acquired vJoy device ID {joystick.ControllerID}!");

                        //Initialize the joystick
                        joystick.Init();

                        //Reset the joystick
                        joystick.Reset();
                        break;
                    default:
                        Console.WriteLine($"Unable to acquire vJoy device ID {joystick.ControllerID}");
                        break;
                }
            }

            return acquiredCount;
        }

        public IVirtualController GetController(in int controllerPort) => Joysticks[controllerPort];

        private void OnDeviceRemoved(bool removed, bool first, object userData)
        {
            string startMessage = "A vJoy device has been removed!";
            if (removed == false)
            {
                startMessage = "A vJoy device has been added!";
            }

            BotProgram.QueueMessage($"{startMessage} {nameof(first)}: {first} | {nameof(userData)}: {userData}");
        }

        public void CheckDeviceIDState(in uint deviceID)
        {
            VjdStat status = VJoyInstance.GetVJDStatus(deviceID);

            switch (status)
            {
                case VjdStat.VJD_STAT_OWN:
                    Console.WriteLine($"vJoy Device {deviceID} is already owned by this feeder");
                    break;
                case VjdStat.VJD_STAT_FREE:
                    Console.WriteLine($"vJoy Device {deviceID} is free!");
                    break;
                case VjdStat.VJD_STAT_BUSY:
                    Console.WriteLine($"vJoy Device {deviceID} is already owned by another feeder - cannot continue");
                    break;
                case VjdStat.VJD_STAT_MISS:
                    Console.WriteLine($"vJoy Device {deviceID} is not installed or disabled - cannot continue");
                    break;
                default:
                    Console.WriteLine($"vJoy Device {deviceID} general error - cannot continue");
                    break;
            }
        }

        public void CheckButtonCount(in uint deviceID)
        {
            int nBtn = VJoyInstance.GetVJDButtonNumber(deviceID);
            int nDPov = VJoyInstance.GetVJDDiscPovNumber(deviceID);
            int nCPov = VJoyInstance.GetVJDContPovNumber(deviceID);
            bool hasX = VJoyInstance.GetVJDAxisExist(deviceID, HID_USAGES.HID_USAGE_X);
            bool hasY = VJoyInstance.GetVJDAxisExist(deviceID, HID_USAGES.HID_USAGE_Y);
            bool hasZ = VJoyInstance.GetVJDAxisExist(deviceID, HID_USAGES.HID_USAGE_Z);
            bool hasRX = VJoyInstance.GetVJDAxisExist(deviceID, HID_USAGES.HID_USAGE_RX);
        
            Console.WriteLine($"Device[{deviceID}]: Buttons: {nBtn} | DiscPOVs: {nDPov} | ContPOVs: {nCPov} | Axes - X:{hasX} Y:{hasY} Z: {hasZ} RX: {hasRX}");
        }
    }
}
