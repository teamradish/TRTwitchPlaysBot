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
using System.IO;
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Gets the channel's info message.
    /// </summary>
    public sealed class InfoCommand : BaseCommand
    {
        private const string EmptyMessage = "No info message set.";

        public override void Initialize(CommandHandler commandHandler)
        {
            base.Initialize(commandHandler);

            AccessLevel = (int)AccessLevels.Levels.Moderator;
        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            if (string.IsNullOrEmpty(BotProgram.BotData.InfoMessage) == true)
            {
                BotProgram.QueueMessage(EmptyMessage);
                return;
            }

            BotProgram.QueueMessage(BotProgram.BotData.InfoMessage);
        }
    }
}
