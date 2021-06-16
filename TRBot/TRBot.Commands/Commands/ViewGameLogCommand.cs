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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRBot.Connection;
using TRBot.Misc;
using TRBot.Utilities;
using TRBot.Consoles;
using TRBot.Parsing;
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// Views a game log.
    /// </summary>
    public class ViewGameLogCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"recent game log number (optional)\"";

        public ViewGameLogCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            int logNum = 0;

            List<GameLog> gameLogs = null;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Order by ascending for most recent
                gameLogs = context.GameLogs.OrderBy(log => log.LogDateTime).ToList();
            }

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
                    QueueMessage("Invalid game log number!");
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
                QueueMessage($"No game log found at number {logNum}!");
                return;
            }

            GameLog gameLog = gameLogs[logNum];

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User logUser = DataHelper.GetUserNoOpen(gameLog.User, context);

                PrintLog(gameLog, (logUser == null) ? false : logUser.IsOptedOut);
            }
        }

        protected void PrintLog(GameLog gameLog, in bool optedOut)
        {
            //Don't display username if they opted out or the name isn't valid
            //The latter is possibly only through the WebSocket client service
            if (optedOut == false && string.IsNullOrEmpty(gameLog.User) == false)
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
