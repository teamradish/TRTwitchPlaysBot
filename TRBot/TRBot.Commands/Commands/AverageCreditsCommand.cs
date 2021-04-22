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
using System.Text;
using System.Linq;
using TRBot.Connection;
using TRBot.Data;
using TRBot.Permissions;

namespace TRBot.Commands
{
    /// <summary>
    /// Views the average credit count among users.
    /// </summary>
    public sealed class AverageCreditsCommand : BaseCommand
    {
        public AverageCreditsCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            long averageCredits = 0L;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                if (context.Users.Count() == 0)
                {
                    QueueMessage("There are no users in the database!");
                    return;
                }

                averageCredits = (long)context.Users.Average(u => u.Stats.Credits);
            }

            QueueMessage($"The average number of credits is {averageCredits}!");
        }
    }
}
