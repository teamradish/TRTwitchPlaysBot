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
using Newtonsoft.Json;

namespace TRBot.Data
{
    /// <summary>
    /// Represents a user object.
    /// </summary>
    public class User
    {
        public string Name;
        public int Level;
        public long Credits;
        public long TotalMessages;
        public long ValidInputs;
        public bool Silenced;
        public int Port;
        public int Subscriber;
        public int BetCounter;
        public bool AutoWhitelisted = false;
        public bool OptedOut = false;

        /// <summary>
        /// Adds to the user's credits.
        /// </summary>
        /// <param name="count">The number of credits to add.</param>
        public void AddCredits(in long count)
        {
            Credits += count;
        }

        /// <summary>
        /// Subtracts from the user's credits.
        /// </summary>
        /// <param name="count">The number of credits to subtract.</param>
        public void SubtractCredits(in long count)
        {
            Credits -= count;
        }

        /// <summary>
        /// Increments the number of valid inputs the user made.
        /// </summary>
        public void IncrementValidInputCount()
        {
            ValidInputs += 1L;
        }

        /// <summary>
        /// Increments the number of messages the user sent.
        /// </summary>
        public void IncrementMsgCount()
        {
            TotalMessages += 1L;
        }

        /// <summary>
        /// Sets the silenced state of the user.
        /// </summary>
        /// <param name="silenced">The silenced state of the user.</param>
        public void SetSilenced(in bool silenced)
        {
            Silenced = silenced;
        }

        /// <summary>
        /// Sets the user's access level.
        /// </summary>
        /// <param name="level">The level.</param>
        public void SetLevel(in int level)
        {
            Level = level;
        }

        /// <summary>
        /// Sets the user's default controller port they're controlling.
        /// </summary>
        /// <param name="port">The port number.</param>
        public void SetPort(in int port)
        {
            Port = port;
        }

        /// <summary>
        /// Sets the user's auto whitelisted status.
        /// This is only applicable if <see cref=BotProgram.Settings.AutoWhitelistEnabled> is true. 
        /// </summary>
        /// <param name="autoWhitelisted">Whether the user was automatically whitelisted or not.</param>
        public void SetAutoWhitelist(in bool autoWhitelisted)
        {
            AutoWhitelisted = autoWhitelisted;
        }

        /// <summary>
        /// Sets the user's opt out state for bot stats.
        /// </summary>
        /// <param name="optedOut">The opt out state.</param>
        /// <remarks>For simplicity, this currently doesn't factor in existing processes, such as ongoing duels or group bets.</remarks>
        public void SetOptOut(in bool optedOut)
        {
            OptedOut = optedOut;
        }
    }
}
