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
        /// The consoles that inputs are supported for.
        /// </summary>
        public enum InputConsoles
        {
            NES,
            SNES,
            N64,
            GC,
            Wii,
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
            { InputConsoles.Custom, new CustomConsole() }
        };

        /// <summary>
        /// The console value associated with the current console inputs are being sent for.
        /// </summary>
        public static InputConsoles CurrentConsoleVal { get; private set; } = InputConsoles.GC;

        /// <summary>
        /// The current console inputs are being sent for.
        /// </summary>
        public static ConsoleBase CurrentConsole { get; private set; } = null;

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
