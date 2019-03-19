using System;
using System.Collections.Generic;
using System.Text;
using vJoyInterfaceWrap;
using static vJoyInterfaceWrap.vJoy;

namespace KimimaruBot
{
    public class VJoyController : IDisposable
    {
        /// <summary>
        /// Minimum acceptable vJoy device ID.
        /// </summary>
        public const uint MIN_VJOY_DEVICE_ID = 1;

        /// <summary>
        /// Maximum acceptable vJoy device ID.
        /// </summary>
        public const uint MAX_VJOY_DEVICE_ID = 16;

        /// <summary>
        /// Tells whether the vJoy instance is initialized.
        /// </summary>
        public static bool Initialized => (VJoyInstance != null);

        public static vJoy VJoyInstance { get; private set; } = null;
        public static VJoyController Joystick { get; private set; } = null;

        /// <summary>
        /// Holds the button states for the controller, updated by the bot. true means pressed, and false means released.
        /// </summary>
        public readonly Dictionary<string, bool> ButtonStates = new Dictionary<string, bool>();

        /// <summary>
        /// The ID of the controller.
        /// </summary>
        public uint ControllerID { get; private set; } = 0;

        //private JoystickState JSState = default(JoystickState);

        public VJoyController(in uint controllerID)
        {
            ControllerID = controllerID;
        }

        public void Dispose()
        {
            if (Initialized == false)
                return;

            Reset();
            VJoyInstance.RelinquishVJD(ControllerID);
        }

        public void Reset()
        {
            if (Initialized == false)
                return;

            string[] keys = new string[ButtonStates.Keys.Count];
            ButtonStates.Keys.CopyTo(keys, 0);
            for (int i = 0; i < keys.Length; i++)
            {
                ButtonStates[keys[i]] = false;
            }

            VJoyInstance.ResetVJD(ControllerID);
        }

        public void PressAxis(HID_USAGES axis, in bool min, in int percent)
        {
            long val = 0L;
            if (min)
            {
                VJoyInstance.GetVJDAxisMin(ControllerID, axis, ref val);
                VJoyInstance.SetAxis((int)(val * (percent / 100f)), ControllerID, axis);
            }
            else
            {
                VJoyInstance.GetVJDAxisMax(ControllerID, axis, ref val);
                VJoyInstance.SetAxis((int)(val * (percent / 100f)), ControllerID, axis);
            }
        }

        public void PressButton(in string buttonName)
        {
            if (ButtonStates[buttonName] == true) return;

            ButtonStates[buttonName] = true;
            VJoyInstance.SetBtn(true, ControllerID, InputGlobals.INPUTS[buttonName]);
        }

        public void ReleaseButton(in string buttonName)
        {
            if (ButtonStates[buttonName] == false) return;

            ButtonStates[buttonName] = false;
            VJoyInstance.SetBtn(false, ControllerID, InputGlobals.INPUTS[buttonName]);
        }

        public void SetButtons(InputGlobals.InputConsoles console)
        {
            ButtonStates.Clear();

            string[] inputs = InputGlobals.GetValidInputs(console);
            for (int i = 0; i < inputs.Length; i++)
            {
                ButtonStates.Add(inputs[i], false);
            }

            Console.WriteLine($"Set controller {ControllerID} buttons to {console}");

            //Reset the controller when we set buttons
            //If we switch to a different console, this will clear all other inputs, including ones not supported by this console
            Reset();
        }

        public static void Initialize()
        {
            if (Initialized == true) return;

            VJoyInstance = new vJoy();

            if (VJoyInstance.vJoyEnabled() == false)
            {
                Console.WriteLine("vJoy driver not enabled!");
                return;
            }

            Joystick = new VJoyController(1);
            Joystick.SetButtons(InputGlobals.CurrentConsole);

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

                    //Reset the joystick
                    Joystick.Reset();
                    break;
                default:
                    Console.WriteLine($"Unable to acquire vJoy device ID {Joystick.ControllerID}");
                    break;
            }
        }

        public static void CleanUp()
        {
            if (Initialized == false)
            {
                Console.WriteLine("Not initialized; cannot clean up");
                return;
            }

            Joystick?.Dispose();
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
