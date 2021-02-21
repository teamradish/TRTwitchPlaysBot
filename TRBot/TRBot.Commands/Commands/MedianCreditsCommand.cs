/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
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
using TRBot.Data;
using TRBot.Permissions;
using TRBot.Utilities;

namespace TRBot.Commands
{
    /// <summary>
    /// Views the median credits count among users.
    /// </summary>
    public sealed class MedianCreditsCommand : BaseCommand
    {
        public MedianCreditsCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            string creditsName = DataHelper.GetCreditsName();

            long median = 0;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                if (context.Users.Count() == 0)
                {
                    QueueMessage("There are no users in the database!");
                    return;
                }

                List<User> orderedCredits = context.Users.OrderByDescending(u => u.Stats.Credits).ToList();

                int medianIndex = orderedCredits.Count / 2;

                if (medianIndex >= orderedCredits.Count)
                {
                    QueueMessage("Sorry, there's not enough data available in the database!");
                    return;
                }

                median = orderedCredits[medianIndex].Stats.Credits;
            }

            QueueMessage($"The median number of {creditsName.Pluralize(false, 0)} in the database is {median}!");
        }
    }
}
