/* This file is part of TRBot.
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

namespace TRBot.Permissions
{
    public static class PermissionConstants
    {
        /// <summary>
        /// The ability to set the input console.
        /// </summary>
        public const string SET_CONSOLE_ABILITY = "setconsole";
        
        /// <summary>
        /// User-specific silence state - inputs are ignored.
        /// </summary>
        public const string SILENCED_ABILITY = "silenced";
        
        /// <summary>
        /// The ability to set the global default input duration.
        /// </summary>
        public const string SET_DEFAULT_INPUT_DUR = "setdefaultinputdur";
        
        /// <summary>
        /// The ability to set the global max input duration.
        /// </summary>
        public const string SET_MAX_INPUT_DUR = "setmaxinputdur";

        /// <summary>
        /// User-specific default input duration.
        /// </summary>
        public const string USER_DEFAULT_INPUT_DIR = "userdefaultinputdur";

        /// <summary>
        /// User-specific max input duration.
        /// </summary>
        public const string USER_MAX_INPUT_DIR = "usermaxinputdur";

        public const string UPDATE_OTHER_USER_ABILITES = "updateotheruserabilities";

        //Abilities for games/extras
        public const string BET_ABILITY = "betability";
        public const string DUEL_ABILITY = "duelability";
        public const string GROUP_BET_ABILITY = "groupbetability";
        public const string JUMP_ROPE_ABILITY = "jumpropeability";
        public const string FEED_ABILITY = "feedability";
        public const string INPUT_EXERCISE_ABILITY = "inputexerciseability";
        public const string CALCULATE_ABILITY = "calculateability";
        public const string CHATBOT_ABILITY = "chatbotability";
        public const string BINGO_ABILITY = "bingoability";
    }
}
