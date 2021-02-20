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
using vJoyInterfaceWrap;
using System.Runtime.CompilerServices;
using TRBot.Logging;
using static vJoyInterfaceWrap.vJoy;

namespace TRBot.VirtualControllers
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

        public VJoyControllerManager()
        {

        }

        ~VJoyControllerManager()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        public void Initialize()
        {
            if (Initialized == true) return;

            VJoyInstance = new vJoy();

            if (VJoyInstance.vJoyEnabled() == false)
            {
                TRBotLogger.Logger.Error("vJoy driver not enabled!");
                return;
            }

            //The data object passed in here will be returned in the callback
            //This can be used to update the object's data when a vJoy device is connected or disconnected from the computer
            //This does not get fired when the vJoy device is acquired or relinquished
            VJoyInstance.RegisterRemovalCB(OnDeviceRemoved, null);

            string vendor = VJoyInstance.GetvJoyManufacturerString();
            string product = VJoyInstance.GetvJoyProductString();
            string serialNum = VJoyInstance.GetvJoySerialNumberString();
            TRBotLogger.Logger.Information($"vJoy driver found - Vendor: {vendor} | Product: {product} | Version: {serialNum}");

            uint dllver = 0;
            uint drvver = 0;
            bool match = VJoyInstance.DriverMatch(ref dllver, ref drvver);

            TRBotLogger.Logger.Information($"Using vJoy DLL version {dllver} and vJoy driver version {drvver} | Version Match: {match}");
        }

        public void Dispose()
        {
            if (Initialized == false)
            {
                TRBotLogger.Logger.Warning("VJoyControllerManager not initialized; cannot clean up");
                return;
            }

            if (Joysticks != null)
            {
                for (int i = 0; i < Joysticks.Length; i++)
                {
                    Joysticks[i]?.Dispose();
                }
            }
            
            VJoyInstance = null;
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

            //Check for max vJoy device ID to ensure we don't try to register more devices than it can support
            if (count > MaxControllers)
            {
                count = MaxControllers;

                TRBotLogger.Logger.Information($"Joystick count of {count} is greater than {nameof(MaxControllers)} of {MaxControllers}. Clamping value to this limit.");
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
                        TRBotLogger.Logger.Information($"Acquired vJoy device ID {joystick.ControllerID}!");

                        //Initialize the joystick
                        joystick.Init();

                        //Reset the joystick
                        joystick.Reset();
                        break;
                    default:
                        TRBotLogger.Logger.Error($"Unable to acquire vJoy device ID {joystick.ControllerID}");
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

            TRBotLogger.Logger.Information($"{startMessage} {nameof(first)}: {first} | {nameof(userData)}: {userData}");
        }

        public void CheckDeviceIDState(in uint deviceID)
        {
            VjdStat status = VJoyInstance.GetVJDStatus(deviceID);

            switch (status)
            {
                case VjdStat.VJD_STAT_OWN:
                    TRBotLogger.Logger.Error($"vJoy Device {deviceID} is already owned by this feeder");
                    break;
                case VjdStat.VJD_STAT_FREE:
                    TRBotLogger.Logger.Information($"vJoy Device {deviceID} is free!");
                    break;
                case VjdStat.VJD_STAT_BUSY:
                    TRBotLogger.Logger.Error($"vJoy Device {deviceID} is already owned by another feeder - cannot continue");
                    break;
                case VjdStat.VJD_STAT_MISS:
                    TRBotLogger.Logger.Error($"vJoy Device {deviceID} is not installed or disabled - cannot continue");
                    break;
                default:
                    TRBotLogger.Logger.Error($"vJoy Device {deviceID} general error - cannot continue");
                    break;
            }
        }
    }
}
