/* This file is part of TRBot.
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

namespace TRBot
{
    /// <summary>
    /// Defines inputs.
    /// </summary>
    public static class InputGlobals
    {
        static InputGlobals()
        {
            ///Kimimaru: Set a console so we have one to start out with
            ///We do this to avoid dictionary lookups each time using the <see cref="CurrentConsoleVal"/> to achieve better performance during inputs
            SetConsole((InputConsoles)BotProgram.BotData.LastConsole, null);
        }

        /// <summary>
        /// The types of virtual controllers.
        /// </summary>
        public enum VControllerTypes
        {
            vJoy,
            uinput,
            xdotool
        }

        /// <summary>
        /// The consoles that inputs are supported for.
        /// </summary>
        public enum InputConsoles
        {
            NES,
            SNES, Genesis,
            N64,
            GC, PS2, GBA,
            Wii,
            PC,
            Custom
        }

        public static readonly Dictionary<string, string> INPUT_SYNONYMS = new Dictionary<string, string>()
        {
            //{ "pause", "start" }
            { "kappa", "#" }
        };

        public static readonly Dictionary<InputConsoles, ConsoleBase> Consoles = new Dictionary<InputConsoles, ConsoleBase>()
        {
            { InputConsoles.NES, new NESConsole() },
            { InputConsoles.SNES, new SNESConsole() }, { InputConsoles.Genesis, new GenesisConsole() },
            { InputConsoles.N64, new N64Console() },
            { InputConsoles.GC, new GCConsole() }, { InputConsoles.PS2, new PS2Console() }, { InputConsoles.GBA, new GBAConsole() },
            { InputConsoles.Wii, new WiiConsole() },
            { InputConsoles.PC, new PCConsole() },
            { InputConsoles.Custom, new CustomConsole() }
        };

        /// <summary>
        /// The console value associated with the current console inputs are being sent for.
        /// </summary>
        public static InputConsoles CurrentConsoleVal { get; private set; } = InputConsoles.NES;

        /// <summary>
        /// The current console inputs are being sent for.
        /// </summary>
        public static ConsoleBase CurrentConsole { get; private set; } = null;

        /// <summary>
        /// The current virtual controller type being used.
        /// </summary>
        public static VControllerTypes CurVControllerType { get; private set; } = GetDefaultSupportedVControllerType();

        /// <summary>
        /// The current virtual controller manager.
        /// </summary>
        public static IVirtualControllerManager ControllerMngr { get; private set; } = null;

        /// <summary>
        /// Sets the console for inputs to be sent for.
        /// </summary>
        /// <param name="consoleVal">The console value.</param>
        /// <param name="consoleArgs">The arguments to send to the console.</param>
        public static void SetConsole(in InputConsoles consoleVal, List<string> consoleArgs)
        {
            if (Consoles.TryGetValue(consoleVal, out ConsoleBase consoleBase) == true)
            {
                CurrentConsoleVal = consoleVal;
                CurrentConsole = consoleBase;
                BotProgram.BotData.LastConsole = (int)CurrentConsoleVal;
                BotProgram.SaveBotData();

                CurrentConsole.HandleArgsOnConsoleChange(consoleArgs);
            }
            else
            {
                Console.WriteLine($"Console {consoleVal} not supported!");
            }
        }

        /// <summary>
        /// Sets the virtual controller manager to a different type utilizing a different set of virtual controllers.
        /// </summary>
        /// <param name="vControllerType">The type of virtual controllers to use.</param>
        public static void SetVirtualController(VControllerTypes vControllerType)
        {
            if (IsVControllerSupported(vControllerType) == false)
            {
                Console.WriteLine($"Virtual controller type {vControllerType} is not supported on this platform.");
                return;
            }
            
            if (vControllerType == CurVControllerType && ControllerMngr != null)
            {
                Console.WriteLine($"Virtual controller {vControllerType} is already in use!");
                return;
            }
            
            CurVControllerType = vControllerType;
            BotProgram.BotData.LastVControllerType = (int)CurVControllerType;
            BotProgram.SaveBotData();
            
            //Clean up and reinitialize the controller manager
            ControllerMngr?.CleanUp();
            
            switch (vControllerType)
            {
                case VControllerTypes.vJoy:
                ControllerMngr = new VJoyControllerManager();
                break;
                case VControllerTypes.uinput:
                ControllerMngr = new UInputControllerManager();
                break;
                case VControllerTypes.xdotool:
                ControllerMngr = new XDotoolControllerManager();
                break;
            }
            
            //We should have a valid controller manager at this point
            //If we don't, something is wrong
            ControllerMngr.Initialize();
        }
        
        /// <summary>
        /// Tells whether a type of virtual controller is supported on the current platform.
        /// </summary>
        /// <param name="vControllerType">The type of virtual controller.</param>
        /// <returns>A bool indicating the type of virtual controller supported.</returns>
        public static bool IsVControllerSupported(in VControllerTypes vControllerType)
        {
            switch (vControllerType)
            {
                #if WINDOWS
                case VControllerTypes.vJoy: return true;
                #else
                case VControllerTypes.uinput: return true;
                case VControllerTypes.xdotool: return true;
                #endif
                default: return false;
            }
        }
        
        /// <summary>
        /// Returns the default virtual controller type supported on the current platform.
        /// </summary>
        /// <returns>The default virtual controller type supported on the current platform.</returns>
        public static VControllerTypes GetDefaultSupportedVControllerType()
        {
            #if WINDOWS
                return VControllerTypes.vJoy;
            #else
                return VControllerTypes.uinput;
            #endif
        }

        /// <summary>
        /// Returns the valid input names of the current console.
        /// </summary>
        public static string[] ValidInputs => CurrentConsole.ValidInputs;

        /// <summary>
        /// Returns the input regex string for the current console.
        /// </summary>
        public static string ValidInputRegexStr => CurrentConsole.InputRegex;

        /// <summary>
        /// Retrieves the valid input names for a given console.
        /// </summary>
        /// <param name="console">The console to retrieve inputs for.</param>
        /// <returns>An array of strings with valid input names for the console.
        /// If the console passed in is not found or supported, an empty array.</returns>
        public static string[] GetValidInputs(in InputConsoles console)
        {
            if (Consoles.TryGetValue(console, out ConsoleBase consoleBase) == true)
            {
                return consoleBase.ValidInputs;
            }

            return Array.Empty<string>();
        }
    }
}
