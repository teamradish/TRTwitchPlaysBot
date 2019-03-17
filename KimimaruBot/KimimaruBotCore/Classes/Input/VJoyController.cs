using System;
using System.Collections.Generic;
using System.Text;
using vJoyInterfaceWrap;
using static vJoyInterfaceWrap.vJoy;

namespace KimimaruBot
{
    public class VJoyController
    {
        /// <summary>
        /// Minimum acceptable vJoy device ID.
        /// </summary>
        public const uint MIN_VJOY_DEVICE_ID = 1;

        /// <summary>
        /// Maximum acceptable vJoy device ID.
        /// </summary>
        public const uint MAX_VJOY_DEVICE_ID = 16;

        public static bool Initialized => (VJoyInstance != null);

        public static vJoy VJoyInstance { get; private set; } = null;
        public static VJoyController Joystick { get; private set; } = null;

        public uint ControllerID { get; private set; } = 0;

        public VJoyController(in uint controllerID)
        {
            ControllerID = controllerID;
        }

        public static void Initialize()
        {
            if (Initialized == true) return;

            VJoyInstance = new vJoy();
            Joystick = new VJoyController(1);

            if (VJoyInstance.vJoyEnabled() == false)
            {
                Console.WriteLine("vJoy driver not enabled!");
                return;
            }

            string vendor = VJoyInstance.GetvJoyManufacturerString();
            string product = VJoyInstance.GetvJoyProductString();
            string serialNum = VJoyInstance.GetvJoySerialNumberString();
            Console.WriteLine($"vJoy driver found - Vendor: {vendor} | Product: {product} | Version: {serialNum}");

            uint dllver = 0;
            uint drvver = 0;
            bool match = VJoyInstance.DriverMatch(ref dllver, ref drvver);

            Console.WriteLine($"Using vJoy DLL version {dllver} and vJoy driver version {drvver} | Version Match: {match}");

            //Acquire the device ID
            VjdStat controllerStatus = VJoyInstance.GetVJDStatus(Joystick.ControllerID);
            switch (controllerStatus)
            {
                case VjdStat.VJD_STAT_FREE:
                    bool acquire = VJoyInstance.AcquireVJD(Joystick.ControllerID);
                    if (acquire == false) goto default;

                    Console.WriteLine($"Acquired vJoy device ID {Joystick.ControllerID}!");
                    break;
                default:
                    Console.WriteLine($"Unable to acquire vJoy device ID {Joystick.ControllerID}");
                    break;
            }
        }

        public static void CheckDeviceIDState(in uint deviceID)
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

        public static void CheckButtonCount(in uint deviceID)
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
