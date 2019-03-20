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

        public static bool IsAxis(in Parser.Input input)
        {
            if (input.name == "l" || input.name == "r")
            {
                return (input.percent < 100);
            }

            return (InputAxes.ContainsKey(input.name) == true);
        }

        public static bool IsWait(in Parser.Input input) => (input.name == "#" || input.name == ".");

        public static bool IsButton(in Parser.Input input)
        {
            if (input.name == "l" || input.name == "r")
            {
                return (input.percent == 100);
            }

            return (IsAxis(input) == false && IsWait(input) == false);
        }

        public static bool IsMinAxis(in string input)
        {
            return (input == "left" || input == "up" || input == "cleft" || input == "cup");
        }

        public static bool IsAbsoluteAxis(in Parser.Input input)
        {
            return ((input.name == "l" || input.name == "r") && input.percent != 100);
        }

        #region Input Definitions

        public static readonly Dictionary<string, uint> InputMap = new Dictionary<string, uint>() {
            { "left", 1 }, { "c", 1 },
            { "right", 2 }, { "z", 2 },
            { "up", 3 }, { "tleft", 3 },
            { "down", 4 }, { "tright", 4 },
            { "a", 5 },
            { "b", 6 },
            { "l", 7 }, { "one", 7 },
            { "r", 8 }, { "two", 8 },
            { "select", 9 }, { "minus", 9 },
            { "start", 10 }, { "plus", 10 },
            { "cleft", 11 }, { "pleft", 11 },
            { "cright", 12 }, { "pright", 12 },
            { "cup", 13 }, { "pup", 13 },
            { "cdown", 14 }, { "pdown", 14 },
            { "dleft", 15 },
            { "dright", 16 },
            { "dup", 17 },
            { "ddown", 18 },
            { "SAVESTATE1", 19 }, { "tforward", 19 },
            { "SAVESTATE2", 20 }, { "tback", 20 },
            { "SAVESTATE3", 21 },
            { "SAVESTATE4", 22 },
            { "SAVESTATE5", 23 },
            { "SAVESTATE6", 24 },
            { "LOADSTATE1", 25 },
            { "LOADSTATE2", 26 },
            { "LOADSTATE3", 27 },
            { "LOADSTATE4", 28 },
            { "LOADSTATE5", 29 },
            { "LOADSTATE6", 30 },
            { "x", 31 }, { "shake", 31 },
            { "y", 32 }, { "point", 32 },
        };

        public static readonly Dictionary<string, HID_USAGES> InputAxes = new Dictionary<string, HID_USAGES>()
        {
            { "left", HID_USAGES.HID_USAGE_X },
            { "right", HID_USAGES.HID_USAGE_X },
            { "up", HID_USAGES.HID_USAGE_Y },
            { "down", HID_USAGES.HID_USAGE_Y },
            { "cleft", HID_USAGES.HID_USAGE_RX },
            { "cright", HID_USAGES.HID_USAGE_RX },
            { "cup", HID_USAGES.HID_USAGE_RY },
            { "cdown", HID_USAGES.HID_USAGE_RY },
            { "l", HID_USAGES.HID_USAGE_RZ },
            { "r", HID_USAGES.HID_USAGE_Z }
        };

        #endregion
    }
}
