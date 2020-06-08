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
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Views a log that has been created.
    /// </summary>
    public sealed class ViewLogCommand : BaseCommand
    {
        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            int logNum = 0;

            //If no log number was specified, use the most recent one
            if (args.Count == 0)
            {
                logNum = BotProgram.BotData.Logs.Count - 1;
            }
            else if (args.Count == 1)
            {
                string num = args[0];
                if (int.TryParse(num, out logNum) == false)
                {
                    BotProgram.MsgHandler.QueueMessage("Invalid log number!");
                    return;
                }

                //Go from back to front (Ex. "!viewlog 1" shows the most recent log)
                logNum = BotProgram.BotData.Logs.Count - logNum;
            }
            else
            {
                BotProgram.MsgHandler.QueueMessage($"Usage: \"recent log number (optional)\"");
                return;
            }

            if (logNum < 0 || logNum >= BotProgram.BotData.Logs.Count)
            {
                BotProgram.MsgHandler.QueueMessage($"No log found!");
                return;
            }

            GameLog log = BotProgram.BotData.Logs[logNum];

            if (string.IsNullOrEmpty(log.User) == false)
            {
                BotProgram.MsgHandler.QueueMessage($"{log.DateTimeString} (UTC) --> {log.User} : {log.LogMessage}");
            }
            else
            {
                BotProgram.MsgHandler.QueueMessage($"{log.DateTimeString} (UTC) --> {log.LogMessage}");
            }
        }
    }
}
