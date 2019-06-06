using System;
using System.Collections.Generic;
using System.Text;

namespace TRBot
{
    /// <summary>
    /// Access levels.
    /// </summary>
    public static class AccessLevels
    {
        /// <summary>
        /// The access levels for commands.
        /// </summary>
        public enum Levels
        {
            User = 0,
            Whitelisted = 1,
            Moderator = 2,
            Admin = 3
        }
    }
}
