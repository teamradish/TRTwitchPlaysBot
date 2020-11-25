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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TRBot.Consoles;
using TRBot.Connection;
using TRBot.VirtualControllers;
using TRBot.Misc;
using TRBot.Parsing;
using TRBot.Permissions;
using TRBot.Utilities;
using static TRBot.Data.SettingsConstants;
using static TRBot.Permissions.PermissionConstants;

namespace TRBot.Data
{
    /// <summary>
    /// Provides default values for data.
    /// </summary>
    public static class DefaultData
    {
        #region Settings

        /// <summary>
        /// Returns a list of Settings objects containing recommended default values.
        /// </summary>
        /// <returns>A list of Settings objects with their default values.</returns>
        public static List<Settings> GetDefaultSettings()
        {
            List<Settings> defaultSettings = new List<Settings>()
            {
                SettingsHelper(MAIN_THREAD_SLEEP, 100L),
                SettingsHelper(CREDITS_NAME, "Credits"),
                SettingsHelper(CREDITS_GIVE_TIME, 120000L),
                SettingsHelper(CREDITS_GIVE_AMOUNT, 100L),
                SettingsHelper(DUEL_TIMEOUT, 60000L),
                SettingsHelper(GROUP_BET_TOTAL_TIME, 120000L),
                SettingsHelper(GROUP_BET_MIN_PARTICIPANTS, 3L),
                SettingsHelper(CHATBOT_ENABLED, false),
                SettingsHelper(CHATBOT_SOCKET_PATH, "ChatterBotSocket"),
                SettingsHelper(CHATBOT_SOCKET_PATH_IS_RELATIVE, true),
                SettingsHelper(BINGO_ENABLED, false),
                SettingsHelper(BINGO_PIPE_PATH, "BingoPipe"),
                SettingsHelper(BINGO_PIPE_PATH_IS_RELATIVE, true),
                SettingsHelper(CLIENT_SERVICE_TYPE, (long)ClientServiceTypes.Twitch),
                SettingsHelper(AUTO_PROMOTE_ENABLED, true),
                SettingsHelper(AUTO_PROMOTE_LEVEL, (long)PermissionLevels.Whitelisted),
                SettingsHelper(AUTO_PROMOTE_INPUT_REQ, 50L),
                SettingsHelper(BOT_MSG_CHAR_LIMIT, 500L),
                SettingsHelper(PERIODIC_MSG_TIME, 1800000L),
                SettingsHelper(MESSAGE_COOLDOWN, 1000L),
                SettingsHelper(CONNECT_MESSAGE, "Your friendly Twitch Plays bot has connected :D ! Use !help to display a list of commands and !tutorial to see how to play! Original input parser by Jdog, aka TwitchPlays_Everything, rewritten and improved over time by the community."),
                SettingsHelper(RECONNECTED_MESSAGE, "Successfully reconnected to chat!"),
                SettingsHelper(PERIODIC_MESSAGE, "Hi! I'm your friendly Twitch Plays bot :D ! Use !help to display a list of commands!"),
                SettingsHelper(AUTOPROMOTE_MESSAGE, "{0} has been promoted to {1}! New commands are available."),
                SettingsHelper(NEW_USER_MESSAGE, "Welcome to the stream, {0} :D ! We hope you enjoy your stay!"),
                SettingsHelper(BEING_HOSTED_MESSAGE, "Thank you for hosting, {0}!!"),
                SettingsHelper(NEW_SUBSCRIBER_MESSAGE, "Thank you for subscribing, {0} :D !!"),
                SettingsHelper(RESUBSCRIBER_MESSAGE, "Thank you for subscribing for {1} months, {0} :D !!"),
                SettingsHelper(SOURCE_CODE_MESSAGE, "This bot is free software licensed under the AGPL v3.0. The code repository and full license terms are at https://github.com/teamradish/TRTwitchPlaysBot - You have the right to obtain source code for the streamer's deployed version of the software."),
                SettingsHelper(GAME_MESSAGE, "This is a game message."),
                SettingsHelper(GAME_MESSAGE_PATH, "GameMessage.txt"),
                SettingsHelper(GAME_MESSAGE_PATH_IS_RELATIVE, true),
                SettingsHelper(INFO_MESSAGE, "Welcome to the channel! You can play games by submitting messages in chat. Type !inputs to see all available buttons."),
                SettingsHelper(TUTORIAL_MESSAGE, "Hi {0}, here's how to play: https://github.com/teamradish/TRTwitchPlaysBot/Wiki/Syntax-Walkthrough.md"),
                SettingsHelper(PERIODIC_INPUT_ENABLED, 0L),
                SettingsHelper(PERIODIC_INPUT_TIME, 1000 * 60 * 5),
                SettingsHelper(PERIODIC_INPUT_PORT, 0L),
                SettingsHelper(PERIODIC_INPUT_VALUE, string.Empty),
                SettingsHelper(TEAMS_MODE_ENABLED, 0L),
                SettingsHelper(TEAMS_MODE_MAX_PORT, 3L),
                SettingsHelper(TEAMS_MODE_NEXT_PORT, 0L),
                SettingsHelper(DEFAULT_INPUT_DURATION, 200L),
                SettingsHelper(MAX_INPUT_DURATION, 60000L),
                SettingsHelper(LAST_CONSOLE, 1L),
                SettingsHelper(LAST_VCONTROLLER_TYPE, (long)VControllerHelper.GetDefaultVControllerTypeForPlatform(TRBotOSPlatform.CurrentOS)),
                SettingsHelper(JOYSTICK_COUNT, 1L),
                SettingsHelper(GLOBAL_INPUT_LEVEL, (long)PermissionLevels.User),
            };

            return defaultSettings;
        }

