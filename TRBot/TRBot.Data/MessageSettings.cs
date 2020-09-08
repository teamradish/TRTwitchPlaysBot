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

namespace TRBot.Data
{
    /// <summary>
    /// Various message settings.
    /// </summary>
    public sealed class MessageSettings
    {
        /// <summary>
        /// The character limit for bot messages.
        /// <para>Some messages that naturally go over this limit may be split into multiple messages.
        /// Examples include listing memes and macros.</para>
        /// </summary>
        public int BotMessageCharLimit = 250;

        /// <summary>
        /// The time, in milliseconds, for outputting the periodic message.
        /// </summary>
        public double PeriodicMessageTime = 1800000d;

        /// <summary>
        /// The time, in milliseconds, before each queued message will be sent.
        /// This is used as a form of rate limiting.
        /// </summary>
        public double MessageCooldown = 1000d;

        /// <summary>
        /// The message to send when the bot connects to a channel. "{0}" is replaced with the name of the bot and "{1}" is replaced with the command identifier.
        /// </summary>
        /// <para>Set empty to display no message upon connecting.</para>
        public string ConnectMessage = "{0} has connected :D ! Use {1}help to display a list of commands and {1}tutorial to see how to play! Original input parser by Jdog, aka TwitchPlays_Everything, converted to C# & improved by the community.";

        /// <summary>
        /// The message to send when the bot reconnects to chat.
        /// </summary>
        public string ReconnectedMsg = "Successfully reconnected to chat!";

        /// <summary>
        /// The message to send periodically according to <see cref="MessageTime"/>.
        /// "{0}" is replaced with the name of the bot and "{1}" is replaced with the command identifier.
        /// </summary>
        /// <para>Set empty to display no messages in the interval.</para>
        public string PeriodicMessage = "Hi! I'm {0} :D ! Use {1}help to display a list of commands!";

        /// <summary>
        /// The message to send when a user is auto whitelisted. "{0}" is replaced with the name of the user whitelisted.
        /// </summary>
        public string AutoWhitelistMsg = "{0} has been whitelisted! New commands are available.";

        /// <summary>
        /// The message to send when a new user is added to the database.
        /// "{0}" is replaced with the name of the user and "{1}" is replaced with the command identifier.
        /// </summary>
        public string NewUserMsg = "Welcome to the stream, {0} :D ! We hope you enjoy your stay!";

        /// <summary>
        /// The message to send when another channel hosts the one the bot is on.
        /// "{0}" is replaced with the name of the channel hosting the one the bot is on.
        /// </summary>
        public string BeingHostedMsg = "Thank you for hosting, {0}!!";

        /// <summary>
        /// The message to send when a user newly subscribes to the channel.
        /// "{0}" is replaced with the name of the subscriber.
        /// </summary>
        public string NewSubscriberMsg = "Thank you for subscribing, {0} :D !!";

        /// <summary>
        /// The message to send when a user resubscribes to the channel.
        /// "{0}" is replaced with the name of the subscriber and "{1}" is replaced with the number of months subscribed for.
        /// </summary>
        public string ReSubscriberMsg = "Thank you for subscribing for {1} months, {0} :D !!";
    }
}
