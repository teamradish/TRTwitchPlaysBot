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
using System.Text;

namespace TRBotDataMigrationTool
{
    public class Settings
{
        public ClientSettings ClientSettings = null;
        public MessageSettings MsgSettings = null;
        public BingoSettings BingoSettings = null;
    
        public double CreditsTime = 2d;
        public long CreditsAmount = 100L;
    
        /// <summary>
        /// The character limit for bot messages. The default is the client service's character limit (Ex. Twitch).
        /// <para>Some messages that naturally go over this limit will be split into multiple messages.
        /// Examples include listing memes and macros.</para>
        /// </summary>
        public int BotMessageCharLimit = 500;
        
        /// <summary>
        /// How long to make the main thread sleep after each iteration.
        /// Higher values use less CPU at the expense of delaying queued messages and routines.
        /// </summary>
        public int MainThreadSleep = 100;

        /// <summary>
        /// If true, automatically whitelists users if conditions are met, including the command count.
        /// </summary>
        public bool AutoWhitelistEnabled = false;
    
        /// <summary>
        /// The number of valid inputs required to whitelist a user if they're not whitelisted and auto whitelist is enabled.
        /// </summary>
        public int AutoWhitelistInputCount = 20;
    
        /// <summary>
        /// If true, will acknowledge that a chat bot is in use and allow interacting with it, provided it's set up.
        /// </summary>
        public bool UseChatBot = false;
    
        /// <summary>
        /// The name of the file for the chatbot's socket in the data directory.
        /// </summary>
        public string ChatBotSocketFilename = "ChatterBotSocket";
    }
}