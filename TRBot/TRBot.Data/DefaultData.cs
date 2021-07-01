/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, version 3 of the License.
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
                SettingsHelper(CHATBOT_SOCKET_PATH, Path.Combine(Path.GetTempPath(), "ChatterBotSocket")),
                SettingsHelper(CHATBOT_SOCKET_PATH_IS_RELATIVE, false),
                SettingsHelper(BINGO_ENABLED, false),
                SettingsHelper(BINGO_PIPE_PATH, Path.Combine(Path.GetTempPath(), "BingoPipe")),
                SettingsHelper(BINGO_PIPE_PATH_IS_RELATIVE, false),
                SettingsHelper(CLIENT_SERVICE_TYPE, (long)ClientServiceTypes.Twitch),
                SettingsHelper(LOG_LEVEL, (long)Serilog.Events.LogEventLevel.Information),
                SettingsHelper(AUTO_PROMOTE_ENABLED, true),
                SettingsHelper(AUTO_PROMOTE_LEVEL, (long)PermissionLevels.Whitelisted),
                SettingsHelper(AUTO_PROMOTE_INPUT_REQ, 50L),
                SettingsHelper(BOT_MSG_CHAR_LIMIT, 500L),
                SettingsHelper(PERIODIC_MSG_TIME, 1800000L),
                SettingsHelper(MESSAGE_THROTTLE_TYPE, (long)MessageThrottlingOptions.MsgCountPerInterval),
                SettingsHelper(MESSAGE_COOLDOWN, 30000L),
                SettingsHelper(MESSAGE_THROTTLE_COUNT, 20L),
                SettingsHelper(MESSAGE_PREFIX, string.Empty),
                SettingsHelper(RECONNECT_TIME, 5000L),
                SettingsHelper(CONNECT_MESSAGE, "Your friendly Twitch Plays bot has connected :D ! Use !inputs for all buttons, !tutorial to learn how to play, and !help for a list of bot commands! Original input parser by Jdog, aka TwitchPlays_Everything, rewritten and improved over time by the community."),
                SettingsHelper(RECONNECTED_MESSAGE, "Successfully reconnected to chat!"),
                SettingsHelper(PERIODIC_MESSAGE, "Hi! I'm your friendly Twitch Plays bot :D ! Use !inputs for all buttons, !tutorial for how to play, and !help for a list of bot commands!"),
                SettingsHelper(AUTOPROMOTE_MESSAGE, "{0} has been promoted to {1}! New commands and permissions are available!"),
                SettingsHelper(NEW_USER_MESSAGE, "Welcome to the stream, {0} :D ! We hope you enjoy your stay!"),
                SettingsHelper(BEING_HOSTED_MESSAGE, "Thank you for hosting, {0}!! You rock!"),
                SettingsHelper(NEW_SUBSCRIBER_MESSAGE, "Thank you for subscribing, {0} :D !!"),
                SettingsHelper(RESUBSCRIBER_MESSAGE, "Thank you for subscribing for {1} months, {0} :D !!"),
                SettingsHelper(SOURCE_CODE_MESSAGE, "This bot is free software licensed under the AGPL v3.0. The code repository and full license terms are at https://codeberg.org/kimimaru/TRBot - You have the right to obtain source code for the streamer's deployed version of the software."),
                SettingsHelper(PERIODIC_MESSAGE_ROTATION, PERIODIC_MESSAGE),
                SettingsHelper(GAME_MESSAGE_PATH, Path.Combine(DataConstants.DATA_FOLDER_NAME, "GameMessage.txt")),
                SettingsHelper(GAME_MESSAGE_PATH_IS_RELATIVE, true),
                SettingsHelper(INFO_MESSAGE, "Welcome to the channel! You can play games by submitting messages in chat. Type !inputs to see all available buttons."),
                SettingsHelper(TUTORIAL_MESSAGE, "Hi {0}, here's how to play: https://codeberg.org/kimimaru/TRBot/src/branch/master/Wiki/Syntax-Walkthrough.md"),
                SettingsHelper(DOCUMENTATION_MESSAGE, "Hi {0}, here's documentation on TRBot: https://codeberg.org/kimimaru/TRBot/src/branch/master/Wiki/Home.md"),
                SettingsHelper(DONATE_MESSAGE, "You can further support TRBot's development by donating to the developer. All donations go towards improving TRBot: https://liberapay.com/kimimaru/"),
                SettingsHelper(SLOTS_BLANK_EMOTE, "FailFish"),
                SettingsHelper(SLOTS_CHERRY_EMOTE, "Kappa"),
                SettingsHelper(SLOTS_PLUM_EMOTE, "HeyGuys"),
                SettingsHelper(SLOTS_WATERMELON_EMOTE, "SeemsGood"),
                SettingsHelper(SLOTS_ORANGE_EMOTE, "CoolCat"),
                SettingsHelper(SLOTS_LEMON_EMOTE, "PartyTime"),
                SettingsHelper(SLOTS_BAR_EMOTE, "PogChamp"),
                SettingsHelper(PERIODIC_INPUT_ENABLED, 0L),
                SettingsHelper(PERIODIC_INPUT_TIME, 1000 * 60 * 5),
                SettingsHelper(PERIODIC_INPUT_PORT, 0L),
                SettingsHelper(PERIODIC_INPUT_VALUE, string.Empty),
                SettingsHelper(TEAMS_MODE_ENABLED, 0L),
                SettingsHelper(TEAMS_MODE_MAX_PORT, 3L),
                SettingsHelper(TEAMS_MODE_NEXT_PORT, 0L),
                SettingsHelper(DEFAULT_INPUT_DURATION, 200L),
                SettingsHelper(MAX_INPUT_DURATION, 60000L),
                SettingsHelper(GLOBAL_MID_INPUT_DELAY_ENABLED, 0L),
                SettingsHelper(GLOBAL_MID_INPUT_DELAY_TIME, 34L),
                SettingsHelper(MAX_USER_RECENT_INPUTS, 5L),
                SettingsHelper(MAX_USER_SIMULATE_STRING_LENGTH, 30000L),
                SettingsHelper(USER_SIMULATE_CREDIT_COST, 1000L),
                SettingsHelper(DEMOCRACY_VOTE_TIME, 10000L),
                SettingsHelper(DEMOCRACY_RESOLUTION_MODE, (long)DemocracyResolutionModes.ExactSequence),
                SettingsHelper(INPUT_MODE_VOTE_TIME, 60000L),
                SettingsHelper(INPUT_MODE_CHANGE_COOLDOWN, 1000L * 60L * 15L),
                SettingsHelper(INPUT_MODE_NEXT_VOTE_DATE, DataHelper.GetStrFromDateTime(DateTime.UnixEpoch)),
                SettingsHelper(LAST_CONSOLE, 1L),
                SettingsHelper(LAST_VCONTROLLER_TYPE, (long)VControllerHelper.GetDefaultVControllerTypeForPlatform(TRBotOSPlatform.CurrentOS)),
                SettingsHelper(JOYSTICK_COUNT, 1L),
                SettingsHelper(GLOBAL_INPUT_LEVEL, (long)PermissionLevels.User),
                SettingsHelper(INPUT_MODE, (long)InputModes.Anarchy),
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
            long userPerm = (long)PermissionLevels.User;
            long whitelistedPerm = (long)PermissionLevels.Whitelisted;
            long vipPerm = (long)PermissionLevels.VIP;
            long modPerm = (long)PermissionLevels.Moderator;
            long adminPerm = (long)PermissionLevels.Admin;
            long superAdminPerm = (long)PermissionLevels.Superadmin;

            List<CommandData> defaultCommands = new List<CommandData>()
            {
                new CommandData("sourcecode", "TRBot.Commands.MessageCommand", userPerm, true, true, SettingsConstants.SOURCE_CODE_MESSAGE),
                new CommandData("info", "TRBot.Commands.MessageCommand", userPerm, true, true, SettingsConstants.INFO_MESSAGE),
                new CommandData("tutorial", "TRBot.Commands.MessageCommand", userPerm, true, true, SettingsConstants.TUTORIAL_MESSAGE),
                new CommandData("documentation", "TRBot.Commands.MessageCommand", userPerm, true, true, SettingsConstants.DOCUMENTATION_MESSAGE),
                new CommandData("donate", "TRBot.Commands.MessageCommand", userPerm, true, true, SettingsConstants.DONATE_MESSAGE),
                new CommandData("version", "TRBot.Commands.VersionCommand", userPerm, true, true),
                new CommandData("console", "TRBot.Commands.GetSetConsoleCommand", userPerm, true, true),
                new CommandData("inputs", "TRBot.Commands.InputInfoCommand", userPerm, true, true),
                new CommandData("macros", "TRBot.Commands.ListMacrosCommand", userPerm, true, true),
                new CommandData("addmacro", "TRBot.Commands.AddMacroCommand", userPerm, true, true),
                new CommandData("removemacro", "TRBot.Commands.RemoveMacroCommand", userPerm, true, true),
                new CommandData("showmacro", "TRBot.Commands.ShowMacroCommand", userPerm, true, true),
                new CommandData("memes", "TRBot.Commands.ListMemesCommand", userPerm, true, true),
                new CommandData("addmeme", "TRBot.Commands.AddMemeCommand", userPerm, true, true),
                new CommandData("removememe", "TRBot.Commands.RemoveMemeCommand", userPerm, true, true),
                new CommandData("viewlog", "TRBot.Commands.ViewGameLogCommand", userPerm, true, true),
                new CommandData("stopall", "TRBot.Commands.StopAllInputsCommand", userPerm, true, true),
                new CommandData("allabilities", "TRBot.Commands.ListPermissionAbilitiesCommand", userPerm, true, true),
                new CommandData("userabilities", "TRBot.Commands.ListUserAbilitiesCommand", userPerm, true, true),
                new CommandData("updateabilities", "TRBot.Commands.UpdateAllUserAbilitiesCommand", userPerm, true, true),
                new CommandData("level", "TRBot.Commands.LevelCommand", userPerm, true, true),
                new CommandData("runninginputs", "TRBot.Commands.NumRunningInputsCommand", userPerm, true, true),
                new CommandData("pressedinputs", "TRBot.Commands.ListPressedInputsCommand", userPerm, true, true),
                new CommandData("vcontroller", "TRBot.Commands.VirtualControllerCommand", userPerm, true, true),
                new CommandData("userinfo", "TRBot.Commands.UserInfoCommand", userPerm, true, true),
                new CommandData("help", "TRBot.Commands.ListCmdsCommand", userPerm, true, true),
                new CommandData("controllercount", "TRBot.Commands.ControllerCountCommand", userPerm, true, true),
                new CommandData("credits", "TRBot.Commands.CreditsCommand", userPerm, true, true),
                new CommandData("averagecredits", "TRBot.Commands.AverageCreditsCommand", userPerm, true, false),
                new CommandData("mediancredits", "TRBot.Commands.MedianCreditsCommand", userPerm, true, false),
                new CommandData("leaderboard", "TRBot.Commands.CreditLeaderboardCommand", userPerm, true, false),
                new CommandData("time", "TRBot.Commands.TimeCommand", userPerm, true, true),
                new CommandData("uptime", "TRBot.Commands.UptimeCommand", userPerm, true, true),
                new CommandData("chat", "TRBot.Commands.ChatbotCommand", userPerm, true, false),
                new CommandData("bingo", "TRBot.Commands.BingoCommand", userPerm, true, false),
                new CommandData("port", "TRBot.Commands.ControllerPortCommand", userPerm, true, true),
                new CommandData("optstats", "TRBot.Commands.OptStatsCommand", userPerm, true, true),
                new CommandData("ignorememes", "TRBot.Commands.IgnoreMemesCommand", userPerm, true, true),
                new CommandData("length", "TRBot.Commands.InputLengthCommand", userPerm, true, true),
                new CommandData("clearstats", "TRBot.Commands.ClearUserStatsCommand", userPerm, true, true),
                new CommandData("calculate", "TRBot.Commands.CalculateCommand", userPerm, true, true),
                new CommandData("listresinputs", "TRBot.Commands.ListRestrictedInputsCommand", userPerm, true, true),
                new CommandData("bet", "TRBot.Commands.BetCreditsCommand", userPerm, true, true),
                new CommandData("transfer", "TRBot.Commands.TransferCreditsCommand", userPerm, true, true),
                new CommandData("groupbet", "TRBot.Commands.EnterGroupBetCommand", userPerm, true, true),
                new CommandData("exitgroupbet", "TRBot.Commands.LeaveGroupBetCommand", userPerm, true, true),
                new CommandData("randnum", "TRBot.Commands.RandomNumberCommand", userPerm, true, false),
                new CommandData("defaultinputdur", "TRBot.Commands.DefaultInputDurCommand", userPerm, true, true),
                new CommandData("maxinputdur", "TRBot.Commands.MaxInputDurCommand", userPerm, true, true),
                new CommandData("say", "TRBot.Commands.SayCommand", userPerm, true, true),
                new CommandData("reverse", "TRBot.Commands.ReverseCommand", userPerm, true, true),
                new CommandData("numlogs", "TRBot.Commands.NumGameLogsCommand", userPerm, true, true),
                new CommandData("reverseparse", "TRBot.Commands.ReverseParseInputCommand", userPerm, true, true),
                new CommandData("cmdinfo", "TRBot.Commands.CmdInfoCommand", userPerm, true, true),
                new CommandData("duel", "TRBot.Commands.DuelCommand", userPerm, true, true),
                new CommandData("highfive", "TRBot.Commands.HighFiveCommand", userPerm, true, true),
                new CommandData("inspiration", "TRBot.Commands.InspirationCommand", userPerm, true, true),
                //By default, exclude the common savestate inputs from input exercises
                new CommandData("exercise", "TRBot.Commands.InputExerciseCommand", userPerm, true, true, "ss,incs,decs,ss1,ss2,ss3,ss4,ss5,ss6,ls1,ls2,ls3,ls4,ls5,ls6"),
                new CommandData("slots", "TRBot.Commands.SlotsCommand", userPerm, true, true),
                new CommandData("inputperms", "TRBot.Commands.GlobalInputPermissionsCommand", userPerm, true, true),
                new CommandData("midinputdelay", "TRBot.Commands.MidInputDelayCommand", userPerm, true, true),
                new CommandData("listsyn", "TRBot.Commands.ListInputSynonymsCommand", userPerm, true, true),
                new CommandData("teamsmode", "TRBot.Commands.GetSetTeamsModeCommand", modPerm, true, true),
                new CommandData("teamsmaxport", "TRBot.Commands.GetSetTeamsModeMaxPortCommand", modPerm, true, false),
                new CommandData("periodicinput", "TRBot.Commands.TogglePeriodicInputCommand", userPerm, true, true),
                new CommandData("periodicinputport", "TRBot.Commands.GetSetPeriodicInputPortCommand", userPerm, true, false),
                new CommandData("periodicinputseq", "TRBot.Commands.GetSetPeriodicInputSequenceCommand", userPerm, true, true),
                new CommandData("periodicinputtime", "TRBot.Commands.GetSetPeriodicInputTimeCommand", userPerm, true, false),
                new CommandData("invalidcombo", "TRBot.Commands.ListInvalidInputComboCommand", userPerm, true, true),
                new CommandData("recentinput", "TRBot.Commands.ListUserRecentInputsCommand", userPerm, true, true),
                new CommandData("recentinputcount", "TRBot.Commands.GetSetMaxUserRecentInputsCommand", userPerm, true, true),
                new CommandData("inputmode", "TRBot.Commands.GetSetInputModeCommand", userPerm, true, true),
                new CommandData("dresmode", "TRBot.Commands.GetSetDemocracyResModeCommand", userPerm, true, true),
                new CommandData("dvotetime", "TRBot.Commands.GetSetDemocracyVoteTimeCommand", userPerm, true, true),
                new CommandData("vote", "TRBot.Commands.VoteForInputModeCommand", userPerm, true, true),
                new CommandData("votetime", "TRBot.Commands.GetSetInputModeVoteTimeCommand", userPerm, true, false),
                new CommandData("votecooldown", "TRBot.Commands.GetSetInputModeCooldownCommand", userPerm, true, false),
                new CommandData("userdefaultinputdur", "TRBot.Commands.UserDefaultInputDurCommand", userPerm, true, false),
                new CommandData("usermaxinputdur", "TRBot.Commands.UserMaxInputDurCommand", userPerm, true, false),
                new CommandData("listsilenced", "TRBot.Commands.ListSilencedUsersCommand", userPerm, true, true),
                new CommandData("simulate", "TRBot.Commands.UserSimulateCommand", userPerm, true, true),

                new CommandData("addlog", "TRBot.Commands.AddGameLogCommand", whitelistedPerm, true, true),
                new CommandData("addsyn", "TRBot.Commands.AddInputSynonymCommand", whitelistedPerm, true, true),
                new CommandData("removesyn", "TRBot.Commands.RemoveInputSynonymCommand", whitelistedPerm, true, true),
                new CommandData("viewmultilogs", "TRBot.Commands.ViewMultipleGameLogsCommand", whitelistedPerm, true, true),

                new CommandData("setmessage", "TRBot.Commands.SetGameMessageCommand", vipPerm, true, true, SettingsConstants.GAME_MESSAGE_PATH),

                new CommandData("reload", "TRBot.Commands.ReloadCommand", modPerm, true, true),
                new CommandData("setlevel", "TRBot.Commands.SetUserLevelCommand", modPerm, true, true),
                new CommandData("toggleability", "TRBot.Commands.UpdateUserAbilityCommand", modPerm, true, true),
                new CommandData("restrictinput", "TRBot.Commands.AddRestrictedInputCommand", modPerm, true, true),
                new CommandData("unrestrictinput", "TRBot.Commands.RemoveRestrictedInputCommand", modPerm, true, true),
                new CommandData("addinvalidcombo", "TRBot.Commands.AddInvalidInputComboCommand", modPerm, true, true),
                new CommandData("removeinvalidcombo", "TRBot.Commands.RemoveInvalidInputComboCommand", modPerm, true, true),
                new CommandData("userabilitylvloverride", "TRBot.Commands.GetSetUserAbilityLvlOverrideCommand", modPerm, true, true),
                new CommandData("silence", "TRBot.Commands.SilenceUserCommand", modPerm, true, true),
                new CommandData("unsilence", "TRBot.Commands.UnsilenceUserCommand", modPerm, true, true),

                new CommandData("addcmd", "TRBot.Commands.AddCmdCommand", adminPerm, true, true),
                new CommandData("removecmd", "TRBot.Commands.RemoveCmdCommand", adminPerm, true, true),
                new CommandData("togglecmd", "TRBot.Commands.ToggleCmdCommand", adminPerm, true, true),
                new CommandData("addconsole", "TRBot.Commands.AddConsoleCommand", adminPerm, true, true),
                new CommandData("removeconsole", "TRBot.Commands.RemoveConsoleCommand", adminPerm, true, true),
                new CommandData("addinput", "TRBot.Commands.AddInputCommand", adminPerm, true, true),
                new CommandData("removeinput", "TRBot.Commands.RemoveInputCommand", adminPerm, true, true),
                new CommandData("setinputlevel", "TRBot.Commands.SetInputLevelCommand", adminPerm, true, true),
                new CommandData("toggleinput", "TRBot.Commands.SetInputEnabledCommand", adminPerm, true, true),
                new CommandData("updateeveryoneabilities", "TRBot.Commands.UpdateEveryoneAbilitiesCommand", adminPerm, true, true),

                new CommandData("exec", "TRBot.Commands.ExecCommand", superAdminPerm, false, true),
                new CommandData("exportbotdata", "TRBot.Commands.ExportBotDataCommand", superAdminPerm, true, true),
                new CommandData("exportdataset", "TRBot.Commands.ExportDatasetToTextCommand", superAdminPerm, true, true),
                new CommandData("cleardataset", "TRBot.Commands.ClearDatasetCommand", superAdminPerm, false, true),
                new CommandData("forceinitdefaults", "TRBot.Commands.ForceInitDataCommand", superAdminPerm, true, true),
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
                new N64Console(), new GBCConsole(),
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
                new PermissionAbility(INPUT_EXERCISE_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),
                new PermissionAbility(CALCULATE_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),
                new PermissionAbility(CHATBOT_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),
                new PermissionAbility(BINGO_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),
                new PermissionAbility(TRANSFER_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),
                new PermissionAbility(SLOTS_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),
                new PermissionAbility(VOTE_INPUT_MODE_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),
                new PermissionAbility(ADD_INPUT_MACRO_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),
                new PermissionAbility(REMOVE_INPUT_MACRO_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),
                new PermissionAbility(ADD_MEME_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),
                new PermissionAbility(REMOVE_MEME_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),
                new PermissionAbility(ADD_INPUT_SYNONYM_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),
                new PermissionAbility(REMOVE_INPUT_SYNONYM_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),
                new PermissionAbility(STOP_ALL_INPUTS_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),
                new PermissionAbility(SIMULATE_ABILITY, PermissionLevels.User, PermissionLevels.Moderator),

                new PermissionAbility(SET_GAME_MESSAGE_ABILITY, PermissionLevels.VIP, PermissionLevels.VIP),

                new PermissionAbility(SET_CONSOLE_ABILITY, PermissionLevels.Moderator, PermissionLevels.Moderator),
                new PermissionAbility(SET_DEFAULT_INPUT_DUR_ABILITY, PermissionLevels.Moderator, PermissionLevels.Moderator),
                new PermissionAbility(SET_MAX_INPUT_DUR_ABILITY, PermissionLevels.Moderator, PermissionLevels.Moderator),
                new PermissionAbility(SET_MID_INPUT_DELAY_ABILITY, PermissionLevels.Moderator, PermissionLevels.Moderator),
                new PermissionAbility(SET_TEAMS_MODE_ABILITY, PermissionLevels.Moderator, PermissionLevels.Moderator),
                new PermissionAbility(SET_TEAMS_MODE_MAX_PORT_ABILITY, PermissionLevels.Moderator, PermissionLevels.Moderator),
                new PermissionAbility(SET_PERIODIC_INPUT_ABILITY, PermissionLevels.Moderator, PermissionLevels.Moderator),
                new PermissionAbility(SET_PERIODIC_INPUT_PORT_ABILITY, PermissionLevels.Moderator, PermissionLevels.Moderator),
                new PermissionAbility(SET_PERIODIC_INPUT_TIME_ABILITY, PermissionLevels.Moderator, PermissionLevels.Moderator),
                new PermissionAbility(SET_PERIODIC_INPUT_SEQUENCE_ABILITY, PermissionLevels.Moderator, PermissionLevels.Moderator),
                new PermissionAbility(SET_MAX_USER_RECENT_INPUTS_ABILITY, PermissionLevels.Moderator, PermissionLevels.Moderator),
                new PermissionAbility(START_VOTE_INPUT_MODE_ABILITY, PermissionLevels.Moderator, PermissionLevels.Moderator),

                new PermissionAbility(UPDATE_OTHER_USER_ABILITES, PermissionLevels.Admin, PermissionLevels.Admin),
                new PermissionAbility(SET_GLOBAL_INPUT_LEVEL_ABILITY, PermissionLevels.Admin, PermissionLevels.Admin),
                new PermissionAbility(SET_VCONTROLLER_TYPE_ABILITY, PermissionLevels.Admin, PermissionLevels.Admin),
                new PermissionAbility(SET_VCONTROLLER_COUNT_ABILITY, PermissionLevels.Admin, PermissionLevels.Admin),
                new PermissionAbility(SET_DEMOCRACY_VOTE_TIME_ABILITY, PermissionLevels.Admin, PermissionLevels.Admin),
                new PermissionAbility(SET_DEMOCRACY_RESOLUTION_MODE_ABILITY, PermissionLevels.Admin, PermissionLevels.Admin),
                new PermissionAbility(SET_INPUT_MODE_ABILITY, PermissionLevels.Admin, PermissionLevels.Admin),
                new PermissionAbility(SET_INPUT_MODE_VOTE_TIME_ABILITY, PermissionLevels.Admin, PermissionLevels.Admin),
                new PermissionAbility(SET_INPUT_MODE_CHANGE_COOLDOWN_ABILITY, PermissionLevels.Admin, PermissionLevels.Admin),

                PermissionAbility.CreateWithMinLvlGrant(SILENCED_ABILITY, PermissionLevels.Moderator),
                PermissionAbility.CreateWithMinLvlGrant(USER_DEFAULT_INPUT_DUR_ABILITY, PermissionLevels.Moderator),
                PermissionAbility.CreateWithMinLvlGrant(USER_MAX_INPUT_DUR_ABILITY, PermissionLevels.Moderator),
                PermissionAbility.CreateWithMinLvlGrant(USER_MID_INPUT_DELAY_ABILITY, PermissionLevels.User),
            };

            return defaultPermissions;
        }

        #endregion
    }
}
