/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * TRBot is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with TRBot.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using Newtonsoft.Json;

namespace TRBotDataMigrationTool
{
    /// <summary>
    /// Legacy data for the bot, such as users, macros and memes.
    /// </summary>
    public sealed class BotData
    {
        public readonly ConcurrentDictionary<string, string> Macros = new ConcurrentDictionary<string, string>(Environment.ProcessorCount * 2, 64);
        public readonly ConcurrentDictionary<string, string> Memes = new ConcurrentDictionary<string, string>(Environment.ProcessorCount * 2, 64);
        public readonly ConcurrentDictionary<string, User> Users = new ConcurrentDictionary<string, User>(Environment.ProcessorCount * 2, 128);
        public readonly List<GameLog> Logs = new List<GameLog>(32);
        public readonly ConcurrentDictionary<int, GameLog> SavestateLogs = new ConcurrentDictionary<int, GameLog>(Environment.ProcessorCount * 2, 8);
        public readonly HashSet<string> SilencedUsers = new HashSet<string>(16);
        public readonly InputAccessData InputAccess = new InputAccessData();
        public readonly InvalidButtonComboData InvalidBtnCombos = new InvalidButtonComboData();
        public readonly InputSynonymData InputSynonyms = new InputSynonymData();

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
        /// The number of joysticks connected.
        /// If using vJoy, you must have it configured to use all of these joysticks.
        /// </summary>
        public int JoystickCount = 1;
        
        /// <summary>
        /// The last type of virtual controller used.
        /// This will be overwritten on a platform that doesn't support a specific virtual controller.
        /// </summary>
        public int LastVControllerType = 0;
        
        /// <summary>
        /// The lowest permission available for inputs.
        /// Users with levels above this value can perform inputs.
        /// </summary>
        public int InputPermissions = (int)AccessLevels.Levels.User;
    }
}