        private static Settings SettingsHelper(string key, string value_str)
        {
            return new Settings(key, value_str, 0L);
        }

        private static Settings SettingsHelper(string key, in long value_int)
        {
            return new Settings(key, string.Empty, value_int);
        }

        private static Settings SettingsHelper(string key, in bool value)
        {
            return new Settings(key, string.Empty, value == true ? 1L : 0L);
        }

        #endregion

        #region Command Data

        /// <summary>
        /// Returns a list of CommandData objects containing recommended default values.
        /// </summary>
        /// <returns>A list of CommandData objects with their default values.</returns>
        public static List<CommandData> GetDefaultCommands()
        {
            List<CommandData> defaultCommands = new List<CommandData>()
            {
                new CommandData("sourcecode", "TRBot.Commands.MessageCommand", (long)PermissionLevels.User, true, true, SettingsConstants.SOURCE_CODE_MESSAGE),
                new CommandData("info", "TRBot.Commands.MessageCommand", (long)PermissionLevels.User, true, true, SettingsConstants.INFO_MESSAGE),
                new CommandData("tutorial", "TRBot.Commands.MessageCommand", (long)PermissionLevels.User, true, true, SettingsConstants.TUTORIAL_MESSAGE),
                new CommandData("version", "TRBot.Commands.VersionCommand", (long)PermissionLevels.User, true, true),
                new CommandData("console", "TRBot.Commands.GetSetConsoleCommand", (long)PermissionLevels.User, true, true),
                new CommandData("inputs", "TRBot.Commands.InputInfoCommand", (long)PermissionLevels.User, true, true),
                new CommandData("macros", "TRBot.Commands.ListMacrosCommand", (long)PermissionLevels.User, true, true),
                new CommandData("addmacro", "TRBot.Commands.AddMacroCommand", (long)PermissionLevels.User, true, true),
                new CommandData("removemacro", "TRBot.Commands.RemoveMacroCommand", (long)PermissionLevels.User, true, true),
                new CommandData("showmacro", "TRBot.Commands.ShowMacroCommand", (long)PermissionLevels.User, true, true),
                new CommandData("memes", "TRBot.Commands.ListMemesCommand", (long)PermissionLevels.User, true, true),
                new CommandData("addmeme", "TRBot.Commands.AddMemeCommand", (long)PermissionLevels.User, true, true),
                new CommandData("removememe", "TRBot.Commands.RemoveMemeCommand", (long)PermissionLevels.User, true, true),
                new CommandData("viewlog", "TRBot.Commands.ViewGameLogCommand", (long)PermissionLevels.User, true, true),
                new CommandData("stopall", "TRBot.Commands.StopAllInputsCommand", (long)PermissionLevels.User, true, true),
                new CommandData("allabilities", "TRBot.Commands.ListPermissionAbilitiesCommand", (long)PermissionLevels.User, true, true),
                new CommandData("userabilities", "TRBot.Commands.ListUserAbilitiesCommand", (long)PermissionLevels.User, true, true),
                new CommandData("updateabilities", "TRBot.Commands.UpdateAllUserAbilitiesCommand", (long)PermissionLevels.User, true, true),
                new CommandData("level", "TRBot.Commands.LevelCommand", (long)PermissionLevels.User, true, true),
                new CommandData("runninginputs", "TRBot.Commands.NumRunningInputsCommand", (long)PermissionLevels.User, true, true),
                new CommandData("pressedinputs", "TRBot.Commands.ListPressedInputsCommand", (long)PermissionLevels.User, true, true),
                new CommandData("vcontroller", "TRBot.Commands.VirtualControllerCommand", (long)PermissionLevels.User, true, true),
                new CommandData("userinfo", "TRBot.Commands.UserInfoCommand", (long)PermissionLevels.User, true, true),
                new CommandData("help", "TRBot.Commands.ListCmdsCommand", (long)PermissionLevels.User, true, true),
                new CommandData("controllercount", "TRBot.Commands.ControllerCountCommand", (long)PermissionLevels.User, true, true),
                new CommandData("credits", "TRBot.Commands.CreditsCommand", (long)PermissionLevels.User, true, true),
                new CommandData("highestcredits", "TRBot.Commands.HighestCreditsCommand", (long)PermissionLevels.User, true, true),
                new CommandData("averagecredits", "TRBot.Commands.AverageCreditsCommand", (long)PermissionLevels.User, true, true),
                new CommandData("mediancredits", "TRBot.Commands.MedianCreditsCommand", (long)PermissionLevels.User, true, true),
                new CommandData("time", "TRBot.Commands.TimeCommand", (long)PermissionLevels.User, true, true),
                new CommandData("uptime", "TRBot.Commands.UptimeCommand", (long)PermissionLevels.User, true, true),
                new CommandData("chat", "TRBot.Commands.ChatbotCommand", (long)PermissionLevels.User, true, true),
                new CommandData("bingo", "TRBot.Commands.BingoCommand", (long)PermissionLevels.User, true, true),
                new CommandData("port", "TRBot.Commands.ControllerPortCommand", (long)PermissionLevels.User, true, true),
                new CommandData("optstats", "TRBot.Commands.OptStatsCommand", (long)PermissionLevels.User, true, true),
                new CommandData("length", "TRBot.Commands.InputLengthCommand", (long)PermissionLevels.User, true, true),
                new CommandData("clearstats", "TRBot.Commands.ClearUserStatsCommand", (long)PermissionLevels.User, true, true),
                new CommandData("calculate", "TRBot.Commands.CalculateCommand", (long)PermissionLevels.User, true, true),
                new CommandData("listresinputs", "TRBot.Commands.ListRestrictedInputsCommand", (long)PermissionLevels.User, true, true),
                new CommandData("bet", "TRBot.Commands.BetCreditsCommand", (long)PermissionLevels.User, true, true),
                new CommandData("transfer", "TRBot.Commands.TransferCreditsCommand", (long)PermissionLevels.User, true, true),
                new CommandData("groupbet", "TRBot.Commands.EnterGroupBetCommand", (long)PermissionLevels.User, true, true),
                new CommandData("exitgroupbet", "TRBot.Commands.LeaveGroupBetCommand", (long)PermissionLevels.User, true, true),
                new CommandData("randnum", "TRBot.Commands.RandomNumberCommand", (long)PermissionLevels.User, true, true),
                new CommandData("defaultinputdur", "TRBot.Commands.DefaultInputDurCommand", (long)PermissionLevels.User, true, true),
                new CommandData("maxinputdur", "TRBot.Commands.MaxInputDurCommand", (long)PermissionLevels.User, true, true),
                new CommandData("say", "TRBot.Commands.SayCommand", (long)PermissionLevels.User, true, true),
                new CommandData("reverse", "TRBot.Commands.ReverseCommand", (long)PermissionLevels.User, true, true),
                new CommandData("numlogs", "TRBot.Commands.NumGameLogsCommand", (long)PermissionLevels.User, true, true),
                new CommandData("reverseparse", "TRBot.Commands.ReverseParseInputCommand", (long)PermissionLevels.User, true, true),
                new CommandData("cmdinfo", "TRBot.Commands.CmdInfoCommand", (long)PermissionLevels.User, true, true),
                new CommandData("duel", "TRBot.Commands.DuelCommand", (long)PermissionLevels.User, true, true),
                new CommandData("highfive", "TRBot.Commands.HighFiveCommand", (long)PermissionLevels.User, true, true),
                new CommandData("inspiration", "TRBot.Commands.InspirationCommand", (long)PermissionLevels.User, true, true),
                //By default, exclude the common savestate inputs from input exercises
                new CommandData("exercise", "TRBot.Commands.InputExerciseCommand", (long)PermissionLevels.User, true, true, "ss,incs,decs,ss1,ss2,ss3,ss4,ss5,ss6,ls1,ls2,ls3,ls4,ls5,ls6"),
                new CommandData("slots", "TRBot.Commands.SlotsCommand", (long)PermissionLevels.User, true, true),
                new CommandData("inputperms", "TRBot.Commands.GlobalInputPermissionsCommand", (long)PermissionLevels.User, true, true),

                new CommandData("addlog", "TRBot.Commands.AddGameLogCommand", (long)PermissionLevels.Whitelisted, true, true),

                new CommandData("setmessage", "TRBot.Commands.SetGameMessageCommand", (long)PermissionLevels.VIP, true, true),

                new CommandData("reload", "TRBot.Commands.ReloadCommand", (long)PermissionLevels.Moderator, true, true),
                new CommandData("listsyn", "TRBot.Commands.ListInputSynonymsCommand", (long)PermissionLevels.Moderator, true, true),
                new CommandData("addsyn", "TRBot.Commands.AddInputSynonymCommand", (long)PermissionLevels.Moderator, true, true),
                new CommandData("removesyn", "TRBot.Commands.RemoveInputSynonymCommand", (long)PermissionLevels.Moderator, true, true),
                new CommandData("setlevel", "TRBot.Commands.SetUserLevelCommand", (long)PermissionLevels.Moderator, true, true),
                new CommandData("toggleability", "TRBot.Commands.UpdateUserAbilityCommand", (long)PermissionLevels.Moderator, true, true),
                new CommandData("restrictinput", "TRBot.Commands.AddRestrictedInputCommand", (long)PermissionLevels.Moderator, true, true),
                new CommandData("unrestrictinput", "TRBot.Commands.RemoveRestrictedInputCommand", (long)PermissionLevels.Moderator, true, true),
                new CommandData("teamsmode", "TRBot.Commands.GetSetTeamsModeCommand", (long)PermissionLevels.Moderator, true, true),
                new CommandData("teamsmaxport", "TRBot.Commands.GetSetTeamsModeMaxPortCommand", (long)PermissionLevels.Moderator, true, true),

                new CommandData("addcmd", "TRBot.Commands.AddCmdCommand", (long)PermissionLevels.Admin, true, true),
                new CommandData("removecmd", "TRBot.Commands.RemoveCmdCommand", (long)PermissionLevels.Admin, true, true),
                new CommandData("togglecmd", "TRBot.Commands.ToggleCmdCommand", (long)PermissionLevels.Admin, true, true),
                new CommandData("addconsole", "TRBot.Commands.AddConsoleCommand", (long)PermissionLevels.Admin, true, true),
                new CommandData("removeconsole", "TRBot.Commands.RemoveConsoleCommand", (long)PermissionLevels.Admin, true, true),
                new CommandData("addinput", "TRBot.Commands.AddInputCommand", (long)PermissionLevels.Admin, true, true),
                new CommandData("removeinput", "TRBot.Commands.RemoveInputCommand", (long)PermissionLevels.Admin, true, true),
                new CommandData("setinputlevel", "TRBot.Commands.SetInputLevelCommand", (long)PermissionLevels.Admin, true, true),
                new CommandData("toggleinput", "TRBot.Commands.SetInputEnabledCommand", (long)PermissionLevels.Admin, true, true),

                new CommandData("exec", "TRBot.Commands.ExecCommand", (long)PermissionLevels.Superadmin, false, false),
                new CommandData("exportbotdata", "TRBot.Commands.ExportBotDataCommand", (long)PermissionLevels.Superadmin, true, true),
                new CommandData("forceinitdefaults", "TRBot.Commands.ForceInitDataCommand", (long)PermissionLevels.Superadmin, true, true),
            };

            return defaultCommands;
        }

