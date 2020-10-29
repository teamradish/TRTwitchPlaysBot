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
using System.Linq;
using TRBot.Connection;
using TRBot.Permissions;
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// Tells how many game logs exist.
    /// </summary>
    public sealed class NumGameLogsCommand : BaseCommand
    {
        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            int numLogs = 0;
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                numLogs = context.GameLogs.Count();
            }

            //Have slightly different messages depending on the number of game logs available
            if (numLogs == 0)
            {
                QueueMessage("There are no game logs!");
            }
            else if (numLogs == 1)
            {
                QueueMessage("There is 1 game log!");
            }
            else
            {
                QueueMessage($"There are {numLogs} game logs!");
            }
        }
    }
}
