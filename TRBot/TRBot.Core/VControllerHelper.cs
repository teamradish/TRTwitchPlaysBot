using System;
using System.Collections.Generic;
using TRBot.Parsing;
using TRBot.Connection;
using TRBot.Consoles;
using TRBot.VirtualControllers;
using TRBot.Data;
using TRBot.Utilities;

namespace TRBot.Core
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
        public static VirtualControllerTypes GetDefaultVControllerTypeForPlatform(in OSPlatform.OS operatingSystem)
        {
            switch (operatingSystem)
            {
                case OSPlatform.OS.Windows: return VirtualControllerTypes.vJoy;
                case OSPlatform.OS.GNULinux: return VirtualControllerTypes.uinput;
                default: return VirtualControllerTypes.Invalid;
            }
        }

        /// <summary>
        /// Returns the default virtual controller manager of a given operating system. 
        /// </summary>
        /// <param name="operatingSystem">The operating system to get the default virtual controller manager for.</param>
        /// <returns>The default virtual controller manager of the given operating system.</returns>
        public static IVirtualControllerManager GetDefaultVControllerMngrForPlatform(in OSPlatform.OS operatingSystem)
        {
            switch (operatingSystem)
            {
                case OSPlatform.OS.Windows: return new VJoyControllerManager();
                case OSPlatform.OS.GNULinux: return new UInputControllerManager();
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
        public static bool IsVControllerSupported(in VirtualControllerTypes vControllerType, in OSPlatform.OS operatingSystem)
        {
            switch (vControllerType)
            {
                case VirtualControllerTypes.vJoy:
                    return operatingSystem == OSPlatform.OS.Windows;
                case VirtualControllerTypes.uinput:
                    return operatingSystem == OSPlatform.OS.GNULinux;
                case VirtualControllerTypes.xdotool:
                    return operatingSystem == OSPlatform.OS.GNULinux;
                default:
                    return false;
            }
        }
    }
}
