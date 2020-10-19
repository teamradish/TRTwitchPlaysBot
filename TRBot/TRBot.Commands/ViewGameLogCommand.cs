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
using TRBot.Misc;
using TRBot.Utilities;
using TRBot.Consoles;
using TRBot.ParserData;
using TRBot.Parsing;
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// Views a game log.
    /// </summary>
    public sealed class ViewGameLogCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"recent log number (optional)\"";

        public ViewGameLogCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            int logNum = 0;

            using BotDBContext context = DatabaseManager.OpenContext();

            //Order by ascending for most recent
            List<GameLog> gameLogs = context.GameLogs.OrderBy(log => log.LogDateTime).ToList();

            //If no log number was specified, use the most recent one
            if (arguments.Count == 0)
            {
                logNum = gameLogs.Count - 1;
            }
            else if (arguments.Count == 1)
            {
                string num = arguments[0];
                if (int.TryParse(num, out logNum) == false)
                {
                    QueueMessage("Invalid log number!");
                    return;
                }

                //Go from back to front (Ex. "!viewlog 1" shows the most recent log)
                logNum = gameLogs.Count - logNum;
            }
            else
            {
                QueueMessage(UsageMessage);
                return;
            }

            if (logNum < 0 || logNum >= gameLogs.Count)
            {
                QueueMessage($"No log found!");
                return;
            }

            GameLog gameLog = gameLogs[logNum];

            User user = DataHelper.GetUserNoOpen(gameLog.User, context);

            //Don't display username if they opted out
            if (user != null && user.Stats.OptedOut == 0)
            {
                QueueMessage($"{gameLog.LogDateTime} (UTC) --> {gameLog.User} : {gameLog.LogMessage}");
            }
            else
            {
                QueueMessage($"{gameLog.LogDateTime} (UTC) --> {gameLog.LogMessage}");
            }
        }
    }
}
