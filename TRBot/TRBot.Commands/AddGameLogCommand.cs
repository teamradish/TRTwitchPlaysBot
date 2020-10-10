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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRBot.Connection;
using TRBot.Common;
using TRBot.Utilities;
using TRBot.Consoles;
using TRBot.ParserData;
using TRBot.Parsing;
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// Adds a game log.
    /// </summary>
    public sealed class AddGameLogCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"log message (string)\"";

        public AddGameLogCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            string logMessage = args.Command.ArgumentsAsString;

            if (string.IsNullOrEmpty(logMessage) == true)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string username = args.Command.ChatMessage.Username.ToLowerInvariant();

            //Add a new log
            GameLog newLog = new GameLog();
            newLog.LogMessage = logMessage;
            newLog.User = username;
            newLog.LogDateTime = DateTime.UtcNow;

            //Add the game log to the database
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                context.GameLogs.Add(newLog);
                context.SaveChanges();
            }
            
            QueueMessage("Successfully logged message!");
        }
    }
}
