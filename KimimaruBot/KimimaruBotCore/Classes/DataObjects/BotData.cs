using System;
using System.Collections.Generic;
using System.Text;

namespace KimimaruBot
{
    /// <summary>
    /// Data for the bot, such as users, macros and memes.
    /// </summary>
    public sealed class BotData
    {
        public readonly Dictionary<string, string> Macros = new Dictionary<string, string>();
        public readonly Dictionary<string, string> Memes = new Dictionary<string, string>();
        public readonly Dictionary<string, User> Users = new Dictionary<string, User>();
        public readonly List<GameLog> Logs = new List<GameLog>();
        public readonly JumpRopeData JRData = new JumpRopeData();
        public readonly Dictionary<int, GameLog> SavestateLogs = new Dictionary<int, GameLog>();
        public readonly HashSet<string> SilencedUsers = new HashSet<string>();
        public string GameMessage = string.Empty;
        public int LastConsole = 0;

        /// <summary>
        /// The default duration of an input if no duration is specified.
        /// </summary>
        public int DefaultInputDuration = 200;

        /// <summary>
        /// The max duration of a given input sequence.
        /// </summary>
        public int MaxInputDuration = 60000;

        /// <summary>
        /// The number of joysticks connected. You must have vJoy configured for this many joysticks.
        /// </summary>
        public int JoystickCount = 1;
    }
}
