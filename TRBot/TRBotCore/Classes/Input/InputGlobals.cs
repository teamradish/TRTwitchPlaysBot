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
            SNES,
            N64,
            GC,
            Wii,
            PS2,
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
            { InputConsoles.SNES, new SNESConsole() },
            { InputConsoles.N64, new N64Console() },
            { InputConsoles.GC, new GCConsole() },
            { InputConsoles.Wii, new WiiConsole() },
            { InputConsoles.PS2, new PS2Console() },
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

        /// <summary>
        /// Validates how long the pause input is held for. This is to prevent resetting the game for certain inputs.
        /// <para>Kimimaru: NOTE - make this more comprehensive.
        /// It should handle consistent and separated holds (Ex. "_start left1s" and "start300ms start500ms").</para>
        /// </summary>
        /// <param name="parsedInputs"></param>
        /// <param name="pauseInput"></param>
        /// <param name="maxPauseDuration"></param>
        /// <returns></returns>
        public static bool IsValidPauseInputDuration(in List<List<Parser.Input>> parsedInputs, in string pauseInput, in int maxPauseDuration)
        {
            //Check for pause duration
            if (maxPauseDuration >= 0)
            {
                for (int i = 0; i < parsedInputs.Count; i++)
                {
                    List<Parser.Input> inputList = parsedInputs[i];
                    for (int j = 0; j < inputList.Count; j++)
                    {
                        Parser.Input input = inputList[j];

                        //We found pause
                        if (input.name == pauseInput)
                        {
                            //Check how long the pause input is held for; if it's held for longer than the max, it's not valid
                            if (input.duration >= maxPauseDuration)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }
    }
}
