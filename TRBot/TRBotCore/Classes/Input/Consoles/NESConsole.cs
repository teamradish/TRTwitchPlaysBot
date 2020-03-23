using System;
using System.Collections.Generic;
using System.Text;

namespace TRBot
{
    /// <summary>
    /// The NES, or Famicom.
    /// </summary>
    public sealed class NESConsole : ConsoleBase
    {
        public override Dictionary<string, int> InputAxes { get; protected set; } = new Dictionary<string, int>();

        public override Dictionary<string, uint> ButtonInputMap { get; protected set; } = new Dictionary<string, uint>()
        {
            { "left", 1 },
            { "right", 2 },
            { "up", 3 },
            { "down", 4 },
            { "a", 5 },
            { "b", 6 },
            { "select", 7 },
            { "start", 8 },
            { "savestate1", 19 }, { "ss1", 19 },
            { "savestate2", 20 }, { "ss2", 20 },
            { "savestate3", 21 }, { "ss3", 21 },
            { "savestate4", 22 }, { "ss4", 22 },
            { "savestate5", 23 }, { "ss5", 23 },
            { "savestate6", 24 }, { "ss6", 24 },
            { "loadstate1", 25 }, { "ls1", 25 },
            { "loadstate2", 26 }, { "ls2", 26 },
            { "loadstate3", 27 }, { "ls3", 27 },
            { "loadstate4", 28 }, { "ls4", 28 },
            { "loadstate5", 29 }, { "ls5", 29 },
            { "loadstate6", 30 }, { "ls6", 30 },
        };

        public override string[] ValidInputs { get; protected set; } = new string[]
        {
            "up", "down", "left", "right", "a", "b", "select", "start",
            "savestate1", "savestate2", "savestate3", "savestate4", "savestate5", "savestate6", "ss1", "ss2", "ss3", "ss4", "ss5", "ss6",
            "loadstate1", "loadstate2", "loadstate3", "loadstate4", "loadstate5", "loadstate6", "ls1", "ls2", "ls3", "ls4", "ls5", "ls6",
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
