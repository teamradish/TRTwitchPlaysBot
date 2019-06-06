using System;
using System.Collections.Generic;
using System.Text;

namespace TRBot
{
    /// <summary>
    /// The Nintendo Wii.
    /// </summary>
    public sealed class WiiConsole : ConsoleBase
    {
        /// <summary>
        /// The various input modes for the Wii.
        /// This is not a bit field to make defining inputs easier.
        /// </summary>
        public enum InputModes
        {
            Remote = 0,
            RemoteNunchuk = 1
        }

        /// <summary>
        /// The current input mode for the Wii.
        /// </summary>
        public InputModes InputMode = InputModes.RemoteNunchuk;

        public override string[] ValidInputs { get; protected set; } = new string[28]
        {
            "left", "right", "up", "down",
            "pleft", "pright", "pup", "pdown",
            "tleft", "tright", "tup", "tdown",
            "dleft", "dright", "dup", "ddown",
            "a", "b", "one", "two", "minus", "plus",
            "c", "z",
            "shake", "point",
            "#", "."
        };

        public override Dictionary<string, HID_USAGES> InputAxes { get; protected set; } = new Dictionary<string, HID_USAGES>()
        {
            { "left", HID_USAGES.HID_USAGE_X },
            { "right", HID_USAGES.HID_USAGE_X },
            { "up", HID_USAGES.HID_USAGE_Y },
            { "down", HID_USAGES.HID_USAGE_Y },
            { "tleft", HID_USAGES.HID_USAGE_RX },
            { "tright", HID_USAGES.HID_USAGE_RX },
            { "tforward", HID_USAGES.HID_USAGE_RY },
            { "tback", HID_USAGES.HID_USAGE_RY },
            { "pleft", HID_USAGES.HID_USAGE_RZ },
            { "pright", HID_USAGES.HID_USAGE_RZ },
            { "pup", HID_USAGES.HID_USAGE_Z },
            { "pdown", HID_USAGES.HID_USAGE_Z }
        };

        //Kimimaru: NOTE - Though vJoy supports up to 128 buttons, Dolphin supports only 32 max
        //The Wii Remote + Nunchuk has more than 32 inputs, so since we can't fit them all, we'll need some modes to toggle
        //and change the input map based on the input scheme
        //We might have to make some sacrifices, such as fewer savestates or disallowing use of the D-pad if a Nunchuk is being used
        public override Dictionary<string, uint> ButtonInputMap { get; protected set; } = new Dictionary<string, uint>() {
            { "left", 1 }, { "c", 1 },
            { "right", 2 }, { "z", 2 },
            { "up", 3 }, { "tleft", 3 },
            { "down", 4 }, { "tright", 4 },
            { "a", 5 },
            { "b", 6 },
            { "one", 7 },
            { "two", 8 },
            { "minus", 9 },
            { "plus", 10 },
            { "pleft", 11 },
            { "pright", 12 },
            { "pup", 13 },
            { "pdown", 14 },
            { "dleft", 15 },
            { "dright", 16 },
            { "dup", 17 },
            { "ddown", 18 },
            { "savestate1", 19 }, { "tforward", 19 },
            { "savestate2", 20 }, { "tback", 20 },
            { "savestate3", 21 },
            { "savestate4", 22 },
            { "savestate5", 23 },
            { "savestate6", 24 },
            { "loadstate1", 25 },
            { "loadstate2", 26 },
            { "loadstate3", 27 },
            { "loadstate4", 28 },
            { "loadstate5", 29 },
            { "loadstate6", 30 },
            { "shake", 31 },
            { "point", 32 }
        };

        public override void HandleArgsOnConsoleChange(List<string> arguments)
        {
            if (arguments?.Count == 0) return;

            for (int i = 0; i < arguments.Count; i++)
            {
                //Check for enum mode
                if (Enum.TryParse(arguments[i], true, out InputModes mode) == true)
                {
                    InputMode = mode;
                    BotProgram.QueueMessage($"Changed Wii input mode to {mode}!");
                }

                //Kimimaru: I'm debating including the ability to set it by number, so we'll keep it commented until a decision is made
                //if (int.TryParse(arguments[i], out int inputMode) == true)
                //{
                //    InputModes newInputMode = (InputModes)inputMode;
                //    InputMode = (InputModes)inputMode;
                //    Console.WriteLine($"Changed Wii input mode to {newInputMode}");
                //}
            }
        }

        public override bool GetAxis(in Parser.Input input, out HID_USAGES axis)
        {
            return InputAxes.TryGetValue(input.name, out axis);
        }

        public override bool IsAbsoluteAxis(in Parser.Input input)
        {
            return false;
        }

        public override bool IsAxis(in Parser.Input input)
        {
            return (InputAxes.ContainsKey(input.name) == true);
        }

        public override bool IsMinAxis(in Parser.Input input)
        {
            return (input.name == "left" || input.name == "up" || input.name == "tleft" || input.name == "tforward"
                || input.name == "pleft" || input.name == "pup");
        }

        public override bool IsButton(in Parser.Input input)
        {
            return (IsWait(input) == false && IsAxis(input) == false);
        }
    }
}
