using System;
using System.Collections.Generic;
using System.Text;

namespace TRBot
{
    /// <summary>
    /// The Nintendo 64.
    /// </summary>
    public sealed class N64Console : ConsoleBase
    {
        public override string[] ValidInputs { get; protected set; } = new string[20]
        {
            "left", "right", "up", "down",
            "dleft", "dright", "dup", "ddown",
            "cleft", "cright", "cup", "cdown",
            "a", "b", "l", "r", "z",
            "start",
            "#", "."
        };

        public override Dictionary<string, HID_USAGES> InputAxes { get; protected set; } = new Dictionary<string, HID_USAGES>()
        {
            { "left", HID_USAGES.HID_USAGE_X },
            { "right", HID_USAGES.HID_USAGE_X },
            { "up", HID_USAGES.HID_USAGE_Y },
            { "down", HID_USAGES.HID_USAGE_Y }
        };

        public override Dictionary<string, uint> ButtonInputMap { get; protected set; } = new Dictionary<string, uint>()
        {
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
        };

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
            return InputAxes.ContainsKey(input.name);
        }

        public override bool IsMinAxis(in Parser.Input input)
        {
            return (input.name == "up" || input.name == "left");
        }
        
        public override bool IsButton(in Parser.Input input)
        {
            return (IsWait(input) == false);
        }
    }
}
