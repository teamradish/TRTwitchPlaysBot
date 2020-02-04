using System;
using System.Collections.Generic;
using System.Text;

namespace TRBot
{
    /// <summary>
    /// A custom console.
    /// </summary>
    public sealed class CustomConsole : ConsoleBase
    {
        public override Dictionary<string, HID_USAGES> InputAxes { get; protected set; } = new Dictionary<string, HID_USAGES>();

        public override Dictionary<string, uint> ButtonInputMap { get; protected set; } = new Dictionary<string, uint>()
        {
            { "left", 3 }, { "l", 3 },
            { "right", 4 }, { "r", 4 },
            { "up", 1 }, { "u", 1 },
            { "down", 2 }, { "d", 2 },
            { "grab", 5 }, { "g", 5 },
            { "select", 6 }, { "s", 6 },
            { "pause", 7 }, { "p", 7 }, { "start", 7 },
            { "restart", 8 },
            { "undo", 9 },
            { "back", 10 },
            { "viewmap", 11 },
        };

        public override string[] ValidInputs { get; protected set; } = new string[]
        {
            "up", "u", "down", "d", "left", "l", "right", "r", "grab", "g",
            "select", "s", "pause", "p", "start", "restart", "undo", "back", "viewmap", 
            "#", "."
        };

        public override bool GetAxis(in Parser.Input input, out HID_USAGES axis)
        {
            axis = default;
            return false;
        }

        public override bool IsAbsoluteAxis(in Parser.Input input) => false;

        public override bool IsAxis(in Parser.Input input) => false;

        public override bool IsMinAxis(in Parser.Input input) => false;

        public override bool IsButton(in Parser.Input input)
        {
            return (IsWait(input) == false);
        }
    }
}
