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
    /// Adds a log for the current game.
    /// </summary>
    public sealed class LogCommand : BaseCommand
    {
        public LogCommand()
        {
            AccessLevel = (int)AccessLevels.Levels.Whitelisted;
        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            string logMessage = e.Command.ArgumentsAsString;

            if (string.IsNullOrEmpty(logMessage) == true)
            {
                BotProgram.QueueMessage("Please enter a message for the log.");
                return;
            }

            DateTime curTime = DateTime.UtcNow;

            //Add a new log
            GameLog newLog = new GameLog();
            newLog.LogMessage = logMessage;
            newLog.User = e.Command.ChatMessage.Username;

            string date = curTime.ToShortDateString();
            string time = curTime.ToLongTimeString();
            newLog.DateTimeString = $"{date} at {time}";

            BotProgram.BotData.Logs.Add(newLog);
            BotProgram.SaveBotData();

            BotProgram.QueueMessage("Successfully logged message!");
        }
    }
}
