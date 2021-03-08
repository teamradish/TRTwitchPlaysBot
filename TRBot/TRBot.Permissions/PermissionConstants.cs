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
        /// The ability to set the global mid input delay.
        /// </summary>
        public const string SET_MID_INPUT_DELAY_ABILITY = "setmidinputdelay";

        /// <summary>
        /// User-specific default input duration.
        /// </summary>
        public const string USER_DEFAULT_INPUT_DUR_ABILITY = "userdefaultinputdur";

        /// <summary>
        /// User-specific max input duration.
        /// </summary>
        public const string USER_MAX_INPUT_DUR_ABILITY = "usermaxinputdur";

        /// <summary>
        /// User-specific mid input delay.
        /// </summary>
        public const string USER_MID_INPUT_DELAY_ABILITY = "usermidinputdelay";

        public const string UPDATE_OTHER_USER_ABILITES = "updateotheruserabilities";
        public const string SET_GLOBAL_INPUT_LEVEL_ABILITY = "setglobalinputlevel";
        public const string SET_VCONTROLLER_TYPE_ABILITY = "setvcontrollertype";
        public const string SET_VCONTROLLER_COUNT_ABILITY = "setvcontrollercount";

        public const string SET_GAME_MESSAGE_ABILITY = "setgamemessage";

        public const string SET_PERIODIC_INPUT_ABILITY = "setperiodicinput";
        public const string SET_PERIODIC_INPUT_PORT_ABILITY = "setperiodicinputport";
        public const string SET_PERIODIC_INPUT_TIME_ABILITY = "setperiodicinputtime";
        public const string SET_PERIODIC_INPUT_SEQUENCE_ABILITY = "setperiodicinputsequence";
        
        public const string SET_MAX_USER_RECENT_INPUTS_ABILITY = "setmaxuserrecentinputs"; 

        public const string SET_DEMOCRACY_VOTE_TIME_ABILITY = "setdemocracyvotetime";
        public const string SET_DEMOCRACY_RESOLUTION_MODE_ABILITY = "setdemocracyresolutionmode";
        public const string SET_INPUT_MODE_VOTE_TIME_ABILITY = "setinputmodevotetime";
        public const string SET_INPUT_MODE_CHANGE_COOLDOWN_ABILITY = "setinputmodechangecooldown";
        public const string SET_INPUT_MODE_ABILITY = "setinputmode";
        public const string VOTE_INPUT_MODE_ABILITY = "voteinputmode";

        public const string ADD_INPUT_MACRO_ABILITY = "addinputmacro";
        public const string REMOVE_INPUT_MACRO_ABILITY = "removeinputmacro";
        public const string ADD_MEME_ABILITY = "addmeme";
        public const string REMOVE_MEME_ABILITY = "removememe";
        public const string ADD_INPUT_SYNONYM_ABILITY = "addinputsynonym";
        public const string REMOVE_INPUT_SYNONYM_ABILITY = "removeinputsynonym";

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
        public const string SLOTS_ABILITY = "slots";
        public const string SET_TEAMS_MODE_ABILITY = "setteamsmode";
        public const string SET_TEAMS_MODE_MAX_PORT_ABILITY = "setteamsmodemaxport";
    }
}
