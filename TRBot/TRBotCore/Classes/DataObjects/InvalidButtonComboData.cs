using System;
using System.Collections.Generic;
using System.Text;

namespace TRBot
{
    /// <summary>
    /// Data for invalid button combos.
    /// </summary>
    public sealed class InvalidButtonComboData
    {
        public readonly Dictionary<int, List<string>> InvalidCombos = new Dictionary<int, List<string>>(4)
        {
            { (int)InputGlobals.InputConsoles.GBA, new List<string>() { "a", "b", "start", "select" } }
        };
    }
}
