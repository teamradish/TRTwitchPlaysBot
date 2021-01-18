/* Copyright (C) 2019-2020 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot,software for playing games through text.
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
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace TRBot.Data
{
    /// <summary>
    /// Constants for bot setting keys.
    /// </summary>
    public static class SettingsConstants
    {
        public const string MAIN_THREAD_SLEEP = "main_thread_sleep";
        
        public const string CREDITS_NAME = "credits_name";
        public const string CREDITS_GIVE_TIME = "credits_give_time";
        public const string CREDITS_GIVE_AMOUNT = "credits_give_amount";
        public const string DUEL_TIMEOUT = "duel_timeout";
        public const string GROUP_BET_TOTAL_TIME = "group_bet_total_time";
        public const string GROUP_BET_MIN_PARTICIPANTS = "group_bet_min_participants";

        public const string CHATBOT_ENABLED = "chatbot_enabled";
        public const string CHATBOT_SOCKET_PATH = "chatbot_socket_path";
        public const string CHATBOT_SOCKET_PATH_IS_RELATIVE = "chatbot_socket_path_is_relative";

        public const string BINGO_ENABLED = "bingo_enabled";
        public const string BINGO_PIPE_PATH = "bingo_pipe_path";
        public const string BINGO_PIPE_PATH_IS_RELATIVE = "bingo_pipe_path_is_relative";

        public const string CLIENT_SERVICE_TYPE = "client_service_type";
        public const string LOG_LEVEL = "log_level";

        public const string AUTO_PROMOTE_ENABLED = "auto_promote_enabled";
        public const string AUTO_PROMOTE_LEVEL = "auto_promote_level";
        public const string AUTO_PROMOTE_INPUT_REQ = "auto_promote_input_req";

        public const string BOT_MSG_CHAR_LIMIT = "bot_message_char_limit";
        public const string PERIODIC_MSG_TIME = "periodic_message_time";
        public const string MESSAGE_THROTTLE_TYPE = "message_throttle_type";
        public const string MESSAGE_COOLDOWN = "message_cooldown";
        public const string MESSAGE_THROTTLE_COUNT = "message_throttle_count";
        public const string RECONNECT_TIME = "reconnect_time";

        public const string CONNECT_MESSAGE = "connect_message";
        public const string RECONNECTED_MESSAGE = "reconnected_message";
        public const string PERIODIC_MESSAGE = "periodic_message";
        public const string AUTOPROMOTE_MESSAGE = "auto_promote_message";
        public const string NEW_USER_MESSAGE = "new_user_message";
        public const string BEING_HOSTED_MESSAGE = "being_hosted_message";
        public const string NEW_SUBSCRIBER_MESSAGE = "new_subscriber_message";
        public const string RESUBSCRIBER_MESSAGE = "resubscriber_message";
        public const string SOURCE_CODE_MESSAGE = "source_code_message";

        public const string GAME_MESSAGE = "game_message";
        public const string GAME_MESSAGE_PATH = "game_message_path";
        public const string GAME_MESSAGE_PATH_IS_RELATIVE = "game_message_path_is_relative";
        public const string INFO_MESSAGE = "info_message";
        public const string TUTORIAL_MESSAGE = "tutorial_message";
        public const string DOCUMENTATION_MESSAGE = "documentation_message";
        public const string DONATE_MESSAGE = "donate_message";
        
        public const string SLOTS_BLANK_EMOTE = "slots_blank_emote";
        public const string SLOTS_CHERRY_EMOTE = "slots_cherry_emote";
        public const string SLOTS_PLUM_EMOTE = "slots_plum_emote";
        public const string SLOTS_WATERMELON_EMOTE = "slots_watermelon_emote";
        public const string SLOTS_ORANGE_EMOTE = "slots_orange_emote";
        public const string SLOTS_LEMON_EMOTE = "slots_lemon_emote";
        public const string SLOTS_BAR_EMOTE = "slots_bar_emote";
        
        public const string PERIODIC_INPUT_ENABLED = "periodic_input_enabled";
        public const string PERIODIC_INPUT_TIME = "periodic_input_time";
        public const string PERIODIC_INPUT_PORT = "periodic_input_port";
        public const string PERIODIC_INPUT_VALUE = "periodic_input_value";

        public const string TEAMS_MODE_ENABLED = "teams_mode_enabled";
        public const string TEAMS_MODE_MAX_PORT = "teams_mode_max_port";
        public const string TEAMS_MODE_NEXT_PORT = "teams_mode_next_port";

        public const string DEFAULT_INPUT_DURATION = "default_input_duration";
        public const string MAX_INPUT_DURATION = "max_input_duration";
        
        public const string GLOBAL_MID_INPUT_DELAY_ENABLED = "global_mid_input_delay_enabled";
        public const string GLOBAL_MID_INPUT_DELAY_TIME = "global_mid_input_delay_time";

        public const string MAX_USER_RECENT_INPUTS = "max_user_recent_inputs";

        public const string DEMOCRACY_VOTE_TIME = "democracy_vote_time";
        public const string DEMOCRACY_RESOLUTION_MODE = "democracy_resolution_mode";
        public const string INPUT_MODE_VOTE_TIME = "input_mode_vote_time";
        public const string INPUT_MODE_CHANGE_COOLDOWN = "input_mode_change_cooldown";
        public const string INPUT_MODE_NEXT_VOTE_DATE = "input_mode_next_vote_date";

        public const string LAST_CONSOLE = "last_console";
        public const string LAST_VCONTROLLER_TYPE = "last_vcontroller_type";
        public const string JOYSTICK_COUNT = "joystick_count";

        public const string GLOBAL_INPUT_LEVEL = "global_input_level";
        public const string INPUT_MODE = "input_mode";

        public const string FIRST_LAUNCH = "first_launch";
        public const string FORCE_INIT_DEFAULTS = "force_init_defaults";
        public const string DATA_VERSION_NUM = "data_version";
    }
}
