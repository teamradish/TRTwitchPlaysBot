using System;
using System.Collections.Generic;
using System.Text;

namespace TRBot
{
    /// <summary>
    /// Access data for defined inputs.
    /// </summary>
    public sealed class InputAccessData
    {
        public Dictionary<string, int> InputAccessDict = new Dictionary<string, int>(4)
        {
            { "savestate1", (int)AccessLevels.Levels.Moderator },
            { "savestate2", (int)AccessLevels.Levels.Moderator },
            { "ss1", (int)AccessLevels.Levels.Moderator },
            { "ss2", (int)AccessLevels.Levels.Moderator }
        };
    }
}
