using System;
using System.Collections.Generic;
using System.Text;

namespace KimimaruBot
{
    /// <summary>
    /// The Nintendo GameCube.
    /// </summary>
    public sealed class GCConsole : ConsoleBase
    {
        public override string[] ValidInputs { get; protected set; } = new string[22]
        {
            "left", "right", "up", "down",
            "dleft", "dright", "dup", "ddown",
            "cleft", "cright", "cup", "cdown",
            "a", "b", "l", "r", "x", "y", "z",
            "start",
            "#", "."
        };

        public override Dictionary<string, HID_USAGES> InputAxes { get; protected set; } = new Dictionary<string, HID_USAGES>()
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

        public override Dictionary<string, uint> ButtonInputMap { get; protected set; } = new Dictionary<string, uint>() {
            { "left", 1 },
            { "right", 2 },
            { "up", 3 },
            { "down", 4 },
            { "a", 5 },
            { "b", 6 },
            { "l", 7 },
            { "r", 8 },
            { "z", 9 },
            { "start", 10 },
            { "cleft", 11 },
            { "cright", 12 },
            { "cup", 13 },
            { "cdown", 14 },
            { "dleft", 15 },
            { "dright", 16 },
            { "dup", 17 },
            { "ddown", 18 },
            { "savestate1", 19 },
            { "savestate2", 20 },
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
            { "x", 31 },
            { "y", 32 },
        };

        public override bool GetAxis(in Parser.Input input, out HID_USAGES axis)
        {
            if (input.name == "l" || input.name == "r")
            {
                if (input.percent == 100)
                {
                    axis = default;
                    return false;
                }
            }

            return InputAxes.TryGetValue(input.name, out axis);
        }

        public override bool IsAbsoluteAxis(in Parser.Input input)
        {
            return ((input.name == "l" || input.name == "r") && input.percent != 100);
        }

        public override bool IsAxis(in Parser.Input input)
        {
            if (input.name == "l" || input.name == "r")
            {
                return (input.percent < 100);
            }

            return (InputAxes.ContainsKey(input.name) == true);
        }

        public override bool IsMinAxis(in Parser.Input input)
        {
            return (input.name == "left" || input.name == "up" || input.name == "cleft" || input.name == "cup");
        }

        public override bool IsButton(in Parser.Input input)
        {
            if (input.name == "l" || input.name == "r")
            {
                return (input.percent == 100);
            }

            return (IsWait(input) == false && IsAxis(input) == false);
        }
    }
}