        /// <summary>
        /// Returns a list of default consoles.
        /// </summary>
        /// <returns>A list of GameConsoles.</returns>
        public static List<GameConsole> GetDefaultConsoles()
        {
            List<GameConsole> defaultConsoles = new List<GameConsole>()
            {
                new NESConsole(),
                new SNESConsole(),
                new GenesisConsole(),
                new N64Console(),
                new PS2Console(), new GCConsole(), new GBAConsole(),
                new WiiConsole(), new PS4Console(),
                new SwitchConsole(),
                new PCConsole(),
            };

            return defaultConsoles;
        }

        /// <summary>
        /// Returns a list of default permission abilities.
        /// </summary>
        /// <returns>A list of PermissionAbilities.</returns>
        public static List<PermissionAbility> GetDefaultPermAbilities()
        {
            List<PermissionAbility> defaultPermissions = new List<PermissionAbility>()
            {
                new PermissionAbility(BET_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),
                new PermissionAbility(DUEL_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),
                new PermissionAbility(GROUP_BET_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),
                new PermissionAbility(JUMP_ROPE_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),
                new PermissionAbility(FEED_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),
                new PermissionAbility(INPUT_EXERCISE_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),
                new PermissionAbility(CALCULATE_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),
                new PermissionAbility(CHATBOT_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),
                new PermissionAbility(BINGO_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),
                new PermissionAbility(TRANSFER_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),
                new PermissionAbility(SLOTS_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),

                new PermissionAbility(SET_GAME_MESSAGE_ABILITY, PermissionLevels.VIP, PermissionLevels.VIP),

                new PermissionAbility(SET_CONSOLE_ABILITY, PermissionLevels.Moderator, PermissionLevels.Moderator),
                new PermissionAbility(SET_DEFAULT_INPUT_DUR_ABILITY, PermissionLevels.Moderator, PermissionLevels.Moderator),
                new PermissionAbility(SET_MAX_INPUT_DUR_ABILITY, PermissionLevels.Moderator, PermissionLevels.Moderator),
                new PermissionAbility(SET_TEAMS_MODE_ABILITY, PermissionLevels.Moderator, PermissionLevels.Moderator),
                new PermissionAbility(SET_TEAMS_MODE_MAX_PORT_ABILITY, PermissionLevels.Moderator, PermissionLevels.Moderator),
                
                new PermissionAbility(UPDATE_OTHER_USER_ABILITES, PermissionLevels.Admin, PermissionLevels.Admin),
                new PermissionAbility(SET_GLOBAL_INPUT_LEVEL_ABILITY, PermissionLevels.Admin, PermissionLevels.Admin),
                new PermissionAbility(SET_VCONTROLLER_TYPE_ABILITY, PermissionLevels.Admin, PermissionLevels.Admin),
                new PermissionAbility(SET_VCONTROLLER_COUNT_ABILITY, PermissionLevels.Admin, PermissionLevels.Admin),

                PermissionAbility.CreateWithMinLvlGrant(SILENCED_ABILITY, PermissionLevels.Moderator),
                PermissionAbility.CreateWithMinLvlGrant(USER_DEFAULT_INPUT_DIR_ABILITY, PermissionLevels.Moderator),
                PermissionAbility.CreateWithMinLvlGrant(USER_MAX_INPUT_DIR_ABILITY, PermissionLevels.Moderator),
            };

            return defaultPermissions;
        }

        #endregion
    }
}
