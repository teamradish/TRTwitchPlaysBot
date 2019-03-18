using System;
using System.Collections.Generic;
using System.Text;

namespace KimimaruBot
{
    /// <summary>
    /// Defines inputs.
    /// </summary>
    public static class InputGlobals
    {
        /// <summary>
        /// The consoles that inputs are supported for.
        /// </summary>
        public enum InputConsoles
        {
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

        /// <summary>
        /// The current console inputs are being sent for.
        /// </summary>
        public static InputConsoles CurrentConsole = InputConsoles.SNES;

        //SNES
        private static readonly string[] SNESInputs = new string[14]
        {
            "left", "right", "up", "down",
            "a", "b", "l", "r", "x", "y",
            "start", "select",
            "#", "."
        };

        //N64        
        private static readonly string[] N64Inputs = new string[20]
        {
            "left", "right", "up", "down",
            "dleft", "dright", "dup", "ddown",
            "cleft,", "cright", "cup", "cdown",
            "a", "b", "l", "r", "z",
            "start",
            "#", "."
        };

        //GC
        private static readonly string[] GCInputs = new string[22]
        {
            "left", "right", "up", "down",
            "dleft", "dright", "dup", "ddown",
            "cleft,", "cright", "cup", "cdown",
            "a", "b", "l", "r", "x", "y", "z",
            "start",
            "#", "."
        };

        //Wii
        private static readonly string[] WiiInputs = new string[24]
        {
            "left", "right", "up", "down",
            "pleft", "pright", "pup", "pdown",
            "tleft,", "tright", "tup", "tdown",
            "a", "b", "one", "two", "minus", "plus",
            "c", "z",
            "shake", "point",
            "#", "."
        };

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
        public static string[] ValidInputs => GetValidInputs(CurrentConsole);

        /// <summary>
        /// Retrieves the valid input names for a given console.
        /// </summary>
        /// <param name="console">The console to retrieve inputs for.</param>
        /// <returns>An array of strings with valid input names for the console.</returns>
        public static string[] GetValidInputs(in InputConsoles console)
        {
            switch(console)
            {
                case InputConsoles.SNES:
                default: return SNESInputs;
                case InputConsoles.N64: return N64Inputs;
                case InputConsoles.GC: return GCInputs;
                case InputConsoles.Wii: return WiiInputs;
            }
        }

        #region Input Definitions

        #endregion
    }
}
