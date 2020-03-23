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
        public override Dictionary<string, int> InputAxes { get; protected set; } = new Dictionary<string, int>();

        public override Dictionary<string, uint> ButtonInputMap { get; protected set; } = new Dictionary<string, uint>()
        {
            { "left", (int)GlobalButtonVals.BTN3 }, { "l", (int)GlobalButtonVals.BTN3 },
            { "right", (int)GlobalButtonVals.BTN4 }, { "r", (int)GlobalButtonVals.BTN4 },
            { "up", (int)GlobalButtonVals.BTN1 }, { "u", (int)GlobalButtonVals.BTN1 },
            { "down", (int)GlobalButtonVals.BTN2 }, { "d", (int)GlobalButtonVals.BTN2 },
            { "grab", (int)GlobalButtonVals.BTN5 }, { "g", (int)GlobalButtonVals.BTN5 },
            { "select", (int)GlobalButtonVals.BTN6 }, { "s", (int)GlobalButtonVals.BTN6 },
            { "pause", (int)GlobalButtonVals.BTN7 }, { "p", (int)GlobalButtonVals.BTN7 }, { "start", (int)GlobalButtonVals.BTN7 },
            { "restart", (int)GlobalButtonVals.BTN8 },
            { "undo", (int)GlobalButtonVals.BTN9 },
            { "back", (int)GlobalButtonVals.BTN10 }, { "b", (int)GlobalButtonVals.BTN10 },
            { "viewmap", (int)GlobalButtonVals.BTN11 }, { "v", (int)GlobalButtonVals.BTN11 },
        };

        public override string[] ValidInputs { get; protected set; } = new string[]
        {
            "up", "u", "down", "d", "left", "l", "right", "r", "grab", "g",
            "select", "s", "pause", "p", "start", "restart", "undo", "back", "b", "viewmap", "v", 
            "#", "."
        };

        public override bool GetAxis(in Parser.Input input, out int axis)
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
