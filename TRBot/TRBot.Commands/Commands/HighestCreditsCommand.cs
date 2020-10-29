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
using TRBot.Data;
using TRBot.Permissions;

namespace TRBot.Commands
{
    /// <summary>
    /// Views the highest credits count among users.
    /// </summary>
    public sealed class HighestCreditsCommand : BaseCommand
    {
        private const int MAX_TOP_USERS = 10;
        private string UsageMessage = $"Usage: \"# of top users (int - up to {MAX_TOP_USERS}, optional)\"";

        public HighestCreditsCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count > 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            int numTop = 1;

            using BotDBContext context = DatabaseManager.OpenContext();

            if (context.Users.Count() == 0)
            {
                QueueMessage("There are no users in the database!");
                return;
            }

            if (arguments.Count > 0)
            {
                string numTopStr = arguments[0];
                if (int.TryParse(numTopStr, out numTop) == false)
                {
                    QueueMessage(UsageMessage);
                    return;
                }

                if (numTop < 1 || numTop > MAX_TOP_USERS)
                {
                    QueueMessage($"The number to display is less than 1 or greater than {MAX_TOP_USERS}!");
                    return;
                }
            }

            IOrderedQueryable<User> orderedCredits = context.Users.OrderByDescending(u => u.Stats.Credits);
    
            StringBuilder strBuilder = new StringBuilder(numTop * 8);

            int curCount = 0;
            foreach (User user in orderedCredits)
            {
                strBuilder.Append(curCount + 1).Append('.').Append(' ');

                strBuilder.Append(user.Name).Append(' ').Append('(').Append(user.Stats.Credits).Append(')');

                curCount++;
                if (curCount >= numTop)
                {
                    break;
                }

                strBuilder.Append(' ');
            }

            int botCharLimit = (int)DataHelper.GetSettingIntNoOpen(SettingsConstants.BOT_MSG_CHAR_LIMIT, context, 500L);

            QueueMessageSplit(strBuilder.ToString(), botCharLimit, ") ");
        }
    }
}
