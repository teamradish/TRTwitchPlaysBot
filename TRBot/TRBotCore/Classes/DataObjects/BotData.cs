using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TRBot
{
    /// <summary>
    /// Data for the bot, such as users, macros and memes.
    /// </summary>
    public sealed class BotData
    {
        /// <summary>
        /// This dictionary is used to improve the speed of macro lookups for the parser, which needs to iterate through them since there are no spaces.
        /// This is used only internally by the bot and is not saved in the JSON.
        /// </summary>
        [JsonIgnore]
        public readonly Dictionary<char, List<string>> ParserMacroLookup = new Dictionary<char, List<string>>(16);

        public readonly Dictionary<string, string> Macros = new Dictionary<string, string>(64);
        public readonly Dictionary<string, string> Memes = new Dictionary<string, string>(64);
        public readonly Dictionary<string, User> Users = new Dictionary<string, User>(128);
        public readonly List<GameLog> Logs = new List<GameLog>(32);
        public readonly JumpRopeData JRData = new JumpRopeData();
        public readonly Dictionary<int, GameLog> SavestateLogs = new Dictionary<int, GameLog>(8);
        public readonly HashSet<string> SilencedUsers = new HashSet<string>(16);
        public readonly InputAccessData InputAccess = new InputAccessData();
        public string GameMessage = string.Empty;
        public string InfoMessage = string.Empty;
        public int LastConsole = 0;

        /// <summary>
        /// The maximum duration the pause button can be held; this is often used to prevent inputs involved in resetting the game.
        /// Set this to -1 to allow it to be held indefinitely.
        /// </summary>
        public int MaxPauseHoldDuration = 500;

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
        
        /// <summary>
        /// The last type of virtual controller used.
        /// This will be overwritten if on a platform that doesn't support a specific virtual controller.
        /// </summary>
        public int LastVControllerType = 0;
    }
}
