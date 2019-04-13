using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace KimimaruBot
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
            SetConsole(CurrentConsoleVal, null);
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
            { InputConsoles.Wii, new WiiConsole() }
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

                CurrentConsole.HandleArgsOnConsoleChange(consoleArgs);
            }
            else
            {
                Console.WriteLine($"Console {consoleVal} not supported!");
            }
        }

        /// <summary>
        /// The default duration of an input.
        /// </summary>
        public const int DURATION_DEFAULT = 200;

        /// <summary>
        /// The max duration of a given input sequence.
        /// </summary>
        public const int DURATION_MAX = 60000;

        /// <summary>
        /// Returns the valid input names of the current console.
        /// </summary>
        public static string[] ValidInputs => CurrentConsole.ValidInputs;

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
