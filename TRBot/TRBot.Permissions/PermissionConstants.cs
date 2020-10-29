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
        public const string SET_DEFAULT_INPUT_DUR_ABILITY = "setdefaultinputdur";
        
        /// <summary>
        /// The ability to set the global max input duration.
        /// </summary>
        public const string SET_MAX_INPUT_DUR_ABILITY = "setmaxinputdur";

        /// <summary>
        /// User-specific default input duration.
        /// </summary>
        public const string USER_DEFAULT_INPUT_DIR_ABILITY = "userdefaultinputdur";

        /// <summary>
        /// User-specific max input duration.
        /// </summary>
        public const string USER_MAX_INPUT_DIR_ABILITY = "usermaxinputdur";

        public const string UPDATE_OTHER_USER_ABILITES = "updateotheruserabilities";
        public const string SET_GLOBAL_INPUT_LEVEL_ABILITY = "setglobalinputlevel";
        public const string SET_VCONTROLLER_TYPE_ABILITY = "setvcontrollertype";
        public const string SET_VCONTROLLER_COUNT_ABILITY = "setvcontrollercount";

        public const string SET_GAME_MESSAGE_ABILITY = "setgamemessage";

        //Abilities for games/extras
        public const string BET_ABILITY = "bet";
        public const string DUEL_ABILITY = "duel";
        public const string GROUP_BET_ABILITY = "groupbet";
        public const string JUMP_ROPE_ABILITY = "jumprope";
        public const string FEED_ABILITY = "feed";
        public const string INPUT_EXERCISE_ABILITY = "inputexercise";
        public const string CALCULATE_ABILITY = "calculate";
        public const string CHATBOT_ABILITY = "chatbot";
        public const string BINGO_ABILITY = "bingo";
        public const string TRANSFER_ABILITY = "transfer";
    }
}
