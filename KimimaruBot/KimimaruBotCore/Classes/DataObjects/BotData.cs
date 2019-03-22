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
    }
}
