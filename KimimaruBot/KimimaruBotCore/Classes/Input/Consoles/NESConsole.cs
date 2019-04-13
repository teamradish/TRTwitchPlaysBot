using System;
using System.Collections.Generic;
using System.Text;

namespace KimimaruBot
{
    /// <summary>
    /// The NES, or Famicom.
    /// </summary>
    public sealed class NESConsole : ConsoleBase
    {
        public override Dictionary<string, HID_USAGES> InputAxes { get; protected set; } = new Dictionary<string, HID_USAGES>();

        public override Dictionary<string, uint> ButtonInputMap { get; protected set; } = new Dictionary<string, uint>()
        {
            { "up", 0 },
            { "down", 1 },
            { "left", 2 },
            { "right", 3 },
            { "a", 4 },
            { "b", 5 },
            { "select", 6 },
            { "start", 7 }
        };

        public override string[] ValidInputs { get; protected set; } = new string[10]
        {
            "up", "down", "left", "right", "a", "b", "select", "start",
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
