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
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Clears all game logs.
    /// </summary>
    public sealed class ClearGameLogsCommand : BaseCommand
    {
        private const string ConfirmationArg = "yes";

        public ClearGameLogsCommand()
        {

        }

        public override void Initialize(CommandHandler commandHandler)
        {
            AccessLevel = (int)AccessLevels.Levels.Admin;
        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count != 1)
            {
                BotProgram.QueueMessage($"Enter \"{ConfirmationArg}\" as an argument to confirm clearing all game logs.");
                return;
            }

            string arg = args[0];

            if (arg != ConfirmationArg)
            {
                BotProgram.QueueMessage($"Enter \"{ConfirmationArg}\" as an argument to confirm clearing all game logs.");
                return;
            }

            //Clear all logs
            BotProgram.BotData.Logs.Clear();
            BotProgram.SaveBotData();

            BotProgram.QueueMessage("Successfully cleared all game logs!");
        }
    }
}
