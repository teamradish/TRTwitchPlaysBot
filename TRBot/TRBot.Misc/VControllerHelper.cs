using System;
using System.Collections.Generic;
using TRBot.Parsing;
using TRBot.Connection;
using TRBot.Consoles;
using TRBot.VirtualControllers;
using TRBot.Utilities;

namespace TRBot.Misc
{
    /// <summary>
    /// Contains helper methods regarding virtual controllers.
    /// </summary>
    public static class VControllerHelper
    {
        /// <summary>
        /// Returns the default type of virtual controller of a given operating system. 
        /// </summary>
        /// <param name="operatingSystem">The operating system to get the default virtual controller type for.</param>
        /// <returns>The default virtual controller type of the given operating system.</returns>
        public static VirtualControllerTypes GetDefaultVControllerTypeForPlatform(in TRBotOSPlatform.OS operatingSystem)
        {
            switch (operatingSystem)
            {
                case TRBotOSPlatform.OS.Windows: return VirtualControllerTypes.vJoy;
                case TRBotOSPlatform.OS.GNULinux: return VirtualControllerTypes.uinput;
                default: return VirtualControllerTypes.Invalid;
            }
        }

        /// <summary>
        /// Returns the default virtual controller manager of a given operating system. 
        /// </summary>
        /// <param name="operatingSystem">The operating system to get the default virtual controller manager for.</param>
        /// <returns>The default virtual controller manager of the given operating system.</returns>
        public static IVirtualControllerManager GetDefaultVControllerMngrForPlatform(in TRBotOSPlatform.OS operatingSystem)
        {
            switch (operatingSystem)
            {
                case TRBotOSPlatform.OS.Windows: return new VJoyControllerManager();
                case TRBotOSPlatform.OS.GNULinux: return new UInputControllerManager();
                default: return null;
            }
        }
        
        /// <summary>
        /// Returns the virtual controller manager of a given virtual controller type. 
        /// </summary>
        /// <param name="vControllerType">The virtual controller type to get the virtual controller manager for.</param>
        /// <returns>The virtual controller manager for the given virtual controller type.</returns>
        public static IVirtualControllerManager GetVControllerMngrForType(in VirtualControllerTypes vControllerType)
        {
            switch (vControllerType)
            {
                case VirtualControllerTypes.vJoy: return new VJoyControllerManager();
                case VirtualControllerTypes.uinput: return new UInputControllerManager();
                case VirtualControllerTypes.xdotool: return new XDotoolControllerManager();
                default: return null;
            }
        }

        /// <summary>
        /// Tells if a given virtual controller type is supported by a given operating system.
        /// </summary>
        /// <param name="vControllerType">The virtual controller type.</param>
        /// <param name="operatingSystem">The operating system.</param>
        /// <returns>true if the given virtual controller type is supported by the given operating system, otherwise false.</returns>
        public static bool IsVControllerSupported(in VirtualControllerTypes vControllerType, in TRBotOSPlatform.OS operatingSystem)
        {
            switch (vControllerType)
            {
                case VirtualControllerTypes.vJoy:
                    return operatingSystem == TRBotOSPlatform.OS.Windows;
                case VirtualControllerTypes.uinput:
                    return operatingSystem == TRBotOSPlatform.OS.GNULinux;
                case VirtualControllerTypes.xdotool:
                    return operatingSystem == TRBotOSPlatform.OS.GNULinux;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Validates a given virtual controller type by switching an unsupported type to a supported one on a given operating system.
        /// </summary>
        /// <param name="lastVControllerType">The virtual controller type.</param>
        /// <param name="operatingSystem">The operating system.</param>
        /// <returns>A valid virtual controller type on the given operating system.</returns>
        public static VirtualControllerTypes ValidateVirtualControllerType(in VirtualControllerTypes lastVControllerType, in TRBotOSPlatform.OS operatingSystem)
        {
            return ValidateVirtualControllerType((long)lastVControllerType, operatingSystem);
        }

        /// <summary>
        /// Validates a given virtual controller type by switching an unsupported type to a supported one on a given operating system.
        /// </summary>
        /// <param name="lastVControllerType">The virtual controller type.</param>
        /// <param name="operatingSystem">The operating system.</param>
        /// <returns>A valid virtual controller type on the given operating system.</returns>
        public static VirtualControllerTypes ValidateVirtualControllerType(in long lastVControllerType, in TRBotOSPlatform.OS operatingSystem)
        {
            VirtualControllerTypes newVControllerType = (VirtualControllerTypes)lastVControllerType;

            //Check if the virtual controller type is supported on this platform
            if (IsVControllerSupported(newVControllerType, operatingSystem) == false)
            {
                //It's not supported, so return a supported value to prevent issues on this platform
                newVControllerType = VControllerHelper.GetDefaultVControllerTypeForPlatform(operatingSystem);
            }

            return newVControllerType;
        }
    }
}
