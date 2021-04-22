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
using System.Threading;
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
    /// Views a given number of game logs in reverse chronological order.
    /// </summary>
    public sealed class ViewMultipleGameLogsCommand : ViewGameLogCommand
    {
        /// <summary>
        /// The number of milliseconds to wait between printing each log.
        /// </summary>
        private const int MILLISECONDS_BETWEEN_LOGS = 3000;

        private const string CANCEL_ARG = "cancel";

        private string UsageMessage = "Usage: \"number of game logs to view (int)\" or \"cancel\"";
        
        private bool CurrentlyPrintingLogs = false;
        private bool ShouldCancelPrinting = false;

        public ViewMultipleGameLogsCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count != 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string logCountStr = arguments[0].ToLowerInvariant();

            //Set it to cancel
            if (logCountStr == CANCEL_ARG)
            {
                if (CurrentlyPrintingLogs == true)
                {   
                    ShouldCancelPrinting = true;
                    QueueMessage("Cancelled printing game logs!");
                }
                else
                {
                    QueueMessage("This command isn't currently printing game logs.");
                }
                return;
            }
            else if (CurrentlyPrintingLogs == true)
            {
                QueueMessage("This command is already printing out game logs. Be patient and wait for it to finish :)");
                return;
            }

            //Validate the number
            if (int.TryParse(logCountStr, out int printLogCount) == false)
            {
                QueueMessage("This is not a valid number of game logs to view!");
                return;
            }
            
            //No logs to view
            if (printLogCount <= 0)
            {
                QueueMessage("You have to view at least one game log!");
                return;
            }

            //Return if this value is greater than the number of logs
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                int gameLogCount = context.GameLogs.Count();
                if (printLogCount > gameLogCount)
                {
                    QueueMessage($"Argument is greater than the current game log count of {gameLogCount}!");
                    return;
                }
            }

            //Increase the log count by 1 because we're starting from 1 - the log at 0 doesn't exist
            PrintLogs(args, printLogCount + 1);
        }

        private async void PrintLogs(EvtChatCommandArgs args, int printLogCount)
        {
            CurrentlyPrintingLogs = true;
            
            //Simply invoke the base command with different arguments
            for (int i = 1; i < printLogCount; i++)
            {
                EvtChatCommandArgs newArgs = new EvtChatCommandArgs();
                
                string numStr = i.ToString();
                
                List<string> argList = new List<string>(1) { numStr };

                newArgs.Command = new EvtChatCommandData(argList, numStr, args.Command.ChatMessage,
                    args.Command.CommandIdentifier, args.Command.CommandText);
                
                base.ExecuteCommand(newArgs);

                //Wait for the next argument
                await Task.Delay(MILLISECONDS_BETWEEN_LOGS);

                //Break early if there are no more logs
                //This is possible only if logs were removed while printing
                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    if (i >= context.GameLogs.Count())
                    {
                        ShouldCancelPrinting = true;
                    }
                }

                //Break early if we should cancel printing logs
                if (ShouldCancelPrinting == true)
                {
                    break;
                }
            }

            CurrentlyPrintingLogs = false;
            ShouldCancelPrinting = false;
        }
    }
}
