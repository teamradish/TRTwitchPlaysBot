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
using TRBot.Connection;
using TRBot.Data;
using TRBot.Permissions;

namespace TRBot.Commands
{
    /// <summary>
    /// Displays a user's most recent inputs.
    /// </summary>
    public sealed class ListUserRecentInputsCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"username (optional)\" \"recent input number (int)\"";

        public ListUserRecentInputsCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count < 1 || arguments.Count > 2)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string inputUsername = (arguments.Count == 2) ? arguments[0].ToLowerInvariant() : args.Command.ChatMessage.Username.ToLowerInvariant();

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User inputUser = DataHelper.GetUserNoOpen(inputUsername, context);

                if (inputUser == null)
                {
                    QueueMessage($"User does not exist in database!");
                    return;
                }

                if (inputUser.IsOptedOut == true)
                {
                    QueueMessage($"{inputUsername} is opted out of bot stats, so you can't see their recent inputs.");
                    return;
                }

                if (inputUser.RecentInputs.Count == 0)
                {
                    QueueMessage($"{inputUsername} doesn't have any recent inputs!");
                    return;
                }
            }

            //Get the recent input number
            string num = (arguments.Count == 1) ? arguments[0] : arguments[1];
            
            if (int.TryParse(num, out int recentInputNum) == false)
            {
                QueueMessage("Invalid recent input number!");
                return;
            }

            string recentInputSeq = string.Empty;
            int finalRecentInputNum = recentInputNum;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User inputUser = DataHelper.GetUserNoOpen(inputUsername, context);

                //Go from back to front (Ex. 1 shows the most recent input)
                finalRecentInputNum = inputUser.RecentInputs.Count - recentInputNum;

                if (finalRecentInputNum < 0 || finalRecentInputNum >= inputUser.RecentInputs.Count)
                {
                    QueueMessage($"No recent input sequence found!");
                    return;
                }

                recentInputSeq = inputUser.RecentInputs[finalRecentInputNum].InputSequence;
            }

            QueueMessage($"{inputUsername}'s #{recentInputNum} most recent input: {recentInputSeq}");
        }
    }
}
