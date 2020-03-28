using System;
using System.Collections.Generic;
using System.Text;

namespace TRBot
{
    /// <summary>
    /// A desktop computer.
    /// </summary>
    public sealed class PCConsole : ConsoleBase
    {
        public override string[] ValidInputs { get; protected set; } = new string[]
        {
            "left", "right", "up", "down",
            "lclick", "mclick", "rclick",
            "return", "space",
            "#", "."
        };

        public override Dictionary<string, int> InputAxes { get; protected set; } = new Dictionary<string, int>()
        {
            { "left", (int)GlobalAxisVals.AXIS_X },
            { "right", (int)GlobalAxisVals.AXIS_X },
            { "up", (int)GlobalAxisVals.AXIS_Y },
            { "down", (int)GlobalAxisVals.AXIS_Y }
        };

        public override Dictionary<string, uint> ButtonInputMap { get; protected set; } = new Dictionary<string, uint>()
        {
            { "left", (int)GlobalButtonVals.BTN1 },
            { "right", (int)GlobalButtonVals.BTN2 },
            { "up", (int)GlobalButtonVals.BTN3 },
            { "down", (int)GlobalButtonVals.BTN4 },
            { "lclick", (int)GlobalButtonVals.BTN5 },
            { "mclick", (int)GlobalButtonVals.BTN6 },
            { "rclick", (int)GlobalButtonVals.BTN7 },
            { "return", (int)GlobalButtonVals.BTN8 },
            { "space", (int)GlobalButtonVals.BTN9 },
        };

        public override bool GetAxis(in Parser.Input input, out int axis)
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
