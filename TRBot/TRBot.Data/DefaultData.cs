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
using TRBot.VirtualControllers;
using static TRBot.Data.SettingsConstants;

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
                SettingsHelper(CHATBOT_ENABLED, false),
                SettingsHelper(CHATBOT_SOCKET_PATH, "ChatterBotSocket"),
                SettingsHelper(CHATBOT_SOCKET_PATH_IS_RELATIVE, true),
                SettingsHelper(BINGO_ENABLED, false),
                SettingsHelper(BINGO_PIPE_PATH, "BingoPipe"),
                SettingsHelper(BINGO_PIPE_PATH_IS_RELATIVE, true),
                SettingsHelper(CLIENT_SERVICE_TYPE, (long)ClientServiceTypes.Twitch),
                SettingsHelper(AUTO_PROMOTE_ENABLED, true),
                SettingsHelper(AUTO_PROMOTE_LEVEL, 1L),
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
                SettingsHelper(GAME_MESSAGE, "Message"),
                SettingsHelper(INFO_MESSAGE, "Welcome to the channel! You can play games by submitting messages in chat. Type !inputs to see all available buttons."),
                SettingsHelper(DEFAULT_INPUT_DURATION, 200L),
                SettingsHelper(MAX_INPUT_DURATION, 60000L),
                SettingsHelper(LAST_CONSOLE, 1L),
                SettingsHelper(LAST_VCONTROLLER_TYPE, (long)VirtualControllerTypes.vJoy),
                SettingsHelper(JOYSTICK_COUNT, 1L),
                SettingsHelper(FIRST_LAUNCH, 1L)
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
                new CommandData("sourcecode", "TRBot.Commands.MessageCommand", 0, true, true, SettingsConstants.SOURCE_CODE_MESSAGE),
                new CommandData("info", "TRBot.Commands.MessageCommand", 0, true, true, SettingsConstants.INFO_MESSAGE),
                new CommandData("reload", "TRBot.Commands.ReloadCommand", 3, true, true),
                new CommandData("addcmd", "TRBot.Commands.AddCmdCommand", 4, true, true),
                new CommandData("removecmd", "TRBot.Commands.RemoveCmdCommand", 4, true, true),
                new CommandData("version", "TRBot.Commands.VersionCommand", 0, true, true),
                new CommandData("addconsole", "TRBot.Commands.AddConsoleCommand", 4, true, true),
                new CommandData("removeconsole", "TRBot.Commands.RemoveConsoleCommand", 4, true, true),
                new CommandData("console", "TRBot.Commands.GetSetConsoleCommand", 0, true, true),
                new CommandData("inputs", "TRBot.Commands.InputInfoCommand", 0, true, true),
                new CommandData("addinput", "TRBot.Commands.AddInputCommand", 4, true, true),
                new CommandData("removeinput", "TRBot.Commands.RemoveInputCommand", 4, true, true),
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
                new WiiConsole(), new PCConsole()
            };

            return defaultConsoles;
        }

        #endregion
    }
}
