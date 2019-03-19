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
        public static InputConsoles CurrentConsole = InputConsoles.GC;

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

        public static Dictionary<string, uint> INPUTS = new Dictionary<string, uint>()
        {
            { "a", 1 },
            { "b", 2 },
            { "l", 3 },
            { "r", 4 },
            { "x", 5 },
            { "y", 6 },
            { "start", 7 },
            { "dup", 8 },
            { "ddown", 9 },
            { "dleft", 10 },
            { "dright", 11 },
            { "z", 12 },
        };
        /*public static Dictionary<string, uint> INPUTS = new Dictionary<string, uint>() {
            { "left", 0 }, { "c", 0 },
            { "right", 1 }, { "z", 1 },
            { "up", 2 }, { "tleft", 2 },
            { "down", 3 }, { "tright", 3 },
            { "a", 4 },
            { "b", 5 },
            { "l", 6 }, { "one", 6 },
            { "r", 7 }, { "two", 7 },
            { "select", 8 }, { "minus", 8 },
            { "start", 9 }, { "plus", 9 },
            { "cleft", 10 }, { "pleft", 10 },
            { "cright", 11 }, { "pright", 11 },
            { "cup", 12 }, { "pup", 12 },
            { "cdown", 13 }, { "pdown", 13 },
            { "dleft", 14 },
            { "dright", 15 },
            { "dup", 16 },
            { "ddown", 17 },
            { "SAVESTATE1", 18 }, { "tforward", 18 },
            { "SAVESTATE2", 19 }, { "tback", 19 },
            { "SAVESTATE3", 20 },
            { "SAVESTATE4", 21 },
            { "SAVESTATE5", 22 },
            { "SAVESTATE6", 23 },
            { "LOADSTATE1", 24 },
            { "LOADSTATE2", 25 },
            { "LOADSTATE3", 26 },
            { "LOADSTATE4", 27 },
            { "LOADSTATE5", 28 },
            { "LOADSTATE6", 29 },
            { "x", 30 }, { "shake", 30 },
            { "y", 31 }, { "point", 31 },
        };*/

        #endregion
    }
}
